Imports Core.Common
Imports Mesh.Services

Namespace Logic
    Public Class EmptySpace

        Private ReadOnly builder As IEmptySpaceBuilder
        Private ReadOnly checker As IBoundaryNodeChecker
        Private ReadOnly calculator As ICellCalculator
        Private ReadOnly initializer As IInitializer

        Public Sub New(builder As IEmptySpaceBuilder, checker As IBoundaryNodeChecker, calculator As ICellCalculator, initializer As IInitializer)

            Me.builder = builder
            Me.checker = checker
            Me.calculator = calculator
            Me.initializer = initializer

        End Sub

        Public Sub Logic(farfield As Farfield)

            'call service to prep data 
            initializer.DataPreparer()

            'build grid
            builder.BuildEmptySpace(farfield)

            'calculate lengths, make sure all nodes on boundary have .boundary = True
            checker.CheckBoundaryNodes(farfield)

            If farfield.Gridtype <> GridType.Rectangles Then
                calculator.CalculateLengths()
            Else
                calculator.CalculateLengthsSquares()
            End If

        End Sub

    End Class
End Namespace