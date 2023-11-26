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

            var thisList = CellList
               .Where(cell => cell.V1 == thisnode || cell.V2 == thisnode || cell.V3 == thisnode)
               .OrderBy(cell => cell.R.X)
               .AsParallel().ToList();

            return thisList;
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
        /// Sorts cells in the repository to set a calc order for the mesh, working from (X) left to right
        /// and (Y) top to bottom
        /// </summary>
        public void SortCells()
        {

            Repository.CellList.Sort((t1, t2) => { int ret = t1.R.X.CompareTo(t2.R.X);
                                                        return ret != 0 ? ret : t2.R.Y.CompareTo(t1.R.Y);
                                                 });
        }

        /// <summary>
        /// Finds the adjacent cell and side to a given node pair
        /// </summary>
        /// <param name="nA"></param>
        /// <param name="nB"></param>
        /// <returns>Id, SideName</returns>
        public (int?, SideName?) AdjacentCellEdge((int nA, int nB, Vector2 r, Edge e) nodePair, int this_t)
        {
            //boundary and surface edges may not have a matching element/edge, so we need to be prepared to return nulls
            SideName? sideName;
            (int?, SideName?) result;

            Cell? t_adj = (from Cell t in CellList
                           let values = new[] { t.V1, t.V2, t.V3, t.V4, t.V5, t.V6, t.V7, t.V8 }
                           where values.Contains(nodePair.nA) && values.Contains(nodePair.nB) && t.Id != this_t
                           select t).FirstOrDefault();

            if (t_adj != null)
            {
                //matching sides is easy because we can match on the mid point position vectors
                //of the edges in this cell
                sideName = (from Edge e in t_adj.Edges
                            where e.R == nodePair.r
                            select e.SideName).FirstOrDefault();

                result = (Repository.CellList.IndexOf(t_adj), sideName);

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
        public List<Cell> AdjacentCells(int configuration, CellNodes n)
        {

            var configurationToCondition = new Dictionary<int, Func<Cell, bool>>
            {
                {1, t => t.V1 == n.N1 && t.V3 == n.N2},
                {2, t => t.V2 == n.N2 && t.V1 == n.N3},
                {3, t => t.V2 == n.N1 && t.V3 == n.N3},
                {4, t => t.V2 == n.N1 && t.V1 == n.N2},
                {5, t => t.V3 == n.N2 && t.V2 == n.N3},
                {6, t => t.V3 == n.N1 && t.V1 == n.N3}
            };

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

            var edgeToCondition = new Dictionary<string, Func<Node, bool>>
            {
                {"top", n => n.R.Y == farfield.Height && n.R.X != 0 && n.R.X != farfield.Width},
                {"bottom", n => n.R.Y == 0 && n.R.X != 0 && n.R.X != farfield.Width},
                {"right", n => n.R.X == farfield.Width && n.R.Y != 0 && n.R.Y != farfield.Height},
                {"left", n => n.R.X == 0 && n.R.Y != 0 && n.R.Y != farfield.Height}
            };

            var edgeToOrder = new Dictionary<string, Func<Node, double>>
            {
                {"top", n => n.R.X},
                {"bottom", n => n.R.X},
                {"right", n => n.R.Y},
                {"left", n => n.R.Y}
            };

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

            var edgeToCondition = new Dictionary<string, Func<Cell, bool>>
            {
                {"top", t => t.Edge1.R.Y == farfield.Height},
                {"bottom", t => t.Edge1.R.Y == 0},
                {"left", t => t.Edge1.R.X == 0},
                {"right", t => t.Edge1.R.X == farfield.Width},
                {"all", t => true}
            };

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
        public Array FindHorizontalEdge(int t, Edge vertSide, Edge longSide)
        {
            var h = (from l in CellList[t].Edges
                     where l.SideName != vertSide.SideName && l.SideName != longSide.SideName
                     select l).ToArray();

            //returning an array forces imediate execution
            return h;
        }

        /// <summary>
        /// Returns the top left most border cell on the lid
        /// </summary>
        /// <param name="farfield"></param>
        /// <returns>Cell</returns>
        public Cell TopLeft(Farfield farfield)
        {

            Cell result = (from c in GetElementsByBoundary("top", farfield)
                           orderby c.R.X ascending
                           select c).AsParallel().First();

            return result;
        }

        /// <summary>
        /// Returns the top right-most border cell on the lid boundary
        /// </summary>
        /// <param name="farfield"></param>
        /// <returns>Cell</returns>
        public Cell TopRight(Farfield farfield)
        {

            Cell result = (from c in GetElementsByBoundary("top", farfield)
                           orderby c.R.X descending
                           select c).AsParallel().First();

            return result;
        }
    }
}