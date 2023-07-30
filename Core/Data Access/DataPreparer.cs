using Core.Interfaces;
using Core.DataCollections;

namespace Core.Data
{
    public class DataPreparer : IDataPreparer
    {
        public void PrepareRepository()
        {

            // clears out any existing data in lists
            Repository.ClearLists();
        }
    }
}
