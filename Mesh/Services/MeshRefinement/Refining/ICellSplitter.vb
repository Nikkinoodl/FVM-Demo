Namespace Services
    Public Interface ICellSplitter

        ''' <summary>
        ''' Refines the mesh by splitting cells
        ''' </summary>
        Sub SplitCells()

        ''' <summary>
        ''' Completes the mesh splitting process
        ''' </summary>
        Sub CleanOrphanNodes()

        ''' <summary>
        ''' Refines a regular grid which comprises square cells
        ''' </summary>
        Sub DivideRegularCells()

    End Interface
End Namespace