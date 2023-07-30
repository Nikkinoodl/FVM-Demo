Imports Mesh.Data

Namespace Models
    Public Class ReplacementTriangle : Inherits Triangle
        'when triangle t in the trianglelist must be replaced

        Private ReadOnly _t As Integer

        Public Sub New()

        End Sub

        Public Sub New(this_t As Integer, this_id As Integer, this_v1 As Integer, this_v2 As Integer, this_v3 As Integer, this_s1 As String, this_s2 As String, this_s3 As String)

            't is the index of the triangle in trianglelist
            _t = this_t
            Id = this_id
            V1 = this_v1
            V2 = this_v2
            V3 = this_v3
            S1 = this_s1
            S2 = this_s2
            S3 = this_s3

            Repository.Trianglelist.Remove(Repository.Trianglelist(_t))
            Repository.Trianglelist.Insert(_t, Me)

        End Sub

    End Class
End Namespace