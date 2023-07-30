Imports Core.Interfaces
Imports Core.Domain

Namespace Services
    Public Class StatusSetter : Implements IStatusSetter

        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

        ''' <summary>
        ''' Resets the status of each mesh cell
        ''' </summary>
        Public Sub SetCompleteStatus() Implements IStatusSetter.SetCompleteStatus

            For Each cell In data.CellList

                cell.Complete = False

            Next
        End Sub

    End Class
End Namespace