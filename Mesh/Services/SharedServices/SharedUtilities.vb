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

            Dim sideMap As New Dictionary(Of CellType, Integer) From {
                {CellType.triangle, 3},
                {CellType.quad, 4},
                {CellType.pent, 5},
                {CellType.hex, 6},
                {CellType.oct, 8}}

            Dim value As Integer = Nothing
            If Not sideMap.TryGetValue(t.CellType, value) Then Throw New Exception()

            Return value

        End Function

        ''' <summary>
        ''' Returns the node ids for the given cell, expressed as an array (of integer)
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Friend Shared Function GetNodes(t As Cell) As Array

            Dim nodeMap As New Dictionary(Of CellType, Integer?()) From {
                {CellType.triangle, {t.V1, t.V2, t.V3}},
                {CellType.quad, {t.V1, t.V2, t.V3, t.V4}},
                {CellType.pent, {t.V1, t.V2, t.V3, t.V4, t.V5}},
                {CellType.hex, {t.V1, t.V2, t.V3, t.V4, t.V5, t.V6}},
                {CellType.oct, {t.V1, t.V2, t.V3, t.V4, t.V5, t.V6, t.V7, t.V8}}}

            Dim value As Integer?() = Nothing

            If Not nodeMap.TryGetValue(t.CellType, value) Then Throw New Exception()

            Return value.Where(Function(x) x.HasValue).Select(Function(x) x.Value).ToArray()

        End Function

        ''' <summary>
        ''' Gets the position vector for the given edge midpoint of a triangular cell
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Friend Shared Function FindMidPoint(e As Edge, r As CellNodeVectors) As Vector2

            Dim sideCalculations As New Dictionary(Of SideName, Func(Of Vector2)) From {
                {SideName.S1, Function() (r.R2 + r.R3) * 0.5},
                {SideName.S2, Function() (r.R1 + r.R3) * 0.5},
                {SideName.S3, Function() (r.R1 + r.R2) * 0.5}}

            Dim value As Func(Of Vector2) = Nothing

            If Not sideCalculations.TryGetValue(e.SideName, value) Then
                Throw New Exception("Invalid SideName")
            End If

            Return value.Invoke()

        End Function

        ''' <summary>
        ''' Gets the position vector for the given edge midpoint of a quad cell
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Friend Shared Function FindMidPointQuads(e As Edge, r As CellNodeVectors) As Vector2

            Dim sideCalculations As New Dictionary(Of SideName, Func(Of Vector2)) From {
                {SideName.S1, Function() (r.R1 + r.R2) * 0.5},
                {SideName.S2, Function() (r.R2 + r.R3) * 0.5},
                {SideName.S3, Function() (r.R3 + r.R4) * 0.5},
                {SideName.S4, Function() (r.R4 + r.R1) * 0.5}}

            Dim value As Func(Of Vector2) = Nothing

            If Not sideCalculations.TryGetValue(e.SideName, value) Then
                Throw New Exception("Invalid SideName")
            End If

            Return value.Invoke()

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

        ''' <summary>
        ''' Dictionary of node assignments for matching the nodes of border cells to adjoining cells
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        Friend Shared Function CreateNodeAssignment(t As Cell) As Dictionary(Of Tuple(Of CellType, SideName), (Integer?, Integer?))

            Return New Dictionary(Of Tuple(Of CellType, SideName), (Integer?, Integer?)) From {
                            {Tuple.Create(CellType.triangle, SideName.S1), (t.V2, t.V3)},
                            {Tuple.Create(CellType.triangle, SideName.S2), (t.V1, t.V3)},
                            {Tuple.Create(CellType.triangle, SideName.S3), (t.V1, t.V2)},
                            {Tuple.Create(CellType.quad, SideName.S1), (t.V2, t.V1)},
                            {Tuple.Create(CellType.quad, SideName.S2), (t.V3, t.V2)},
                            {Tuple.Create(CellType.quad, SideName.S3), (t.V4, t.V3)},
                            {Tuple.Create(CellType.quad, SideName.S4), (t.V1, t.V4)},
                            {Tuple.Create(CellType.pent, SideName.S1), (t.V2, t.V1)},
                            {Tuple.Create(CellType.pent, SideName.S2), (t.V3, t.V2)},
                            {Tuple.Create(CellType.pent, SideName.S3), (t.V4, t.V3)},
                            {Tuple.Create(CellType.pent, SideName.S4), (t.V5, t.V4)},
                            {Tuple.Create(CellType.pent, SideName.S5), (t.V1, t.V5)},
                            {Tuple.Create(CellType.hex, SideName.S1), (t.V2, t.V1)},
                            {Tuple.Create(CellType.hex, SideName.S2), (t.V3, t.V2)},
                            {Tuple.Create(CellType.hex, SideName.S3), (t.V4, t.V3)},
                            {Tuple.Create(CellType.hex, SideName.S4), (t.V5, t.V4)},
                            {Tuple.Create(CellType.hex, SideName.S5), (t.V6, t.V5)},
                            {Tuple.Create(CellType.hex, SideName.S6), (t.V1, t.V6)},
                            {Tuple.Create(CellType.oct, SideName.S1), (t.V2, t.V1)},
                            {Tuple.Create(CellType.oct, SideName.S2), (t.V3, t.V2)},
                            {Tuple.Create(CellType.oct, SideName.S3), (t.V4, t.V3)},
                            {Tuple.Create(CellType.oct, SideName.S4), (t.V5, t.V4)},
                            {Tuple.Create(CellType.oct, SideName.S5), (t.V6, t.V5)},
                            {Tuple.Create(CellType.oct, SideName.S6), (t.V7, t.V6)},
                            {Tuple.Create(CellType.oct, SideName.S7), (t.V8, t.V7)},
                            {Tuple.Create(CellType.oct, SideName.S8), (t.V1, t.V8)}}

        End Function

        ''' <summary>
        ''' Returns triangular edge cell nodes in a standard order
        ''' </summary>
        ''' <param name="nodes"></param>
        ''' <param name="positionVectors"></param>
        ''' <returns></returns>
        Friend Shared Function EdgeCellNodes(nodes As CellNodes, positionVectors As CellNodeVectors) As (Integer, Integer, Integer)

            'apex is first item, base is second and third
            Dim conditions As New List(Of Tuple(Of Boolean, (Integer, Integer, Integer))) From {
                New Tuple(Of Boolean, (Integer, Integer, Integer))(positionVectors.R1.Y = positionVectors.R2.Y, (nodes.N3, nodes.N1, nodes.N2)),
                New Tuple(Of Boolean, (Integer, Integer, Integer))(positionVectors.R2.Y = positionVectors.R3.Y, (nodes.N1, nodes.N2, nodes.N3)),
                New Tuple(Of Boolean, (Integer, Integer, Integer))(positionVectors.R3.Y = positionVectors.R1.Y, (nodes.N2, nodes.N3, nodes.N1))}

            Return conditions.First(Function(tuple) tuple.Item1).Item2

        End Function

        ''' <summary>
        ''' For Delaunay Triangulation, sets target vertex of the adjoining cell that will be tested
        ''' to see if it lies inside the circumcircle
        ''' </summary>
        ''' <param name="configuration"></param>
        ''' <param name="adjacentCells"></param>
        Friend Shared Function ProcessAdjacent(configuration As Integer, t_adj As Cell) As Integer

            Dim configMap As New Dictionary(Of Integer, Func(Of Integer)) From {
                {1, Function() t_adj.V2},
                {2, Function() t_adj.V3},
                {3, Function() t_adj.V1},
                {4, Function() t_adj.V3},
                {5, Function() t_adj.V1},
                {6, Function() t_adj.V2}}

            Dim value As Func(Of Integer) = Nothing

            If Not configMap.TryGetValue(configuration, value) Then
                Throw New Exception("Invalid configuration")
            End If

            Return value.Invoke()

        End Function
    End Class

End Namespace