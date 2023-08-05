Namespace Services
    Public Interface ICellSplitter

        ''' <summary>
        ''' Refines the mesh by splitting triangular cells between the longest side and the opposing vertex
        ''' </summary>
        Sub SplitCells()

        ''' <summary>
        ''' Refines an equilateral triangle grid
        ''' </summary>
        Sub DivideEquilateral()

        ''' <summary>
        ''' Refines a rectangular grid
        ''' </summary>
        Sub DivideRegularCells()

    End Interface
End Namespace