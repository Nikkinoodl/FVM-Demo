Imports System.Numerics
Imports Core.Common

Namespace Factories

    ''' <summary>
    ''' This factory is used to create basic grid elements (nodes and cells)
    ''' </summary>
    Public Interface IGridFactory

#Region "Nodes"

        ''' <summary>
        ''' Add a node when the boundary and/or surface flags must be specified
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="r"></param>
        ''' <param name="this_surface"></param>
        ''' <param name="this_boundary"></param>
        Sub RequestNode(this_id As Integer, r As Vector2, this_surface As Boolean,
                        this_boundary As Boolean)

        ''' <summary>
        ''' Adds a single node to the farfield interior
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="thisPosition"></param>
        Sub AddNode(this_id As Integer, thisPosition As Vector2)

        ''' <summary>
        ''' Adds a node on the farfield boundary
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="this_x"></param>
        ''' <param name="this_y"></param>
        Sub AddBoundaryNode(this_id As Integer, this_x As Single, this_y As Single)

        ''' <summary>
        ''' Adds nodes at the corners of the farfield boundary
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub AddCornerNodes(farfield As Farfield)

        ''' <summary>
        ''' Adds new nodes in the middle of the farfield left and right boundaries
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub AddMidBoundaryNodes(farfield As Farfield)

        ''' <summary>
        ''' Adds a new onde in the middle of the farfield top boundary
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub AddTopBoundaryNode(farfield As Farfield)

        ''' <summary>
        ''' Adds a new airfoil node (deprecated)
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="this_x"></param>
        ''' <param name="this_y"></param>
        Sub AddAirfoilNode(this_id As Integer, this_x As Single, this_y As Single)

#End Region

#Region "Cells"

        ''' <summary>
        ''' Adds a new triangular cell
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="this_n1"></param>
        ''' <param name="this_n2"></param>
        ''' <param name="this_n3"></param>
        ''' <param name="sideType1"></param>
        ''' <param name="sideType2"></param>
        ''' <param name="sideType3"></param>
        Sub AddCell(this_id As Integer, this_n1 As Integer, this_n2 As Integer, this_n3 As Integer,
                        Optional sideType1 As SideType = SideType.none,
                        Optional sideType2 As SideType = SideType.none,
                        Optional sideType3 As SideType = SideType.none)

        ''' <summary>
        ''' Adds a new rectangular cell
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="n1"></param>
        ''' <param name="n2"></param>
        ''' <param name="n3"></param>
        ''' <param name="n4"></param>
        ''' <param name="s1"></param>
        ''' <param name="s2"></param>
        ''' <param name="s3"></param>
        ''' <param name="s4"></param>
        Sub AddQuad(this_id As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer,
                    Optional s1 As SideType = SideType.none,
                    Optional s2 As SideType = SideType.none,
                    Optional s3 As SideType = SideType.none,
                    Optional s4 As SideType = SideType.none)

        ''' <summary>
        ''' Adds a new pentagonal cell
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="n1"></param>
        ''' <param name="n2"></param>
        ''' <param name="n3"></param>
        ''' <param name="n4"></param>
        ''' <param name="n5"></param>
        ''' <param name="s1"></param>
        ''' <param name="s2"></param>
        ''' <param name="s3"></param>
        ''' <param name="s4"></param>
        ''' <param name="s5"></param>
        Sub AddPent(this_id As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer, n5 As Integer,
                   Optional s1 As SideType = SideType.none, Optional s2 As SideType = SideType.none,
                   Optional s3 As SideType = SideType.none, Optional s4 As SideType = SideType.none,
                   Optional s5 As SideType = SideType.none)

        ''' <summary>
        ''' Adds a new hexagonal cell
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="n1"></param>
        ''' <param name="n2"></param>
        ''' <param name="n3"></param>
        ''' <param name="n4"></param>
        ''' <param name="n5"></param>
        ''' <param name="n6"></param>
        ''' <param name="s1"></param>
        ''' <param name="s2"></param>
        ''' <param name="s3"></param>
        ''' <param name="s4"></param>
        ''' <param name="s5"></param>
        ''' <param name="s6"></param>
        Sub AddHex(this_id As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer, n5 As Integer,
                   n6 As Integer, Optional s1 As SideType = SideType.none, Optional s2 As SideType = SideType.none,
                   Optional s3 As SideType = SideType.none, Optional s4 As SideType = SideType.none,
                   Optional s5 As SideType = SideType.none, Optional s6 As SideType = SideType.none)

        ''' <summary>
        ''' Adds an octagonal cell
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="n1"></param>
        ''' <param name="n2"></param>
        ''' <param name="n3"></param>
        ''' <param name="n4"></param>
        ''' <param name="n5"></param>
        ''' <param name="n6"></param>
        ''' <param name="n7"></param>
        ''' <param name="n8"></param>
        ''' <param name="s1"></param>
        ''' <param name="s2"></param>
        ''' <param name="s3"></param>
        ''' <param name="s4"></param>
        ''' <param name="s5"></param>
        ''' <param name="s6"></param>
        ''' <param name="s7"></param>
        ''' <param name="s8"></param>
        Sub AddOct(this_id As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer, n5 As Integer,
                   n6 As Integer, n7 As Integer, n8 As Integer,
                   Optional s1 As SideType = SideType.none, Optional s2 As SideType = SideType.none,
                   Optional s3 As SideType = SideType.none, Optional s4 As SideType = SideType.none,
                   Optional s5 As SideType = SideType.none, Optional s6 As SideType = SideType.none,
                   Optional s7 As SideType = SideType.none, Optional s8 As SideType = SideType.none)


#End Region

#Region "Initial builds"

        ''' <summary>
        ''' Performs the initial build of an irregular triangle mesh
        ''' </summary>
        Sub SetupIrregularTriangleGrid()

        ''' <summary>
        ''' Performs the initial build of a regular triangular mesh
        ''' </summary>
        Sub SetupEquilateralTriangleGrid()

        ''' <summary>
        ''' Performs the initial build of a quad mesh
        ''' </summary>
        Sub SetupRectangularGrid()

#End Region

#Region "Replacements"

        ''' <summary>
        ''' Replaces an existing triangular cell with a new one
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="newId"></param>
        ''' <param name="this_n1"></param>
        ''' <param name="this_n2"></param>
        ''' <param name="this_n3"></param>
        ''' <param name="sideType1"></param>
        ''' <param name="sideType2"></param>
        ''' <param name="sideType3"></param>
        Sub ReplaceCell(t As Integer, newId As Integer, this_n1 As Integer, this_n2 As Integer, this_n3 As Integer,
                            Optional sideType1 As SideType = SideType.none,
                            Optional sideType2 As SideType = SideType.none,
                            Optional sideType3 As SideType = SideType.none)

        ''' <summary>
        ''' Replaces a existing quad cell with a new one
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="newId"></param>
        ''' <param name="n1"></param>
        ''' <param name="n2"></param>
        ''' <param name="n3"></param>
        ''' <param name="n4"></param>
        ''' <param name="s1"></param>
        ''' <param name="s2"></param>
        ''' <param name="s3"></param>
        ''' <param name="s4"></param>
        Sub ReplaceCellQuad(t As Integer, newId As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer,
                              Optional s1 As SideType = SideType.none,
                              Optional s2 As SideType = SideType.none,
                              Optional s3 As SideType = SideType.none,
                              Optional s4 As SideType = SideType.none)

        ''' <summary>
        ''' Replaces a triangular cell with a quad cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="newId"></param>
        ''' <param name="n1"></param>
        ''' <param name="n2"></param>
        ''' <param name="n3"></param>
        ''' <param name="n4"></param>
        ''' <param name="s1"></param>
        ''' <param name="s2"></param>
        ''' <param name="s3"></param>
        ''' <param name="s4"></param>
        Sub ReplaceTriWithQuad(t As Integer, newId As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer,
                              Optional s1 As SideType = SideType.none,
                              Optional s2 As SideType = SideType.none,
                              Optional s3 As SideType = SideType.none,
                              Optional s4 As SideType = SideType.none)


        ''' <summary>
        ''' Updates the nodes and optionally the side types of an existing triangle
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="this_n1"></param>
        ''' <param name="this_n2"></param>
        ''' <param name="this_n3"></param>
        ''' <param name="sideType1"></param>
        ''' <param name="sideType2"></param>
        ''' <param name="sideType3"></param>
        Sub UpdateCell(t As Integer, this_n1 As Integer, this_n2 As Integer, this_n3 As Integer,
                           Optional sideType1 As SideType = SideType.none,
                           Optional sideType2 As SideType = SideType.none,
                           Optional sideType3 As SideType = SideType.none)

#End Region

    End Interface
End Namespace
