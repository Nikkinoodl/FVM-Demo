Imports Core.Common
Imports Mesh.Factories

Namespace Services
    Public Class BoundaryNodeChecker : Implements IBoundaryNodeChecker

        Private ReadOnly factory As IGridFactory
        Public Sub New(factory As IGridFactory)

            Me.factory = factory

        End Sub

        ''' <summary>
        ''' Ensure that all nodes on the edge of the farfield have the correct flag set
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub CheckBoundaryNodes(farfield As Farfield) Implements IBoundaryNodeChecker.CheckBoundaryNodes

            factory.CheckBoundaryNode(farfield)

        End Sub

    End Class
End Namespace