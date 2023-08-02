using Core.Common;

namespace Core.Domain
{
    public class CalcDomain
    {
        /// <summary>
        /// The overall duration of the timeloop in seconds
        /// </summary>
        public float Tmax { get; set; } = 5.0F;

        /// <summary>
        /// The incremental amount of time corresponding to each step of the timeloop in seconds
        /// </summary>
        public float Dt { get; set; } = 0.001F;

        /// <summary>
        /// The scenario being modelled (not used)
        /// </summary>
        public CalcType CalcType { get; set; } = CalcType.LidCavity;

        /// <summary>
        /// Number of iterations of the pressure calculations (not used)
        /// </summary>
        public int Piter { get; set; }

        /// <summary>
        /// Under relaxation factor (not used)
        /// </summary>
        public float Relax { get; set; }

    }
}
