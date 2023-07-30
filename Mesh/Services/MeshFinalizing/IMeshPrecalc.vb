Namespace Services
    Public Interface IMeshPrecalc

        ''' <summary>
        ''' Find the adjoining element/face for each triangular cell
        ''' </summary>
        Sub FindAdjoiningCells()

        ''' <summary>
        ''' Find the adjoinging element/face for each square cell
        ''' </summary>
        Sub FindAdjoiningCellsSquare()

    End Interface
End Namespace