Imports System.Numerics
Imports Core.Domain
Imports Core.Interfaces

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

                                                Dim n1 = t.V1
                                                Dim n2 = t.V2
                                                Dim n3 = t.V3

                                                Dim r1 As Vector2 = data.NodeV(n1).R
                                                Dim r2 As Vector2 = data.NodeV(n2).R
                                                Dim r3 As Vector2 = data.NodeV(n3).R

                                                't center
                                                t.R = New Vector2((r1.X + r2.X + r3.X) / 3, (r1.Y + r2.Y + r3.Y) / 3)

                                                'calculate length metrics
                                                t.Edge1.L = Vector2.Distance(r3, r2)
                                                t.Edge2.L = Vector2.Distance(r1, r3)
                                                t.Edge3.L = Vector2.Distance(r2, r1)

                                            End Sub)
        End Sub

        Public Sub CalculateLengthsSquares() Implements ICellCalculator.CalculateLengthsSquares

            Parallel.ForEach(data.CellList, Sub(t)

                                                Dim n1 = t.V1
                                                Dim n2 = t.V2
                                                Dim n3 = t.V3
                                                Dim n4 = t.V4

                                                Dim r1 As Vector2 = data.NodeV(n1).R
                                                Dim r2 As Vector2 = data.NodeV(n2).R
                                                Dim r3 As Vector2 = data.NodeV(n3).R
                                                Dim r4 As Vector2 = data.NodeV(n4).R

                                                't center
                                                t.R = New Vector2((r1.X + r2.X + r3.X + r4.X) / 4, (r1.Y + r2.Y + r3.Y + r4.Y) / 4)

                                                'calculate length metrics
                                                t.Edge1.L = Vector2.Distance(r1, r2)
                                                t.Edge2.L = Vector2.Distance(r2, r3)
                                                t.Edge3.L = Vector2.Distance(r3, r4)
                                                t.Edge4.L = Vector2.Distance(r4, r1)

                                            End Sub)
        End Sub

        ''' <summary>
        ''' Calculates mid point of each cell edge
        ''' </summary>
        Public Sub CalculateMidPoints() Implements ICellCalculator.CalculateMidPoints

            Parallel.ForEach(data.CellList, Sub(cell)

                                                Dim n1 = cell.V1
                                                Dim n2 = cell.V2
                                                Dim n3 = cell.V3

                                                Dim r1 As Vector2 = data.NodeV(n1).R
                                                Dim r2 As Vector2 = data.NodeV(n2).R
                                                Dim r3 As Vector2 = data.NodeV(n3).R

                                                cell.Edge1.R = Vector2.Add(r2, r3) * 0.5
                                                cell.Edge2.R = Vector2.Add(r1, r3) * 0.5
                                                cell.Edge3.R = Vector2.Add(r1, r2) * 0.5

                                            End Sub)
        End Sub

        Public Sub CalculateMidPointsSquares() Implements ICellCalculator.CalculateMidPointsSquares

            Parallel.ForEach(data.CellList, Sub(cell)

                                                Dim n1 = cell.V1
                                                Dim n2 = cell.V2
                                                Dim n3 = cell.V3
                                                Dim n4 = cell.V4

                                                Dim r1 As Vector2 = data.NodeV(n1).R
                                                Dim r2 As Vector2 = data.NodeV(n2).R
                                                Dim r3 As Vector2 = data.NodeV(n3).R
                                                Dim r4 As Vector2 = data.NodeV(n4).R

                                                cell.Edge1.R = Vector2.Add(r1, r2) * 0.5
                                                cell.Edge2.R = Vector2.Add(r2, r3) * 0.5
                                                cell.Edge3.R = Vector2.Add(r3, r4) * 0.5
                                                cell.Edge4.R = Vector2.Add(r4, r1) * 0.5

                                            End Sub)
        End Sub

        ''' <summary>
        ''' Calculates the inverse area of each triangular cell using Hero's rule
        ''' </summary>
        Public Sub CalculateAreas() Implements ICellCalculator.CalculateAreas

            Parallel.ForEach(data.CellList, Sub(t)

                                                Dim area As Single
                                                Dim p As Single

                                                p = (t.Edge1.L + t.Edge2.L + t.Edge3.L) * 0.5
                                                area = Math.Sqrt(p * (p - t.Edge1.L) * (p - t.Edge2.L) * (p - t.Edge3.L))

                                                t.AreaI = 1 / area

                                            End Sub)
        End Sub

        ''' <summary>
        ''' Calculates the inverse area of a rectangular grid cell
        ''' </summary>
        Public Sub CalculateAreasSquares() Implements ICellCalculator.CalculateAreasSquares

            Parallel.ForEach(data.CellList, Sub(t)

                                                t.AreaI = 1 / (t.Edge1.L * t.Edge2.L)

                                            End Sub)
        End Sub


        ''' <summary>
        ''' Calculates the vector from a cell center to each edge center
        ''' </summary>
        Public Sub CalculateFaceVectors() Implements ICellCalculator.CalculateFaceVectors

            Parallel.ForEach(data.CellList, Sub(t)
                                                For Each e As Edge In t.Edges

                                                    'face vectors calculated by subtracting vector to cell center 
                                                    e.Rp = Vector2.Subtract(e.R, t.R)

                                                Next
                                            End Sub)
        End Sub

        ''' <summary>
        ''' Calculates the outward facing normals for each edge in each cell. Note that this 
        ''' assumes all cell nodes are assigned in a clockwise order.
        ''' </summary>
        Public Sub CalculateFaceNormals() Implements ICellCalculator.CalculateFaceNormals
            Parallel.ForEach(data.CalcCells, Sub(t)

                                                 'Position vectors for vertices
                                                 Dim r1 As Vector2 = data.NodeV(t.V1).R
                                                 Dim r2 As Vector2 = data.NodeV(t.V2).R
                                                 Dim r3 As Vector2 = data.NodeV(t.V3).R

                                                 'Vector representing sides, eg S3 = vector V2 - vector V1
                                                 Dim s1 As Vector2 = Vector2.Subtract(r3, r2)
                                                 Dim s2 As Vector2 = Vector2.Subtract(r1, r3)
                                                 Dim s3 As Vector2 = Vector2.Subtract(r2, r1)

                                                 'Rotate counter clockwise by 90 degrees by (x, y) => (-y, x)
                                                 'and normalize to unit vector
                                                 t.Edge1.N = Vector2.Normalize(New Vector2(-s1.Y, s1.X))
                                                 t.Edge2.N = Vector2.Normalize(New Vector2(-s2.Y, s2.X))
                                                 t.Edge3.N = Vector2.Normalize(New Vector2(-s3.Y, s3.X))

                                             End Sub)
        End Sub

        Public Sub CalculateFaceNormalsSquares() Implements ICellCalculator.CalculateFaceNormalsSquares

            Parallel.ForEach(data.CellList, Sub(t)

                                                'square cell edges are always oriented in W N E S order
                                                t.Edge1.N = New Vector2(-1, 0)
                                                t.Edge2.N = New Vector2(0, 1)
                                                t.Edge3.N = New Vector2(1, 0)
                                                t.Edge4.N = New Vector2(0, -1)

                                            End Sub)
        End Sub

    End Class
End Namespace