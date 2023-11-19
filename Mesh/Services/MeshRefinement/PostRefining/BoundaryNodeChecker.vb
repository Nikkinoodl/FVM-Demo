Imports Core.Common
Imports Core.Interfaces

Namespace Services
    Public Class BoundaryNodeChecker : Implements IBoundaryNodeChecker

        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

        ''' <summary>
        ''' Ensure that all nodes on the edge of the farfield have the correct flag set
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub CheckBoundaryNodes(farfield As Farfield) Implements IBoundaryNodeChecker.CheckBoundaryNodes

            data.CheckBoundaryNode(farfield)

        End Sub

    End Class
End Namespace