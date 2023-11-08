Imports Core.Domain
Imports Core.Common
Imports System.Numerics

Namespace Factories

    ''' <summary>
    ''' This factory is used to create basic grid elements (nodes and cells)
    ''' </summary>
    Public Class GridFactory : Implements IGridFactory

#Region "Constructor"

        Public Sub New()

        End Sub
#End Region

#Region "Simple Create/Update/Replace Methods"
        ''' <summary>
        ''' Add a new node which optionally lies on the boundary or airfoil surface
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="r"></param>
        ''' <param name="this_surface"></param>
        ''' <param name="this_boundary"></param>
        Public Sub RequestNode(this_id As Integer, r As Vector2, this_surface As Boolean, this_boundary As Boolean) Implements IGridFactory.RequestNode
            Dim newNode As New Node(this_id, r, this_surface, this_boundary)
        End Sub

        ''' <summary>
        ''' Adds a generic node which does not lie on the boundary or airfoil surface
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="thisPosition"></param>
        Public Sub AddNode(this_id As Integer, thisPosition As Vector2) Implements IGridFactory.AddNode
            Dim newNode As New Node(this_id, thisPosition)
        End Sub

        ''' <summary>
        ''' Add a node on the boundary of the farfield
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="this_x"></param>
        ''' <param name="this_y"></param>
        Public Sub AddBoundaryNode(this_id As Integer, this_x As Single, this_y As Single) Implements IGridFactory.AddBoundaryNode
            Dim newNode As New Node(this_id, this_x, this_y, False, True)
        End Sub

        ''' <summary>
        ''' Add a node on the surface of the airfoil
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="this_x"></param>
        ''' <param name="this_y"></param>
        Public Sub AddAirfoilNode(this_id As Integer, this_x As Single, this_y As Single) Implements IGridFactory.AddAirfoilNode
            Dim newNode As New Node(this_id, this_x, this_y, True, False)
        End Sub

        ''' <summary>
        ''' Adds a cell with three nodes
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="this_n1"></param>
        ''' <param name="this_n2"></param>
        ''' <param name="this_n3"></param>
        ''' <param name="this_s1"></param>
        ''' <param name="this_s2"></param>
        ''' <param name="this_s3"></param>
        Public Sub AddCell(this_id As Integer, this_n1 As Integer, this_n2 As Integer, this_n3 As Integer,
                               Optional sideType1 As SideType = SideType.none,
                               Optional sideType2 As SideType = SideType.none,
                               Optional sideType3 As SideType = SideType.none) Implements IGridFactory.AddCell

            Dim edge1 As New Edge(0, sideType1)
            Dim edge2 As New Edge(1, sideType2)
            Dim edge3 As New Edge(2, sideType3)

            Dim newCell As New Cell(this_id, this_n1, this_n2, this_n3, edge1, edge2, edge3)

        End Sub

        ''' <summary>
        ''' Adds a cell with four nodes
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
        Public Sub AddQuad(this_id As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer,
                    Optional s1 As SideType = SideType.none,
                    Optional s2 As SideType = SideType.none,
                    Optional s3 As SideType = SideType.none,
                    Optional s4 As SideType = SideType.none) Implements IGridFactory.AddQuad
            Dim e1 As New Edge(0, s1)
            Dim e2 As New Edge(1, s2)
            Dim e3 As New Edge(2, s3)
            Dim e4 As New Edge(3, s4)

            Dim newCell As New Cell(this_id, n1, n2, n3, n4, e1, e2, e3, e4)

        End Sub

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
        Public Sub AddPent(this_id As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer, n5 As Integer,
                   Optional s1 As SideType = SideType.none, Optional s2 As SideType = SideType.none,
                   Optional s3 As SideType = SideType.none, Optional s4 As SideType = SideType.none,
                   Optional s5 As SideType = SideType.none) Implements IGridFactory.AddPent

            Dim e1 As New Edge(0, s1)
            Dim e2 As New Edge(1, s2)
            Dim e3 As New Edge(2, s3)
            Dim e4 As New Edge(3, s4)
            Dim e5 As New Edge(4, s5)

            Dim newCell As New Cell(this_id, n1, n2, n3, n4, n5, e1, e2, e3, e4, e5)

        End Sub

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
        Public Sub AddHex(this_id As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer,
                    n5 As Integer, n6 As Integer,
                    Optional s1 As SideType = SideType.none,
                    Optional s2 As SideType = SideType.none,
                    Optional s3 As SideType = SideType.none,
                    Optional s4 As SideType = SideType.none,
                    Optional s5 As SideType = SideType.none,
                    Optional s6 As SideType = SideType.none) Implements IGridFactory.AddHex

            Dim e1 As New Edge(0, s1)
            Dim e2 As New Edge(1, s2)
            Dim e3 As New Edge(2, s3)
            Dim e4 As New Edge(3, s4)
            Dim e5 As New Edge(4, s5)
            Dim e6 As New Edge(5, s6)

            Dim newCell As New Cell(this_id, n1, n2, n3, n4, n5, n6, e1, e2, e3, e4, e5, e6)

        End Sub

        ''' <summary>
        ''' Adds a new octagonal cell
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
        Public Sub AddOct(this_id As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer, n5 As Integer,
                   n6 As Integer, n7 As Integer, n8 As Integer,
                   Optional s1 As SideType = SideType.none, Optional s2 As SideType = SideType.none,
                   Optional s3 As SideType = SideType.none, Optional s4 As SideType = SideType.none,
                   Optional s5 As SideType = SideType.none, Optional s6 As SideType = SideType.none,
                   Optional s7 As SideType = SideType.none, Optional s8 As SideType = SideType.none) Implements IGridFactory.AddOct

            Dim e1 As New Edge(0, s1)
            Dim e2 As New Edge(1, s2)
            Dim e3 As New Edge(2, s3)
            Dim e4 As New Edge(3, s4)
            Dim e5 As New Edge(4, s5)
            Dim e6 As New Edge(5, s6)
            Dim e7 As New Edge(6, s7)
            Dim e8 As New Edge(7, s8)

            Dim newCell As New Cell(this_id, n1, n2, n3, n4, n5, n6, n7, n8, e1, e2, e3, e4, e5, e6, e7, e8)

        End Sub

        ''' <summary>
        ''' Replaces an existing grid cell with a new triangular cell
        ''' </summary>
        ''' <param name="t">this index of the cell in Celllist</param>
        ''' <param name="newId"></param>
        ''' <param name="this_n1"></param>
        ''' <param name="this_n2"></param>
        ''' <param name="this_n3"></param>
        ''' <param name="this_s1"></param>
        ''' <param name="this_s2"></param>
        ''' <param name="this_s3"></param>
        Public Sub ReplaceCell(t As Integer, newId As Integer, this_n1 As Integer, this_n2 As Integer,
                                   this_n3 As Integer,
                                   Optional side_1 As SideType = SideType.none,
                                   Optional side_2 As SideType = SideType.none,
                                   Optional side_3 As SideType = SideType.none) Implements IGridFactory.ReplaceCell

            Dim replacecell As Cell = New ReplacementCell(t, newId, this_n1, this_n2, this_n3, side_1, side_2, side_3)

        End Sub

        ''' <summary>
        ''' Replaces an existing quad cell with a new one
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
        Public Sub ReplaceCellQuad(t As Integer, newId As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer,
                              Optional s1 As SideType = SideType.none,
                              Optional s2 As SideType = SideType.none,
                              Optional s3 As SideType = SideType.none,
                              Optional s4 As SideType = SideType.none) Implements IGridFactory.ReplaceCellQuad

            Dim replaceCell As Cell = New ReplacementCell(t, newId, n1, n2, n3, n4, s1, s2, s3, s4)

        End Sub

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
        Public Sub ReplaceTriWithQuad(t As Integer, newId As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer,
                              Optional s1 As SideType = SideType.none,
                              Optional s2 As SideType = SideType.none,
                              Optional s3 As SideType = SideType.none,
                              Optional s4 As SideType = SideType.none) Implements IGridFactory.ReplaceTriWithQuad

            Dim upgradeCell As Cell = New UpgradeCell(t, newId, n1, n2, n3, n4, s1, s2, s3, s4)

        End Sub

        ''' <summary>
        ''' Update the properties of an existing triangular grid cell
        ''' To avoid confusion, note that t is the index, not the Id
        ''' </summary>
        ''' <param name="t">Index</param>
        ''' <param name="this_n1"></param>
        ''' <param name="this_n2"></param>
        ''' <param name="this_n3"></param>
        ''' <param name="this_s1"></param>
        ''' <param name="this_s2"></param>
        ''' <param name="this_s3"></param>
        Public Sub UpdateCell(t As Integer, this_n1 As Integer, this_n2 As Integer, this_n3 As Integer,
                                  Optional sideType1 As SideType = SideType.none,
                                  Optional sideType2 As SideType = SideType.none,
                                  Optional sideType3 As SideType = SideType.none) Implements IGridFactory.UpdateCell

            Dim updateCell As Cell = New UpdateCell(t, this_n1, this_n2, this_n3, sideType1, sideType2, sideType3)

        End Sub
#End Region

#Region "Empty Space Build Methods"

        ''' <summary>
        ''' Adds corner nodes to an empty farfield when no airfoil is present
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub AddCornerNodes(farfield As Farfield) Implements IGridFactory.AddCornerNodes

            AddBoundaryNode(0, 0, 0)
            AddBoundaryNode(1, 0, farfield.Height)
            AddBoundaryNode(2, farfield.Width, farfield.Height)
            AddBoundaryNode(3, farfield.Width, 0)

        End Sub

        ''' <summary>
        ''' Add mid boundary nodes to an empty farfield when no airfoil is present
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub AddMidBoundaryNodes(farfield As Farfield) Implements IGridFactory.AddMidBoundaryNodes

            AddBoundaryNode(4, 0, farfield.Height / 2)
            AddBoundaryNode(5, farfield.Width, farfield.Height / 2)

        End Sub

        ''' <summary>
        ''' Adds a node at the center of the farfield top boundary
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub AddTopBoundaryNode(farfield As Farfield) Implements IGridFactory.AddTopBoundaryNode

            AddBoundaryNode(4, farfield.Width / 2, farfield.Height)

        End Sub

        ''' <summary>
        ''' Creates the first set of irregular triangular grid cells in a mesh when no airfoil is present
        ''' </summary>
        Public Sub SetupIrregularTriangleGrid() Implements IGridFactory.SetupIrregularTriangleGrid

            'Note that nodes are always assigned to vertices in a clockwise arrangement
            AddCell(0, 0, 4, 3, SideType.none, SideType.boundary, SideType.boundary)
            AddCell(1, 4, 1, 3, SideType.none, SideType.none, SideType.boundary)
            AddCell(2, 1, 5, 3, SideType.boundary, SideType.none, SideType.none)
            AddCell(3, 1, 2, 5, SideType.boundary, SideType.none, SideType.boundary)

        End Sub

        ''' <summary>
        ''' Creates the first cells in an equilateral triangle mesh when no airfoil is present
        ''' </summary>
        Public Sub SetupEquilateralTriangleGrid() Implements IGridFactory.SetupEquilateralTriangleGrid

            'nodes are always assigned to vertices in a clockwise arrangement,
            AddCell(0, 0, 1, 4, SideType.boundary, SideType.none, SideType.boundary)
            AddCell(1, 0, 4, 3, SideType.none, SideType.boundary, SideType.none)
            AddCell(2, 3, 4, 2, SideType.boundary, SideType.boundary, SideType.none)

        End Sub

        ''' <summary>
        ''' Creates a the first cell in a rectangular mesh when no airfoil is present
        ''' </summary>
        Public Sub SetupRectangularGrid() Implements IGridFactory.SetupRectangularGrid

            'nodes are always assigned to vertices in a clockwise arrangement
            AddQuad(0, 0, 1, 2, 3, SideType.boundary, SideType.boundary, SideType.boundary, SideType.boundary)

        End Sub
#End Region
    End Class
End Namespace
