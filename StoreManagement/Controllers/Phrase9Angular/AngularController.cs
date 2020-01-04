using Microsoft.Office.Interop.Excel;
using QRCoder;
using StoreManagement.Class;
using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
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

        [Route("api/AngularController/GetAccessoriesAvailableByConceptionPK")]
        [HttpGet]
        public IHttpActionResult GetAccessoriesAvailableByConceptionPK(int conceptionPK)
        {
            List<Client_AccessoryDetail_Angular> result = new List<Client_AccessoryDetail_Angular>();
            try
            {
                Conception conception = db.Conceptions.Find(conceptionPK);
                List<Accessory> accessories = (from acc in db.Accessories
                                               where acc.CustomerPK == conception.CustomerPK
                                               select acc).ToList();
                foreach (var accessory in accessories)
                {
                    ConceptionAccessory temp = (from ca in db.ConceptionAccessories
                                                where ca.ConceptionPK == conceptionPK && ca.AccessoryPK == accessory.AccessoryPK
                                                select ca).FirstOrDefault();
                    if (accessory.IsActive && temp == null)
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

            public Client_Accessories_Stored_Angular(Accessory accessory, double instoredQuantity, string customerName, string supplierName)
            {
                AccessoryPK = accessory.AccessoryPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                InstoredQuantity = instoredQuantity;
                CustomerName = customerName;
                SupplierName = supplierName;
            }

            public int AccessoryPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public double InstoredQuantity { get; set; }

            public string CustomerName { get; set; }

            public string SupplierName { get; set; }
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
                    if (storingDAO.EntriesQuantity(entries) > 0)
                    {
                        result.Add(new Client_Accessories_Stored_Angular(accessory, storingDAO.EntriesQuantity(entries)));
                    }
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetAccessoryByPKStoring")]
        [HttpGet]
        public IHttpActionResult GetAccessoryByPKStoring(int accessoryPK)
        {
            Client_Accessories_Stored_Angular result = new Client_Accessories_Stored_Angular();
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                Accessory accessory = db.Accessories.Find(accessoryPK);
                List<Entry> entries = (from e in db.Entries
                                       where e.AccessoryPK == accessory.AccessoryPK
                                       select e).ToList();
                result = new Client_Accessories_Stored_Angular(accessory, storingDAO.EntriesQuantity(entries));
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
                    Supplier supplier = db.Suppliers.Find(accessory.SupplierPK);
                    if (storingDAO.EntriesQuantity(entries) > 0)
                    {
                        result.Add(new Client_Accessories_Stored_Angular(accessory, storingDAO.EntriesQuantity(entries), "", supplier.SupplierName));
                    }
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
                    Customer customer = db.Customers.Find(accessory.CustomerPK);
                    if (storingDAO.EntriesQuantity(entries) > 0)
                    {
                        result.Add(new Client_Accessories_Stored_Angular(accessory, storingDAO.EntriesQuantity(entries), customer.CustomerName, ""));
                    }
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

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_PackedItem_Angular2
        {
            public Client_PackedItem_Angular2(Pack pack, double packedQuantity, double passedQuantity, double instoredQuantity)
            {
                PackID = pack.PackID;
                DateCreated = pack.DateCreated;
                PackedQuantity = packedQuantity;
                PassedQuantity = passedQuantity;
                InstoredQuantity = instoredQuantity;
            }

            public string PackID { get; set; }

            public DateTime DateCreated { get; set; }

            public double PackedQuantity { get; set; }

            public double PassedQuantity { get; set; }

            public double InstoredQuantity { get; set; }
        }

        [Route("api/AngularController/GetPackedItemStoringByAccessoryPK")]
        [HttpGet]
        public IHttpActionResult GetPackedItemStoringByAccessoryPK(int AccessoryPK)
        {
            List<Client_PackedItem_Angular2> result = new List<Client_PackedItem_Angular2>();
            StoringDAO storingDAO = new StoringDAO();
            IdentifyItemDAO identifyItemDAO = new IdentifyItemDAO();
            try
            {
                List<OrderedItem> orderedItems = (from oI in db.OrderedItems.OrderByDescending(unit => unit.OrderedItemPK)
                                                  where oI.AccessoryPK == AccessoryPK
                                                  select oI).ToList();
                foreach (var orderedItem in orderedItems)
                {
                    List<PackedItem> packedItems = (from pI in db.PackedItems.OrderByDescending(unit => unit.PackedItemPK)
                                                    where pI.OrderedItemPK == orderedItem.OrderedItemPK
                                                    select pI).ToList();
                    foreach (var packedItem in packedItems)
                    {
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                         where cI.PackedItemPK == packedItem.PackedItemPK
                                                         select cI).FirstOrDefault();
                        if (classifiedItem != null)
                        {
                            // đã pass == 2
                            if (classifiedItem.QualityState == 2)
                            {
                                PassedItem passedItem = (from pI in db.PassedItems
                                                         where pI.ClassifiedItemPK == classifiedItem.ClassifiedItemPK
                                                         select pI).FirstOrDefault();
                                List<Entry> entries = (from e in db.Entries
                                                       where e.ItemPK == passedItem.PassedItemPK && e.IsRestored == false
                                                       select e).ToList();
                                result.Add(new Client_PackedItem_Angular2(pack, packedItem.PackedQuantity,
                                    identifyItemDAO.FinalQuantity(packedItem.PackedItemPK), storingDAO.EntriesQuantity(entries)));
                            }
                        }
                    }
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }
        public class Client_RestoredItem_Angular
        {
            public Client_RestoredItem_Angular(Restoration restoration, double restoredQuantity, double storedQuantity)
            {
                RestorationID = restoration.RestorationID;
                DateCreated = restoration.DateCreated;
                RestoredQuantity = restoredQuantity;
                InstoredQuantity = storedQuantity;
            }

            public string RestorationID { get; set; }

            public DateTime DateCreated { get; set; }

            public double RestoredQuantity { get; set; }

            public double InstoredQuantity { get; set; }
        }

        [Route("api/AngularController/GetRestoredItemStoringByAccessoryPK")]
        [HttpGet]
        public IHttpActionResult GetRestoredItemStoringByAccessoryPK(int AccessoryPK)
        {
            List<Client_RestoredItem_Angular> result = new List<Client_RestoredItem_Angular>();
            StoringDAO storingDAO = new StoringDAO();
            try
            {
                List<RestoredItem> restoredItems = (from rI in db.RestoredItems
                                                    where rI.AccessoryPK == AccessoryPK
                                                    select rI).ToList();
                foreach (var restoredItem in restoredItems)
                {
                    Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);
                    List<Entry> entries = (from e in db.Entries
                                           where e.ItemPK == restoredItem.RestoredItemPK && e.IsRestored == true
                                           select e).ToList();
                    result.Add(new Client_RestoredItem_Angular(restoration, restoredItem.RestoredQuantity, storingDAO.EntriesQuantity(entries)));
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        ///// Manager API
        [Route("api/AngularController/GetRestorationWithFilter")]
        [HttpGet]
        public IHttpActionResult GetRestorationWithFilter(DateTime start, DateTime end)
        {
            List<Restoration> result = new List<Restoration>();
            // make it one more day to make sure < end will be right answer
            end = end.AddDays(1);
            try
            {
                // if start > 1900 then select query
                if (start.Year > 1900)
                {
                    result = (from re in db.Restorations
                              where re.DateCreated >= start && re.DateCreated <= end
                              select re).ToList();
                }
                // if start <= 1900 then select all
                else
                {
                    result = db.Restorations.ToList();
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_Restoration_Angular
        {
            public Client_Restoration_Angular()
            {
            }

            public Client_Restoration_Angular(Restoration restoration, string systemUserName)
            {
                RestorationPK = restoration.RestorationPK;
                RestorationID = restoration.RestorationID;
                DateCreated = restoration.DateCreated;
                IsReceived = restoration.IsReceived;
                UserID = restoration.UserID;
                SystemUserName = systemUserName;
                Comment = restoration.Comment;
            }

            public int RestorationPK { get; set; }

            public string RestorationID { get; set; }

            public DateTime DateCreated { get; set; }

            public bool IsReceived { get; set; }

            public string UserID { get; set; }

            public string SystemUserName { get; set; }

            public string Comment { get; set; }
        }

        [Route("api/AngularController/GetRestorationByPK")]
        [HttpGet]
        public IHttpActionResult GetRestorationByPK(int restorationPK)
        {
            Client_Restoration_Angular result;
            try
            {
                Restoration restoration = db.Restorations.Find(restorationPK);
                SystemUser systemUser = db.SystemUsers.Find(restoration.UserID);
                result = new Client_Restoration_Angular(restoration, systemUser.Name);
                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_ReceivingSession_Angular
        {
            public Client_ReceivingSession_Angular()
            {
            }

            public Client_ReceivingSession_Angular(ReceivingSession receivingSession, string systemUserName)
            {
                ReceivingSessionPK = receivingSession.ReceivingSessionPK;
                ExecutedDate = receivingSession.ExecutedDate;
                UserID = receivingSession.UserID;
                SystemUserName = systemUserName;
                RestorationPK = receivingSession.RestorationPK;
            }

            public int ReceivingSessionPK { get; set; }

            public DateTime ExecutedDate { get; set; }

            public string UserID { get; set; }

            public string SystemUserName { get; set; }

            public int RestorationPK { get; set; }

        }

        [Route("api/AngularController/GetReceivingSessionByRestorationPK")]
        [HttpGet]
        public IHttpActionResult GetReceivingSessionByRestorationPK(int restorationPK)
        {
            Client_ReceivingSession_Angular result;
            try
            {
                ReceivingSession receivingSession = (from Rss in db.ReceivingSessions
                                                     where Rss.RestorationPK == restorationPK
                                                     select Rss).FirstOrDefault();
                SystemUser systemUser = db.SystemUsers.Find(receivingSession.UserID);
                result = new Client_ReceivingSession_Angular(receivingSession, systemUser.Name);
                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_RestoredItem_Receiving_Angular
        {
            public Client_RestoredItem_Receiving_Angular()
            {
            }

            public Client_RestoredItem_Receiving_Angular(Accessory accessory, double restoredQuantity)
            {
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                RestoredQuantity = restoredQuantity;
            }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public double RestoredQuantity { get; set; }
        }

        [Route("api/AngularController/GetRestoredItemByRestorationPK")]
        [HttpGet]
        public IHttpActionResult GetRestoredItemByRestorationPK(int restorationPK)
        {
            List<Client_RestoredItem_Receiving_Angular> result = new List<Client_RestoredItem_Receiving_Angular>();
            try
            {
                List<RestoredItem> restoredItems = (from rI in db.RestoredItems.OrderByDescending(unit => unit.RestoredItemPK)
                                                    where rI.RestorationPK == restorationPK
                                                    select rI).ToList();
                foreach (var restoredItem in restoredItems)
                {
                    Accessory accessory = db.Accessories.Find(restoredItem.AccessoryPK);
                    result.Add(new Client_RestoredItem_Receiving_Angular(accessory, restoredItem.RestoredQuantity));
                }
                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_CountingSession_Angular
        {

            public Client_CountingSession_Angular(CountingSession countingSession, Accessory accessory, string packID, string boxID)
            {
                CountingSessionPK = countingSession.CountingSessionPK;
                ExecutedDate = countingSession.ExecutedDate;
                UserID = countingSession.UserID;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                PackID = packID;
                BoxID = boxID;
            }

            public Client_CountingSession_Angular(CountingSession countingSession, Accessory accessory, string packID, string boxID, string systemUserName, double identifiedQuantity)
            {
                CountingSessionPK = countingSession.CountingSessionPK;
                CountedQuantity = countingSession.CountedQuantity;
                IdentifiedQuantity = identifiedQuantity;
                ExecutedDate = countingSession.ExecutedDate;
                UserID = countingSession.UserID;
                SystemUserName = systemUserName;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                PackID = packID;
                BoxID = boxID;
            }

            public int CountingSessionPK { get; set; }

            public double CountedQuantity { get; set; }

            public double IdentifiedQuantity { get; set; }

            public DateTime ExecutedDate { get; set; }

            public string UserID { get; set; }

            public string SystemUserName { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public string PackID { get; set; }

            public string BoxID { get; set; }
        }

        [Route("api/AngularController/GetCountingListInspectingWithFilter")]
        [HttpGet]
        public IHttpActionResult GetCountingListInspectingWithFilter(DateTime start, DateTime end)
        {
            List<Client_CountingSession_Angular> result = new List<Client_CountingSession_Angular>();
            List<CountingSession> tempCountingSessions;
            // make it one more day to make sure < end will be right answer
            end = end.AddDays(1);
            try
            {
                // if start > 1900 then select query
                if (start.Year > 1900)
                {
                    tempCountingSessions = (from re in db.CountingSessions
                                            where re.ExecutedDate >= start && re.ExecutedDate <= end
                                            select re).ToList();
                }
                // if start <= 1900 then select all
                else
                {
                    tempCountingSessions = db.CountingSessions.ToList();
                }
                foreach (var ss in tempCountingSessions)
                {
                    IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);
                    // query box
                    UnstoredBox uBox = db.UnstoredBoxes.Find(identifiedItem.UnstoredBoxPK);
                    Box box = db.Boxes.Find(uBox.BoxPK);

                    // query pack
                    PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                    Pack pack = db.Packs.Find(packedItem.PackPK);

                    // query accessory
                    OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                    Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);

                    result.Add(new Client_CountingSession_Angular(ss, accessory, pack.PackID, box.BoxID));
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/AngularController/GetCountingSessionByPK")]
        [HttpGet]
        public IHttpActionResult GetCountingSessionByPK(int countingSessionPK)
        {
            Client_CountingSession_Angular result;
            try
            {
                CountingSession ss = db.CountingSessions.Find(countingSessionPK);

                IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);
                // query box
                UnstoredBox uBox = db.UnstoredBoxes.Find(identifiedItem.UnstoredBoxPK);
                Box box = db.Boxes.Find(uBox.BoxPK);
                string boxID = box.BoxID.Substring(0, box.BoxID.Length - 3);

                // query pack
                PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                Pack pack = db.Packs.Find(packedItem.PackPK);

                // query accessory
                OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);

                SystemUser systemUser = db.SystemUsers.Find(ss.UserID);

                result = new Client_CountingSession_Angular(ss, accessory, pack.PackID, box.BoxID, systemUser.Name, identifiedItem.IdentifiedQuantity);

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_PackedItem_Inspecting_Angular
        {
            public Client_PackedItem_Inspecting_Angular(string packID, Accessory accessory, PackedItem packedItem)
            {
                PackedItemPK = packedItem.PackedItemPK;
                PackID = packID;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                PackedQuantity = packedItem.PackedQuantity;
            }

            public int PackedItemPK { get; set; }

            public string PackID { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public double PackedQuantity { get; set; }
        }

        [Route("api/AngularController/GetPackedItemByCountingSessionPK")]
        [HttpGet]
        public IHttpActionResult GetPackedItemByCountingSessionPK(int countingSessionPK)
        {
            Client_PackedItem_Inspecting_Angular result;
            try
            {
                CountingSession ss = db.CountingSessions.Find(countingSessionPK);

                IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);

                // query pack
                PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                Pack pack = db.Packs.Find(packedItem.PackPK);

                // query accessory
                OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);

                SystemUser systemUser = db.SystemUsers.Find(ss.UserID);

                result = new Client_PackedItem_Inspecting_Angular(pack.PackID, accessory, packedItem);

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_CheckingSession_Angular
        {

            public Client_CheckingSession_Angular(CheckingSession checkingSession, Accessory accessory, string packID, string boxID)
            {
                CheckingSessionPK = checkingSession.CheckingSessionPK;
                ExecutedDate = checkingSession.ExecutedDate;
                UserID = checkingSession.UserID;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                PackID = packID;
                BoxID = boxID;
            }

            public Client_CheckingSession_Angular(CheckingSession checkingSession, Accessory accessory, string packID, string boxID, string systemUserName, double identifiedQuantity)
            {
                CheckingSessionPK = checkingSession.CheckingSessionPK;
                CheckedQuantity = checkingSession.CheckedQuantity;
                UnqualifiedQuantity = checkingSession.UnqualifiedQuantity;
                IdentifiedQuantity = identifiedQuantity;
                ExecutedDate = checkingSession.ExecutedDate;
                UserID = checkingSession.UserID;
                SystemUserName = systemUserName;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                PackID = packID;
                BoxID = boxID;
            }

            public int CheckingSessionPK { get; set; }

            public double CheckedQuantity { get; set; }

            public double UnqualifiedQuantity { get; set; }

            public double IdentifiedQuantity { get; set; }

            public DateTime ExecutedDate { get; set; }

            public string UserID { get; set; }

            public string SystemUserName { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public string PackID { get; set; }

            public string BoxID { get; set; }
        }


        [Route("api/AngularController/GetCheckingListInspectingWithFilter")]
        [HttpGet]
        public IHttpActionResult GetCheckingListInspectingWithFilter(DateTime start, DateTime end)
        {
            List<Client_CheckingSession_Angular> result = new List<Client_CheckingSession_Angular>();
            List<CheckingSession> tempCheckingSessions;
            // make it one more day to make sure < end will be right answer
            end = end.AddDays(1);
            try
            {
                // if start > 1900 then select query
                if (start.Year > 1900)
                {
                    tempCheckingSessions = (from re in db.CheckingSessions
                                            where re.ExecutedDate >= start && re.ExecutedDate <= end
                                            select re).ToList();
                }
                // if start <= 1900 then select all
                else
                {
                    tempCheckingSessions = db.CheckingSessions.ToList();
                }
                foreach (var ss in tempCheckingSessions)
                {
                    IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);
                    // query box
                    UnstoredBox uBox = db.UnstoredBoxes.Find(identifiedItem.UnstoredBoxPK);
                    Box box = db.Boxes.Find(uBox.BoxPK);

                    // query pack
                    PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                    Pack pack = db.Packs.Find(packedItem.PackPK);

                    // query accessory
                    OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                    Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);

                    result.Add(new Client_CheckingSession_Angular(ss, accessory, pack.PackID, box.BoxID));
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/AngularController/GetCheckingSessionByPK")]
        [HttpGet]
        public IHttpActionResult GetCheckingSessionByPK(int checkingSessionPK)
        {
            Client_CheckingSession_Angular result;
            try
            {
                CheckingSession ss = db.CheckingSessions.Find(checkingSessionPK);

                IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);
                // query box
                UnstoredBox uBox = db.UnstoredBoxes.Find(identifiedItem.UnstoredBoxPK);
                Box box = db.Boxes.Find(uBox.BoxPK);
                string boxID = box.BoxID.Substring(0, box.BoxID.Length - 3);

                // query pack
                PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                Pack pack = db.Packs.Find(packedItem.PackPK);

                // query accessory
                OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);

                SystemUser systemUser = db.SystemUsers.Find(ss.UserID);

                result = new Client_CheckingSession_Angular(ss, accessory, pack.PackID, box.BoxID, systemUser.Name, identifiedItem.IdentifiedQuantity);

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/AngularController/GetPackedItemByCheckingSessionPK")]
        [HttpGet]
        public IHttpActionResult GetPackedItemByCheckingSessionPK(int checkingSessionPK)
        {
            Client_PackedItem_Inspecting_Angular result;
            try
            {
                CheckingSession ss = db.CheckingSessions.Find(checkingSessionPK);

                IdentifiedItem identifiedItem = db.IdentifiedItems.Find(ss.IdentifiedItemPK);

                // query pack
                PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                Pack pack = db.Packs.Find(packedItem.PackPK);

                // query accessory
                OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);

                SystemUser systemUser = db.SystemUsers.Find(ss.UserID);

                result = new Client_PackedItem_Inspecting_Angular(pack.PackID, accessory, packedItem);

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_ClassifyingSession_Angular
        {
            public Client_ClassifyingSession_Angular()
            {

            }

            public Client_ClassifyingSession_Angular(ClassifyingSession classifyingSession, Accessory accessory, string packID, int qualityState, PackedItem packedItem)
            {
                CountedQuantity = 0;
                IdentifiedQuantity = 0;
                CheckedQuantity = 0;
                UnqualifiedQuantity = 0;
                FinalQuantity = 0;
                PackedItemPK = packedItem.PackedItemPK;
                ExecutedDate = classifyingSession.ExecutedDate;
                UserID = classifyingSession.UserID;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                PackID = packID;
                QualityState = qualityState;
            }

            public Client_ClassifyingSession_Angular(ClassifyingSession classifyingSession, Accessory accessory, string packID, int qualityState, string systemUserName, double finalQuantity)
            {
                CountedQuantity = 0;
                IdentifiedQuantity = 0;
                CheckedQuantity = 0;
                UnqualifiedQuantity = 0;
                FinalQuantity = 0;
                FinalQuantity = finalQuantity;
                ExecutedDate = classifyingSession.ExecutedDate;
                UserID = classifyingSession.UserID;
                SystemUserName = systemUserName;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                PackID = packID;
                QualityState = qualityState;
            }

            public Client_ClassifyingSession_Angular(Accessory accessory, string packID)
            {
                CountedQuantity = 0;
                IdentifiedQuantity = 0;
                CheckedQuantity = 0;
                UnqualifiedQuantity = 0;
                FinalQuantity = 0;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                PackID = packID;
                QualityState = 0;
            }

            public int PackedItemPK { get; set; }

            public double CountedQuantity { get; set; }

            public double IdentifiedQuantity { get; set; }

            public double CheckedQuantity { get; set; }

            public double UnqualifiedQuantity { get; set; }

            public double FinalQuantity { get; set; }

            public DateTime ExecutedDate { get; set; }

            public int QualityState { get; set; }

            public string UserID { get; set; }

            public string SystemUserName { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public string PackID { get; set; }

            public double Sample { get; set; }

            public double DefectLimit { get; set; }
        }

        [Route("api/AngularController/GetClassifyingListInspectingWithFilter")]
        [HttpGet]
        public IHttpActionResult GetClassifyingListInspectingWithFilter(DateTime start, DateTime end)
        {
            List<Client_ClassifyingSession_Angular> result = new List<Client_ClassifyingSession_Angular>();
            List<ClassifyingSession> tempClassifyingSessions;
            // make it one more day to make sure < end will be right answer
            end = end.AddDays(1);
            try
            {
                // if start > 1900 then select query
                if (start.Year > 1900)
                {
                    tempClassifyingSessions = (from re in db.ClassifyingSessions.OrderByDescending(unit => unit.ClassifyingSessionPK)
                                               where re.ExecutedDate >= start && re.ExecutedDate <= end
                                               select re).ToList();
                }
                // if start <= 1900 then select all
                else
                {
                    tempClassifyingSessions = db.ClassifyingSessions.ToList();
                }
                foreach (var ss in tempClassifyingSessions)
                {
                    // query classify item
                    ClassifiedItem classifiedItem = db.ClassifiedItems.Find(ss.ClassifiedItemPK);

                    // query pack
                    PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                    Pack pack = db.Packs.Find(packedItem.PackPK);

                    // query accessory
                    OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                    Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);

                    result.Add(new Client_ClassifyingSession_Angular(ss, accessory, pack.PackID, classifiedItem.QualityState, packedItem));
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/AngularController/GetClassifyingSessionByPK")]
        [HttpGet]
        public IHttpActionResult GetClassifyingSessionByPK(int packedItemPK)
        {
            Client_ClassifyingSession_Angular result = new Client_ClassifyingSession_Angular();
            try
            {
                // query pack
                PackedItem packedItem = db.PackedItems.Find(packedItemPK);
                Pack pack = db.Packs.Find(packedItem.PackPK);

                // query classify item
                ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                 where cI.PackedItemPK == packedItem.PackedItemPK
                                                 select cI).FirstOrDefault();

                // query accessory
                OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);

                if (classifiedItem != null)
                {

                    ClassifyingSession ss = (from Css in db.ClassifyingSessions
                                             where Css.ClassifiedItemPK == classifiedItem.ClassifiedItemPK
                                             select Css).FirstOrDefault();

                    SystemUser systemUser = db.SystemUsers.Find(ss.UserID);

                    result = new Client_ClassifyingSession_Angular(ss, accessory, pack.PackID, classifiedItem.QualityState, systemUser.Name,
                        new IdentifyItemDAO().FinalQuantity(packedItem.PackedItemPK));
                }
                else
                {
                    result = new Client_ClassifyingSession_Angular(accessory, pack.PackID);
                }

                // query lấy sum count and check
                List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                        where iI.PackedItemPK == packedItem.PackedItemPK
                                                        select iI).ToList();
                foreach (var identifiedItem in identifiedItems)
                {
                    if (identifiedItem.IsCounted)
                    {
                        CountingSession countingSession = (from Css in db.CountingSessions
                                                           where Css.IdentifiedItemPK == identifiedItem.IdentifiedItemPK
                                                           select Css).FirstOrDefault();
                        result.IdentifiedQuantity += identifiedItem.IdentifiedQuantity;
                        result.CountedQuantity += countingSession.CountedQuantity;
                    }
                    if (identifiedItem.IsChecked)
                    {
                        CheckingSession checkingSession = (from Css in db.CheckingSessions
                                                           where Css.IdentifiedItemPK == identifiedItem.IdentifiedItemPK
                                                           select Css).FirstOrDefault();
                        result.CheckedQuantity += checkingSession.CheckedQuantity;
                        result.UnqualifiedQuantity += checkingSession.UnqualifiedQuantity;
                    }
                }

                // sample & defect limit
                PackedItemsDAO dao = new PackedItemsDAO();
                dao.IsInitAllCalculate(packedItem.PackedItemPK);
                result.Sample = dao.Sample;
                result.DefectLimit = dao.DefectLimit;

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/AngularController/GetPackedItemByClassifyingSessionPK")]
        [HttpGet]
        public IHttpActionResult GetPackedItemByClassifyingSessionPK(int packedItemPK)
        {
            Client_PackedItem_Inspecting_Angular result;
            try
            {
                // query pack
                PackedItem packedItem = db.PackedItems.Find(packedItemPK);
                Pack pack = db.Packs.Find(packedItem.PackPK);

                // query accessory
                OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);

                result = new Client_PackedItem_Inspecting_Angular(pack.PackID, accessory, packedItem);

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/AngularController/GetRequestsWithFilter")]
        [HttpGet]
        public IHttpActionResult GetRequestsWithFilter(DateTime start, DateTime end)
        {
            List<Request> result = new List<Request>();
            // make it one more day to make sure < end will be right answer
            end = end.AddDays(1);
            try
            {
                // if start > 1900 then select query
                if (start.Year > 1900)
                {
                    result = (from re in db.Requests
                              where re.DateCreated >= start && re.DateCreated <= end
                              select re).ToList();
                }
                // if start <= 1900 then select all
                else
                {
                    result = db.Requests.ToList();
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/AngularController/GetRequestByPKIssuing")]
        [HttpGet]
        public IHttpActionResult GetRequestByPKIssuing(int requestPK)
        {
            Request result;
            try
            {
                result = db.Requests.Find(requestPK);
                SystemUser systemUser = db.SystemUsers.Find(result.UserID);
                result.UserID = systemUser.Name + " (" + result.UserID + ")";
                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_RequestedItem_Angular
        {
            public Client_RequestedItem_Angular()
            {

            }

            public Client_RequestedItem_Angular(Accessory accessory, RequestedItem requestedItem)
            {
                RequestedItemPK = requestedItem.RequestedItemPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                RequestedQuantity = requestedItem.RequestedQuantity;
            }

            public int RequestedItemPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public double RequestedQuantity { get; set; }
        }

        [Route("api/AngularController/GetRequestedItemsByRequestPKIssuing")]
        [HttpGet]
        public IHttpActionResult GetRequestedItemsByRequestPKIssuing(int requestPK)
        {
            List<Client_RequestedItem_Angular> result = new List<Client_RequestedItem_Angular>();
            try
            {
                List<RequestedItem> temp = (from rI in db.RequestedItems
                                            where rI.RequestPK == requestPK
                                            select rI).ToList();
                foreach (var requestedItem in temp)
                {
                    DemandedItem demandedItem = db.DemandedItems.Find(requestedItem.DemandedItemPK);
                    Accessory accessory = db.Accessories.Find(demandedItem.AccessoryPK);
                    result.Add(new Client_RequestedItem_Angular(accessory, requestedItem));
                }
                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_Session_Activity_Angular
        {
            public Client_Session_Activity_Angular()
            {
            }

            public Client_Session_Activity_Angular(DateTime executedDate, string userID, string content)
            {
                ExecutedDate = executedDate;
                UserID = userID;
                Content = content;
            }

            public DateTime ExecutedDate { get; set; }

            public string UserID { get; set; }

            public string Content { get; set; }
        }

        [Route("api/AngularController/GetSessionsWithFilter")]
        [HttpGet]
        public IHttpActionResult GetSessionsWithFilter(DateTime start, DateTime end, int sessionNum)
        {
            List<Client_Session_Activity_Angular> result = new List<Client_Session_Activity_Angular>();
            AngularDAO angularDAO = new AngularDAO();
            // make it one more day to make sure < end will be right answer
            end = end.AddDays(1);
            try
            {
                // if start > 1900 then select query
                if (start.Year > 1900)
                {
                    if (sessionNum == 12)
                    {
                        for (int i = 1; i <= 6; i++)
                        {
                            result.AddRange(angularDAO.GetSessions(start, end, i));
                        }
                    }
                    else if (sessionNum == 13)
                    {
                        for (int i = 7; i <= 11; i++)
                        {
                            result = angularDAO.GetSessions(start, end, i);
                        }
                    }
                    else
                    {
                        result = angularDAO.GetSessions(start, end, sessionNum);
                    }
                }
                // if start <= 1900 then select all
                else
                {
                    if (sessionNum == 12)
                    {
                        for (int i = 1; i <= 6; i++)
                        {
                            result.AddRange(angularDAO.GetSessions(i));
                        }
                    }
                    else if (sessionNum == 13)
                    {
                        for (int i = 7; i <= 11; i++)
                        {
                            result.AddRange(angularDAO.GetSessions(i));
                        }
                    }
                    else
                    {
                        result = angularDAO.GetSessions(sessionNum);
                    }
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_Session_Verification_Angular
        {
            public Client_Session_Verification_Angular(int sessionPK, string userID, DateTime executedDate, double quantity, bool isVerified, bool isDiscard)
            {
                SessionPK = sessionPK;
                UserID = userID;
                ExecutedDate = executedDate;
                Quantity = quantity;
                IsVerified = isVerified;
                IsDiscard = isDiscard;
            }

            public int SessionPK { get; set; }

            public string UserID { get; set; }

            public DateTime ExecutedDate { get; set; }

            public double Quantity { get; set; }

            public bool IsVerified { get; set; }

            public bool IsDiscard { get; set; }
        }

        [Route("api/AngularController/GetSessionsWithFilterVerification")]
        [HttpGet]
        public IHttpActionResult GetSessionsWithFilterVerification(DateTime start, DateTime end, int sessionNum)
        {
            List<Client_Session_Verification_Angular> result = new List<Client_Session_Verification_Angular>();
            AngularDAO angularDAO = new AngularDAO();
            StoringDAO storingDAO = new StoringDAO();
            // make it one more day to make sure < end will be right answer
            end = end.AddDays(1);
            try
            {
                // if start > 1900 then select query
                if (start.Year > 1900)
                {
                    // adjust
                    if (sessionNum == 1)
                    {
                        List<AdjustingSession> adjustingSessions = (from Ass in db.AdjustingSessions
                                                                    where Ass.ExecutedDate >= start && Ass.ExecutedDate <= end
                                                                    select Ass).ToList();
                        foreach (var ss in adjustingSessions)
                        {
                            SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                            Entry entry = (from e in db.Entries
                                           where e.SessionPK == ss.AdjustingSessionPK
                                           && (e.KindRoleName == "AdjustingMinus"
                                           || e.KindRoleName == "AdjustingPlus")
                                           select e).FirstOrDefault();
                            bool isDiscard = false;
                            double adjustedQuantity = storingDAO.EntryQuantity(entry);
                            result.Add(new Client_Session_Verification_Angular(ss.AdjustingSessionPK, systemUser.Name + " (" + ss.UserID + ")",
                            ss.ExecutedDate, adjustedQuantity, ss.IsVerified, isDiscard));
                        }
                    }
                    // discard
                    else
                    {
                        List<DiscardingSession> discardingSessions = (from Dss in db.DiscardingSessions
                                                                      where Dss.ExecutedDate >= start && Dss.ExecutedDate <= end
                                                                      select Dss).ToList();
                        foreach (var ss in discardingSessions)
                        {
                            SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                            List<Entry> entries = (from e in db.Entries
                                                   where e.SessionPK == ss.DiscardingSessionPK
                                                   && e.KindRoleName == "Discarding"
                                                   select e).ToList();
                            bool isDiscard = true;
                            result.Add(new Client_Session_Verification_Angular(ss.DiscardingSessionPK, systemUser.Name + " (" + ss.UserID + ")",
                            ss.ExecutedDate, storingDAO.EntriesQuantity(entries), ss.IsVerified, isDiscard));
                        }
                    }
                }
                // if start <= 1900 then select all
                else
                {
                    // adjust
                    if (sessionNum == 1)
                    {
                        List<AdjustingSession> adjustingSessions = (from Ass in db.AdjustingSessions
                                                                    select Ass).ToList();
                        foreach (var ss in adjustingSessions)
                        {
                            SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                            Entry entry = (from e in db.Entries
                                           where e.SessionPK == ss.AdjustingSessionPK
                                           && (e.KindRoleName == "AdjustingMinus"
                                           || e.KindRoleName == "AdjustingPlus")
                                           select e).FirstOrDefault();
                            bool isDiscard = false;
                            double adjustedQuantity = storingDAO.EntryQuantity(entry);
                            result.Add(new Client_Session_Verification_Angular(ss.AdjustingSessionPK, systemUser.Name + " (" + ss.UserID + ")",
                            ss.ExecutedDate, adjustedQuantity, ss.IsVerified, isDiscard));
                        }
                    }
                    // discard
                    else
                    {
                        List<DiscardingSession> discardingSessions = (from Dss in db.DiscardingSessions
                                                                      select Dss).ToList();
                        foreach (var ss in discardingSessions)
                        {
                            SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                            List<Entry> entries = (from e in db.Entries
                                                   where e.SessionPK == ss.DiscardingSessionPK
                                                   && e.KindRoleName == "Discarding"
                                                   select e).ToList();
                            bool isDiscard = true;
                            result.Add(new Client_Session_Verification_Angular(ss.DiscardingSessionPK, systemUser.Name + " (" + ss.UserID + ")",
                            ss.ExecutedDate, (storingDAO.EntriesQuantity(entries)), ss.IsVerified, isDiscard));
                        }
                    }
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_OrderedItem_Receiving_Angular
        {
            public Client_OrderedItem_Receiving_Angular(int orderedItemPK, Accessory accessory)
            {
                OrderedItemPK = orderedItemPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
            }

            public int OrderedItemPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }
        }

        [Route("api/AngularController/GetAccessoriesByOrderPK")]
        [HttpGet]
        public IHttpActionResult GetAccessoriesByOrderPK(int orderPK)
        {
            List<Client_OrderedItem_Receiving_Angular> result = new List<Client_OrderedItem_Receiving_Angular>();
            try
            {
                Order order = db.Orders.Find(orderPK);
                if (order == null)
                {
                    return Content(HttpStatusCode.Conflict, "ORDER KHÔNG TỒN TẠI");
                }

                List<OrderedItem> orderedItems = (from oI in db.OrderedItems
                                                  where oI.OrderPK == orderPK
                                                  select oI).ToList();
                foreach (var item in orderedItems)
                {
                    Accessory accessory = db.Accessories.Find(item.AccessoryPK);
                    result.Add(new Client_OrderedItem_Receiving_Angular(item.OrderedItemPK, accessory));
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class Client_PackedItem_Receiving_Angular
        {
            public Client_PackedItem_Receiving_Angular(Accessory accessory, PackedItem packedItem, double sumIdentifiedQuantity, double finalQuantity)
            {
                PackedItemPK = packedItem.PackedItemPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                PackedQuantity = packedItem.PackedQuantity;
                SumIdentifiedQuantity = sumIdentifiedQuantity;
                FinalQuantity = finalQuantity;
            }

            public int PackedItemPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public double PackedQuantity { get; set; }

            public double SumIdentifiedQuantity { get; set; }

            public double FinalQuantity { get; set; }
            // pack property
            public bool IsOpened { get; set; }
            // classifiedItem property
            public bool IsClassified { get; set; }
        }

        [Route("api/AngularController/GetPackedItemByPK")]
        [HttpGet]
        public IHttpActionResult GetPackedItemByPK(int packedItemPK)
        {
            Client_PackedItem_Receiving_Angular result;
            try
            {
                PackedItem packedItem = db.PackedItems.Find(packedItemPK);
                if (packedItem == null)
                {
                    return Content(HttpStatusCode.Conflict, "ORDER KHÔNG TỒN TẠI");
                }
                OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);

                List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                        where iI.PackedItemPK == packedItem.PackedItemPK
                                                        select iI).ToList();
                double sumIdentifiedQuantity = 0;
                foreach (var item in identifiedItems)
                {
                    sumIdentifiedQuantity += item.IdentifiedQuantity;
                }

                result = new Client_PackedItem_Receiving_Angular(accessory, packedItem, sumIdentifiedQuantity
                    , new IdentifyItemDAO().FinalQuantity(packedItem.PackedItemPK));

                // query isopened and isclassified
                Pack pack = db.Packs.Find(packedItem.PackPK);
                result.IsOpened = pack.IsOpened;

                ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                 where cI.PackedItemPK == packedItem.PackedItemPK
                                                 select cI).FirstOrDefault();
                if (classifiedItem != null)
                {
                    result.IsClassified = true;
                }
                else
                {
                    result.IsClassified = false;
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class Client_IdentifiedItem_Receiving_Angular
        {
            public Client_IdentifiedItem_Receiving_Angular(string boxID, string userID, DateTime executedDate, IdentifiedItem identifiedItem)
            {
                BoxID = boxID;
                IdentifiedQuantity = identifiedItem.IdentifiedQuantity;
                UserID = userID;
                ExecutedDate = executedDate;
                IsChecked = identifiedItem.IsChecked;
                IsCounted = identifiedItem.IsCounted;
            }

            public string BoxID { get; set; }

            public double IdentifiedQuantity { get; set; }

            public string UserID { get; set; }

            public DateTime ExecutedDate { get; set; }

            public int CheckingSessionPK { get; set; }

            public bool IsChecked { get; set; }

            public int CountingSessionPK { get; set; }

            public bool IsCounted { get; set; }
        }

        [Route("api/AngularController/GetIdentifiedItemByPackedItemPK")]
        [HttpGet]
        public IHttpActionResult GetIdentifiedItemByPackedItemPK(int packedItemPK)
        {
            List<Client_IdentifiedItem_Receiving_Angular> result = new List<Client_IdentifiedItem_Receiving_Angular>();
            try
            {
                PackedItem packedItem = db.PackedItems.Find(packedItemPK);
                if (packedItem == null)
                {
                    return Content(HttpStatusCode.Conflict, "ORDER KHÔNG TỒN TẠI");
                }
                OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);
                Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);

                List<IdentifiedItem> identifiedItems = (from iI in db.IdentifiedItems
                                                        where iI.PackedItemPK == packedItem.PackedItemPK
                                                        select iI).ToList();

                foreach (var item in identifiedItems)
                {
                    IdentifyingSession ss = db.IdentifyingSessions.Find(item.IdentifyingSessionPK);
                    UnstoredBox uBox = db.UnstoredBoxes.Find(item.UnstoredBoxPK);
                    Box box = db.Boxes.Find(uBox.BoxPK);

                    SystemUser systemUser = db.SystemUsers.Find(ss.UserID);

                    result.Add(new Client_IdentifiedItem_Receiving_Angular(box.BoxID, systemUser.Name + " (" + ss.UserID + ")", ss.ExecutedDate, item));
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class Client_Box_Information_Angular
        {
            public Client_Box_Information_Angular(Box box, string status, int itemQuantity)
            {
                BoxPK = box.BoxPK;
                BoxID = box.BoxID;
                DateCreated = box.DateCreated;
                Status = status;
                ItemQuantity = itemQuantity;
            }

            public int BoxPK { get; set; }

            public string BoxID { get; set; }

            public DateTime DateCreated { get; set; }

            public string Status { get; set; }

            public int ItemQuantity { get; set; }
        }


        [Route("api/AngularController/GetBoxesInformation")]
        [HttpGet]
        public IHttpActionResult GetBoxesInformation()
        {
            BoxDAO boxDAO = new BoxDAO();
            StoringDAO storingDAO = new StoringDAO();
            List<Client_Box_Information_Angular> result = new List<Client_Box_Information_Angular>();
            try
            {
                List<Box> boxes = db.Boxes.Where(unit => unit.IsActive == true && unit.BoxID != "InvisibleBox")
                    .OrderByDescending(unit => unit.BoxPK).ToList();
                foreach (var box in boxes)
                {
                    StoredBox sBox = boxDAO.GetStoredBoxbyBoxPK(box.BoxPK);
                    UnstoredBox uBox = boxDAO.GetUnstoredBoxbyBoxPK(box.BoxPK);
                    // Nếu box đã identify
                    if (uBox.IsIdentified)
                    {
                        // nếu box chưa được store
                        if (!(boxDAO.IsStored(box.BoxPK)))
                        {
                            List<Client_IdentifiedItemRead> client_IdentifiedItems = new List<Client_IdentifiedItemRead>();
                            List<IdentifiedItem> identifiedItems;
                            identifiedItems = (from iI in db.IdentifiedItems.OrderByDescending(unit => unit.PackedItemPK)
                                               where iI.UnstoredBoxPK == uBox.UnstoredBoxPK
                                               select iI).ToList();

                            foreach (var identifiedItem in identifiedItems)
                            {
                                PackedItem packedItem = db.PackedItems.Find(identifiedItem.PackedItemPK);
                                // lấy pack ID
                                Pack pack = db.Packs.Find(packedItem.PackPK);

                                // lấy phụ liệu tương ứng
                                OrderedItem orderedItem = db.OrderedItems.Find(packedItem.OrderedItemPK);

                                Accessory accessory = db.Accessories.Find(orderedItem.AccessoryPK);
                                // lấy qualityState
                                ClassifiedItem classifiedItem = (from cI in db.ClassifiedItems
                                                                 where cI.PackedItemPK == packedItem.PackedItemPK
                                                                 select cI).FirstOrDefault();
                                int? qualityState = null;
                                if (classifiedItem != null) qualityState = classifiedItem.QualityState;
                                client_IdentifiedItems.Add(new Client_IdentifiedItemRead(identifiedItem, accessory, pack.PackID, qualityState));
                            }
                            result.Add(new Client_Box_Information_Angular(box, "THÙNG ĐÃ GHI NHẬN", identifiedItems.Count));
                        }
                        else
                        {
                            Client_Shelf client_Shelf;
                            List<Client_InBoxItem> client_InBoxItems = new List<Client_InBoxItem>();

                            Shelf shelf = db.Shelves.Find(sBox.ShelfPK);
                            Row row = db.Rows.Find(shelf.RowPK);
                            client_Shelf = new Client_Shelf(shelf.ShelfID, row.RowID);

                            // Get list inBoxItem
                            List<Entry> entries = (from e in db.Entries
                                                   where e.StoredBoxPK == sBox.StoredBoxPK
                                                   select e).ToList();
                            HashSet<KeyValuePair<int, bool>> listItemPK = new HashSet<KeyValuePair<int, bool>>();
                            foreach (var entry in entries)
                            {
                                listItemPK.Add(new KeyValuePair<int, bool>(entry.ItemPK, entry.IsRestored));
                            }
                            foreach (var itemPK in listItemPK)
                            {
                                List<Entry> tempEntries = new List<Entry>();
                                foreach (var entry in entries)
                                {
                                    if (entry.ItemPK == itemPK.Key && entry.IsRestored == itemPK.Value) tempEntries.Add(entry);
                                }
                                if (tempEntries.Count > 0 && storingDAO.EntriesQuantity(tempEntries) > 0)
                                {
                                    Entry entry = tempEntries[0];
                                    PassedItem passedItem;
                                    RestoredItem restoredItem;
                                    if (entry.IsRestored)
                                    {
                                        restoredItem = db.RestoredItems.Find(entry.ItemPK);
                                        Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);
                                        Accessory accessory = db.Accessories.Find(restoredItem.AccessoryPK);
                                        client_InBoxItems.Add(new Client_InBoxItem(accessory, restoration.RestorationID, storingDAO.EntriesQuantity(tempEntries), restoredItem.RestoredItemPK, true));
                                    }
                                    else
                                    {
                                        passedItem = db.PassedItems.Find(entry.ItemPK);
                                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(passedItem.ClassifiedItemPK);
                                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                                        // lấy pack ID
                                        Pack pack = (from p in db.Packs
                                                     where p.PackPK == packedItem.PackPK
                                                     select p).FirstOrDefault();

                                        // lấy phụ liệu tương ứng
                                        OrderedItem orderedItem = (from oI in db.OrderedItems
                                                                   where oI.OrderedItemPK == packedItem.OrderedItemPK
                                                                   select oI).FirstOrDefault();

                                        Accessory accessory = (from a in db.Accessories
                                                               where a.AccessoryPK == orderedItem.AccessoryPK
                                                               select a).FirstOrDefault();
                                        client_InBoxItems.Add(new Client_InBoxItem(accessory, pack.PackID, storingDAO.EntriesQuantity(tempEntries), passedItem.PassedItemPK, false));
                                    }
                                }
                            }

                            result.Add(new Client_Box_Information_Angular(box, "THÙNG TRONG KHO", client_InBoxItems.Count));
                        }
                    }
                    else
                    {
                        result.Add(new Client_Box_Information_Angular(box, "THÙNG TRẮNG", 0));
                    }
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/AngularController/GetActivitiesWithFilter")]
        [HttpGet]
        public IHttpActionResult GetActivitiesWithFilter(DateTime start, DateTime end, string actionType, string objectKind)
        {
            List<Activity> result = new List<Activity>();
            // make it one more day to make sure < end will be right answer
            end = end.AddDays(1);
            try
            {
                // if start > 1900 then select query
                if (start.Year > 1900)
                {
                    result = db.Activities.Where(act => act.Action == actionType && act.Object == objectKind
                    && act.ExecutedDate >= start && act.ExecutedDate <= end).ToList();
                }
                // if start <= 1900 then select all
                else
                {
                    result = db.Activities.Where(act => act.Action == actionType && act.Object == objectKind).ToList();
                }
                foreach (var item in result)
                {
                    SystemUser systemUser = db.SystemUsers.Find(item.UserID);
                    item.UserID = systemUser.Name + " (" + item.UserID + ")";
                }
                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_AccessoryID_Item_Angular
        {
            public List<string> accessoryID { get; set; }

            public List<string> item { get; set; }
        }

        [Route("api/AngularController/GetAccessoryByAccessoryIDAndItemImportExcel")]
        [HttpPost]
        public IHttpActionResult GetAccessoryByAccessoryIDAndItemImportExcel(Client_AccessoryID_Item_Angular list)
        {
            try
            {
                List<Accessory> result = new List<Accessory>();
                for (int i = 0; i < list.accessoryID.Count; i++)
                {
                    if (list.item[i] != null)
                    {
                        string str = list.item[i];
                        Accessory temp = db.Accessories.Where(acc => acc.Item == str).FirstOrDefault();
                        if (temp == null && list.accessoryID[i] != null)
                        {
                            str = list.accessoryID[i];
                            temp = db.Accessories.Where(acc => acc.AccessoryID == str).FirstOrDefault();
                        }
                        if (temp != null)
                        {
                            result.Add(temp);
                        }
                    }
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_InstoredItem_Angular
        {
            public Client_InstoredItem_Angular(string boxID, string containerID, bool isRestored, Accessory accessory)
            {
                BoxID = boxID;
                ContainerID = containerID;
                IsRestored = isRestored;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
            }

            public string BoxID { get; set; }

            public string ContainerID { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public bool IsRestored { get; set; }
        }

        [Route("api/AngularController/GetInstoredItemBySessionPKVerification")]
        [HttpGet]
        public IHttpActionResult GetInstoredItemBySessionPKVerification(int sessionPK, int sessionNum)
        {
            try
            {
                Client_InstoredItem_Angular result;
                if (sessionNum == 1)
                {
                    AdjustingSession ss = db.AdjustingSessions.Find(sessionPK);
                    Entry entry = (from e in db.Entries
                                   where e.SessionPK == ss.AdjustingSessionPK
                                   && (e.KindRoleName == "AdjustingMinus"
                                   || e.KindRoleName == "AdjustingPlus")
                                   select e).FirstOrDefault();

                    //  query Accessory
                    Accessory accessory = db.Accessories.Find(entry.AccessoryPK);

                    //  query BoxID
                    StoredBox sBox = db.StoredBoxes.Find(entry.StoredBoxPK);
                    Box box = db.Boxes.Find(sBox.BoxPK);

                    //  query ContainerID
                    string containerID;
                    if (entry.IsRestored)
                    {
                        RestoredItem restoredItem = db.RestoredItems.Find(entry.ItemPK);
                        Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);
                        containerID = restoration.RestorationID;
                    }
                    else
                    {
                        PassedItem passedItem = db.PassedItems.Find(entry.ItemPK);
                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(passedItem.ClassifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        containerID = pack.PackID;
                    }

                    result = new Client_InstoredItem_Angular(box.BoxID, containerID, entry.IsRestored, accessory);
                }
                else
                {
                    DiscardingSession ss = db.DiscardingSessions.Find(sessionPK);
                    Entry entry = (from e in db.Entries
                                   where e.SessionPK == ss.DiscardingSessionPK
                                   && (e.KindRoleName == "Discarding")
                                   select e).FirstOrDefault();

                    //  query Accessory
                    Accessory accessory = db.Accessories.Find(entry.AccessoryPK);

                    //  query BoxID
                    StoredBox sBox = db.StoredBoxes.Find(entry.StoredBoxPK);
                    Box box = db.Boxes.Find(sBox.BoxPK);

                    //  query ContainerID
                    string containerID;
                    if (entry.IsRestored)
                    {
                        RestoredItem restoredItem = db.RestoredItems.Find(entry.ItemPK);
                        Restoration restoration = db.Restorations.Find(restoredItem.RestorationPK);
                        containerID = restoration.RestorationID;
                    }
                    else
                    {
                        PassedItem passedItem = db.PassedItems.Find(entry.ItemPK);
                        ClassifiedItem classifiedItem = db.ClassifiedItems.Find(passedItem.ClassifiedItemPK);
                        PackedItem packedItem = db.PackedItems.Find(classifiedItem.PackedItemPK);
                        Pack pack = db.Packs.Find(packedItem.PackPK);
                        containerID = pack.PackID;
                    }

                    result = new Client_InstoredItem_Angular(box.BoxID, containerID, entry.IsRestored, accessory);
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        public class Client_Session_Detail_Verification_Angular
        {
            public Client_Session_Detail_Verification_Angular(double initialQuantity, double quantity, string userID, DateTime executedDate, bool isVerified)
            {
                InitialQuantity = initialQuantity;
                Quantity = quantity;
                UserID = userID;
                ExecutedDate = executedDate;
                IsVerified = isVerified;
            }

            public double InitialQuantity { get; set; }

            public double Quantity { get; set; }

            public string UserID { get; set; }

            public DateTime ExecutedDate { get; set; }

            public bool IsVerified { get; set; }
        }

        [Route("api/AngularController/GetSessionBySessionPKVerification")]
        [HttpGet]
        public IHttpActionResult GetSessionBySessionPKVerification(int sessionPK, int sessionNum)
        {
            try
            {
                Client_Session_Detail_Verification_Angular result;
                if (sessionNum == 1)
                {
                    AdjustingSession ss = db.AdjustingSessions.Find(sessionPK);
                    Entry entry = (from e in db.Entries
                                   where e.SessionPK == ss.AdjustingSessionPK
                                   && (e.KindRoleName == "AdjustingMinus"
                                   || e.KindRoleName == "AdjustingPlus")
                                   select e).FirstOrDefault();
                    double initialQuantity = 0;

                    List<Entry> entries = db.Entries.Where(unit => unit.ItemPK == entry.ItemPK).ToList();
                    initialQuantity = new StoringDAO().EntriesQuantity(entries);

                    SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                    result = new Client_Session_Detail_Verification_Angular(initialQuantity, new StoringDAO().EntryQuantity(entry)
                        , systemUser.Name + " (" + ss.UserID + ")", ss.ExecutedDate, ss.IsVerified);
                }
                else
                {
                    DiscardingSession ss = db.DiscardingSessions.Find(sessionPK);
                    Entry entry = (from e in db.Entries
                                   where e.SessionPK == ss.DiscardingSessionPK
                                   && (e.KindRoleName == "Discarding")
                                   select e).FirstOrDefault();
                    double initialQuantity = 0;

                    List<Entry> entries = db.Entries.Where(unit => unit.ItemPK == entry.ItemPK).ToList();
                    initialQuantity = new StoringDAO().EntriesQuantity(entries);

                    SystemUser systemUser = db.SystemUsers.Find(ss.UserID);
                    result = new Client_Session_Detail_Verification_Angular(initialQuantity, new StoringDAO().EntryQuantity(entry)
                        , systemUser.Name + " (" + ss.UserID + ")", ss.ExecutedDate, ss.IsVerified);

                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }


        [Route("api/AngularController/GetVerificationBySessionPKVerification")]
        [HttpGet]
        public IHttpActionResult GetVerificationBySessionPKVerification(int sessionPK, int sessionNum)
        {
            try
            {
                Verification result;
                if (sessionNum == 1)
                {
                    result = db.Verifications.Where(unit => unit.SessionPK == sessionPK && unit.IsDiscard == false).FirstOrDefault();
                    if (result != null)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(result.UserID);
                        result.UserID = systemUser.Name + " (" + systemUser.UserID + ")";
                    }
                }
                else
                {
                    result = db.Verifications.Where(unit => unit.SessionPK == sessionPK && unit.IsDiscard == true).FirstOrDefault();
                    if (result != null)
                    {
                        SystemUser systemUser = db.SystemUsers.Find(result.UserID);
                        result.UserID = systemUser.Name + " (" + systemUser.UserID + ")";
                    }
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }


        // QR CODE GENERATOR

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public class FileResult : IHttpActionResult
        {
            private readonly string filePath;
            private readonly string contentType;

            public FileResult(string filePath, string contentType)
            {
                this.filePath = filePath;
                this.contentType = contentType;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.Run(() =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(File.OpenRead(filePath))
                    };
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "stamp.pdf"
                    };
                    //var contentType = this.contentType ?? MimeMapping.GetMimeMapping(Path.GetExtension(filePath));
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(this.contentType);

                    return response;
                }, cancellationToken);
            }
        }

        [Route("api/AngularController/GetQRCODE")]
        [HttpGet]
        public IHttpActionResult GetQRCODE(string id)
        {
            try
            {
                // generate qr code
                var image = HttpContext.Current.Server.MapPath("~/Image");
                var imgPath = Path.Combine(image, "logo.png");
                var imgSavePath = Path.Combine(image, "temp.png");
                string hash = Base64Encode(id);
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(hash, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(imgPath));

                if (File.Exists(imgSavePath))
                {
                    File.Delete(imgSavePath);
                }
                qrCodeImage.Save(imgSavePath, System.Drawing.Imaging.ImageFormat.Png);
                qrCodeImage.Dispose();


                // excel
                var excel = HttpContext.Current.Server.MapPath("~/ExcelSheets");
                // chọn file excel ứng với loại id
                //if (id.Contains("box"))
                //{

                //}
                //else if (id.Contains("shelf"))
                //{

                //}
                //else if (id.Contains("row"))
                //{

                //}
                var excelPath = Path.Combine(excel, "stampSample.xlsx");

                Application excelApp = new Application();
                Workbook wb;
                Worksheet ws;
                wb = excelApp.Workbooks.Open(excelPath);
                ws = wb.Worksheets[1];
                Microsoft.Office.Interop.Excel.Range oRange = (Microsoft.Office.Interop.Excel.Range)ws.Cells[15, 2];
                float Left = (float)((double)oRange.Left + 25);
                float Top = (float)((double)oRange.Top + 5);
                const float ImageSize = 420;
                ws.Shapes.AddPicture(imgSavePath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, ImageSize, ImageSize);
                ws.PageSetup.BottomMargin = 0;
                ws.PageSetup.FooterMargin = 0;
                ws.PageSetup.HeaderMargin = 0;
                ws.PageSetup.LeftMargin = 0;
                ws.PageSetup.RightMargin = 0;
                ws.PageSetup.TopMargin = 0;

                //if (id.Contains("box"))
                //{

                //}
                //else if (id.Contains("shelf"))
                //{

                //}
                //else if (id.Contains("row"))
                //{

                //}
                var stamp = Path.Combine(excel, "stamp.xlsx");
                if (File.Exists(stamp))
                {
                    File.Delete(stamp);
                }
                wb.SaveAs(stamp);

                // pdf
                var pdfPath = Path.Combine(excel, "stamp.pdf");

                object misValue = System.Reflection.Missing.Value;
                XlFixedFormatType paramExportFormat = XlFixedFormatType.xlTypePDF;
                XlFixedFormatQuality paramExportQuality = XlFixedFormatQuality.xlQualityStandard;
                bool paramIgnorePrintAreas = false;

                wb.ExportAsFixedFormat(paramExportFormat, pdfPath, paramExportQuality, true, paramIgnorePrintAreas, 1, 1, false, misValue);

                // close excel
                wb.Close();
                excelApp.Quit();

                return new FileResult(pdfPath, "application/octet-stream");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.OK, e);
            }
        }
    }
}

