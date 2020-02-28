using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace StoreManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        private UserModel db = new UserModel();
        public class UserClient
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class Client_SystemLogin_User
        {
            public string RoleName { get; set; }

            public string Fullname { get; set; }
        }

        [Route("api/SystemLogin")]
        [HttpPost]
        public IHttpActionResult SystemLogin([FromBody] UserClient userClient)
        {
            try
            {
                //SqlParameter userID = new SqlParameter("@userID", userClient.Username);
                //SqlParameter Password = new SqlParameter("@Password", userClient.Password);
                //string roleName = db.Database.SqlQuery<string>("exec SystemLogin @userID, @Password", userID, Password).FirstOrDefault();
                SystemUser systemUser = db.SystemUsers.Where(unit => unit.UserID == userClient.Username && unit.Password == userClient.Password).FirstOrDefault();
                if (systemUser == null) return NotFound();
                else
                {
                    Client_SystemLogin_User result = new Client_SystemLogin_User()
                    {
                        RoleName = systemUser.RoleName,
                        Fullname = systemUser.Name
                    };
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
            }
        }

        [Route("api/GetHashByUserID")]
        [HttpPost]
        public IHttpActionResult GetHashByUserID([FromBody] UserClient userClient)
        {
            List<string> result = new List<string>();
            try
            {
                //SqlParameter userID = new SqlParameter("@userID", userClient.Username);
                //SqlParameter Password = new SqlParameter("@Password", userClient.Password);
                //string roleName = db.Database.SqlQuery<string>("exec SystemLogin @userID, @Password", userID, Password).FirstOrDefault();
                SystemUser systemUser = db.SystemUsers.Find(userClient.Username);
                if (systemUser.Password == userClient.Password)
                {
                    if (db.Roles.Find(systemUser.RoleName) == null) return NotFound();
                    else
                    {
                        string hash = Base64Encode(userClient.Username + "~!~" + DateTime.Now.ToString());
                        result.Add(hash);
                        result.Add(systemUser.RoleName);
                        return Content(HttpStatusCode.OK, result);
                    }
                }
                else
                {
                    return Content(HttpStatusCode.Conflict, "SAI MẬT KHẨU!");
                }

            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
            }
        }
        public class Client_Hash
        {
            public Client_Hash()
            {
            }

            public Client_Hash(string hash)
            {
                this.Hash = hash;
            }

            public string Hash { get; set; }
        }

        [Route("api/GetUserIDByHash")]
        [HttpPost]
        public IHttpActionResult GetUserIDByHash([FromBody]Client_Hash h)
        {
            try
            {
                string userID = Base64Decode(h.Hash).Split(new[] { "~!~" }, StringSplitOptions.None)[0];
                return Content(HttpStatusCode.OK, userID);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
            }
        }

        [Route("api/DeviceLogin")]
        [HttpPost]
        public IHttpActionResult DeviceLogin(string deviceCode)
        {
            try
            {
                SqlParameter deviceCodeParam = new SqlParameter("@DeviceCode", deviceCode);
                String isActive = db.Database.SqlQuery<String>("exec DeviceLogin @DeviceCode", deviceCodeParam).FirstOrDefault();
                if (isActive.Equals("false")) return Ok(false);
                return Ok(true);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
            }
        }

        [Route("api/ActiveDevice")]
        public IHttpActionResult ActiveDevice(string deviceCode, string deviceName)
        {
            Device device = new Device(deviceCode, deviceName);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Devices.Add(device);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (DeviceExists(device.DeviceName))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok("KÍCH HOẠT THIẾT BỊ THÀNH CÔNG!");
        }

        private bool DeviceExists(string name)
        {
            return db.Devices.Count(e => e.DeviceName == name) > 0;
        }

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
    }
}
