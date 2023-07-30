Imports System.Numerics
Imports Core.Common

Namespace Factories

    ''' <summary>
    ''' This factory is used to create basic grid elements (nodes and cells)
    ''' </summary>
    Public Interface IGridFactory

        Sub RequestNode(this_id As Integer, this_x As Single, this_y As Single, this_surface As Boolean,
                        this_boundary As Boolean)
        Sub RequestNode(this_id As Integer, r As Vector2, this_surface As Boolean,
                        this_boundary As Boolean)
        Sub AddNode(this_id As Integer, this_x As Single, this_y As Single)
        Sub AddNode(this_id As Integer, thisPosition As Vector2)
        Sub AddBoundaryNode(this_id As Integer, this_x As Single, this_y As Single)
        Sub AddCornerNodes(farfield As Farfield)
        Sub AddMidBoundaryNodes(farfield As Farfield)
        Sub AddAirfoilNode(this_id As Integer, this_x As Single, this_y As Single)
        Sub AddCell(this_id As Integer, this_n1 As Integer, this_n2 As Integer, this_n3 As Integer,
                        Optional sideType1 As SideType = SideType.none,
                        Optional sideType2 As SideType = SideType.none,
                        Optional sideType3 As SideType = SideType.none)
        Sub AddSquare(this_id As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer,
                    Optional s1 As SideType = SideType.none,
                    Optional s2 As SideType = SideType.none,
                    Optional s3 As SideType = SideType.none,
                    Optional s4 As SideType = SideType.none)
        Sub SetupEmptySpaceCells()

        Sub SetupRegularGrid()

        'If this class is a factory, then this is the returns department.  Replacement goods only.
        Sub ReplaceCell(t As Integer, newId As Integer, this_n1 As Integer, this_n2 As Integer, this_n3 As Integer,
                            Optional sideType1 As SideType = SideType.none,
                            Optional sideType2 As SideType = SideType.none,
                            Optional sideType3 As SideType = SideType.none)

        Sub ReplaceCellSquare(t As Integer, newId As Integer, n1 As Integer, n2 As Integer, n3 As Integer, n4 As Integer,
                              s1 As SideType, s2 As SideType, s3 As SideType, s4 As SideType)

        'Updating of cells
        Sub UpdateCell(t As Integer, this_n1 As Integer, this_n2 As Integer, this_n3 As Integer,
                           Optional sideType1 As SideType = SideType.none,
                           Optional sideType2 As SideType = SideType.none,
                           Optional sideType3 As SideType = SideType.none)

    End Interface
End Namespace
