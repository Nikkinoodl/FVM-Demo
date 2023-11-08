using Core.DataCollections;
using Core.Common;

namespace Core.Domain
{
    public class Cell : BaseCell
    {

        public Cell() { }

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
            CellType = CellType.triangle;
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
            CellType = CellType.quad;
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

        /// <summary>
        /// Create cell with 5 sides
        /// </summary>
        /// <param name="id"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="n3"></param>
        /// <param name="n4"></param>
        /// <param name="n5"></param>
        /// <param name="n6"></param>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <param name="e3"></param>
        /// <param name="e4"></param>
        /// <param name="e5"></param>
        /// <param name="e6"></param>
        public Cell(int id, int n1, int n2, int n3, int n4, int n5,
                    Edge e1, Edge e2, Edge e3, Edge e4, Edge e5)
        {
            Id = id;
            V1 = n1;
            V2 = n2;
            V3 = n3;
            V4 = n4;
            V5 = n5;
            CellType = CellType.pent;
            Complete = false;
            Edge1 = e1;
            Edge2 = e2;
            Edge3 = e3;
            Edge4 = e4;
            Edge5 = e5;
            Edges = new List<Edge>
            {
                Edge1,
                Edge2,
                Edge3,
                Edge4,
                Edge5
            };

            Repository.CellList.Add(this);
        }

        /// <summary>
        /// Create cell with 6 sides
        /// </summary>
        /// <param name="id"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="n3"></param>
        /// <param name="n4"></param>
        /// <param name="n5"></param>
        /// <param name="n6"></param>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <param name="e3"></param>
        /// <param name="e4"></param>
        /// <param name="e5"></param>
        /// <param name="e6"></param>
        public Cell(int id, int n1, int n2, int n3, int n4, int n5, int n6,
                    Edge e1, Edge e2, Edge e3, Edge e4, Edge e5, Edge e6)
        {
            Id = id;
            V1 = n1;
            V2 = n2;
            V3 = n3;
            V4 = n4;
            V5 = n5;
            V6 = n6;
            CellType = CellType.hex;
            Complete = false;
            Edge1 = e1;
            Edge2 = e2;
            Edge3 = e3;
            Edge4 = e4;
            Edge5 = e5;
            Edge6 = e6;
            Edges = new List<Edge>
            {
                Edge1,
                Edge2,
                Edge3,
                Edge4,
                Edge5,
                Edge6
            };

            Repository.CellList.Add(this);
        }

        /// <summary>
        /// Create cell with 8 sides
        /// </summary>
        /// <param name="id"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="n3"></param>
        /// <param name="n4"></param>
        /// <param name="n5"></param>
        /// <param name="n6"></param>
        /// <param name="n7"></param>
        /// <param name="n8"></param>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <param name="e3"></param>
        /// <param name="e4"></param>
        /// <param name="e5"></param>
        /// <param name="e6"></param>
        /// <param name="e7"></param>
        /// <param name="e8"></param>
        public Cell(int id, int n1, int n2, int n3, int n4, int n5, int n6, int n7, int n8,
            Edge e1, Edge e2, Edge e3, Edge e4, Edge e5, Edge e6, Edge e7, Edge e8)
        {
            Id = id;
            V1 = n1;
            V2 = n2;
            V3 = n3;
            V4 = n4;
            V5 = n5;
            V6 = n6;
            V7 = n7;
            V8 = n8;
            CellType = CellType.oct;
            Complete = false;
            Edge1 = e1;
            Edge2 = e2;
            Edge3 = e3;
            Edge4 = e4;
            Edge5 = e5;
            Edge6 = e6;
            Edge7 = e7;
            Edge8 = e8;
            Edges = new List<Edge>
            {
                Edge1,
                Edge2,
                Edge3,
                Edge4,
                Edge5,
                Edge6,
                Edge7,
                Edge8
            };

            Repository.CellList.Add(this);
        }

        #endregion
    }
}

