using StoreManagement.Models;
using System;
using System.Collections.Generic;
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
            public string DeviceName { get; set; }
        }

        [Route("api/SystemLogin")]
        [HttpPost]
        public IHttpActionResult login(UserClient userClient)
        {
            try
            {
                SqlParameter Username = new SqlParameter("@Username", userClient.Username);
                SqlParameter Password = new SqlParameter("@Password", userClient.Password);
                SqlParameter DeviceName = new SqlParameter("@DeviceName", userClient.DeviceName);
                String role = db.Database.SqlQuery<String>("exec SystemLogin @Username, @Password, @DeviceName", Username,Password,DeviceName).FirstOrDefault();
                if (role == null) return NotFound();
                return Ok(role);
            }
            catch(Exception e)
            {
                return NotFound();
            }
        }
    }
}
