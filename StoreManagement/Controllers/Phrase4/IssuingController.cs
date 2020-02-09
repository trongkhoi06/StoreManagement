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
                    demand = issuingDAO.CreateDemand(customerPK, conception.ConceptionPK, totalDemand, workplacePK, userID);
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
                    //if (issuingDAO.GetRequestFromDemandPK(demandPK).Count > 0)
                    //{
                    //    return Content(HttpStatusCode.Conflict, "DEMAND ĐÃ CÓ YÊU CẦU XUẤT!");
                    //}
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
                    //if (issuingDAO.GetRequestFromDemandPK(demandPK).Count > 0)
                    //{
                    //    return Content(HttpStatusCode.Conflict, "DEMAND ĐÃ CÓ YÊU CẦU XUẤT!");
                    //}
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

            public Accessory_RestoreItem(Accessory accessory)
            {
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
            }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }
        }

        public class Client_ConceptionsAndAccessoryTypes
        {
            public List<int> conceptionPKs { get; set; }

            public List<int> accessorytypePKs { get; set; }
        }

        [Route("api/IssuingController/GetAccessoriesForRestoreItem")]
        [HttpPost]
        public IHttpActionResult GetAccessoriesForRestoreItem(Client_ConceptionsAndAccessoryTypes input)
        {
            List<Accessory_RestoreItem> result = new List<Accessory_RestoreItem>();
            try
            {
                foreach (var conceptionPK in input.conceptionPKs)
                {
                    List<int> tempAccessoriesPK = (from unit in db.ConceptionAccessories
                                                   where unit.ConceptionPK == conceptionPK
                                                   select unit.AccessoryPK).ToList();
                    foreach (var AccessoryPK in tempAccessoriesPK)
                    {
                        Accessory tempAccessory = db.Accessories.Find(AccessoryPK);

                        if (input.accessorytypePKs.Contains(tempAccessory.AccessoryTypePK))
                            result.Add(new Accessory_RestoreItem(tempAccessory));
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

        [Route("api/IssuingController/RestoreItems")]
        [HttpPost]
        public IHttpActionResult RestoreItems(string userID, string comment, List<Client_AccessoryPK_RestoredQuantity> list)
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

        [Route("api/IssuingController/GetDemandsByWorkplaceByUserID")]
        [HttpGet]
        public IHttpActionResult GetDemandsByWorkplaceByUserID(string userID)
        {
            try
            {
                List<Client_Demand_IssueItems> result = new List<Client_Demand_IssueItems>();
                Workplace workplace = db.Workplaces.Find(db.SystemUsers.Find(userID).WorkplacePK);
                List<Demand> demands = db.Demands.Where(unit => unit.WorkplacePK == workplace.WorkplacePK && unit.IsOpened).ToList();
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
                    List<Client_Box_Shelf_Row> client_Boxes = issuingDAO.StoredBox_ItemPK_IsRestoredOfEntries(accessory);
                    result.Add(new Client_DemandedItem(item, accessory, issuingDAO.IssuedQuantity(item.DemandedItemPK), client_Boxes));
                }
                //List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                //                                      where rI.RequestPK == request.RequestPK
                //                                      select rI).ToList();
                //foreach (var requestedItem in requestedItems)
                //{
                //    DemandedItem demandedItem = db.DemandedItems.Find(requestedItem.DemandedItemPK);
                //    Accessory accessory = db.Accessories.Find(demandedItem.AccessoryPK);
                //    List<Client_Box_Shelf_Row> client_Boxes = issuingDAO.StoredBox_ItemPK_IsRestoredOfEntries(accessory);
                //    client_RequestedItemDetails.Add(new Client_DemandedItem(requestedItem, accessory, issuingDAO.InStoredQuantity(accessory.AccessoryPK), client_Boxes));
                //}
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }
    }
}
