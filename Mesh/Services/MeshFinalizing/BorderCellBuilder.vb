﻿Imports Core.Interfaces
Imports Core.Domain
Imports Mesh.Factories
Imports Core.Common

Namespace Services
    Public Class BorderCellBuilder : Implements IBorderCellBuilder

        Private ReadOnly data As IDataAccessService
        Private ReadOnly factory As IFinalizingFactory

        Public Sub New(data As IDataAccessService, factory As IFinalizingFactory)

            Me.data = data
            Me.factory = factory

        End Sub

        Public Sub CreateBorderCells(farfield As Farfield) Implements IBorderCellBuilder.CreateBorderCells

#If DEBUG Then
            '(Debugging) check for incorrectly assigned boundary edges
            CheckBoundaryEdges(farfield)
#End If
            'Call factory to create a set of zero-height border cells around the farfield boundary
            'these are used to set boundary conditions and simplify cfd calcs
            factory.AddBorderCells(farfield)

        End Sub

        Public Sub CreateBorderCellsSquare(farfield As Farfield) Implements IBorderCellBuilder.CreateBorderCellsSquare

#If DEBUG Then
            '(Debugging) check for incorrectly assigned boundary edges
            CheckBoundaryEdges(farfield)
#End If

            'Call factory to create a set of zero-height border cells around the farfield boundary
            'these are used to set boundary conditions and simplify cfd calcs
            factory.AddBorderCellsSquare(farfield)

        End Sub

        Private Sub CheckBoundaryEdges(farfield As Farfield) Implements IBorderCellBuilder.CheckBoundaryEdges
            Dim celllist As List(Of Cell) = data.CellList

            For Each t As Cell In celllist
                If IsNothing(t) = False Then

                    Dim edges As New List(Of Edge) From {
                    t.Edge1,
                    t.Edge2,
                    t.Edge3
                }

                    'add fourth side for square grid types

                    If IsNothing(t.Edge4) = False Then
                        edges.Add(t.Edge4)
                    End If

                    For Each s As Edge In edges

                        If (s.R.Y = 0 And s.SideType <> SideType.boundary) Then
                            Debug.WriteLine("Invalid edge type on farfield bottom boundary")
                            s.SideType = SideType.boundary
                        End If

                        If (s.R.Y = farfield.Height And s.SideType <> SideType.boundary) Then
                            Debug.WriteLine("Invalid edge type on farfield top boundary")
                            s.SideType = SideType.boundary
                        End If

                        If (s.R.Y <> farfield.Height And s.R.Y <> 0 And s.R.X <> 0 And s.R.X <> farfield.Width And s.SideType = SideType.boundary) Then
                            Debug.WriteLine("Invalid boundary edge in farfield interior cell")
                            s.SideType = SideType.none
                        End If

                    Next
                End If
            Next
        End Sub

    End Class
End Namespace