Imports Mesh.Data

Namespace Models
    Public Class Triangle : Inherits BaseTriangle

        Public Sub New()

        End Sub

#Region "fields"
        Private _area As Double
#End Region

#Region "public properties"

        Public Overloads ReadOnly Property Area As Double 'area of triangle available for ranking
            Get
                'calculated using Hero's formula
                Dim value As Double
                Dim p As Double = (L1 + L2 + L3) * 0.5
                value = Math.Sqrt(p * (p - L1) * (p - L2) * (p - L3))
                _area = value
                Return _area
            End Get
        End Property
#End Region

#Region "constructor overloads"

        Public Sub New(this_id As Integer, this_v1 As Integer, this_v2 As Integer, this_v3 As Integer)
            Id = this_id
            V1 = this_v1
            V2 = this_v2
            V3 = this_v3
            Complete = False
            S1 = Nothing
            S2 = Nothing
            S3 = Nothing
            Repository.Trianglelist.Add(Me)
        End Sub

        Public Sub New(this_id As Integer, this_v1 As Integer, this_v2 As Integer, this_v3 As Integer, this_s1 As String, this_s2 As String, this_s3 As String)
            Id = this_id
            V1 = this_v1
            V2 = this_v2
            V3 = this_v3
            Complete = False
            S1 = this_s1
            S2 = this_s2
            S3 = this_s3
            Repository.Trianglelist.Add(Me)
        End Sub
#End Region

    End Class
End Namespace
