Imports Mesh.Data

Namespace Models
    Public Class OutletTriangle : Inherits Triangle
        'When the edge of a triangle t in the trianglelist must be updated to show that it's
        'an outlet edge. To avoid confusion, note that this_t is the index, not the Id

        Public Sub New(this_t As Integer)

            Dim this As Triangle

            this = Repository.Trianglelist(this_t)

            If this.S1 = "boundary" Then
                this.S1 = "outlet"
            ElseIf this.S2 = "boundary" Then
                this.S2 = "outlet"
            ElseIf this.S3 = "boundary" Then
                this.S3 = "outlet"
            End If

            this.Complete = True

        End Sub

    End Class
End Namespace
