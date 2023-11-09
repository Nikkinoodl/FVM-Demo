using Core.Interfaces;
using Core.DataCollections;

namespace Core.Data
{
    public class DataPreparer : IDataPreparer
    {

        /// <summary>
        /// Prepare the data storage
        /// </summary>
        public void PrepareRepository()
        {

            // clears out any existing data in lists
            Repository.ClearLists();
        }
    }
}
