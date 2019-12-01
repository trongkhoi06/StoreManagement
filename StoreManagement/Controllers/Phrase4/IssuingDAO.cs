using StoreManagement.Class;
using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StoreManagement.Controllers
{
    public class IssuingDAO
    {
        private UserModel db = new UserModel();

        //public double InStoreQuantity(int accessoryPK)
        //{
        //    double result = 0;
        //    StoringDAO storingDAO = new StoringDAO();
        //    try
        //    {
        //        List<PassedItem> passedItems = new List<PassedItem>();
        //        Accessory accessory = db.Accessories.Find(accessoryPK);
        //        // lấy restoredItems
        //        List<RestoredItem> restoredItems = (from rI in db.RestoredItems
        //                                            where rI.AccessoryPK == accessory.AccessoryPK
        //                                            select rI).ToList();
        //        // lấy passedItems
        //        List<OrderedItem> orderedItems = (from oI in db.OrderedItems
        //                                          where oI.AccessoryPK == accessory.AccessoryPK
        //                                          select oI).ToList();
        //        List<List<PackedItem>> packedItemss = new List<List<PackedItem>>();
        //        foreach (var orderedItem in orderedItems)
        //        {
        //            List<PackedItem> packedItems = (from pI in db.PackedItems
        //                                            where pI.OrderedItemPK == orderedItem.OrderedItemPK
        //                                            select pI).ToList();
        //            if (packedItems.Count > 0) packedItemss.Add(packedItems);
        //        }
        //        if (packedItemss.Count > 0)
        //        {
        //            foreach (var packedItems in packedItemss)
        //            {
        //                foreach (var packedItem in packedItems)
        //                {
        //                    ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
        //                                                     where cI.PackedItemPK == packedItem.PackedItemPK
        //                                                     select cI).FirstOrDefault();
        //                    if (classifiedItem != null && classifiedItem.QualityState == 2)
        //                    {
        //                        PassedItem passedItem = (from p)
        //                    }
        //                }
        //            }
        //        }
                
        //        //storingDAO.InBoxQuantity()
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    return result;
        //}
    }
}

