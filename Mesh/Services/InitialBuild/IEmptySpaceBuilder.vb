Imports Core.Common

Namespace Services
    Public Interface IEmptySpaceBuilder

        ''' <summary>
        ''' Creates a coarse mesh in an empty space that does not contain an airfoil
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub BuildEmptySpace(farfield As Farfield)

    End Interface
End Namespace