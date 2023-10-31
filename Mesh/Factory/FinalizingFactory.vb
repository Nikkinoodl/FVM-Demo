Imports Core.Common
Imports Core.Domain
Imports Core.Interfaces

Namespace Factories
    Public Class FinalizingFactory : Implements IFinalizingFactory
#Region "Fields"

        Private ReadOnly data As IDataAccessService

#End Region
#Region "Constructor"
        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

#End Region

#Region "Finalizing Methods"

        ''' <summary>
        ''' Adds zero-height cells that have only one edge around the border of the farfield with triangular mesh. These are
        ''' used to set boundary conditions
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub AddBorderCells(farfield As Farfield) Implements IFinalizingFactory.AddBorderCells

            Dim boundaryCells As List(Of Cell) = data.BoundaryCell

            For Each t In boundaryCells

                For Each e As Edge In t.Edges

                    If e.SideType = SideType.boundary Then

                        Dim newId = data.MaxCellId + 1
                        Dim v1 As Integer
                        Dim v2 As Integer

                        Dim borderCellEdge As New Edge(SideName.S1, SideType.border) With {
                            .R = e.R,
                            .L = e.L,
                            .AdjoiningCell = data.CellList.IndexOf(t),
                            .AdjoiningEdge = e.SideName
                        }

                        If t.Edge4 Is Nothing Then      'triangular cell

                            If e.SideName = SideName.S1 Then

                                v1 = t.V2
                                v2 = t.V3

                            ElseIf e.SideName = SideName.S2 Then

                                v1 = t.V1
                                v2 = t.V3

                            Else

                                v1 = t.V1
                                v2 = t.V2

                            End If


                        Else                            'quad cell

                            If e.SideName = SideName.S1 Then

                                v1 = t.V2
                                v2 = t.V1

                            ElseIf e.SideName = SideName.S2 Then

                                v1 = t.V3
                                v2 = t.V2

                            ElseIf e.SideName = SideName.S3 Then

                                v1 = t.V4
                                v2 = t.V3

                            Else

                                v1 = t.V1
                                v2 = t.V4

                            End If

                        End If


                        'add new border cell
                        Dim bc As New BorderCell(newId, v1, v2, borderCellEdge)

                        'set the new border cell id as the adjoining cell on the existing boundary edge
                        e.AdjoiningCell = newId

                    End If

                Next

            Next

        End Sub

        ''' <summary>
        ''' Adds zero-height cells that have only one edge around the border of the farfield with square mesh type. These are
        ''' used to set boundary conditions (deprecated)
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub AddBorderCellsSquare(farfield As Farfield) Implements IFinalizingFactory.AddBorderCellsSquare

            'find cells with an edge on the boundary of the farfield
            Dim boundaryCells As List(Of Cell) = data.BoundaryCell

            For Each t In boundaryCells

                For Each e As Edge In t.Edges

                    'we only want a border cell on the boundary edge, so ignore all other types of edges
                    If e.SideType = SideType.boundary Then

                        Dim newId = data.MaxCellId + 1
                        Dim v1 As Integer
                        Dim v2 As Integer

                        'create new edge, copying as much data as we can from the existing one on the boundary
                        Dim edge As New Edge(SideName.S1, SideType.border) With {
                            .R = e.R,
                            .L = e.L,
                            .AdjoiningCell = data.CellList.IndexOf(t),
                            .AdjoiningEdge = e.SideName
                        }

                        If e.SideName = SideName.S1 Then

                            v1 = t.V2
                            v2 = t.V1

                        ElseIf e.SideName = SideName.S2 Then

                            v1 = t.V3
                            v2 = t.V2

                        ElseIf e.SideName = SideName.S3 Then

                            v1 = t.V4
                            v2 = t.V3

                        Else

                            v1 = t.V1
                            v2 = t.V4

                        End If

                        'add new border cell
                        Dim bc As New BorderCell(newId, v1, v2, edge)

                        'set the new border cell id as the adjoining cell on the existing boundary edge
                        e.AdjoiningCell = newId

                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' Wraps an airfoil surface with a layer of zero height cells that have only one edge
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub AddAirfoilBorderCells(farfield As Farfield) Implements IFinalizingFactory.AddAirfoilBorderCells

            'find cells with an edge on the airfoil surface
            Dim boundaryCells As List(Of Cell) = data.SurfaceCell

            For Each t In boundaryCells

                For Each e As Edge In t.Edges

                    'we only want a border cell on the airfoil edge, so ignore all other types of edges
                    If e.SideType = SideType.surface Then

                        Dim newId = data.MaxCellId + 1
                        Dim this_v2 As Integer
                        Dim this_v3 As Integer

                        Dim this_edge As New Edge(SideName.S1, SideType.border) With {
                            .R = e.R,
                            .L = e.L,
                            .AdjoiningCell = data.CellList.IndexOf(t),
                            .AdjoiningEdge = e.SideName
                        }

                        If e.SideName = SideName.S1 Then

                            this_v2 = t.V3
                            this_v3 = t.V2

                        ElseIf e.SideName = SideName.S2 Then

                            this_v2 = t.V3
                            this_v3 = t.V1

                        Else

                            this_v2 = t.V2
                            this_v3 = t.V1

                        End If

                        Dim bc As New BorderCell(newId, this_v2, this_v3, this_edge, BorderType.Airfoil)

                        'set the new border cell id as the adjoining cell on the existing boundary edge
                        e.AdjoiningCell = newId

                    End If
                Next
            Next


        End Sub
#End Region
    End Class
End Namespace