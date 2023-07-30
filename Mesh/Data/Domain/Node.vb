Imports Mesh.Data

Namespace Models
    Public Class Node : Inherits BaseNode

#Region "constructor"
        Public Sub New()
            Repository.Nodelist.Add(Me)
        End Sub
#End Region

#Region "constructor overloads"
        Public Sub New(this_id As Integer, this_x As Double, this_y As Double)
            Id = this_id
            X = this_x
            Y = this_y
            Surface = False
            Boundary = False
            Inlet = False
            Outlet = False
            Repository.Nodelist.Add(Me)
        End Sub

        Public Sub New(this_id As Integer, this_x As Double, this_y As Double, this_surface As Boolean, this_boundary As Boolean)
            Id = this_id
            X = this_x
            Y = this_y
            Surface = this_surface
            Boundary = this_boundary
            Inlet = False
            Outlet = False
            Repository.Nodelist.Add(Me)
        End Sub
#End Region

    End Class
End Namespace