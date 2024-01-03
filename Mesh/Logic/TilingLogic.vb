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

            If farfield.Tiling = Tiling.Kis Then

                'kis tiling logic is independent of grid type
                splitter.DivideKis(farfield)

            ElseIf farfield.Tiling = Tiling.Ortho Then

                'ortho tiling logic for quad grids reuses an existing method which
                If farfield.Gridtype = GridType.Quads Then

                    splitter.DivideRectangularCells()

                Else

                    splitter.DivideOrtho(farfield)

                End If

            ElseIf farfield.Tiling = Tiling.Meta Then

                'meta tiling logic is independent of grid type
                splitter.DivideKis(farfield)

                checker.CheckBoundaryNodes(farfield)
                calculator.CalculateLengths()

                splitter.SplitCells(True)

            ElseIf farfield.Tiling = Tiling.Trunc Then

                'the initial step of trunc tiling logic is independent of grid type, but is not
                'available for irregular grids as we can't control the size of resulting polygons
                If farfield.Gridtype = GridType.Triangles Then

                    MsgBox("Unable to perform this tiling on irregular triangle grids")

                    Exit Sub

                End If

                splitter.DivideTrunc(farfield)
                checker.CheckBoundaryNodes(farfield)

                'the offcuts from congruent cells are combined using different methods depending
                'on the initial cell type
                If farfield.Gridtype = GridType.Quads Then

                    splitter.CombineQuadGrid(farfield)

                Else

                    splitter.CombineTriangleGrid(farfield)

                End If

            End If

            'calculate lengths, make sure all nodes on boundary have .boundary = True
            checker.CheckBoundaryNodes(farfield)
            calculator.CalculateLengths()

        End Sub

    End Class
End Namespace