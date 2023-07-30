Imports Core.Domain
Imports Core.Common

Namespace Factories
    Public Interface IFinalizingFactory

        ''' <summary>
        ''' Adds zero-height cells that have only one edge around the border of the farfield with triangular grid type
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub AddBorderCells(farfield As Farfield)

        ''' <summary>
        ''' Adds zero-height cells that have only one edge around the border of the farfield with square grid type
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub AddBorderCellsSquare(farfield As Farfield)

    End Interface
End Namespace
