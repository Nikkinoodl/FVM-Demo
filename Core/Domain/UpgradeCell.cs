using Core.DataCollections;
using Core.Common;

namespace Core.Domain
{

    public class UpgradeCell : Cell
    {

        /// <summary>
        /// Deletes a triangular cell in CellList and replaces it with a quad
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="s3"></param>
        /// <param name="s4"></param>
        public UpgradeCell(int t, int id, int v1, int v2, int v3, int v4, SideType s1, SideType s2, SideType s3, SideType s4)
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
        /// Deletes a triangular cell in CellList and replaces it with a pentagon
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="v5"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="s3"></param>
        /// <param name="s4"></param>
        /// <param name="s5"></param>
        public UpgradeCell(int t, int id, int v1, int v2, int v3, int v4, int v5, SideType s1, SideType s2, SideType s3, SideType s4, SideType s5)
        {
            Cell cell = Repository.CellList[t];
            cell.Edges.Clear();

            Id = id;
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
            V5 = v5;

            CellType = CellType.pent;

            Edge1 = new Edge(SideName.S1, s1);
            Edge2 = new Edge(SideName.S2, s2);
            Edge3 = new Edge(SideName.S3, s3);
            Edge4 = new Edge(SideName.S4, s4);
            Edge5 = new Edge(SideName.S5, s5);

            Edges = new List<Edge>
            {
                Edge1,
                Edge2,
                Edge3,
                Edge4,
                Edge5
            };

            Repository.CellList.Remove(Repository.CellList[t]);
            Repository.CellList.Insert(t, this);

        }



        /// <summary>
        /// Deletes a triangular cell in CellList and replaces it with a hexagon
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="v5"></param>
        /// <param name="v6"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="s3"></param>
        /// <param name="s4"></param>
        /// <param name="s5"></param>
        /// <param name="s6"></param>
        public UpgradeCell(int t, int id, int v1, int v2, int v3, int v4, int v5, int v6, SideType s1, SideType s2, SideType s3, SideType s4, SideType s5, SideType s6)
        {
            Cell cell = Repository.CellList[t];
            cell.Edges.Clear();

            Id = id;
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
            V5 = v5;
            V6 = v6;

            CellType = CellType.hex;

            Edge1 = new Edge(SideName.S1, s1);
            Edge2 = new Edge(SideName.S2, s2);
            Edge3 = new Edge(SideName.S3, s3);
            Edge4 = new Edge(SideName.S4, s4);
            Edge5 = new Edge(SideName.S5, s5);
            Edge6 = new Edge(SideName.S6, s6);

            Edges = new List<Edge>
            {
                Edge1,
                Edge2,
                Edge3,
                Edge4,
                Edge5,
                Edge6
            };

            Repository.CellList.Remove(Repository.CellList[t]);
            Repository.CellList.Insert(t, this);

        }

    }
}
