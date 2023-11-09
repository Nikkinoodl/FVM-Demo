Imports Core.Interfaces
Imports Core.Domain
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
            '                       v2---------v3      2
            '                        \         /     /    \
            '                         \       /    /        \
            '                          \     /   /            \
            '                             v1    1 -------------3
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

                'Cycle through cells.
                'To avoid confusion, note that t is the index of the cell, not the Id
                For t = 0 To numcells - 1

                    'exclude if already processed
                    If Not data.CellList(t).Complete Then

                        'find SideType of joining side
                        Dim side = JoiningSide(config, t)

                        'exclude if joining side is boundary or surface
                        If Not (side = SideType.boundary Or side = SideType.surface) Then

                            'get this cell vertex nodes
                            'GetCellData(t)
                            Dim nodes As CellNodes = GetNodeDetails(t)

                            'identify adjacent cells
                            Dim adjacentCells = data.AdjacentCells(config, nodes)

                            'make sure we're only doing this if there is an adjacent cell on this side
                            If adjacentCells.Count() = 1 Then

                                'identify which node will be np
                                Dim np = ProcessAdjacent(config, adjacentCells.First)

                                Dim t2 = data.CellList.IndexOf(adjacentCells.First)

                                'get position vectors
                                Dim position As (r1 As Vector2, rP As Vector2) = GetCoords(nodes.N1, np)
                                Dim rCenter = GetCellCenter(t)

                                'If np is in cell circumcircle
                                If CheckInCircle(position.r1, position.rP, rCenter) And t <> t2 Then
                                    'Flip the cells
                                    FlipCells(config, t, t2, np, nodes)
                                End If

                            End If
                        End If
                    End If
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
        ''' Gets the node ids of the vertices of the given cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Private Function GetNodeDetails(t As Integer) As CellNodes

            Dim result = New CellNodes With {
                .N1 = data.CellList(t).V1,
                .N2 = data.CellList(t).V2,
                .N3 = data.CellList(t).V3
            }

            Return result

        End Function

        ''' <summary>
        ''' Returns the side types of the given cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Private Function GetSideTypes(t As Integer) As CellSideTypes

            Dim result = New CellSideTypes With {
                .S1 = data.CellList(t).Edge1.SideType,
                .S2 = data.CellList(t).Edge2.SideType,
                .S3 = data.CellList(t).Edge3.SideType
            }

            Return result

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
        ''' Identifies the side type of the adjoining edge
        ''' </summary>
        ''' <param name="configuration"></param>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Private Function JoiningSide(configuration As Integer, t As Integer) As SideType

            Select Case configuration
                Case 1, 4
                    Return data.CellList(t).Edge3.SideType
                Case 2, 5
                    Return data.CellList(t).Edge1.SideType
                Case 3, 6
                    Return data.CellList(t).Edge2.SideType
                Case Else
                    Throw New Exception()
            End Select

        End Function

        ''' <summary>
        ''' Sets target vertex of the adjoining cell that will be tested to see if it lies inside or
        ''' outside the circumcircle
        ''' </summary>
        ''' <param name="configuration"></param>
        ''' <param name="adjacentCells"></param>
        Private Function ProcessAdjacent(configuration As Integer, t_adj As Cell) As Integer
            'Sets np for adjacent cell
            Dim np As Integer
            Dim t2 = data.CellList.IndexOf(t_adj)

            Select Case configuration
                Case 1, 6
                    np = t_adj.V2
                Case 2, 4
                    np = t_adj.V3
                Case 3, 5
                    np = t_adj.V1
                Case Else
                    Throw New Exception()
            End Select

            Return np

        End Function

        ''' <summary>
        ''' Flips the adjoining edge of a pair of cells
        ''' </summary>
        ''' <param name="configuration"></param>
        ''' <param name="factory"></param>
        Private Sub FlipCells(configuration As Integer, t As Integer, t2 As Integer, np As Integer, n As CellNodes)

            Dim s1 = GetSideTypes(t)
            Dim s2 = GetSideTypes(t2)

            Select Case configuration
                Case 1
                    factory.UpdateCell(t, np, n.N2, n.N3, s1.S1, SideType.none, s2.S1)
                    factory.UpdateCell(t2, np, n.N3, n.N1, s1.S2, s2.S3, SideType.none)
                Case 2
                    factory.UpdateCell(t, np, n.N3, n.N1, s1.S2, SideType.none, s2.S2)
                    factory.UpdateCell(t2, np, n.N1, n.N2, s1.S3, s2.S1, SideType.none)
                Case 3
                    factory.UpdateCell(t, np, n.N1, n.N2, s1.S3, SideType.none, s2.S3)
                    factory.UpdateCell(t2, np, n.N2, n.N3, s1.S1, s2.S2, SideType.none)
                Case 4
                    factory.UpdateCell(t, np, n.N2, n.N3, s1.S1, SideType.none, s2.S2)
                    factory.UpdateCell(t2, np, n.N3, n.N1, s1.S2, s2.S1, SideType.none)
                Case 5
                    factory.UpdateCell(t, np, n.N3, n.N1, s1.S2, SideType.none, s2.S3)
                    factory.UpdateCell(t2, np, n.N1, n.N2, s1.S3, s2.S2, SideType.none)
                Case 6
                    factory.UpdateCell(t, np, n.N1, n.N2, s1.S3, SideType.none, s2.S1)
                    factory.UpdateCell(t2, np, n.N2, n.N3, s1.S1, s2.S3, SideType.none)
                Case Else
                    Throw New Exception
            End Select
        End Sub
    End Class
End Namespace

