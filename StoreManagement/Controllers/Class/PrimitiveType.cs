using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagement.Class
{
    public class PrimitiveType
    {
        public static bool isValidIntegerQuantity(int quantity)
        {
            if (quantity > 0 && quantity < 100000000)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isValidQuantity(double quantity)
        {
            if (quantity > 0 && quantity < 100000000)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isValidOrderID(string orderID)
        {
            if (orderID != null && orderID.Length <= 25 && orderID != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isValidComment(string comment)
        {
            if (comment == null || comment.Length <= 50)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isValidAddress(string address)
        {
            if (address == null || address.Length <= 200)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isValidPhoneNumber(string phoneNumber)
        {
            if (phoneNumber == null || phoneNumber.Length <= 20)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isValidCode(string code)
        {
            if (code != null && code.Length <= 3 && code != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isValidName(string name)
        {
            if (name != null && name.Length <= 30 && name != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}