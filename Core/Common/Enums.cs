namespace Core.Common
{
    /// <summary>
    /// Identifies which vertex of a grid element lies opposite. Use S4 only for square meshes
    /// </summary>
    public enum SideName
    {
        S1,
        S2,
        S3,
        S4
    }

    /// <summary>
    /// Identifies special characteristics of the side that might required special handling, e.g. boundary conditions
    /// </summary>
    public enum SideType
    {
        /// <summary>
        /// An internal edge that does not form part of the farfield boundary or airfoil surface
        /// </summary>
        none,

        /// <summary>
        /// An edge that forms part of the farfield boundary
        /// </summary>
        boundary,

        /// <summary>
        /// An edge that forms part of the airfoil surface
        /// </summary>
        surface,
        
        /// <summary>
        /// An inlet edge (deprecated)
        /// </summary>
        inlet,

        /// <summary>
        /// An outlet edge (deprecated)
        /// </summary>
        outlet,

        /// <summary>
        /// A edge reserved for use in zero-height border cells used for setting boundary conditions.
        /// </summary>
        border
    };

    /// <summary>
    /// Identifies the types of scenarios that can be modelled with this software
    /// </summary>
    public enum CalcType
    { 
        LidCavity,
        WindTunnel
    };

    public enum GridType
    {
        Triangles,
        Rectangles
    }
}
