Imports Core.Interfaces
Imports Core.Common
Imports Mesh.Factories

Namespace Services
    Public Class NodeCleaner : Implements INodeCleaner

        Private numtriangles, vp, n As Integer
        Private n1, n2, n3 As Integer
        Private x1, x2, x3, y1, y2, y3, xp, yp As Double
        Private s1, s2, s3, b1, b2, b3 As Boolean

        Private ReadOnly data As IDataAccessService
        Private ReadOnly factory As IGridFactory

        Public Sub New(data As IDataAccessService, factory As IGridFactory)

            Me.data = data
            Me.factory = factory

        End Sub

        Public Sub CleanOrpanNodes() Implements INodeCleaner.CleanOrphanNodes
            'finds triangles that have an orphan node in the middle of one side and splits them at the orphan node

            numtriangles = data.Trianglelist.Count
            n = data.Nodelist.Count

            'current max id in trianglelist and new id for new triangles
            Dim maxId = data.Trianglelist.Max(Function(p) p.Id)
            Dim newId = maxId + 1

            'cycle through 1 less than the count
            For t = 0 To numtriangles - 1

                'Get node ids for this triangle
                GetNodeId(t)

                'Get details for each node
                GetNodeDetails()

                Dim config As Integer

                'check for orphan nodes on Edge1
                xp = MidPoint(x2, x3)
                yp = MidPoint(y2, y3)

                If data.Exists(xp, yp) > 0 Then           'if there is an orphan node

                    config = 1

                    'point vp to the existing node
                    vp = data.FindNode(xp, yp)

                    'Split the existing triangle in two and return incremented newId
                    newId = DivideTriangles(t, config, newId)

                    'Continue For

                End If

                'check for orphan nodes on Edge2
                xp = MidPoint(x3, x1)
                yp = MidPoint(y3, y1)

                If data.Exists(xp, yp) > 0 Then           'if there is an orphan node

                    config = 2

                    'point vp to the existing node
                    vp = data.FindNode(xp, yp)

                    'Split the existing triangle in two and return incremented newId
                    newId = DivideTriangles(t, config, newId)

                    'Continue For

                End If


                ''check for orphan nodes on Edge3
                xp = MidPoint(x2, x1)
                yp = MidPoint(y2, y1)

                If data.Exists(xp, yp) > 0 Then           'if there is an orphan node

                    config = 3

                    'point vp to the existing node
                    vp = data.FindNode(xp, yp)

                    'Split the existing triangle in two and return incremented newId
                    newId = DivideTriangles(t, config, newId)

                    'Continue For

                End If
            Next
        End Sub

        Private Sub GetNodeDetails()
            'Get details for each node

            With data.NodeV(n1)
                x1 = .X
                y1 = .Y
                s1 = .Surface   'is it an airfoil node?
                b1 = .Boundary ' is it a boundary node?
            End With

            With data.NodeV(n2)
                x2 = .X
                y2 = .Y
                s2 = .Surface   'is it an airfoil node?
                b2 = .Boundary ' is it a boundary node?
            End With

            With data.NodeV(n3)
                x3 = .X
                y3 = .Y
                s3 = .Surface   'is it an airfoil node?
                b3 = .Boundary ' is it a boundary node?
            End With
        End Sub

        Private Sub GetNodeId(t As Integer)
            'Get the node ids of the triangle vertices

            n1 = data.Trianglelist(t).V1
            n2 = data.Trianglelist(t).V2
            n3 = data.Trianglelist(t).V3

        End Sub

        Private Shared Function MidPoint(a As Double, b As Double) As Double
            'Finds the mid point of two cordinates
            Return (a + b) / 2
        End Function

        Private Function DivideTriangles(t As Integer, config As Integer, newid As Integer) As Integer
            'Replace the existing triangle instance and create a new triangle instance

            Dim s0 As SideType = SideType.none
            Dim s1 As SideType = data.Trianglelist(t).Edge1.SideType
            Dim s2 As SideType = data.Trianglelist(t).Edge2.SideType
            Dim s3 As SideType = data.Trianglelist(t).Edge3.SideType

            Select Case config
                Case 1
                    'The new face must always be of SideType.none : other faces inherit their existing state.
                    factory.ReplaceTriangle(t, newid, n1, n2, vp, s1, s0, s3)
                    factory.AddTriangle(newid + 1, n1, vp, n3, s1, s2, s0)
                Case 2
                    factory.ReplaceTriangle(t, newid, vp, n2, n3, s1, s2, s0)
                    factory.AddTriangle(newid + 1, n1, n2, vp, s0, s2, s3)
                Case 3
                    factory.ReplaceTriangle(t, newid, n1, vp, n3, s0, s2, s3)
                    factory.AddTriangle(newid + 1, vp, n2, n3, s1, s0, s3)
                Case Else
                    Throw New Exception
            End Select

            Return newid + 2

        End Function

    End Class
End Namespace