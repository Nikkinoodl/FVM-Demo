using Core.DataCollections;
using System.Numerics;

namespace Core.Domain
{
    public class Node : BaseNode
    {

        /// <summary>
        /// Create a new internal node (not on farfield boundary or airfoil surface)
        /// </summary>
        /// <param name="this_id"></param>
        /// <param name="position"></param>
        public Node(int this_id, Vector2 position)
        {
            Id = this_id;
            R = position;
            Surface = false;
            Boundary = false;
            Repository.Nodelist.Add(this);
        }

        /// <summary>
        /// Create a new node with position specified using x,y coordinates
        /// </summary>
        /// <param name="this_id"></param>
        /// <param name="this_x"></param>
        /// <param name="this_y"></param>
        /// <param name="this_surface"></param>
        /// <param name="this_boundary"></param>
        public Node(int this_id, float this_x, float this_y, bool this_surface, bool this_boundary)
        {
            Id = this_id;
            R = new Vector2(this_x, this_y);
            Surface = this_surface;
            Boundary = this_boundary;
            Repository.Nodelist.Add(this);
        }

        /// <summary>
        /// Create a new node with position specified as vector2
        /// </summary>
        /// <param name="this_id"></param>
        /// <param name="position"></param>
        public Node(int this_id, Vector2 position, bool this_surface, bool this_boundary)
        {
            Id = this_id;
            R = position;
            Surface = this_surface;
            Boundary = this_boundary;
            Repository.Nodelist.Add(this);
        }

    }
}

