Imports Core.Interfaces
Imports Core.Domain
Imports System.Numerics

Namespace Services
    Public Class GridSmoother : Implements IGridSmoother

        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

        ''' <summary>
        ''' Evens out the distribution of irregular triangular grid nodes
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

            'Takes each node in turn, finds the cells that contain this node and re-centers the node
            'in relation to the other nodes of these cells

            'Loop through each node in parallel
            Parallel.ForEach(data.SmoothNode(), Sub(node)
                                                    'set variables and counters
                                                    Dim n = 0
                                                    Dim thisr As New Vector2

                                                    'find the cells that contain this node
                                                    For Each cell In data.SmoothCell(node.Id)
                                                        n += 1
                                                        Dim n1 = cell.V1
                                                        Dim n2 = cell.V2
                                                        Dim n3 = cell.V3

                                                        'aggregate the position vector of all nodes
                                                        thisr = data.Nodelist(n1).R + data.Nodelist(n2).R + data.Nodelist(n3).R + thisr

                                                    Next

                                                    'Update node position vector
                                                    data.Nodelist(node.Id).R = thisr / (3 * n)

                                                End Sub)
            'Next
        End Sub
    End Class
End Namespace