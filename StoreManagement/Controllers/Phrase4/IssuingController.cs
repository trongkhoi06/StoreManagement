using StoreManagement.Class;
using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace StoreManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IssuingController : ApiController
    {
        private UserModel db = new UserModel();

        [Route("api/IssuingController/CreateDemand")]
        [HttpPost]
        public IHttpActionResult CreateDemand(int customerPK, int conceptionPK, int totalDemand, int workplacePK, string userID, [FromBody] List<Client_Accessory_DemandedQuantity_Comment> list)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                Demand demand = null;
                try
                {
                    // kiểm khi chạy lệnh
                    if (!PrimitiveType.isValidIntegerQuantity(totalDemand))
                    {
                        return Content(HttpStatusCode.Conflict, SystemMessage.NotPassPrimitiveType);
                    }
                    Conception conception = db.Conceptions.Find(conceptionPK);
                    if (conception.CustomerPK != customerPK)
                    {
                        return Content(HttpStatusCode.Conflict, "KHÔNG ĐÚNG KHÁCH HÀNG");
                    }
                    demand = issuingDAO.CreateDemand(conception.ConceptionPK, totalDemand, workplacePK, userID);
                    issuingDAO.CreateDemandedItems(demand, list, conception.ConceptionPK);
                }
                catch (Exception e)
                {
                    if (demand != null)
                    {
                        issuingDAO.DeleteDemand(demand.DemandPK);
                    }
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "TẠO YÊU CẦU XUẤT THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/IssuingController/EditDemand")]
        [HttpPut]
        public IHttpActionResult EditDemand(int demandPK, int demandedItemPK, double demandedQuantity, string comment, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                Demand demand = null;
                try
                {
                    demand = db.Demands.Find(demandPK);
                    if (demand.UserID != userID)
                    {
                        return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
                    }
                    if (issuingDAO.GetIssueFromDemandPKNotStorebacked(demandPK).Count > 0)
                    {
                        return Content(HttpStatusCode.Conflict, "DEMAND ĐÃ CÓ PHIẾU XUẤT!");
                    }
                    issuingDAO.UpdateDemandedItem(demandedItemPK, demandedQuantity, comment);
                    issuingDAO.UpdateDemand(demandPK);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "TẠO YÊU CẦU XUẤT THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/IssuingController/DeleteDemand")]
        [HttpDelete]
        public IHttpActionResult DeleteDemand(int demandPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                Demand demand = null;
                try
                {
                    demand = db.Demands.Find(demandPK);
                    if (demand.UserID != userID)
                    {
                        return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
                    }
                    if (issuingDAO.GetIssueFromDemandPKNotStorebacked(demandPK).Count > 0)
                    {
                        return Content(HttpStatusCode.Conflict, "DEMAND ĐÃ CÓ PHIẾU XUẤT!");
                    }
                    issuingDAO.DeleteDemandedItem(demandPK);
                    issuingDAO.DeleteDemand(demandPK);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "TẠO YÊU CẦU XUẤT THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/IssuingController/SwiftDemandState")]
        [HttpPut]
        public IHttpActionResult SwiftDemandState(int demandPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                try
                {
                    if (issuingDAO.GetIssueFromDemandPKNotStorebacked(demandPK).Count == 0)
                    {
                        return Content(HttpStatusCode.Conflict, "DEMAND CHƯA CÓ PHIẾU XUẤT, KHÔNG THỂ ĐÓNG!");
                    }
                    issuingDAO.SwiftDemand(demandPK);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "TẠO YÊU CẦU XUẤT THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/IssuingController/GetDemandsPrepared")]
        [HttpGet]
        public IHttpActionResult GetDemandsPrepared()
        {
            try
            {
                List<Client_Demand_IssueItems> result = new List<Client_Demand_IssueItems>();
                List<Demand> demands = db.Demands.Where(unit => unit.IsOpened).ToList();
                foreach (var demand in demands)
                {
                    result.Add(new Client_Demand_IssueItems(demand, db.Conceptions.Find(demand.ConceptionPK).ConceptionCode));
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/IssuingController/GetDemandedItemsByDemandPrepared")]
        [HttpGet]
        public IHttpActionResult GetDemandedItemsByDemandPrepared(int demandPK)
        {
            List<Client_DemandedItem> result = new List<Client_DemandedItem>();
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                Demand demand = db.Demands.Find(demandPK);
                if (demand == null) return Content(HttpStatusCode.Conflict, "ĐƠN CẤP PHÁT KHÔNG TỒN TẠI!");
                List<DemandedItem> demandedItems = db.DemandedItems.Where(unit => unit.DemandPK == demandPK).ToList();
                foreach (var item in demandedItems)
                {
                    Accessory accessory = db.Accessories.Find(item.AccessoryPK);
                    AccessoryType accessoryType = db.AccessoryTypes.Find(accessory.AccessoryTypePK);
                    List<Client_Box_Shelf_Row> client_Boxes = issuingDAO.StoredBox_ItemPK_IsRestoredOfEntries(accessory);
                    result.Add(new Client_DemandedItem(item, accessory, issuingDAO.IssuedQuantity(item.DemandedItemPK), client_Boxes, accessoryType.Name));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class StoredItemForIssue
        {
            public StoredItemForIssue(int itemPK, bool isRestored, double issuedQuantity, string oldBoxID, string newBoxID)
            {
                ItemPK = itemPK;
                IsRestored = isRestored;
                IssuedQuantity = issuedQuantity;
                OldBoxID = oldBoxID;
                NewBoxID = newBoxID;
            }

            public int ItemPK { get; set; }

            public bool IsRestored { get; set; }

            public double IssuedQuantity { get; set; }

            public int AccessoryPK { get; set; }

            public string OldBoxID { get; set; }

            public string NewBoxID { get; set; }
        }

        [Route("api/IssuingController/GetIsBoxEmpty")]
        [HttpGet]
        public IHttpActionResult GetIsBoxEmpty(string boxID)
        {
            try
            {
                bool result = false;
                BoxDAO boxDAO = new BoxDAO();
                Box box = boxDAO.GetBoxByBoxID(boxID);
                if (boxDAO.IsEmptyCase(box.BoxPK))
                {
                    result = true;
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/IssuingController/CollectStoredItemForIssue")]
        [HttpPost]
        public IHttpActionResult CollectStoredItemForIssue(int demandPK, string userID, List<StoredItemForIssue> input)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                Issue issue = null;
                IssuingDAO issuingDAO = new IssuingDAO();
                try
                {
                    Demand demand = db.Demands.Find(demandPK);
                    if (demand == null || demand.IsOpened == false)
                    {
                        return Content(HttpStatusCode.Conflict, "ĐƠN CẤP PHÁP KHÔNG HỢP LỆ!");
                    }
                    issue = issuingDAO.CreateIssue(userID, demandPK);
                    issuingDAO.CreateEntryAndIssuedGroup(input, demandPK, issue);
                    return Content(HttpStatusCode.OK, "THU THẬP ĐỒ ĐỂ XUẤT THÀNH CÔNG!");
                }
                catch (Exception e)
                {
                    if (issue != null)
                    {
                        issuingDAO.DeleteIssue(issue.IssuePK);
                    }
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        public class Client_Issue1
        {
            public Client_Issue1(Issue issue, Demand demand, Workplace workplace)
            {
                IssuePK = issue.IssuePK;
                IssueID = issue.IssueID;
                ExecutedDate = issue.ExecutedDate;
                TotalDemand = demand.TotalDemand;
                WorkplaceID = workplace.WorkplaceID;
            }

            public int IssuePK { get; set; }

            public string IssueID { get; set; }

            public DateTime ExecutedDate { get; set; }

            public double TotalDemand { get; set; }

            public string WorkplaceID { get; set; }
        }

        [Route("api/IssuingController/GetIssueByUserID")]
        [HttpGet]
        public IHttpActionResult GetIssueByUserID(string userID)
        {
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                List<Client_Issue1> result = new List<Client_Issue1>();

                List<Issue> issues = db.Issues.Where(unit => unit.UserID == userID).ToList();
                foreach (var issue in issues)
                {
                    Demand demand = db.Demands.Find(issue.DemandPK);
                    Workplace workplace = db.Workplaces.Find(demand.WorkplacePK);
                    result.Add(new Client_Issue1(issue, demand, workplace));
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_IssuedGroup
        {
            public Client_IssuedGroup(IssuedGroup issuedGroup, Box box, Accessory accessory, string typeName)
            {
                GroupQuantity = issuedGroup.IssuedGroupQuantity;
                BoxID = box.BoxID;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                TypeName = typeName;
            }

            public double GroupQuantity { get; set; }

            public string BoxID { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public string TypeName { get; set; }
        }

        public class Client_IssuedItem : IEquatable<Client_IssuedItem>
        {
            public Client_IssuedItem(DemandedItem demandedItem, Accessory accessory, string typeName)
            {
                DemandedQuantity = demandedItem.DemandedQuantity;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                TypeName = typeName;
            }

            public double DemandedQuantity { get; set; }

            public double SumIssuedGroupQuantity { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public string TypeName { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as Client_IssuedItem);
            }

            public bool Equals(Client_IssuedItem other)
            {
                return other != null &&
                       AccessoryID == other.AccessoryID;
            }

            public override int GetHashCode()
            {
                return -885560036 + EqualityComparer<string>.Default.GetHashCode(AccessoryID);
            }
        }

        public class Client_IssuedGroups_IssuedItems
        {
            public Client_IssuedGroups_IssuedItems(List<Client_IssuedGroup> issuedGroups, List<Client_IssuedItem> issuedItems)
            {
                IssuedGroups = issuedGroups;
                IssuedItems = issuedItems;
            }

            public List<Client_IssuedGroup> IssuedGroups { get; set; }

            public List<Client_IssuedItem> IssuedItems { get; set; }
        }


        [Route("api/IssuingController/GetIssuedItemsAndGroupByIssue")]
        [HttpGet]
        public IHttpActionResult GetIssuedItemsAndGroupByIssue(int issuePK)
        {
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                Client_IssuedGroups_IssuedItems result;
                List<Client_IssuedItem> client_IssuedItems = new List<Client_IssuedItem>();
                List<Client_IssuedGroup> client_IssuedGroups = new List<Client_IssuedGroup>();
                Issue issue = db.Issues.Find(issuePK);
                List<IssuedGroup> issuedGroups = db.IssuedGroups.Where(unit => unit.IssuePK == issue.IssuePK).ToList();
                foreach (var issuedGroup in issuedGroups)
                {
                    Box box = db.Boxes.Find(db.UnstoredBoxes.Find(issuedGroup.UnstoredBoxPK).BoxPK);
                    Accessory accessory = db.Accessories.Find(issuedGroup.AccessoryPK);
                    AccessoryType accessoryType = db.AccessoryTypes.Find(accessory.AccessoryTypePK);
                    DemandedItem demandedItem = db.DemandedItems.Find(issuedGroup.DemandedItemPK);

                    client_IssuedGroups.Add(new Client_IssuedGroup(issuedGroup, box, accessory, accessoryType.Name));

                    Client_IssuedItem client_IssuedItem = new Client_IssuedItem(demandedItem, accessory, accessoryType.Name);
                    if (!client_IssuedItems.Contains(client_IssuedItem))
                    {
                        client_IssuedItem.SumIssuedGroupQuantity = issuedGroup.IssuedGroupQuantity;
                        client_IssuedItems.Add(client_IssuedItem);
                    }
                    else
                    {
                        client_IssuedItems.Find(unit => unit == client_IssuedItem).SumIssuedGroupQuantity += client_IssuedItem.SumIssuedGroupQuantity;
                    }
                }

                result = new Client_IssuedGroups_IssuedItems(client_IssuedGroups, client_IssuedItems);
                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_Box_Shelf_Storeback
        {
            public string BoxID { get; set; }

            public string ShelfID { get; set; }
        }

        [Route("api/IssuingController/StorebackIssue")]
        [HttpPost]
        public IHttpActionResult StorebackIssue(int issuePK, string userID, List<Client_Box_Shelf_Storeback> input)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                try
                {
                    Issue issue = db.Issues.Find(issuePK);
                    if (issue.UserID != userID)
                    {
                        return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
                    }
                    if (issue.IsConfirmed == true || issue.IsStorebacked == true)
                    {
                        return Content(HttpStatusCode.Conflict, "PHIẾU CẤP PHÁT KHÔNG HỢP LỆ!");
                    }
                    StorebackSession storebackSession = issuingDAO.CreateStorebackSession(issuePK, userID);
                    issuingDAO.CreateEntryAndUpdateIssueThings(issuePK, storebackSession, input);
                    return Content(HttpStatusCode.OK, "NHẬP KHO THÀNH CÔNG!");
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/IssuingController/GetIssueByUserWorkplace")]
        [HttpGet]
        public IHttpActionResult GetIssueByUserWorkplace(string userID)
        {
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                List<Client_Issue1> result = new List<Client_Issue1>();
                SystemUser systemUser = db.SystemUsers.Find(userID);
                List<Issue> issues = db.Issues.Where(unit => unit.UserID == userID).ToList();
                foreach (var issue in issues)
                {
                    Demand demand = db.Demands.Find(issue.DemandPK);
                    Workplace workplace = db.Workplaces.Find(demand.WorkplacePK);
                    if (workplace.WorkplacePK == systemUser.WorkplacePK)
                    {
                        result.Add(new Client_Issue1(issue, demand, workplace));
                    }
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/IssuingController/ConfirmReceivingIssue")]
        [HttpPost]
        public IHttpActionResult ConfirmReceivingIssue(int issuePK, string userID, List<string> takenAwayBoxIDs)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Receiver"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                try
                {
                    SystemUser systemUser = db.SystemUsers.Find(userID);
                    Issue issue = db.Issues.Find(issuePK);
                    Demand demand = db.Demands.Find(issue.DemandPK);
                    if (demand.WorkplacePK != systemUser.WorkplacePK)
                    {
                        return Content(HttpStatusCode.Conflict, "BẠN KHÔNG ĐẾN TỪ ĐÚNG NƠI LÀM VIỆC CỦA ĐƠN XUẤT!");
                    }
                    if (issue.IsConfirmed == true || issue.IsStorebacked == true)
                    {
                        return Content(HttpStatusCode.Conflict, "PHIẾU XUẤT KHÔNG HỢP LỆ!");
                    }

                    issuingDAO.CreateConfirmingSessionAndUpdateIssueThings(userID, issuePK, takenAwayBoxIDs);

                    return Content(HttpStatusCode.OK, "NHÂN PHIẾU XUẤT THÀNH CÔNG!");
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/IssuingController/GetAllDemand")]
        [HttpGet]
        public IHttpActionResult GetAllDemand()
        {
            List<Client_Demand> client_Demands = new List<Client_Demand>();
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                List<Demand> demands = (from d in db.Demands.OrderByDescending(unit => unit.DemandPK)
                                        where d.IsOpened == true
                                        select d).ToList();
                foreach (var demand in demands)
                {
                    Conception conception = db.Conceptions.Find(demand.ConceptionPK);
                    client_Demands.Add(new Client_Demand(demand, conception));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, client_Demands);
        }

        //[Route("api/IssuingController/GetDemandAndDemandedItemsByDemandPK")]
        //[HttpGet]
        //public IHttpActionResult GetDemandAndDemandedItemsByDemandPK(int demandPK)
        //{
        //    List<Client_DemandDetail> client_Demands = new List<Client_DemandDetail>();
        //    IssuingDAO issuingDAO = new IssuingDAO();
        //    try
        //    {
        //        Demand demand = db.Demands.Find(demandPK);
        //        List<DemandedItem> demandedItems = (from dI in db.DemandedItems
        //                                            where dI.DemandPK == demandPK
        //                                            select dI).ToList();
        //        foreach (var demandedItem in demandedItems)
        //        {
        //            Accessory accessory = db.Accessories.Find(demandedItem.AccessoryPK);
        //            List<RequestedItem> requestedItems = (from rI in db.RequestedItems
        //                                                  where rI.DemandedItemPK == demandedItem.DemandedItemPK
        //                                                  select rI).ToList();
        //            client_Demands.Add(new Client_DemandDetail(demandedItem, accessory,
        //                issuingDAO.TotalRequestedQuantity(requestedItems),
        //                //issuingDAO.TotalRequestedQuantityConfirmed(requestedItems),
        //                issuingDAO.InStoredQuantity(accessory.AccessoryPK) - issuingDAO.InRequestedQuantity(accessory.AccessoryPK)));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
        //    }
        //    return Content(HttpStatusCode.OK, client_Demands);
        //}

        public class Algo_itempk_isRestored : IEquatable<Algo_itempk_isRestored>
        {
            public Algo_itempk_isRestored()
            {
            }

            public Algo_itempk_isRestored(int itemPK, bool isRestored)
            {
                ItemPK = itemPK;
                IsRestored = isRestored;
            }

            public int ItemPK { get; set; }

            public bool IsRestored { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as Algo_itempk_isRestored);
            }

            public bool Equals(Algo_itempk_isRestored other)
            {
                return other != null &&
                       ItemPK == other.ItemPK &&
                       IsRestored == other.IsRestored;
            }

            public override int GetHashCode()
            {
                var hashCode = -1201230142;
                hashCode = hashCode * -1521134295 + ItemPK.GetHashCode();
                hashCode = hashCode * -1521134295 + IsRestored.GetHashCode();
                return hashCode;
            }
        }

        //[Route("api/IssuingController/GetEmptyBoxIDsPrepared")]
        //[HttpPost]
        //public IHttpActionResult GetEmptyBoxIDsPrepared([FromBody] List<Client_ItemPK_IsRestored_StoredBoxPK_IssuedQuantity> list)
        //{
        //    List<string> boxIDs = new List<string>();
        //    IssuingDAO issuingDAO = new IssuingDAO();
        //    StoringDAO storingDAO = new StoringDAO();
        //    try
        //    {
        //        // tính ra số lượng xuất các item theo itempk,isrestored đc input gửi xuống
        //        Dictionary<int, Dictionary<Algo_itempk_isRestored, double>> Map = new Dictionary<int, Dictionary<Algo_itempk_isRestored, double>>();
        //        foreach (var items in list)
        //        {
        //            foreach (var item in items.BoxAndQuantity)
        //            {
        //                if (!Map.ContainsKey(item.StoredBoxPK))
        //                {
        //                    Dictionary<Algo_itempk_isRestored, double> item_isRestored_quantity = new Dictionary<Algo_itempk_isRestored, double>
        //                    {
        //                        { new Algo_itempk_isRestored(items.ItemPK, items.IsRestored), item.Quantity }
        //                    };

        //                    Map.Add(item.StoredBoxPK, item_isRestored_quantity);
        //                }
        //                else
        //                {
        //                    Algo_itempk_isRestored temp = new Algo_itempk_isRestored(items.ItemPK, items.IsRestored);
        //                    if (!Map[item.StoredBoxPK].ContainsKey(temp))
        //                    {
        //                        Map[item.StoredBoxPK].Add(temp, item.Quantity);
        //                    }
        //                    else
        //                    {
        //                        Map[item.StoredBoxPK][temp] += item.Quantity;
        //                    }
        //                }
        //            }
        //        }
        //        // kiểm lại hàng trong kho
        //        foreach (var items in Map)
        //        {
        //            bool IsEmpty = true;
        //            StoredBox storedBox = db.StoredBoxes.Find(items.Key);
        //            Box box = db.Boxes.Find(storedBox.BoxPK);

        //            HashSet<Algo_itempk_isRestored> tempHSAll = new HashSet<Algo_itempk_isRestored>();
        //            HashSet<Algo_itempk_isRestored> tempHSTaking = new HashSet<Algo_itempk_isRestored>();
        //            // lấy hết item trong storebox ra
        //            List<Entry> tempEntries = (from e in db.Entries
        //                                       where e.StoredBoxPK == items.Key
        //                                       select e).ToList();
        //            foreach (var entry in tempEntries)
        //            {
        //                tempHSAll.Add(new Algo_itempk_isRestored(entry.ItemPK, entry.IsRestored));
        //            }

        //            foreach (var item in items.Value)
        //            {
        //                // lấy hết item trong lượt xuất
        //                tempHSTaking.Add(new Algo_itempk_isRestored(item.Key.ItemPK, item.Key.IsRestored));
        //                List<Entry> entries = (from e in db.Entries
        //                                       where e.StoredBoxPK == items.Key && e.ItemPK == item.Key.ItemPK && e.IsRestored == item.Key.IsRestored
        //                                       select e).ToList();
        //                double tempQuantity = storingDAO.AvailableQuantity(storedBox, item.Key.ItemPK, item.Key.IsRestored);
        //                if (item.Value > tempQuantity) throw new Exception("SỐ LƯỢNG XUẤT VƯỢT QUÁ HÀNG TRONG KHO");
        //                // nếu số lượng xuất nhỏ hơn số lượng trong box thì box còn hàng
        //                if (item.Value < tempQuantity) IsEmpty = false;

        //            }
        //            // nếu item trong store box nhiều hơn lượt xuất thì box còn hàng
        //            if (tempHSAll.Count > tempHSTaking.Count) IsEmpty = false;
        //            if (IsEmpty)
        //            {
        //                boxIDs.Add(box.BoxID);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
        //    }
        //    return Content(HttpStatusCode.OK, boxIDs);
        //}

        //[Route("api/IssuingController/PrepareRequest")]
        //[HttpPost]
        //public IHttpActionResult PrepareRequest(int requestPK, string userID, [FromBody] Client_InputPrepareRequestAPI input)
        //{
        //    if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
        //    {
        //        IssuingDAO issuingDAO = new IssuingDAO();
        //        StoringDAO storingDAO = new StoringDAO();
        //        BoxDAO boxDAO = new BoxDAO();
        //        IssuingSession issuingSession = null;
        //        try
        //        {
        //            Request request = db.Requests.Find(requestPK);
        //            if (request.IsIssued)
        //            {
        //                return Content(HttpStatusCode.Conflict, "YÊU CẦU NHẬN ĐÃ ĐƯỢC PHÁT RỒI, KHÔNG THỂ PHÁT NỮA");
        //            }
        //            issuingSession = issuingDAO.CreateIssuingSession(userID, requestPK, input.boxIDs);
        //            storingDAO.CreateIssueEntry(input, issuingSession, request);
        //            boxDAO.ChangeIsActiveBoxes(input.boxIDs, false);
        //            issuingDAO.UpdateRequestIsIssued(requestPK, true);
        //        }
        //        catch (Exception e)
        //        {
        //            if (issuingSession != null)
        //            {
        //                issuingDAO.DeleteIssuingSession(issuingSession.IssuingSessionPK);
        //            }
        //            issuingDAO.UpdateRequestIsIssued(requestPK, false);
        //            boxDAO.ChangeIsActiveBoxes(input.boxIDs, true);
        //            return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
        //        }
        //        return Content(HttpStatusCode.OK, "TẠO YÊU CẦU XUẤT THÀNH CÔNG!");
        //    }
        //    else
        //    {
        //        return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
        //    }
        //}

        [Route("api/IssuingController/GetCustomerAndConceptionForRestoreItem")]
        [HttpGet]
        public IHttpActionResult GetCustomerAndConceptionForRestoreItem()
        {
            List<Client_Customer_Conception> result = new List<Client_Customer_Conception>();
            IssuingDAO issuingDAO = new IssuingDAO();
            BoxDAO boxDAO = new BoxDAO();
            try
            {
                List<Customer> customers = db.Customers.ToList();
                foreach (var customer in customers)
                {
                    List<Conception> conceptions = (from cc in db.Conceptions
                                                    where cc.CustomerPK == customer.CustomerPK
                                                    select cc).ToList();
                    result.Add(new Client_Customer_Conception(customer.CustomerName, conceptions));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/IssuingController/GetAllAccessoryType")]
        [HttpGet]
        public IHttpActionResult GetAllAccessoryType()
        {
            List<AccessoryType> result;
            try
            {
                result = db.AccessoryTypes.ToList();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class Accessory_RestoreItem
        {
            public Accessory_RestoreItem()
            {
            }

            public Accessory_RestoreItem(Accessory accessory, string typeName)
            {
                AccessoryPK = accessory.AccessoryPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                AccessoryTypePK = accessory.AccessoryTypePK;
                TypeName = typeName;
            }

            public int AccessoryPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public int AccessoryTypePK { get; set; }

            public string TypeName { get; set; }
        }

        public class Client_ConceptionsAndAccessoryTypes
        {
            public List<int> conceptionPKs { get; set; }

            public List<int> accessorytypePKs { get; set; }
        }

        [Route("api/IssuingController/GetAccessoriesForRestoreItem")]
        [HttpPost]
        public IHttpActionResult GetAccessoriesForRestoreItem(List<int> conceptionPKs)
        {
            List<Accessory_RestoreItem> result = new List<Accessory_RestoreItem>();
            try
            {
                if (conceptionPKs.Count == 0)
                {
                    List<Accessory> accessories = db.Accessories.ToList();
                    foreach (var item in accessories)
                    {
                        AccessoryType accessoryType = db.AccessoryTypes.Find(item.AccessoryTypePK);
                        result.Add(new Accessory_RestoreItem(item, accessoryType.Name));
                    }
                }
                foreach (var conceptionPK in conceptionPKs)
                {
                    List<int> tempAccessoriesPK = (from unit in db.ConceptionAccessories
                                                   where unit.ConceptionPK == conceptionPK
                                                   select unit.AccessoryPK).ToList();
                    foreach (var AccessoryPK in tempAccessoriesPK)
                    {
                        Accessory tempAccessory = db.Accessories.Find(AccessoryPK);
                        AccessoryType accessoryType = db.AccessoryTypes.Find(tempAccessory.AccessoryTypePK);
                        result.Add(new Accessory_RestoreItem(tempAccessory, accessoryType.Name));
                    }
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class Client_AccessoryPK_RestoredQuantity
        {
            public Client_AccessoryPK_RestoredQuantity()
            {
            }

            public Client_AccessoryPK_RestoredQuantity(int assessoryPK, double restoredQuantity)
            {
                AssessoryPK = assessoryPK;
                RestoredQuantity = restoredQuantity;
            }

            public int AssessoryPK { get; set; }

            public double RestoredQuantity { get; set; }
        }

        [Route("api/IssuingController/CreateRestoration")]
        [HttpPost]
        public IHttpActionResult CreateRestoration(string userID, string comment, List<Client_AccessoryPK_RestoredQuantity> list)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Receiver"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                Restoration restoration = null;
                try
                {
                    restoration = issuingDAO.CreateRestoration(userID, comment);
                    issuingDAO.CreateRestoredItems(restoration, list);
                }
                catch (Exception e)
                {
                    if (restoration != null)
                    {
                        issuingDAO.DeleteRestoration(restoration.RestorationPK);
                    }
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "TRẢ HÀNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/IssuingController/GetRestorationByUserID")]
        [HttpGet]
        public IHttpActionResult GetRestorationByUserID(string userID)
        {
            try
            {
                List<Restoration> result = new List<Restoration>();

                result = db.Restorations.ToList();

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_RestoredItem
        {
            public Client_RestoredItem()
            {
            }

            public Client_RestoredItem(Accessory accessory, RestoredItem restoredItem)
            {
                RestoredItemPK = restoredItem.RestoredItemPK;
                RestoredQuantity = restoredItem.RestoredQuantity;
                AccessoryPK = accessory.AccessoryPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
            }

            public int RestoredItemPK { get; set; }

            public double RestoredQuantity { get; set; }

            public int AccessoryPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }
        }

        //[Route("api/IssuingController/GetRestorationByUserID")]
        //[HttpGet]
        //public IHttpActionResult GetRestoredItemByRestoration(int restorationPK)
        //{
        //    try
        //    {
        //        List<Client_RestoredItem> result = new List<Client_RestoredItem>();
        //        Restoration restoration = db.Restorations.Find(restorationPK);
        //        List<RestoredItem> restoredItems = db.RestoredItems.Where(unit => unit.RestorationPK);
        //        result = db.Restorations.ToList();

        //        return Content(HttpStatusCode.OK, result);
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
        //    }
        //}

        public class Client_RestoredItemPK_RestoredQuantity
        {
            public Client_RestoredItemPK_RestoredQuantity()
            {
            }

            public Client_RestoredItemPK_RestoredQuantity(int restoredItemPK, double restoredQuantity)
            {
                RestoredItemPK = restoredItemPK;
                RestoredQuantity = restoredQuantity;
            }

            public int RestoredItemPK { get; set; }

            public double RestoredQuantity { get; set; }
        }

        [Route("api/IssuingController/EditRestoration")]
        [HttpPut]
        public IHttpActionResult EditRestoration(int restorationPK, string userID, string comment, List<Client_RestoredItemPK_RestoredQuantity> list)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Receiver"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                Restoration restoration = null;
                try
                {
                    restoration = db.Restorations.Find(restorationPK);
                    if (restoration == null) return Content(HttpStatusCode.Conflict, "MÃ PHIẾU TRẢ HÀNG KHÔNG HỢP LỆ!");
                    if (restoration.IsReceived) return Content(HttpStatusCode.Conflict, "PHIẾU TRẢ ĐÃ ĐƯỢC NHẬN, KHÔNG THỂ THAY ĐỔI!");
                    if (restoration.UserID != userID) return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
                    issuingDAO.UpdateRestoration(restorationPK, comment);
                    issuingDAO.UpdateRestoredItems(list);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "CHỈNH SỬA HÀNG TRẢ THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/IssuingController/DeleteRestoration")]
        [HttpDelete]
        public IHttpActionResult DeleteRestoration(int restorationPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Receiver"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                Restoration restoration = null;
                try
                {
                    restoration = db.Restorations.Find(restorationPK);
                    if (restoration == null) return Content(HttpStatusCode.Conflict, "MÃ PHIẾU TRẢ HÀNG KHÔNG HỢP LỆ!");
                    if (restoration.IsReceived) return Content(HttpStatusCode.Conflict, "PHIẾU TRẢ ĐÃ ĐƯỢC NHẬN, KHÔNG THỂ THAY ĐỔI!");
                    if (restoration.UserID != userID) return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
                    issuingDAO.DeleteRestoredItems(restorationPK);
                    issuingDAO.DeleteRestoration(restorationPK);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "XÓA HÀNG TRẢ THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        public class Client_Box_List
        {

            public class Client_RestoredItemPK_PlacedQuantity
            {
                public int RestoredItemPK { get; set; }

                public double PlacedQuantity { get; set; }
            }

            public string BoxID { get; set; }

            public List<Client_RestoredItemPK_PlacedQuantity> ListItem { get; set; }
        }

        [Route("api/IssuingController/ReceiveRestoredItem")]
        [HttpPost]
        public IHttpActionResult ReceiveRestoredItem(int restorationPK, List<Client_Box_List> list, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Receiver"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                ReceivingSession receivingSession;
                Restoration restoration = null;
                try
                {
                    restoration = db.Restorations.Find(restorationPK);
                    if (restoration == null) return Content(HttpStatusCode.Conflict, "MÃ PHIẾU TRẢ HÀNG KHÔNG HỢP LỆ!");
                    if (restoration.IsReceived) return Content(HttpStatusCode.Conflict, "PHIẾU TRẢ ĐÃ ĐƯỢC NHẬN, KHÔNG THỂ THAY ĐỔI!");
                    receivingSession = issuingDAO.CreateReceivingSession(restorationPK, userID);
                    issuingDAO.UpdateRestoration(restorationPK, true);
                    issuingDAO.CreateEntryReceiving(list, receivingSession);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "NHẬN HÀNG ĐƯỢC TRẢ THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        public class Client_Demand_IssueItems
        {
            public Client_Demand_IssueItems(Demand demand, string conceptionCode)
            {
                DemandPK = demand.DemandPK;
                DemandID = demand.DemandID;
                TotalDemand = demand.TotalDemand;
                DateCreated = demand.DateCreated;
                ConceptionCode = conceptionCode;
                UserID = demand.UserID;
            }

            public int DemandPK { get; set; }

            public string DemandID { get; set; }

            public double TotalDemand { get; set; }

            public DateTime DateCreated { get; set; }

            public string ConceptionCode { get; set; }

            public string UserID { get; set; }
        }

        //[Route("api/IssuingController/GetDemandsByWorkplaceByUserID")]
        //[HttpGet]
        //public IHttpActionResult GetDemandsByWorkplaceByUserID(string userID)
        //{
        //    try
        //    {
        //        List<Client_Demand_IssueItems> result = new List<Client_Demand_IssueItems>();
        //        Workplace workplace = db.Workplaces.Find(db.SystemUsers.Find(userID).WorkplacePK);
        //        List<Demand> demands = db.Demands.Where(unit => unit.WorkplacePK == workplace.WorkplacePK && unit.IsOpened).ToList();
        //        foreach (var demand in demands)
        //        {
        //            result.Add(new Client_Demand_IssueItems(demand, db.Conceptions.Find(demand.ConceptionPK).ConceptionCode));
        //        }

        //        return Content(HttpStatusCode.OK, result);
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
        //    }
        //}

        public class Client_Restoration
        {
            public Client_Restoration(Restoration restoration, string userName)
            {
                RestorationPK = restoration.RestorationPK;
                RestorationID = restoration.RestorationID;
                DateCreated = restoration.DateCreated;
                IsReceived = restoration.IsReceived;
                UserID = restoration.UserID;
                UserName = userName;
                Comment = restoration.Comment;
            }

            public int RestorationPK { get; set; }

            public string RestorationID { get; set; }

            public DateTime DateCreated { get; set; }

            public bool IsReceived { get; set; }

            public string UserID { get; set; }

            public string UserName { get; set; }

            public string Comment { get; set; }
        }

        [Route("api/IssuingController/GetRestorationNotReceived")]
        [HttpGet]
        public IHttpActionResult GetRestorationNotReceived()
        {
            try
            {
                List<Client_Restoration> result = new List<Client_Restoration>();
                List<Restoration> restorations = db.Restorations.Where(unit => unit.IsReceived == false).ToList();
                foreach (var restoration in restorations)
                {
                    SystemUser systemUser = db.SystemUsers.Find(restoration.UserID);
                    result.Add(new Client_Restoration(restoration, systemUser.Name));
                }
                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_RestoredItem_Identify
        {
            public Client_RestoredItem_Identify(int restoredItemPK, double restoredQuantity, string accessoryID, string accessoryDescription, string item, string art, string color, string typeName)
            {
                RestoredItemPK = restoredItemPK;
                RestoredQuantity = restoredQuantity;
                AccessoryID = accessoryID;
                AccessoryDescription = accessoryDescription;
                Item = item;
                Art = art;
                Color = color;
                TypeName = typeName;
            }

            public int RestoredItemPK { get; set; }

            public double RestoredQuantity { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public string TypeName { get; set; }
        }

        [Route("api/IssuingController/GetRestoredItemByRestoration")]
        [HttpGet]
        public IHttpActionResult GetRestoredItemByRestoration(int restorationPK)
        {
            try
            {
                Restoration restoration = db.Restorations.Find(restorationPK);
                List<Client_RestoredItem_Identify> result = new List<Client_RestoredItem_Identify>();

                List<RestoredItem> restoredItems = db.RestoredItems
                            .Where(unit => unit.RestorationPK == restoration.RestorationPK).ToList();

                foreach (var item in restoredItems)
                {
                    Accessory accessory = db.Accessories.Find(item.AccessoryPK);
                    AccessoryType accessoryType = db.AccessoryTypes.Find(accessory.AccessoryTypePK);
                    result.Add(new Client_RestoredItem_Identify(item.RestoredItemPK, item.RestoredQuantity,
                        accessory.AccessoryID, accessory.AccessoryDescription, accessory.Item, accessory.Art, accessory.Color, accessoryType.Name));
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class RestoredGroupItem
        {
            public RestoredGroupItem(int restoredItemPK, double groupQuantity, int unstoredBoxPK)
            {
                RestoredItemPK = restoredItemPK;
                GroupQuantity = groupQuantity;
                UnstoredBoxPK = unstoredBoxPK;
            }

            public int RestoredItemPK { get; set; }

            public double GroupQuantity { get; set; }

            public int UnstoredBoxPK { get; set; }
        }

        [Route("api/IssuingController/IdentifyRestoredGroupItems")]
        [HttpPost]
        public IHttpActionResult IdentifyRestoredGroupItems(int restorationPK, string userID, List<RestoredGroupItem> input)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                ReceivingSession receivingSession = null;
                try
                {
                    receivingSession = issuingDAO.CreateReceivingSession(restorationPK, userID);
                    issuingDAO.CreateRestoredGroup(restorationPK, userID, receivingSession, input);

                    return Content(HttpStatusCode.OK, "GHI NHẬN THÀNH CÔNG!");
                }
                catch (Exception e)
                {
                    if (receivingSession != null)
                    {
                        issuingDAO.DeleteReceivingSession(receivingSession.ReceivingSessionPK);
                    }
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
            }
            else
            {

                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }
    }
}
