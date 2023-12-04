Imports System.Numerics
Imports Core.Common
Imports Core.Domain
Imports Core.Interfaces
Imports Mesh.Services.SharedUtilities

Namespace Services
    Public Class MeshPrecalc : Implements IMeshPrecalc

        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

        ''' <summary>
        ''' Find the adjoining element/face for cell element/face
        ''' </summary>
        Public Sub FindAjoiningCells() Implements IMeshPrecalc.FindAdjoiningCells

            Parallel.ForEach(data.CalcCells, Sub(t)

                                                 'get cell nodes and calculate number of sides
                                                 Dim n = GetNodes(t)
                                                 Dim nSides = n.Length

                                                 'position vectors
                                                 Dim r = data.GetPositionVectorsAsArray(n)

                                                 'edges
                                                 Dim e = t.Edges.ToArray

                                                 'use a list of tuples to hold node pairs for this cell
                                                 Dim nodePairs As New List(Of (nA As Integer, nB As Integer, r As Vector2, e As Edge))

                                                 If nSides = 3 Then  'special processing for triangles

                                                     nodePairs.Add((n(2), n(1), r(0), e(0)))    'side1
                                                     nodePairs.Add((n(0), n(2), r(1), e(1)))    'side2
                                                     nodePairs.Add((n(1), n(0), r(2), e(2)))    'side3

                                                 Else               'all other cells

                                                     For i As Integer = 0 To nSides - 1

                                                         'node pairs
                                                         If i = 0 Then    'close the loop on the cell

                                                             nodePairs.Add((n(i), n(nSides - 1), r(nSides - 1), e(nSides - 1)))

                                                         Else

                                                             nodePairs.Add((n(i), n(i - 1), r(i - 1), e(i - 1)))

                                                         End If

                                                     Next

                                                 End If

                                                 For Each nodePair In nodePairs

                                                     'get the matching cell index and edge
                                                     Dim result As (t_adj As Integer?, sideName As SideName?)

                                                     result = data.AdjacentCellEdge(nodePair, t.Id)

                                                     If result.t_adj IsNot Nothing Then

                                                         'update this edge with neighbor's details if one exists
                                                         nodePair.e.AdjoiningCell = result.t_adj
                                                         nodePair.e.AdjoiningEdge = result.sideName

                                                         'calculate position vectors and ratios
                                                         nodePair.e.Rk = data.CellList(result.t_adj).R - t.R

                                                         'determine weighting. Note that this will be 0.5 on regular shaped cells,
                                                         'and 1.0 on boundary cells
                                                         nodePair.e.W = nodePair.e.Rp.Length() / nodePair.e.Rk.Length()

                                                         nodePair.e.Lk = nodePair.e.L / nodePair.e.Rk.Length()

                                                         t.Lk += nodePair.e.Lk

                                                     Else

                                                         Debug.WriteLine("Error matching edges at cell: " & t.Id)

                                                     End If

                                                 Next

                                             End Sub)
        End Sub

    End Class
End Namespace