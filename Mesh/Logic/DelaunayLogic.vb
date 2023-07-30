Imports Mesh.Services

Namespace Logic
    Public Class DelaunayLogic

        Private ReadOnly delaunay As IDelaunay
        Private ReadOnly setter As IStatusSetter
        Private ReadOnly sorter As ICellSorter
        Private ReadOnly calculator As ICellCalculator

        Public Sub New(delaunay As IDelaunay, setter As IStatusSetter, sorter As ICellSorter, calculator As ICellCalculator)

            Me.delaunay = delaunay
            Me.setter = setter
            Me.sorter = sorter
            Me.calculator = calculator

        End Sub

        Public Sub Logic()

            'do Delaunay triangulation
            delaunay.Delaunay()

            'calc lengths and reset complete status
            calculator.CalculateLengths()
            setter.SetCompleteStatus()

        End Sub

    End Class
End Namespace