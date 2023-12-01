Imports System.Numerics
Imports Core.Common
Imports Core.Data
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

            Dim sideMap = Dictionaries.SideMap()
            Dim value As Integer = Nothing

            If Not sideMap.TryGetValue(t.CellType, value) Then Throw New Exception("Invalid CellType")

            Return value

        End Function

        ''' <summary>
        ''' Returns the node ids for the given cell, expressed as an array (of integer)
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Friend Shared Function GetNodes(t As Cell) As Array

            Dim nodeMap = Dictionaries.NodeMap(t)
            Dim value As Integer?() = Nothing

            If Not nodeMap.TryGetValue(t.CellType, value) Then Throw New Exception("Invalid CellType")

            Return value.Where(Function(x) x.HasValue).Select(Function(x) x.Value).ToArray()

        End Function

        ''' <summary>
        ''' Gets the position vector of the midpoint of a triangular cell edge
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Friend Shared Function FindMidPoint(e As Edge, r As CellNodeVectors) As Vector2

            Dim sideCalculations = Dictionaries.SideMidPointsTriangle(r)
            Dim value As Func(Of Vector2) = Nothing

            If Not sideCalculations.TryGetValue(e.SideName, value) Then
                Throw New Exception("Invalid SideName")
            End If

            Return value.Invoke()

        End Function

        ''' <summary>
        ''' Gets the position vector of the midpoint of a triangular cell edge
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Friend Shared Function FindMidPoint(e As Edge, r As Vector2()) As Vector2

            Dim sideCalculations = Dictionaries.SideMidPointsTriangle(r)
            Dim value As Func(Of Vector2) = Nothing

            If Not sideCalculations.TryGetValue(e.SideName, value) Then
                Throw New Exception("Invalid SideName")
            End If

            Return value.Invoke()

        End Function

        ''' <summary>
        ''' Returns the longest edge of a cell when the cell edge lengths are known
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="positionVectors"></param>
        ''' <returns></returns>
        Friend Shared Function FindLongSide(t As Cell) As Edge

            Dim lengths As New List(Of Tuple(Of Single, Edge)) From {
                New Tuple(Of Single, Edge)(t.Edge1.L, t.Edge1),
                New Tuple(Of Single, Edge)(t.Edge2.L, t.Edge2),
                New Tuple(Of Single, Edge)(t.Edge3.L, t.Edge3)}

            Return lengths.OrderByDescending(Function(tuple) tuple.Item1).First().Item2

        End Function

        ''' <summary>
        ''' Gets the position vector of the midpoint of a quad cell edge
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Friend Shared Function FindMidPointQuads(e As Edge, r As CellNodeVectors) As Vector2

            Dim sideCalculations = Dictionaries.SideMidPoints(r)
            Dim value As Func(Of Vector2) = Nothing

            If Not sideCalculations.TryGetValue(e.SideName, value) Then
                Throw New Exception("Invalid SideName")
            End If

            Return value.Invoke()

        End Function

        ''' <summary>
        ''' Determines if a triangular cell contains a right angle triangle
        ''' </summary>
        ''' <param name="pV"></param>
        ''' <returns></returns>
        Friend Shared Function HasRightAngle(t As Cell) As Boolean

            Return Vector2.Dot(t.Edge3.Lv, t.Edge2.Lv) = 0 Or Vector2.Dot(t.Edge1.Lv, t.Edge3.Lv) = 0 Or Vector2.Dot(t.Edge2.Lv, t.Edge1.Lv) = 0

        End Function

        ''' <summary>
        ''' Inspects a triangular cell to determine if any of the edges are vertical
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Friend Shared Function HasVerticalSide(t As Cell) As Boolean

            Return Vector2.Dot(t.Edge1.Lv, Vector2.UnitX) = 0 Or Vector2.Dot(t.Edge2.Lv, Vector2.UnitX) = 0 Or Vector2.Dot(t.Edge3.Lv, Vector2.UnitX) = 0

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

        ''' <summary>
        ''' Returns triangular edge cell nodes in a standard order
        ''' </summary>
        ''' <param name="nodes"></param>
        ''' <param name="positionVectors"></param>
        ''' <returns></returns>
        Friend Shared Function EdgeCellNodes(nodes As CellNodes, positionVectors As CellNodeVectors) As (Integer, Integer, Integer)

            Dim orderedNodes = Dictionaries.OrderedNodes(nodes, positionVectors)

            Return orderedNodes.First(Function(tuple) tuple.Item1).Item2

        End Function

        ''' <summary>
        ''' For Delaunay Triangulation, sets target vertex of the adjoining cell that will be tested
        ''' to see if it lies inside the circumcircle
        ''' </summary>
        ''' <param name="configuration"></param>
        ''' <param name="adjacentCells"></param>
        Friend Shared Function ProcessAdjacent(configuration As Integer, t_adj As Cell) As Integer

            Dim configMap = Dictionaries.ConfigMap(t_adj)
            Dim value As Func(Of Integer?) = Nothing

            If Not configMap.TryGetValue(configuration, value) Then
                Throw New Exception("Invalid configuration")
            End If

            Return value.Invoke()

        End Function
    End Class

End Namespace