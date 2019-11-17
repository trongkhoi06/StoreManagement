using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using StoreManagement.Class;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    public class PacksController
    {
        private UserModel db = new UserModel();
        public Pack CreatePack(string PackID, int OrderPK)
        {
            PrimitiveType primitiveType = new PrimitiveType();
            if (primitiveType.isPackID(PackID))
            {
                // Khởi tạo Pack
                Pack Pack = new Pack(PackID, OrderPK);
                db.Packs.Add(Pack);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    if (PackExists(Pack.PackID))
                    {
                        throw new Exception("PackID ĐÃ TỒN TẠI");
                    }
                    else
                    {
                        throw e;
                    }
                }
                return Pack;
            }
            else
            {
                throw new Exception("MÃ ĐƠN ĐẶT HÀNG KHÔNG PHÙ HỢP");
            }
        }

        public Pack GetPackByPackPK(int packPK)
        {
            return db.Packs.Find(packPK);
        }

        internal bool isContainIdentifiedItem(int packPK)
        {
            bool result = false;
            try
            {
                //SqlParameter OrderParam = new SqlParameter("@Pack", orderPK);
                //result = (db.Database.SqlQuery<string>("exec isContainPack @OrderPK", OrderParam).FirstOrDefault()).Equals("True");
                List<PackedItem> packedItems = (from pi in db.PackedItems
                                                where pi.PackPK == packPK
                                                select pi).ToList();

                foreach (var packedItem in packedItems)
                {
                    List<IdentifiedItem> temp = (from ii in db.IdentifiedItems
                                                 where ii.PackedItemPK == packedItem.PackedItemPK
                                                 select ii).ToList();
                    if (temp.Count != 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        private bool PackExists(string id)
        {
            return db.Packs.Count(e => e.PackID == id) > 0;
        }

        public void DeletePack(int PackPK)
        {
            Pack pack = db.Packs.Find(PackPK);
            if (pack == null)
            {
                throw new Exception("ĐƠN HÀNG KHÔNG TỒN TẠI");
            }
            db.Packs.Remove(pack);
            db.SaveChanges();
        }

        public void SwiftPackState(int packPK)
        {

            Pack pack = db.Packs.Find(packPK);
            if (pack.IsOpened)
            {
                pack.IsOpened = !pack.IsOpened;
            }
            else
            {
                if (!isContainIdentifiedItem(packPK))
                {
                    pack.IsOpened = !pack.IsOpened;
                }
                else
                {
                    throw new Exception("PACK ĐÃ CHỨA CLASSIFIED ITEM");
                }
            }
            db.Entry(pack).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}