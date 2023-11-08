using Core.DataCollections;
using Core.Common;

namespace Core.Domain
{

    public class ReplacementCell : Cell
    {

        /// <summary>
        /// Deletes a cell in CellList and replaces it with a trianglular cell
        /// </summary>
        public ReplacementCell(int t, int id, int v1, int v2, int v3, SideType s1, SideType s2, SideType s3)
        {

            // t is the index of the cell in celllist
            Cell cell = Repository.CellList[t];

            Id = id;
            V1 = v1;
            V2 = v2;
            V3 = v3;
            CellType = CellType.triangle;
            Edge1 = cell.Edge1;
            Edge2 = cell.Edge2;
            Edge3 = cell.Edge3;
            Edge1.SideType = s1;
            Edge2.SideType = s2;
            Edge3.SideType = s3;

            Edges = new List<Edge>
            {
                Edge1,
                Edge2,
                Edge3
            };

            Repository.CellList.Remove(Repository.CellList[t]);
            Repository.CellList.Insert(t, this);
        }

        /// <summary>
        /// Deletes a quad cell in CellList and replaces it with another
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="s3"></param>
        public ReplacementCell(int t, int id, int v1, int v2, int v3, int v4, SideType s1, SideType s2, SideType s3, SideType s4)
        {

            // t is the index of the cell in celllist
            Cell cell = Repository.CellList[t];

            Id = id;
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
            CellType = CellType.quad;
            Edge1 = cell.Edge1;
            Edge2 = cell.Edge2;
            Edge3 = cell.Edge3;
            Edge4 = cell.Edge4;

            Edge1.SideType = s1;
            Edge2.SideType = s2;
            Edge3.SideType = s3;
            Edge4.SideType = s4;

            Edges = new List<Edge>
            {
                Edge1,
                Edge2,
                Edge3,
                Edge4
            };

            Repository.CellList.Remove(Repository.CellList[t]);
            Repository.CellList.Insert(t, this);
        }

        /// <summary>
        /// Deletes a triangular cell in CellList and replaces it with a quad cell
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <param name="upgrade"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="s3"></param>
        /// <param name="s4"></param>
        public ReplacementCell(int t, int id, int v1, int v2, int v3, int v4, SideType s1, SideType s2, SideType s3, SideType s4, bool upgrade)
        {
            Cell cell = Repository.CellList[t];
            cell.Edges.Clear();

            Id = id;
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
            CellType = CellType.quad;

            Edge1 = new Edge(SideName.S1, s1);
            Edge2 = new Edge(SideName.S2, s2);
            Edge3 = new Edge(SideName.S3, s3);
            Edge4 = new Edge(SideName.S4, s4);

            Edges.Add(Edge1);
            Edges.Add(Edge2);
            Edges.Add(Edge3);
            Edges.Add(Edge4);

            Repository.CellList.Remove(Repository.CellList[t]);
            Repository.CellList.Insert(t, this);

        }

    }
}
