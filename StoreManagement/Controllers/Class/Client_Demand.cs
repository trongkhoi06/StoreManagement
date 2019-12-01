using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_Demand
    {
        public Client_Demand()
        {
        }

        public Client_Demand(Demand demand,Conception conception)
        {
            DemandPK = demand.DemandPK;
            DemandID = demand.DemandID;
            StartWeek = demand.StartWeek;
            EndWeek = demand.EndWeek;
            ReceiveDivision = demand.ReceiveDivision;
            ConceptionCode = conception.ConceptionCode;
        }

        public int DemandPK { get; set; }
        
        public string DemandID { get; set; }

        public int StartWeek { get; set; }

        public int EndWeek { get; set; }

        public string ReceiveDivision { get; set; }

        public string ConceptionCode { get; set; }
        
    }
}