Imports Core.Common

Namespace Services
    Public Interface IBoundaryNodeChecker

        ''' <summary>
        ''' Ensure that all nodes on the edge of the farfield have the correct flag set
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub CheckBoundaryNodes(farfield As Farfield)

    End Interface
End Namespace