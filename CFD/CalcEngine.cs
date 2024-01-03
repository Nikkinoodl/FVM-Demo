using CFDCore;
using Core.Common;
using Core.Domain;
using Core.Interfaces;
using System.Diagnostics;
using System.Numerics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace CFD
{
    public class CalcEngine : ICalcEngine
    {
        #region "Fields"
        private readonly IDataAccessService _data;
        private readonly IBCSetter _bcSetter;
        #endregion

        #region "Constructor"
        public CalcEngine(IDataAccessService data, IBCSetter bcSetter)
        {
            _data = data;
            _bcSetter = bcSetter;
        }
        #endregion

        /// <summary>
        /// A center-difference cell-centered FVM calculation engine for use in diffusion dominated regimes
        /// </summary>
        /// <param name="farfield"></param>
        /// <param name="calc"></param>
        /// <param name="fluid"></param>
        public void CellCenterEngine(Farfield farfield, CalcDomain calc, Fluid fluid)
        {
            //pre-inversion of some values to reduce calc time
            float dti = 1 / calc.Dt;
            float rhoi = 1 / fluid.Rho;

            //set initial boundary conditions
            _bcSetter.DirichletConditions(farfield, calc, fluid);

            //timer for the main calc loop
            Stopwatch watch = new();
            watch.Start();

            //main loop timesteps
            for (float t = 0; t < calc.Tmax; t += calc.Dt)
            {

                //momentum calcs in parallel
                Parallel.ForEach(_data.CalcCells(), cell =>
                {
                    float convectionU = 0;
                    float diffusionU = 0;
                    float convectionV = 0;
                    float diffusionV = 0;

                    //Loop through the cell edges.
                    foreach (Edge e in cell.Edges)
                    {

                        //U
                        //interpolate for face values using weighting (ignore any numerical diffusion perpendicular to face normal)
                        float phiIpU = _data.CellList[e.AdjoiningCell].Vel.X * e.W + cell.Vel.X * (1 - e.W);

                        //diffusion calculated as nu * div.grad(phi) boundary geometry is taken into account
                        diffusionU += (_data.CellList[e.AdjoiningCell].Vel.X - cell.Vel.X) * e.Lk;

                        //calculate convection terms using Green-Gauss without interpolating velocity as div.(U phi)
                        convectionU += Vector2.Dot(cell.Vel * phiIpU, e.N) * e.L;

                        //V
                        //interpolate for face values using weighting (ignore any numerical diffusion perpendicular to face normal)
                        float phiIpV = _data.CellList[e.AdjoiningCell].Vel.Y * e.W + cell.Vel.Y * (1 - e.W);

                        //diffusion calculated as nu * div.grad(phi) boundary geometry is taken into account
                        diffusionV += (_data.CellList[e.AdjoiningCell].Vel.Y - cell.Vel.Y) * e.Lk;

                        //calculate convection terms using Green-Gauss without interpolating velocity as div.(U phi)
                        convectionV += Vector2.Dot(cell.Vel * phiIpV, e.N) * e.L;
                    }

                    //predictor step uStar and Vstar. We divide by area here so it is only done in one place.
                    cell.VelStar = new Vector2(cell.Vel.X + (diffusionU * fluid.Nu - convectionU) * calc.Dt * cell.AreaI, cell.Vel.Y + (diffusionV * fluid.Nu - convectionV) * calc.Dt * cell.AreaI);

                });

                //Pressure calculations//
                //PPE:  del p = (rho/dt) * divergence_vel_star calculated with central differencing
                Parallel.ForEach(_data.CalcCells(), cell =>
                {

                    //RHS of PPE
                    float b = 0;

                    foreach (Edge e in cell.Edges)
                    {
                        
                        //Vector2 velStarIP;
                        b += Vector2.Dot(_data.CellList[e.AdjoiningCell].VelStar * e.W, e.N) * e.L;
                    }

                    // division by area here is cancelled out with later multiplication by area,
                    // see [**] below
                    cell.B = b * fluid.Rho * dti;

                });

                //optional iteration on P
                for (int n = 0; n < calc.Piter; n++)
                {

                    //all cells (including borders) need P clone
                    Parallel.ForEach(_data.CellList, cell => { cell.PN = cell.P; });

                    //on a regular grid, the calc for p is the exact equivalent of the calc for p in a difference equation 
                    Parallel.ForEach(_data.CalcCells(), cell =>
                    {
                        float pTerm = 0;

                        foreach (Edge e in cell.Edges)
                        {

                            //note that the ratio Lk takes cell and boundary geometry into into account
                            pTerm += _data.CellList[e.AdjoiningCell].PN * e.Lk;

                        }

                        // [**] earlier division by area is cancelled out with multiplication by area here
                        cell.P = (pTerm - cell.B) / cell.Lk;

                    });

                    //reset Neumann boundary conditions
                    _bcSetter.NeumannConditions(farfield, calc);

                }

                //corrector step is only applied to non-border cells
                Parallel.ForEach(_data.CalcCells(), cell =>
                {
                    //reconstruct gradP at cell center with new P values
                    Vector2 gradP = Vector2.Zero;

                    foreach (Edge e in cell.Edges)
                    {
                        //since we don't need to derive the second derivative, we use face values for P
                        gradP += _data.CellList[e.AdjoiningCell].P * e.W * e.L * e.N;
                    }

                    //do corrector step
                    cell.Vel = cell.VelStar - gradP * calc.Dt * cell.AreaI * rhoi;

                });
            }

            watch.Stop();
            Utilities.ElapsedTime = watch.Elapsed.ToString();
        }
    }
}
