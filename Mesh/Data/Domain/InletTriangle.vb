Imports Mesh.Data

Namespace Models
    Public Class InletTriangle : Inherits Triangle
        'When the edge of a triangle t in the trianglelist must be updated to show that it's
        'an inlet edge. To avoid confusion, note that this_t is an index, not an Id

        Public Sub New(this_t As Integer)

            Dim this As Triangle

            this = Repository.Trianglelist(this_t)

            If this.S1 = "boundary" Then
                this.S1 = "inlet"
            ElseIf this.S2 = "boundary" Then
                this.S2 = "inlet"
            ElseIf this.S3 = "boundary" Then
                this.S3 = "inlet"
            End If

            this.Complete = True

        End Sub

    End Class
End Namespace
