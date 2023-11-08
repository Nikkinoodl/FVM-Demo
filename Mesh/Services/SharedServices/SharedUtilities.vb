Imports System.Numerics
Imports Core.Common
Imports Core.Domain

Namespace Services
    Public Class SharedUtilities

        ''' <summary>
        ''' Returns an array of the side types of the edges of the given cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Public Shared Function GetSideTypes(t As Cell) As Array

            Dim nSides = GetNumberSides(t)
            Dim s(nSides) As SideType
            Dim i As Integer = 0

            For Each e As Edge In t.Edges

                s(i) = e.SideType

                i += 1

            Next

            Return s.ToArray

        End Function

        ''' <summary>
        ''' Returns an array of the side names of the edges of the given cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Public Shared Function GetSideNames(t As Cell) As Array

            Dim nSides = GetNumberSides(t)
            Dim s(nSides) As SideName
            Dim i As Integer = 0

            For Each e As Edge In t.Edges

                s(i) = e.SideName

                i += 1

            Next

            Return s.ToArray

        End Function

        ''' <summary>
        ''' Returns the number of sides possessed by a cell as determined by its CellType
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Public Shared Function GetNumberSides(t As Cell) As Integer

            Select Case t.CellType
                Case CellType.triangle
                    Return 3
                Case CellType.quad
                    Return 4
                Case CellType.pent
                    Return 5
                Case CellType.hex
                    Return 6
                Case CellType.oct
                    Return 8
                Case Else
                    Throw New Exception()
            End Select

        End Function

        ''' <summary>
        ''' Returns the node ids for the given cell, expressed as an array (of integer)
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Public Shared Function GetNodes(t As Cell) As Array

            Dim nSides = GetNumberSides(t)
            Dim n(nSides - 1) As Integer

            Select Case t.CellType
                Case CellType.triangle
                    n(0) = t.V1
                    n(1) = t.V2
                    n(2) = t.V3
                Case CellType.quad
                    n(0) = t.V1
                    n(1) = t.V2
                    n(2) = t.V3
                    n(3) = t.V4
                Case CellType.pent
                    n(0) = t.V1
                    n(1) = t.V2
                    n(2) = t.V3
                    n(3) = t.V4
                    n(4) = t.V5
                Case CellType.hex
                    n(0) = t.V1
                    n(1) = t.V2
                    n(2) = t.V3
                    n(3) = t.V4
                    n(4) = t.V5
                    n(5) = t.V6
                Case CellType.oct
                    n(0) = t.V1
                    n(1) = t.V2
                    n(2) = t.V3
                    n(3) = t.V4
                    n(4) = t.V5
                    n(5) = t.V6
                    n(6) = t.V7
                    n(7) = t.V8
            End Select

            Return n.ToArray

        End Function

        ''' <summary>
        ''' Gets the position vector for the given edge midpoint of a triangular cell
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Public Shared Function FindMidPoint(e As Edge, r As CellNodeVectors) As Vector2

            Dim rP As Vector2

            Select Case e.SideName
                Case SideName.S1
                    rP = Vector2.Add(r.R2, r.R3) * 0.5
                Case SideName.S2
                    rP = Vector2.Add(r.R1, r.R3) * 0.5
                Case SideName.S3
                    rP = Vector2.Add(r.R1, r.R2) * 0.5
                Case Else
                    Throw New Exception
            End Select

            Return rP

        End Function

        ''' <summary>
        ''' Gets the position vector for the given edge midpoint of a quad cell
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Public Shared Function FindMidPointQuads(e As Edge, r As CellNodeVectors) As Vector2

            Dim rP As Vector2

            Select Case e.SideName
                Case SideName.S1
                    rP = Vector2.Add(r.R2, r.R1) * 0.5
                Case SideName.S2
                    rP = Vector2.Add(r.R3, r.R2) * 0.5
                Case SideName.S3
                    rP = Vector2.Add(r.R4, r.R3) * 0.5
                Case SideName.S4
                    rP = Vector2.Add(r.R1, r.R4) * 0.5
                Case Else
                    Throw New Exception
            End Select

            Return rP

        End Function

        ''' <summary>
        ''' Determines if a set of position vectors creates a right angle triangle
        ''' </summary>
        ''' <param name="positionVectors"></param>
        ''' <returns></returns>
        Public Shared Function HasRightAngle(positionVectors As CellNodeVectors) As Boolean

            Dim r1 = Vector2.Subtract(positionVectors.R3, positionVectors.R2)
            Dim r2 = Vector2.Subtract(positionVectors.R1, positionVectors.R3)
            Dim r3 = Vector2.Subtract(positionVectors.R2, positionVectors.R1)

            If Vector2.Dot(r3, r2) = 0 Or Vector2.Dot(r1, r3) = 0 Or Vector2.Dot(r2, r1) = 0 Then
                Return True
            Else
                Return False
            End If

        End Function

    End Class

End Namespace