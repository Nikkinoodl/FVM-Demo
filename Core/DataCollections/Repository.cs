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

        // Clear lists
        static public void ClearLists()
        {
            Nodelist?.Clear();
            CellList?.Clear();
        }
    }
}

