using Core.DataCollections;
using System.Numerics;

namespace Core.Domain
{
    public class Node : BaseNode
    {
        public Node()
        {
            Repository.Nodelist.Add(this);
        }

        //public Node(int this_id, float this_x, float this_y)
        //{
        //    Id = this_id;
        //    R = new Vector2(this_x, this_y);
        //    Surface = false;
        //    Boundary = false;
        //    Repository.Nodelist.Add(this);
        //}

        public Node(int this_id, Vector2 position)
        {
            Id = this_id;
            R = position;
            Surface = false;
            Boundary = false;
            Repository.Nodelist.Add(this);
        }

        public Node(int this_id, float this_x, float this_y, bool this_surface, bool this_boundary)
        {
            Id = this_id;
            R = new Vector2(this_x, this_y);
            Surface = this_surface;
            Boundary = this_boundary;
            Repository.Nodelist.Add(this);
        }

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

