Imports Core.Common
Imports Mesh.Services

Namespace Logic
    Public Class TilingLogic

        Private ReadOnly checker As IBoundaryNodeChecker
        Private ReadOnly calculator As ICellCalculator
        Private ReadOnly splitter As ICellSplitter

        Public Sub New(checker As IBoundaryNodeChecker, calculator As ICellCalculator, splitter As ICellSplitter)

            Me.checker = checker
            Me.calculator = calculator
            Me.splitter = splitter

        End Sub

        Public Sub Logic(farfield As Farfield)

            calculator.CalculateMidPoints()

            'tiling logic is independent of the grid type
            If farfield.Tiling = Tiling.Kis Then

                splitter.DivideKis(farfield)

            ElseIf farfield.Tiling = Tiling.Join Then

                If farfield.Gridtype = GridType.Quads Then

                    splitter.DivideRectangularCells()

                Else

                    splitter.DivideJoin(farfield)

                End If

            ElseIf farfield.Tiling = Tiling.KisAndJoin Then

                splitter.DivideKis(farfield)

                checker.CheckBoundaryNodes(farfield)
                calculator.CalculateLengths()

                splitter.SplitCells(True)

            ElseIf farfield.Tiling = Tiling.Trunc Then

                splitter.DivideTrunc(farfield)

            End If

            'calculate lengths, make sure all nodes on boundary have .boundary = True
            checker.CheckBoundaryNodes(farfield)
            calculator.CalculateLengths()

        End Sub

    End Class
End Namespace