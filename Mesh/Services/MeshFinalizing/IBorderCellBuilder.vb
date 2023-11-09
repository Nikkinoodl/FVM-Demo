Imports Core.Common

Namespace Services
    Public Interface IBorderCellBuilder

        ''' <summary>
        ''' Initiates the creation of a layer of cells at the farfield edge which are used for setting
        ''' boundary conditions
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub CreateBorderCells(farfield As Farfield)

    End Interface
End Namespace