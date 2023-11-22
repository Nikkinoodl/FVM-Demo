namespace Core.Domain

{
    public abstract class BaseTriangle
    {
        public int Id { get; set; }
        public int V1 { get; set; }   // each vertex is a node
        public int V2 { get; set; }
        public int V3 { get; set; }
        public double AvgX { get; set; }  // avgx and avgy are x and y coords of triangle center
        public double AvgY { get; set; }     
        public Edge Edge1 { get; set; }    //holds information related to edge opposide V1
        public Edge Edge2 { get; set; }    //holds information related to edge opposide V2
        public Edge Edge3 { get; set; }    //holds information related to edge opposide V3
        public double Flux { get; set; }  // total flux for the element
        public bool Complete { get; set; }
        public double Area { get; }
    }
}

