Imports Mesh.Services

Namespace Logic
    Public Class Smooth

        Private ReadOnly calculator As ICellCalculator
        Private ReadOnly smoother As IGridSmoother
        Private ReadOnly setter As IStatusSetter

        Public Sub New(calculator As ICellCalculator, smoother As IGridSmoother, setter As IStatusSetter)

            Me.calculator = calculator
            Me.smoother = smoother
            Me.setter = setter

        End Sub

        Public Sub Logic(farfield As Object)

            'run cycles of laplace smoothing
            If farfield.Smoothingcycles > 0 Then
                For n As Integer = 1 To farfield.Smoothingcycles
                    smoother.SmoothGrid()
                Next
            End If

            'calculate lengths, sort cells and set each cell's status to complete
            calculator.CalculateLengths()
            setter.SetCompleteStatus()


        End Sub

    End Class
End Namespace