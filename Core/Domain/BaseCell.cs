using Core.Common;
using System.Numerics;

namespace Core.Domain

{
    public abstract class BaseCell
    {
        /// <summary>
        /// Unique identifier for the cell
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The id of the node at cell vertex 1
        /// </summary>
        public int? V1 { get; set; }

        /// <summary>
        /// The id of the node at cell vertex 2
        /// </summary>
        public int? V2 { get; set; }

        /// <summary>
        /// The id of the node at cell vertex 3
        /// </summary>
        public int? V3 { get; set; }

        /// <summary>
        /// Optional node id for use with cells with n > 3 sides
        /// </summary>
        public int? V4 { get; set; }

        /// <summary>
        /// Optional node id for use with cells with n > 4 sides
        /// </summary>
        public int? V5 { get; set; }

        /// <summary>
        /// Optional node id for use with cells with n > 5 sides
        /// </summary>
        public int? V6 { get; set; }

        /// <summary>
        /// Optional node id for use with cells with n > 6 sides
        /// </summary>
        public int? V7 { get; set; }

        /// <summary>
        /// Optional node id for use with cells with n > 7 sides
        /// </summary>
        public int? V8 { get; set; }

        /// <summary>
        /// Distinguishes cell type and, by implication, the way in which vertex nodes and sides are numbered
        /// </summary>
        public CellType CellType { get; set; }

        //Basic geometry

        /// <summary>
        /// Position vector of cell center
        /// </summary>
        public Vector2 R { get; set; }

        /// <summary>
        /// Inverse of the cell area
        /// </summary>
        public float AreaI { get; set; }

        /// <summary>
        /// Sum of the ratios of cell side lengths to distance from cell center to adjoining cell center
        /// </summary>
        public float Lk { get; set; }  

        //Edges

        /// <summary>
        /// Cell edge (usage depends on naming scheme)
        /// </summary>
        public Edge Edge1 { get; set; }

        /// <summary>
        /// Cell edge (usage depends on naming scheme)
        /// </summary>
        public Edge? Edge2 { get; set; }

        /// <summary>
        /// Cell edge (usage depends on naming scheme)
        /// </summary>
        public Edge? Edge3 { get; set; }

        /// <summary>
        /// Optional cell edge for n > 3 sided cells
        /// </summary>
        public Edge? Edge4 { get; set; }

        /// <summary>
        /// Optional cell edge for n > 4 sided cells
        /// </summary>
        public Edge? Edge5 { get; set; }

        /// <summary>
        /// Optional cell edge for n > 5 sided cells
        /// </summary>
        public Edge? Edge6 { get; set; }

        /// <summary>
        /// Optional cell edge for n > 6 sided cells
        /// </summary>
        public Edge? Edge7 { get; set; }

        /// <summary>
        /// Optional cell edge for n > 7 sided cells
        /// </summary>
        public Edge? Edge8 { get; set; }    

        /// <summary>
        /// Ordered list of cell edges. Added on cell creation.
        /// </summary>
        public List<Edge> Edges { get; set; }


        //CFD calc results

        /// <summary>
        /// Velocity vector
        /// </summary>
        public Vector2 Vel { get; set; }

        /// <summary>
        /// Predictor step velocity vector
        /// </summary>
        public Vector2 VelStar { get; set; }

        /// <summary>
        /// New cell pressure
        /// </summary>
        public float P { get; set; }

        /// <summary>
        /// Old cell pressure
        /// </summary>
        public float PN { get; set; }

        /// <summary>
        /// RHS of Poisson pressure equation
        /// </summary>
        public float B { get; set; }

        /// <summary>
        /// A spare property used for experimentation and debugging
        /// </summary>
        public float Test { get; set; }  

        /// <summary>
        /// Flag to aid processing
        /// </summary>
        public bool Complete { get; set; }

        /// <summary>
        /// Identifies if this cell is a zero-height border cell used to set boundary conditions
        /// </summary>
        public bool BorderCell { get; set; }

        /// <summary>
        /// Identifies the type of border cell, so that it is easier to set boundary conditions by type. A 
        /// placeholder in the event that we must distinguish between boundary and surface types
        /// </summary>
        public BorderType? BorderCellType { get; set; }

    }
}

