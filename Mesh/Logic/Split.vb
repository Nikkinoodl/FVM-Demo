Imports Core.Common
Imports Mesh.Services

Namespace Logic
    Public Class Split

        Private ReadOnly checker As IBoundaryNodeChecker
        Private ReadOnly calculator As ICellCalculator
        Private ReadOnly sorter As ICellSorter
        Private ReadOnly splitter As ICellSplitter

        Public Sub New(checker As IBoundaryNodeChecker, calculator As ICellCalculator, sorter As ICellSorter, splitter As ICellSplitter)

            Me.checker = checker
            Me.calculator = calculator
            Me.sorter = sorter
            Me.splitter = splitter

        End Sub

        Public Sub Logic(farfield As Farfield)

            'Use different methods depending on the grid type

            If farfield.Gridtype = GridType.Triangles Then

                'simple cell refinement
                splitter.SplitCells()

                'calculate lengths, make sure all nodes on boundary have .boundary = True
                checker.CheckBoundaryNodes(farfield)
                calculator.CalculateLengths()

            ElseIf farfield.Gridtype = GridType.RegularTriangles Then

                'simple cell refinement
                splitter.SplitCells()

                'calculate lengths, make sure all nodes on boundary have .boundary = True
                checker.CheckBoundaryNodes(farfield)
                calculator.CalculateLengths()

            ElseIf farfield.Gridtype = GridType.Equilateral Then

                'divide up grid, preserving triangle shapes
                splitter.DivideEquilateral()

                'calculate lengths, make sure all nodes on boundary have .boundary = True
                checker.CheckBoundaryNodes(farfield)
                calculator.CalculateLengths()

            Else

                'divide up rectangular grid elements
                splitter.DivideRegularCells()

                'calculate lengths, make sure all nodes on boundary have .boundary = True
                checker.CheckBoundaryNodes(farfield)
                calculator.CalculateLengthsSquares()

            End If

        End Sub

    End Class
End Namespace