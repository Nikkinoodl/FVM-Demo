Imports System.Numerics
Imports Core.Common
Imports Core.Data
Imports Core.Domain

Namespace Services
    Public Class SharedUtilities

        ''' <summary>
        ''' Returns the side types of a cell as an array
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Friend Shared Function GetSideTypesAsArray(t As Cell) As SideType()

            Dim sides = New List(Of SideType)

            For Each e As Edge In t.Edges

                sides.Add(e.SideType)

            Next

            Return sides.ToArray()

        End Function

        ''' <summary>
        ''' Returns the node ids for the given cell, expressed as an array (of integer)
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Friend Shared Function GetNodesAsArray(t As Cell) As Integer()

            Dim nodeMap = Dictionaries.NodeMap(t)
            Dim value As Integer?() = Nothing

            If Not nodeMap.TryGetValue(t.CellType, value) Then Throw New Exception("Invalid CellType")

            Return value.Where(Function(x) x.HasValue).Select(Function(x) x.Value).ToArray()

        End Function

        ''' <summary>
        ''' Gets the position vector of the midpoint of a triangular cell edge
        ''' </summary>
        ''' <param name="e"></param>
        ''' <param name="r"></param>
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
        ''' Gets the position vector of all edge midpoint of a triangular cell edge
        ''' </summary>
        ''' <param name="e"></param>
        ''' <param name="r"></param>
        ''' <returns></returns>
        Friend Shared Sub FindTriangleMidPoints(t As Cell, r As Vector2())

            Dim sideCalculations = Dictionaries.SideMidPointsTriangle(r)
            Dim value As Func(Of Vector2) = Nothing

            For Each e As Edge In t.Edges

                If Not sideCalculations.TryGetValue(e.SideName, value) Then
                    Throw New Exception("Invalid SideName")
                End If

                e.R = value.Invoke()

            Next

        End Sub

        ''' <summary>
        ''' Calculates mid point of all edges on a non-triangular cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="r"></param>
        ''' <param name="nSides"></param>
        Friend Shared Sub FindMidPoints(t As Cell, r As Vector2(), nSides As Integer)

            For i As Integer = 0 To nSides - 1

                'calculate mid points
                If i < nSides - 1 Then

                    t.Edges(i).R = (r(i) + r(i + 1)) * 0.5

                Else  'close the loop on the cell

                    t.Edges(i).R = (r(i) + r(0)) * 0.5

                End If

            Next

        End Sub

        ''' <summary>
        ''' Calculates and sets the center of a cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="r"></param>
        ''' <param name="nSides"></param>
        Friend Shared Sub CalculateCellCenter(t As Cell, r As Vector2(), nSides As Integer)

            'calculate cell center
            t.R = Vector2.Zero

            For i As Integer = 0 To nSides - 1

                'cell center
                t.R += r(i)

            Next

            'divide by number of nodes (same as number of sides)
            t.R /= nSides

        End Sub

        ''' <summary>
        ''' Calculates edge vectors and lengths of a triangular cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="r"></param>
        Friend Shared Sub CalculateTriangleEdgeLengths(t As Cell, r As Vector2())

            Dim sideCalculations = Dictionaries.EdgeVectorsTriangle(r)
            Dim value As Func(Of Vector2) = Nothing

            For Each e As Edge In t.Edges

                If Not sideCalculations.TryGetValue(e.SideName, value) Then
                    Throw New Exception("Invalid SideName")
                End If

                e.Lv = value.Invoke()
                e.L = e.Lv.Length()

            Next

        End Sub

        ''' <summary>
        ''' Calculates edge vectors and length of all edges on a non-triangular cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="r"></param>
        ''' <param name="nSides"></param>
        Friend Shared Sub CalculateEdgeLengths(t As Cell, r As Vector2(), nSides As Integer)

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

        End Sub

        ''' <summary>
        ''' Calculates the area of a cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="r"></param>
        ''' <param name="nSides"></param>
        Friend Shared Sub CalculateCellArea(t As Cell, r As Vector2(), nSides As Integer)

            Dim area As Single

            'calculate area using linear algebra
            For i As Integer = 0 To nSides - 1

                'edge vectors
                If i < nSides - 1 Then

                    area += r(i).X * r(i + 1).Y - r(i).Y * r(i + 1).X

                Else  'close the loop on the cell

                    area += r(i).X * r(0).Y - r(i).Y * r(0).X

                End If

            Next

            t.AreaI = 1 / (0.5 * Math.Abs(area))


        End Sub

        ''' <summary>
        ''' Returns the longest edge of a cell when the cell edge lengths are already set
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
        Friend Shared Function FindMidPointQuads(e As Edge, r As Vector2()) As Vector2

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
        Friend Shared Function CalcAngleToYAxis(r As Vector2, n As Node) As Single

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
        Friend Shared Function CalcAngleToYAxis(r As Vector2, rC As Vector2) As Single

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
        ''' Returns triangular edge cell nodes in a standard order (N1, N2, N3)
        ''' </summary>
        ''' <param name="nodes"></param>
        ''' <param name="positionVectors"></param>
        ''' <returns></returns>
        Friend Shared Function EdgeCellNodes(nodes As Integer(), positionVectors As Vector2()) As (Integer, Integer, Integer)

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

        ''' <summary>
        ''' Identifies the side type of the adjoining edge for Delaunay Triangulation
        ''' </summary>
        ''' <param name="configuration"></param>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Friend Shared Function JoiningSide(configuration As Integer, t As Cell) As SideType

            Dim configMap = Dictionaries.ConfigToSideType(t)
            Dim value As Func(Of SideType) = Nothing

            If Not configMap.TryGetValue(configuration, value) Then
                Throw New Exception("Invalid configuration")
            End If

            Return value.Invoke()

        End Function
    End Class

End Namespace