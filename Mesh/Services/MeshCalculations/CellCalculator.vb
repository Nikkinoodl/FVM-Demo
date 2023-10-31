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

                                                Dim n4 As Integer?
                                                Dim r4 As Vector2 = Vector2.Zero

                                                Dim r1 As Vector2 = data.NodeV(n1).R
                                                Dim r2 As Vector2 = data.NodeV(n2).R
                                                Dim r3 As Vector2 = data.NodeV(n3).R

                                                If t.V4 IsNot Nothing Then   'quad cell

                                                    n4 = t.V4
                                                    r4 = data.NodeV(n4).R

                                                    't center
                                                    t.R = New Vector2((r1.X + r2.X + r3.X + r4.X) / 4, (r1.Y + r2.Y + r3.Y + r4.Y) / 4)

                                                    'calculate length metrics
                                                    t.Edge1.L = Vector2.Distance(r1, r2)
                                                    t.Edge2.L = Vector2.Distance(r2, r3)
                                                    t.Edge3.L = Vector2.Distance(r3, r4)
                                                    t.Edge4.L = Vector2.Distance(r4, r1)

                                                    'calculate edge vectors
                                                    t.Edge1.Lv = Vector2.Subtract(r1, r2)
                                                    t.Edge2.Lv = Vector2.Subtract(r2, r3)
                                                    t.Edge3.Lv = Vector2.Subtract(r3, r4)
                                                    t.Edge4.Lv = Vector2.Subtract(r4, r1)


                                                Else   'triangular cell

                                                    't center
                                                    t.R = New Vector2((r1.X + r2.X + r3.X) / 3, (r1.Y + r2.Y + r3.Y) / 3)

                                                    'calculate length metrics
                                                    t.Edge1.L = Vector2.Distance(r3, r2)
                                                    t.Edge2.L = Vector2.Distance(r1, r3)
                                                    t.Edge3.L = Vector2.Distance(r2, r1)

                                                    'calculate edge vectors
                                                    t.Edge1.Lv = Vector2.Subtract(r3, r2)
                                                    t.Edge2.Lv = Vector2.Subtract(r1, r3)
                                                    t.Edge3.Lv = Vector2.Subtract(r2, r1)

                                                End If
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

                                                Dim n4 As Integer?
                                                Dim r4 As Vector2 = Vector2.Zero

                                                If cell.V4 IsNot Nothing Then       'quad cell

                                                    n4 = cell.V4
                                                    r4 = data.NodeV(n4).R

                                                    cell.Edge1.R = Vector2.Add(r1, r2) * 0.5
                                                    cell.Edge2.R = Vector2.Add(r2, r3) * 0.5
                                                    cell.Edge3.R = Vector2.Add(r3, r4) * 0.5
                                                    cell.Edge4.R = Vector2.Add(r4, r1) * 0.5

                                                Else                                'triangle

                                                    cell.Edge1.R = Vector2.Add(r2, r3) * 0.5
                                                    cell.Edge2.R = Vector2.Add(r1, r3) * 0.5
                                                    cell.Edge3.R = Vector2.Add(r1, r2) * 0.5

                                                End If

                                            End Sub)
        End Sub

        ''' <summary>
        ''' Calculates the inverse area of each cell using Heron's rule for triangles and node vectors for quads
        ''' </summary>
        Public Sub CalculateAreas() Implements ICellCalculator.CalculateAreas

            Parallel.ForEach(data.CellList, Sub(t)

                                                Dim area As Single
                                                Dim p As Single

                                                If t.Edge4 IsNot Nothing Then

                                                    Dim nodes As CellNodes = GetNodeDetails(t)
                                                    Dim pV As CellNodeVectors = GetPositionVectors(nodes)

                                                    area = 0.5 * ((pV.R1.X * pV.R2.Y + pV.R2.X * pV.R3.Y + pV.R3.X * pV.R4.Y + pV.R4.X * pV.R1.Y) _
                                                                - (pV.R1.Y * pV.R2.X + pV.R2.Y * pV.R3.X + pV.R3.Y * pV.R4.X + pV.R4.Y * pV.R1.X))

                                                Else


                                                    p = (t.Edge1.L + t.Edge2.L + t.Edge3.L) * 0.5
                                                    area = Math.Sqrt(p * (p - t.Edge1.L) * (p - t.Edge2.L) * (p - t.Edge3.L))

                                                End If

                                                t.AreaI = 1 / Math.Abs(area)


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
                                                 Dim r4 As Vector2 = Vector2.Zero

                                                 'Edge vectors
                                                 Dim s1 As Vector2
                                                 Dim s2 As Vector2
                                                 Dim s3 As Vector2
                                                 Dim s4 As Vector2

                                                 If t.V4 IsNot Nothing Then

                                                     r4 = data.NodeV(t.V4).R

                                                     s1 = Vector2.Subtract(r2, r1)
                                                     s2 = Vector2.Subtract(r3, r2)
                                                     s3 = Vector2.Subtract(r4, r3)
                                                     s4 = Vector2.Subtract(r1, r4)

                                                     t.Edge4.N = Vector2.Normalize(New Vector2(-s4.Y, s4.X))

                                                 Else

                                                     'Vector representing sides, eg S3 = vector V2 - vector V1
                                                     s1 = Vector2.Subtract(r3, r2)
                                                     s2 = Vector2.Subtract(r1, r3)
                                                     s3 = Vector2.Subtract(r2, r1)

                                                 End If

                                                 'Rotate counter clockwise by 90 degrees by (x, y) => (-y, x)
                                                 'and normalize to unit vector
                                                 t.Edge1.N = Vector2.Normalize(New Vector2(-s1.Y, s1.X))
                                                 t.Edge2.N = Vector2.Normalize(New Vector2(-s2.Y, s2.X))
                                                 t.Edge3.N = Vector2.Normalize(New Vector2(-s3.Y, s3.X))

                                             End Sub)
        End Sub

        ''' <summary>
        ''' Gets the node ids of the vertices of the given cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Private Function GetNodeDetails(t As Cell) As CellNodes

            Dim result = New CellNodes With {
                .N1 = t.V1,
                .N2 = t.V2,
                .N3 = t.V3
            }

            If IsNothing(t.V4) = False Then

                result.N4 = t.V4

            End If

            Return result

        End Function

        ''' <summary>
        ''' Gets the position vectors of the vertices of a given cell
        ''' </summary>
        ''' <param name="n1"></param>
        ''' <param name="n2"></param>
        ''' <param name="n3"></param>
        ''' <returns></returns>
        Private Function GetPositionVectors(n As CellNodes) As CellNodeVectors

            Dim result As New CellNodeVectors With {
                .R1 = data.NodeV(n.N1).R,
                .R2 = data.NodeV(n.N2).R,
                .R3 = data.NodeV(n.N3).R
            }

            If IsNothing(n.N4) = False Then

                result.R4 = data.NodeV(n.N4).R

            End If

            Return result

        End Function

    End Class
End Namespace