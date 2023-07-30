Imports Core.Common

Namespace Services
    Public Interface IBoundaryNodeChecker

        ''' <summary>
        ''' Fixes occasional blips where boundary nodes can be misclassified, leading to problems
        ''' with the smoothing cycle. this is a legacy from the early stages of development
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub CheckBoundaryNodes(farfield As Farfield)

    End Interface
End Namespace