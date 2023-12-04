using Core.Common;
using Core.Domain;
using System.Numerics;

namespace Core.Data
{
    public class Dictionaries
    {
        #region "Cell Geometry"

        /// <summary>
        /// The sequence in which triangular cell nodes should be ordered when one side is horizontal
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="position vectors"></param>
        /// <returns></returns>
        public static List<Tuple<bool, (int, int, int)>> OrderedNodes(int[] nodes, Vector2[] pV)
        {
            return new List<Tuple<bool, (int, int, int)>>()
            {
                new(pV[0].Y == pV[1].Y, (nodes[2], nodes[0], nodes[1])),
                new(pV[1].Y == pV[2].Y, (nodes[0], nodes[1], nodes[2])),
                new(pV[2].Y == pV[0].Y, (nodes[1], nodes[2], nodes[0]))
            };
        }

        /// <summary>
        /// A formula for finding the mid point of each quad cell edge
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Dictionary<SideName, Func<Vector2>> SideMidPoints(Vector2[] r)
        {
            return new Dictionary<SideName, Func<Vector2>>
            {
                {SideName.S1, () => (r[0] + r[1]) * 0.5F},
                {SideName.S2, () => (r[1] + r[2]) * 0.5F},
                {SideName.S3, () => (r[2] + r[3]) * 0.5F},
                {SideName.S4, () => (r[3] + r[0]) * 0.5F}
            };
        }

        /// <summary>
        /// A formula for finding the mid point of each triangular cell edge
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Dictionary<SideName, Func<Vector2>> SideMidPointsTriangle(Vector2[] r)
        {
            return new Dictionary<SideName, Func<Vector2>>
            {
                {SideName.S1, () => (r[1] + r[2]) * 0.5F},
                {SideName.S2, () => (r[0] + r[2]) * 0.5F},
                {SideName.S3, () => (r[0] + r[1]) * 0.5F}
            };
        }

        /// <summary>
        /// The vertex properties of each cell type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<CellType, int?[]> NodeMap(Cell t)
        {
            return new Dictionary<CellType, int?[]>
            {
                {CellType.triangle, new int?[] {t.V1, t.V2, t.V3}},
                {CellType.quad, new int?[] {t.V1, t.V2, t.V3, t.V4}},
                {CellType.pent, new int?[] {t.V1, t.V2, t.V3, t.V4, t.V5}},
                {CellType.hex, new int?[] {t.V1, t.V2, t.V3, t.V4, t.V5, t.V6}},
                {CellType.oct, new int?[] {t.V1, t.V2, t.V3, t.V4, t.V5, t.V6, t.V7, t.V8}}
            };
        }

       /// <summary>
        /// The vertices which lie at either end of a cell edge
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<Tuple<CellType, SideName>, Tuple<int?, int?>> CreateNodeAssignment(Cell t)
        {
            return new Dictionary<Tuple<CellType, SideName>, Tuple<int?, int?>>()
            {
                {Tuple.Create(CellType.triangle, SideName.S1), Tuple.Create(t.V2, t.V3)},
                {Tuple.Create(CellType.triangle, SideName.S2), Tuple.Create(t.V1, t.V3)},
                {Tuple.Create(CellType.triangle, SideName.S3), Tuple.Create(t.V1, t.V2)},
                {Tuple.Create(CellType.quad, SideName.S1), Tuple.Create(t.V2, t.V1)},
                {Tuple.Create(CellType.quad, SideName.S2), Tuple.Create(t.V3, t.V2)},
                {Tuple.Create(CellType.quad, SideName.S3), Tuple.Create(t.V4, t.V3)},
                {Tuple.Create(CellType.quad, SideName.S4), Tuple.Create(t.V1, t.V4)},
                {Tuple.Create(CellType.pent, SideName.S1), Tuple.Create(t.V2, t.V1)},
                {Tuple.Create(CellType.pent, SideName.S2), Tuple.Create(t.V3, t.V2)},
                {Tuple.Create(CellType.pent, SideName.S3), Tuple.Create(t.V4, t.V3)},
                {Tuple.Create(CellType.pent, SideName.S4), Tuple.Create(t.V5, t.V4)},
                {Tuple.Create(CellType.pent, SideName.S5), Tuple.Create(t.V1, t.V5)},
                {Tuple.Create(CellType.hex, SideName.S1), Tuple.Create(t.V2, t.V1)},
                {Tuple.Create(CellType.hex, SideName.S2), Tuple.Create(t.V3, t.V2)},
                {Tuple.Create(CellType.hex, SideName.S3), Tuple.Create(t.V4, t.V3)},
                {Tuple.Create(CellType.hex, SideName.S4), Tuple.Create(t.V5, t.V4)},
                {Tuple.Create(CellType.hex, SideName.S5), Tuple.Create(t.V6, t.V5)},
                {Tuple.Create(CellType.hex, SideName.S6), Tuple.Create(t.V1, t.V6)},
                {Tuple.Create(CellType.oct, SideName.S1), Tuple.Create(t.V2, t.V1)},
                {Tuple.Create(CellType.oct, SideName.S2), Tuple.Create(t.V3, t.V2)},
                {Tuple.Create(CellType.oct, SideName.S3), Tuple.Create(t.V4, t.V3)},
                {Tuple.Create(CellType.oct, SideName.S4), Tuple.Create(t.V5, t.V4)},
                {Tuple.Create(CellType.oct, SideName.S5), Tuple.Create(t.V6, t.V5)},
                {Tuple.Create(CellType.oct, SideName.S6), Tuple.Create(t.V7, t.V6)},
                {Tuple.Create(CellType.oct, SideName.S7), Tuple.Create(t.V8, t.V7)},
                {Tuple.Create(CellType.oct, SideName.S8), Tuple.Create(t.V1, t.V8)}
            };
        }

        /// <summary>
        /// The relation between SideName and cellnodetypes can be used to determine 
        /// whether a new node on the edge lies on the boundary or interior of the farfield
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Dictionary<SideName, Tuple<bool, bool>> SideNodeMap(bool[] s)
        {
            return new Dictionary<SideName, Tuple<bool, bool>>()
            {
                {SideName.S1, new Tuple<bool, bool>(s[1], s[2])},
                {SideName.S2, new Tuple<bool, bool>(s[0], s[2])},
                {SideName.S3, new Tuple<bool, bool>(s[1], s[0])}
            };
        }

        #endregion

        #region "Edge Matching"

        /// <summary>
        /// Target vertex of the adjoining cell in Delaunay Triangulation
        /// </summary>
        /// <param name="t_adj"></param>
        /// <returns></returns>
        public static Dictionary<int, Func<int?>> ConfigMap(Cell t_adj)
        {
            return new Dictionary<int, Func<int?>>
            {
                {1, () => t_adj.V2},
                {2, () => t_adj.V3},
                {3, () => t_adj.V1},
                {4, () => t_adj.V3},
                {5, () => t_adj.V1},
                {6, () => t_adj.V2}
            };
        }

        /// <summary>
        /// Identifies the sidetype of the adjoining edge in Delaunay triangulation
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<int, Func<SideType>> ConfigToSideType(Cell t)
        {
            return new Dictionary<int, Func<SideType>>
            {
                {1, () => t.Edge3.SideType},
                {2, () => t.Edge1.SideType},
                {3, () => t.Edge2.SideType},
                {4, () => t.Edge3.SideType},
                {5, () => t.Edge1.SideType},
                {6, () => t.Edge2.SideType}
            };
        }

        /// <summary>
        /// Vertex matching patterns used in Delaunay Triangulation
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Dictionary<int, Func<Cell, bool>> ConfigurationToCondition(int[] n)
        {
            return new Dictionary<int, Func<Cell, bool>>
            {
                {1, t  => t.V1 == n[0] && t.V3 == n[1]},
                {2, t  => t.V2 == n[1] && t.V1 == n[2]},
                {3, t  => t.V2 == n[0] && t.V3 == n[2]},
                {4, t  => t.V2 == n[0] && t.V1 == n[1]},
                {5, t  => t.V3 == n[1] && t.V2 == n[2]},
                {6, t  => t.V3 == n[0] && t.V1 == n[2]}
            };
        }
        #endregion

        #region "Position in Farfield"

        /// <summary>
        /// The conditions which are used to determine if a node lies on one of the farfield edges
        /// </summary>
        /// <param name="farfield"></param>
        /// <returns></returns>
        public static Dictionary<string, Func<Node, bool>> EdgeToCondition(Farfield farfield)
        {
            return new Dictionary<string, Func<Node, bool>>
            {
                {"top", n => n.R.Y == farfield.Height && n.R.X != 0 && n.R.X != farfield.Width},
                {"bottom", n => n.R.Y == 0 && n.R.X != 0 && n.R.X != farfield.Width},
                {"right", n => n.R.X == farfield.Width && n.R.Y != 0 && n.R.Y != farfield.Height},
                {"left", n => n.R.X == 0 && n.R.Y != 0 && n.R.Y != farfield.Height}
            };
        }

        /// <summary>
        /// The property to use for ordering nodes on each farfield edge
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Func<Node, double>> EdgeToOrder()
        {
            return new Dictionary<string, Func<Node, double>>
            {
                {"top", n => n.R.X},
                {"bottom", n => n.R.X},
                {"right", n => n.R.Y},
                {"left", n => n.R.Y}
            };
        }

        /// <summary>
        /// The conditions which determine when a cell edge lies on one of the farfield boundaries
        /// </summary>
        /// <param name="farfield"></param>
        /// <returns></returns>
        public static Dictionary<string, Func<Cell, bool>> EdgeOnBoundary(Farfield farfield)
        {
            return new Dictionary<string, Func<Cell, bool>>
            {
                {"top", t => t.Edge1.R.Y == farfield.Height},
                {"bottom", t => t.Edge1.R.Y == 0},
                {"left", t => t.Edge1.R.X == 0},
                {"right", t => t.Edge1.R.X == farfield.Width},
                {"all", t => true}
            };
        }
        #endregion
    }
}
