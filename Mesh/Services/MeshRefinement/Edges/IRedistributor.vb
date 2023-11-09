Imports Core.Common

Namespace Services

    Public Interface IRedistributor

        ''' <summary>
        ''' Reallocates nodes on the farfield boundary edges
        ''' </summary>
        ''' <param name="farfield"></param>
        Sub Redistribute(farfield As Farfield)

    End Interface
End Namespace