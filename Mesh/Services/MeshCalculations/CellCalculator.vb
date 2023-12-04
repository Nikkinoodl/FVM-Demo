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
            Dim nSides = n.Length()
            Dim r() = data.GetPositionVectorsAsArray(n)

            CalculateCellCenter(t, r, nSides)

            'triangles get special processing because of side naming convention
            If t.CellType = CellType.triangle Then

                'calculate edge vectors
                t.Edge1.Lv = r(2) - r(1)
                t.Edge2.Lv = r(0) - r(2)
                t.Edge3.Lv = r(1) - r(0)

                For i As Integer = 0 To nSides - 1

                    'lengths
                    t.Edges(i).L = t.Edges(i).Lv.Length()

                Next

            Else

                'use a shared utility sub for non-triangular cells
                CalculateEdgeLengths(t, r, nSides)

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
            Dim nSides = n.Length()
            Dim r() = data.GetPositionVectorsAsArray(n)

            'triangles get special processing because of side naming convention
            If nSides = 3 Then

                FindTriangleMidPoints(t, r)

                'For i As Integer = 0 To nSides - 1

                '    'use shared utility sub for triangle calcs
                '    t.Edges(i).R = FindMidPoint(t.Edges(i), r)

                'Next

            Else

                'use shared utility sub for non-triangle calcs
                FindMidPoints(t, r, nSides)

            End If

        End Sub

        ''' <summary>
        ''' Calculates the inverse area of each cell using linear algebra
        ''' </summary>
        Public Sub CalculateAreas() Implements ICellCalculator.CalculateAreas

            Parallel.ForEach(data.CellList, Sub(t)

                                                Dim n() As Integer = GetNodes(t)
                                                Dim nSides = n.Length()
                                                Dim r() = data.GetPositionVectorsAsArray(n)

                                                CalculateCellArea(t, r, nSides)

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