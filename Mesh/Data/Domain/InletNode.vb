Imports Mesh.Data

Namespace Models
    Public Class InletNode : Inherits BaseNode
        'when a node must be updated to show that it sits on the inlet edge. corner nodes are not excluded.

        'constructor
        Public Sub New(this_n As Integer)

            Dim this As Node

            this = Repository.Nodelist(this_n)

            If this.Boundary = True Then
                this.Inlet = True
            End If

        End Sub

    End Class
End Namespace
