using StoreManagement.Class;
using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using static StoreManagement.Controllers.IssuingController;

namespace StoreManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AngularController : ApiController
    {
        private UserModel db = new UserModel();

        [Route("api/AngularController/GetAllAccessoryType")]
        [HttpGet]
        public IHttpActionResult GetAllAccessoryType()
        {
            List<AccessoryType> result;
            try
            {
                result = db.AccessoryTypes.OrderByDescending(unit => unit.AccessoryTypePK).ToList();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetAllCustomerActive")]
        [HttpGet]
        public IHttpActionResult GetAllCustomerActive()
        {
            List<Customer> result;
            try
            {
                result = db.Customers.Where(unit => unit.IsActive).OrderByDescending(unit => unit.CustomerPK).ToList();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetAllSupplierActive")]
        [HttpGet]
        public IHttpActionResult GetAllSupplierActive()
        {
            List<Supplier> result;
            try
            {
                result = db.Suppliers.Where(unit => unit.IsActive).OrderByDescending(unit => unit.SupplierPK).ToList();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetAllCustomer")]
        [HttpGet]
        public IHttpActionResult GetAllCustomer()
        {
            List<Customer> result;
            try
            {
                result = db.Customers.OrderByDescending(unit => unit.CustomerPK).ToList();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetAllSupplier")]
        [HttpGet]
        public IHttpActionResult GetAllSupplier()
        {
            List<Supplier> result;
            try
            {
                result = db.Suppliers.OrderByDescending(unit => unit.SupplierPK).ToList();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class Client_Conception_Angular
        {
            public Client_Conception_Angular()
            {
            }

            public Client_Conception_Angular(Conception conception, string customerName)
            {
                ConceptionPK = conception.ConceptionPK;
                ConceptionCode = conception.ConceptionCode;
                Description = conception.Description;
                Year = conception.Year;
                Season = conception.Season;
                IsActive = conception.IsActive;
                CustomerPK = conception.CustomerPK;
                CustomerName = customerName;
            }

            public int ConceptionPK { get; set; }

            public string ConceptionCode { get; set; }

            public string Description { get; set; }

            public int Year { get; set; }

            public string Season { get; set; }

            public bool IsActive { get; set; }

            public int CustomerPK { get; set; }

            public string CustomerName { get; set; }
        }

        [Route("api/AngularController/GetAllConception")]
        [HttpGet]
        public IHttpActionResult GetAllConception()
        {
            List<Client_Conception_Angular> result = new List<Client_Conception_Angular>();
            List<Conception> temp;
            try
            {
                temp = db.Conceptions.OrderByDescending(unit => unit.ConceptionPK).ToList();
                foreach (var conception in temp)
                {
                    result.Add(new Client_Conception_Angular(conception, db.Customers.Find(conception.CustomerPK).CustomerName));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);

        }

        public class Client_AccessoryDetail_Angular
        {
            public Client_AccessoryDetail_Angular()
            {
            }

            public Client_AccessoryDetail_Angular(Accessory accessory, string supplierName, string customerName)
            {
                AccessoryPK = accessory.AccessoryPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                IsActive = accessory.IsActive;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                Comment = accessory.Comment;
                Image = accessory.Image;
                AccessoryTypePK = accessory.AccessoryTypePK;
                SupplierPK = accessory.SupplierPK;
                CustomerPK = accessory.CustomerPK;
                SupplierName = supplierName;
                CustomerName = customerName;
            }

            public Client_AccessoryDetail_Angular(Accessory accessory, string accessoryTypeName, string supplierName, string customerName)
            {
                AccessoryPK = accessory.AccessoryPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                IsActive = accessory.IsActive;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                Comment = accessory.Comment;
                Image = accessory.Image;
                AccessoryTypePK = accessory.AccessoryTypePK;
                SupplierPK = accessory.SupplierPK;
                CustomerPK = accessory.CustomerPK;
                AccessoryTypeName = accessoryTypeName;
                SupplierName = supplierName;
                CustomerName = customerName;
            }

            public int AccessoryPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public bool IsActive { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public string Comment { get; set; }

            public string Image { get; set; }

            public int AccessoryTypePK { get; set; }

            public string AccessoryTypeName { get; set; }

            public int SupplierPK { get; set; }

            public string SupplierName { get; set; }

            public int CustomerPK { get; set; }

            public string CustomerName { get; set; }
        }

        [Route("api/AngularController/GetAccessoryByPK")]
        [HttpGet]
        public IHttpActionResult GetAccessoryByPK(int AccessoryPK)
        {

            var path = "http://" + System.Web.HttpContext.Current.Request.Url.Host + ":50000/Image";
            //var root = HttpContext.Current.Server.MapPath("~/Image");
            Client_AccessoryDetail_Angular result;
            try
            {
                Accessory accessory = db.Accessories.Find(AccessoryPK);
                if (accessory.Image == null) accessory.Image = "default.png";
                accessory.Image = path + "/" + accessory.Image;
                Supplier supplier = db.Suppliers.Find(accessory.SupplierPK);
                Customer customer = db.Customers.Find(accessory.CustomerPK);
                AccessoryType accessoryType = db.AccessoryTypes.Find(accessory.AccessoryTypePK);
                result = new Client_AccessoryDetail_Angular(accessory, accessoryType.Name, supplier.SupplierName, customer.CustomerName);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class Client_ConceptionForAccessoryDetail
        {
            public Client_ConceptionForAccessoryDetail(int conceptionPK, string conceptionFullKey)
            {
                ConceptionPK = conceptionPK;
                ConceptionFullKey = conceptionFullKey;
            }
            public Client_ConceptionForAccessoryDetail(int conceptionPK, string conceptionFullKey, string description)
            {
                ConceptionPK = conceptionPK;
                ConceptionFullKey = conceptionFullKey;
                Description = description;
            }

            public int ConceptionPK { get; set; }

            public string ConceptionFullKey { get; set; }

            public string Description { get; set; }
        }

        public class Client_ListConceptionForAccessoryDetail
        {
            public Client_ListConceptionForAccessoryDetail()
            {
                this.UnlinkConceptions = new List<Client_ConceptionForAccessoryDetail>();
                this.LinkConceptions = new List<Client_ConceptionForAccessoryDetail>();
            }

            public List<Client_ConceptionForAccessoryDetail> UnlinkConceptions { get; set; }

            public List<Client_ConceptionForAccessoryDetail> LinkConceptions { get; set; }
        }

        [Route("api/AngularController/GetConceptionFullKeyByCustomerPKAndAccessoryPK")]
        [HttpGet]
        public IHttpActionResult GetConceptionFullKeyByCustomerPKAndAccessoryPK(int accessoryPK, int customerPK)
        {
            Client_ListConceptionForAccessoryDetail result = new Client_ListConceptionForAccessoryDetail();
            try
            {
                List<Conception> conceptions = (from cc in db.Conceptions.OrderByDescending(unit => unit.ConceptionPK)
                                                where cc.CustomerPK == customerPK && cc.IsActive
                                                select cc).ToList();
                foreach (var conception in conceptions)
                {
                    string temp = conception.ConceptionCode + "-" + conception.Season + "." + (conception.Year + "").Substring(2);
                    if ((from ca in db.ConceptionAccessories
                         where ca.ConceptionPK == conception.ConceptionPK && ca.AccessoryPK == accessoryPK
                         select ca).FirstOrDefault() != null)
                    {
                        result.LinkConceptions.Add(new Client_ConceptionForAccessoryDetail(conception.ConceptionPK, temp));
                    }
                    else
                    {
                        result.UnlinkConceptions.Add(new Client_ConceptionForAccessoryDetail(conception.ConceptionPK, temp));
                    }
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetCustomerByPK")]
        [HttpGet]
        public IHttpActionResult GetCustomerByPK(int customerPK)
        {

            Customer result;
            try
            {
                result = db.Customers.Find(customerPK);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetAccessoriesByCustomerPK")]
        [HttpGet]
        public IHttpActionResult GetAccessoriesByCustomerPK(int customerPK)
        {
            List<Client_AccessoryDetail_Angular> result = new List<Client_AccessoryDetail_Angular>();
            try
            {
                List<Accessory> tempList = (from acc in db.Accessories.OrderByDescending(unit => unit.AccessoryPK)
                                            where acc.CustomerPK == customerPK && acc.IsActive
                                            select acc).ToList();

                foreach (var item in tempList)
                {
                    Supplier supplier = db.Suppliers.Find(item.SupplierPK);
                    result.Add(new Client_AccessoryDetail_Angular(item, supplier.SupplierName, ""));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetConceptionsByCustomerPK")]
        [HttpGet]
        public IHttpActionResult GetConceptionsByCustomerPK(int customerPK)
        {
            List<Client_ConceptionForAccessoryDetail> result = new List<Client_ConceptionForAccessoryDetail>();
            try
            {
                List<Conception> tempList = (from cc in db.Conceptions.OrderByDescending(unit => unit.ConceptionPK)
                                             where cc.CustomerPK == customerPK && cc.IsActive
                                             select cc).ToList();
                foreach (var conception in tempList)
                {
                    string temp = conception.ConceptionCode + "-" + conception.Season + "." + (conception.Year + "").Substring(2);
                    result.Add(new Client_ConceptionForAccessoryDetail(conception.ConceptionPK, temp, conception.Description));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetAccessoriesBySupplierPK")]
        [HttpGet]
        public IHttpActionResult GetAccessoriesBySupplierPK(int supplierPK)
        {
            List<Client_AccessoryDetail_Angular> result = new List<Client_AccessoryDetail_Angular>();
            try
            {
                List<Accessory> tempList = (from acc in db.Accessories.OrderByDescending(unit => unit.AccessoryPK)
                                            where acc.SupplierPK == supplierPK && acc.IsActive
                                            select acc).ToList();

                foreach (var item in tempList)
                {
                    Customer customer = db.Customers.Find(item.CustomerPK);
                    result.Add(new Client_AccessoryDetail_Angular(item, "", customer.CustomerName));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetSupplierByPK")]
        [HttpGet]
        public IHttpActionResult GetSupplierByPK(int supplierPK)
        {

            Supplier result;
            try
            {
                result = db.Suppliers.Find(supplierPK);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class Client_ConceptionDetail
        {
            public Client_ConceptionDetail(Conception conception, string customerName)
            {
                ConceptionPK = conception.ConceptionPK;
                ConceptionCode = conception.ConceptionCode;
                Description = conception.Description;
                Year = conception.Year;
                Season = conception.Season;
                IsActive = conception.IsActive;
                CustomerPK = conception.CustomerPK;
                CustomerName = customerName;
            }

            public int ConceptionPK { get; set; }

            public string ConceptionCode { get; set; }

            public string Description { get; set; }

            public int Year { get; set; }

            public string Season { get; set; }

            public bool IsActive { get; set; }

            public int CustomerPK { get; set; }

            public string CustomerName { get; set; }
        }

        [Route("api/AngularController/GetConceptionByPK")]
        [HttpGet]
        public IHttpActionResult GetConceptionByPK(int conceptionPK)
        {
            Client_ConceptionDetail result;
            try
            {
                Conception temp = db.Conceptions.Find(conceptionPK);
                Customer customer = db.Customers.Find(temp.CustomerPK);
                result = new Client_ConceptionDetail(temp, customer.CustomerName);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetAccessoriesByConceptionPK")]
        [HttpGet]
        public IHttpActionResult GetAccessoriesByConceptionPK(int conceptionPK)
        {
            List<Client_AccessoryDetail_Angular> result = new List<Client_AccessoryDetail_Angular>();
            try
            {
                List<ConceptionAccessory> tempList = (from acc in db.ConceptionAccessories.OrderByDescending(unit => unit.AccessoryPK)
                                                      where acc.ConceptionPK == conceptionPK
                                                      select acc).ToList();

                foreach (var item in tempList)
                {
                    Accessory accessory = db.Accessories.Find(item.AccessoryPK);
                    if (accessory.IsActive)
                    {
                        Supplier supplier = db.Suppliers.Find(accessory.SupplierPK);
                        result.Add(new Client_AccessoryDetail_Angular(accessory, supplier.SupplierName, ""));
                    }
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }


        [Route("api/AngularController/GetPacksByOrderPK")]
        [HttpGet]
        public IHttpActionResult GetPacksByOrderPK(int orderPK)
        {
            List<Pack> result;
            try
            {
                result = (from p in db.Packs
                          where p.OrderPK == orderPK
                          select p).ToList();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class Client_OrderedItem_Angular
        {
            public Client_OrderedItem_Angular()
            {
            }

            public Client_OrderedItem_Angular(Accessory accessory, OrderedItem orderedItem, double packedQuantity, double finalQuantity)
            {
                OrderedItemPK = orderedItem.OrderedItemPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                OrderedQuantity = orderedItem.OrderedQuantity;
                Comment = orderedItem.Comment;
                PackedQuantity = packedQuantity;
                FinalQuantity = finalQuantity;
            }

            public int OrderedItemPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public double OrderedQuantity { get; set; }

            public string Comment { get; set; }

            public double PackedQuantity { get; set; }

            public double FinalQuantity { get; set; }
        }

        [Route("api/AngularController/GetOrderedItemsByOrderPK")]
        [HttpGet]
        public IHttpActionResult GetOrderedItemsByOrderPK(int orderPK)
        {
            List<Client_OrderedItem_Angular> result = new List<Client_OrderedItem_Angular>();
            try
            {
                List<OrderedItem> orderedItems = (from oI in db.OrderedItems
                                                  where oI.OrderPK == orderPK
                                                  select oI).ToList();
                foreach (var orderedItem in orderedItems)
                {
                    Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                    List<PackedItem> packedItems = (from pI in db.PackedItems
                                                    where pI.OrderedItemPK == orderedItem.OrderedItemPK
                                                    select pI).ToList();
                    double packedQuantity = 0;
                    double finalQuantity = 0;
                    foreach (var item in packedItems)
                    {
                        packedQuantity += item.PackedQuantity;
                        ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                         where cI.PackedItemPK == item.PackedItemPK
                                                         select cI).FirstOrDefault();
                        if (classifiedItem != null)
                        {
                            if (classifiedItem.QualityState == 2)
                            {
                                finalQuantity = new IdentifyItemDAO().FinalQuantity(item.PackedItemPK);
                            }
                        }
                    }

                    result.Add(new Client_OrderedItem_Angular(accessory, orderedItem, packedQuantity, finalQuantity));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class Client_Order_Angular
        {
            public Client_Order_Angular(Order order, string supplierName, string systemUserName)
            {
                OrderPK = order.OrderPK;
                OrderID = order.OrderID;
                DateCreated = order.DateCreated;
                IsOpened = order.IsOpened;
                SupplierPK = order.SupplierPK;
                SupplierName = supplierName;
                UserID = order.UserID;
                SystemUserName = systemUserName;
            }

            public int OrderPK { get; set; }

            public string OrderID { get; set; }

            public DateTime DateCreated { get; set; }

            public bool IsOpened { get; set; }

            public int SupplierPK { get; set; }

            public string SupplierName { get; set; }

            public string UserID { get; set; }

            public string SystemUserName { get; set; }
        }

        [Route("api/AngularController/GetOrderByOrderPK")]
        [HttpGet]
        public IHttpActionResult GetOrderByOrderPK(int orderPK)
        {
            Client_Order_Angular result;
            try
            {
                Order order = db.Orders.Find(orderPK);
                Supplier supplier = db.Suppliers.Find(order.SupplierPK);
                SystemUser systemUser = db.SystemUsers.Find(order.UserID);
                result = new Client_Order_Angular(order, supplier.SupplierName, systemUser.Name);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class Client_Accessories_Angular
        {
            public Client_Accessories_Angular()
            {
            }

            public Client_Accessories_Angular(Accessory accessory, double orderedQuantity, double packedQuantity, double finalQuantity)
            {
                AccessoryPK = accessory.AccessoryPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                OrderedQuantity = orderedQuantity;
                PackedQuantity = packedQuantity;
                FinalQuantity = finalQuantity;
            }

            public int AccessoryPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public double OrderedQuantity { get; set; }

            public double PackedQuantity { get; set; }

            public double FinalQuantity { get; set; }
        }

        [Route("api/AngularController/GetAccessoriesWithFilter")]
        [HttpGet]
        public IHttpActionResult GetAccessoriesWithFilter(DateTime start, DateTime end)
        {
            List<Client_Accessories_Angular> result = new List<Client_Accessories_Angular>();
            // make it one more day to make sure < end will be right answer
            end = end.AddDays(1);
            try
            {
                List<Accessory> accessories = db.Accessories.ToList();
                foreach (var accessory in accessories)
                {
                    List<OrderedItem> orderedItems = new List<OrderedItem>();
                    // if start > 1900 then select query
                    if (start.Year > 1900)
                    {

                        List<OrderedItem> tempList = (from oI in db.OrderedItems.OrderByDescending(unit => unit.OrderedItemPK)
                                                      where oI.AccessoryPK == accessory.AccessoryPK
                                                      select oI).ToList();

                        foreach (var orderedItem in tempList)
                        {
                            if (db.Orders.Find(orderedItem.OrderPK).DateCreated >= start
                                && db.Orders.Find(orderedItem.OrderPK).DateCreated < end)
                            {
                                orderedItems.Add(orderedItem);
                            }
                        }
                    }
                    // if start <= 1900 then select all
                    else
                    {
                        orderedItems = (from oI in db.OrderedItems.OrderByDescending(unit => unit.OrderedItemPK)
                                        where oI.AccessoryPK == accessory.AccessoryPK
                                        select oI).ToList();
                    }
                    if (orderedItems.Count > 0)
                    {
                        double orderedQuantity = 0;
                        double packedQuantity = 0;
                        double finalQuantity = 0;
                        foreach (var orderedItem in orderedItems)
                        {
                            // tổng các ordered quantity
                            orderedQuantity += orderedItem.OrderedQuantity;
                            List<PackedItem> packedItems = (from pI in db.PackedItems
                                                            where pI.OrderedItemPK == orderedItem.OrderedItemPK
                                                            select pI).ToList();

                            foreach (var item in packedItems)
                            {
                                // tổng các packeditem quantity theo list ordereditem
                                packedQuantity += item.PackedQuantity;
                                ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                                 where cI.PackedItemPK == item.PackedItemPK
                                                                 select cI).FirstOrDefault();
                                if (classifiedItem != null)
                                {
                                    if (classifiedItem.QualityState == 2)
                                    {
                                        // tổng các final quantity
                                        finalQuantity += new IdentifyItemDAO().FinalQuantity(item.PackedItemPK);
                                    }
                                }
                            }
                        }
                        result.Add(new Client_Accessories_Angular(accessory, orderedQuantity, packedQuantity, finalQuantity));
                    }
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        public class Client_Accessories_Issuing_Angular
        {
            public Client_Accessories_Issuing_Angular()
            {
            }

            public Client_Accessories_Issuing_Angular(Accessory accessory, double demandedQuantity, double issuedQuantity)
            {
                AccessoryPK = accessory.AccessoryPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                SumDemandedQuantity = demandedQuantity;
                SumIssuedQuantity = issuedQuantity;
            }

            public int AccessoryPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public double SumDemandedQuantity { get; set; }

            public double SumIssuedQuantity { get; set; }
        }

        [Route("api/AngularController/GetAccessoriesWithFilterIssuing")]
        [HttpGet]
        public IHttpActionResult GetAccessoriesWithFilterIssuing(DateTime start, DateTime end)
        {
            List<Client_Accessories_Issuing_Angular> result = new List<Client_Accessories_Issuing_Angular>();
            // make it one more day to make sure < end will be right answer
            end = end.AddDays(1);
            try
            {
                List<Accessory> accessories = db.Accessories.ToList();
                foreach (var accessory in accessories)
                {
                    List<DemandedItem> demandedItems = new List<DemandedItem>();
                    // if start > 1900 then select query
                    if (start.Year > 1900)
                    {

                        List<DemandedItem> tempList = (from oI in db.DemandedItems.OrderByDescending(unit => unit.DemandedItemPK)
                                                       where oI.AccessoryPK == accessory.AccessoryPK
                                                       select oI).ToList();

                        foreach (var demandedItem in tempList)
                        {
                            if (db.Demands.Find(demandedItem.DemandPK).DateCreated >= start
                                && db.Demands.Find(demandedItem.DemandPK).DateCreated < end)
                            {
                                demandedItems.Add(demandedItem);
                            }
                        }
                    }
                    // if start <= 1900 then select all
                    else
                    {
                        demandedItems = (from oI in db.DemandedItems.OrderByDescending(unit => unit.DemandedItemPK)
                                         where oI.AccessoryPK == accessory.AccessoryPK
                                         select oI).ToList();
                    }
                    if (demandedItems.Count > 0)
                    {
                        double demandedQuantity = 0;
                        double issuedQuantity = 0;
                        foreach (var demandedItem in demandedItems)
                        {
                            // tổng các demanded quantity
                            demandedQuantity += demandedItem.DemandedQuantity;
                            List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                                  where rI.DemandedItemPK == demandedItem.DemandedItemPK
                                                                  select rI).ToList();

                            foreach (var item in requestedItems)
                            {
                                Request request = db.Requests.Find(item.RequestPK);
                                // tổng các issuedQuantity quantity theo list demandeditem
                                if (request.IsIssued)
                                {
                                    issuedQuantity += item.RequestedQuantity;
                                }
                            }
                        }
                        result.Add(new Client_Accessories_Issuing_Angular(accessory, demandedQuantity, issuedQuantity));
                    }
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }


        [Route("api/AngularController/GetAccessoryByPKReceiving")]
        [HttpGet]
        public IHttpActionResult GetAccessoryByPKReceiving(int accessoryPK)
        {
            Client_Accessories_Angular result;
            try
            {
                Accessory accessory = db.Accessories.Find(accessoryPK);
                List<OrderedItem> orderedItems = (from oI in db.OrderedItems.OrderByDescending(unit => unit.OrderedItemPK)
                                                  where oI.AccessoryPK == accessory.AccessoryPK
                                                  select oI).ToList();

                double orderedQuantity = 0;
                double packedQuantity = 0;
                double finalQuantity = 0;
                foreach (var orderedItem in orderedItems)
                {
                    // tổng các ordered quantity
                    orderedQuantity += orderedItem.OrderedQuantity;
                    List<PackedItem> packedItems = (from pI in db.PackedItems
                                                    where pI.OrderedItemPK == orderedItem.OrderedItemPK
                                                    select pI).ToList();

                    foreach (var item in packedItems)
                    {
                        // tổng các packeditem quantity theo list ordereditem
                        packedQuantity += item.PackedQuantity;
                        ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                         where cI.PackedItemPK == item.PackedItemPK
                                                         select cI).FirstOrDefault();
                        if (classifiedItem != null)
                        {
                            if (classifiedItem.QualityState == 2)
                            {
                                // tổng các final quantity
                                finalQuantity += new IdentifyItemDAO().FinalQuantity(item.PackedItemPK);
                            }
                        }
                    }
                }
                result = new Client_Accessories_Angular(accessory, orderedQuantity, packedQuantity, finalQuantity);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        public class Client_PackedItem_Angular
        {
            public Client_PackedItem_Angular()
            {
            }

            public Client_PackedItem_Angular(Pack pack, double packedQuantity, double finalQuantity)
            {
                PackPK = pack.PackPK;
                PackID = pack.PackID;
                DateCreated = pack.DateCreated;
                PackedQuantity = packedQuantity;
                FinalQuantity = finalQuantity;
            }

            public int PackPK { get; set; }

            public string PackID { get; set; }

            public DateTime DateCreated { get; set; }

            public double PackedQuantity { get; set; }

            public double FinalQuantity { get; set; }
        }

        [Route("api/AngularController/GetPackedItemsByAccessoryPKReceiving")]
        [HttpGet]
        public IHttpActionResult GetPackedItemsByAccessoryPKReceiving(int accessoryPK)
        {
            List<Client_PackedItem_Angular> result = new List<Client_PackedItem_Angular>();
            try
            {
                Accessory accessory = db.Accessories.Find(accessoryPK);
                List<OrderedItem> orderedItems = (from oI in db.OrderedItems.OrderByDescending(unit => unit.OrderedItemPK)
                                                  where oI.AccessoryPK == accessory.AccessoryPK
                                                  select oI).ToList();

                foreach (var orderedItem in orderedItems)
                {
                    List<PackedItem> packedItems = (from pI in db.PackedItems
                                                    where pI.OrderedItemPK == orderedItem.OrderedItemPK
                                                    select pI).ToList();

                    foreach (var item in packedItems)
                    {
                        double packedQuantity = 0;
                        double finalQuantity = 0;
                        Pack pack = db.Packs.Find(item.PackPK);
                        // packeditem quantity theo ordereditem
                        packedQuantity = item.PackedQuantity;
                        ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                         where cI.PackedItemPK == item.PackedItemPK
                                                         select cI).FirstOrDefault();
                        if (classifiedItem != null)
                        {
                            if (classifiedItem.QualityState == 2)
                            {
                                // final quantity
                                finalQuantity = new IdentifyItemDAO().FinalQuantity(item.PackedItemPK);
                            }
                        }
                        result.Add(new Client_PackedItem_Angular(pack, packedQuantity, finalQuantity));
                    }

                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }


        public class Client_Accessories_Stored_Angular
        {
            public Client_Accessories_Stored_Angular()
            {
            }

            public Client_Accessories_Stored_Angular(Accessory accessory, double instoredQuantity)
            {
                AccessoryPK = accessory.AccessoryPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                InstoredQuantity = instoredQuantity;
            }

            public int AccessoryPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public double InstoredQuantity { get; set; }
        }

        [Route("api/AngularController/GetAccessoriesStoring")]
        [HttpGet]
        public IHttpActionResult GetAccessoriesStoring()
        {
            List<Client_Accessories_Stored_Angular> result = new List<Client_Accessories_Stored_Angular>();
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                List<Accessory> accessories = db.Accessories.ToList();
                foreach (var accessory in accessories)
                {
                    List<Entry> entries = (from e in db.Entries
                                           where e.AccessoryPK == accessory.AccessoryPK
                                           select e).ToList();
                    result.Add(new Client_Accessories_Stored_Angular(accessory, storingDAO.EntriesQuantity(entries)));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }
        // có thể ko cần
        [Route("api/AngularController/GetAccessoriesStoringByCustomerPK")]
        [HttpGet]
        public IHttpActionResult GetAccessoriesStoringByCustomerPK(int customerPK)
        {
            List<Client_Accessories_Stored_Angular> result = new List<Client_Accessories_Stored_Angular>();
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                Customer customer = db.Customers.Find(customerPK);
                if (customer == null)
                {
                    return Content(HttpStatusCode.Conflict, "KHÁCH HÀNG KHÔNG TỒN TẠI!");
                }
                List<Accessory> accessories = (from acc in db.Accessories
                                               where acc.CustomerPK == customerPK
                                               select acc).ToList();
                foreach (var accessory in accessories)
                {
                    List<Entry> entries = (from e in db.Entries
                                           where e.AccessoryPK == accessory.AccessoryPK
                                           select e).ToList();
                    result.Add(new Client_Accessories_Stored_Angular(accessory, storingDAO.EntriesQuantity(entries)));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        // có thể không cần
        [Route("api/AngularController/GetAccessoriesStoringBySupplierPK")]
        [HttpGet]
        public IHttpActionResult GetAccessoriesStoringBySupplierPK(int supplierPK)
        {
            List<Client_Accessories_Stored_Angular> result = new List<Client_Accessories_Stored_Angular>();
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                Supplier supplier = db.Suppliers.Find(supplierPK);
                if (supplier == null)
                {
                    return Content(HttpStatusCode.Conflict, "NHÀ CUNG CẤP KHÔNG TỒN TẠI!");
                }
                List<Accessory> accessories = (from acc in db.Accessories
                                               where acc.SupplierPK == supplierPK
                                               select acc).ToList();
                foreach (var accessory in accessories)
                {
                    List<Entry> entries = (from e in db.Entries
                                           where e.AccessoryPK == accessory.AccessoryPK
                                           select e).ToList();
                    result.Add(new Client_Accessories_Stored_Angular(accessory, storingDAO.EntriesQuantity(entries)));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetAccessoryByPKIssuing")]
        [HttpGet]
        public IHttpActionResult GetAccessoryByPKIssuing(int accessoryPK)
        {
            Client_Accessories_Issuing_Angular result = new Client_Accessories_Issuing_Angular();
            try
            {
                Accessory accessory = db.Accessories.Find(accessoryPK);
                List<DemandedItem> demandedItems = new List<DemandedItem>();
                demandedItems = (from oI in db.DemandedItems.OrderByDescending(unit => unit.DemandedItemPK)
                                 where oI.AccessoryPK == accessory.AccessoryPK
                                 select oI).ToList();
                if (demandedItems.Count > 0)
                {
                    double demandedQuantity = 0;
                    double issuedQuantity = 0;
                    foreach (var demandedItem in demandedItems)
                    {
                        // tổng các demanded quantity
                        demandedQuantity += demandedItem.DemandedQuantity;
                        List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                              where rI.DemandedItemPK == demandedItem.DemandedItemPK
                                                              select rI).ToList();

                        foreach (var item in requestedItems)
                        {
                            Request request = db.Requests.Find(item.RequestPK);
                            // tổng các issuedQuantity quantity theo list demandeditem
                            if (request.IsIssued)
                            {
                                issuedQuantity += item.RequestedQuantity;
                            }
                        }
                    }
                    result = new Client_Accessories_Issuing_Angular(accessory, demandedQuantity, issuedQuantity);
                }


            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        public class Client_Demand_Angular
        {
            public Client_Demand_Angular()
            {
            }

            public Client_Demand_Angular(Demand demand, double demandedQuantity, double issuedQuantity)
            {
                DemandPK = demand.DemandPK;
                DemandID = demand.DemandID;
                DateCreated = demand.DateCreated;
                DemandedQuantity = demandedQuantity;
                IssuedQuantity = issuedQuantity;
            }

            public int DemandPK { get; set; }

            public string DemandID { get; set; }

            public DateTime DateCreated { get; set; }

            public double DemandedQuantity { get; set; }

            public double IssuedQuantity { get; set; }

        }

        [Route("api/AngularController/GetDemandsByAccessoryPKIssuing")]
        [HttpGet]
        public IHttpActionResult GetDemandsByAccessoryPKIssuing(int accessoryPK)
        {
            List<Client_Demand_Angular> result = new List<Client_Demand_Angular>();
            try
            {
                List<DemandedItem> demandedItems = (from dI in db.DemandedItems
                                                    where dI.AccessoryPK == accessoryPK
                                                    select dI).ToList();
                foreach (var demandedItem in demandedItems)
                {
                    double demandedQuantity = 0;
                    double issuedQuantity = 0;
                    demandedQuantity += demandedItem.DemandedQuantity;
                    List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                          where rI.DemandedItemPK == demandedItem.DemandedItemPK
                                                          select rI).ToList();
                    Demand demand = db.Demands.Find(demandedItem.DemandPK);

                    foreach (var item in requestedItems)
                    {
                        Request request = db.Requests.Find(item.RequestPK);
                        if (request.IsIssued)
                        {
                            issuedQuantity += item.RequestedQuantity;
                        }
                    }
                    result.Add(new Client_Demand_Angular(demand, demandedQuantity, issuedQuantity));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        public class Client_Demand_Angular2
        {
            public Client_Demand_Angular2()
            {
            }

            public Client_Demand_Angular2(Demand demand, Conception conception)
            {
                DemandPK = demand.DemandPK;
                DemandID = demand.DemandID;
                ReceiveDivision = demand.ReceiveDivision;
                ConceptionCode = conception.ConceptionCode;
                DateCreated = demand.DateCreated;
                IsOpened = demand.IsOpened;
                UserID = demand.UserID;
            }

            public int DemandPK { get; set; }

            public string DemandID { get; set; }

            public string ReceiveDivision { get; set; }

            public string ConceptionCode { get; set; }

            public DateTime DateCreated { get; set; }

            public bool IsOpened { get; set; }

            public string UserID { get; set; }

        }

        [Route("api/AngularController/GetDemandsIssuing")]
        [HttpGet]
        public IHttpActionResult GetDemandsIssuing(DateTime start, DateTime end)
        {
            List<Client_Demand_Angular2> result = new List<Client_Demand_Angular2>();
            List<Demand> demands;
            try
            {
                // make it one more day to make sure < end will be right answer
                end = end.AddDays(1);
                // if start > 1900 then select query
                if (start.Year > 1900)
                {
                    demands = (from de in db.Demands.OrderByDescending(unit => unit.DemandPK)
                               where de.DateCreated >= start && de.DateCreated < end
                               select de).ToList();
                }
                // if start <= 1900 then select all
                else
                {
                    demands = db.Demands.ToList();
                }

                foreach (var demand in demands)
                {
                    Conception conception = db.Conceptions.Find(demand.ConceptionPK);
                    result.Add(new Client_Demand_Angular2(demand, conception));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        public class Client_DemandedItem_Issued_Angular
        {
            public Client_DemandedItem_Issued_Angular()
            {
            }

            public Client_DemandedItem_Issued_Angular(DemandedItem demandedItem, Accessory accessory, double demandedQuantity, double issuedQuantity)
            {
                DemandedItemPK = demandedItem.DemandedItemPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                DemandedQuantity = demandedQuantity;
                IssuedQuantity = issuedQuantity;
                Comment = demandedItem.Comment;
            }

            public int DemandedItemPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public string Comment { get; set; }

            public double DemandedQuantity { get; set; }

            public double IssuedQuantity { get; set; }
        }

        [Route("api/AngularController/GetDemandedItemsByDemandPKIssuing")]
        [HttpGet]
        public IHttpActionResult GetDemandedItemsByDemandPKIssuing(int demandPK)
        {
            List<Client_DemandedItem_Issued_Angular> result = new List<Client_DemandedItem_Issued_Angular>();
            try
            {
                Demand demand = db.Demands.Find(demandPK);
                if (demand == null)
                {
                    return Content(HttpStatusCode.Conflict, "PHIẾU CẤP PHÁT KHÔNG TỒN TẠI");
                }
                List<DemandedItem> demandedItems = (from dI in db.DemandedItems
                                                    where dI.DemandPK == demand.DemandPK

                                                    select dI).ToList();
                foreach (var demandedItem in demandedItems)
                {
                    double demandedQuantity = 0;
                    double issuedQuantity = 0;
                    demandedQuantity += demandedItem.DemandedQuantity;
                    List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                          where rI.DemandedItemPK == demandedItem.DemandedItemPK
                                                          select rI).ToList();
                    Accessory accessory = db.Accessories.Find(demandedItem.AccessoryPK);

                    foreach (var item in requestedItems)
                    {
                        Request request = db.Requests.Find(item.RequestPK);
                        if (request.IsIssued)
                        {
                            issuedQuantity += item.RequestedQuantity;
                        }
                    }
                    result.Add(new Client_DemandedItem_Issued_Angular(demandedItem, accessory, demandedQuantity, issuedQuantity));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetRequestsByDemandPKIssuing")]
        [HttpGet]
        public IHttpActionResult GetRequestsByDemandPKIssuing(int demandPK)
        {
            List<Request> result = new List<Request>();
            try
            {
                result = (from re in db.Requests
                          where re.DemandPK == demandPK
                          select re).ToList();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        public class Client_Demand_Angular3
        {
            public Client_Demand_Angular3()
            {
            }

            public Client_Demand_Angular3(Demand demand, Conception conception, Customer customer, SystemUser systemUser)
            {
                DemandPK = demand.DemandPK;
                DemandID = demand.DemandID;
                CustomerName = customer.CustomerName;
                ReceiveDivision = demand.ReceiveDivision;
                ConceptionCode = conception.ConceptionCode;
                DateCreated = demand.DateCreated;
                IsOpened = demand.IsOpened;
                TotalDemand = demand.TotalDemand;
                UserID = demand.UserID;
                SystemUserName = systemUser.Name;
            }

            public int DemandPK { get; set; }

            public string DemandID { get; set; }

            public string ReceiveDivision { get; set; }

            public string ConceptionCode { get; set; }

            public DateTime DateCreated { get; set; }

            public bool IsOpened { get; set; }

            public string CustomerName { get; set; }

            public double TotalDemand { get; set; }

            public string UserID { get; set; }

            public string SystemUserName { get; set; }
        }

        [Route("api/AngularController/GetDemandByDemandPKIssuing")]
        [HttpGet]
        public IHttpActionResult GetDemandByDemandPKIssuing(int demandPK)
        {
            Client_Demand_Angular3 result;
            try
            {
                Demand demand = db.Demands.Find(demandPK);
                Conception conception = db.Conceptions.Find(demand.ConceptionPK);
                Customer customer = db.Customers.Find(conception.CustomerPK);
                SystemUser systemUser = db.SystemUsers.Find(demand.UserID);
                result = new Client_Demand_Angular3(demand, conception, customer, systemUser);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        public class Client_DemandedItem_Issued_Angular2
        {
            public Client_DemandedItem_Issued_Angular2()
            {
            }

            public Client_DemandedItem_Issued_Angular2(Accessory accessory, double demandedQuantity, double issuedQuantity)
            {
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                DemandedQuantity = demandedQuantity;
                IssuedQuantity = issuedQuantity;
            }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public double DemandedQuantity { get; set; }

            public double IssuedQuantity { get; set; }
        }

        [Route("api/AngularController/GetDemandedItemByConceptionPK")]
        [HttpGet]
        public IHttpActionResult GetDemandedItemByConceptionPK(int conceptionPK)
        {
            List<Client_DemandedItem_Issued_Angular2> result = new List<Client_DemandedItem_Issued_Angular2>();
            try
            {
                Dictionary<int, Client_DemandedItem_Issued_Angular2> tempDictionary = new Dictionary<int, Client_DemandedItem_Issued_Angular2>();
                Conception conception = db.Conceptions.Find(conceptionPK);
                if (conception == null)
                {
                    return Content(HttpStatusCode.Conflict, "MÃ HÀNG KHÔNG TỒN TẠI");
                }
                List<Demand> demands = (from de in db.Demands.OrderByDescending(unit => unit.DemandPK)
                                        where de.ConceptionPK == conception.ConceptionPK
                                        select de).ToList();
                foreach (var demand in demands)
                {
                    List<DemandedItem> demandedItems = (from dI in db.DemandedItems
                                                        where dI.DemandPK == demand.DemandPK
                                                        select dI).ToList();
                    foreach (var demandedItem in demandedItems)
                    {
                        double demandedQuantity = 0;
                        double issuedQuantity = 0;
                        demandedQuantity += demandedItem.DemandedQuantity;
                        List<RequestedItem> requestedItems = (from rI in db.RequestedItems
                                                              where rI.DemandedItemPK == demandedItem.DemandedItemPK
                                                              select rI).ToList();
                        Accessory accessory = db.Accessories.Find(demandedItem.AccessoryPK);

                        foreach (var item in requestedItems)
                        {
                            Request request = db.Requests.Find(item.RequestPK);
                            if (request.IsIssued)
                            {
                                issuedQuantity += item.RequestedQuantity;
                            }
                        }
                        if (!tempDictionary.Keys.Contains(accessory.AccessoryPK))
                        {
                            tempDictionary.Add(accessory.AccessoryPK, new Client_DemandedItem_Issued_Angular2(accessory, demandedQuantity, issuedQuantity));
                        }
                        else
                        {
                            tempDictionary[accessory.AccessoryPK].DemandedQuantity += demandedQuantity;
                            tempDictionary[accessory.AccessoryPK].IssuedQuantity += issuedQuantity;
                        }
                    }

                }
                foreach (var item in tempDictionary)
                {
                    result.Add(item.Value);
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }


    }
}

