using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_Request
    {
        public Client_Request()
        {
        }

        public Client_Request(Request request, Demand demand, Conception conception)
        {
            RequestPK = request.RequestPK;
            RequestID = request.RequestID;
            DateCreated = request.DateCreated;
            ExpectedDate = request.ExpectedDate;
            IsIssued = request.IsIssued;
            IsConfirmed = request.IsConfirmed;
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

        public bool IsIssued { get; set; }

        public bool IsConfirmed { get; set; }

        public string Comment { get; set; }

        public string DemandID { get; set; }

        public int StartWeek { get; set; }

        public int EndWeek { get; set; }

        public string ReceiveDivision { get; set; }

        public string ConceptionCode { get; set; }

    }
}