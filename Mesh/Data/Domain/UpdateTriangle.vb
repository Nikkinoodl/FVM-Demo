Imports Mesh.Data

Namespace Models
    Public Class UpdateTriangle : Inherits Triangle
        'When triangle t in the trianglelist must be updated
        'To avoid confusion, note that t is the index, not the Id
        Public Sub New()

        End Sub
        Public Sub New(t As Integer, this_v1 As Integer, this_v2 As Integer, this_v3 As Integer, this_s1 As String, this_s2 As String, this_s3 As String)

            Dim this As Triangle = Repository.Trianglelist(t)

            this.V1 = this_v1
            this.V2 = this_v2
            this.V3 = this_v3
            this.Complete = True
            this.S1 = this_s1
            this.S2 = this_s2
            this.S3 = this_s3

        End Sub

    End Class
End Namespace
