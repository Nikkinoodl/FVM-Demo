﻿using Core.Domain;
using Core.Common;
using System.Numerics;

namespace Core.Interfaces
{
    public interface IDataAccessService
    {
        /// <summary>
        /// A list of all cells
        /// </summary>
        List<Cell> CellList { get; set; }

        /// <summary>
        /// A list of all nodes
        /// </summary>
        List<Node> Nodelist { get; set; }

        /// <summary>
        /// A list of all nodes in the interior of the farfield
        /// </summary>
        /// <returns></returns>
        List<Node> InteriorNodes();

        /// <summary>
        /// A list of all nodes
        /// </summary>
        /// <returns></returns>
        List<Node> AllNodes();

        /// <summary>
        /// Returns the position vectors of a non-triangular cell as an array
        /// </summary>
        /// <param name="n"></param>
        /// <param name="nSides"></param>
        /// <returns></returns>
        Vector2[] GetPositionVectorsAsArray(int[] n);

        /// <summary>
        /// Returns the surface types of the nodes in a triangular cell as an array
        /// </summary>
        /// <param name="n"></param>
        /// <param name="nSides"></param>
        /// <returns></returns>
        Boolean[] GetNodeSurfaceAsArray(int[] n);

        /// <summary>
        /// Returns the current maximum cell Id
        /// </summary>
        /// <returns></returns>
        int MaxCellId();

        /// <summary>
        /// Checks for an existing node at the given position vector
        /// </summary>
        /// <param name="rP"></param>
        /// <returns>count</returns>
        int Exists(double xp, double yp);

        /// <summary>
        /// Checks for an existing node at the given coordinates
        /// </summary>
        /// <param name="rP"></param>
        /// <returns>count</returns>
        int Exists(Vector2 rP);

        /// <summary>
        /// Returns the id of an existing node
        /// </summary>
        /// <param name="xp"></param>
        /// <param name="yp"></param>
        /// <returns>id</returns>
        int FindNode(double xp, double yp);

        /// <summary>
        /// Returns the id of an existing node at the given position vector
        /// </summary>
        /// <param name="rP"></param>
        /// <returns>id</returns>
        int FindNode(Vector2 rP);

        /// <summary>
        /// Returns an ordered list of target nodes that can be used for CFD calcs
        /// </summary>
        /// <returns>List of Node</returns>
        List<Node> CalcNodes();

        /// <summary>
        /// Returns a specific node
        /// </summary>
        /// <param name="n">Node Id</param>
        /// <returns>Node</returns>
        Node NodeV(int? n);

        /// <summary>
        /// Returns a list of unprocessed cells
        /// </summary>
        /// <returns>List of Cell</returns>
        List<Cell> IncompleteCells();

        /// <summary>
        /// Returns a list of the cells that contains a specific node
        /// </summary>
        /// <param name="thisnode"></param>
        /// <returns>List of Cell</returns>
        List<Cell> SmoothCell(int thisnode);

        /// <summary>
        /// Counts the number of cells that share a given node
        /// </summary>
        /// <param name="thisnode"></param>
        /// <returns></returns>
        int CellClusterCount(int thisnode, CellType cellType);

        /// <summary>
        /// Returns a list of cells that share a given node
        /// </summary>
        /// <param name="thisnode"></param>
        /// <returns></returns>
        List<Cell> CellCluster(int thisnode, CellType cellType);

        /// <summary>
        /// Returns a list of nodes that are candidates for smoothing. Surface and boundary nodes are excluded
        /// </summary>
        /// <returns>List of Node</returns>
        List<Node> SmoothNode();

        /// <summary>
        /// Returns a list of all nodes that are either on the boundary of the farfield or on the surface of the airfoil
        /// </summary>
        /// <param name="farfield"></param>
        /// <returns>List of Node</returns>
        List<Node> BoundaryNode(Farfield farfield);

        /// <summary>
        /// Returns a list of cells that have an edge on the farfield boundary
        /// note that zero-height border cells are not included
        /// </summary>
        /// <returns>List of Cell</returns>
        List<Cell> BoundaryCell();

        /// <summary>
        /// Returns a list of cells that have an edge on the airfoil surface
        /// note that zero-height border cells are not included
        /// </summary>
        /// <returns></returns>
        List<Cell> SurfaceCell();

        /// <summary>
        /// Returns a list of all grid cells except zero-height border cells
        /// </summary>
        /// <returns>List(Cell)</returns>
        List<Cell> CalcCells();

        /// <summary>
        /// Finds the adjacent cell and side to a given node pair
        /// </summary>
        /// <param name="nA"></param>
        /// <param name="nB"></param>
        /// <returns>Id, SideName</returns>
        (int?, SideName?) AdjacentCellEdge((int nA, int nB, Edge targetEdge) nodePair, int this_t);

        /// <summary>
        /// Returns a list of cells with given nodes used in Delaunay triangulation
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="n3"></param>
        /// <returns>List of Cell</returns>
        /// <exception cref="Exception"></exception>
        List<Cell> AdjacentCells(int configuration, int[] n);

        /// <summary>
        /// Returns an ordered list of nodes on the edge of the farfield boundary. Note that this
        /// excludes the corner nodes.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="farfield"></param>
        /// <returns>List of Node</returns>
        /// <exception cref="Exception"></exception>
        List<Node> EdgeBoundary(string edge, Farfield farfield);

        /// <summary>
        /// Returns a list of the zero-height border cells which lie on a specified farfield boundary
        /// eg: "top", "bottom", "left", "right" or "all"
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="farfield"></param>
        /// <returns>List of Cell</returns>
        List<Cell> GetElementsByBoundary(string edge, Farfield farfield);

        /// <summary>
        /// Returns a list of the zero-height border cells which lie on the airfoil surface
        /// </summary>
        /// <returns></returns>
        List<Cell> GetAirfoilSurfaceElements();

        /// <summary>
        /// Finds the longest side of a cell
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Edge FindLongestSide(int t);

        /// <summary>
        /// Finds the longest side of a cell
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Edge FindLongestSide(Cell t);

        /// <summary>
        /// Finds the third edge in a triangle
        /// </summary>
        /// <param name="t"></param>
        /// <param name="e"></param>
        /// <param name="longSide"></param>
        /// <returns></returns>
        Edge[]? FindHorizontalEdge(int t, Edge vertSide, Edge longSide);

    }
}
