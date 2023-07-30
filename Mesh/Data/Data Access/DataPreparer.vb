Namespace Data
    Public Class DataPreparer
        Implements IDataPreparer

        Sub PrepareRepository() Implements IDataPreparer.PrepareRepository

            'clears out any existing data in lists
            Repository.ClearLists()

        End Sub

    End Class
End Namespace