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
                        issuingDAO.TotalRequestedQuantityConfirmed(requestedItems),
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
            // kiểm trước khi chạy lệnh
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 7)
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

                    client_RequestedItemDetails.Add(new Client_RequestedItemDetail(request, accessory, demandedItem.DemandedQuantity, sumOfOtherRequestedItem,
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
            // kiểm trước khi chạy lệnh
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 7)
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
            // kiểm trước khi chạy lệnh
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 7)
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
                return Content(HttpStatusCode.OK, "TẠO YÊU CẦU XUẤT THÀNH CÔNG!");
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
                    List<Client_Box_Shelf_Row> client_Boxes = issuingDAO.StoredBoxesOfEntries(accessory);
                    client_RequestedItemDetails.Add(new Client_RequestedItem(request, accessory, issuingDAO.InStoredQuantity(accessory.AccessoryPK), client_Boxes));
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
        public IHttpActionResult PrepareRequest(int requestPK, string userID, [FromBody] List<RequestedItem> requestedItems,[FromBody] List<List<Client_StoredBoxPK_IssuedQuantity>> client_StoredBoxPK_IssuedQuantitiess,List<string> boxIDs)
        {
            // kiểm trước khi chạy lệnh
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 7)
            {
                IssuingDAO issuingDAO = new IssuingDAO();
                try
                {

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




        


    }
}
