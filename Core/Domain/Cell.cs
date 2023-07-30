using Core.DataCollections;

namespace Core.Domain
{
    public class Cell : BaseCell
    {
        public Cell(){}

        #region Constructor

        /// <summary>
        /// Create cell with 1-3 sides
        /// </summary>
        /// <param name="id"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="n3"></param>
        /// <param name="n4"></param>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <param name="e3"></param>
        /// <param name="e4"></param>
        public Cell(int id, int n1, int n2, int? n3, Edge e1, Edge? e2, Edge? e3)
        {
            Id = id;
            V1 = n1;
            V2 = n2;
            V3 = n3;
            Complete = false;
            Edge1 = e1;
            Edge2 = e2;
            Edge3 = e3;
            Edges = new List<Edge>
            {
                Edge1
            };
            if (Edge2 != null) Edges.Add(Edge2);
            if (Edge3 != null) Edges.Add(Edge3);

            Repository.CellList.Add(this);
        }


        /// <summary>
        /// Create cell with 4 sides
        /// </summary>
        /// <param name="id"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="n3"></param>
        /// <param name="n4"></param>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <param name="e3"></param>
        /// <param name="e4"></param>
        public Cell(int id, int n1, int n2, int n3, int n4, Edge e1, Edge e2, Edge e3, Edge e4)
        {
            Id = id;
            V1 = n1;
            V2 = n2;
            V3 = n3;
            V4 = n4;
            Complete = false;
            Edge1 = e1;
            Edge2 = e2;
            Edge3 = e3;
            Edge4 = e4;
            Edges = new List<Edge>
            {
                Edge1,
                Edge2,
                Edge3,
                Edge4
            };

            Repository.CellList.Add(this);
        }


        #endregion
    }
}

