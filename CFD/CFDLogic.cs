using Core.Interfaces;
using Core.Domain;
using Core.Common;
using CFD;
using System.Numerics;

namespace CFDCore
{
    /// <summary>
    /// This class contains methods that take the requested solution type and variables from the form and pass them to the
    /// calc engine so that the boundary conditions can be structured properly.
    /// </summary>
    public class CFDLogic
    {

        #region Fields

        private readonly IDataAccessService _data;
        private readonly ICalcEngine _calcEngine;

        #endregion

        #region Constructor
        public CFDLogic(IDataAccessService data, ICalcEngine calcEngine)
        {
            _data = data;
            _calcEngine = calcEngine;
        }
        #endregion

        #region Methods
        /// <summary>
        /// An attempt at FVM flow simulation based on the 2D incompressible Navier-Stokes equations.
        /// </summary>
        /// <param name="farfield"></param>
        public void FlowSimulation(Farfield farfield, CalcDomain calc, Fluid fluid)
        {
            //before starting calc engine, reset all stored parameters so we can reuse the grid if needed
            Parallel.ForEach(_data.CellList, c =>
            {
                c.VelStar = Vector2.Zero;
                c.Vel = Vector2.Zero;
                c.P = 0;
                c.PN = 0;
                c.B = 0;
                c.Test = 0;
            });

            _calcEngine.CellCenterEngine(farfield, calc, fluid);

        }
        #endregion
    }
}
