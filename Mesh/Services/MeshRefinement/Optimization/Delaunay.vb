Imports Core.Interfaces
Imports Core.Common
Imports Mesh.Factories
Imports System.Numerics
Imports Mesh.Services.SharedUtilities

Namespace Services
    Public Class Delaunay : Implements IDelaunay

        Private ReadOnly data As IDataAccessService
        Private ReadOnly factory As IGridFactory

        Public Sub New(data As IDataAccessService, factory As IGridFactory)

            Me.data = data
            Me.factory = factory

        End Sub

        Public Sub Delaunay() Implements IDelaunay.Delaunay
            'Laplace smoothing and SetCompleteStatus should be run before this

            'We are after every combination of vertex arrangements.
            '
            'Each possible configuration (numbered 1 through 6) is checked through iteration
            '-see comments below to see how this is coded.
            'Cell configurations are used by private methods in this class and in the Cellquery class.
            '
            'In these diagrams 'this' refers to the current (known) cell and 'adj' is the adjacent cell. 'np' is the
            'vertex on the 'adj' cell that is being tested to see if it lies within the circumcircle of 'this' cell.
            '
            ' 1. v2 is np
            '
            '                       v2---------v3    2
            '                        \         /   /   \
            '                         \       /   /     \
            '                          \     /   /       \
            '                             v1    1 --------3
            '
            '                           adj         this
            ' 
            '
            ' 2. v3 is np
            '
            '                                 2       v2--------- v3
            '                              /     \      \       /
            '                             /       \      \     /
            '                            /         \      \   /
            '                           1 ----------3      v1
            '                               this            adj
            '
            '
            ' 3. v1 is np
            '
            '                                 2      
            '                              /     \ 
            '                             /       \  
            '                            /         \ 
            '                           1 ----------3
            '                               this        
            '
            '                           v2--------- v3
            '                             \       /
            '                              \     /
            '                               \   /
            '                                 v1
            '                                adj
            '
            ' 4.: v3 is np
            '
            '                       v3---------v1      2
            '                        \         /     /    \
            '                         \       /    /        \
            '                          \     /   /            \
            '                             v2    1 -------------3
            '
            '                           adj         this
            ' 
            '
            ' 5. v1 is np
            '
            '                                 2       v3--------- v1
            '                              /     \      \       /
            '                             /       \      \     /
            '                            /         \      \   /
            '                           1 ----------3      v2
            '                               this            adj
            '
            '
            ' 6. v2 is np
            '
            '                                 2      
            '                              /     \ 
            '                             /       \  
            '                            /         \ 
            '                           1 ----------3
            '                               this        
            '
            '                           v3--------- v1
            '                             \       /
            '                              \     /
            '                               \   /
            '                                 v2
            '                                adj
            '
            '

            Dim configurations = New Integer() {1, 2, 3, 4, 5, 6}

            Dim numcells = data.CellList.Count

            For Each config In configurations

                'cycle through cells.
                'to avoid confusion, note that t is the index of the cell, not the Id
                For t = 0 To numcells - 1

                    Dim cell = data.CellList(t)

                    'find SideType of joining side
                    Dim side = JoiningSide(config, cell)

                    'exclude cell if joining side is boundary or surface
                    If (side = SideType.boundary Or side = SideType.surface) Then Continue For

                    'get this cell vertex nodes and identify adjacent cells
                    Dim nodes = GetNodesAsArray(cell)
                    Dim adjacentCells = data.AdjacentCells(config, nodes)

                    'make sure we're only doing this if there is an adjacent cell on this side
                    If adjacentCells.Count() = 0 Then Continue For

                    'identify which node will be np and get the index of the first adjacent cell
                    Dim np = ProcessAdjacent(config, adjacentCells.First)
                    Dim t2 = data.CellList.IndexOf(adjacentCells.First)

                    'get position vectors
                    Dim position As (r1 As Vector2, rP As Vector2) = GetCoords(nodes(0), np)
                    Dim rCenter = GetCellCenter(t)

                    'we only do Delaunay when np is in cell circumcircle
                    If Not CheckInCircle(position.r1, position.rP, rCenter) Or t = t2 Then Continue For

                    'flip the cells
                    FlipCells(config, t, t2, np, nodes)

                Next
            Next
        End Sub

        ''' <summary>
        ''' Returns a tuple of position vectors
        ''' </summary>
        Private Function GetCoords(n1 As Integer, np As Integer) As (Vector2, Vector2)

            Dim r1 = data.NodeV(n1).R
            Dim rP = data.NodeV(np).R

            Return (r1, rP)

        End Function

        ''' <summary>
        ''' Finds  the center of the cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Private Function GetCellCenter(t As Integer) As Vector2

            Dim result = data.CellList(t).R

            Return result

        End Function

        ''' <summary>
        ''' Flips the adjoining edge of a pair of cells. See the diagram in the comments above to
        ''' understand how nodes are assigned.
        ''' </summary>
        ''' <param name="configuration"></param>
        ''' <param name="factory"></param>
        Private Sub FlipCells(configuration As Integer, t As Integer, t2 As Integer, np As Integer, n As Integer())

            Dim s1 = GetSideTypesAsArray(data.CellList(t))
            Dim s2 = GetSideTypesAsArray(data.CellList(t2))

            Select Case configuration

                Case 1

                    factory.UpdateCell(t, np, n(1), n(2), s1(0), SideType.none, s2(0))
                    factory.UpdateCell(t2, np, n(2), n(0), s1(1), s2(2), SideType.none)

                Case 2

                    factory.UpdateCell(t, np, n(2), n(0), s1(1), SideType.none, s2(1))
                    factory.UpdateCell(t2, np, n(0), n(1), s1(2), s2(0), SideType.none)

                Case 3

                    factory.UpdateCell(t, np, n(0), n(1), s1(2), SideType.none, s2(2))
                    factory.UpdateCell(t2, np, n(1), n(2), s1(0), s2(1), SideType.none)

                Case 4

                    factory.UpdateCell(t, np, n(1), n(2), s1(0), SideType.none, s2(1))
                    factory.UpdateCell(t2, np, n(2), n(0), s1(1), s2(0), SideType.none)

                Case 5

                    factory.UpdateCell(t, np, n(2), n(0), s1(1), SideType.none, s2(2))
                    factory.UpdateCell(t2, np, n(0), n(1), s1(2), s2(1), SideType.none)

                Case 6

                    factory.UpdateCell(t, np, n(0), n(1), s1(2), SideType.none, s2(0))
                    factory.UpdateCell(t2, np, n(1), n(2), s1(0), s2(2), SideType.none)

                Case Else

                    Throw New Exception

            End Select

        End Sub

    End Class
End Namespace

