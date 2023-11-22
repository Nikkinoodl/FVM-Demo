using CFDCore;
using Core.Common;
using Core.Domain;
using Core.Interfaces;
using System.Numerics;

namespace CFD
{
    public class BCSetter : IBCSetter
    {
        #region "Fields"
        private readonly IDataAccessService _data;
        #endregion

        #region "Constructor"
        public BCSetter(IDataAccessService data)
        {
            _data = data;
        }
        #endregion

        #region "Public Methods"

        /// <summary>
        /// Sets Dirichlet boundary conditions on the farfield and around the airfoil using special zero-height
        /// border cells
        /// </summary>
        /// <param name="farfield"></param>
        /// <param name="calc"></param>
        /// <param name="fluid"></param>
        public void DirichletConditions(Farfield farfield, CalcDomain calc, Fluid fluid)
        {

            //set the lid conditions (all cell variables have already been zeroed)
            List<Cell> lidCells = _data.GetElementsByBoundary("top", farfield);

            foreach (Cell t in lidCells)
            {
                t.Vel = new Vector2(fluid.InletU, 0);
                t.P = fluid.InletP;
            }
        }

        /// <summary>
        /// Sets Neumann boundary conditions around the farfield edge and airfoil, if present.
        /// </summary>
        /// <param name="calc"></param>
        public void NeumannConditions(Farfield farfield, CalcDomain calc)
        {

            //setting dp/dx and dp/dy to zero on the top, left, right and bottom edges means
            //setting P at the zero-height border cells to be the same as on their adjoining cell.

            List<Cell> borderCells;

            string[] borders = { "left", "bottom", "right" };

            foreach (string b in borders)
            {
                //select all zero-height border cells on the farfield boundary
                borderCells = _data.GetElementsByBoundary(b, farfield);

                //then match the pressure to the adjoining cell, forcing gradP to be zero
                foreach (Cell c in borderCells)
                {
                    foreach (Edge e in c.Edges)
                    {
                        c.P = _data.CellList[e.AdjoiningCell].P;
                    }
                }
            }
        }

        #endregion
    }
}
