Imports Mesh.Services
Imports Core.Common
Imports Core.Common.MeshConstants

Namespace Logic
    Public Class Build

        Private ReadOnly reader As IAirfoilNodeReader
        Private ReadOnly checker As IBoundaryNodeChecker
        Private ReadOnly calculator As ICellCalculator
        Private ReadOnly sorter As ICellSorter
        Private ReadOnly scaler As IScaler
        Private ReadOnly grid As IGridBuilder
        Private ReadOnly initializer As IInitializer

        Public Sub New(reader As IAirfoilNodeReader, checker As IBoundaryNodeChecker, calculator As ICellCalculator, sorter As ICellSorter, scaler As IScaler, grid As IGridBuilder, initializer As IInitializer)

            Me.reader = reader
            Me.checker = checker
            Me.calculator = calculator
            Me.sorter = sorter
            Me.scaler = scaler
            Me.grid = grid
            Me.initializer = initializer

        End Sub

        Public Sub Logic(farfield As Farfield)

            'Call service to prep data 
            initializer.DataPreparer()

            'Read the airfoil nodes (otherwise we default to nodes at 0,0)
            Try
                reader.ReadAirfoilNodes(farfield.Filename)
            Catch
                MsgBox(MSGFILEERROR)
                Exit Sub
            End Try

            'Apply scaling and offset to airfoil nodes
            'This must be run before preliminary grid is set
            scaler.AirfoilScaler(farfield)

            'Build grid
            grid.SetPrelimGrid(farfield)

            'calculate lengths, make sure all nodes on boundary have .boundary = True
            checker.CheckBoundaryNodes(farfield)
            calculator.CalculateLengths()

        End Sub

    End Class
End Namespace