Imports Core.Common
Imports Mesh.Services

Namespace Logic
    Public Class Finalize

        Private ReadOnly cellBuilder As IBorderCellBuilder
        Private ReadOnly setter As IStatusSetter
        Private ReadOnly calculator As ICellCalculator
        Private ReadOnly meshprecalc As IMeshPrecalc

        Public Sub New(cellBuilder As IBorderCellBuilder, setter As IStatusSetter, calculator As ICellCalculator,
                       meshprecalc As IMeshPrecalc)

            Me.cellBuilder = cellBuilder
            Me.setter = setter
            Me.calculator = calculator
            Me.meshprecalc = meshprecalc

        End Sub

        Public Sub Logic(farfield As Farfield)

            If farfield.Gridtype = GridType.Triangles Then

                'Some changes to cells prior to starting CFD
                'Find edge midpoints
                calculator.CalculateMidPoints()

                'Remove any 'complete' flags on the cells
                setter.SetCompleteStatus()

                'Calculate Areas
                calculator.CalculateAreas()

                'Calculate face vectors and face normals
                calculator.CalculateFaceVectors()
                calculator.CalculateFaceNormals()

                'Add zero-height border cells for setting boundary conditions
                cellBuilder.CreateBorderCells(farfield)

                'Find adjoining cell and face for each edge
                meshprecalc.FindAdjoiningCells()

            Else

                'Some changes to cells prior to starting CFD
                'Find edge midpoints
                calculator.CalculateMidPointsSquares()

                'Remove any 'complete' flags on the cells
                setter.SetCompleteStatus()

                'Calculate Areas
                calculator.CalculateAreasSquares()

                'Calculate face vectors and face normals
                calculator.CalculateFaceVectors()
                calculator.CalculateFaceNormalsSquares()

                'Add zero-height border cells for setting boundary conditions
                cellBuilder.CreateBorderCellsSquare(farfield)

                'Find adjoining cell and face for each edge
                meshprecalc.FindAdjoiningCellsSquare()

            End If

        End Sub

    End Class
End Namespace