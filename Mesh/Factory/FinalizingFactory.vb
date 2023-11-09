Imports Core.Common
Imports Core.Domain
Imports Core.Interfaces
Imports Mesh.Services.SharedUtilities

Namespace Factories

    ''' <summary>
    ''' This factory adds cells around the edge of the fairfield that allow the setting of boundary conditions
    ''' </summary>
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
        ''' Adds zero-height cells that have only one edge around the border of the farfield
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub AddBorderCells(farfield As Farfield) Implements IFinalizingFactory.AddBorderCells

            Dim boundaryCells As List(Of Cell) = data.BoundaryCell
            Dim newId = data.MaxCellId + 1

            For Each t In boundaryCells

                For Each e As Edge In t.Edges

                    If e.SideType = SideType.boundary Then

                        Dim v1 As Integer
                        Dim v2 As Integer

                        Dim borderCellEdge As New Edge(SideName.S1, SideType.border) With {
                            .R = e.R,
                            .L = e.L,
                            .AdjoiningCell = data.CellList.IndexOf(t),
                            .AdjoiningEdge = e.SideName
                        }

                        If t.CellType = CellType.triangle Then      'triangular cell

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

                        ElseIf t.CellType = CellType.quad Then

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

                        ElseIf t.CellType = CellType.pent Then

                            If e.SideName = SideName.S1 Then

                                v1 = t.V2
                                v2 = t.V1

                            ElseIf e.SideName = SideName.S2 Then

                                v1 = t.V3
                                v2 = t.V2

                            ElseIf e.SideName = SideName.S3 Then

                                v1 = t.V4
                                v2 = t.V3

                            ElseIf e.SideName = SideName.S4 Then

                                v1 = t.V5
                                v2 = t.V4

                            Else

                                v1 = t.V1
                                v2 = t.V5

                            End If

                        ElseIf t.CellType = CellType.hex Then

                            If e.SideName = SideName.S1 Then

                                v1 = t.V2
                                v2 = t.V1

                            ElseIf e.SideName = SideName.S2 Then

                                v1 = t.V3
                                v2 = t.V2

                            ElseIf e.SideName = SideName.S3 Then

                                v1 = t.V4
                                v2 = t.V3

                            ElseIf e.SideName = SideName.S4 Then

                                v1 = t.V5
                                v2 = t.V4

                            ElseIf e.SideName = SideName.s5 Then

                                v1 = t.V6
                                v2 = t.V5

                            Else

                                v1 = t.V1
                                v2 = t.V6

                            End If

                        ElseIf t.CellType = CellType.oct Then

                            If e.SideName = SideName.S1 Then

                                v1 = t.V2
                                v2 = t.V1

                            ElseIf e.SideName = SideName.S2 Then

                                v1 = t.V3
                                v2 = t.V2

                            ElseIf e.SideName = SideName.S3 Then

                                v1 = t.V4
                                v2 = t.V3

                            ElseIf e.SideName = SideName.S4 Then

                                v1 = t.V5
                                v2 = t.V4

                            ElseIf e.SideName = SideName.s5 Then

                                v1 = t.V6
                                v2 = t.V5

                            ElseIf e.SideName = SideName.s6 Then

                                v1 = t.V7
                                v2 = t.V6

                            ElseIf e.SideName = SideName.s7 Then

                                v1 = t.V8
                                v2 = t.V7

                            Else

                                v1 = t.V1
                                v2 = t.V8

                            End If

                        Else

                            Throw New Exception()

                        End If


                        'add new border cell
                        Dim bc As New BorderCell(newId, v1, v2, borderCellEdge)

                        'set the new border cell id as the adjoining cell on the existing boundary edge
                        e.AdjoiningCell = newId

                        newId += 1

                    End If

                Next

            Next

        End Sub

        ''' <summary>
        ''' Wraps an airfoil surface with a layer of zero height cells that have only one edge. This method is
        ''' restricted to use on irregular triangule grids only.
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub AddAirfoilBorderCells(farfield As Farfield) Implements IFinalizingFactory.AddAirfoilBorderCells

            'find cells with an edge on the airfoil surface
            Dim boundaryCells As List(Of Cell) = data.SurfaceCell

            Dim newId = data.MaxCellId + 1

            For Each t In boundaryCells

                For Each e As Edge In t.Edges

                    'we only want a border cell on the airfoil edge, so ignore all other types of edges
                    If e.SideType = SideType.surface Then

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

                        newId += 1

                    End If
                Next
            Next


        End Sub
#End Region
    End Class
End Namespace