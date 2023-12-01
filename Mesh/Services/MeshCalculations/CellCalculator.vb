Imports System.Numerics
Imports Core.Common
Imports Core.Data
Imports Core.Domain
Imports Core.Interfaces
Imports Mesh.Services.SharedUtilities

Namespace Services
    Public Class CellCalculator : Implements ICellCalculator

        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

        ''' <summary>
        ''' Finds the center of each cell and calculates the length of each cell edge
        ''' </summary>
        Public Sub CalculateLengths() Implements ICellCalculator.CalculateLengths

            Parallel.ForEach(data.CellList, Sub(t)

                                                CalculateLength(t)

                                            End Sub)

        End Sub

        ''' <summary>
        ''' Calculates the length and cell center of a single cell
        ''' </summary>
        ''' <param name="t"></param>
        Public Sub CalculateLength(t As Cell) Implements ICellCalculator.CalculateLength

            Dim n() As Integer = GetNodes(t)
            Dim nSides As Integer = GetNumberSides(t)
            Dim r(nSides) As Vector2

            'calculate cell center
            t.R = Vector2.Zero

            'find position vectors for all cell nodes
            For i As Integer = 0 To nSides - 1

                r(i) = data.NodeV(n(i)).R

                'cell center
                t.R += r(i)

            Next

            'finalize cell center position
            t.R /= nSides

            'triangles get special processing because of side naming convention
            If t.CellType = CellType.triangle Then

                t.Edge1.L = Vector2.Distance(r(2), r(1))
                t.Edge2.L = Vector2.Distance(r(0), r(2))
                t.Edge3.L = Vector2.Distance(r(1), r(0))

                'calculate edge vectors
                t.Edge1.Lv = r(2) - r(1)
                t.Edge2.Lv = r(0) - r(2)
                t.Edge3.Lv = r(1) - r(0)

            Else

                'all other cells are processed here
                For i As Integer = 0 To nSides - 1

                    'edge vectors
                    If i < nSides - 1 Then

                        t.Edges(i).Lv = r(i + 1) - r(i)

                    Else     'closing the loop on the cell

                        t.Edges(i).Lv = r(0) - r(i)

                    End If

                    'lengths
                    t.Edges(i).L = t.Edges(i).Lv.Length

                Next

            End If

        End Sub

        ''' <summary>
        ''' Calculates mid point of each cell edge
        ''' </summary>
        Public Sub CalculateMidPoints() Implements ICellCalculator.CalculateMidPoints

            Parallel.ForEach(data.CellList, Sub(t)

                                                CalculateMidPoint(t)

                                            End Sub)

        End Sub

        ''' <summary>
        ''' Calculates the mid point of each edge of a single cell
        ''' </summary>
        Public Sub CalculateMidPoint(t As Cell) Implements ICellCalculator.CalculateMidPoint

            Dim n() As Integer = GetNodes(t)
            Dim nSides As Integer = GetNumberSides(t)
            Dim r(nSides) As Vector2

            'first find position vectors for all cell nodes
            For i As Integer = 0 To nSides - 1

                r(i) = data.NodeV(n(i)).R

            Next

            If nSides = 3 Then

                t.Edge1.R = (r(1) + r(2)) * 0.5
                t.Edge2.R = (r(0) + r(2)) * 0.5
                t.Edge3.R = (r(0) + r(1)) * 0.5

            Else           'all other cells use standard side naming

                For i As Integer = 0 To nSides - 1

                    'calculate mid points
                    If i < nSides - 1 Then

                        t.Edges(i).R = (r(i) + r(i + 1)) * 0.5

                    Else  'close the loop on the cell

                        t.Edges(i).R = (r(i) + r(0)) * 0.5

                    End If

                Next

            End If

        End Sub

        ''' <summary>
        ''' Calculates the inverse area of each cell using linear algebra
        ''' </summary>
        Public Sub CalculateAreas() Implements ICellCalculator.CalculateAreas

            Parallel.ForEach(data.CellList, Sub(t)

                                                Dim nSides As Integer = GetNumberSides(t)
                                                Dim n() As Integer = GetNodes(t)
                                                Dim r(nSides) As Vector2

                                                Dim area As Single = 0
                                                Dim p As Single = 0

                                                'first find position vectors for all cell nodes
                                                For i As Integer = 0 To nSides - 1

                                                    r(i) = data.NodeV(n(i)).R

                                                Next

                                                'calculate all areas using linear algebra
                                                For i As Integer = 0 To nSides - 1

                                                    'edge vectors
                                                    If i < nSides - 1 Then

                                                        area += r(i).X * r(i + 1).Y - r(i).Y * r(i + 1).X

                                                    Else  'close the loop on the cell

                                                        area += r(i).X * r(0).Y - r(i).Y * r(0).X

                                                    End If

                                                Next


                                                t.AreaI = 1 / (0.5 * Math.Abs(area))


                                            End Sub)

        End Sub

        ''' <summary>
        ''' Calculates the vector from a cell center to each edge center
        ''' </summary>
        Public Sub CalculateFaceVectors() Implements ICellCalculator.CalculateFaceVectors

            Parallel.ForEach(data.CellList, Sub(t)

                                                For Each e As Edge In t.Edges

                                                    'face vectors calculated by subtracting vector to cell center 
                                                    e.Rp = e.R - t.R

                                                Next

                                            End Sub)

        End Sub

        ''' <summary>
        ''' Calculates the outward facing normals for each edge in each cell. Note that this 
        ''' assumes all cell nodes are assigned in a clockwise order.
        ''' </summary>
        Public Sub CalculateFaceNormals() Implements ICellCalculator.CalculateFaceNormals
            Parallel.ForEach(data.CalcCells, Sub(t)

                                                 'Rotate counter clockwise by 90 degrees by (x, y) => (-y, x)
                                                 'and normalize to unit vector
                                                 For Each e As Edge In t.Edges

                                                     e.N = Vector2.Normalize(New Vector2(-e.Lv.Y, e.Lv.X))

                                                 Next

                                             End Sub)
        End Sub


    End Class
End Namespace