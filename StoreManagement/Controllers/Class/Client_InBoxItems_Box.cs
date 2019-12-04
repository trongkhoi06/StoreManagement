using StoreManagement.Models;
using System.Collections.Generic;

namespace StoreManagement.Class
{
    public class Client_InBoxItems_Box<T>
    {
        public Client_InBoxItems_Box()
        {
        }

        public Client_InBoxItems_Box(T boxes, List<Client_InBoxItem> inBoxItems)
        {
            Boxes = boxes;
            InBoxItems = inBoxItems;
        }

        public T Boxes { get; set; }

        public List<Client_InBoxItem> InBoxItems { get; set; }
    }
}
