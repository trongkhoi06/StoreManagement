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
        public class DeviceClient
        {
            public string DeviceName { get; set; }
        }
        public class UserClient
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [Route("api/SystemLogin")]
        [HttpPost]
        public IHttpActionResult SystemLogin(UserClient userClient)
        {
            try
            {
                SqlParameter Username = new SqlParameter("@Username", userClient.Username);
                SqlParameter Password = new SqlParameter("@Password", userClient.Password);
                String role = db.Database.SqlQuery<String>("exec SystemLogin @Username, @Password", Username,Password).FirstOrDefault();
                if (role == null) return NotFound();
                return Ok(role);
            }
            catch(Exception e)
            {
                return NotFound();
            }
        }

        [Route("api/DeviceLogin")]
        [HttpPost]
        public IHttpActionResult DeviceLogin(DeviceClient device)
        {
            try
            {
                SqlParameter DeviceName = new SqlParameter("@DeviceName", device.DeviceName);
                String isActive = db.Database.SqlQuery<String>("exec DeviceLogin @DeviceName", DeviceName).FirstOrDefault();
                if (isActive.Equals("false")) return Ok(false);
                return Ok(true);
            }
            catch (Exception e)
            {
                return Ok("Exception! something was wrong");
            }
        }

        [Route("api/ActiveDevice")]
        public IHttpActionResult ActiveDevice(Device device)
        {
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

            return Ok("Active Device successfully");
        }

        private bool DeviceExists(string name)
        {
            return db.Devices.Count(e => e.DeviceName == name) > 0;
        }
    }
}
