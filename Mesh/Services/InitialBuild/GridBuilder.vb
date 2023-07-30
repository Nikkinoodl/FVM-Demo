Imports Core.Interfaces
Imports Core.Common
Imports Mesh.Factories
Imports System.Numerics

Namespace Services
    Public Class GridBuilder : Implements IGridBuilder

        Private t As Integer
        Private ReadOnly nextn As Integer
        Private ReadOnly thisv As Integer
        Private ReadOnly lastv As Integer

        Private ReadOnly factory As IGridFactory
        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService, factory As IGridFactory)

            Me.data = data
            Me.factory = factory

        End Sub

        Public Sub SetPrelimGrid(farfield As Object) Implements IGridBuilder.SetPrelimGrid
            'takes the first pass at building the mesh

            'index for cells will start at zero
            t = 0

            'numnodes is the number of nodes that define the airfoil
            'and will be used throughout the grid build
            'each subsequent layer will have the same number of nodes
            'NOTE: this is assumed to be an even number when boundary nodes are assigned
            Dim numnodes As Integer = data.Nodelist.Count()

            'validation of inputs which can only be done when numnodes is available
            farfield.ValidateNodeTrade(numnodes)
            farfield.ValidateOffset(numnodes)

            'start at the airfoil surface and work out towards
            'the farfield in successive layers, creating nodes and cells as we go
            CalcLayers(farfield, numnodes)

            'Calculate and establish farfield boundary nodes
            CalcBoundaryNodes(farfield, numnodes)

            'Link the final node layer to the farfield boundary
            ConnectToBoundary(farfield, numnodes)

        End Sub

#Region "Private_Methods"
        Private Sub CalcLayers(farfield As Object, numnodes As Integer)
            'Sets the initial grid by creating layers of cells starting at the airfoil.

            'Index of the current node is returned by a lambda function
            Dim thisv = Function(s) numnodes + s

            'In the first pass, we start with the existing nodes that define the airfoil
            'surface and use them to create a set of new nodes. We use both sets of nodes to
            'build a mesh of cells. If more layers are needed, we use the new nodes to create another
            'layer of nodes and cells and so on.
            For j = 1 To farfield.layers

                'get the height of cells in this layer
                Dim h As Single = farfield.CellHeight(j)

                'the index of the first and last nodes in this layer are (zero based index)
                Dim firstnode As Integer = data.Nodelist.Count - numnodes
                Dim lastnode As Integer = data.Nodelist.Count - 1

                'To calculate the index of the next node to be added we use a simple counter called with a lambda function.
                'When we create a layer of cells, the last cell in the layer has to link up with the first
                'one we create, so we always skip back to first node in the layer to close the grid.
                Dim nextn = Function(s) As Integer
                                If s = lastnode Then
                                    Return firstnode
                                Else
                                    Return s + 1
                                End If
                            End Function

                'Index of previously created node is returned by a lambda function. Again, the nodes of the last cell in a layer
                'have to be made to join up with the first node created.
                Dim lastv = Function(s)
                                'if thisv is the first node in the layer then lastv is the lastnode in the layer
                                If thisv(s) = lastnode + 1 Then
                                    Return lastnode + numnodes
                                Else
                                    Return thisv(s) - 1
                                End If
                            End Function

                'loop through the existing nodes that form the base of the layer
                'each layer will have the same number of new nodes added
                For n = firstnode To lastnode

                    'calculate position of and set up new node
                    CalcNewNode(thisv(n), nextn(n), n, h)

                    'calculate a cell formed by base nodes and new node
                    If j = 1 Then
                        'the first layer of cells has a surface edge
                        factory.AddCell(t, n, thisv(n), nextn(n), SideType.none, SideType.surface, SideType.none)
                    Else
                        'subsequent layers
                        factory.AddCell(t, n, thisv(n), nextn(n))
                    End If

                    t += 1

                    'create the interlocking cell
                    factory.AddCell(t, n, lastv(n), thisv(n))
                    t += 1

                Next
            Next
        End Sub

        Private Sub CalcBoundaryNodes(farfield As Object, numnodes As Integer)
            'calculates and sets up nodes on the farfield boundary when airfoil is present

            'establish number of nodes to be assigned to the farfield boundary sides
            Dim nw As Integer = farfield.NodesOnBoundary("width", numnodes)
            Dim nh As Integer = farfield.NodesOnBoundary("height", numnodes)

            'counter for new nodes on farfield boundary.
            Dim i As Integer = 1

            'the existing first and last nodes in this layer are (zero based index)
            Dim firstnode = data.Nodelist.Count - numnodes
            Dim lastnode = data.Nodelist.Count - 1

            Dim thisx, thisy As Single

            For n = firstnode To lastnode

                'i is used to point to nodes in the layer.
                'Dim i = Function(s) s - firstnode + 1

                'calculate new node which forms cell with the two base nodes
                'start at the bottom left
                If i = 1 Then   'bottom left

                    thisx = 0
                    thisy = 0

                ElseIf i > 1 And i < nh + 2 Then 'left side

                    Dim nodeFraction = (i - 1) / (nh + 1)
                    Dim lengthFraction = NodeDistribution(nodeFraction)

                    thisx = 0
                    thisy = lengthFraction * farfield.height

                ElseIf i = nh + 2 Then  'top left corner

                    thisx = 0
                    thisy = farfield.height

                ElseIf i > nh + 2 And i < nh + nw + 3 Then ' top side

                    Dim nodeFraction = (i - (nh + 2)) / (nw + 1)
                    Dim lengthFraction = NodeDistribution(nodeFraction)

                    thisx = lengthFraction * farfield.width
                    thisy = farfield.height

                ElseIf i = nh + nw + 3 Then ' top right corner

                    thisx = farfield.width
                    thisy = farfield.height

                ElseIf i > nh + nw + 3 And i < nw + 2 * nh + 4 Then ' right side

                    Dim nodeFraction = (i - (nh + nw + 3)) / (nh + 1)
                    Dim lengthFraction = NodeDistributionReverse(nodeFraction)

                    thisx = farfield.width
                    thisy = lengthFraction * farfield.height

                ElseIf i = nw + 2 * nh + 4 Then  'bottom right corner

                    thisx = farfield.width
                    thisy = 0

                ElseIf i > nw + 2 * nh + 4 And i < 2 * nw + 2 * nh + 5 Then 'bottom side

                    Dim nodeFraction = (i - (nw + 2 * nh + 4)) / (nw + 1)
                    Dim lengthFraction = NodeDistributionReverse(nodeFraction)

                    thisx = lengthFraction * farfield.width
                    thisy = 0

                Else

                    Throw New Exception

                End If

                factory.AddBoundaryNode(numnodes + n, thisx, thisy)

                i += 1

            Next
        End Sub

        Private Sub ConnectToBoundary(farfield As Object, numnodes As Integer)
            'Connect the uppermost node layer to the newly created farfield boundary nodes

            Dim totalNodes As Integer = data.Nodelist.Count

            'points to the id of the first node in the outer layer
            Dim firstnode As Integer = totalNodes - 2 * numnodes

            'points to the id of the last node in the outer layer
            Dim lastnode As Integer = firstnode + numnodes - 1

            'nextn is unaffected by offset
            Dim nextn = Function(s)
                            'when we've cycled through the layer, next n must skip back to first n in the layer
                            If s = lastnode Then
                                Return firstnode
                            Else
                                Return s + 1
                            End If
                        End Function

            'points to the boundary node that is used as the vertex of a cell
            Dim thisv = Function(s)
                            'accounting for the offset
                            If s > lastnode - farfield.offset Then
                                Return s + farfield.offset
                            Else
                                Return s + numnodes + farfield.offset
                            End If
                        End Function

            'points to the other vertex of the base of the interlocking cell
            Dim lastv = Function(s)
                            'if thisv is the first node in the boundary then lastv is the lastnode in the boundary
                            If thisv(s) = lastnode + 1 Then
                                Return lastnode + numnodes
                            Else
                                Return thisv(s) - 1
                            End If
                        End Function
            For n = firstnode To lastnode

                'create a new cell formed by base nodes and new node
                factory.AddCell(t, n, thisv(n), nextn(n))
                t += 1

                'create the interlocking cell on the boundary of the farfield
                factory.AddCell(t, n, lastv(n), thisv(n), SideType.boundary, SideType.none, SideType.none)
                t += 1
            Next
        End Sub
#End Region

#Region "Utilities"
        Private Sub CalcNewNode(thisv As Integer, nextn As Integer, n As Integer, h As Single)
            'Calculates the position of the nodes that create a new layer (all except surface nodes and boundary nodes)

            'base
            Dim base = Vector2.Subtract(data.Nodelist(nextn).R, data.Nodelist(n).R)

            'rotate 90 degrees clockwise, normalize and multiply by cell height
            Dim normal As New Vector2(base.Y, base.X * -1)
            normal = Vector2.Normalize(normal) * h

            'midpoint of base
            Dim mid = Vector2.Add(data.Nodelist(nextn).R, data.Nodelist(n).R) * 0.5

            'add normal to base midpoint to get new node position
            Dim newNodePosition = Vector2.Add(mid, normal)

            'add the new node
            factory.AddNode(thisv, newNodePosition)

        End Sub

        Private Function NodeDistribution(nodeFraction As Single) As Single
            'provides the base calculation for the distribution of nodes on the
            'left And top sides of the farfield boundary
            'the original cosine function has been superceded by a separate edge node distribution service
            '(Services/MeshRefinement/Edges/Redistributor)

            Dim result = nodeFraction
            Return result

        End Function

        Private Function NodeDistributionReverse(nodeFraction As Single) As Single
            'provides the base calculation for the distribution of nodes on the
            'left And top sides of the farfield boundary
            'the original cosine function has been superceded by a separate edge node distribution service
            '(Services/MeshRefinement/Edges/Redistributor)

            Dim result = 1 - nodeFraction
            Return result

        End Function

#End Region

    End Class
End Namespace