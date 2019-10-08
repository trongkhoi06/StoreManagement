using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        // GET: api/SystemUsers
        public IQueryable<SystemUser> GetSystemUsers()
        {
            return db.SystemUsers;
        }

        // GET: api/SystemUsers/5
        [ResponseType(typeof(SystemUser))]
        public IHttpActionResult GetSystemUser(string id)
        {
            SystemUser systemUser = db.SystemUsers.Find(id);
            if (systemUser == null)
            {
                return NotFound();
            }

            return Ok(systemUser);
        }

        // PUT: api/SystemUsers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSystemUser(string id, SystemUser systemUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != systemUser.Username)
            {
                return BadRequest();
            }

            db.Entry(systemUser).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Update Successfully!");
        }

        // POST: api/SystemUsers
        [ResponseType(typeof(SystemUser))]
        public IHttpActionResult PostSystemUser(SystemUser systemUser)
        {
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
                if (SystemUserExists(systemUser.Username))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Insert Successfully!");
        }

        // DELETE: api/SystemUsers/5
        [ResponseType(typeof(SystemUser))]
        public IHttpActionResult DeleteSystemUser(string id)
        {
            SystemUser systemUser = db.SystemUsers.Find(id);
            if (systemUser == null)
            {
                return NotFound();
            }

            db.SystemUsers.Remove(systemUser);
            db.SaveChanges();

            return Ok("Delete Successfully!");
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
            return db.SystemUsers.Count(e => e.Username == id) > 0;
        }
    }
}