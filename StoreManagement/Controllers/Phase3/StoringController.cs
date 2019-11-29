using StoreManagement.Class;
using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StoreManagement.Controllers
{
    public class StoringController : ApiController
    {
        private UserModel db = new UserModel();
        [Route("api/StoringController/StoreBoxBusiness")]
        [HttpPost]
        public IHttpActionResult StoreBoxBusiness(string boxID, double countedQuantity, string userID)
        {
            // kiểm trước khi chạy lệnh
            SystemUser systemUser = db.SystemUsers.Find(userID);
            // check role of system user
            if (systemUser != null && systemUser.RoleID == 4)
            {
                // khởi tạo
                CountingItemController countingItemController = new CountingItemController();
                PackedItemsController packedItemsController = new PackedItemsController();
                // chạy lệnh store box

                return Content(HttpStatusCode.OK, "STORE BOX THÀNH CÔNG");
            }
            else
            {
                return Content(HttpStatusCode.Conflict, "BẠN KHÔNG CÓ QUYỀN ĐỂ THỰC HIỆN VIỆC NÀY");
            }

        }

    }
}
