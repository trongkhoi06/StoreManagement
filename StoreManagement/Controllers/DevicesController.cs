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
    public class DevicesController : ApiController
    {
        private UserModel db = new UserModel();

        public partial class DeviceUser
        {
            public string DeviceName { get; set; }

            public string DeviceInformation { get; set; }

            public bool IsDeleted { get; set; }
            public DateTime DateCreated { get; set; }
        }

        // GET: api/Devices
        public IHttpActionResult GetDevices()
        {
            try
            {
                List<DeviceUser> devices = db.Database.SqlQuery<DeviceUser>("exec GetDevices").ToList();
                return Ok(devices);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        // GET: api/Devices/5
        [ResponseType(typeof(Device))]
        public IHttpActionResult GetDevice(int id)
        {
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return NotFound();
            }

            return Ok(device);
        }

        // PUT: api/Devices/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDevice(int id, Device device)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != device.DeviceID)
            {
                return BadRequest();
            }

            db.Entry(device).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Devices
        [ResponseType(typeof(Device))]
        public IHttpActionResult PostDevice(Device device)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Devices.Add(device);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = device.DeviceID }, device);
        }

        // DELETE: api/Devices/5
        [HttpDelete]
        public IHttpActionResult DeleteDevice(string id)
        {
            try
            {
                SqlParameter DeviceName = new SqlParameter("@DeviceName", id);
                // use execsqlcommand when there is 0 thing in return
                db.Database.ExecuteSqlCommand("exec DeleteDevices @DeviceName", DeviceName);
                return Ok("Delete Successfully!");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.Conflict,e.Message);
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

        private bool DeviceExists(int id)
        {
            return db.Devices.Count(e => e.DeviceID == id) > 0;
        }
    }
}