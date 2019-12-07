using StoreManagement.Models;
using System.Collections.Generic;

namespace StoreManagement.Class
{
    public class Client_InBoxItems_Box<T>
    {
        public Client_InBoxItems_Box()
        {
        }

        public Client_InBoxItems_Box(T boxIDs, List<Client_InBoxItem> inBoxItems , string row)
        {
            Row = row;
            BoxIDs = boxIDs;
            InBoxItems = inBoxItems;
        }

        public string Row { get; set; }

        public T BoxIDs { get; set; }

        public List<Client_InBoxItem> InBoxItems { get; set; }
    }
}
