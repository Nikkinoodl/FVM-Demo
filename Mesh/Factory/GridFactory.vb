Imports Core.Domain
Imports Core.Common
Imports System.Numerics
Imports System.Windows.Forms.VisualStyles

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
        ''' Add a new node which optionally lies on the boundary or airfoil surface (deprecated)
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="this_x"></param>
        ''' <param name="this_y"></param>
        ''' <param name="this_surface"></param>
        ''' <param name="this_boundary"></param>
        Public Sub RequestNode(this_id As Integer, this_x As Single, this_y As Single, this_surface As Boolean, this_boundary As Boolean) Implements IGridFactory.RequestNode
            Dim newNode As New Node(this_id, this_x, this_y, this_surface, this_boundary)
        End Sub

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
        ''' Add a generic node which does not lie on the boundary or airfoil surface (deprecated)
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="this_x"></param>
        ''' <param name="this_y"></param>
        Public Sub AddNode(this_id As Integer, this_x As Single, this_y As Single) Implements IGridFactory.AddNode
            Dim newNode As New Node(this_id, this_x, this_y, False, False)
        End Sub

        ''' <summary>
        ''' Adds a generic node which does not lie on the boundary or airfoil surface
        ''' </summary>
        ''' <param name="this_id"></param>
        ''' <param name="this_x"></param>
        ''' <param name="this_y"></param>
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
        ''' Add a new cell
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

        Public Sub AddSquare(this_id As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer,
                    Optional s1 As SideType = SideType.none,
                    Optional s2 As SideType = SideType.none,
                    Optional s3 As SideType = SideType.none,
                    Optional s4 As SideType = SideType.none) Implements IGridFactory.AddSquare
            Dim e1 As New Edge(0, s1)
            Dim e2 As New Edge(1, s2)
            Dim e3 As New Edge(2, s3)
            Dim e4 As New Edge(3, s4)

            Dim newCell As New Cell(this_id, n1, n2, n3, n4, e1, e2, e3, e4)

        End Sub

        ''' <summary>
        ''' Replace an existing grid cell with a new one
        ''' To avoid confusion, note that t is the index, not the Id
        ''' </summary>
        ''' <param name="t">Index</param>
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

        Public Sub ReplaceCellSquare(t As Integer, newId As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer,
                                   s1 As SideType, s2 As SideType, s3 As SideType, s4 As SideType) Implements IGridFactory.ReplaceCellSquare

            Dim replaceCell As Cell = New ReplacementCell(t, newId, n1, n2, n3, n4, s1, s2, s3, s4)

        End Sub

        ''' <summary>
        ''' Update the properties of an existing grid cell
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
        ''' Creates the first set of triangular grid cells in a mesh when no airfoil is present
        ''' </summary>
        Public Sub SetupEmptySpaceCells() Implements IGridFactory.SetupEmptySpaceCells

            'Note that nodes are always assigned to vertices in a clockwise arrangement

            AddCell(0, 0, 4, 3, SideType.none, SideType.boundary, SideType.boundary)
            AddCell(1, 4, 1, 3, SideType.none, SideType.none, SideType.boundary)
            AddCell(2, 1, 5, 3, SideType.boundary, SideType.none, SideType.none)
            AddCell(3, 1, 2, 5, SideType.boundary, SideType.none, SideType.boundary)

        End Sub

        ''' <summary>
        ''' Creates a the first cell in a rectangular mesh when no airfoil is present
        ''' </summary>
        Public Sub SetupRegularGrid() Implements IGridFactory.SetupRegularGrid

            'Note that nodes are always assigned to vertices in a clockwise arrangement
            AddSquare(0, 0, 1, 2, 3, SideType.boundary, SideType.boundary, SideType.boundary, SideType.boundary)

        End Sub
#End Region
    End Class
End Namespace
