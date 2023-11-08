﻿Imports Core.Interfaces
Imports Core.Common
Imports Mesh.Factories
Imports System.Numerics
Imports Core.Domain
Imports Mesh.Services.SharedUtilities

Namespace Services
    Public Class CellSplitter : Implements ICellSplitter

#Region "Fields"

        Private ReadOnly data As IDataAccessService
        Private ReadOnly factory As IGridFactory

#End Region

#Region "Constructor"

        Public Sub New(data As IDataAccessService, factory As IGridFactory)

            Me.data = data
            Me.factory = factory

        End Sub

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Refines the mesh by splitting triangular cells between the longest side and the opposing vertex
        ''' </summary>
        Public Sub SplitCells(Optional ignoreRightAngleTriangles As Boolean = False) Implements ICellSplitter.SplitCells

            Dim numcells = data.CellList.Count()
            Dim n = data.Nodelist.Count

            'current max id in celllist and new id for cell inserts
            Dim maxId = data.CellList.Max(Function(p) p.Id)
            Dim newId = maxId + 1

            'cycle through all existing cells (t is the index of CellList)
            'we use the index to loop because we will be adding new items to CellList as we go
            For t = 0 To numcells - 1

                'get node ids and details for the current cell
                Dim nodes As CellNodes = GetNodeDetails(t)
                Dim nodeTypeCollection As CellNodeTypes = GetNodeSurface(nodes)
                Dim positionVectors As CellNodeVectors = GetPositionVectors(nodes)

                'in some tiling operations we must skip tiling if the cell has a right angle
                If ignoreRightAngleTriangles = True Then

                    If HasRightAngle(positionVectors) = True Then
                        Continue For
                    End If

                End If

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

                'create a new node at the mid point of longest side
                Dim rP = FindMidPoint(longSide, positionVectors)

                Dim vp As Integer

                If data.Exists(rP) > 0 Then   'if there is already a node there make sure we don't overwrite it

                    'point vp to the existing node
                    vp = data.FindNode(rP)

                Else                           'else create a new node

                    vp = n                     'assign index to new node

                    'create a new node at the np point and return incremented n
                    n = CreateNewNode(longSide, vp, rP, nodeTypeCollection)

                End If

                'split the cell between the new node and its opposing vertex and return incremented newId
                newId = DivideCells(t, longSide, newId, vp, nodes)

            Next

            'complete the cell splitting prcess
            CleanOrphanNodes()

        End Sub

        ''' <summary>
        ''' Refines an equilateral triangle grid by dividing each cell into four equal triangles
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

                    Else                           'else create a new node

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

                            Else                           'else create a new node

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
        ''' Refines a rectangular grid by performing a join tiling. Each cell is divided by joining the center of
        ''' each edge to a new node at the cell center. Do not use if irregular quad cells exist.
        ''' </summary>
        Public Sub DivideRectangularCells() Implements ICellSplitter.DivideRectangularCells

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

                    Dim rP = FindMidPointQuads(e, positionVectors)
                    Dim vp As Integer

                    If data.Exists(rP) > 0 Then           'if there is an orphan node

                        'point vp to the existing node
                        vp = data.FindNode(rP)

                    Else                                   'create new mid point node

                        vp = n        'assign index to new node

                        n = CreateNewNodeQuad(e, vp, rP)

                    End If

                    'add vp to a new node list
                    centerNodes.Add(vp)

                Next

                'finally, create new node at the cell center
                n = CreateNewCenterNodeQuad(n, positionVectors)

                'split the existing cell in four and return incremented newId
                newId = DivideCellsQuads(t, newId, nodes, centerNodes, n - 1)

            Next

        End Sub

        ''' <summary>
        ''' Refines a mesh (any type) by performing a kis tiling. Each cell is divided by joining each node
        ''' to a new node at the cell center.
        ''' </summary>
        Public Sub DivideKis(farfield As Farfield) Implements ICellSplitter.DivideKis

            Dim numcells = data.CellList.Count
            Dim n = data.Nodelist.Count

            'current max id in celllist and new id for new cells
            Dim maxId = data.CellList.Max(Function(p) p.Id)
            Dim newId = maxId + 1

            'cycle through 1 less than the count
            For t = 0 To numcells - 1

                'equilateral cells with edges on the left and right farfield boundaries
                'are divided as if they continued outside the boundary.
                If farfield.Gridtype = GridType.Equilateral Then

                    For Each e As Edge In data.CellList(t).Edges

                        'this identifies the left/right edge cells and vertical side
                        If e.R.X = 0 Or e.R.X = farfield.Width Then

                            Dim nodes As CellNodes = GetNodeDetails(t)
                            Dim nodeTypeCollection As CellNodeTypes = GetNodeSurface(nodes)

                            'use cell center location to determine orientation of cell
                            If data.CellList(t).R.Y > e.R.Y Then

                                'add new node nearer to top of cell
                                n = CreateNewNode(e, n, New Vector2(e.R.X, e.R.Y + 0.18 * e.L), nodeTypeCollection)

                            Else

                                'add new node nearer to bottom of cell
                                n = CreateNewNode(e, n, New Vector2(e.R.X, e.R.Y - 0.18 * e.L), nodeTypeCollection)

                            End If

                            'divide the cell using the new node on the current side
                            newId = DivideCells(t, e, newId, n - 1, nodes)

                            'skip remaining edges and further processing of cell
                            GoTo next_cell

                        End If

                    Next
                End If

                'add new node at cell center
                n = CreateCenterNode(n, t)

                'a list of named tuples for node pairs, ordered in the same way we encounter the sides
                Dim nodePairs As New List(Of (nA As Integer, nB As Integer, sName As SideName, sType As SideType))

                nodePairs = GetNodePairList(t)

                'cycle through the node pairs and each node pair/edge combo seeds a new cell
                For Each np As (Integer, Integer, SideName, SideType) In nodePairs

                    If np.Item3 = SideName.S1 Then

                        '2D kis tiling always creates triangular cells 
                        factory.ReplaceCell(t, newId, np.Item1, np.Item2, n - 1, SideType.none, SideType.none, np.Item4)

                    Else

                        factory.AddCell(newId, np.Item1, np.Item2, n - 1, SideType.none, SideType.none, np.Item4)

                    End If

                    newId += 1
                Next

next_cell:

            Next

        End Sub

        ''' <summary>
        ''' Refines a triangular mesh by performing a join tiling. Each cell is divided by joining the midpoint of
        ''' each side to a new node at the cell center.
        ''' </summary>
        Public Sub DivideJoin(farfield As Farfield) Implements ICellSplitter.DivideJoin

            Dim numcells = data.CellList.Count
            Dim n = data.Nodelist.Count

            'current max id in celllist and new id for new cells
            Dim maxId = data.CellList.Max(Function(p) p.Id)
            Dim newId = maxId + 1

            'cycle through 1 less than the count
            For t = 0 To numcells - 1

                Dim nodes As CellNodes = GetNodeDetails(t)
                Dim nodeTypeCollection As CellNodeTypes = GetNodeSurface(nodes)
                Dim positionVectors As CellNodeVectors = GetPositionVectors(nodes)

                'equilateral cells with edges on the left and right farfield boundaries
                'are divided as if they continued outside the boundary.
                If farfield.Gridtype = GridType.Equilateral Then

                    For Each e As Edge In data.CellList(t).Edges

                        Dim LR As Integer       'is cell aligned to the left edge or right edge?
                        Dim PSD As Integer      'is it pointy side down (is apex below the base)?

                        If e.R.X = 0 Then
                            LR = 1
                        ElseIf e.R.X = farfield.Width Then
                            LR = 2
                        End If

                        'this identifies the left/right edge cells and vertical side
                        If e.R.X = 0 Or e.R.X = farfield.Width Then

                            Dim vs As Integer = n

                            'use cell center location to determine orientation of cell
                            If data.CellList(t).R.Y > e.R.Y Then

                                PSD = 2       'apex is below base

                                'add new node nearer to top of cell
                                n = CreateNewNode(e, vs, New Vector2(e.R.X, e.R.Y + 0.18 * e.L), nodeTypeCollection)

                            Else

                                PSD = 0        'apex is above base

                                'add new node nearer to bottom of cell
                                n = CreateNewNode(e, vs, New Vector2(e.R.X, e.R.Y - 0.18 * e.L), nodeTypeCollection)

                            End If

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

                            'configuration determines the orientation of this edge cell
                            Dim config As Integer = LR + PSD

                            'config = 1                       2
                            '
                            '               A                           A
                            '                |\                         /|
                            '             vs |_\ vp                 vp /_| vs
                            '                |  \                     /  |
                            '                |___\                   /___|
                            '               C    B                  C    B
                            '
                            '
                            ' 3                                4 
                            '
                            '               B ____ C              B  ____ C
                            '                |   /                   \   |
                            '             vs |__/ vp                vp\__| vs
                            '                | /                       \ |
                            '                |/                         \|
                            '               A                            A

                            Dim triSide2 As SideType = SideType.none
                            Dim triSide3 As SideType = SideType.none

                            Dim quadSide1 As SideType       'value assigned conditionally in code below
                            Dim quadSide2 As SideType = SideType.none
                            Dim quadSide4 As SideType = SideType.none

                            'order new nodes to make cell creation easier - side types
                            'depend on cell orientation
                            Dim k As (Integer, Integer)

                            If config = 1 Or config = 4 Then

                                k = (vp, vs)
                                triSide2 = SideType.boundary
                                quadSide2 = e.SideType

                            Else

                                k = (vs, vp)
                                triSide3 = SideType.boundary
                                quadSide4 = e.SideType

                            End If

                            'put cell nodes into group, order as A, B, C in above diagram
                            Dim newNodes As (Integer, Integer, Integer)

                            'apex is first item, base is second and third
                            If positionVectors.R1.Y = positionVectors.R2.Y Then

                                newNodes = (nodes.N3, nodes.N1, nodes.N2)

                            ElseIf positionVectors.R2.Y = positionVectors.R3.Y Then

                                newNodes = (nodes.N1, nodes.N2, nodes.N3)

                            Else

                                newNodes = (nodes.N2, nodes.N3, nodes.N1)

                            End If

                            'the side type of the new quad horizontal edge
                            quadSide1 = IIf(data.NodeV(newNodes.Item2).R.Y = 0 Or
                            data.NodeV(newNodes.Item2).R.Y = farfield.Height, SideType.boundary, SideType.none)

                            'replace and add cells
                            factory.ReplaceCell(t, newId, newNodes.Item1, k.Item1, k.Item2,, triSide2, triSide3)
                            factory.AddQuad(newId + 1, newNodes.Item2, newNodes.Item3, k.Item2, k.Item1,
                                            quadSide1, quadSide2,, quadSide4)

                            newId += 2

                            'skip to the next cell
                            GoTo Next_Cell

                        End If

                    Next

                End If

                'interior cells 

                'add new node at cell center
                Dim vc As Integer = n
                n = CreateCenterNode(vc, t)

                'add nodes on each edge
                Dim r12 = FindMidPoint(data.CellList(t).Edge3, positionVectors)
                Dim v12 As Integer

                If data.Exists(r12) > 0 Then   'if there is already a node there make sure we don't overwrite it

                    'point vp to the existing node
                    v12 = data.FindNode(r12)

                Else                           'else create a new node

                    v12 = n                     'assign index to new node

                    'Create a new node and return incremented n
                    factory.AddNode(v12, r12)
                    n += 1

                End If


                Dim r23 = FindMidPoint(data.CellList(t).Edge1, positionVectors)
                Dim v23 As Integer

                If data.Exists(r23) > 0 Then

                    v23 = data.FindNode(r23)

                Else

                    v23 = n
                    factory.AddNode(v23, r23)
                    n += 1

                End If

                Dim r31 = FindMidPoint(data.CellList(t).Edge2, positionVectors)
                Dim v31

                If data.Exists(r31) > 0 Then

                    v31 = data.FindNode(r31)

                Else

                    v31 = n
                    factory.AddNode(v31, r31)
                    n += 1

                End If

                'side types in the existing triangle
                Dim s() As SideType = GetSideTypes(data.CellList(t))

                factory.ReplaceTriWithQuad(t, newId, nodes.N1, v12, vc, v31, s(2),,, s(1))
                factory.AddQuad(newId + 1, nodes.N2, v23, vc, v12, s(0),,, s(2))
                factory.AddQuad(newId + 2, nodes.N3, v31, vc, v23, s(1),,, s(0))

                newId += 3

Next_Cell:
            Next

        End Sub

        ''' <summary>
        ''' Refines a mesh by performing a trunc tiling. Each cell is divided by truncating it at each node.
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub DivideTrunc(farfield As Farfield) Implements ICellSplitter.DivideTrunc

            Dim numcells = data.CellList.Count
            Dim n = data.Nodelist.Count

            'current max id in celllist and new id for new cells
            Dim maxId = data.CellList.Max(Function(p) p.Id)
            Dim newId = maxId + 1

            'offset of nodes from center or side for octagons and hexagons
            Dim delta As Single

            If farfield.Gridtype = GridType.Quads Then

                delta = 0.5 / (1 + Math.Sqrt(2))

            Else

                delta = 1 / 6

            End If

            'cycle through 1 less than the count
            For t = 0 To numcells - 1

                Dim nodes As CellNodes = GetNodeDetails(t)
                Dim nodeTypeCollection As CellNodeTypes = GetNodeSurface(nodes)
                Dim positionVectors As CellNodeVectors = GetPositionVectors(nodes)

                Dim r(5, 2) As Vector2
                Dim vp(5, 2) As Integer

                'equilateral cells with edges on the left and right farfield boundaries
                If farfield.Gridtype = GridType.Equilateral Then

                    For Each e As Edge In data.CellList(t).Edges

                        'this identifies the left/right edge cells and vertical side
                        If e.R.X = 0 Or e.R.X = farfield.Width Then

                            Dim LR As Integer       'is cell aligned to the left edge or right edge?
                            Dim PSD As Integer      'is it pointy side down (is apex below the base)?

                            If e.R.X = 0 Then
                                LR = 1
                            ElseIf e.R.X = farfield.Width Then
                                LR = 2
                            End If

                            'create new node on vertical side - there is no risk of creating overlapping nodes.
                            vp(1, 1) = n

                            'use cell center location to determine orientation of cell
                            If data.CellList(t).R.Y > e.R.Y Then

                                PSD = 2       'apex is below base

                                'node nearer to top of cell
                                r(1, 1) = New Vector2(e.R.X, e.R.Y - delta * e.L)

                            Else

                                PSD = 0        'apex is above base

                                'node nearer to bottom of cell
                                r(1, 1) = New Vector2(e.R.X, e.R.Y + delta * e.L)

                            End If

                            n = CreateNewNode(e, vp(1, 1), r(1, 1), nodeTypeCollection)


                            'configuration determines the orientation of the cell
                            Dim config As Integer = LR + PSD

                            'determine long side and horizontal side
                            Dim longSide As Edge = data.FindLongestSide(t)
                            Dim h As Edge = FindHorizontalEdge(t, e, longSide)

                            'two new nodes will be created on the longest side and one on the horizontal side
                            If config = 1 Or config = 4 Then

                                r(2, 1) = Vector2.Subtract(longSide.R, delta * longSide.Lv)
                                r(2, 2) = Vector2.Add(longSide.R, delta * longSide.Lv)

                                r(3, 1) = Vector2.Add(h.R, delta * h.Lv)


                            Else     'config = 2 or 3

                                r(2, 1) = Vector2.Add(longSide.R, delta * longSide.Lv)
                                r(2, 2) = Vector2.Subtract(longSide.R, delta * longSide.Lv)

                                r(3, 1) = Vector2.Subtract(h.R, delta * h.Lv)

                            End If

                            'long side nodes
                            For j As Integer = 1 To 2
                                If data.Exists(r(2, j)) > 0 Then   'if there is already a node there make sure we don't overwrite it

                                    'point vp to the existing node
                                    vp(2, j) = data.FindNode(r(2, j))

                                Else                           'else create a new node (default and most common behavior)

                                    vp(2, j) = n                     'assign index to new node

                                    'Create a new node at the np point and return incremented n
                                    n = CreateNewNode(longSide, vp(2, j), r(2, j), nodeTypeCollection)

                                End If
                            Next

                            'horizontal node
                            If data.Exists(r(3, 1)) > 0 Then   'if there is already a node there make sure we don't overwrite it

                                'point vp to the existing node
                                vp(3, 1) = data.FindNode(r(3, 1))

                            Else                           'else create a new node (default and most common behavior)

                                vp(3, 1) = n                     'assign index to new node

                                'Create a new node at the np point and return incremented n
                                n = CreateNewNode(h, vp(3, 1), r(3, 1), nodeTypeCollection)

                            End If



                            'config = 1                       2
                            '
                            '               A                           A
                            '                |\                         /|
                            '        vp(1,1) |_\ vp(2,1)       vp(2,1) /_| vp(1,1)
                            '                |  \ vp(2,2)     vp(2,2) /  |
                            '                |__/\                   /\__|
                            '               C    B                  C    B
                            '                   vp(3,1)                vp(3,1)
                            '
                            ' 3                                4 
                            '                  vp(3,1)                vp(3,1)
                            '               B ____ C              B  ____ A
                            '                |  \/ vp(2,2)   vp(2,2) \/  |
                            '        vp(1,1) |__/ vp(2,1)     vp(2,1) \__| vp(1,1)
                            '                | /                       \ |
                            '                |/                         \|
                            '               A                            A

                            Dim triSide2 As SideType = SideType.none
                            Dim triSide3 As SideType = SideType.none

                            Dim pSide1 As SideType = h.SideType
                            Dim pSide2 As SideType = SideType.none
                            Dim pSide5 As SideType = SideType.none

                            'order new nodes to make cell creation easier - side types
                            'depend on cell orientation
                            Dim k As (Integer, Integer)
                            Dim m As (Integer, Integer)

                            If config = 1 Or config = 4 Then
                                k = (vp(2, 1), vp(1, 1))
                                m = (vp(3, 1), vp(2, 2))
                                triSide2 = SideType.boundary
                                pSide2 = e.SideType
                            Else
                                k = (vp(1, 1), vp(2, 1))
                                m = (vp(2, 2), vp(3, 1))
                                triSide3 = SideType.boundary
                                pSide5 = e.SideType
                            End If

                            'put cell nodes into group, order as A, B, C in above diagram
                            Dim newNodes As (Integer, Integer, Integer)

                            'apex is first item, base is second and third
                            If positionVectors.R1.Y = positionVectors.R2.Y Then

                                newNodes = (nodes.N3, nodes.N1, nodes.N2)

                            ElseIf positionVectors.R2.Y = positionVectors.R3.Y Then

                                newNodes = (nodes.N1, nodes.N2, nodes.N3)

                            Else

                                newNodes = (nodes.N2, nodes.N3, nodes.N1)

                            End If

                            'replace and add cells
                            factory.ReplaceCell(t, newId, newNodes.Item1, k.Item1, k.Item2,, triSide2, triSide3)

                            If config = 1 Or config = 4 Then

                                factory.AddCell(newId + 1, newNodes.Item2, vp(3, 1), vp(2, 2),,, h.SideType)
                                factory.AddPent(newId + 2, vp(3, 1), newNodes.Item3, vp(1, 1), vp(2, 1), vp(2, 2), h.SideType, pSide2,,,)

                            Else

                                factory.AddCell(newId + 1, newNodes.Item3, vp(2, 2), vp(3, 1), , h.SideType,)
                                factory.AddPent(newId + 2, newNodes.Item2, vp(3, 1), vp(2, 2), vp(2, 1), vp(1, 1), h.SideType,,,, pSide5)

                            End If

                            newId += 3

                            'skip to the next cell
                            GoTo Next_Cell

                        End If

                    Next

                End If

                'interior equilateral cells, all irregular triangle cells, all quad cells

                Dim i As Integer = 1

                'cycle through each edge and create new nodes
                For Each e As Edge In data.CellList(t).Edges

                    For j As Integer = 1 To 2

                        r(i, 1) = Vector2.Subtract(e.R, e.Lv * delta)
                        r(i, 2) = Vector2.Add(e.R, e.Lv * delta)

                        If data.Exists(r(i, j)) > 0 Then   'if there is already a node there make sure we don't overwrite it

                            'point vp to the existing node
                            vp(i, j) = data.FindNode(r(i, j))

                        Else                           'else create a new node

                            vp(i, j) = n               'assign index to new node

                            'Create a new node and return incremented n
                            factory.AddNode(vp(i, j), r(i, j))
                            n += 1

                        End If

                    Next

                    i += 1

                Next

                'side types in the existing triangle
                Dim s() As SideType = GetSideTypes(data.CellList(t))

                If farfield.Gridtype = GridType.Quads Then

                    factory.ReplaceCell(t, newId, nodes.N1, vp(1, 1), vp(4, 2),, s(3), s(0))
                    factory.AddCell(newId + 1, nodes.N2, vp(2, 1), vp(1, 2),, s(0), s(1))
                    factory.AddCell(newId + 2, nodes.N3, vp(3, 1), vp(2, 2),, s(1), s(2))
                    factory.AddCell(newId + 3, nodes.N4, vp(4, 1), vp(3, 2),, s(2), s(3))
                    factory.AddOct(newId + 4, vp(1, 1), vp(1, 2), vp(2, 1), vp(2, 2), vp(3, 1), vp(3, 2), vp(4, 1), vp(4, 2), s(0),, s(1),, s(2),, s(3),)

                    newId += 5


                Else            'equilateral or irregular triangles

                    factory.ReplaceCell(t, newId, nodes.N1, vp(3, 1), vp(2, 2),, s(1), s(2))
                    factory.AddCell(newId + 1, nodes.N2, vp(1, 1), vp(3, 2),, s(2), s(0))
                    factory.AddCell(newId + 2, nodes.N3, vp(2, 1), vp(1, 2),, s(0), s(1))
                    factory.AddHex(newId + 3, vp(1, 1), vp(1, 2), vp(2, 1), vp(2, 2), vp(3, 1), vp(3, 2), (0),, s(1),, (2),)

                    newId += 4

                End If


Next_Cell:
            Next

        End Sub

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Returns an ordered node pair/edge tuple
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Private Function GetNodePairList(t As Integer) As List(Of (nA As Integer, nB As Integer, sName As SideName, sType As SideType))

            'get node details for cell
            Dim nodes As CellNodes = GetNodeDetails(t)

            'side names and types in the existing triangle
            Dim s() As SideName = GetSideNames(data.CellList(t))
            Dim p() As SideType = GetSideTypes(data.CellList(t))

            If data.CellList(t).CellType = CellType.triangle Then

                Dim nodePairs As New List(Of (nA As Integer, nB As Integer, sName As SideName, sType As SideType)) _
                    From {(nodes.N2, nodes.N3, s(0), p(0)),
                          (nodes.N3, nodes.N1, s(1), p(1)),
                          (nodes.N1, nodes.N2, s(2), p(2))}

                Return nodePairs

            ElseIf data.CellList(t).CellType = CellType.quad Then

                Dim nodePairs As New List(Of (nA As Integer, nB As Integer, sName As SideName, sType As SideType)) _
                    From {(nodes.N1, nodes.N2, s(0), p(0)),
                          (nodes.N2, nodes.N3, s(1), p(1)),
                          (nodes.N3, nodes.N4, s(2), p(2)),
                          (nodes.N4, nodes.N1, s(3), p(3))}

                Return nodePairs

            Else

                Throw New Exception()

            End If

        End Function

        ''' <summary>
        ''' Gets the node ids of the vertices of the given triangular cell
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
        ''' Inspects a triangular cell to determine if any of the edges are vertical
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
        ''' Creates a new node at the center of a given cell
        ''' </summary>
        ''' <param name="vp"></param>
        ''' <param name="rP"></param>
        ''' <returns></returns>
        Private Function CreateCenterNode(n As Integer, t As Integer) As Integer

            Dim rP As Vector2 = data.CellList(t).R

            'add node
            factory.RequestNode(n, rP, False, False)

            Return n + 1

        End Function

        ''' <summary>
        ''' Creates a new node on the given edge of a triangular cell
        ''' </summary>
        ''' <param name="e"></param>
        ''' <param name="n"></param>
        ''' <returns>incremented node index (for next node)</returns>
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
        ''' Creates a new node on the given edge of a quad cell
        ''' </summary>
        ''' <param name="e"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Private Function CreateNewNodeQuad(e As Edge, vp As Integer, rP As Vector2) As Integer

            'For quad cells, the new node always inherits the surface/boundary flags of its edge
            factory.RequestNode(vp, rP, False, IIf(e.SideType = SideType.boundary, True, False))

            Return vp + 1

        End Function

        ''' <summary>
        ''' Creates a new node at the center of a quad cell
        ''' </summary>
        ''' <param name="n"></param>
        ''' <param name="positionVectors"></param>
        ''' <returns></returns>
        Private Function CreateNewCenterNodeQuad(n As Integer, positionVectors As CellNodeVectors) As Integer

            'position vector of the center point
            Dim rP As Vector2 = Vector2.Add(positionVectors.R1, Vector2.Add(positionVectors.R2, Vector2.Add(positionVectors.R3, positionVectors.R4))) * 0.25

            'add node
            factory.RequestNode(n, rP, False, False)

            Return n + 1

        End Function

        ''' <summary>
        ''' Replaces an existing triangular with two additional new cells
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="e"></param>
        ''' <param name="newid"></param>
        ''' <returns></returns>
        Private Function DivideCells(t As Integer, e As Edge, newid As Integer, vp As Integer, n As CellNodes) As Integer

            'side types in the existing triangle
            Dim s() As SideType = GetSideTypes(data.CellList(t))

            'refer to the diagram at the SplitCells sub for better understanding of what is being done.
            Select Case e.SideName
                Case SideName.S1
                    'The new face must always be of SideType.none : other faces inherit their existing state.
                    factory.ReplaceCell(t, newid, n.N1, n.N2, vp, s(0),, s(2))
                    factory.AddCell(newid + 1, n.N1, vp, n.N3, s(0), s(1),)
                Case SideName.S2
                    factory.ReplaceCell(t, newid, vp, n.N2, n.N3, s(0), s(1),)
                    factory.AddCell(newid + 1, n.N1, n.N2, vp,, s(1), s(2))
                Case SideName.S3
                    factory.ReplaceCell(t, newid, n.N1, vp, n.N3,, s(1), s(2))
                    factory.AddCell(newid + 1, vp, n.N2, n.N3, s(0),, s(2))
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

            'side types in the existing triangle
            Dim s() As SideType = GetSideTypes(data.CellList(t))

            'replace the original cell
            factory.ReplaceCell(t, newid, n.N1, m(2), m(1),, s(1), s(2))

            'add three new cells
            factory.AddCell(newid + 1, n.N2, m(0), m(2),, s(2), s(0))
            factory.AddCell(newid + 2, n.N3, m(1), m(0),, s(0), s(1))
            factory.AddCell(newid + 3, m(0), m(1), m(2),,,)

            Return newid + 4

        End Function

        ''' <summary>
        ''' Replace an existing rectangular cell instance with four new rectangular cells
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="e"></param>
        ''' <param name="newid"></param>
        ''' <returns></returns>
        Private Function DivideCellsQuads(t As Integer, newid As Integer, n As CellNodes, m As List(Of Integer), c As Integer) As Integer

            'side types in the existing triangle
            Dim s() As SideType = GetSideTypes(data.CellList(t))

            'replace the original cell
            factory.ReplaceCellQuad(t, newid, n.N1, m(0), c, m(3), s(0),,, s(3))

            'add three new cells
            factory.AddQuad(newid + 1, m(0), n.N2, m(1), c, s(0), s(1),,)
            factory.AddQuad(newid + 2, c, m(1), n.N3, m(2),, s(1), s(2),)
            factory.AddQuad(newid + 3, m(3), c, m(2), n.N4,,, s(2), s(3))

            Return newid + 4

        End Function

        ''' <summary>
        ''' Finds the horizontal edge in a euilateral triangle grid boundary cell
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="e"></param>
        ''' <param name="longSide"></param>
        ''' <returns></returns>
        Private Function FindHorizontalEdge(t As Integer, e As Edge, longSide As Edge) As Edge

            Dim h As Edge = (From l In data.CellList(t).Edges
                             Where l.SideName <> e.SideName And l.SideName <> longSide.SideName
                             Select l).FirstOrDefault
            Return h

        End Function

#End Region

    End Class
End Namespace