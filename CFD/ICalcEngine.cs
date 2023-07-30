using CFDCore;
using Core.Common;
using Core.Domain;

namespace CFD
{
    public interface ICalcEngine
    {
        void CellCenterEngine(Farfield farfield, CalcDomain calc, Fluid fluid);

    }
}
