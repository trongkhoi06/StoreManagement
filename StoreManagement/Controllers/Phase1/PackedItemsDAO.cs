using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using StoreManagement.Class;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    public class PackedItemsDAO
    {
        private UserModel db = new UserModel();

        public double Sample { get; set; }

        public double DefectLimit { get; set; }

        public double SumOfCheckedQuantity { get; set; }

        public double SumOfUnqualifiedQuantity { get; set; }

        public double SumOfCountedQuantity { get; set; }

        public double SumOfIdentifiedQuantity { get; set; }

        public bool IsPackedItemCreated(int PackPK, List<Client_OrderedItemPK_PackedQuantity_Comment> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                db.PackedItems.Add(new PackedItem(PackPK, list[i]));
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        public PackedItem GetPackedItem(int pk)
        {
            PackedItem packedItem = db.PackedItems.Find(pk);
            return packedItem;
        }

        public bool IsPackedItemClassified(IdentifiedItem identifiedItem)
        {
            try
            {
                PackedItem packedItem = (from pI in db.PackedItems
                                         where pI.PackedItemPK == identifiedItem.PackedItemPK
                                         select pI).FirstOrDefault();
                return packedItem.IsClassified;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool IsUpdatedPackedItem(PackedItem packedItem)
        {
            PackedItem dbPackedItem = GetPackedItem(packedItem.PackedItemPK);
            if (packedItem.PackedQuantity == 0)
            {
                db.PackedItems.Remove(dbPackedItem);
            }
            else
            {
                dbPackedItem.PackedQuantity = packedItem.PackedQuantity;
                dbPackedItem.Comment = packedItem.Comment;
                dbPackedItem.IsClassified = packedItem.IsClassified;
                db.Entry(dbPackedItem).State = EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw e;
            }

            return true;
        }

        public void DeletePackedItem(int PackedItemPK)
        {
            PackedItem packedItem = db.PackedItems.Find(PackedItemPK);
            if (packedItem == null)
            {
                throw new Exception("ITEM PACK KHÔNG TỒN TẠI");
            }
            db.PackedItems.Remove(packedItem);
            db.SaveChanges();
        }

        public bool ChangeContract(int packedItemPK, string contractNumber)
        {
            PackedItem packedItem = db.PackedItems.Find(packedItemPK);
            packedItem.ContractNumber = contractNumber;
            try
            {
                db.Entry(packedItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw e;
            }

            return true;
        }

        public bool IsInitAllCalculate(int packedItemPK)
        {
            try
            {
                List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                        where iI.IsChecked == true && iI.PackedItemPK == packedItemPK
                                                        select iI).ToList();
                foreach (var item in identifiedItems)
                {
                    // init session
                    CountingSession countingSession = (from countss in db.CountingSessions
                                                       where countss.IdentifiedItemPK == item.IdentifiedItemPK
                                                       select countss).FirstOrDefault();
                    CheckingSession checkingSession = (from checkss in db.CheckingSessions
                                                       where checkss.IdentifiedItemPK == item.IdentifiedItemPK
                                                       select checkss).FirstOrDefault();

                    // assign value to variable
                    Sample += item.IdentifiedQuantity / 10;
                    DefectLimit += item.IdentifiedQuantity / 10;
                    SumOfIdentifiedQuantity += item.IdentifiedQuantity;
                    if (countingSession != null)
                    {
                        SumOfCountedQuantity += countingSession.CountedQuantity;
                    }
                    if (checkingSession != null)
                    {
                        SumOfUnqualifiedQuantity += checkingSession.UnqualifiedQuantity;
                        SumOfCheckedQuantity += checkingSession.CheckedQuantity;
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }
    }
}