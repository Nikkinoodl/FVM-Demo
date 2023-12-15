Imports Core.Common
Imports Core.Interfaces
Imports System.Numerics

Namespace Services
    Public Class Redistributor : Implements IRedistributor

        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

        ''' <summary>
        ''' Reallocates nodes on the farfield boundary edges
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub Redistribute(farfield As Farfield) Implements IRedistributor.Redistribute

            Dim edges = New String() {"top", "bottom", "right", "left"}

            'Loop though each edge in parallel
            Parallel.ForEach(edges, Sub(edge)

                                        'get the ordered (by x or y, according to side) list of nodes
                                        Dim sideNodes = data.EdgeBoundary(edge, farfield)

                                        'get the number of nodes to be redistributed
                                        Dim nodeCount = sideNodes.Count()

                                        'using a simple harmonic distribution
                                        Dim nodeFraction = Function(n) n / (nodeCount + 1)
                                        Dim lengthFraction = Function(n) nodeFraction(n) + 0.01 * Math.Cos(Math.PI * nodeFraction(n))

                                        'loop through nodes on each edge in turn
                                        For Each node In sideNodes

                                            Dim id = node.Id
                                            Dim i = sideNodes.IndexOf(node) + 1

                                            Select Case edge
                                                Case "top", "bottom"
                                                    data.Nodelist(id).R = New Vector2(lengthFraction(i) * farfield.Width, data.Nodelist(id).R.Y)
                                                Case "left", "right"
                                                    data.Nodelist(id).R = New Vector2(data.Nodelist(id).R.X, lengthFraction(i) * farfield.Height)
                                                Case Else
                                                    Throw New Exception
                                            End Select

                                        Next
                                    End Sub)
        End Sub

    End Class
End Namespace