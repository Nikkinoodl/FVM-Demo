using System.Numerics;

namespace Core.Domain
{
    public abstract class BaseNode
    {
        //Basic mesh

        /// <summary>
        /// A unique id
        /// </summary>
        public int Id { get; set; }

        //Basic geometry

        /// <summary>
        /// Node position vector
        /// </summary>
        public Vector2 R { get; set; }    //node position vector

        //Mesh configuration

        /// <summary>
        /// Indicates if node lies on airfoil surface
        /// </summary>
        public bool Surface { get; set; }

        /// <summary>
        /// Indicates if node lies on farfield boundary
        /// </summary>
        public bool Boundary { get; set; }

        /// <summary>
        /// Indicates if node sits at an airfoil trailing edge
        /// </summary>
        public bool Te_posn { get; set; }

    }
}