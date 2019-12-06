using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_IssuingSession
    {
        public Client_IssuingSession()
        {
        }

        public Client_IssuingSession(IssuingSession issuingSession, Demand demand, Request request, Conception conception)
        {
            IssuingSessionPK = issuingSession.IssuingSessionPK;
            ExecutedDate = issuingSession.ExecutedDate;
            DemandID = demand.DemandID;
            StartWeek = demand.StartWeek;
            EndWeek = demand.EndWeek;
            ReceiveDivision = demand.ReceiveDivision;
            ConceptionCode = conception.ConceptionCode;
            RequestID = request.RequestID;
            IsConfirmed = request.IsConfirmed;
        }

        public int IssuingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public string DemandID { get; set; }

        public int StartWeek { get; set; }

        public int EndWeek { get; set; }

        public string ReceiveDivision { get; set; }

        public string ConceptionCode { get; set; }

        public string RequestID { get; set; }

        public bool IsConfirmed { get; set; }

    }
}