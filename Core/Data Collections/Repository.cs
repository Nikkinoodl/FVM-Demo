using Core.Domain;

namespace Core.DataCollections
{
    public static class Repository
    {
        /// <summary>
        /// A list of all nodes that make up the mesh
        /// </summary>
        public static List<Node> Nodelist { get; set; } = new();

        /// <summary>
        /// A list of all cells that make up the mesh
        /// </summary>
        public static List<Cell> CellList { get; set; } = new();

        /// <summary>
        /// Clears data from lists
        /// </summary>
        static public void ClearLists()
        {
            Nodelist?.Clear();
            CellList?.Clear();
        }
    }
}

