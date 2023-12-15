using Core.Domain;
using Core.Interfaces;
using Core.DataCollections;
using Core.Common;
using System.Numerics;

namespace Core.Data
{
    /// <summary>
    /// Functions that return information about data collections
    /// </summary>
    public class DataAccessService : IDataAccessService
    {
        /// <summary>
        /// A list of all cells
        /// </summary>        
        public List<Cell> CellList { get; set; } = Repository.CellList;

        /// <summary>
        /// A list of all nodes
        /// </summary>
        public List<Node> Nodelist { get; set; } = Repository.Nodelist;

        /// <summary>
        /// Returns a list of all interior nodes
        /// </summary>
        /// <returns></returns>
        public List<Node> InteriorNodes()
        {

            var nodeList = (from node in Nodelist
                            where node.Boundary == false
                            select node).ToList();

            return nodeList;
        }

        /// <summary>
        /// Returns a list of all nodes
        /// </summary>
        /// <returns></returns>
        public List<Node> AllNodes()
        {
            var nodeList = (from node in Nodelist
                            select node).ToList();

            return nodeList;
        }

        /// <summary>
        /// Returns the surface types of the nodes in a triangular cell as an array
        /// </summary>
        /// <param name="n"></param>
        /// <param name="nSides"></param>
        /// <returns></returns>
        public bool[] GetNodeSurfaceAsArray(int[] n)
        {
            bool[] surfaceArray = { NodeV(n[0]).Surface, NodeV(n[1]).Surface, NodeV(n[2]).Surface };

            return surfaceArray;
        }

        /// <summary>
        /// Returns the position vectors of a cell as an array
        /// </summary>
        /// <param name="n"></param>
        /// <param name="nSides"></param>
        /// <returns></returns>
        public Vector2[] GetPositionVectorsAsArray(int[] n)
        {

            var r = new Vector2[n.Length];

            //find position vectors for all nodes in cell
            for (int i = 0; i < n.Length; i++)
            {

                r[i] = NodeV(n[i]).R;

            }

            return r;

        }

        /// <summary>
        /// Returns the maximum cell Id
        /// </summary>
        /// <returns></returns>
        public int MaxCellId()
        {

            var thisId = (from cell in CellList
                          select cell.Id).Max();

            return thisId;
        }
        
        /// <summary>
        /// Checks for an existing node at the given position vector
        /// </summary>
        /// <param name="rP"></param>
        /// <returns></returns>
        public int Exists(Vector2 rP)
        {

            int countNode = (from node in Nodelist
                             where node.R == rP
                             select node.Id).AsParallel().Count();

            return countNode;
        }

        /// <summary>
        /// Checks for an existing node at the given coordinates
        /// </summary>
        /// <param name="xp"></param>
        /// <param name="yp"></param>
        /// <returns></returns>
        public int Exists(double xp, double yp)
        {

            int countNode = (from node in Nodelist
                             where node.R.X == xp && node.R.Y == yp
                             select node.Id).AsParallel().Count();

            return countNode;
        }

        /// <summary>
        /// Returns the id of an existing node
        /// </summary>
        /// <param name="xp"></param>
        /// <param name="yp"></param>
        /// <returns></returns>
        public int FindNode(double xp, double yp)
        {

            var n = (from node in Nodelist
                     where node.R.X == xp && node.R.Y == yp
                     select node.Id).AsParallel().FirstOrDefault();

            return n;
        }

        /// <summary>
        /// Returns the id of an existing node at the given position vector
        /// </summary>
        /// <param name="rP"></param>
        /// <returns></returns>
        public int FindNode(Vector2 rP)
        {

            var n = (from node in Nodelist
                     where node.R == rP
                     select node.Id).AsParallel().FirstOrDefault();

            return n;
        }

        /// <summary>
        /// Returns an ordered list of target nodes that can be used for CFD calcs
        /// </summary>
        /// <returns></returns>
        public List<Node> CalcNodes() 
        {

            var thislist = (from node in Nodelist
                           where node.Boundary == false && node.Surface == false  // nodes on the surface or boundary aren't selected
                           select node).OrderBy(n => n.R.X).ThenBy(n => n.R.Y).ToList();

            return thislist;
        }

        /// <summary>
        /// Returns a specific node
        /// </summary>
        /// <param name="n">Node Id</param>
        /// <returns></returns>
        public Node NodeV(int? n)
        {

            if (n != null)
            {
                var thisNode = (from node in Nodelist
                                where node.Id == n
                                select node).Single();

                return thisNode;
            }
            else
            {
                throw new Exception();
            }

        }

        /// <summary>
        /// Returns a list of unprocessed cells
        /// </summary>
        /// <returns></returns>
        public List<Cell> IncompleteCells()
        {

            var incompleteList = (from cell in CellList
                             where cell.Complete == false
                             select cell).ToList();

            return incompleteList;
        }

        /// <summary>
        /// Returns a list of all cells except zero-height border cells
        /// of the grid.
        /// </summary>
        /// <returns></returns>
        public List<Cell> CalcCells()
        {

            var cellList = CellList
                .Where(t => !t.BorderCell)
                .OrderBy(t => t.R.X).ThenBy(t => t.R.Y)
                .AsParallel().ToList();

            return cellList;
        }

        /// <summary>
        /// Returns a list of the cells that contains a specific node
        /// </summary>
        /// <param name="thisnode"></param>
        /// <returns></returns>
        public List<Cell> SmoothCell(int thisnode)
        {

            //var thisList = CellList
            //   .Where(cell => cell.V1 == thisnode || cell.V2 == thisnode || cell.V3 == thisnode)
            //   .OrderBy(cell => cell.R.X)
            //   .AsParallel().ToList();

            var cluster = (from t in CellList
                let values = new[] { t.V1, t.V2, t.V3, t.V4, t.V5, t.V6, t.V7, t.V8 }
                where values.Contains(thisnode)
                orderby t.R.X
                select t).AsParallel().ToList();

            return cluster;
        }

        /// <summary>
        /// Counts the number of cells that share a given node
        /// </summary>
        /// <param name="thisnode"></param>
        /// <returns></returns>
        public int CellClusterCount(int thisnode, CellType cellType)
        {

            var total = CellList
                .Count(cell => cell.CellType == cellType && (cell.V1 == thisnode || cell.V2 == thisnode || cell.V3 == thisnode));           

            return total;
        }

        /// <summary>
        /// Returns a list of cells that share a given node
        /// </summary>
        /// <param name="thisnode"></param>
        /// <returns></returns>
        public List<Cell> CellCluster(int thisnode, CellType cellType)
        {

            var cluster = CellList
                        .Where(cell => cell.CellType == cellType && (cell.V1 == thisnode || cell.V2 == thisnode || cell.V3 == thisnode))
                        .ToList();

            return cluster;

        }

        /// <summary>
        /// Returns a list of nodes that are candidates for smoothing. Surface and boundary nodes are excluded
        /// </summary>
        /// <returns></returns>
        public List<Node> SmoothNode()
        {

            var thislist = (from node in Nodelist
                           where node.Boundary == false && node.Surface == false  // nodes on the surface or boundary aren't moved
                           select node).AsParallel().ToList();

            return thislist;
        }

        /// <summary>
        /// Returns a list of cells that have an edge on the farfield boundary, independent of
        /// grid type. Note that zero-height border cells are not included
        /// </summary>
        /// <returns></returns>
        public List<Cell> BoundaryCell()
        {

            var thislist = (from cell in CellList
                           .Where(c => c.Edges.Any(e => e.SideType == SideType.boundary))
                           select cell).ToList();

            return thislist;
        }

        /// <summary>
        /// Returns a list of cells that have an edge on the airfoil surface
        /// note that zero-height border cells are not included
        /// </summary>
        /// <returns></returns>
        public List<Cell> SurfaceCell()
        {

            var thislist = (from cell in CellList
                           .Where(c => c.Edges.Any(e => e.SideType == SideType.surface))
                           select cell).AsParallel().ToList();

            return thislist;
        }


        /// <summary>
        /// Returns a list of all nodes that are either on the boundary of the farfield or on the surface of the airfoil
        /// </summary>
        /// <param name="farfield"></param>
        /// <returns></returns>
        public List<Node> BoundaryNode(Farfield farfield)
        {

            var boundarynodes = (from node in Nodelist
                                where node.R.X == 0 || node.R.X == farfield.Width || node.R.Y == 0 || node.R.Y == farfield.Height
                                select node).ToList();

            return boundarynodes;
        }

        /// <summary>
        /// Finds the cell and edge which are adjacent to the given node pair
        /// </summary>
        /// <param name="nA"></param>
        /// <param name="nB"></param>
        /// <returns>Id, SideName</returns>
        public (int?, SideName?) AdjacentCellEdge((int nA, int nB, Edge targetEdge) nodePair, int targetCell)
        {

            //we may need to return nulls for boundary/surface cells depending on when this function is called
            (int?, SideName?) result;

            Cell? adjacentCell = (from Cell t in CellList
                           let values = new[] { t.V1, t.V2, t.V3, t.V4, t.V5, t.V6, t.V7, t.V8 }
                           where values.Contains(nodePair.nA) && values.Contains(nodePair.nB) && t.Id != targetCell
                           select t).FirstOrDefault();

            if (adjacentCell != null)
            {

                //find the matching side by comparing the mid-point position vector of the nodepair edge with the
                //mid-point position vector of each side in adjacentCell
                SideName? adjacentSide = (from Edge e in adjacentCell.Edges
                            where e.R == nodePair.targetEdge.R
                            select e.SideName).FirstOrDefault();

                result = (CellList.IndexOf(adjacentCell), adjacentSide);

            }
            else
            {

                result = (null, null);

            }

            return result;
        }

        /// <summary>
        /// Returns a list of cells with given nodes used in Delaunay triangulation
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="n3"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Cell> AdjacentCells(int configuration, int[] n)
        {

            var configurationToCondition = Dictionaries.ConfigurationToCondition(n);

            if (!configurationToCondition.TryGetValue(configuration, out Func<Cell, bool>? value))
            {
                throw new Exception();
            }

            var basequery = IncompleteCells();
            var filterquery = basequery.Where(value).AsParallel().ToList();

            return filterquery;

        }

        /// <summary>
        /// Returns an ordered list of nodes on the edge of the farfield boundary. Note that this
        /// excludes the corner nodes.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="farfield"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Node> EdgeBoundary(string edge, Farfield farfield)
        {

            var edgeToCondition = Dictionaries.EdgeToCondition(farfield);
            var edgeToOrder = Dictionaries.EdgeToOrder();

            if (!edgeToCondition.TryGetValue(edge, out Func<Node, bool>? conditionValue) || !edgeToOrder.TryGetValue(edge, out Func<Node, double>? orderValue))
            {
                throw new Exception();
            }

            var basequery = Nodelist;
            var filterquery = basequery.Where(conditionValue).AsParallel();
            var orderquery = filterquery.OrderBy(orderValue).AsParallel().ToList();

            return orderquery;

        }

        /// <summary>
        /// Returns a list of the zero-height border cells which lie on a specified farfield boundary
        /// eg: "top", "bottom", "left", "right" or "all"
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="farfield"></param>
        /// <returns></returns>
        public List<Cell> GetElementsByBoundary(string edge, Farfield farfield)
        {

            var edgeToCondition = Dictionaries.EdgeOnBoundary(farfield);

            if (!edgeToCondition.TryGetValue(edge, out Func<Cell, bool>? value))
            {
                throw new Exception();
            }

            var basequery = (from cell in CellList
                             where cell.BorderCell == true && cell.BorderCellType == BorderType.Farfield
                             select cell).AsParallel();

            var filterquery = basequery.Where(value).AsParallel().ToList();

            return filterquery;

        }

        /// <summary>
        /// Returns a list of the zero-height border cells which lie on the airfoil surface
        /// </summary>
        /// <returns></returns>
        public List<Cell> GetAirfoilSurfaceElements()
        {

            var basequery = (from cell in CellList
                            where cell.BorderCell == true && cell.BorderCellType == BorderType.Airfoil
                            select cell).AsParallel().ToList();

            return basequery;
        }

        /// <summary>
        /// Finds the longest side of a cell
        /// </summary>
        /// <param name="t"></param>
        /// <returns>Edge</returns>
        public Edge FindLongestSide(int t)
        {

            Edge e = (from c in CellList[t].Edges
                      orderby c.L descending
                      select c).AsParallel().First();

            return e;
        }

        /// <summary>
        /// Finds the longest side of a cell
        /// </summary>
        /// <param name="t"></param>
        /// <returns>Edge</returns>
        public Edge FindLongestSide(Cell t)
        {

            Edge e = (from c in t.Edges
                     orderby c.L descending
                     select c).AsParallel().First();

            return e;
        }

        /// <summary>
        /// Finds the third edge in a triangle
        /// </summary>
        /// <param name="t"></param>
        /// <param name="vertSide"></param>
        /// <param name="longSide"></param>
        /// <returns></returns>
        public Edge[]? FindHorizontalEdge(int t, Edge vertSide, Edge longSide)
        {
            var h = (from l in CellList[t].Edges
                     where l.SideName != vertSide.SideName && l.SideName != longSide.SideName
                     select l).ToArray();

            //returning an array forces imediate execution
            return h;
        }

    }
}