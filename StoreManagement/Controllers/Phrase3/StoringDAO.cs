using StoreManagement.Class;
using StoreManagement.Controllers.Class;
using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static StoreManagement.Controllers.StoringController;

namespace StoreManagement.Controllers
{
  public class StoringDAO
  {
    private UserModel db = new UserModel();

    public StoredBox UpdateStoredBox(int boxPK, int? shelfPK)
    {
      try
      {
        // tạo storedBox
        StoredBox storedBox = db.StoredBoxes.Where(unit => unit.BoxPK == boxPK).FirstOrDefault();
        storedBox.ShelfPK = shelfPK;
        db.Entry(storedBox).State = EntityState.Modified;
        db.SaveChanges();
        return storedBox;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public StoringSession CreateStoringSession(string userID)
    {
      try
      {
        // tạo ss
        StoringSession storingSession = new StoringSession(userID);
        db.StoringSessions.Add(storingSession);
        db.SaveChanges();
        return db.StoringSessions.OrderByDescending(unit => unit.StoringSessionPK).FirstOrDefault();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public void CreateEntriesAndUpdateItem(Box box, StoredBox storedBox, StoringSession storingSession)
    {
      try
      {
        IdentifyItemDAO identifyItemDAO = new IdentifyItemDAO();
        BoxDAO boxDAO = new BoxDAO();
        UnstoredBox unstoredBox = boxDAO.GetUnstoredBoxbyBoxPK(box.BoxPK);

        List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                where iI.UnstoredBoxPK == unstoredBox.UnstoredBoxPK
                                                select iI).ToList();
        foreach (var identifiedItem in identifiedItems)
        {
          PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
          // lấy accessory
          OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
          Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
          //
          ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                           where cI.PackedItemPK == packedItem.PackedItemPK
                                           select cI).FirstOrDefault();
          if (classifiedItem != null)
          {
            PassedItem passedItem = (from pI in db.PassedItems
                                     where pI.ClassifiedItemPK == classifiedItem.ClassifiedItemPK
                                     select pI).FirstOrDefault();
            if (passedItem != null)
            {
              UpdatePassedItem(passedItem.PassedItemPK);
              // tạo entry
              Entry entry = new Entry(storedBox, "Storing", storingSession.StoringSessionPK, false,
                  identifyItemDAO.ActualQuantity(identifiedItem.IdentifiedItemPK), passedItem.PassedItemPK, accessory);
              db.Entries.Add(entry);
            }
            else
            {
              throw new Exception("PHỤ LIỆU KHÔNG HỢP LỆ");
            }
          }
          else
          {
            throw new Exception("PHỤ LIỆU KHÔNG HỢP LỆ");
          }

          identifiedItem.StoringSessionPK = storingSession.StoringSessionPK;
          identifiedItem.UnstoredBoxPK = null;
          db.Entry(identifiedItem).State = EntityState.Modified;
        }

        List<RestoredGroup> restoredGroups = (from rg in db.RestoredGroups
                                              where rg.UnstoredBoxPK == unstoredBox.UnstoredBoxPK
                                              select rg).ToList();
        foreach (var restoredGroup in restoredGroups)
        {
          RestoredItem restoredItem = db.RestoredItems.Find(restoredGroup.RestoredItemPK);
          // lấy accessory
          Accessory accessory = db.Accessories.Find(restoredItem.AccessoryPK);

          // tạo entry
          Entry entry = new Entry(storedBox, "Storing", storingSession.StoringSessionPK, true,
              restoredGroup.GroupQuantity, restoredItem.RestoredItemPK, accessory);
          db.Entries.Add(entry);

          restoredGroup.UnstoredBoxPK = null;
          restoredGroup.StoringSessionPK = storingSession.StoringSessionPK;
          db.Entry(restoredGroup).State = EntityState.Modified;
        }


        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public void CreateEntriesAndUpdateItem2(Box box, StoringSession storingSession, List<Client_GroupItem_Store> input)
    {
      try
      {
        IdentifyItemDAO identifyItemDAO = new IdentifyItemDAO();
        BoxDAO boxDAO = new BoxDAO();
        UnstoredBox unstoredBox = boxDAO.GetUnstoredBoxbyBoxPK(box.BoxPK);
        StoredBox storedBox = boxDAO.GetStoredBoxbyBoxPK(box.BoxPK);

        foreach (var item in input)
        {
          IdentifiedItem identifiedItem = db.IdentifiedItems.Find(item.PK);
          if (!item.IsRestored)
          {
            PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
            // lấy accessory
            OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
            Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
            //
            ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                             where cI.PackedItemPK == packedItem.PackedItemPK
                                             select cI).FirstOrDefault();
            if (classifiedItem != null)
            {
              PassedItem passedItem = (from pI in db.PassedItems
                                       where pI.ClassifiedItemPK == classifiedItem.ClassifiedItemPK
                                       select pI).FirstOrDefault();
              if (passedItem != null)
              {
                UpdatePassedItem(passedItem.PassedItemPK);
                // tạo entry
                Entry entry = new Entry(storedBox, "Storing", storingSession.StoringSessionPK, item.IsRestored,
                    identifyItemDAO.ActualQuantity(identifiedItem.IdentifiedItemPK), passedItem.PassedItemPK, accessory);
                db.Entries.Add(entry);
              }
              else
              {
                throw new Exception("PHỤ LIỆU KHÔNG HỢP LỆ");
              }
            }
            else
            {
              throw new Exception("PHỤ LIỆU KHÔNG HỢP LỆ");
            }

            identifiedItem.StoringSessionPK = storingSession.StoringSessionPK;
            db.Entry(identifiedItem).State = EntityState.Modified;
          }
          else
          {
            RestoredGroup restoredGroup = db.RestoredGroups.Find(item.PK);
            RestoredItem restoredItem = db.RestoredItems.Find(restoredGroup.RestoredItemPK);
            // lấy accessory
            Accessory accessory = db.Accessories.Find(restoredItem.AccessoryPK);

            // tạo entry
            Entry entry = new Entry(storedBox, "Storing", storingSession.StoringSessionPK, item.IsRestored,
                restoredGroup.GroupQuantity, restoredItem.RestoredItemPK, accessory);
            db.Entries.Add(entry);

            restoredGroup.StoringSessionPK = storingSession.StoringSessionPK;
            db.Entry(restoredGroup).State = EntityState.Modified;
          }
        }

        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    private void UpdatePassedItem(int passedItemPK)
    {
      try
      {
        PassedItem passedItem = db.PassedItems.Find(passedItemPK);
        passedItem.IsStored = true;
        db.Entry(passedItem).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public void DeleteStoringSession(int storingSessionPK)
    {
      try
      {
        StoringSession storingSession = db.StoringSessions.Find(storingSessionPK);
        db.StoringSessions.Remove(storingSession);
        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public double EntryQuantity(Entry entry)
    {
      double result = 0;
      AdjustingSession adjustingSession;
      Verification verification;
      KindRole kindRole = db.KindRoles.Find(entry.KindRoleName);
      switch (kindRole.KindRoleName)
      {
        case "Discarding":
          DiscardingSession discardingSession = db.DiscardingSessions.Find(entry.SessionPK);
          verification = (from ver in db.Verifications
                          where ver.SessionPK == discardingSession.DiscardingSessionPK && !ver.IsDiscard
                          select ver).FirstOrDefault();
          if (!discardingSession.IsVerified)
          {
            return -1;
          }
          else if (discardingSession.IsVerified && !verification.IsApproved)
          {

          }
          else if (discardingSession.IsVerified && verification.IsApproved)
          {
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
          }
          break;
        // chưa approve cũng tính vào vì có thể là số lượng thực được chấp thuận gây số âm
        case "AdjustingMinus":
          adjustingSession = db.AdjustingSessions.Find(entry.SessionPK);
          verification = (from ver in db.Verifications
                          where ver.SessionPK == adjustingSession.AdjustingSessionPK && !ver.IsDiscard
                          select ver).FirstOrDefault();
          if (!adjustingSession.IsVerified)
          {
            return -1;
          }
          else if (adjustingSession.IsVerified && !verification.IsApproved)
          {

          }
          else if (adjustingSession.IsVerified && verification.IsApproved)
          {
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
          }
          break;
        // approve mới tính vào vì là gây ra vấn đề lấy số ảo
        case "AdjustingPlus":
          adjustingSession = db.AdjustingSessions.Find(entry.SessionPK);
          verification = (from ver in db.Verifications
                          where ver.SessionPK == adjustingSession.AdjustingSessionPK && !ver.IsDiscard
                          select ver).FirstOrDefault();
          if (!adjustingSession.IsVerified)
          {
            return -1;
          }
          else if (adjustingSession.IsVerified && !verification.IsApproved)
          {

          }
          else if (adjustingSession.IsVerified && verification.IsApproved)
          {
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
          }
          break;
        case "In":
          result += entry.Quantity * (kindRole.Sign ? 1 : -1);
          break;
        case "Issuing":
          result += entry.Quantity * (kindRole.Sign ? 1 : -1);
          break;
        case "Out":
          result += entry.Quantity * (kindRole.Sign ? 1 : -1);
          break;
        case "Receiving":
          result += entry.Quantity * (kindRole.Sign ? 1 : -1);
          break;
        case "Storing":
          result += entry.Quantity * (kindRole.Sign ? 1 : -1);
          break;
        default:
          break;
      }
      return result;
    }

    public double EntriesQuantity(List<Entry> entries)
    {
      double result = 0;
      foreach (var entry in entries)
      {
        KindRole kindRole = db.KindRoles.Find(entry.KindRoleName);
        AdjustingSession adjustingSession;
        Verification verification;
        switch (entry.KindRoleName)
        {
          case "Discarding":
            DiscardingSession discardingSession = db.DiscardingSessions.Find(entry.SessionPK);
            verification = (from ver in db.Verifications
                            where ver.SessionPK == discardingSession.DiscardingSessionPK && !ver.IsDiscard
                            select ver).FirstOrDefault();
            if (!discardingSession.IsVerified)
            {
              return -1;
            }
            else if (discardingSession.IsVerified && !verification.IsApproved)
            {

            }
            else if (discardingSession.IsVerified && verification.IsApproved)
            {
              result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            }
            break;
          // chưa approve cũng tính vào vì có thể là số lượng thực được chấp thuận gây số âm
          case "AdjustingMinus":
            adjustingSession = db.AdjustingSessions.Find(entry.SessionPK);
            verification = (from ver in db.Verifications
                            where ver.SessionPK == adjustingSession.AdjustingSessionPK && !ver.IsDiscard
                            select ver).FirstOrDefault();
            if (!adjustingSession.IsVerified)
            {
              return -1;
            }
            else if (adjustingSession.IsVerified && !verification.IsApproved)
            {

            }
            else if (adjustingSession.IsVerified && verification.IsApproved)
            {
              result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            }
            break;
          // approve mới tính vào vì là gây ra vấn đề lấy số ảo
          case "AdjustingPlus":
            adjustingSession = db.AdjustingSessions.Find(entry.SessionPK);
            verification = (from ver in db.Verifications
                            where ver.SessionPK == adjustingSession.AdjustingSessionPK && !ver.IsDiscard
                            select ver).FirstOrDefault();
            if (!adjustingSession.IsVerified)
            {
              return -1;
            }
            else if (adjustingSession.IsVerified && !verification.IsApproved)
            {

            }
            else if (adjustingSession.IsVerified && verification.IsApproved)
            {
              result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            }
            break;
          case "In":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          case "Issuing":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          case "Out":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          case "Receiving":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          case "Storing":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          default:
            break;
        }
      }
      return result;
    }

    public double InBoxQuantity(StoredBox sBox, int itemPK, bool isRestored)
    {
      double result = 0;
      List<Entry> entries = (from e in db.Entries
                             where e.StoredBoxPK == sBox.StoredBoxPK && e.IsRestored == isRestored && e.ItemPK == itemPK
                             select e).ToList();
      foreach (var entry in entries)
      {
        AdjustingSession adjustingSession;
        Verification verification;
        KindRole kindRole = db.KindRoles.Find(entry.KindRoleName);
        switch (entry.KindRoleName)
        {
          // approve mới tính vào vì là số lượng thực được chấp thuận
          // chưa approve cũng tính vào vì có thể là số lượng thực được chấp thuận gây số âm
          case "Discarding":
            DiscardingSession discardingSession = db.DiscardingSessions.Find(entry.SessionPK);
            verification = (from ver in db.Verifications
                            where ver.SessionPK == discardingSession.DiscardingSessionPK && !ver.IsDiscard
                            select ver).FirstOrDefault();
            if (!discardingSession.IsVerified)
            {
              return -1;
            }
            else if (discardingSession.IsVerified && !verification.IsApproved)
            {

            }
            else if (discardingSession.IsVerified && verification.IsApproved)
            {
              result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            }
            break;
          // chưa approve cũng tính vào vì có thể là số lượng thực được chấp thuận gây số âm
          case "AdjustingMinus":
            adjustingSession = db.AdjustingSessions.Find(entry.SessionPK);
            verification = (from ver in db.Verifications
                            where ver.SessionPK == adjustingSession.AdjustingSessionPK && !ver.IsDiscard
                            select ver).FirstOrDefault();
            if (!adjustingSession.IsVerified)
            {
              return -1;
            }
            else if (adjustingSession.IsVerified && !verification.IsApproved)
            {

            }
            else if (adjustingSession.IsVerified && verification.IsApproved)
            {
              result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            }
            break;
          // approve mới tính vào vì là gây ra vấn đề lấy số ảo
          case "AdjustingPlus":
            adjustingSession = db.AdjustingSessions.Find(entry.SessionPK);
            verification = (from ver in db.Verifications
                            where ver.SessionPK == adjustingSession.AdjustingSessionPK && !ver.IsDiscard
                            select ver).FirstOrDefault();
            if (!adjustingSession.IsVerified)
            {
              return -1;
            }
            else if (adjustingSession.IsVerified && !verification.IsApproved)
            {

            }
            else if (adjustingSession.IsVerified && verification.IsApproved)
            {
              result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            }
            break;
          case "In":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          case "Issuing":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          case "Out":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          case "Receiving":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          case "Storing":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          default:
            break;
        }
      }
      return result;
    }

    public double AvailableQuantity(StoredBox sBox, int itemPK, bool isRestored)
    {
      double result = 0;
      List<Entry> entries = (from e in db.Entries
                             where e.StoredBoxPK == sBox.StoredBoxPK && e.IsRestored == isRestored && e.ItemPK == itemPK
                             select e).ToList();
      if (entries.Count < 0)
      {
        throw new Exception("PHỤ LIỆU KHÔNG ĐƯỢC TÌM THẤY");
      }
      foreach (var entry in entries)
      {
        AdjustingSession adjustingSession;
        Verification verification;
        KindRole kindRole = db.KindRoles.Find(entry.KindRoleName);
        switch (entry.KindRoleName)
        {
          // chưa approve cũng tính vào vì có thể là số lượng thực được chấp thuận gây số âm
          case "Discarding":
            DiscardingSession discardingSession = db.DiscardingSessions.Find(entry.SessionPK);
            verification = (from ver in db.Verifications
                            where ver.SessionPK == discardingSession.DiscardingSessionPK && !ver.IsDiscard
                            select ver).FirstOrDefault();
            if (!discardingSession.IsVerified)
            {
              return -1;
            }
            else if (discardingSession.IsVerified && !verification.IsApproved)
            {

            }
            else if (discardingSession.IsVerified && verification.IsApproved)
            {
              result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            }
            break;
          // chưa approve cũng tính vào vì có thể là số lượng thực được chấp thuận gây số âm
          case "AdjustingMinus":
            adjustingSession = db.AdjustingSessions.Find(entry.SessionPK);
            verification = (from ver in db.Verifications
                            where ver.SessionPK == adjustingSession.AdjustingSessionPK && !ver.IsDiscard
                            select ver).FirstOrDefault();
            if (!adjustingSession.IsVerified)
            {
              return -1;
            }
            else if (adjustingSession.IsVerified && !verification.IsApproved)
            {

            }
            else if (adjustingSession.IsVerified && verification.IsApproved)
            {
              result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            }
            break;
          // approve mới tính vào vì là gây ra vấn đề lấy số ảo
          case "AdjustingPlus":
            adjustingSession = db.AdjustingSessions.Find(entry.SessionPK);
            verification = (from ver in db.Verifications
                            where ver.SessionPK == adjustingSession.AdjustingSessionPK && !ver.IsDiscard
                            select ver).FirstOrDefault();
            if (!adjustingSession.IsVerified)
            {
              return -1;
            }
            else if (adjustingSession.IsVerified && !verification.IsApproved)
            {

            }
            else if (adjustingSession.IsVerified && verification.IsApproved)
            {
              result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            }
            break;
          case "In":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          case "Issuing":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          case "Out":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          case "Receiving":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          case "Storing":
            result += entry.Quantity * (kindRole.Sign ? 1 : -1);
            break;
          default:
            break;
        }
      }
      return result;
    }

    public TransferringSession CreateTransferingSession(string boxFromID, string boxToID, string userID)
    {
      TransferringSession result;
      BoxDAO boxDAO = new BoxDAO();
      try
      {
        Box boxFrom = boxDAO.GetBoxByBoxID(boxFromID);
        StoredBox sBoxFrom = boxDAO.GetStoredBoxbyBoxPK(boxFrom.BoxPK);
        Box boxTo = boxDAO.GetBoxByBoxID(boxToID);
        StoredBox sBoxTo = boxDAO.GetStoredBoxbyBoxPK(boxTo.BoxPK);
        db.TransferringSessions.Add(new TransferringSession(sBoxFrom.StoredBoxPK, sBoxTo.StoredBoxPK, userID));
        db.SaveChanges();
        result = (from tSS in db.TransferringSessions.OrderByDescending(unit => unit.TransferingSessionPK)
                  select tSS).FirstOrDefault();
        return result;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public void CreateInAndOutEntry(List<Client_ItemPK_TransferQuantity_IsRestored> list, StoredBox sBoxFrom, StoredBox sBoxTo, TransferringSession transferringSession)
    {
      try
      {
        foreach (var item in list)
        {
          AvailableItem availableItem = new AvailableItem(sBoxFrom.StoredBoxPK, item.ItemPK, item.IsRestored);
          if (availableItem.IsPending)
          {
            throw new Exception("CỤM PHỤ LIỆU ĐANG CHỜ XỬ LÝ ĐIỀU CHỈNH!");
          }
          if (item.TransferQuantity > availableItem.AvailableQuantity)
          {
            throw new Exception("SỐ LƯỢNG CHUYỂN CỦA CỤM PHỤ LIỆU KHÔNG HỢP LỆ!");
          }

          Accessory accessory;
          if (item.IsRestored)
          {
            RestoredItem restoredItem = db.RestoredItems.Find(item.ItemPK);
            accessory = db.Accessories.Find(restoredItem.AccessoryPK);
          }
          else
          {
            PassedItem passedItem = db.PassedItems.Find(item.ItemPK);
            ClassifiedItem classifiedItem = db.ClassifiedItems.Find(passedItem.ClassifiedItemPK);
            PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
            OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
            accessory = db.Accessories.Find(orderedItem.AccessoryPK);
          }
          // Tạo out entry
          Entry entry = new Entry(sBoxFrom, "Out", transferringSession.TransferingSessionPK,
          item.IsRestored, item.TransferQuantity, item.ItemPK, accessory);
          db.Entries.Add(entry);
          // Tạo in entry
          entry = new Entry(sBoxTo, "In", transferringSession.TransferingSessionPK,
          item.IsRestored, item.TransferQuantity, item.ItemPK, accessory);
          db.Entries.Add(entry);
        }
        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }

    }

    public void DeleteTransferingSession(int transferingSessionPK)
    {
      try
      {
        TransferringSession transferringSession = db.TransferringSessions.Find(transferingSessionPK);
        db.TransferringSessions.Remove(transferringSession);
        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public MovingSession CreateMovingSession(StoredBox storedBox, Shelf shelf, string userID)
    {
      try
      {
        MovingSession movingSession = new MovingSession(storedBox, shelf, userID);
        db.MovingSessions.Add(movingSession);
        db.SaveChanges();
        movingSession = (from Mss in db.MovingSessions.OrderByDescending(unit => unit.MovingSessionPK)
                         select Mss).FirstOrDefault();
        return movingSession;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public void UpdateStoredBoxShelfPK(int storedBoxPK, int shelfPK)
    {
      try
      {
        StoredBox storedBox = db.StoredBoxes.Find(storedBoxPK);
        storedBox.ShelfPK = shelfPK;
        db.Entry(storedBox).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public void DeleteMovingSession(int movingSessionPK)
    {
      try
      {
        MovingSession movingSession = db.MovingSessions.Find(movingSessionPK);
        db.MovingSessions.Remove(movingSession);
        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public AdjustingSession CreateAdjustingSession(string comment, bool isVerified, string userID)
    {
      try
      {
        AdjustingSession adjustingSession = new AdjustingSession(comment, isVerified, userID);
        db.AdjustingSessions.Add(adjustingSession);
        db.SaveChanges();
        adjustingSession = (from Ass in db.AdjustingSessions.OrderByDescending(unit => unit.AdjustingSessionPK)
                            select Ass).FirstOrDefault();
        return adjustingSession;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public void CreateAdjustEntry(StoredBox sBox, int itemPK, double adjustedQuantity, bool isRestored, bool isMinus, AdjustingSession adjustingSession)
    {
      try
      {
        Entry entry;
        Accessory accessory;
        if (isRestored)
        {
          RestoredItem restoredItem = db.RestoredItems.Find(itemPK);
          accessory = db.Accessories.Find(restoredItem.AccessoryPK);
        }
        else
        {
          PassedItem passedItem = db.PassedItems.Find(itemPK);
          ClassifiedItem classifiedItem = db.ClassifiedItems.Find(passedItem.ClassifiedItemPK);
          PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
          OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
          accessory = db.Accessories.Find(orderedItem.AccessoryPK);
        }
        if (isMinus)
        {
          entry = new Entry(sBox, "AdjustingMinus", adjustingSession.AdjustingSessionPK, isRestored, adjustedQuantity, itemPK, accessory);
        }
        else
        {
          entry = new Entry(sBox, "AdjustingPlus", adjustingSession.AdjustingSessionPK, isRestored, adjustedQuantity, itemPK, accessory);
        }
        db.Entries.Add(entry);
        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public void DeleteAdjustingSession(int adjustingSessionPK)
    {
      try
      {
        AdjustingSession adjustingSession = db.AdjustingSessions.Find(adjustingSessionPK);
        db.AdjustingSessions.Remove(adjustingSession);
        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public void UpdateAdjustingSession(int adjustingSessionPK, bool isVerified)
    {
      try
      {
        AdjustingSession adjustingSession = db.AdjustingSessions.Find(adjustingSessionPK);
        adjustingSession.IsVerified = isVerified;
        db.Entry(adjustingSession).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public Verification CreateVerification(int sessionPK, string userID, bool isApproved, bool isDiscard)
    {
      try
      {
        Verification verification = new Verification(isApproved, userID, isDiscard, sessionPK);
        db.Verifications.Add(verification);
        db.SaveChanges();
        verification = (from ver in db.Verifications.OrderByDescending(unit => unit.VerificationPK)
                        select ver).FirstOrDefault();
        return verification;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public DiscardingSession CreateDiscardingSession(string comment, bool isVerified, string userID)
    {
      try
      {
        DiscardingSession discardingSession = new DiscardingSession(comment, isVerified, userID);
        db.DiscardingSessions.Add(discardingSession);
        db.SaveChanges();
        discardingSession = (from Ass in db.DiscardingSessions.OrderByDescending(unit => unit.DiscardingSessionPK)
                             select Ass).FirstOrDefault();
        return discardingSession;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public void CreateDiscardEntry(StoredBox sBox, int itemPK, double discardedQuantity, bool isRestored, DiscardingSession discardingSession)
    {
      try
      {
        Entry entry;
        Accessory accessory;
        if (isRestored)
        {
          RestoredItem restoredItem = db.RestoredItems.Find(itemPK);
          accessory = db.Accessories.Find(restoredItem.AccessoryPK);
        }
        else
        {
          PassedItem passedItem = db.PassedItems.Find(itemPK);
          ClassifiedItem classifiedItem = db.ClassifiedItems.Find(passedItem.ClassifiedItemPK);
          PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
          OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
          accessory = db.Accessories.Find(orderedItem.AccessoryPK);
        }
        entry = new Entry(sBox, "Discarding", discardingSession.DiscardingSessionPK, isRestored, discardedQuantity, itemPK, accessory);
        db.Entries.Add(entry);
        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public void DeleteDiscardingSession(int discardingSessionPK)
    {
      try
      {
        DiscardingSession discardingSession = db.DiscardingSessions.Find(discardingSessionPK);
        db.DiscardingSessions.Remove(discardingSession);
        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public void UpdateDiscardingSession(int discardingSessionPK, bool isVerified)
    {
      try
      {
        DiscardingSession discardingSession = db.DiscardingSessions.Find(discardingSessionPK);
        discardingSession.IsVerified = isVerified;
        db.Entry(discardingSession).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    private bool IsDemandedItemsContainAccessory(List<DemandedItem> demandedItems, Accessory accessory)
    {
      foreach (var item in demandedItems)
      {
        if (item.AccessoryPK == accessory.AccessoryPK) return true;
      }
      return false;
    }
  }
}

