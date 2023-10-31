Imports System.Numerics
Imports Core.Common
Imports Core.Domain
Imports Core.Interfaces

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

            Parallel.ForEach(data.CalcCells, Sub(cell)

                                                 Dim t As Integer = data.CellList.IndexOf(cell)

                                                 Dim n1 As Integer = cell.V1
                                                 Dim n2 As Integer = cell.V2
                                                 Dim n3 As Integer = cell.V3
                                                 Dim n4 As Integer?

                                                 Dim e1 As Edge = cell.Edge1
                                                 Dim e2 As Edge = cell.Edge2
                                                 Dim e3 As Edge = cell.Edge3
                                                 Dim e4 As Edge

                                                 Dim r1 As Vector2 = e1.R
                                                 Dim r2 As Vector2 = e2.R
                                                 Dim r3 As Vector2 = e3.R
                                                 Dim r4 As Vector2


                                                 'Use a list of tuples to hold node pairs
                                                 Dim nodePairs As New List(Of (nA As Integer, nB As Integer, r As Vector2, e As Edge))

                                                 If cell.V4 IsNot Nothing Then      'quad cell

                                                     n4 = cell.V4
                                                     e4 = cell.Edge4
                                                     r4 = e4.R

                                                     nodePairs.Add((n2, n1, r1, e1))
                                                     nodePairs.Add((n3, n2, r2, e2))
                                                     nodePairs.Add((n4, n3, r3, e3))
                                                     nodePairs.Add((n1, n4, r4, e4))

                                                 Else                               'triangle cell

                                                     nodePairs.Add((n3, n2, r1, e1))
                                                     nodePairs.Add((n1, n3, r2, e2))
                                                     nodePairs.Add((n2, n1, r3, e3))

                                                 End If

                                                 For Each nodePair In nodePairs

                                                     'get the matching cell Id and edge
                                                     Dim result As (t_adj As Integer?, sideName As SideName?)

                                                     result = data.AdjacentCellEdge(nodePair, cell.Id)

                                                     If result.t_adj IsNot Nothing Then

                                                         'update this edge with neighbor's details if one exists
                                                         nodePair.e.AdjoiningCell = result.t_adj
                                                         nodePair.e.AdjoiningEdge = result.sideName

                                                         'calculate position vectors and ratios
                                                         nodePair.e.Rk = Vector2.Subtract(data.CellList(result.t_adj).R, cell.R)

                                                         'determine weighting. Note that this will be 0.5 on regular shaped cells,
                                                         'and 1.0 on boundary cells
                                                         nodePair.e.W = nodePair.e.Rp.Length() / nodePair.e.Rk.Length()

                                                         nodePair.e.Lk = nodePair.e.L / nodePair.e.Rk.Length()

                                                         cell.Lk += nodePair.e.Lk

                                                     Else
                                                         Debug.WriteLine("Error matching edges at cell: " & cell.Id)
                                                     End If

                                                 Next

                                             End Sub)
        End Sub

    End Class
End Namespace