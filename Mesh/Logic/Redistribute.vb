Imports Mesh.Services

Namespace Logic
    Public Class Redistribute

        Private ReadOnly calculator As ICellCalculator
        Private ReadOnly redistributor As IRedistributor

        Public Sub New(calculator As ICellCalculator, redistributor As IRedistributor)

            Me.calculator = calculator
            Me.redistributor = redistributor

        End Sub

        Public Sub Logic(farfield As Object)

            'Redistribute boundary nodes
            redistributor.Redistribute(farfield)

            'Recalculate cell sides
            calculator.CalculateLengths()

        End Sub

    End Class
End Namespace