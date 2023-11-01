Imports Core.Common
Imports Mesh.Services

Namespace Logic
    Public Class Split

        Private ReadOnly checker As IBoundaryNodeChecker
        Private ReadOnly calculator As ICellCalculator
        Private ReadOnly splitter As ICellSplitter

        Public Sub New(checker As IBoundaryNodeChecker, calculator As ICellCalculator, splitter As ICellSplitter)

            Me.checker = checker
            Me.calculator = calculator
            Me.splitter = splitter

        End Sub

        Public Sub Logic(farfield As Farfield)

            'Use different methods depending on the grid type
            If farfield.Gridtype = GridType.Triangles Then

                'simple cell refinement
                splitter.SplitCells()

            ElseIf farfield.Gridtype = GridType.Equilateral Then

                'divide up grid, preserving triangle shapes
                splitter.DivideEquilateral()

            Else

                'divide up rectangular grid elements
                splitter.DivideRectangularCells()

            End If

            'calculate lengths, make sure all nodes on boundary have .boundary = True
            checker.CheckBoundaryNodes(farfield)
            calculator.CalculateLengths()

        End Sub

    End Class
End Namespace