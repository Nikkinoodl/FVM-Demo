Namespace AppSettings
    Public Class MeshConstants
        'Hard coded constants used throughout the application are defined here

        Public Const DIALOGTITLE As String = "Select a File"
        Public Const FILELOCATION As String = "C:\Users\Simon\Projects\Apps"
        Public Const MSGCOMPLETE As String = "Complete"
        Public Const MSGINITIALIZE As String = "Initializing"
        Public Const MSGDELAUNAY As String = "Starting Delaunay Trianguation"
        Public Const MSGDIVIDE As String = "Refining Grid"
        Public Const MSGLOADED As String = "Data Loaded From File"
        Public Const MSGCONSTRUCT As String = "Building Grid"
        Public Const MSGREDISTRIBUTE As String = "Redistributing Boundary Edge Nodes"
        Public Const MSGSMOOTH As String = "Performing Laplace Smoothing"
        Public Const MSGNUMNODES As String = "The value of nodetrade exceeds the number of nodes on the boundary side. Setting to maximum allowed value."
        Public Const MSGOFFSET As String = "The value of offset exceeds the number of nodes in a layer. Reducing to maximum allowed value."
        Public Const MSGMINOFFSET As String = "The value of offset must be at least 2"
        Public Const MSGSMOOTHINGCYCLES As String = "There must be at least 1 smoothing cycle if used. Recommend a minimum of 8"
        Public Const MSGFILEERROR As String = "There was an error reading the file"
        Public Const MSGOVERLAP As String = "There are overlapping nodes"
        Public Const MSGFLOW As String = "Flow direction must be an integer between 1 and 4"
        Public Const MSGFINALIZED As String = "Grid is finalized and ready for CFD solution"

    End Class
End Namespace