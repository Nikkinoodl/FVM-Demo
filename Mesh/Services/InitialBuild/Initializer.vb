Imports Core.Interfaces

Namespace Services
    Public Class Initializer
        Implements IInitializer

        Private ReadOnly preparer As IDataPreparer

        Public Sub New(dataPreparer As IDataPreparer)
            preparer = dataPreparer
        End Sub

        ''' <summary>
        ''' Reset working storage
        ''' </summary>
        Public Sub DataPreparer() Implements IInitializer.DataPreparer

            preparer.PrepareRepository()

        End Sub

    End Class
End Namespace