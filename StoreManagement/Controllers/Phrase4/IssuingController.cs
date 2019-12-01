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
                    Request request = issuingDAO.CreateRequest(requestID, expectedDate, false, false, comment, demandPK);

                    // create requestedItems
                    issuingDAO.createRequestedItems(list, request.RequestPK);
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
