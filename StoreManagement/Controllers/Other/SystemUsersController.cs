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
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SystemUsersController : ApiController
    {
        private UserModel db = new UserModel();

        public class Client_User
        {
            public string UserID { get; set; }
            public string RoleName { get; set; }
            public string Name { get; set; }
            public DateTime DateCreated { get; set; }
            public bool IsDeleted { get; set; }
        }

        [HttpGet]
        public IHttpActionResult GetSystemUsers()
        {
            try
            {
                List<Client_User> systemUsers = db.Database.SqlQuery<Client_User>("exec GetUsers").ToList();
                return Ok(systemUsers);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
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
        public IHttpActionResult InsertSystemUser(Client_User user)
        {
            if (db.Roles.Find(user.RoleName) == null)
            {
                return Conflict();
            }
            SystemUser systemUser = new SystemUser
            {
                UserID = user.UserID,
                RoleName = user.RoleName,
                Name = user.Name,
                DateCreated = user.DateCreated,
                Password = "PDG@123"
            };
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SystemUsers.Add(systemUser);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (SystemUserExists(systemUser.UserID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok("THÊM MỚI NHÂN VIÊN THÀNH CÔNG!");
        }

        // DELETE: api/SystemUsers/5
        [HttpDelete]
        public IHttpActionResult DeleteSystemUser(string id)
        {
            try
            {
                SqlParameter userID = new SqlParameter("@userID", id);
                // use execsqlcommand when there is 0 thing in return
                db.Database.ExecuteSqlCommand("exec DeleteUsers @userID", userID);
                return Ok("XÓA NHÂN VIÊN THÀNH CÔNG!");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
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