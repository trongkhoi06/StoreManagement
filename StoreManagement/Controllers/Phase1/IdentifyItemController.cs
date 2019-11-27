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
    public class IdentifyItemController
    {
        private UserModel db = new UserModel();

        public IdentifyingSession createdIdentifyingSession(string EmployeeCode)
        {
            IdentifyingSession identifyingSession = new IdentifyingSession(EmployeeCode);
            db.IdentifyingSessions.Add(identifyingSession);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            identifyingSession = (from iss in db.IdentifyingSessions.OrderByDescending(unit => unit.IdentifyingSessionPK)
                                  select iss).FirstOrDefault();
            return identifyingSession;
        }

        public void createIndentifyItem(IdentifiedItem item)
        {
            try
            {
                db.IdentifiedItems.Add(item);
                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public void changeExecutedDate(IdentifyingSession session)
        {
            try
            {
                session.ExecutedDate = DateTime.Now;
                db.Entry(session).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public void updateIdentifiedItem(int IdentifiedItemPK, int IdentifiedQuantity)
        {
            try
            {
                IdentifiedItem identifiedItem = db.IdentifiedItems.Find(IdentifiedItemPK);
                identifiedItem.IdentifiedQuantity = IdentifiedQuantity;
                db.Entry(identifiedItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void deleteIdentifiedItem(int IdentifiedItemPK)
        {
            try
            {
                IdentifiedItem identifiedItem = db.IdentifiedItems.Find(IdentifiedItemPK);
                db.IdentifiedItems.Remove(identifiedItem);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void deleteIdentifiedItemsOfSession(int IdentifyingSessionPK)
        {
            try
            {
                IdentifyingSession identifyingSession = db.IdentifyingSessions.Find(IdentifyingSessionPK);
                List<IdentifiedItem> list = (from ii in db.IdentifiedItems
                                             where ii.IdentifyingSessionPK == IdentifyingSessionPK
                                             select ii).ToList();
                foreach (var item in list)
                {
                    db.IdentifiedItems.Remove(item);
                }
                db.IdentifyingSessions.Remove(identifyingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ArrangeIndentifiedItem(IdentifiedItem identifiedItem)
        {
            try
            {
                db.Entry(identifiedItem).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ArrangingSession createArrangingSession(int boxFromPK, int boxToPK, string userID)
        {
            ArrangingSession arrangingSession = new ArrangingSession(boxFromPK, boxToPK, userID);
            db.ArrangingSessions.Add(arrangingSession);
            try
            {
                db.SaveChanges();
                arrangingSession = (from ass in db.ArrangingSessions.OrderByDescending(unit => unit.ArrangingSessionPK)
                                    select ass).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
            return arrangingSession;
        }

        public void MapItemWithSession(IdentifiedItem_ArrangingSession identifiedItem_ArrangingSession)
        {
            db.IdentifiedItem_ArrangingSessions.Add(identifiedItem_ArrangingSession);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int ActualQuantity(int identifiedItemPK)
        {
            int result = 0;
            int numCase = 0;
            try
            {
                IdentifiedItem item = db.IdentifiedItems.Find(identifiedItemPK);
                CheckingSession checkingSession = (from checkss in db.CheckingSessions
                                                   where checkss.IdentifiedItemPK == item.IdentifiedItemPK
                                                   select checkss).FirstOrDefault();
                CountingSession countingSession = (from countss in db.CountingSessions
                                                   where countss.IdentifiedItemPK == item.IdentifiedItemPK
                                                   select countss).FirstOrDefault();
                if (item.IsChecked == false && item.IsCounted == false) numCase = 1;
                if (item.IsChecked == false && item.IsCounted == true) numCase = 2;
                if (item.IsChecked == true && item.IsCounted == false) numCase = 3;
                if (item.IsChecked == true && item.IsCounted == true)
                {

                    if (checkingSession.ExecutedDate < countingSession.ExecutedDate) numCase = 4;
                    else numCase = 5;
                }
                switch (numCase)
                {
                    case 1:
                        result += item.IdentifiedQuantity;
                        break;
                    case 2:
                        result += countingSession.CountedQuantity;
                        break;
                    case 3:
                        result += item.IdentifiedQuantity - checkingSession.UnqualifiedQuantity;
                        break;
                    case 4:
                        result += countingSession.CountedQuantity;
                        break;
                    case 5:
                        result += countingSession.CountedQuantity - checkingSession.UnqualifiedQuantity;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public int GenerateFinalQuantity(int packedItemPK)
        {
            int result = 0;
            try
            {
                List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                        where iI.PackedItemPK == packedItemPK
                                                        select iI).ToList();
                foreach (var item in identifiedItems)
                {
                    int numCase = 0;
                    CheckingSession checkingSession = (from checkss in db.CheckingSessions
                                                       where checkss.IdentifiedItemPK == item.IdentifiedItemPK
                                                       select checkss).FirstOrDefault();
                    CountingSession countingSession = (from countss in db.CountingSessions
                                                       where countss.IdentifiedItemPK == item.IdentifiedItemPK
                                                       select countss).FirstOrDefault();
                    if (item.IsChecked == false && item.IsCounted == false) numCase = 1;
                    if (item.IsChecked == false && item.IsCounted == true) numCase = 2;
                    if (item.IsChecked == true && item.IsCounted == false) numCase = 3;
                    if (item.IsChecked == true && item.IsCounted == true)
                    {

                        if (checkingSession.ExecutedDate < countingSession.ExecutedDate) numCase = 4;
                        else numCase = 5;
                    }
                    switch (numCase)
                    {
                        case 1:
                            result += item.IdentifiedQuantity;
                            break;
                        case 2:
                            result += countingSession.CountedQuantity;
                            break;
                        case 3:
                            result += item.IdentifiedQuantity - checkingSession.UnqualifiedQuantity;
                            break;
                        case 4:
                            result += countingSession.CountedQuantity;
                            break;
                        case 5:
                            result += countingSession.CountedQuantity - checkingSession.UnqualifiedQuantity;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }


    }
}
