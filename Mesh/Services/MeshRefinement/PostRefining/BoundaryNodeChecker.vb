Imports Core.Common
Imports Core.Interfaces

Namespace Services
    Public Class BoundaryNodeChecker : Implements IBoundaryNodeChecker

        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

        ''' <summary>
        ''' Fixes occasional blips where boundary nodes can be misclassified, leading to problems
        ''' with the smoothing cycle. this is a legacy from the early stages of development
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub CheckBoundaryNodes(farfield As Farfield) Implements IBoundaryNodeChecker.CheckBoundaryNodes

            data.CheckBoundaryNode(farfield)

        End Sub

    End Class
End Namespace