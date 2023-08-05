Imports Core.Interfaces
Imports Core.Common
Imports Mesh.Factories
Imports System.Numerics
Imports Core.Domain

Namespace Services
    Public Class CellSplitter : Implements ICellSplitter

        Private ReadOnly data As IDataAccessService
        Private ReadOnly factory As IGridFactory

        Public Sub New(data As IDataAccessService, factory As IGridFactory)

            Me.data = data
            Me.factory = factory

        End Sub

        ''' <summary>
        ''' Refines the mesh by splitting triangular cells between the longest side and the opposing vertex
        ''' </summary>
        Public Sub SplitCells() Implements ICellSplitter.SplitCells

            Dim numcells = data.CellList.Count()
            Dim n = data.Nodelist.Count

            'current max id in celllist and new id for cell inserts
            Dim maxId = data.CellList.Max(Function(p) p.Id)
            Dim newId = maxId + 1

            'cycle through all existing cells (t is the index of CellList)
            'we use the index to loop because we will be adding new items to CellList as we go
            For t = 0 To numcells - 1

                'Get node ids and details for the current cell
                Dim nodes As CellNodes = GetNodeDetails(t)
                Dim nodeTypeCollection As CellNodeTypes = GetNodeSurface(nodes)
                Dim positionVectors As CellNodeVectors = GetPositionVectors(nodes)

                'Use this diagram to help with diagnosing problems and to understand node order
                'before and after splits.  All methods number the vertices in a clockwise direction
                '
                'config =1     L1 is longest side

                '               n2
                '                |\
                '                | \ np
                '                |  \
                '                |___\
                '               n1     n3
                '
                'config = 2     L2 is longest side

                '               n3
                '                |\
                '                | \ np
                '                |  \
                '                |___\
                '               n2     n1
                '
                'config = 3     L3 is longest side (default)

                '               n1
                '                |\
                '                | \ np
                '                |  \
                '                |___\
                '               n3     n2


                'determine which is the longest side
                Dim longSide As Edge = data.FindLongestSide(t)

                'A new node will be created at the mid point of longest side
                Dim rP = FindMidPoint(longSide, positionVectors)

                Dim vp As Integer

                If data.Exists(rP) > 0 Then   'if there is already a node there make sure we don't overwrite it

                    'point vp to the existing node
                    vp = data.FindNode(rP)

                Else                           'else create a new node (default and most common behavior)

                    vp = n                     'assign index to new node

                    'Create a new node at the np point and return incremented n
                    n = CreateNewNode(longSide, vp, rP, nodeTypeCollection)

                End If

                'Split the cell between the new node and its opposing vertex and return incremented newId
                newId = DivideCells(t, longSide, newId, vp, nodes)

            Next

            'Complete the cell splitting prcess
            CleanOrphanNodes()

        End Sub

        ''' <summary>
        ''' Refines a grid by preserving equilateral triangles
        ''' </summary>
        Public Sub DivideEquilateral() Implements ICellSplitter.DivideEquilateral

            Dim numcells = data.CellList.Count
            Dim n = data.Nodelist.Count

            'current max id in celllist and new id for new cells
            Dim maxId = data.CellList.Max(Function(p) p.Id)
            Dim newId = maxId + 1

            'cycle through 1 less than the count
            For t = 0 To numcells - 1

                'Get node ids and details for this cell
                Dim nodes As CellNodes = GetNodeDetails(t)
                Dim nodeTypeCollection As CellNodeTypes = GetNodeSurface(nodes)
                Dim positionVectors As CellNodeVectors = GetPositionVectors(nodes)
                Dim newNodes As New List(Of Integer)

                'check for half-equilateral triangles at the left and right edges. These need special handling.
                If HasRightAngle(positionVectors) = True Then

                    'determine which is the longest side
                    Dim longSide As Edge = data.FindLongestSide(t)

                    'A new node will be created at the mid point of longest side
                    Dim rP = FindMidPoint(longSide, positionVectors)

                    Dim vp As Integer

                    If data.Exists(rP) > 0 Then   'if there is already a node there make sure we don't overwrite it

                        'point vp to the existing node
                        vp = data.FindNode(rP)

                    Else                           'else create a new node (default and most common behavior)

                        vp = n                     'assign index to new node

                        'Create a new node at the np point and return incremented n
                        n = CreateNewNode(longSide, vp, rP, nodeTypeCollection)

                    End If

                    'Split the cell between the new node and its opposing vertex and return incremented newId
                    newId = DivideCells(t, longSide, newId, vp, nodes)

                    'we have created two cells with the following indexes
                    Dim index1 = data.CellList.FindIndex(Function(p) p.Id = newId - 2)
                    Dim index2 = data.CellList.FindIndex(Function(p) p.Id = newId - 1)

                    'figure out which of these newly created cells has a vertical side and repeat the
                    'split for that cell
                    For Each c As Integer In {index1, index2}

                        nodes = GetNodeDetails(c)
                        positionVectors = GetPositionVectors(nodes)

                        If HasVerticalSide(c) = True Then

                            'determine which is the longest side
                            'this has to be calculated as side lengths are not known for newly created cells
                            longSide = FindLongSide(c, positionVectors)

                            'find the mid point of longest side
                            rP = FindMidPoint(longSide, positionVectors)

                            If data.Exists(rP) > 0 Then   'if there is already a node there make sure we don't overwrite it

                                'point vp to the existing node
                                vp = data.FindNode(rP)

                            Else                           'else create a new node (default and most common behavior)

                                vp = n                     'assign index to new node

                                'create a new node at the np point and return incremented n
                                n = CreateNewNode(longSide, vp, rP, nodeTypeCollection)

                            End If

                            'split the cell between the new node and its opposing vertex and return incremented newId
                            newId = DivideCells(c, longSide, newId, vp, nodes)

                            Exit For

                        End If

                    Next

                Else

                    'otherwise treat as a standard equilateral triangle
                    For Each e As Edge In data.CellList(t).Edges

                        Dim rP = FindMidPoint(e, positionVectors)
                        Dim vp As Integer

                        If data.Exists(rP) > 0 Then           'if there is an orphan node

                            'point vp to the existing node
                            vp = data.FindNode(rP)

                        Else                                   'create new mid point node

                            vp = n        'assign index to new node
                            n = CreateNewNode(e, vp, rP, nodeTypeCollection)

                        End If

                        'add vp to a new node list, note that new nodes are added in the same order as
                        'edges
                        newNodes.Add(vp)

                    Next

                    'split the existing cell in four and return incremented newId
                    newId = DivideCellsEquilateral(t, newId, nodes, newNodes)

                End If

            Next

        End Sub

        ''' <summary>
        ''' Refines a regular grid which comprises square cells
        ''' </summary>
        Public Sub DivideRegularCells() Implements ICellSplitter.DivideRegularCells

            Dim numcells = data.CellList.Count
            Dim n = data.Nodelist.Count

            'current max id in celllist and new id for new cells
            Dim maxId = data.CellList.Max(Function(p) p.Id)
            Dim newId = maxId + 1

            'cycle through 1 less than the count
            For t = 0 To numcells - 1

                'Get node ids and details for this cell
                Dim nodes As CellNodes = GetNodeDetails(t)
                Dim positionVectors As CellNodeVectors = GetPositionVectors(nodes)
                Dim centerNodes As New List(Of Integer)

                For Each e As Edge In data.CellList(t).Edges

                    Dim rP = FindMidPointSquares(e, positionVectors)
                    Dim vp As Integer

                    If data.Exists(rP) > 0 Then           'if there is an orphan node

                        'point vp to the existing node
                        vp = data.FindNode(rP)

                    Else                                   'create new mid point node

                        vp = n        'assign index to new node

                        n = CreateNewNodeRectangle(e, vp, rP)

                    End If

                    'add vp to a new node list
                    centerNodes.Add(vp)

                Next

                'finally, create new node at the cell center
                n = CreateNewCenterNodeRectangle(n, positionVectors)

                'split the existing cell in four and return incremented newId
                newId = DivideCellsSquares(t, newId, nodes, centerNodes, n - 1)

            Next

        End Sub

        ''' <summary>
        ''' Gets the position vector for the given edge midpoint
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Private Function FindMidPoint(e As Edge, r As CellNodeVectors) As Vector2

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
        ''' Gets the position vector for the given edge midpoint
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Private Function FindMidPointSquares(e As Edge, r As CellNodeVectors) As Vector2

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
        ''' Gets the node ids of the vertices of the given cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Private Function GetNodeDetails(t As Integer) As CellNodes

            Dim result = New CellNodes With {
                .N1 = data.CellList(t).V1,
                .N2 = data.CellList(t).V2,
                .N3 = data.CellList(t).V3
            }

            If IsNothing(data.CellList(t).V4) = False Then

                result.N4 = data.CellList(t).V4

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

        ''' <summary>
        ''' Determines if a set of position vectors creates a right angle triangle and, if so, returns an
        ''' integer corresponding with the right-angled node
        ''' </summary>
        ''' <param name="positionVectors"></param>
        ''' <returns></returns>
        Private Function HasRightAngle(positionVectors As CellNodeVectors) As Boolean

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
        ''' Inspects a cell to see if check if any of the edges is vertical
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Private Function HasVerticalSide(t As Integer) As Boolean

            Dim nodes As CellNodes = GetNodeDetails(t)
            Dim positionVectors As CellNodeVectors = GetPositionVectors(nodes)

            Dim r1 = Vector2.Subtract(positionVectors.R3, positionVectors.R2)
            Dim r2 = Vector2.Subtract(positionVectors.R1, positionVectors.R3)
            Dim r3 = Vector2.Subtract(positionVectors.R2, positionVectors.R1)

            If Vector2.Dot(r1, Vector2.UnitX) = 0 Or Vector2.Dot(r2, Vector2.UnitX) = 0 Or Vector2.Dot(r3, Vector2.UnitX) = 0 Then
                Return True
            Else
                Return False
            End If

        End Function

        ''' <summary>
        ''' Returns the longest edge of a cell when the cell lengths have not yet been calculated
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="positionVectors"></param>
        ''' <returns></returns>
        Private Function FindLongSide(t As Integer, positionVectors As CellNodeVectors) As Edge

            Dim l1 = Vector2.Subtract(positionVectors.R3, positionVectors.R2).Length
            Dim l2 = Vector2.Subtract(positionVectors.R1, positionVectors.R3).Length
            Dim l3 = Vector2.Subtract(positionVectors.R2, positionVectors.R1).Length

            If l1 > l2 And l1 > l3 Then
                Return data.CellList(t).Edge1
            ElseIf l2 > l1 And l2 > l3 Then
                Return data.CellList(t).Edge2
            Else
                Return data.CellList(t).Edge3
            End If


        End Function

        ''' <summary>
        ''' Gets the surface flags associated with the vertex nodes of a triangular cell
        ''' </summary>
        ''' <param name="n1"></param>
        ''' <param name="n2"></param>
        ''' <param name="n3"></param>
        ''' <returns></returns>
        Private Function GetNodeSurface(n As CellNodes) As CellNodeTypes

            Dim result As New CellNodeTypes With {
                    .S1 = data.NodeV(n.N1).Surface,
                    .S2 = data.NodeV(n.N2).Surface,
                    .S3 = data.NodeV(n.N3).Surface
            }

            Return result

        End Function

        ''' <summary>
        ''' Creates a new node on the given edge
        ''' </summary>
        ''' <param name="e"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Private Function CreateNewNode(e As Edge, vp As Integer, rP As Vector2, s As CellNodeTypes) As Integer

            'If nodes to either side are both surface nodes, then np must be a surface node.
            'Boundary node identification must come from the existing cell (not the nodes) as lines between nodes
            'can cut across corners of the calc domain

            Select Case e.SideName
                Case SideName.S1
                    factory.RequestNode(vp, rP, s.S2 And s.S3, IIf(e.SideType = SideType.boundary, True, False))
                Case SideName.S2
                    factory.RequestNode(vp, rP, s.S1 And s.S3, IIf(e.SideType = SideType.boundary, True, False))
                Case SideName.S3
                    factory.RequestNode(vp, rP, s.S2 And s.S1, IIf(e.SideType = SideType.boundary, True, False))
                Case Else
                    Throw New Exception
            End Select

            Return vp + 1

        End Function

        ''' <summary>
        ''' Creates a new node on the given edge of a square cell
        ''' </summary>
        ''' <param name="e"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Private Function CreateNewNodeRectangle(e As Edge, vp As Integer, rP As Vector2) As Integer

            'If nodes to either side are both surface nodes, then np must be a surface node.
            'Boundary node identification must come from the existing cell (not the nodes) as lines between nodes
            'can cut across corners of the calc domain

            factory.RequestNode(vp, rP, False, IIf(e.SideType = SideType.boundary, True, False))

            Return vp + 1

        End Function

        ''' <summary>
        ''' Creates a new node at the center of a rectangular cell
        ''' </summary>
        ''' <param name="n"></param>
        ''' <param name="positionVectors"></param>
        ''' <returns></returns>
        Private Function CreateNewCenterNodeRectangle(n As Integer, positionVectors As CellNodeVectors) As Integer

            'position vector of the center point
            Dim rP As Vector2 = Vector2.Add(positionVectors.R1, Vector2.Add(positionVectors.R2, Vector2.Add(positionVectors.R3, positionVectors.R4))) * 0.25

            'add node
            factory.RequestNode(n, rP, False, False)

            Return n + 1

        End Function


        ''' <summary>
        ''' Replace an existing cell instance and create a new cell instance
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="e"></param>
        ''' <param name="newid"></param>
        ''' <returns></returns>
        Private Function DivideCells(t As Integer, e As Edge, newid As Integer, vp As Integer, n As CellNodes) As Integer

            Dim s0 As SideType = SideType.none
            Dim s1 As SideType = data.CellList(t).Edge1.SideType
            Dim s2 As SideType = data.CellList(t).Edge2.SideType
            Dim s3 As SideType = data.CellList(t).Edge3.SideType

            'Refer to the diagram at the SplitCells sub for better understanding of what is being done.
            Select Case e.SideName
                Case SideName.S1
                    'The new face must always be of SideType.none : other faces inherit their existing state.
                    factory.ReplaceCell(t, newid, n.N1, n.N2, vp, s1, s0, s3)
                    factory.AddCell(newid + 1, n.N1, vp, n.N3, s1, s2, s0)
                Case SideName.S2
                    factory.ReplaceCell(t, newid, vp, n.N2, n.N3, s1, s2, s0)
                    factory.AddCell(newid + 1, n.N1, n.N2, vp, s0, s2, s3)
                Case SideName.S3
                    factory.ReplaceCell(t, newid, n.N1, vp, n.N3, s0, s2, s3)
                    factory.AddCell(newid + 1, vp, n.N2, n.N3, s1, s0, s3)
                Case Else
                    Throw New Exception
            End Select

            Return newid + 2

        End Function

        ''' <summary>
        ''' Finds triangular cells that have an orphan node on one edge and splits them at the orphan node
        ''' </summary>
        Private Sub CleanOrphanNodes()

            Dim numcells = data.CellList.Count
            Dim n = data.Nodelist.Count

            'current max id in celllist and new id for new cells
            Dim maxId = data.CellList.Max(Function(p) p.Id)
            Dim newId = maxId + 1

            'cycle through 1 less than the count
            For t = 0 To numcells - 1

                'Get node ids and details for this cell
                Dim nodes As CellNodes = GetNodeDetails(t)
                Dim positionVectors As CellNodeVectors = GetPositionVectors(nodes)

                For Each e As Edge In data.CellList(t).Edges

                    Dim rP = FindMidPoint(e, positionVectors)
                    Dim vp As Integer

                    If data.Exists(rP) > 0 Then           'if there is an orphan node

                        'point vp to the existing node
                        vp = data.FindNode(rP)

                        'Split the existing cell in two and return incremented newId
                        newId = DivideCells(t, e, newId, vp, nodes)

                        Continue For

                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' Replaces an existing equilateral triangle with four new equilateral triangle cells
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="newid"></param>
        ''' <param name="n"></param>
        ''' <param name="m"></param>
        ''' <param name="i"></param>
        ''' <returns></returns>
        Private Function DivideCellsEquilateral(t As Integer, newid As Integer, n As CellNodes, m As List(Of Integer)) As Integer

            Dim s0 As SideType = SideType.none
            Dim s1 As SideType = data.CellList(t).Edge1.SideType
            Dim s2 As SideType = data.CellList(t).Edge2.SideType
            Dim s3 As SideType = data.CellList(t).Edge3.SideType

            'replace the original cell
            factory.ReplaceCell(t, newid, n.N1, m(2), m(1), s0, s2, s3)

            'add three new cells
            factory.AddCell(newid + 1, n.N2, m(0), m(2), s0, s3, s1)
            factory.AddCell(newid + 2, n.N3, m(1), m(0), s0, s1, s2)
            factory.AddCell(newid + 3, m(0), m(1), m(2), s0, s0, s0)

            Return newid + 4

        End Function

        ''' <summary>
        ''' Replace an existing square cell instance with four new square cells
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="e"></param>
        ''' <param name="newid"></param>
        ''' <returns></returns>
        Private Function DivideCellsSquares(t As Integer, newid As Integer, n As CellNodes, m As List(Of Integer), c As Integer) As Integer

            Dim s0 As SideType = SideType.none
            Dim s1 As SideType = data.CellList(t).Edge1.SideType
            Dim s2 As SideType = data.CellList(t).Edge2.SideType
            Dim s3 As SideType = data.CellList(t).Edge3.SideType
            Dim s4 As SideType = data.CellList(t).Edge4.SideType

            'replace the original cell
            factory.ReplaceCellSquare(t, newid, n.N1, m(0), c, m(3), s1, s0, s0, s4)

            'add three new cells
            factory.AddSquare(newid + 1, m(0), n.N2, m(1), c, s1, s2, s0, s0)
            factory.AddSquare(newid + 2, c, m(1), n.N3, m(2), s0, s2, s3, s0)
            factory.AddSquare(newid + 3, m(3), c, m(2), n.N4, s0, s0, s3, s4)

            Return newid + 4

        End Function
    End Class
End Namespace