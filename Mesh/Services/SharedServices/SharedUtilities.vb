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
        Friend Shared Function GetSideTypes(t As Cell) As Array

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
        Friend Shared Function GetSideNames(t As Cell) As Array

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
        Friend Shared Function GetNumberSides(t As Cell) As Integer

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
        Friend Shared Function GetNodes(t As Cell) As Array

            Dim nSides = GetNumberSides(t)
            Dim n(nSides - 1) As Integer

            Select Case t.CellType
                Case CellType.triangle

                    n = {t.V1, t.V2, t.V3}

                Case CellType.quad

                    n = {t.V1, t.V2, t.V3, t.V4}

                Case CellType.pent

                    n = {t.V1, t.V2, t.V3, t.V4, t.V5}

                Case CellType.hex

                    n = {t.V1, t.V2, t.V3, t.V4, t.V5, t.V6}

                Case CellType.oct

                    n = {t.V1, t.V2, t.V3, t.V4, t.V5, t.V6, t.V7, t.V8}

            End Select

            Return n.ToArray

        End Function

        ''' <summary>
        ''' Gets the position vector for the given edge midpoint of a triangular cell
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Friend Shared Function FindMidPoint(e As Edge, r As CellNodeVectors) As Vector2

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
        Friend Shared Function FindMidPointQuads(e As Edge, r As CellNodeVectors) As Vector2

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
        Friend Shared Function HasRightAngle(positionVectors As CellNodeVectors) As Boolean

            Dim r1 = Vector2.Subtract(positionVectors.R3, positionVectors.R2)
            Dim r2 = Vector2.Subtract(positionVectors.R1, positionVectors.R3)
            Dim r3 = Vector2.Subtract(positionVectors.R2, positionVectors.R1)

            If Vector2.Dot(r3, r2) = 0 Or Vector2.Dot(r1, r3) = 0 Or Vector2.Dot(r2, r1) = 0 Then
                Return True
            Else
                Return False
            End If

        End Function

        ''' <summary>
        ''' Checks if rP lies outside circumcircle of cell centered on rCenter
        ''' </summary>
        ''' <param name="rP"></param>
        ''' <param name="r1"></param>
        ''' <param name="rCenter"></param>
        ''' <returns></returns>
        Friend Shared Function CheckInCircle(r1 As Vector2, rP As Vector2, rCenter As Vector2) As Boolean

            Dim rad, p As Double

            'circumcircle radius
            rad = Vector2.Distance(rCenter, r1)

            'distance from cell center to rP
            p = Vector2.Distance(rP, rCenter)

            'compare and return true if in circumcircle
            If p < rad Then

                Return True

            Else

                Return False

            End If

        End Function

        ''' <summary>
        ''' Calculates the angle between the vector from n to r and the Y axis
        ''' </summary>
        ''' <param name="r"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Friend Shared Function CalcAngleToYAxis(r As Vector2, n As Node) As Double

            Dim dot, det As Single
            Dim theta As Double

            dot = Vector2.Dot(Vector2.Subtract(r, n.R), Vector2.UnitY)
            det = Vector2.Subtract(r, n.R).X * 1
            theta = Math.Atan2(det, dot)

            Return theta

        End Function

        Friend Shared Function CalcAngleToYAxis(r As Vector2, rC As Vector2) As Double

            Dim dot, det As Single
            Dim theta As Double

            dot = Vector2.Dot(Vector2.Subtract(r, rC), Vector2.UnitY)
            det = Vector2.Subtract(r, rC).X * 1
            theta = Math.Atan2(det, dot)

            Return theta

        End Function

    End Class

End Namespace