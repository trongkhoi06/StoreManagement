using StoreManagement.Models;

namespace StoreManagement.Class
{
    public class Client_InBoxItem
    {
        public Client_InBoxItem()
        {
        }

        public Client_InBoxItem(Accessory accessory,string containerID,double inBoxQuantity,int itemPK, bool isRestored)
        {
            AccessoryID = accessory.AccessoryID;
            AccessoryDescription = accessory.AccessoryDescription;
            Art = accessory.Art;
            Color = accessory.Color;
            Item = accessory.Item;
            ContainerID = containerID;
            InBoxQuantity = inBoxQuantity;
            ItemPK = itemPK;
            IsRestored = isRestored;
        }

        public string AccessoryID { get; set; }

        public string AccessoryDescription { get; set; }

        public string Art { get; set; }

        public string Color { get; set; }

        public string Item { get; set; }

        public string ContainerID { get; set; }

        public double InBoxQuantity { get; set; }

        public int ItemPK { get; set; }

        public bool IsRestored { get; set; }
    }
}
