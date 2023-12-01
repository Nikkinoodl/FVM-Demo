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
        /// Returns the node ids of a triangular or quad cell
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public CellNodes GetNodeDetails(int t)
        {
            var result = new CellNodes()
            {
                N1 = (int)CellList[t].V1,
                N2 = (int)CellList[t].V2,
                N3 = (int)CellList[t].V3
            };

            if (CellList[t].V4 != null)
            {
                result.N4 = CellList[t].V4;
            }

            return result;
        }

        /// <summary>
        /// Returns the surface types of the nodes of a triangular cell
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public CellNodeTypes GetNodeSurface(CellNodes n)
        {
            return new CellNodeTypes()
            {
                S1 = NodeV(n.N1).Surface,
                S2 = NodeV(n.N2).Surface,
                S3 = NodeV(n.N3).Surface
            };
        }

        /// <summary>
        /// Returns the position vectors associated with each node of a triangular or quad cell
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public CellNodeVectors GetPositionVectors(CellNodes n)
        {
            var result = new CellNodeVectors()
            {
                R1 = NodeV(n.N1).R,
                R2 = NodeV(n.N2).R,
                R3 = NodeV(n.N3).R
            };

            if (n.N4 != null)
            {
                result.R4 = NodeV(n.N4).R;
            }

            return result;
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
        /// Gets the SideType of each edge in a triangular cell
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public CellSideTypes GetSideTypes(int t)
        {
            return new CellSideTypes()
            {
                S1 = CellList[t].Edge1.SideType,
                S2 = CellList[t].Edge2.SideType,
                S3 = CellList[t].Edge3.SideType
            };

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