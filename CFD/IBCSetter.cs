using CFDCore;
using Core.Common;
using Core.Domain;

namespace CFD
{
    public interface IBCSetter
    {
        /// <summary>
        /// Sets Dirichlet boundary conditions on the farfield and around the airfoil
        /// </summary>
        /// <param name="farfield"></param>
        /// <param name="calc"></param>
        /// <param name="fluid"></param>
        void DirichletConditions(Farfield farfield, CalcDomain calc, Fluid fluid);

        /// <summary>
        /// Sets Neumann boundary conditions abround the farfield edge and around the airfoil if required.
        /// </summary>
        /// <param name="calc"></param>
        void NeumannConditions(Farfield farfield, CalcDomain calc);
    }
}
