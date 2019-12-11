using StoreManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class Client_Customer_Conception
    {
        public class ConceptionPK_ConceptionFullKey
        {
            public ConceptionPK_ConceptionFullKey()
            {
            }

            public ConceptionPK_ConceptionFullKey(int conceptionPK, string conceptionFullKey)
            {
                ConceptionPK = conceptionPK;
                ConceptionFullKey = conceptionFullKey;
            }

            public int ConceptionPK { get; set; }

            public string ConceptionFullKey { get; set; }
        }

        public Client_Customer_Conception()
        {
        }

        public Client_Customer_Conception(string customerName, List<Conception> conceptions)
        {
            CustomerName = customerName;

            List<ConceptionPK_ConceptionFullKey> tempList = new List<ConceptionPK_ConceptionFullKey>();
            foreach (var conception in conceptions)
            {
                string temp = conception.ConceptionCode + conception.Season + (conception.Year + "").Substring(2);
                ConceptionPK_ConceptionFullKey tempConception = new ConceptionPK_ConceptionFullKey(conception.ConceptionPK, temp);
                tempList.Add(tempConception);
            }


            Conceptions = tempList;
        }

        public string CustomerName { get; set; }

        public List<ConceptionPK_ConceptionFullKey> Conceptions { get; set; }
    }
}