Imports Core.Common

Namespace Services
    Public Interface IBorderCellBuilder

        Sub CreateBorderCells(farfield As Farfield)

        Sub CheckBoundaryEdges(farfield As Farfield)

    End Interface
End Namespace