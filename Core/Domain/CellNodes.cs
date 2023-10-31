namespace Core.Domain
{
    public class CellNodes
    {

        public int N1 { get; set; }

        public int N2 { get; set; }

        public int N3 { get; set; }

        //optional nodes that are used for N_sides > 3
        public int? N4 { get; set; }
    }
}
