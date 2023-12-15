
using Core.Common;
using Core.DataCollections;

namespace Core.Domain
{
    /// <summary>
    /// A cell with zero height consisting of a single edge. Use to set boundary conditions
    /// </summary>
    public class BorderCell : Cell
    {

        #region Constructor
        public BorderCell(int this_id, int v1, int v2, Edge edge1, BorderType borderType = BorderType.Farfield)
        {
            Id = this_id;
            V1 = v1;
            V2 = v2;
            R = edge1.R;
            Complete = false;
            BorderCell = true;
            BorderCellType = borderType;
            CellType = CellType.line;
            Edge1 = edge1;
            Edges = new List<Edge>
            {
                Edge1
            };

            Repository.CellList.Add(this);
        }

        #endregion
    }
}
