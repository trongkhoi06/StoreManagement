using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Content_InnerException
    {
        public Exception e { get; set; }

        public Content_InnerException(Exception e)
        {
            this.e = e;
        }

        public string InnerMessage()
        {
            Exception temp = e;
            //if (temp.Message.Contains("~AST-ERR~"))
            //{
            //    return temp.Message.Substring(0, temp.Message.Length - 9);
            //}
            //else
            //{
            //    return "ĐÃ CÓ LỖI XẢY RA, VUI LÒNG THỬ LẠI!";
            //}
            //lấy message để biết lỗi
            while (temp.InnerException != null)
            {
                temp = temp.InnerException;
            };
            return temp.Message;


            //return "ĐÃ CÓ LỖI XẢY RA, VUI LÒNG THỬ LẠI!";
        }
    }
}