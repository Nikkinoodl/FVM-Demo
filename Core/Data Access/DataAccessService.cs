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
                           select node).OrderBy(n => n.R.X).ThenBy(n => n.R.Y);

            return thislist.ToList();
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
                                select node).AsParallel().Single();

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

            var incomplete = from cell in CellList
                             where cell.Complete == false
                             select cell;

            return incomplete.ToList();
        }

        /// <summary>
        /// Returns a list of all cells except zero-height border cells
        /// of the grid.
        /// </summary>
        /// <returns></returns>
        public List<Cell> CalcCells()
        {

            var cellList = (from t in CellList
                        where t.BorderCell == false
                        orderby t.R.X ascending
                        orderby t.R.Y ascending
                        select t).AsParallel();
                        
            return cellList.ToList();
        }

        /// <summary>
        /// Returns a list of the cells that contains a specific node
        /// </summary>
        /// <param name="thisnode"></param>
        /// <returns></returns>
        public List<Cell> SmoothCell(int thisnode)
        {

            var thislist = (from cell in CellList
                           where cell.V1 == thisnode || cell.V2 == thisnode || cell.V3 == thisnode
                           orderby cell.R.X
                           select cell).AsParallel();

            return thislist.ToList();
        }

        /// <summary>
        /// Returns a list of nodes that are candidates for smoothing. Surface and boundary nodes are excluded
        /// </summary>
        /// <returns></returns>
        public List<Node> SmoothNode()
        {

            var thislist = (from node in Nodelist
                           where node.Boundary == false && node.Surface == false  // nodes on the surface or boundary aren't moved
                           select node).AsParallel();

            return thislist.ToList();
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
                           select cell).AsParallel();

            return thislist.ToList();
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
                           select cell).AsParallel();

            return thislist.ToList();
        }


        /// <summary>
        /// Returns a list of all nodes that are either on the boundary of the farfield or on the surface of the airfoil
        /// </summary>
        /// <param name="farfield"></param>
        /// <returns></returns>
        public List<Node> BoundaryNode(Farfield farfield)
        {

            var boundarynodes = (from node in Nodelist
                                where node.Boundary == true || node.Surface == true
                                select node).AsParallel();

            return boundarynodes.ToList();
        }

        /// <summary>
        /// Ensures that all nodes on the boundary have the correct attribute set
        /// </summary>
        /// <param name="farfield"></param>
        public void CheckBoundaryNode(Farfield farfield)
        {

            foreach (var node in BoundaryNode(farfield))
                node.Boundary = true;
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
                           where (t.V4 == nodePair.nA || t.V3 == nodePair.nA || t.V2 == nodePair.nA || t.V1 == nodePair.nA) && (t.V4 == nodePair.nB || t.V3 == nodePair.nB || t.V2 == nodePair.nB || t.V1 == nodePair.nB)
                           where t.Id != this_t
                           select t).FirstOrDefault();

            if (t_adj != null)
            {
                //matching sides is easy because we can match on the mid point position vector 
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

            var basequery = IncompleteCells();

            var filterquery = basequery.Where(t =>
            {
                switch (configuration)
                {
                    case 1:
                        {
                            return t.V1 == n.N1 && t.V3 == n.N2;
                        }

                    case 2:
                        {
                            return t.V2 == n.N2 && t.V1 == n.N3;
                        }

                    case 3:
                        {
                            return t.V2 == n.N1 && t.V3 == n.N3;
                        }

                    case 4:
                        {
                            return t.V2 == n.N1 && t.V1 == n.N2 ;
                        }

                    case 5:
                        {
                            return t.V3 == n.N2 && t.V2 == n.N3;
                        }

                    case 6:
                        {
                            return t.V3 == n.N1 && t.V1 == n.N3;
                        }
                    default:
                        {
                            throw new Exception();
                        }
                }
            }).AsParallel();

            return filterquery.ToList();
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

            var basequery = Nodelist;

            var filterquery = basequery.Where(n =>
            {
                switch (edge)
                {
                    case "top":
                        {
                            return n.R.Y == farfield.Height && n.R.X != 0 && n.R.X != farfield.Width;
                        }

                    case "bottom":
                        {
                            return n.R.Y == 0 && n.R.X != 0 && n.R.X != farfield.Width;
                        }

                    case "right":
                        {
                            return n.R.X == farfield.Width && n.R.Y != 0 && n.R.Y != farfield.Height;
                        }

                    case "left":
                        {
                            return n.R.X == 0 && n.R.Y != 0 && n.R.Y != farfield.Height;
                        }

                    default:
                        {
                            throw new Exception();
                        }
                }
            }).AsParallel();

            var orderquery = filterquery.OrderBy(n =>
            {
                switch (edge)
                {
                    case "top":
                    case "bottom":
                        {
                            return n.R.X;
                        }

                    case "left":
                    case "right":
                        {
                            return n.R.Y;
                        }

                    default:
                        {
                            throw new Exception();
                        }
                }
            }).AsParallel();
            return orderquery.ToList();
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
            var basequery = (from cell in CellList
                            where cell.BorderCell == true
                            where cell.BorderCellType == BorderType.Farfield
                            select cell).AsParallel();

            var filterquery = basequery.Where(t =>
            {
                switch (edge)
                {
                    case "top":
                        {
                            return t.Edge1.R.Y == farfield.Height;
                        }
                    case "bottom":
                        {
                            return t.Edge1.R.Y == 0;
                        }
                    case "left":
                        {
                            return t.Edge1.R.X == 0;
                        }
                    case "right":
                        {
                            return t.Edge1.R.X == farfield.Width;
                        }
                    case "all":
                        {
                            return true;
                        }
                    default:
                        {
                            throw new Exception();
                        }
                  }
            }).AsParallel();

            return filterquery.ToList();
        }


        /// <summary>
        /// Returns a list of the zero-height border cells which lie on the airfoil surface
        /// </summary>
        /// <returns></returns>
        public List<Cell> GetAirfoilSurfaceElements()
        {

            var basequery = (from cell in CellList
                            where cell.BorderCell == true
                            where cell.BorderCellType == BorderType.Airfoil
                            select cell).AsParallel();

            return basequery.ToList();
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

