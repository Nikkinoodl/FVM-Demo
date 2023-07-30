﻿using CFDCore;
using Core.Common;
using Core.Domain;
using Core.Interfaces;
using System.Diagnostics;
using System.Numerics;

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
                //momentum (parallel foreach loops (U, V treated separately) appear to cause unpredictability in results)
                foreach (Cell p in _data.CalcCells())
                {
                    float convectionU = 0;
                    float diffusionU = 0;
                    float convectionV = 0;
                    float diffusionV = 0;

                    //Loop through the cell edges.
                    foreach (Edge e in p.Edges)
                    {
                        //U
                        //interpolate for face values using weighting (ignore any numerical diffusion perpendicular to face normal)
                        float phiIpU = _data.CellList[e.AdjoiningCell].Vel.X * e.W + p.Vel.X * (1 - e.W);

                        //diffusion calculated as nu * div.grad(phi) boundary geometry is taken into account
                        diffusionU += (_data.CellList[e.AdjoiningCell].Vel.X - p.Vel.X) * e.Lk;

                        //calculate convection terms using Green-Gauss without interpolating velocity as div.(U phi)
                        convectionU += Vector2.Dot(p.Vel * phiIpU, e.N) * e.L;

                        //V
                        //interpolate for face values using weighting (ignore any numerical diffusion perpendicular to face normal)
                        float phiIpV = _data.CellList[e.AdjoiningCell].Vel.Y * e.W + p.Vel.Y * (1 - e.W);

                        //diffusion calculated as nu * div.grad(phi) boundary geometry is taken into account
                        diffusionV += (_data.CellList[e.AdjoiningCell].Vel.Y - p.Vel.Y) * e.Lk;

                        //calculate convection terms using Green-Gauss without interpolating velocity as div.(U phi)
                        convectionV += Vector2.Dot(p.Vel * phiIpV, e.N) * e.L;
                    }

                    //predictor step uStar and Vstar. We divide by area here so it is only done in one place.
                    p.VelStar = new Vector2(p.Vel.X + (diffusionU * fluid.Nu - convectionU) * calc.Dt * p.AreaI, p.Vel.Y + (diffusionV * fluid.Nu - convectionV) * calc.Dt * p.AreaI);

                }

                //Pressure calculations//
                //PPE:  del p = (rho/dt) * divergence_vel_star calculated with central differencing
                Parallel.ForEach(_data.CalcCells(), p =>
                {
                    //RHS of PPE
                    float b = 0;

                    foreach (Edge e in p.Edges)
                    {
                        //Vector2 velStarIP;
                        b += Vector2.Dot(_data.CellList[e.AdjoiningCell].VelStar * e.W, e.N) * e.L;
                    }

                    // [**] division by area here
                    p.B = b * fluid.Rho * dti;

                });

                //optional iteration on P
                for (int n = 0; n < calc.Piter; n++)
                {

                    //all cells (including borders) need P clone
                    Parallel.ForEach(_data.CellList, cell => { cell.PN = cell.P; });

                    //the calc for p is the exact equivalent of the calc for p in a difference equation on a regular grid
                    Parallel.ForEach(_data.CalcCells(), p =>
                    {
                        float pTerm = 0;

                        foreach (Edge e in p.Edges)
                        {
                            //note that the ratio Lk takes cell and boundary geometry into into account
                            pTerm += _data.CellList[e.AdjoiningCell].PN * e.Lk;
                        }

                        // [**] cancels out with multiplication by area here
                        p.P = (pTerm - p.B) / p.Lk;

                    });

                    //reset Neumann boundary conditions
                    _bcSetter.NeumannConditions(farfield, calc);

                }

                //corrector step is only applied to non-border cells
                Parallel.ForEach(_data.CalcCells(), p =>
                {
                    //reconstruct gradP at cell center with new P values
                    Vector2 gradP = Vector2.Zero;

                    foreach (Edge e in p.Edges)
                    {
                        //since we don't need to derive the second derivative, we use face values for P
                        gradP = Vector2.Add(_data.CellList[e.AdjoiningCell].P * e.W * e.L * e.N, gradP);
                    }

                    //do corrector step
                    p.Vel = Vector2.Subtract(p.VelStar, gradP * calc.Dt * p.AreaI * rhoi);

                });
            }

            watch.Stop();
            Utilities.ElapsedTime = watch.Elapsed.ToString();
        }
    }
}