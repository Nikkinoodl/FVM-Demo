Imports Core.Interfaces

Namespace Services
    Public Class CellSorter : Implements ICellSorter

        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

        ''' <summary>
        ''' Determines the order in which cells will be processed during mesh refinement, optimization
        ''' and CFD
        ''' </summary>
        Public Sub SortCells() Implements ICellSorter.SortCells

            data.SortCells()

        End Sub

    End Class
End Namespace