Imports Core.Interfaces
Imports Core.Domain
Imports Mesh.Services.SharedUtilities
Imports System.Numerics

Namespace Services
    Public Class GridSmoother : Implements IGridSmoother

        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

        ''' <summary>
        ''' Evens out the distribution of grid nodes
        ''' </summary>
        Public Sub SmoothGrid() Implements IGridSmoother.SmoothGrid

            ' Implements Laplace smoothing
            ' i.e. new position vector of np becomes the average (x, y) of A, B, C, D and np
            '
            '                                   A
            '                                  /|\ 
            '                                 / | \ 
            '                                /  |  \
            '                               /   |   \
            '                              /    |    \
            '                             /     |     \
            '                            /      |      \
            '                           /       |       \
            '                          /        |        \
            '                         /         |         \
            '                      B  --------- np -------- C
            '                         \         |          /
            '                          \        |         /
            '                           \       |        /
            '                            \      |       /
            '                             \     |      /
            '                              \    |     /
            '                               \   |    /
            '                                \  |   /
            '                                 \ |  /  
            '                                  \| / 
            '                                   D            
            '

            'Takes each node in turn, finds the cluster of cells that contain this node and re-centers
            'the node in relation to the cluster. This above diagram shows four adjoinging triangles,
            'but the method can be used for any type of cells.

            'loop through each node in parallel
            Parallel.ForEach(data.SmoothNode(), Sub(node)

                                                    'get cell cluster and count
                                                    Dim cluster = data.SmoothCell(node.Id)
                                                    Dim cellCount = cluster.Count()

                                                    'cell average position vector
                                                    Dim cellR(cellCount) As Vector2

                                                    'cluster average position vector
                                                    Dim aggregateR As New Vector2

                                                    Dim i = 0

                                                    'find the cells that contain this node
                                                    For Each cell In cluster

                                                        cellR(i) = Vector2.Zero

                                                        Dim cellNodes = GetNodesAsArray(cell)
                                                        Dim r = data.GetPositionVectorsAsArray(cellNodes)
                                                        Dim size = cellNodes.Length

                                                        'aggregate the position vector of all nodes
                                                        For j As Integer = 0 To size - 1

                                                            cellR(i) += r(j)

                                                        Next

                                                        'average by the number of nodes in cell
                                                        cellR(i) /= size

                                                        'add to the cluster aggregate
                                                        aggregateR += cellR(i)

                                                        i += 1

                                                    Next

                                                    'average across the number of cells in the cluster
                                                    node.R = aggregateR / cellCount

                                                End Sub)
            'Next
        End Sub
    End Class
End Namespace