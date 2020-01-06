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

        //public string InnerMessage()
        //{
        //    Exception temp = e;
        //    while(temp.InnerException != null)
        //    {
        //        temp = temp.InnerException;
        //    };
        //    return temp.Message;
        //}
        public string InnerMessage()
        {
            return "ĐÃ CÓ LỖI XẢY RA, VUI LÒNG THỬ LẠI!";
        }
    }
}