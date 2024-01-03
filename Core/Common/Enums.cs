namespace Core.Common
{
    /// <summary>
    /// Distinguishes the way in which vertex nodes and sides are numbered. All numbering schemes proceed
    /// clockwise around the cell.
    /// </summary>
    public enum CellType
    {
        none,      //not set
        line,      //border cell with one edge, side 1 lies between n1 and n2
        triangle,  //triangular cell, side n lies opposite vertex n
        quad,      //quadrangular cell, side n lies between vertex n and vertex n+1
        pent,      //pentagonal cell, side n lies between vertex n and vertex n+1
        hex,       //hexagonal cell, side n lies between vertex n and vertex n+1
        oct        //octagonal cell, side n lies between vertex n and vertex n+1
    }

    /// <summary>
    /// Used to distinguish between edges when using the Edges list.
    /// </summary>
    public enum SideName
    {
        S1,
        S2,
        S3,
        S4,
        S5,
        S6,
        S7,
        S8
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
        /// An edge reserved for use in zero-height border cells used for setting boundary conditions.
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

    /// <summary>
    /// Identifies the type of base mesh that is constructed prior to performing tiling operations
    /// </summary>
    public enum GridType
    {
        Triangles,          //triangles, irregular
        Equilateral,        //triangles, equilateral
        Quads               //quadrilles
    }

    /// <summary>
    /// Identifies the types of tiling operations that can be applied to the base cell layouts
    /// </summary>
    public enum Tiling
    {
        None,               //base grid
        Kis,                //join nodes to center of cell
        Ortho,              //join edge centers to center of cell
        Meta,               //apply both kis and ortho
        Trunc               //truncate cell vertices
    }

    /// <summary>
    /// Identifies the type of border cell
    /// </summary>
    public enum BorderType
    {
        Farfield,
        Airfoil
    }
}
