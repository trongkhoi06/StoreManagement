using StoreManagement.Models;
using System.Collections.Generic;

namespace StoreManagement.Class
{
    public class Client_InBoxItems_Box<T>
    {
        public Client_InBoxItems_Box()
        {
        }

        public Client_InBoxItems_Box(T boxes, List<Client_InBoxItem> inBoxItems , string row)
        {
            Row = row;
            Boxes = boxes;
            InBoxItems = inBoxItems;
        }

        public string Row { get; set; }

        public T Boxes { get; set; }

        public List<Client_InBoxItem> InBoxItems { get; set; }
    }
}
