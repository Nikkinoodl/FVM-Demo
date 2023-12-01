Imports Core.Domain

Namespace Services
    Public Interface ICellCalculator

        ''' <summary>
        ''' Calculate the lengths and cell centers of grid cells
        ''' </summary>
        Sub CalculateLengths()

        ''' <summary>
        ''' Calculates the length and cell center of a single cell
        ''' </summary>
        ''' <param name="t"></param>
        Sub CalculateLength(t As Cell)

        ''' <summary>
        ''' Calculates the mid points of all cells
        ''' </summary>
        Sub CalculateMidPoints()

        ''' <summary>
        ''' Calculates the mid point of each edge of a single cell
        ''' </summary>
        Sub CalculateMidPoint(t As Cell)

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