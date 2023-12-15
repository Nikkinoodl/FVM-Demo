Imports Core.Common
Imports Core.Domain
Imports Core.Interfaces
Imports Mesh.Services.SharedUtilities

Namespace Services

    ''' <summary>
    ''' Methods that complete the grid. They must be performed after all elements have been constructed
    ''' </summary>
    Public Class MeshPrecalc : Implements IMeshPrecalc

        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

        ''' <summary>
        ''' Find the adjoining element/face for cell element/face
        ''' </summary>
        Public Sub FindAjoiningCells() Implements IMeshPrecalc.FindAdjoiningCells

            Parallel.ForEach(data.CellList, Sub(t)

                                                'get cell nodes and calculate number of sides
                                                Dim n = GetNodesAsArray(t)
                                                Dim nSides = n.Length

                                                'edges
                                                Dim e = t.Edges.ToArray

                                                'use a list of tuples to hold node pairs for this cell
                                                Dim nodePairs As New List(Of (nA As Integer, nB As Integer, targetEdge As Edge))

                                                If nSides = 3 Then  'special processing for triangles

                                                    nodePairs.Add((n(2), n(1), e(0)))    'side1 spanned by nodes N3 and N2
                                                    nodePairs.Add((n(0), n(2), e(1)))    'side2 spanned by nodes N1 and N3
                                                    nodePairs.Add((n(1), n(0), e(2)))    'side3 spanned by nodes N2 and N1

                                                ElseIf nSides > 3 Then   'all other cells

                                                    For i As Integer = 0 To nSides - 1

                                                        'node pairs
                                                        If i = 0 Then    'close the loop on the cell

                                                            nodePairs.Add((n(i), n(nSides - 1), e(nSides - 1)))

                                                        Else

                                                            nodePairs.Add((n(i), n(i - 1), e(i - 1)))

                                                        End If

                                                    Next

                                                Else        'border cells

                                                    nodePairs.Add((n(1), n(0), e(0)))

                                                End If

                                                For Each nodePair In nodePairs

                                                    'get the matching cell index and edge
                                                    Dim result As (adjacentCell As Integer?, adjacentSide As SideName?)

                                                    result = data.AdjacentCellEdge(nodePair, t.Id)

                                                    'each cell should have at least one adjacent cell
                                                    If result.adjacentCell IsNot Nothing Then

                                                        'update this nodepair edge with neighbor's details if one exists
                                                        nodePair.targetEdge.AdjoiningCell = result.adjacentCell
                                                        nodePair.targetEdge.AdjoiningEdge = result.adjacentSide

                                                        'skip any border cells at this point
                                                        If t.BorderCell = True Then Continue For

                                                        'vector between cell centers
                                                        nodePair.targetEdge.Rk = data.CellList(result.adjacentCell).R - t.R

                                                        'weighting will be 0.5 on regular shaped cells and 1.0 on boundary cells
                                                        nodePair.targetEdge.W = nodePair.targetEdge.Rp.Length() / nodePair.targetEdge.Rk.Length()

                                                        'ratio of edge length to distance vetween cell centers
                                                        nodePair.targetEdge.Lk = nodePair.targetEdge.L / nodePair.targetEdge.Rk.Length()

                                                        'the sum of Lk's over the cell is used in the FVM pressure calc
                                                        t.Lk += nodePair.targetEdge.Lk

                                                    Else

                                                        Debug.WriteLine("Error matching edges at cell: " & t.Id)

                                                    End If

                                                Next

                                            End Sub)
        End Sub

    End Class
End Namespace