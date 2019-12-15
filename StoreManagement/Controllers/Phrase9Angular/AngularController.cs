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

            public int ConceptionPK { get; set; }
            public string ConceptionFullKey { get; set; }
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
                    string temp = conception.ConceptionCode + "-" + conception.Season + (conception.Year + "").Substring(2);
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
            List<Accessory> result;
            try
            {
                result = (from acc in db.Accessories.OrderByDescending(unit => unit.AccessoryPK)
                          where acc.CustomerPK == customerPK && acc.IsActive
                          select acc).ToList();
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
                    string temp = conception.ConceptionCode + "-" + conception.Season + (conception.Year + "").Substring(2);
                    result.Add(new Client_ConceptionForAccessoryDetail(conception.ConceptionPK, temp));
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

