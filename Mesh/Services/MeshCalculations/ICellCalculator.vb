Namespace Services
    Public Interface ICellCalculator

        ''' <summary>
        ''' Calculate the lengths and cell centers of triangular grid cells
        ''' </summary>
        Sub CalculateLengths()

        ''' <summary>
        ''' Calculate the mid points of triangular grid cell edges
        ''' </summary>
        Sub CalculateMidPoints()

        ''' <summary>
        ''' Calculate the vector from the cell center to the center of each cell edge
        ''' </summary>
        Sub CalculateFaceVectors()

        ''' <summary>
        ''' Calculate the outward facing normal for each triangular grid cell
        ''' </summary>
        Sub CalculateFaceNormals()

        ''' <summary>
        ''' Calculate the area of each triangular grid cell
        ''' </summary>
        Sub CalculateAreas()

    End Interface
End Namespace