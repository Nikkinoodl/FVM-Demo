Imports System.Xml.XPath
Imports Core.Common
Imports Core.Data
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

                        'create a dictionary to assign v1 and v2 nodes based on matching cell type and side name
                        Dim nodeAssignment = Dictionaries.CreateNodeAssignment(t)
                        Dim key As Tuple(Of CellType, SideName) = Tuple.Create(t.CellType, e.SideName)
                        Dim value As Tuple(Of Integer?, Integer?) = Nothing

                        'check if the key exists in the dictionary
                        If nodeAssignment.TryGetValue(key, value) Then

                            Dim result As Tuple(Of Integer?, Integer?) = value

                            'set v1 and v2 using the values returned from the dictionary
                            v1 = result.Item1
                            v2 = result.Item2

                            'add new border cell
                            Dim bc As New BorderCell(newId, v1, v2, borderCellEdge)

                            'set the new border cell id as the adjoining cell on the existing boundary edge
                            e.AdjoiningCell = newId

                            newId += 1

                        Else

                            Throw New Exception()

                        End If

                    End If

                Next

            Next

        End Sub

        ''' <summary>
        ''' Wraps an airfoil surface with a layer of zero height cells that have only one edge. This method is
        ''' restricted to use on irregular triangle grids only.
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

                        Dim v2 As Integer
                        Dim v3 As Integer

                        Dim this_edge As New Edge(SideName.S1, SideType.border) With {
                            .R = e.R,
                            .L = e.L,
                            .AdjoiningCell = data.CellList.IndexOf(t),
                            .AdjoiningEdge = e.SideName
                        }

                        'create a dictionary to assign v1 and v2 nodes based on matching cell type and side name
                        Dim nodeAssignment = Dictionaries.CreateNodeAssignment(t)
                        Dim key As Tuple(Of CellType, SideName) = Tuple.Create(t.CellType, e.SideName)
                        Dim value As Tuple(Of Integer?, Integer?) = Nothing

                        'check if the key exists in the dictionary
                        If nodeAssignment.TryGetValue(key, value) Then

                            Dim result As Tuple(Of Integer?, Integer?) = value

                            'set v1 and v2 using the values returned from the dictionary
                            v2 = result.Item1
                            v3 = result.Item2

                            Dim bc As New BorderCell(newId, v2, v3, this_edge, BorderType.Airfoil)

                            'set the new border cell id as the adjoining cell on the existing boundary edge
                            e.AdjoiningCell = newId

                            newId += 1

                        Else

                            Throw New Exception()

                        End If

                    End If

                Next

            Next

        End Sub
#End Region

#Region "Private Methods"


#End Region

    End Class
End Namespace