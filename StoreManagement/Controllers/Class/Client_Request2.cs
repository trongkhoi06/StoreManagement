﻿using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_Request2
    {
        public Client_Request2()
        {
        }

        public Client_Request2(Request request, Demand demand, Conception conception)
        {
            RequestPK = request.RequestPK;
            RequestID = request.RequestID;
            DateCreated = request.DateCreated;
            ExpectedDate = request.ExpectedDate;
            Comment = request.Comment;
            DemandID = demand.DemandID;
            StartWeek = demand.StartWeek;
            EndWeek = demand.EndWeek;
            ReceiveDivision = demand.ReceiveDivision;
            ConceptionCode = conception.ConceptionCode;
        }

        public int RequestPK { get; set; }

        public string RequestID { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime ExpectedDate { get; set; }

        public string Comment { get; set; }

        public string DemandID { get; set; }

        public int? StartWeek { get; set; }

        public int? EndWeek { get; set; }

        public string ReceiveDivision { get; set; }

        public string ConceptionCode { get; set; }

    }
}