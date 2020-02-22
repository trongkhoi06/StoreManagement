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
        
        public double SumOfIdentifiedQuantityCounted { get; set; }

        public bool IsPackedItemCreated(int PackPK, List<Client_OrderedItemPK_PackedQuantity_Comment> list, int orderPK)
        {
            try
            {
                foreach (var item in list)
                {
                    if (PrimitiveType.isValidQuantity(item.PackedQuantity) && PrimitiveType.isValidComment(item.Comment))
                    {
                        OrderedItem orderedItem = db.OrderedItems.Find(item.OrderedItemPK);
                        if (orderedItem == null) throw new Exception("PHỤ LIỆU ĐƠN ĐẶT KHÔNG TỒN TẠI!");
                        if (orderedItem.OrderPK != orderPK) throw new Exception("PHỤ LIỆU ĐƠN ĐẶT KHÔNG CÙNG ĐƠN ĐẶT!");
                        db.PackedItems.Add(new PackedItem(PackPK, item));
                    }
                    else
                    {
                        throw new Exception(SystemMessage.NotPassPrimitiveType);
                    }
                }
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

        public bool IsUpdatedPackedItem(PackedItem packedItem, int packPK)
        {
            Pack pack = db.Packs.Find(packPK);
            PackedItem dbPackedItem = GetPackedItem(packedItem.PackedItemPK);
            if (packedItem.PackedQuantity == 0)
            {
                db.PackedItems.Remove(dbPackedItem);

                pack.DateCreated = DateTime.Now;
                db.Entry(pack).State = EntityState.Modified;
            }
            else
            {
                if (PrimitiveType.isValidQuantity(packedItem.PackedQuantity) && PrimitiveType.isValidComment(packedItem.Comment))
                {

                    dbPackedItem.PackedQuantity = packedItem.PackedQuantity;
                    dbPackedItem.Comment = packedItem.Comment;
                    db.Entry(dbPackedItem).State = EntityState.Modified;

                    pack.DateCreated = DateTime.Now;
                    db.Entry(pack).State = EntityState.Modified;
                }
                else
                {
                    throw new Exception(SystemMessage.NotPassPrimitiveType);
                }
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

        public void IsInitAllCalculate(int packedItemPK)
        {
            try
            {
                double tempSample = 0;
                double tempDefectLimit = 0;
                List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                        where iI.PackedItemPK == packedItemPK
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
                    tempSample += item.IdentifiedQuantity;
                    tempDefectLimit += item.IdentifiedQuantity;
                    SumOfIdentifiedQuantity += item.IdentifiedQuantity;
                    if (item.IsCounted)
                    {
                        SumOfIdentifiedQuantityCounted += item.IdentifiedQuantity;
                    }
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
                Sample = SampleCaculate(tempSample);
                DefectLimit = DefectLimitCaculate(tempDefectLimit);
            }
            catch (Exception e)
            {
                throw new Exception("HÀM TÍNH SAMPLE VÀ DEFECT LIMIT BỊ LỖI");
            }
        }

        public double SampleCaculate(double tempSample)
        {
            if (tempSample < 151) return 20;
            if (tempSample < 281) return 32;
            if (tempSample < 501) return 50;
            if (tempSample < 1201) return 80;
            if (tempSample < 3201) return 125;
            if (tempSample < 10001) return 200;
            if (tempSample < 35001) return 315;
            if (tempSample < 150001) return 500;
            if (tempSample < 500001) return 800;
            return 1250;
        }
        public double DefectLimitCaculate(double tempDefectLimit)
        {
            if (tempDefectLimit < 151) return 1;
            if (tempDefectLimit < 281) return 1;
            if (tempDefectLimit < 501) return 2;
            if (tempDefectLimit < 1201) return 2;
            if (tempDefectLimit < 3201) return 3;
            if (tempDefectLimit < 10001) return 4;
            if (tempDefectLimit < 35001) return 6;
            if (tempDefectLimit < 150001) return 8;
            if (tempDefectLimit < 500001) return 11;
            return 15;
        }
    }
}