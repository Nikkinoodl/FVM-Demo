Imports Core.Common

Namespace Services
    Public Interface ICellSplitter

        ''' <summary>
        ''' Refines the mesh by splitting triangular cells between the longest side and the opposing vertex
        ''' </summary>
        Sub SplitCells(Optional ignoreRightAngleTriangles As Boolean = False)

        ''' <summary>
        ''' Refines an equilateral triangle grid by dividing each cell into four equal triangles
        ''' </summary>
        Sub DivideEquilateral()

        ''' <summary>
        ''' Refines a rectangular grid by performing a join tiling. Each cell is divided by joining the center of
        ''' each edge to a new node at the cell center
        ''' </summary>
        Sub DivideRectangularCells()

        ''' <summary>
        ''' Refines the mesh by performing a kis tiling. Each cell is divided by joining each node
        ''' to a new node at the cell center.
        ''' </summary>
        Sub DivideKis(farfield As Farfield)

        ''' <summary>
        ''' Refines a triangular mesh by performing a join tiling. Each cell is divided by joining the midpoint of
        ''' each side to a new node at the cell center.
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub DivideOrtho(farfield As Farfield)

        ''' <summary>
        ''' Refines a mesh by performing a trunc tiling. Each cell is divided by truncating it at each node.
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub DivideTrunc(farfield As Farfield)

        ''' <summary>
        ''' Combines corner offcuts of the quad trunc tiling operations into single cells
        ''' </summary>
        Sub CombineQuadGrid(farfield As Farfield)

        ''' <summary>
        ''' Combines corner offcuts of the triangle trunc tiling operations into single cells
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub CombineTriangleGrid(farfield As Farfield)

    End Interface
End Namespace