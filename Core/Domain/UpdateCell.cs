using Core.DataCollections;
using Core.Common;

namespace Core.Domain
{
    public class UpdateCell : Cell
    {

        /// <summary>
        /// Updates the details of an existing triangular cell
        /// </summary>
        /// <param name="t">the index of the cell in Celllist</param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="s3"></param>
        public UpdateCell(int t, int v1, int v2, int v3, SideType s1, SideType s2, SideType s3)
        {
            Cell cell = Repository.CellList[t];

            cell.V1 = v1;
            cell.V2 = v2;
            cell.V3 = v3;
            cell.Complete = true;
            cell.Edge1.SideType = s1;
            cell.Edge2.SideType = s2;
            cell.Edge3.SideType = s3;
        }
    }
}
