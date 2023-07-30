Imports Mesh.Services

Namespace Logic
    Public Class ResetData

        Private ReadOnly initializer As IInitializer

        Public Sub New(initializer As IInitializer)

            Me.initializer = initializer

        End Sub

        Public Sub Logic()

            'Call service to prep data 
            initializer.DataPreparer()

        End Sub

    End Class
End Namespace