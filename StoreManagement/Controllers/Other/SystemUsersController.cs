using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using StoreManagement.Class;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SystemUsersController : ApiController
    {
        private UserModel db = new UserModel();

        public class Client_User
        {
            public Client_User(SystemUser systemUser)
            {
                UserID = systemUser.UserID;
                RoleName = systemUser.RoleName;
                Name = systemUser.Name;
                DateCreated = systemUser.DateCreated;
                IsDeleted = systemUser.IsDeleted;
                WorkplacePK = systemUser.WorkplacePK;
            }

            public string UserID { get; set; }

            public string RoleName { get; set; }

            public string Name { get; set; }

            public DateTime DateCreated { get; set; }

            public bool IsDeleted { get; set; }

            public int WorkplacePK { get; set; }

            public string WorkplaceName { get; set; }
        }

        public class Client_User_Create
        {

            public string UserID { get; set; }

            public string RoleName { get; set; }

            public string Name { get; set; }

            public int WorkplacePK { get; set; }
        }

        [HttpGet]
        public IHttpActionResult GetSystemUsers()
        {
            try
            {
                //List<Client_User> systemUsers = db.Database.SqlQuery<Client_User>("exec GetUsers").ToList();
                List<Client_User> result = new List<Client_User>();
                List<SystemUser> systemUsers = db.SystemUsers.Where(unit => unit.RoleName != "Administrator").ToList();
                foreach (var user in systemUsers)
                {
                    result.Add(new Client_User(user));
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        // GET: api/SystemUsers/5
        [Route("api/SystemUsers/ID")]
        [HttpPost]
        public IHttpActionResult GetSystemUser(string userID)
        {
            SystemUser systemUser = db.SystemUsers.Find(userID);
            if (systemUser == null)
            {
                return NotFound();
            }

            return Ok(systemUser);
        }

        // PUT: api/SystemUsers
        [HttpPut]
        public IHttpActionResult UpdateSystemUser(Client_User user)
        {
            SystemUser systemUser = db.SystemUsers.Find(user.UserID);
            systemUser.Name = user.Name;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(systemUser).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemUserExists(user.UserID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("CẬP NHẬT THÀNH CÔNG");
        }

        [Route("api/SystemUsers/ResetPassword")]
        [HttpPut]
        public IHttpActionResult ResetPassword(string userID)
        {
            SystemUser systemUser = db.SystemUsers.Find(userID);
            systemUser.Password = "PDG@123";
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(systemUser).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemUserExists(userID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("RESET MẬT KHẨU THÀNH CÔNG!");
        }

        [Route("api/SystemUsers/ChangePassword")]
        [HttpPut]
        public IHttpActionResult ChangePassword(string oldPassword, string newPassword, string userID)
        {
            try
            {
                SystemUser systemUser = db.SystemUsers.Find(userID);
                if (systemUser.Password != oldPassword)
                {
                    return Content(HttpStatusCode.Conflict, "SAI MẬT KHẨU!");
                }
                systemUser.Password = newPassword;
                db.Entry(systemUser).State = EntityState.Modified;
                db.SaveChanges();

                return Ok("ĐỔI MẬT KHẨU THÀNH CÔNG!");
            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/SystemUsers/ChangeName")]
        [HttpPut]
        public IHttpActionResult ChangeName(string name, string password, string userID)
        {
            try
            {
                SystemUser systemUser = db.SystemUsers.Find(userID);
                if (systemUser.Password != password)
                {
                    return Content(HttpStatusCode.Conflict, "SAI MẬT KHẨU!");
                }
                systemUser.Name = name;
                db.Entry(systemUser).State = EntityState.Modified;
                db.SaveChanges();

                return Ok("ĐỔI TÊN THÀNH CÔNG!");
            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        [Route("api/SystemUsers/Recover")]
        [HttpPut]
        public IHttpActionResult RecoverUser(string userID)
        {
            SystemUser systemUser = db.SystemUsers.Find(userID);
            systemUser.IsDeleted = false;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(systemUser).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemUserExists(userID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("HỒI PHỤC NHÂN VIÊN THÀNH CÔNG");
        }

        // POST: api/SystemUsers
        [HttpPost]
        public IHttpActionResult InsertSystemUser(Client_User_Create user)
        {
            try
            {
                //SystemUser admin = db.SystemUsers.Find(userID);
                //if (admin.RoleName != "Administrator")
                //{
                //    return Content(HttpStatusCode.Conflict,"BẠN KHÔNG PHẢI LÀ ADMIN");
                //}
                if (db.Roles.Find(user.RoleName) == null)
                {
                    return Content(HttpStatusCode.Conflict, "QUYỀN CỦA USER KHÔNG HỢP LỆ!");
                }
                if (user.Name.Length > 30)
                {
                    return Content(HttpStatusCode.Conflict, "TÊN CỦA NHÂN VIÊN KHÔNG ĐƯỢC QUÁ 30 KÍ TỰ!");
                }
                SystemUser systemUser = new SystemUser
                {
                    UserID = user.UserID,
                    RoleName = user.RoleName,
                    Name = user.Name,
                    WorkplacePK = user.WorkplacePK,
                    DateCreated = DateTime.Now,
                    Password = "PDG@123"
                };

                db.SystemUsers.Add(systemUser);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }

            return Ok("THÊM MỚI NHÂN VIÊN THÀNH CÔNG!");
        }

        // DELETE: api/SystemUsers/5
        [HttpDelete]
        public IHttpActionResult DeleteSystemUser(string id)
        {
            try
            {
                //SqlParameter userID = new SqlParameter("@userID", id);
                //// use execsqlcommand when there is 0 thing in return
                //db.Database.ExecuteSqlCommand("exec DeleteUsers @userID", userID);
                SystemUser systemUser = db.SystemUsers.Find(id);
                systemUser.IsDeleted = true;
                db.Entry(systemUser).State = EntityState.Modified;
                db.SaveChanges();
                return Ok("XÓA NHÂN VIÊN THÀNH CÔNG!");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
            }
        }

        // POST: api/SystemUsers
        [Route("api/SystemUsers/CreateWorkplace")]
        [HttpPost]
        public IHttpActionResult CreateWorkplace(string workplaceID, string userID)
        {
            try
            {
                Workplace workplace = new Workplace(workplaceID, false);
                if (db.SystemUsers.Find(userID).RoleName != "Administrator")
                {
                    return Content(HttpStatusCode.Conflict, "PHẢI LÀ ADMIN MỚI CÓ QUYỀN TẠO WORKPLACE");
                }
                db.Workplaces.Add(workplace);
                db.SaveChanges();
                return Ok("THAO TÁC THÀNH CÔNG!");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        // DELETE: api/SystemUsers/5
        [Route("api/SystemUsers/DeleteWorkplace")]
        [HttpDelete]
        public IHttpActionResult DeleteWorkplace(int workplacePK, string userID)
        {
            try
            {
                Workplace workplace = db.Workplaces.Find(workplacePK);
                if (db.SystemUsers.Find(userID).RoleName != "Administrator")
                {
                    return Content(HttpStatusCode.Conflict, "PHẢI LÀ ADMIN MỚI CÓ QUYỀN XÓA WORKPLACE");
                }
                SystemUser systemUser = db.SystemUsers.Where(unit => unit.WorkplacePK == workplace.WorkplacePK).FirstOrDefault();
                Demand demand = db.Demands.Where(unit => unit.WorkplacePK == workplace.WorkplacePK).FirstOrDefault();
                if (systemUser != null || demand != null)
                {
                    return Content(HttpStatusCode.Conflict, "ĐƠN VỊ ĐÃ CÓ NHÂN VIÊN HOẶC ĐƠN CẤP PHÁT!");
                }

                db.Workplaces.Remove(workplace);
                db.SaveChanges();
                return Ok("THAO TÁC THÀNH CÔNG!");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
            }
        }


        [Route("api/SystemUsers/ChangeUserWorkplace")]
        [HttpPut]
        public IHttpActionResult ChangeUserWorkplace(string userIDChanging, int workplacePK, string userID)
        {
            try
            {
                SystemUser systemUser = db.SystemUsers.Find(userIDChanging);
                if (db.SystemUsers.Find(userID).RoleName != "Administrator")
                {
                    return Content(HttpStatusCode.Conflict, "PHẢI LÀ ADMIN MỚI CÓ QUYỀN TẠO WORKPLACE");
                }
                Workplace workplace = db.Workplaces.Find(workplacePK);
                if (workplace == null)
                {
                    return Content(HttpStatusCode.Conflict, "WORKPLACE KHÔNG TỒN TẠI");
                }
                systemUser.WorkplacePK = workplace.WorkplacePK;
                db.Entry(systemUser).State = EntityState.Modified;
                db.SaveChanges();
                return Ok("ĐỔI ĐƠN VỊ CỦA NHÂN VIÊN THÀNH CÔNG!");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, new Content_InnerException(e).InnerMessage());
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SystemUserExists(string id)
        {
            return db.SystemUsers.Count(e => e.UserID == id) > 0;
        }
    }
}