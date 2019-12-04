using StoreManagement.Models;
using System.Collections.Generic;

namespace StoreManagement.Class
{
    public class Client_InBoxItems_Row<T>
    {
        public Client_InBoxItems_Row()
        {
        }

        public Client_InBoxItems_Row(T rows, List<Client_InBoxItem> inBoxItems)
        {
            Rows = rows;
            InBoxItems = inBoxItems;
        }

        public T Rows { get; set; }

        public List<Client_InBoxItem> InBoxItems { get; set; }
    }
}
