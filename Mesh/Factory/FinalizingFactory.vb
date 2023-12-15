Imports Core.Common
Imports Core.Data
Imports Core.Domain
Imports Core.Interfaces

Namespace Factories

    ''' <summary>
    ''' This factory creates the additional mesh components needed for CFD solutions
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
        ''' Adds zero-height cells that have only one edge around the border of the farfield. These are
        ''' used to set boundary conditions at the farfield edge
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub AddBorderCells(farfield As Farfield) Implements IFinalizingFactory.AddBorderCells

            'start with selection of cells that have an edge with SideType.Boundary
            Dim boundaryCells As List(Of Cell) = data.BoundaryCell
            Dim newId = data.MaxCellId + 1

            For Each boundaryCell In boundaryCells

                For Each e As Edge In boundaryCell.Edges

                    'exclude any edge that isn't on the boundary
                    If e.SideType <> SideType.boundary Then Continue For

                    Dim v1 As Integer
                    Dim v2 As Integer

                    'new border cell that will adjoin the boundary cell
                    Dim borderCellEdge As New Edge(SideName.S1, SideType.border) With
                    {
                        .R = e.R,
                        .L = e.L
                    }

                    'create a dictionary to assign v1 and v2 nodes based on matching cell type and side name
                    Dim nodeAssignment = Dictionaries.CreateNodeAssignment(boundaryCell)
                    Dim key As Tuple(Of CellType, SideName) = Tuple.Create(boundaryCell.CellType, e.SideName)
                    Dim value As Tuple(Of Integer?, Integer?) = Nothing

                    'check if the key exists in the dictionary
                    If nodeAssignment.TryGetValue(key, value) = False Then

                        Throw New Exception("CellType/SideName combination does not exist in dictionary")

                    End If

                    'set v1 and v2 using the values returned from the dictionary
                    v1 = value.Item1
                    v2 = value.Item2

                    'add new border cell
                    Dim bc As New BorderCell(newId, v1, v2, borderCellEdge)

                    newId += 1

                Next

            Next

        End Sub

        ''' <summary>
        ''' Wraps an airfoil surface with a layer of border cells. This method is restricted to use on
        ''' irregular triangle grids only and requires airfoil building methods to be added (they are not
        ''' incuded in this version)
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub AddAirfoilBorderCells(farfield As Farfield) Implements IFinalizingFactory.AddAirfoilBorderCells

            'find cells with an edge on the airfoil surface
            Dim surfaceCells As List(Of Cell) = data.SurfaceCell

            Dim newId = data.MaxCellId + 1

            For Each surfaceCell In surfaceCells

                For Each e As Edge In surfaceCell.Edges

                    'we only want a border cell on the airfoil edge, so ignore all other types of edges
                    If e.SideType <> SideType.surface Then Continue For

                    Dim v2 As Integer
                    Dim v3 As Integer

                    Dim this_edge As New Edge(SideName.S1, SideType.border) With {
                        .R = e.R,
                        .L = e.L,
                        .AdjoiningCell = data.CellList.IndexOf(surfaceCell),
                        .AdjoiningEdge = e.SideName
                    }

                    'create a dictionary to assign v1 and v2 nodes based on matching cell type and side name
                    Dim nodeAssignment = Dictionaries.CreateNodeAssignment(surfaceCell)
                    Dim key As Tuple(Of CellType, SideName) = Tuple.Create(surfaceCell.CellType, e.SideName)
                    Dim value As Tuple(Of Integer?, Integer?) = Nothing

                    'check if the key exists in the dictionary
                    If nodeAssignment.TryGetValue(key, value) = False Then

                        Throw New Exception()

                    End If

                    Dim result As Tuple(Of Integer?, Integer?) = value

                    'set v1 and v2 using the values returned from the dictionary
                    v2 = result.Item1
                    v3 = result.Item2

                    Dim bc As New BorderCell(newId, v2, v3, this_edge, BorderType.Airfoil)

                    'set the new border cell id as the adjoining cell on the existing surface edge
                    e.AdjoiningCell = data.CellList.IndexOf(data.CellList.Find(Function(c) c.Id = newId))

                    newId += 1

                Next

            Next

        End Sub
#End Region

    End Class
End Namespace