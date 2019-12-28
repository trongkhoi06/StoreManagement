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
    public class InformationController : ApiController
    {
        private UserModel db = new UserModel();

        [Route("api/InformationController/GetAllCustomer")]
        [HttpGet]
        public IHttpActionResult GetAllCustomer()
        {
            List<Customer> result;
            try
            {
                result = db.Customers.ToList();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        public class Client_Accessory_Information
        {
            public Client_Accessory_Information(Accessory accessory, string accessoryTypeName)
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

            public int CustomerPK { get; set; }
        }

        [Route("api/InformationController/GetAllAccessory")]
        [HttpGet]
        public IHttpActionResult GetAllAccessory()
        {
            List<Client_Accessory_Information> result = new List<Client_Accessory_Information>();
            try
            {
                List<Accessory> accessories = db.Accessories.ToList();
                foreach (var accessory in accessories)
                {
                    AccessoryType accessoryType = db.AccessoryTypes.Find(accessory.AccessoryTypePK);
                    result.Add(new Client_Accessory_Information(accessory, accessoryType.Name));
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
            return Content(HttpStatusCode.OK, result);
        }

        [Route("api/InformationController/CreateCustomer")]
        [HttpPost]
        public IHttpActionResult CreateCustomer(string name, string code, string address, string phoneNumber, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    if (address == "undefined") address = "";
                    if (phoneNumber == "undefined") phoneNumber = "";
                    informationDAO.CreateCustomer(name, code, address, phoneNumber, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "TẠO KHÁCH HÀNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/EditCustomer")]
        [HttpPut]
        public IHttpActionResult EditCustomer(int customerPK, string address, string phoneNumber, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.UpdateCustomer(customerPK, address, phoneNumber, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "CHỈNH SỬA THÔNG TIN KHÁCH HÀNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/DeleteCustomer")]
        [HttpDelete]
        public IHttpActionResult DeleteCustomer(int customerPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.DeleteCustomer(customerPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "XÓA KHÁCH HÀNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/DeactiveCustomer")]
        [HttpPut]
        public IHttpActionResult DeactiveCustomer(int customerPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.DeactiveCustomer(customerPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "ẨN KHÁCH HÀNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/ActiveCustomer")]
        [HttpPut]
        public IHttpActionResult ActiveCustomer(int customerPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.ActiveCustomer(customerPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "HIỆN KHÁCH HÀNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/CreateConception")]
        [HttpPost]
        public IHttpActionResult CreateConception(int customerPK, string conceptionCode, int year, string season, string description, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.CreateConception(customerPK, conceptionCode, year, season, description, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "TẠO MÃ HÀNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/DeleteConception")]
        [HttpDelete]
        public IHttpActionResult DeleteConception(int conceptionPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.DeleteConception(conceptionPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "XÓA MÃ HÀNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/DeactiveConception")]
        [HttpPut]
        public IHttpActionResult DeactiveConception(int conceptionPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.DeactiveConception(conceptionPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "ẨN MÃ HÀNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/ActiveConception")]
        [HttpPut]
        public IHttpActionResult ActiveConception(int conceptionPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.ActiveConception(conceptionPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "HIỆN MÃ HÀNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/CreateSupplier")]
        [HttpPost]
        public IHttpActionResult CreateSupplier(string name, string code, string address, string phoneNumber, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    if (address == "undefined") address = "";
                    if (phoneNumber == "undefined") phoneNumber = "";
                    informationDAO.CreateSupplier(name, address, phoneNumber, code, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "TẠO NHÀ CUNG CẤP THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/EditSupplier")]
        [HttpPut]
        public IHttpActionResult EditSupplier(int supplierPK, string address, string phoneNumber, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.UpdateSupplier(supplierPK, address, phoneNumber, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "CHỈNH SỬA NHÀ CUNG CẤP THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/DeleteSupplier")]
        [HttpDelete]
        public IHttpActionResult DeleteSupplier(int supplierPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.DeleteSupplier(supplierPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "XÓA NHÀ CUNG CẤP THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/DeactiveSupplier")]
        [HttpPut]
        public IHttpActionResult DeactiveSupplier(int supplierPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.DeactiveSupplier(supplierPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "ẨN NHÀ CUNG CẤP THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/ActiveSupplier")]
        [HttpPut]
        public IHttpActionResult ActiveSupplier(int supplierPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.ActiveSupplier(supplierPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "HIỆN NHÀ CUNG CẤP THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/CreateAccessory")]
        [HttpPost]
        public IHttpActionResult CreateAccessory(string item, string art, string description, string comment, string color, int customerPK, int supplierPK, int accessoryTypePK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    if (art == "undefined") art = "";
                    if (comment == "undefined") comment = "";
                    if (color == "undefined") color = "";
                    informationDAO.CreateAccessory(item, art, description, comment, color, customerPK, supplierPK, accessoryTypePK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "TẠO PHỤ LIỆU THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/EditAccessory")]
        [HttpPut]
        public IHttpActionResult EditAccessory(int accessoryPK, string comment, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.UpdateAccessory(accessoryPK, comment, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "CHỈNH SỬA PHỤ LIỆU THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/LinkConception")]
        [HttpPost]
        public IHttpActionResult LinkConception(int accessoryPK, int conceptionPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.LinkConception(accessoryPK, conceptionPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "LIÊN KẾT MÃ HÀNG VÀ PHỤ LIỆU THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/LinkConception2")]
        [HttpPost]
        public IHttpActionResult LinkConception2(List<int> accessoryPKs, int conceptionPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.LinkConception(accessoryPKs, conceptionPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "LIÊN KẾT MÃ HÀNG VÀ PHỤ LIỆU THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/UnlinkConception")]
        [HttpPost]
        public IHttpActionResult UnlinkConception(int accessoryPK, int conceptionPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.UnlinkConception(accessoryPK, conceptionPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "GỠ LIÊN KẾT MÃ HÀNG VÀ PHỤ LIỆU THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/DeactiveAccessory")]
        [HttpPut]
        public IHttpActionResult DeactiveAccessory(int accessoryPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.DeactiveAccessory(accessoryPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "ẨN PHỤ LIỆU THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/ReactiveAccessory")]
        [HttpPut]
        public IHttpActionResult ReactiveAccessory(int accessoryPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.ReactiveAccessory(accessoryPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "HIỆN PHỤ LIỆU THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/DeleteAccessory")]
        [HttpDelete]
        public IHttpActionResult DeleteAccessory(int accessoryPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                InformationDAO informationDAO = new InformationDAO();
                try
                {
                    informationDAO.DeleteAccessory(accessoryPK, userID);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "XÓA PHỤ LIỆU THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/CreateBox")]
        [HttpPost]
        public IHttpActionResult CreateBox(int boxKind, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Manager"))
            {
                BoxDAO boxDAO = new BoxDAO();
                try
                {
                    // delete accessory
                    DateTime now = DateTime.Now;
                    string tempDay = (now.Day + "").Length == 1 ? '0' + (now.Day + "") : (now.Day + "");
                    string tempMonth = (now.Month + "").Length == 1 ? '0' + (now.Month + "") : (now.Month + "");
                    string tempYear = (now.Year + "").Substring((now.Year + "").Length - 2);

                    string boxID = tempDay + tempMonth + tempYear;
                    Box box = (from b in db.Boxes.OrderByDescending(unit => unit.BoxPK)
                               where b.BoxID.Contains(boxID)
                               select b).FirstOrDefault();

                    if (box == null)
                    {
                        boxID += "001";
                    }
                    else
                    {
                        int tempInt = Int32.Parse(box.BoxID.Substring(box.BoxID.Length - 6, 3)) + 1;
                        string tempStr = tempInt + "";
                        if (tempStr.Length == 1) boxID += "00" + tempStr;
                        if (tempStr.Length == 2) boxID += "0" + tempStr;
                        if (tempStr.Length == 3) boxID += tempStr;
                    }
                    boxID += "box";
                    box = new Box(boxID);
                    db.Boxes.Add(box);
                    db.SaveChanges();

                    box = boxDAO.GetBoxByBoxID(boxID);
                    boxDAO.CreateBox(boxKind, box.BoxPK);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                }
                return Content(HttpStatusCode.OK, "TẠO THÙNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");
            }
        }

        [Route("api/InformationController/DeleteBox")]
        [HttpPost]
        public IHttpActionResult DeleteBox(int boxPK, string userID)
        {
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Manager") || new ValidationBeforeCommandDAO().IsValidUser(userID, "Staff"))
            {
                BoxDAO boxDAO = new BoxDAO();
                try
                {
                    boxDAO.DeleteBox(boxPK, userID);
                }
                catch (Exception e)
                {
                    //return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
                    return Content(HttpStatusCode.Conflict, "ĐANG CÓ HÀNG TRONG THÙNG");
                }
                return Content(HttpStatusCode.OK, "XÓA THÙNG THÀNH CÔNG!");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY!");

            }
        }

        [Route("api/InformationController/UploadFile")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadFile(int AccessoryPK, string userID)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Content(HttpStatusCode.Conflict, "DATA GỬI LÊN KHÔNG PHẢI HÌNH!");
            }
            if (new ValidationBeforeCommandDAO().IsValidUser(userID, "Merchandiser"))
            {
                try
                {
                    Accessory accessory = db.Accessories.Find(AccessoryPK);
                    if (accessory == null)
                    {
                        return Content(HttpStatusCode.Conflict, "ACCESSORY KHÔNG TỒN TẠI!");
                    }
                    else
                    {
                        // upload img
                        var root = HttpContext.Current.Server.MapPath("~/Image");
                        var provider = new MultipartFormDataStreamProvider(root);
                        await Request.Content.ReadAsMultipartAsync(provider);
                        //foreach (var file in provider.FileData)
                        //{
                        //    var name = file.Headers.ContentDisposition.FileName;
                        //    string now = DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds + "";
                        //    name = now.Replace('.', '8') + ".png";
                        //    var localFileName = file.LocalFileName;
                        //    var filePath = Path.Combine(root, name);
                        //    File.Move(localFileName, filePath);
                        //    if (accessory.Image != null)
                        //    {
                        //        File.Delete(Path.Combine(root, accessory.Image));
                        //    }
                        //    accessory.Image = name;
                        //    db.Entry(accessory).State = EntityState.Modified;
                        //    db.SaveChanges();
                        //    return Content(HttpStatusCode.OK, "ĐĂNG HÌNH THÀNH CÔNG!");
                        //}
                        MultipartFileData file = provider.FileData[0];
                        var name = file.Headers.ContentDisposition.FileName;
                        string now = DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds + "";
                        name = now.Replace('.', '8') + ".png";
                        var localFileName = file.LocalFileName;
                        var filePath = Path.Combine(root, name);
                        File.Move(localFileName, filePath);
                        if (accessory.Image != null)
                        {
                            File.Delete(Path.Combine(root, accessory.Image));
                        }
                        accessory.Image = name;
                        db.Entry(accessory).State = EntityState.Modified;
                        db.SaveChanges();
                        return Content(HttpStatusCode.OK, "ĐĂNG HÌNH THÀNH CÔNG!");
                    }
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
        public class Accessory_RestoreItem2
        {
            public Accessory_RestoreItem2()
            {
            }

            public Accessory_RestoreItem2(Accessory accessory)
            {
                AccessoryPK = accessory.AccessoryPK;
                AccessoryID = accessory.AccessoryID;
                AccessoryDescription = accessory.AccessoryDescription;
                Item = accessory.Item;
                Art = accessory.Art;
                Color = accessory.Color;
                Image = accessory.Image;
                AccessoryTypePK = accessory.AccessoryTypePK;
            }

            public int AccessoryPK { get; set; }

            public string AccessoryID { get; set; }

            public string AccessoryDescription { get; set; }

            public string Item { get; set; }

            public string Art { get; set; }

            public string Color { get; set; }

            public string Image { get; set; }

            public int AccessoryTypePK { get; set; }
        }

        [Route("api/IssuingController/GetAccessoriesForFilter")]
        [HttpPost]
        public IHttpActionResult GetAccessoriesForFilter(Client_ConceptionsAndAccessoryTypes input, bool onlyNonImagedAccessory, string customerName)
        {
            List<Accessory_RestoreItem2> result = new List<Accessory_RestoreItem2>();
            try
            {
                Customer customer = (from cus in db.Customers
                                     where cus.CustomerName == customerName
                                     select cus).FirstOrDefault();
                if (customer == null) return Content(HttpStatusCode.Conflict, "CUSTOMER KHÔNG TỒN TẠI!");
                List<int> tempAccessoriesPK = (from acc in db.Accessories
                                               where acc.CustomerPK == customer.CustomerPK
                                               select acc.AccessoryPK).ToList();
                foreach (var AccessoryPK in tempAccessoriesPK)
                {
                    if (input.conceptionPKs.Count == 0)
                    {
                        Accessory tempAccessory = db.Accessories.Find(AccessoryPK);
                        if (input.accessorytypePKs.Contains(tempAccessory.AccessoryTypePK))
                        {
                            if (onlyNonImagedAccessory == true)
                            {
                                if (tempAccessory.Image == null)
                                {
                                    tempAccessory.Image = "default.png";
                                    result.Add(new Accessory_RestoreItem2(tempAccessory));
                                }
                            }
                            else
                            {
                                if (tempAccessory.Image == null)
                                {
                                    tempAccessory.Image = "default.png";
                                }
                                result.Add(new Accessory_RestoreItem2(tempAccessory));
                            }
                        }
                    }
                    else
                    {
                        foreach (var conceptionPK in input.conceptionPKs)
                        {
                            if ((from unit in db.ConceptionAccessories
                                 where unit.ConceptionPK == conceptionPK && unit.AccessoryPK == AccessoryPK
                                 select unit).FirstOrDefault() != null)
                            {
                                Accessory tempAccessory = db.Accessories.Find(AccessoryPK);
                                if (input.accessorytypePKs.Contains(tempAccessory.AccessoryTypePK))
                                {
                                    if (onlyNonImagedAccessory == true)
                                    {
                                        if (tempAccessory.Image == null)
                                        {
                                            tempAccessory.Image = "default.png";
                                            result.Add(new Accessory_RestoreItem2(tempAccessory));
                                        }
                                    }
                                    else
                                    {
                                        if (tempAccessory.Image == null)
                                        {
                                            tempAccessory.Image = "default.png";
                                        }
                                        result.Add(new Accessory_RestoreItem2(tempAccessory));
                                    }
                                }
                            }
                        }
                    }
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

