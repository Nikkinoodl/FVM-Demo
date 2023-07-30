Imports Core.Interfaces
Imports System.Numerics

Namespace Services
    Public Class Redistributor : Implements IRedistributor

        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

        Public Sub Redistribute(farfield As Object) Implements IRedistributor.Redistribute
            'Reallocate nodes on the boundary edges to provide a more even distribution
            'this is intended to be done after refining the grid and requires smoothing afterwards

            Dim edges = New String() {"top", "bottom", "right", "left"}

            'Loop though each edge in parallel
            Parallel.ForEach(edges, Sub(edge)

                                        'Get the ordered (by x or y, according to side) list of nodes
                                        Dim sideNodes = data.EdgeBoundary(edge, farfield)

                                        'Get the number of nodes to be redistributed
                                        Dim nodeCount = sideNodes.Count()

                                        'using a simple harmonic distribution
                                        Dim nodeFraction = Function(n) n / (nodeCount + 1)
                                        Dim lengthFraction = Function(n) nodeFraction(n) + 0.01 * Math.Cos(Math.PI * nodeFraction(n))

                                        'Loop through nodes on each edge in turn
                                        For Each node In sideNodes

                                            Dim id = node.Id
                                            Dim i = sideNodes.IndexOf(node) + 1

                                            Select Case edge
                                                Case "top", "bottom"
                                                    data.Nodelist(id).R = New Vector2(lengthFraction(i) * farfield.width, data.Nodelist(id).R.Y)
                                                Case "left", "right"
                                                    data.Nodelist(id).R = New Vector2(data.Nodelist(id).R.X, lengthFraction(i) * farfield.height)
                                                Case Else
                                                    Throw New Exception
                                            End Select

                                        Next
                                    End Sub)
        End Sub

    End Class
End Namespace