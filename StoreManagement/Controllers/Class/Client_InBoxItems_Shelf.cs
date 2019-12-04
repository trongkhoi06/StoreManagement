using StoreManagement.Models;
using System.Collections.Generic;

namespace StoreManagement.Class
{
    public class Client_InBoxItems_Shelf<T>
    {
        public Client_InBoxItems_Shelf()
        {
        }

        public Client_InBoxItems_Shelf(T shelf, List<Client_InBoxItem> inBoxItems)
        {
            Shelf = shelf;
            InBoxItems = inBoxItems;
        }

        public T Shelf { get; set; }

        public List<Client_InBoxItem> InBoxItems { get; set; }
    }
}
