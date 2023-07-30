﻿Namespace Services
    Public Interface ICellCalculator

        ''' <summary>
        ''' Calculate the lengths and cell centers of triangular grid cells
        ''' </summary>
        Sub CalculateLengths()

        ''' <summary>
        ''' Calculate the lengths and cell centers of rectangular grid cells
        ''' </summary>
        Sub CalculateLengthsSquares()

        ''' <summary>
        ''' Calculate the mid points of triangular grid cell edges
        ''' </summary>
        Sub CalculateMidPoints()

        ''' <summary>
        ''' Calculate the mid points of rectangular grid cell edges
        ''' </summary>
        Sub CalculateMidPointsSquares()

        ''' <summary>
        ''' Calculate the vector from the cell center to the center of each cell edge
        ''' </summary>
        Sub CalculateFaceVectors()

        ''' <summary>
        ''' Calculate the outward facing normal for each triangular grid cell
        ''' </summary>
        Sub CalculateFaceNormals()

        ''' <summary>
        ''' Calculate the outward facing normal for each rectangular grid cell
        ''' </summary>
        Sub CalculateFaceNormalsSquares()

        ''' <summary>
        ''' Calculate the area of each triangular grid cell
        ''' </summary>
        Sub CalculateAreas()

        ''' <summary>
        ''' Calculate the area of each rectangular grid cell
        ''' </summary>
        Sub CalculateAreasSquares()

    End Interface
End Namespace