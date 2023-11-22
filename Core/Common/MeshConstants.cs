namespace Core.Common
{
    /// <summary>
    /// Hard coded constants used throughout the application
    /// </summary>
    public class MeshConstants
    {
        public const string DIALOGTITLE = "Select a File";
        public const string FILELOCATION = @"C:\Users\Simon\Projects\Apps";
        public const string MSGCOMPLETE = "Complete";
        public const string MSGINITIALIZE = "Initializing";
        public const string MSGDELAUNAY = "Starting Delaunay Triangulation";
        public const string MSGDIVIDE = "Refining Grid";
        public const string MSGLOADED = "Data Loaded From File";
        public const string MSGCONSTRUCT = "Building Grid";
        public const string MSGREDISTRIBUTE = "Redistributing Boundary Edge Nodes";
        public const string MSGSMOOTH = "Performing Laplace Smoothing";
        public const string MSGNUMNODES = "The value of nodetrade exceeds the number of nodes on the boundary side. Setting to maximum allowed value.";
        public const string MSGOFFSET = "The value of offset exceeds the number of nodes in a layer. Reducing to maximum allowed value.";
        public const string MSGMINOFFSET = "The value of offset must be at least 2";
        public const string MSGSMOOTHINGCYCLES = "There must be at least 1 smoothing cycle if used. Recommend a minimum of 8";
        public const string MSGFILEERROR = "There was an error reading the file";
        public const string MSGOVERLAP = "There are overlapping nodes";
        public const string MSGFINALIZED = "Grid is finalized and ready for CFD solution";
        public const string MSGFINISHED1 = "CFD first order solution complete";
        public const string MSGPRECALC = "Grid pre-calc is complete";
        public const string MSGCFDDONE = "CFD run is complete";
        public const string MSGEMPTY = "";
    }
}

