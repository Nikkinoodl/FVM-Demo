Imports Mesh.Models

Namespace Data
    Public Class Repository

        'Create collections available in all components of this application
        Public Shared Property Nodelist As New List(Of Node)
        Public Shared Property Trianglelist As New List(Of Triangle)

        'Clear lists
        Public Shared Sub ClearLists()
            Nodelist.Clear()
            Trianglelist.Clear()
        End Sub

    End Class
End Namespace