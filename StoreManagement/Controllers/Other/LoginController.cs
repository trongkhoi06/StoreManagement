using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        [Route("api/SystemLogin")]
        [HttpPost]
        public IHttpActionResult SystemLogin([FromBody] UserClient userClient)
        {
            try
            {
                SqlParameter userID = new SqlParameter("@userID", userClient.Username);
                SqlParameter Password = new SqlParameter("@Password", userClient.Password);
                string roleName = db.Database.SqlQuery<string>("exec SystemLogin @userID, @Password", userID, Password).FirstOrDefault();
                if (db.Roles.Find(roleName) == null) return NotFound();
                else
                {
                    return Ok(roleName);
                }
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
    }
}
