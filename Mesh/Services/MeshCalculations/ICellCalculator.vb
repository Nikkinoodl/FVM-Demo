Namespace Services
    Public Interface ICellCalculator

        ''' <summary>
        ''' Calculate the lengths and cell centers of triangular grid cells
        ''' </summary>
        Sub CalculateLengths()

        ''' <summary>
        ''' Calculate the mid points
        ''' </summary>
        Sub CalculateMidPoints()

        ''' <summary>
        ''' Calculate the vector from the cell center to the center of each cell edge
        ''' </summary>
        Sub CalculateFaceVectors()

        ''' <summary>
        ''' Calculate the outward facing normals
        ''' </summary>
        Sub CalculateFaceNormals()

        ''' <summary>
        ''' Calculates the inverse area of each cell
        ''' </summary>
        Sub CalculateAreas()

    End Interface
End Namespace