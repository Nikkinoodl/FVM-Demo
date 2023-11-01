Imports Core.Common
Imports Mesh.Factories

Namespace Services
    Public Class EmptySpaceBuilder : Implements IEmptySpaceBuilder

        Private ReadOnly factory As IGridFactory
        Public Sub New(factory As IGridFactory)

            Me.factory = factory

        End Sub

        ''' <summary>
        ''' Creates a coarse mesh in an empty space that does not contain an airfoil
        ''' </summary>
        ''' <param name="farfield"></param>
        Public Sub BuildEmptySpace(farfield As Farfield) Implements IEmptySpaceBuilder.BuildEmptySpace

            If farfield.Gridtype = GridType.Triangles Then

                With factory

                    .AddCornerNodes(farfield)
                    .AddMidBoundaryNodes(farfield)
                    .SetupIrregularTriangleGrid()

                End With

            ElseIf farfield.Gridtype = GridType.Equilateral Then

                With factory

                    .AddCornerNodes(farfield)
                    .AddTopBoundaryNode(farfield)
                    .SetupEquilateralTriangleGrid()

                End With

            Else                         'rectangular grid as default

                With factory

                    .AddCornerNodes(farfield)
                    .SetupRectangularGrid()

                End With

            End If
        End Sub

    End Class
End Namespace