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

            Dim n() As Integer

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
                Case Else
                    Throw New Exception()
            End Select

            Return n

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
                    rP = (r.R2 + r.R3) * 0.5
                Case SideName.S2
                    rP = (r.R1 + r.R3) * 0.5
                Case SideName.S3
                    rP = (r.R1 + r.R2) * 0.5
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
                    rP = (r.R2 + r.R1) * 0.5
                Case SideName.S2
                    rP = (r.R3 + r.R2) * 0.5
                Case SideName.S3
                    rP = (r.R4 + r.R3) * 0.5
                Case SideName.S4
                    rP = (r.R1 + r.R4) * 0.5
                Case Else
                    Throw New Exception
            End Select

            Return rP

        End Function

        ''' <summary>
        ''' Determines if a set of position vectors creates a right angle triangle
        ''' </summary>
        ''' <param name="pV"></param>
        ''' <returns></returns>
        Friend Shared Function HasRightAngle(pV As CellNodeVectors) As Boolean

            Dim r1 = pV.R3 - pV.R2
            Dim r2 = pV.R1 - pV.R3
            Dim r3 = pV.R2 - pV.R1

            Return Vector2.Dot(r3, r2) = 0 Or Vector2.Dot(r1, r3) = 0 Or Vector2.Dot(r2, r1) = 0

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
            Return p < rad

        End Function

        ''' <summary>
        ''' Calculates the angle between the vector from n to r and the Y axis
        ''' </summary>
        ''' <param name="r"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Friend Shared Function CalcAngleToYAxis(r As Vector2, n As Node) As Double

            Dim dot, det As Single


            dot = Vector2.Dot(r - n.R, Vector2.UnitY)
            det = (r - n.R).X * 1

            Return Math.Atan2(det, dot)

        End Function

        ''' <summary>
        ''' Calculates the angle between a vector and the Y axis
        ''' </summary>
        ''' <param name="r"></param>
        ''' <param name="rC"></param>
        ''' <returns></returns>
        Friend Shared Function CalcAngleToYAxis(r As Vector2, rC As Vector2) As Double

            Dim dot, det As Single

            dot = Vector2.Dot(r - rC, Vector2.UnitY)
            det = (r - rC).X * 1

            Return Math.Atan2(det, dot)

        End Function

        ''' <summary>
        ''' Returns a number to indicate whether edge lies on left (1) or right (2) of farfield
        ''' </summary>
        ''' <param name="e"></param>
        ''' <param name="farfield"></param>
        ''' <returns></returns>
        Friend Shared Function LeftOrRightEdge(e As Edge, farfield As Farfield) As SByte

            If e.R.X = 0 Then

                Return 1

            ElseIf e.R.X = farfield.Width Then

                Return 2

            Else

                Return 0

            End If

        End Function

    End Class

End Namespace