using Core.Common;
using System.Numerics;

namespace Core.Domain
{
    /// <summary>
    /// Cell edge
    /// </summary>
    public class Edge
    {
        #region Properties
        /// <summary>
        /// identifies the edge in relation to the opposing vertex number
        /// </summary>
        public SideName SideName { get; set; }

        /// <summary>
        /// enum that classifies the edge type
        /// </summary>
        public SideType SideType { get; set; }


        //Geometry

        /// <summary>
        /// edge length
        /// </summary>
        public float L { get; set; }

        /// <summary>
        /// edge vector
        /// </summary>
        public Vector2 Lv { get; set; }

        /// <summary>
        /// ratio of edge length to distance from cell center to adjoining cell center
        /// </summary>
        public float Lk { get; set; }
        
        /// <summary>
        /// edge mid point position vector
        /// </summary>
        public Vector2 R { get; set; }

        /// <summary>
        /// vector from cell center to edge mid point
        /// </summary>
        public Vector2 Rp { get; set; }

        /// <summary>
        /// face normal (points out from element)
        /// </summary>
        public Vector2 N { get; set; }

        /// <summary>
        /// weighting to use in determining how much of adjoining cell value contributes to face
        /// value 1 = all, 0 = none
        /// </summary>
        public float W { get; set; }

        //Neighbors

        /// <summary>
        /// the index of the cell which adjoins this face
        /// </summary>
        public int AdjoiningCell { get; set; }

        /// <summary>
        /// the edge name (Enum) of the adjoining face
        /// </summary>
        public SideName AdjoiningEdge { get; set; }

        /// <summary>
        /// vector from cell center to adjoining cell center
        /// </summary>
        public Vector2 Rk { get; set; }


        #endregion

        #region Constructor

        public Edge(SideName sideName, SideType sideType)
        {
            SideName = sideName;
            SideType = sideType;
        }

        #endregion
    }
}