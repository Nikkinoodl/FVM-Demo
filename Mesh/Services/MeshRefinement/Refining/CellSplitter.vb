Imports Core.Interfaces
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
            Dim n = data.Nodelist.Count()

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
                Dim result As (newN As Integer, vP As Integer) = FindOrCreateNode(longSide, n, rP, nodeTypeCollection)

                n = result.newN
                Dim vP = result.vP

                'split the cell between the new node and its opposing vertex and return incremented newId
                newId = DivideCells(t, longSide, newId, vP, nodes)

            Next

            'complete the cell splitting prcess
            FindOrphanNodes()

        End Sub

        ''' <summary>
        ''' Refines an equilateral triangle grid by dividing each cell into four equal triangles
        ''' </summary>
        Public Sub DivideEquilateral() Implements ICellSplitter.DivideEquilateral

            Dim numcells = data.CellList.Count()
            Dim n = data.Nodelist.Count()

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

                    'place a node at the mid point of longest side
                    Dim longSide As Edge = data.FindLongestSide(t)
                    Dim rP = FindMidPoint(longSide, positionVectors)
                    Dim result As (newN As Integer, vP As Integer) = FindOrCreateNode(longSide, n, rP, nodeTypeCollection)

                    n = result.newN
                    Dim vP = result.vP

                    'split the cell between the new node and its opposing vertex and return incremented newId
                    newId = DivideCells(t, longSide, newId, vP, nodes)

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
                            longSide = FindLongSide(c, positionVectors)

                            'place node at mid point of longest side
                            rP = FindMidPoint(longSide, positionVectors)

                            result = FindOrCreateNode(longSide, n, rP, nodeTypeCollection)
                            n = result.newN
                            vP = result.vP

                            'split the cell between the new node and its opposing vertex and return incremented newId
                            newId = DivideCells(c, longSide, newId, vP, nodes)

                            Exit For

                        End If

                    Next

                Else

                    'otherwise treat as a standard equilateral triangle
                    For Each e As Edge In data.CellList(t).Edges

                        Dim rP = FindMidPoint(e, positionVectors)
                        Dim result As (newN As Integer, vP As Integer) = FindOrCreateNode(e, n, rP, nodeTypeCollection)

                        n = result.newN
                        newNodes.Add(result.vP)

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

            Dim numcells = data.CellList.Count()
            Dim n = data.Nodelist.Count()

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
                    Dim vP As Integer

                    If data.Exists(rP) > 0 Then           'if there is an orphan node

                        'point vP to the existing node
                        vP = data.FindNode(rP)

                    Else                                   'create new mid point node

                        vP = n        'assign index to new node

                        n = CreateNewNodeQuad(e, vP, rP)

                    End If

                    'add vP to a new node list
                    centerNodes.Add(vP)

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

            Dim numcells = data.CellList.Count()
            Dim n = data.Nodelist.Count()

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

            Dim numcells = data.CellList.Count()
            Dim n = data.Nodelist.Count()

            'current max id in celllist and new id for new cells
            Dim maxId = data.CellList.Max(Function(p) p.Id)
            Dim newId = maxId + 1

            'cycle through 1 less than the count
            For t = 0 To numcells - 1

                Dim nodes As CellNodes = GetNodeDetails(t)
                Dim nodeTypeCollection As CellNodeTypes = GetNodeSurface(nodes)
                Dim positionVectors As CellNodeVectors = GetPositionVectors(nodes)

                Dim result As (newN As Integer, vP As Integer)

                'equilateral cells with edges on the left and right farfield boundaries
                'are divided as if they continued outside the boundary.
                If farfield.Gridtype = GridType.Equilateral Then

                    For Each e As Edge In data.CellList(t).Edges

                        Dim LR = LeftOrRightEdge(e, farfield) 'is cell aligned to the left edge or right edge?

                        'this identifies the left/right edge cells and vertical side
                        If LR > 0 Then

                            Dim PSD As Integer      'is it pointy side down (is apex below the base)?
                            Dim vS As Integer = n

                            'use cell center location to determine orientation of cell
                            If data.CellList(t).R.Y > e.R.Y Then

                                PSD = 2       'apex is below base

                                'add new node nearer to top of cell
                                n = CreateNewNode(e, vS, New Vector2(e.R.X, e.R.Y + 0.18 * e.L), nodeTypeCollection)

                            Else

                                PSD = 0        'apex is above base

                                'add new node nearer to bottom of cell
                                n = CreateNewNode(e, vS, New Vector2(e.R.X, e.R.Y - 0.18 * e.L), nodeTypeCollection)

                            End If

                            'place a node at the mid point of longest side
                            Dim longSide As Edge = data.FindLongestSide(t)
                            Dim rP = FindMidPoint(longSide, positionVectors)
                            result = FindOrCreateNode(longSide, n, rP, nodeTypeCollection)

                            n = result.newN
                            Dim vP = result.vP

                            'configuration determines the orientation of this edge cell
                            Dim config As Integer = LR + PSD

                            'config = 1                       2
                            '
                            '               A                           A
                            '                |\                         /|
                            '             vS |_\ vP                 vP /_| vS
                            '                |  \                     /  |
                            '                |___\                   /___|
                            '               C    B                  C    B
                            '
                            '
                            ' 3                                4 
                            '
                            '               B ____ C              B  ____ C
                            '                |   /                   \   |
                            '             vS |__/ vP                vP\__| vS
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

                                k = (vP, vS)
                                triSide2 = SideType.boundary
                                quadSide2 = e.SideType

                            Else

                                k = (vS, vP)
                                triSide3 = SideType.boundary
                                quadSide4 = e.SideType

                            End If

                            Dim newNodes = EdgeCellNodes(nodes, positionVectors)

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
                Dim vC As Integer = n
                n = CreateCenterNode(vC, t)

                'add nodes on each edge
                Dim r12 = FindMidPoint(data.CellList(t).Edge3, positionVectors)
                Dim r23 = FindMidPoint(data.CellList(t).Edge1, positionVectors)
                Dim r31 = FindMidPoint(data.CellList(t).Edge2, positionVectors)

                'new node on side 3
                result = FindOrCreateNode(n, r12)
                n = result.newN
                Dim v12 As Integer = result.vP

                'new node on side 1
                result = FindOrCreateNode(n, r23)
                n = result.newN
                Dim v23 As Integer = result.vP

                'new node on side 2
                result = FindOrCreateNode(n, r31)
                n = result.newN
                Dim v31 As Integer = result.vP

                'side types in the existing triangle
                Dim s() As SideType = GetSideTypes(data.CellList(t))

                factory.ReplaceTriWithQuad(t, newId, nodes.N1, v12, vC, v31, s(2),,, s(1))
                factory.AddQuad(newId + 1, nodes.N2, v23, vC, v12, s(0),,, s(2))
                factory.AddQuad(newId + 2, nodes.N3, v31, vC, v23, s(1),,, s(0))

                newId += 3

Next_Cell:
            Next

        End Sub

        ''' <summary>
        ''' Refines a mesh by performing a trunc tiling. Each cell is divided by truncating it at each node.
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub DivideTrunc(farfield As Farfield) Implements ICellSplitter.DivideTrunc

            Dim numcells = data.CellList.Count()
            Dim n = data.Nodelist.Count()

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
                Dim result As (newN As Integer, vP As Integer)

                Dim r(5, 2) As Vector2
                Dim vP(5, 2) As Integer

                'equilateral cells with edges on the left and right farfield boundaries
                If farfield.Gridtype = GridType.Equilateral Then

                    For Each e As Edge In data.CellList(t).Edges

                        Dim LR = LeftOrRightEdge(e, farfield)   'is cell aligned to the left edge or right edge?

                        'cells on the edge
                        If LR > 0 Then

                            Dim PSD As Integer      'is it pointy side down (is apex below the base)?

                            'determine long side and horizontal side (use array for horizontal side as we
                            'will be overwriting it)
                            Dim longSide As Edge = data.FindLongestSide(t)
                            Dim h() As Edge = data.FindHorizontalEdge(t, e, longSide)

                            'create new node on vertical side - there is no risk of creating overlapping nodes.
                            vP(1, 1) = n

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

                            n = CreateNewNode(e, vP(1, 1), r(1, 1), nodeTypeCollection)

                            'configuration determines the orientation of the cell
                            Dim config As Integer = LR + PSD

                            'two new nodes will be created on the longest side and one on the horizontal side
                            If config = 1 Or config = 4 Then

                                r(2, 1) = longSide.R - delta * longSide.Lv
                                r(2, 2) = longSide.R + delta * longSide.Lv

                                r(3, 1) = h(0).R + delta * h(0).Lv

                            Else     'config = 2 or 3

                                r(2, 1) = longSide.R + delta * longSide.Lv
                                r(2, 2) = longSide.R - delta * longSide.Lv

                                r(3, 1) = h(0).R - delta * h(0).Lv

                            End If

                            'add long side nodes
                            For j As Integer = 1 To 2

                                result = FindOrCreateNode(longSide, n, r(2, j), nodeTypeCollection)

                                n = result.newN
                                vP(2, j) = result.vP

                            Next

                            'add horizontal node
                            result = FindOrCreateNode(h(0), n, r(3, 1), nodeTypeCollection)

                            n = result.newN
                            vP(3, 1) = result.vP

                            '** Diagram showing how left and right edge cells are truncated **

                            'config = 1                       2
                            '
                            '               A                           A
                            '                |\                         /|
                            '        vP(1,1) |_\ vP(2,1)       vP(2,1) /_| vP(1,1)
                            '                |  \ vP(2,2)     vP(2,2) /  |
                            '                |__/\                   /\__|
                            '               C    B                  C    B
                            '                   vP(3,1)                vP(3,1)
                            '
                            ' 3                                4 
                            '                  vP(3,1)                vP(3,1)
                            '               B ____ C              B  ____ C
                            '                |  \/ vP(2,2)   vP(2,2) \/  |
                            '        vP(1,1) |__/ vP(2,1)     vP(2,1) \__| vP(1,1)
                            '                | /                       \ |
                            '                |/                         \|
                            '               A                            A

                            'put cell nodes into group, order as A, B, C in above diagram
                            Dim newNodes = EdgeCellNodes(nodes, positionVectors)

                            'replace and add cells
                            If config = 1 Or config = 4 Then

                                'replace cell
                                factory.ReplaceCell(t, newId, newNodes.Item1, vP(2, 1), vP(1, 1),, SideType.boundary,)

                                'add new triangle, first node at 'B' or 'C' vertex depending on configuration
                                factory.AddCell(newId + 1, newNodes.Item2, vP(3, 1), vP(2, 2),,,)

                                'add new pentagon - first node at 'B' or 'C' vertex depending on configuration
                                factory.AddPent(newId + 2, newNodes.Item3, vP(1, 1), vP(2, 1), vP(2, 2), vP(3, 1), SideType.boundary,,,,)

                            Else     'config 2 or 3

                                factory.ReplaceCell(t, newId, newNodes.Item1, vP(1, 1), vP(2, 1),,, SideType.boundary)
                                factory.AddCell(newId + 1, newNodes.Item3, vP(2, 2), vP(3, 1),,,)
                                factory.AddPent(newId + 2, newNodes.Item2, vP(3, 1), vP(2, 2), vP(2, 1), vP(1, 1),,,,, SideType.boundary)

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

                        r(i, 1) = e.R - e.Lv * delta
                        r(i, 2) = e.R + e.Lv * delta

                        result = FindOrCreateNode(n, r(i, j))

                        n = result.newN
                        vP(i, j) = result.vP

                    Next

                    i += 1

                Next

                'side types in the existing triangle
                Dim s() As SideType = GetSideTypes(data.CellList(t))

                If farfield.Gridtype = GridType.Quads Then

                    factory.ReplaceCell(t, newId, nodes.N1, vP(1, 1), vP(4, 2),, s(3), s(0))
                    factory.AddCell(newId + 1, nodes.N2, vP(2, 1), vP(1, 2),, s(0), s(1))
                    factory.AddCell(newId + 2, nodes.N3, vP(3, 1), vP(2, 2),, s(1), s(2))
                    factory.AddCell(newId + 3, nodes.N4, vP(4, 1), vP(3, 2),, s(2), s(3))
                    factory.AddOct(newId + 4, vP(1, 1), vP(1, 2), vP(2, 1), vP(2, 2), vP(3, 1), vP(3, 2), vP(4, 1), vP(4, 2), s(0),, s(1),, s(2),, s(3),)

                    newId += 5


                Else            'equilateral or irregular triangles

                    factory.ReplaceCell(t, newId, nodes.N1, vP(3, 1), vP(2, 2),, s(1), s(2))
                    factory.AddCell(newId + 1, nodes.N2, vP(1, 1), vP(3, 2),, s(2), s(0))
                    factory.AddCell(newId + 2, nodes.N3, vP(2, 1), vP(1, 2),, s(0), s(1))
                    factory.AddHex(newId + 3, vP(1, 1), vP(1, 2), vP(2, 1), vP(2, 2), vP(3, 1), vP(3, 2), s(0),, s(1),, s(2),)

                    newId += 4

                End If

Next_Cell:
            Next

        End Sub

        ''' <summary>
        ''' Combines corner offcuts of the quad trunc tiling operations into single cells
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub CombineQuadGrid(farfield As Farfield) Implements ICellSplitter.CombineQuadGrid

            'current max id in celllist and new id for cell inserts
            Dim maxId = data.CellList.Max(Function(p) p.Id)
            Dim newId = maxId + 1

            'first transform interior clusters into quads
            For Each n As Node In data.InteriorNodes

                Dim cluster As List(Of Cell) = data.CellCluster(n.Id, CellType.triangle)
                Dim size = data.CellClusterCount(n.Id, CellType.triangle)

                If size = 4 Then            'interior cluster

                    newId = CombineCluster(newId, n, cluster, size)

                End If

            Next

            'second, transform edge clusters into triangles
            For Each n As Node In data.BoundaryNode(farfield)

                Dim cluster As List(Of Cell) = data.CellCluster(n.Id, CellType.triangle)
                Dim size = data.CellClusterCount(n.Id, CellType.triangle)

                If size = 2 Then        'edge cases

                    newId = CombineEdgeCluster(newId, n, cluster, size, farfield)

                End If

            Next

        End Sub

        ''' <summary>
        ''' Combines corner offcuts of the triangular trunc tiling operations into single cells
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub CombineTriangleGrid(farfield As Farfield) Implements ICellSplitter.CombineTriangleGrid

            'current max id in celllist and new id for cell inserts
            Dim maxId = data.CellList.Max(Function(p) p.Id)
            Dim newId = maxId + 1

            'first, transform interior clusters into hexagons
            For Each n As Node In data.InteriorNodes

                Dim cluster As List(Of Cell) = data.CellCluster(n.Id, CellType.triangle)
                Dim size = data.CellClusterCount(n.Id, CellType.triangle)

                If size = 6 Then        'interior cluster

                    newId = CombineCluster(newId, n, cluster, size)

                End If

            Next

            'Second, transform clusters on the farfield edge into quads And pentagons
            For Each n As Node In data.BoundaryNode(farfield)

                Dim cluster As List(Of Cell) = data.CellCluster(n.Id, CellType.triangle)
                Dim size = data.CellClusterCount(n.Id, CellType.triangle)

                If size > 1 Then  'edge case (3 or 4), corner case (2)

                    newId = CombineEdgeCluster(newId, n, cluster, size, farfield)

                End If

            Next

        End Sub

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Combines a cluster of triangular cells in the interior of the farfield
        ''' into a single quad cell
        ''' </summary>
        ''' <param name="newId"></param>
        ''' <param name="n"></param>
        ''' <param name="cluster"></param>
        ''' <param name="size"></param>
        ''' <returns></returns>
        Private Function CombineCluster(newId As Integer, n As Node, cluster As List(Of Cell), size As Integer) As Integer

            'select distinct nodes in cluster, exclude n  (Id, R, angle to first node)
            Dim nodeArray As New List(Of (Integer, Vector2, Double))

            For Each c As Cell In cluster

                'read cell nodes into array
                Dim nArray() As Integer = GetNodes(c)

                'arrays for vectors
                Dim rArray(3) As Vector2

                'loop through the array of cell nodes
                For i As Integer = 0 To 2

                    'skip the center node
                    If nArray(i) <> n.Id Then

                        Dim j = i

                        'skip duplicates already in the new array
                        If nodeArray.Any(Function(x) x.Item1 = nArray(j)) = False Then

                            'position vector of node
                            rArray(i) = data.NodeV(nArray(i)).R

                            'find the angle between the vector joining the center to the node and the Y axis
                            Dim theta = CalcAngleToYAxis(rArray(i), n)

                            nodeArray.Add((nArray(i), rArray(i), theta))

                        End If

                    End If

                Next

            Next

            'sort by theta
            Dim sortedNodes = nodeArray.OrderBy(Function(x) x.Item3)
            Dim nodeCount = sortedNodes.Count()

            If nodeCount = 4 Then

                'replace first cell with new quad - all sidetypes default to sidetype.none
                factory.ReplaceTriWithQuad(data.CellList.IndexOf(cluster(0)), newId, sortedNodes(0).Item1, sortedNodes(1).Item1, sortedNodes(2).Item1, sortedNodes(3).Item1,,,,)

            ElseIf nodeCount = 6 Then

                'replace first cell with new hexagon - all sidetypes default to sidetype.none
                factory.ReplaceTriWithHex(data.CellList.IndexOf(cluster(0)), newId, sortedNodes(0).Item1, sortedNodes(1).Item1, sortedNodes(2).Item1, sortedNodes(3).Item1, sortedNodes(4).Item1, sortedNodes(5).Item1,,,,,,)

            End If

            'delete remaining cells
            For i As Integer = 1 To size - 1

                factory.DeleteCell(data.CellList.IndexOf(cluster(i)))

            Next

            'delete shared node
            factory.DeleteNode(n)

            Return newId + 1

        End Function

        ''' <summary>
        ''' Combines a cluster of triangular cells at the farfield boundary into a single cell
        ''' </summary>
        ''' <param name="newId"></param>
        ''' <param name="n"></param>
        ''' <param name="cluster"></param>
        ''' <param name="size"></param>
        ''' <returns></returns>
        Private Function CombineEdgeCluster(newId As Integer, n As Node, cluster As List(Of Cell),
                                            size As Integer, farfield As Farfield) As Integer


            'combine distinct cluster nodes into a single array (Id, R, angle to first node)
            Dim nodeArray As New List(Of (Integer, Vector2, Double))

            For Each c As Cell In cluster

                'read cell nodes into array
                Dim nArray() As Integer = GetNodes(c)

                'array for node position vectors
                Dim rArray(3) As Vector2

                'loop through the array of nodes
                For i As Integer = 0 To 2

                    'shared node is only retained if it sits in a corner
                    If nArray(i) = n.Id And IsCornerNode(nArray(i), farfield) = False Then

                        Continue For

                    End If

                    Dim j = i

                    If nodeArray.Any(Function(x) x.Item1 = nArray(j)) = False Then   'skip duplicate nodes

                        'position vector of node
                        rArray(i) = data.NodeV(nArray(i)).R

                        'find the angle between the vector joining the cluster center to node n and the Y axis
                        'this calc works better if the center of rotation is slightly pulled back from n
                        Dim theta = CalcAngleToYAxis(rArray(i), 0.99 * n.R)

                        nodeArray.Add((nArray(i), rArray(i), theta))

                    End If

                Next

            Next

            'sort by theta
            Dim sortedNodes = nodeArray.OrderBy(Function(x) x.Item3)

            'replace original cluster with a single cell
            Dim nodeCount = sortedNodes.Count()
            Dim s(nodeCount) As SideType

            'must use number of nodes to determine new geometry
            If nodeCount = 3 Then   'combine into triangle

                s(0) = IIf(data.NodeV(sortedNodes(1).Item1).Boundary And data.NodeV(sortedNodes(2).Item1).Boundary, SideType.boundary, SideType.none)
                s(1) = IIf(data.NodeV(sortedNodes(0).Item1).Boundary And data.NodeV(sortedNodes(2).Item1).Boundary, SideType.boundary, SideType.none)
                s(2) = IIf(data.NodeV(sortedNodes(0).Item1).Boundary And data.NodeV(sortedNodes(1).Item1).Boundary, SideType.boundary, SideType.none)

                'convert first cell in cluster to fill whole cluster space
                factory.ReplaceCell(data.CellList.IndexOf(cluster(0)), newId, sortedNodes(0).Item1, sortedNodes(1).Item1, sortedNodes(2).Item1, s(0), s(1), s(2))

            Else

                For i As Integer = 0 To nodeCount - 1

                    If i < nodeCount - 1 Then

                        s(i) = IIf(data.NodeV(sortedNodes(i).Item1).Boundary And data.NodeV(sortedNodes(i + 1).Item1).Boundary, SideType.boundary, SideType.none)

                    Else      'close the loop

                        s(i) = IIf(data.NodeV(sortedNodes(i).Item1).Boundary And data.NodeV(sortedNodes(0).Item1).Boundary, SideType.boundary, SideType.none)

                    End If

                Next

                If nodeCount = 4 Then

                    'convert first cell in cluster to fill whole cluster space
                    factory.ReplaceTriWithQuad(data.CellList.IndexOf(cluster(0)), newId, sortedNodes(0).Item1, sortedNodes(1).Item1, sortedNodes(2).Item1, sortedNodes(3).Item1, s(0), s(1), s(2), s(3))

                ElseIf nodeCount = 5 Then

                    'convert first cell in cluster to fill whole cluster space
                    factory.ReplaceTriWithPent(data.CellList.IndexOf(cluster(0)), newId, sortedNodes(0).Item1, sortedNodes(1).Item1, sortedNodes(2).Item1, sortedNodes(3).Item1, sortedNodes(4).Item1, s(0), s(1), s(2), s(3), s(4))

                End If

            End If

            'delete any remaining cells that were part of the cluster
            For i As Integer = 1 To size - 1

                factory.DeleteCell(data.CellList.IndexOf(cluster(i)))

            Next

            'delete shared node if not in a corner
            If IsCornerNode(n.Id, farfield) = False Then

                factory.DeleteNode(n)

            End If

            Return newId + 1

        End Function

        ''' <summary>
        ''' Replaces an existing triangular with two additional new cells
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="e"></param>
        ''' <param name="newid"></param>
        ''' <returns></returns>
        Private Function DivideCells(t As Integer, e As Edge, newid As Integer, vP As Integer, n As CellNodes) As Integer

            'side types in the existing triangle
            Dim s() As SideType = GetSideTypes(data.CellList(t))

            'refer to the diagram at the SplitCells sub for better understanding of what is being done.
            Select Case e.SideName

                Case SideName.S1

                    'The new face must always be of SideType.none : other faces inherit their existing state.
                    factory.ReplaceCell(t, newid, n.N1, n.N2, vP, s(0),, s(2))
                    factory.AddCell(newid + 1, n.N1, vP, n.N3, s(0), s(1),)

                Case SideName.S2

                    factory.ReplaceCell(t, newid, vP, n.N2, n.N3, s(0), s(1),)
                    factory.AddCell(newid + 1, n.N1, n.N2, vP,, s(1), s(2))

                Case SideName.S3

                    factory.ReplaceCell(t, newid, n.N1, vP, n.N3,, s(1), s(2))
                    factory.AddCell(newid + 1, vP, n.N2, n.N3, s(0),, s(2))

                Case Else

                    Throw New Exception

            End Select

            Return newid + 2

        End Function

        ''' <summary>
        ''' Finds triangular cells that have an orphan node on one edge and splits them at the orphan node
        ''' </summary>
        Private Sub FindOrphanNodes()

            Dim numcells = data.CellList.Count()
            Dim n = data.Nodelist.Count()

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
                    Dim vP As Integer

                    If data.Exists(rP) > 0 Then           'if there is an orphan node

                        'point vP to the existing node
                        vP = data.FindNode(rP)

                        'Split the existing cell in two and return incremented newId
                        newId = DivideCells(t, e, newId, vP, nodes)

                        Continue For

                    End If

                Next

            Next

        End Sub
#End Region

#Region "Private Utilities"

        ''' <summary>
        ''' Returns a list of ordered node pair/edge tuples
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Private Function GetNodePairList(t As Integer) As List(Of (nA As Integer, nB As Integer, sName As SideName, sType As SideType))

            Dim c As Cell = data.CellList(t)
            Dim nodePairs As New List(Of (nA As Integer, nB As Integer, sName As SideName, sType As SideType))

            'create a dictionary to assign node pairs based on matching cell type and side name
            Dim nodeAssignment = CreateNodeAssignment(c)

            For Each e As Edge In c.Edges

                Dim key As Tuple(Of CellType, SideName) = Tuple.Create(c.CellType, e.SideName)
                Dim value As (Integer?, Integer?) = Nothing

                If nodeAssignment.TryGetValue(key, value) Then

                    Dim result As (Integer, Integer) = value

                    nodePairs.Add((result.Item1, result.Item2, e.SideName, e.SideType))

                Else

                    Throw New Exception()

                End If

            Next

            Return nodePairs

        End Function

        ''' <summary>
        ''' Gets the node ids of the vertices of the given triangular or quad cell
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
        ''' Gets the position vectors of the vertices of a given triangular or quad cell
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
            Dim pV As CellNodeVectors = GetPositionVectors(nodes)

            Dim vectors As New List(Of Vector2) From {
                pV.R3 - pV.R2,
                pV.R1 - pV.R3,
                pV.R2 - pV.R1}

            Return vectors.Any(Function(v) Vector2.Dot(v, Vector2.UnitX) = 0)

        End Function

        ''' <summary>
        ''' Returns the longest edge of a cell when the cell lengths have not yet been calculated
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="positionVectors"></param>
        ''' <returns></returns>
        Private Function FindLongSide(t As Integer, pV As CellNodeVectors) As Edge

            Dim lengths As New List(Of Tuple(Of Double, Edge)) From {
                New Tuple(Of Double, Edge)((pV.R3 - pV.R2).Length(), data.CellList(t).Edge1),
                New Tuple(Of Double, Edge)((pV.R1 - pV.R3).Length(), data.CellList(t).Edge2),
                New Tuple(Of Double, Edge)((pV.R2 - pV.R1).Length(), data.CellList(t).Edge3)}

            Return lengths.OrderByDescending(Function(tuple) tuple.Item1).First().Item2

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
                    .S3 = data.NodeV(n.N3).Surface}

            Return result

        End Function

        ''' <summary>
        ''' Creates a new node at the center of a given cell
        ''' </summary>
        ''' <param name="vP"></param>
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
        Private Function CreateNewNode(e As Edge, vP As Integer, rP As Vector2, s As CellNodeTypes) As Integer

            Dim sideNodeMap As New Dictionary(Of SideName, Tuple(Of Boolean, Boolean)) From {
                {SideName.S1, New Tuple(Of Boolean, Boolean)(s.S2, s.S3)},
                {SideName.S2, New Tuple(Of Boolean, Boolean)(s.S1, s.S3)},
                {SideName.S3, New Tuple(Of Boolean, Boolean)(s.S2, s.S1)}}

            Dim value As Tuple(Of Boolean, Boolean) = Nothing

            If Not sideNodeMap.TryGetValue(e.SideName, value) Then Throw New Exception

            factory.RequestNode(vP, rP, value.Item1 And value.Item2, e.SideType = SideType.boundary)

            Return vP + 1

        End Function

        ''' <summary>
        ''' Creates a new node on the given edge of a quad cell
        ''' </summary>
        ''' <param name="e"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Private Function CreateNewNodeQuad(e As Edge, vP As Integer, rP As Vector2) As Integer

            'For quad cells, the new node always inherits the surface/boundary flags of its edge
            factory.RequestNode(vP, rP, False, e.SideType = SideType.boundary)

            Return vP + 1

        End Function

        ''' <summary>
        ''' Looks for a node at the given point. If not found, creates a new node.
        ''' Returns both the running next node Id and the Id of the found/created node. Use for interior
        ''' nodes when node boundary/surface flags are unnecessary.
        ''' </summary>
        ''' <param name="e"></param>
        ''' <param name="n"></param>
        ''' <param name="rp"></param>
        ''' <param name="nodeTypeCollection"></param>
        ''' <returns></returns>
        Private Function FindOrCreateNode(e As Edge, n As Integer, rP As Vector2, nodeTypeCollection As CellNodeTypes) As (Integer, Integer)

            Dim vP As Integer

            If data.Exists(rP) > 0 Then          'if there is an orphan node

                'point vP to the existing node
                vP = data.FindNode(rP)

            Else                                  'create new mid point node

                vP = n        'assign index to new node
                n = CreateNewNode(e, vP, rP, nodeTypeCollection)

            End If

            Return (n, vP)

        End Function

        ''' <summary>
        ''' Looks for a node at the given point. If not found, creates a new node.
        ''' Returns both the running next node Id and the Id of the found/created node. Use for interior
        ''' nodes when node boundary/surface flags are unnecessary.
        ''' </summary>
        ''' <param name="e"></param>
        ''' <param name="n"></param>
        ''' <param name="rp"></param>
        ''' <param name="nodeTypeCollection"></param>
        ''' <returns></returns>
        Private Function FindOrCreateNode(n As Integer, rP As Vector2) As (Integer, Integer)

            Dim vP As Integer

            If data.Exists(rP) > 0 Then          'if there is an orphan node

                'point vP to the existing node
                vP = data.FindNode(rP)

            Else                                  'create new mid point node

                vP = n        'assign index to new node
                factory.AddNode(vP, rP)
                n += 1

            End If

            Return (n, vP)

        End Function

        ''' <summary>
        ''' Creates a new node at the center of a quad cell
        ''' </summary>
        ''' <param name="n"></param>
        ''' <param name="positionVectors"></param>
        ''' <returns></returns>
        Private Function CreateNewCenterNodeQuad(n As Integer, positionVectors As CellNodeVectors) As Integer

            'position vector of the center point
            Dim rP As Vector2 = (positionVectors.R1 + positionVectors.R2 + positionVectors.R3 + positionVectors.R4) * 0.25

            'add node
            factory.RequestNode(n, rP, False, False)

            Return n + 1

        End Function

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
        ''' Determines if a node lies in the corner of the farfield
        ''' </summary>
        ''' <param name="n"></param>
        ''' <param name="farfield"></param>
        ''' <returns></returns>
        Private Function IsCornerNode(n As Integer, farfield As Farfield) As Boolean

            Dim r As Vector2 = data.NodeV(n).R

            Return (r.X = 0 Or r.X = farfield.Width) And (r.Y = 0 Or r.Y = farfield.Height)

        End Function

#End Region

    End Class
End Namespace