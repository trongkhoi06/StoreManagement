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
    public class IssuingController : ApiController
    {
        private UserModel db = new UserModel();

        [Route("api/IssuingController/CreateDemand")]
        [HttpPost]
        public IHttpActionResult CreateDemand(int customerPK, string demandID, string conceptionCode, int startWeek, int endWeek, double totalDemand, string receiveDevision, string userID, [FromBody] List<Client_Accessory_DemandedQuantity_Comment> list)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Mechandiser"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                Demand demand = null;
                try
                {
                    // kiểm khi chạy lệnh
                    if (issuingDAO.GetDemandByDemandID(demandID) != null)
                    {
                        return Content(HttpStatusCode.Conflict, "DEMAND ĐÃ TỒN TẠI");
                    }
                    Conception conception = issuingDAO.GetConceptionByConceptionCode(conceptionCode);
                    if (conception.CustomerPK != customerPK)
                    {
                        return Content(HttpStatusCode.Conflict, "KHÔNG ĐÚNG KHÁCH HÀNG");
                    }
                    if (startWeek > 52 || startWeek < 1 || endWeek > 52 || endWeek < 1)
                    {
                        return Content(HttpStatusCode.Conflict, "SỐ LIỆU TUẦN KHÔNG HỢP LỆ");
                    }
                    demand = issuingDAO.CreateDemand(customerPK, demandID, conception.ConceptionPK, startWeek, endWeek, totalDemand, receiveDevision, userID);
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
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Mechandiser"))
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
                    if (issuingDAO.GetRequestFromDemandPK(demandPK).Count > 0)
                    {
                        return Content(HttpStatusCode.Conflict, "DEMAND ĐÃ CÓ YÊU CẦU XUẤT!");
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
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Mechandiser"))
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
                    if (issuingDAO.GetRequestFromDemandPK(demandPK).Count > 0)
                    {
                        return Content(HttpStatusCode.Conflict, "DEMAND ĐÃ CÓ YÊU CẦU XUẤT!");
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
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Mechandiser"))
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
                List<Demand> demands = (from d in db.Demands
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

        [Route("api/IssuingController/GetDemandAndDemandedItemsByDemandPK")]
        [HttpGet]
        public IHttpActionResult GetDemandAndDemandedItemsByDemandPK(int demandPK)
        {
            List<Client_DemandDetail> client_Demands = new List<Client_DemandDetail>();
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                Demand demand = db.Demands.Find(demandPK);
                List<DemandedItem> demandedItems = (from dI in db.DemandedItems
                                                    where dI.DemandPK == demandPK
                                                    select dI).ToList();
                foreach (var demandedItem in demandedItems)
                {
                    Accessory accessory = db.Accessories.Find(demandedItem.AccessoryPK);
                    List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                          where rI.DemandedItemPK == demandedItem.DemandedItemPK
                                                          select rI).ToList();
                    client_Demands.Add(new Client_DemandDetail(demandedItem, accessory,
                        issuingDAO.TotalRequestedQuantity(requestedItems),
                        //issuingDAO.TotalRequestedQuantityConfirmed(requestedItems),
                        issuingDAO.InStoredQuantity(accessory.AccessoryPK) - issuingDAO.InRequestedQuantity(accessory.AccessoryPK)));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, client_Demands);
        }

        [Route("api/IssuingController/CreateRequest")]
        [HttpPost]
        public IHttpActionResult CreateRequest(int demandPK, DateTime expectedDate, string comment, string userID, [FromBody] List<Client_DemandedItemPK_RequestedQuantity> list)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Receiver"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                try
                {
                    foreach (var item in list)
                    {
                        DemandedItem demandedItem = db.DemandedItems.Find(item.DemandedItemPK);
                        Accessory accessory = db.Accessories.Find(demandedItem.AccessoryPK);
                        List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                              where rI.DemandedItemPK == demandedItem.DemandedItemPK
                                                              select rI).ToList();
                        if (item.RequestedQuantity > (issuingDAO.InStoredQuantity(accessory.AccessoryPK)
                                                        - issuingDAO.InRequestedQuantity(accessory.AccessoryPK)))
                        {
                            return Content(HttpStatusCode.Conflict, "SỐ LƯỢNG YÊU CẦU XUẤT KHÔNG HỢP LỆ!");
                        }
                        if (demandedItem.DemandedQuantity < item.RequestedQuantity + issuingDAO.TotalRequestedQuantity(requestedItems))
                        {
                            return Content(HttpStatusCode.Conflict, "SỐ LƯỢNG YÊU CẦU XUẤT KHÔNG HỢP LỆ!");
                        }
                    }
                    if (expectedDate.Date < DateTime.Now)
                    {
                        return Content(HttpStatusCode.Conflict, "NGÀY DỰ KIẾN KHÔNG HỢP LỆ!");
                    }

                    // init requestid
                    int noRequestID;
                    Demand demand = db.Demands.Find(demandPK);
                    if (!demand.IsOpened)
                    {
                        return Content(HttpStatusCode.Conflict, "DEMAND ĐÃ ĐÓNG, KHÔNG THỂ TẠO YÊU CẦU XUẤT!");
                    }
                    Request lastRequest = (from p in db.Requests.OrderByDescending(unit => unit.RequestPK)
                                           where p.RequestID.Contains(demand.DemandID)
                                           select p).FirstOrDefault();
                    if (lastRequest == null)
                    {
                        noRequestID = 1;
                    }
                    else
                    {
                        noRequestID = Int32.Parse(lastRequest.RequestID.Substring(lastRequest.RequestID.Length - 2)) + 1;
                    }
                    string requestID = (noRequestID >= 10) ? (demand.DemandID + "#" + noRequestID) : (demand.DemandID + "#" + "0" + noRequestID);

                    // create request
                    Request request = issuingDAO.CreateRequest(requestID, expectedDate, false, false, comment, demandPK, userID);

                    // create requestedItems
                    issuingDAO.CreateRequestedItems(list, request.RequestPK);
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

        [Route("api/IssuingController/GetRequestByUserID")]
        [HttpGet]
        public IHttpActionResult GetRequestByUserID(string userID)
        {
            List<Client_Request> client_Requests = new List<Client_Request>();
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                List<Request> requests = (from re in db.Requests
                                          where re.UserID == userID
                                          select re).ToList();
                foreach (var request in requests)
                {
                    Demand demand = db.Demands.Find(request.DemandPK);
                    Conception conception = db.Conceptions.Find(demand.ConceptionPK);
                    client_Requests.Add(new Client_Request(request, demand, conception));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, client_Requests);
        }

        [Route("api/IssuingController/GetRequestedItemsByRequestPK")]
        [HttpGet]
        public IHttpActionResult GetRequestedItemsByRequestPK(int requestPK)
        {
            List<Client_RequestedItemDetail> client_RequestedItemDetails = new List<Client_RequestedItemDetail>();
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                Request request = db.Requests.Find(requestPK);
                List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                      where rI.RequestPK == request.RequestPK
                                                      select rI).ToList();
                foreach (var requestedItem in requestedItems)
                {
                    DemandedItem demandedItem = db.DemandedItems.Find(requestedItem.DemandedItemPK);
                    Accessory accessory = db.Accessories.Find(demandedItem.AccessoryPK);
                    double sumOfOtherRequestedItem = issuingDAO.OtherRequestedItem(demandedItem.DemandedItemPK, requestedItem.RequestedItemPK);

                    client_RequestedItemDetails.Add(new Client_RequestedItemDetail(requestedItem, accessory, demandedItem.DemandedQuantity, sumOfOtherRequestedItem,
                        issuingDAO.InStoredQuantity(accessory.AccessoryPK) - issuingDAO.InRequestedQuantity(accessory.AccessoryPK)));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, client_RequestedItemDetails);
        }

        [Route("api/IssuingController/EditRequest")]
        [HttpPut]
        public IHttpActionResult EditRequest(int requestPK, DateTime expectedDate, string comment, string userID, [FromBody] List<Client_RequestedItemPK_RequestedQuantity> list)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Receiver"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                try
                {
                    foreach (var item in list)
                    {
                        RequestedItem requestedItem = db.RequestedItems.Find(item.RequestedItemPK);
                        DemandedItem demandedItem = db.DemandedItems.Find(requestedItem.DemandedItemPK);
                        Accessory accessory = db.Accessories.Find(demandedItem.AccessoryPK);
                        List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                              where rI.DemandedItemPK == demandedItem.DemandedItemPK
                                                              select rI).ToList();
                        if (item.RequestedQuantity > (issuingDAO.InStoredQuantity(accessory.AccessoryPK)
                                                        - issuingDAO.InRequestedQuantity(accessory.AccessoryPK)))
                        {
                            return Content(HttpStatusCode.Conflict, "SỐ LƯỢNG YÊU CẦU XUẤT KHÔNG HỢP LỆ!");
                        }
                        if (demandedItem.DemandedQuantity < item.RequestedQuantity + issuingDAO.TotalRequestedQuantity(requestedItems))
                        {
                            return Content(HttpStatusCode.Conflict, "SỐ LƯỢNG YÊU CẦU XUẤT KHÔNG HỢP LỆ!");
                        }
                    }
                    if (expectedDate.Date < DateTime.Now)
                    {
                        return Content(HttpStatusCode.Conflict, "NGÀY DỰ KIẾN KHÔNG HỢP LỆ!");
                    }
                    Request request = db.Requests.Find(requestPK);
                    if (request.UserID != userID)
                    {
                        return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
                    }
                    if (request.IsIssued)
                    {
                        return Content(HttpStatusCode.Conflict, "YÊU CẦU XUẤT ĐÃ ĐƯỢC CHUẨN BỊ XONG!");
                    }
                    // update request
                    issuingDAO.UpdateRequest(requestPK, comment, expectedDate);

                    // update requestedItems
                    issuingDAO.UpdateRequestedItems(list, requestPK);
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

        [Route("api/IssuingController/DeleteRequest")]
        [HttpDelete]
        public IHttpActionResult DeleteRequest(int requestPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Receiver"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                try
                {
                    Request request = db.Requests.Find(requestPK);
                    if (request.UserID != userID)
                    {
                        return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
                    }
                    if (request.IsIssued)
                    {
                        return Content(HttpStatusCode.Conflict, "YÊU CẦU XUẤT ĐÃ ĐƯỢC CHUẨN BỊ XONG!");
                    }

                    // update requestedItems
                    issuingDAO.DeleteRequestedItems(requestPK);

                    // update request
                    issuingDAO.DeleteRequest(requestPK);

                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "XÓA YÊU CẦU XUẤT THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/IssuingController/GetRequestsNotIssued")]
        [HttpGet]
        public IHttpActionResult GetRequestsNotIssued()
        {
            List<Client_Request2> client_Requests = new List<Client_Request2>();
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                List<Request> requests = (from re in db.Requests
                                          where re.IsIssued == false
                                          select re).ToList();
                foreach (var request in requests)
                {
                    Demand demand = db.Demands.Find(request.DemandPK);
                    Conception conception = db.Conceptions.Find(demand.ConceptionPK);
                    client_Requests.Add(new Client_Request2(request, demand, conception));
                }
            }

            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, client_Requests);
        }

        [Route("api/IssuingController/GetRequestedItemsByRequestPKPrepared")]
        [HttpGet]
        public IHttpActionResult GetRequestedItemsByRequestPKPrepared(int requestPK)
        {
            List<Client_RequestedItem> client_RequestedItemDetails = new List<Client_RequestedItem>();
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                Request request = db.Requests.Find(requestPK);
                List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                      where rI.RequestPK == request.RequestPK
                                                      select rI).ToList();
                foreach (var requestedItem in requestedItems)
                {
                    DemandedItem demandedItem = db.DemandedItems.Find(requestedItem.DemandedItemPK);
                    Accessory accessory = db.Accessories.Find(demandedItem.AccessoryPK);
                    List<Client_Box_Shelf_Row> client_Boxes = issuingDAO.StoredBox_ItemPK_IsRestoredOfEntries(accessory);
                    client_RequestedItemDetails.Add(new Client_RequestedItem(requestedItem, accessory, issuingDAO.InStoredQuantity(accessory.AccessoryPK), client_Boxes));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, client_RequestedItemDetails);
        }

        [Route("api/IssuingController/PrepareRequest")]
        [HttpPost]
        public IHttpActionResult PrepareRequest(int requestPK, string userID, [FromBody] Client_InputPrepareRequestAPI input)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                StoringDAO storingDAO = new StoringDAO();
                BoxDAO boxDAO = new BoxDAO();
                IssuingSession issuingSession = null;
                try
                {
                    issuingDAO.UpdateRequest(requestPK, true);
                    boxDAO.ChangeIsActiveBoxes(input.boxIDs, false);
                    issuingSession = issuingDAO.CreateIssuingSession(userID, requestPK, input.boxIDs);
                    storingDAO.CreateIssueEntry(input, issuingSession);
                }
                catch (Exception e)
                {
                    if (issuingSession != null)
                    {
                        issuingDAO.UpdateRequest(requestPK, false);
                        boxDAO.ChangeIsActiveBoxes(input.boxIDs, true);
                        issuingDAO.DeleteIssuingSession(issuingSession.IssuingSessionPK);
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

        [Route("api/IssuingController/GetIssuingSessionByUserID")]
        [HttpGet]
        public IHttpActionResult GetIssuingSessionByUserID(string userID)
        {
            List<Client_IssuingSession> client_IssuingSessions = new List<Client_IssuingSession>();
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                List<IssuingSession> issuingSessions = (from Iss in db.IssuingSessions
                                                        where Iss.UserID == userID
                                                        select Iss).ToList();
                foreach (var issuingSession in issuingSessions)
                {
                    Request request = db.Requests.Find(issuingSession.RequestPK);
                    Demand demand = db.Demands.Find(request.DemandPK);
                    Conception conception = db.Conceptions.Find(demand.ConceptionPK);
                    client_IssuingSessions.Add(new Client_IssuingSession(issuingSession, demand, request, conception));
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, client_IssuingSessions);
        }

        [Route("api/IssuingController/GetRequestedItemsByIssuingSessionPK")]
        [HttpGet]
        public IHttpActionResult GetRequestedItemsByIssuingSessionPK(int issuingSessionPK)
        {
            List<Client_RequestedItem2> client_RequestedItemDetails = new List<Client_RequestedItem2>();
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                IssuingSession issuingSession = db.IssuingSessions.Find(issuingSessionPK);
                Request request = db.Requests.Find(issuingSession.RequestPK);
                List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                      where rI.RequestPK == request.RequestPK
                                                      select rI).ToList();
                foreach (var requestedItem in requestedItems)
                {
                    DemandedItem demandedItem = db.DemandedItems.Find(requestedItem.DemandedItemPK);
                    Accessory accessory = db.Accessories.Find(demandedItem.AccessoryPK);
                    List<Client_Box_Shelf_Row2> client_Boxes = issuingDAO.StoredBox_ItemPK_IsRestoredOfEntries2(accessory, issuingSessionPK);
                    client_RequestedItemDetails.Add(new Client_RequestedItem2(request, accessory, issuingDAO.InStoredQuantity(accessory.AccessoryPK), client_Boxes));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, client_RequestedItemDetails);
        }

        [Route("api/IssuingController/DeletePreparation")]
        [HttpDelete]
        public IHttpActionResult DeletePreparation(int issuingSessionPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                StoringDAO storingDAO = new StoringDAO();
                BoxDAO boxDAO = new BoxDAO();
                try
                {
                    IssuingSession issuingSession = db.IssuingSessions.Find(issuingSessionPK);
                    if (issuingSession != null)
                    {
                        Request request = db.Requests.Find(issuingSession.RequestPK);
                        if (!request.IsConfirmed)
                        {
                            issuingDAO.UpdateRequest(request.RequestPK, false);
                            List<string> boxIDs = issuingSession.DeactivatedBoxes.Split(new[] { "~!~" }, StringSplitOptions.None).ToList();
                            boxDAO.ChangeIsActiveBoxes(boxIDs, true);
                            issuingDAO.DeleteIssueEntries(issuingSessionPK);
                            issuingDAO.DeleteIssuingSession(issuingSession.IssuingSessionPK);
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "REQUEST ĐÃ ĐƯỢC CONFIRM RỒI NHA!");
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "ĐỪNG PHÁ DỮ LIỆU NHA!");
                    }
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "XÓA YÊU CẦU XUẤT THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/IssuingController/GetRequestsByUserIDForConfirm")]
        [HttpGet]
        public IHttpActionResult GetRequestsByUserIDForConfirm(string userID)
        {
            List<Client_Request2> client_Requests = new List<Client_Request2>();
            IssuingDAO issuingDAO = new IssuingDAO();
            try
            {
                List<Request> requests = (from re in db.Requests
                                          where re.UserID == userID && re.IsIssued == true && re.IsConfirmed == false
                                          select re).ToList();
                foreach (var request in requests)
                {
                    Demand demand = db.Demands.Find(request.DemandPK);
                    Conception conception = db.Conceptions.Find(demand.ConceptionPK);
                    client_Requests.Add(new Client_Request2(request, demand, conception));
                }
            }

            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, client_Requests);
        }

        [Route("api/IssuingController/ConfirmRequest")]
        [HttpPost]
        public IHttpActionResult ConfirmRequest(int requestPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Receiver"))
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                StoringDAO storingDAO = new StoringDAO();
                BoxDAO boxDAO = new BoxDAO();
                try
                {
                    Request request = db.Requests.Find(requestPK);
                    if (request.UserID == userID)
                    {
                        if (!request.IsIssued)
                        {
                            issuingDAO.ConfirmRequest(requestPK, true);
                        }
                        else
                        {
                            return Content(HttpStatusCode.Conflict, "YÊU CẦU XUẤT CHƯA ĐƯỢC CHUẨN BỊ!");
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
                    }
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "XÓA YÊU CẦU XUẤT THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

    }
}
