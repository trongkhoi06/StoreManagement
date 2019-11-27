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
    public class ClassifyingItemController
    {
        private UserModel db = new UserModel();

        public void createClassifyingSession(ClassifyingSession classifyingSession)
        {
            try
            {
                db.ClassifyingSessions.Add(classifyingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateClassifyingSession(ClassifyingSession classifyingSession)
        {
            try
            {
                db.Entry(classifyingSession).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void deleteClassifyingSession(int classifyingSessionPK)
        {
            try
            {
                ClassifyingSession classifyingSession = db.ClassifyingSessions.Find(classifyingSessionPK);
                db.ClassifyingSessions.Remove(classifyingSession);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ClassifiedItem createClassifiedItem(ClassifiedItem classifiedItem)
        {
            try
            {
                db.ClassifiedItems.Add(classifiedItem);
                db.SaveChanges();
                return (from cI in db.ClassifiedItems.OrderByDescending(unit => unit.ClassifiedItemPK)
                        select cI).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateClassifiedItem(ClassifiedItem classifiedItem)
        {
            try
            {
                db.Entry(classifiedItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void deleteClassifiedItem(int classifiedItemPK)
        {
            try
            {
                ClassifiedItem classifiedItem = db.ClassifiedItems.Find(classifiedItemPK);
                db.ClassifiedItems.Remove(classifiedItem);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void createItemByQualityState(int classifiedItemPK, int qualityState)
        {
            try
            {
                switch (qualityState)
                {
                    case 2:
                        db.PassedItems.Add(new PassedItem(classifiedItemPK));
                        break;
                    case 3:
                        db.FailedItems.Add(new FailedItem(classifiedItemPK));
                        break;
                    default:
                        break;
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void deleteItemByQualityState(int classifiedItemPK, int qualityState)
        {
            try
            {

                switch (qualityState)
                {
                    case 2:
                        PassedItem passedItem = (from pI in db.PassedItems
                                                 where pI.ClassifiedItemPK == classifiedItemPK
                                                 select pI).FirstOrDefault();
                        db.PassedItems.Remove(passedItem);
                        break;
                    case 3:
                        FailedItem failedItem = (from fI in db.FailedItems
                                                 where fI.ClassifiedItemPK == classifiedItemPK
                                                 select fI).FirstOrDefault();
                        db.FailedItems.Remove(failedItem);
                        break;
                    default:
                        break;
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void manageItemByQualityState(int classifiedItemPK, int oldQualityState, int newQualityState)
        {
            try
            {
                if (oldQualityState != newQualityState)
                {
                    if (oldQualityState == 1)
                    {
                        createItemByQualityState(classifiedItemPK, newQualityState);
                    }
                    else if (oldQualityState == 2)
                    {
                        if (newQualityState == 3)
                        {
                            deleteItemByQualityState(classifiedItemPK, oldQualityState);
                            createItemByQualityState(classifiedItemPK, newQualityState);
                        }
                    }
                    else if (oldQualityState == 3)
                    {
                        if (newQualityState == 2)
                        {
                            deleteItemByQualityState(classifiedItemPK, oldQualityState);
                            createItemByQualityState(classifiedItemPK, newQualityState);
                        }
                    }
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public bool isNotStoredOrReturned(int classifiedItemPK)
        {
            ClassifiedItem classifiedItem = db.ClassifiedItems.Find(classifiedItemPK);
            if (classifiedItem.QualityState == 2)
            {
                PassedItem passedItem = (from pI in db.PassedItems
                                         where pI.ClassifiedItemPK == classifiedItemPK
                                         select pI).FirstOrDefault();
                if (passedItem.IsStored) return false;
            }
            else if (classifiedItem.QualityState == 3)
            {
                FailedItem failedItem = (from fI in db.FailedItems
                                         where fI.ClassifiedItemPK == classifiedItemPK
                                         select fI).FirstOrDefault();
                if (failedItem.IsReturned) return false;
            }
            return true;
        }
    }
}

