Imports Core.Domain
Imports Core.Common

Namespace Factories

    ''' <summary>
    ''' This factory adds cells around the edge of the fairfield that allow the setting of boundary conditions
    ''' </summary>
    Public Interface IFinalizingFactory

        ''' <summary>
        ''' Adds zero-height cells that have only one edge around the border of the farfield
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub AddBorderCells(farfield As Farfield)

        ''' <summary>
        ''' Wraps an airfoil surface with a layer of zero height cells that have only one edge
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub AddAirfoilBorderCells(farfield As Farfield)

    End Interface
End Namespace
