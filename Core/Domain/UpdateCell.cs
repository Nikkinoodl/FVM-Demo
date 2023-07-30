using Core.DataCollections;
using Core.Common;

namespace Core.Domain
{
    public class UpdateCell : Cell
    {
        // When cell t in the celllist must be updated
        // To avoid confusion, note that t is the index, not the Id

        public UpdateCell(int this_t, int this_v1, int this_v2, int this_v3, SideType this_s1, SideType this_s2, SideType this_s3)
        {
            Cell t = Repository.CellList[this_t];

            t.V1 = this_v1;
            t.V2 = this_v2;
            t.V3 = this_v3;
            t.Complete = true;
            t.Edge1.SideType = this_s1;
            t.Edge2.SideType = this_s2;
            t.Edge3.SideType = this_s3;
        }
    }
}
