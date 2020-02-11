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
    public class InformationDAO
    {
        private UserModel db = new UserModel();

        public void CreateCustomer(string name, string code, string address, string phoneNumber, string userID)
        {
            try
            {
                // create customer
                Customer customer = new Customer(name, code, address, phoneNumber);
                db.Customers.Add(customer);

                // lưu activity create
                Activity activity = new Activity("create", customer.CustomerName, "Customer", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateCustomer(int customerPK, string address, string phoneNumber, string userID)
        {
            try
            {
                // update customer
                Customer customer = db.Customers.Find(customerPK);
                customer.Address = address;
                customer.PhoneNumber = phoneNumber;
                db.Entry(customer).State = EntityState.Modified;


                // lưu activity update
                Activity activity = new Activity("update", customer.CustomerName, "Customer", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteCustomer(int customerPK, string userID)
        {
            try
            {
                // delete customer
                Customer customer = db.Customers.Find(customerPK);
                db.Customers.Remove(customer);

                // lưu activity update
                Activity activity = new Activity("delete", customer.CustomerName, "Customer", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeactiveCustomer(int customerPK, string userID)
        {
            try
            {
                // deactive customer
                Customer customer = db.Customers.Find(customerPK);
                customer.IsActive = false;
                db.Entry(customer).State = EntityState.Modified;

                // lưu activity update
                Activity activity = new Activity("deactive", customer.CustomerName, "Customer", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ActiveCustomer(int customerPK, string userID)
        {
            try
            {
                // active customer
                Customer customer = db.Customers.Find(customerPK);
                customer.IsActive = true;
                db.Entry(customer).State = EntityState.Modified;

                // lưu activity update
                Activity activity = new Activity("reactive", customer.CustomerName, "Customer", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateConception(int customerPK, string conceptionCode, int year, string season, string description, string userID)
        {
            try
            {
                // create conception
                Conception conception = new Conception(conceptionCode, description, year, season, customerPK);
                db.Conceptions.Add(conception);

                // lưu activity create
                Activity activity = new Activity("create", conception.ConceptionCode, "Conception", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteConception(int conceptionPK, string userID)
        {
            try
            {
                ConceptionAccessory conceptionAccessory = (from ca in db.ConceptionAccessories
                                                           where ca.ConceptionPK == conceptionPK
                                                           select ca).FirstOrDefault();
                if (conceptionAccessory != null) throw new Exception("MÃ HÀNG ĐÃ CÓ GẮN PHỤ LIỆU, KHÔNG ĐƯỢC XÓA");
                // delete conception
                Conception conception = db.Conceptions.Find(conceptionPK);
                db.Conceptions.Remove(conception);

                // lưu activity delete
                Activity activity = new Activity("delete", conception.ConceptionCode, "Conception", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeactiveConception(int conceptionPK, string userID)
        {
            try
            {
                // deactive conception
                Conception conception = db.Conceptions.Find(conceptionPK);
                conception.IsActive = false;
                db.Entry(conception).State = EntityState.Modified;

                // lưu activity deactive
                Activity activity = new Activity("deactive", conception.ConceptionCode, "Conception", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ActiveConception(int conceptionPK, string userID)
        {
            try
            {
                // active conception
                Conception conception = db.Conceptions.Find(conceptionPK);
                conception.IsActive = true;
                db.Entry(conception).State = EntityState.Modified;

                // lưu activity active
                Activity activity = new Activity("reactive", conception.ConceptionCode, "Conception", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateSupplier(string name, string address, string phoneNumber, string supplierCode, string userID)
        {
            try
            {
                // create supplier
                Supplier supplier = new Supplier(name, address, phoneNumber, supplierCode);
                db.Suppliers.Add(supplier);

                // lưu activity create
                Activity activity = new Activity("create", supplier.SupplierName, "Supplier", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateSupplier(int supplierPK, string address, string phoneNumber, string userID)
        {
            try
            {
                // update conception
                Supplier supplier = db.Suppliers.Find(supplierPK);
                supplier.SupplierAddress = address;
                supplier.SupplierPhoneNumber = phoneNumber;
                db.Entry(supplier).State = EntityState.Modified;

                // lưu activity update
                Activity activity = new Activity("update", supplier.SupplierName, "Supplier", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteSupplier(int supplierPK, string userID)
        {
            try
            {
                Accessory accessory = (from a in db.Accessories
                                       where a.SupplierPK == supplierPK
                                       select a).FirstOrDefault();
                if (accessory != null) throw new Exception("CÓ PHỤ LIỆU ĐANG SỬ DỤNG CỦA NHÀ CUNG CẤP NÀY");
                // delete supplier
                Supplier supplier = db.Suppliers.Find(supplierPK);
                db.Suppliers.Remove(supplier);

                // lưu activity delete
                Activity activity = new Activity("delete", supplier.SupplierName, "Supplier", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeactiveSupplier(int supplierPK, string userID)
        {
            try
            {
                // deactive supplier
                Supplier supplier = db.Suppliers.Find(supplierPK);
                supplier.IsActive = false;
                db.Entry(supplier).State = EntityState.Modified;

                // lưu activity deactive
                Activity activity = new Activity("deactive", supplier.SupplierName, "Supplier", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ActiveSupplier(int supplierPK, string userID)
        {
            try
            {
                // active supplier
                Supplier supplier = db.Suppliers.Find(supplierPK);
                supplier.IsActive = true;
                db.Entry(supplier).State = EntityState.Modified;

                // lưu activity active
                Activity activity = new Activity("reactive", supplier.SupplierName, "Supplier", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateAccessory(string item, string art, string description, string comment, string color, int customerPK, int supplierPK, int accessoryTypePK, string userID)
        {
            try
            {
                string accessoryID = "";
                AccessoryType accessoryType = db.AccessoryTypes.Find(accessoryTypePK);
                Customer customer = db.Customers.Find(customerPK);
                Supplier supplier = db.Suppliers.Find(supplierPK);

                // check if accessoryType or customer or supplier is exist
                if (accessoryType == null || customer == null || supplier == null)
                {
                    throw new Exception("THÔNG TIN PHỤ LIỆU KHÔNG HỢP LỆ");
                }

                // tạo ID cho phụ liệu
                List<Accessory> accessories = (from acc in db.Accessories.OrderByDescending(unit => unit.AccessoryPK)
                                               where acc.AccessoryTypePK == accessoryTypePK && acc.CustomerPK == customerPK
                                               select acc).ToList();
                if (accessories.Count == 0)
                    accessoryID = accessoryType.Abbreviation + "-" + customer.CustomerCode + "-" + "00001" + "-" + supplier.SupplierCode;
                else
                {
                    string tempStr;
                    Int32 tempInt;
                    Accessory tmpAccessory = accessories.Where(unit => unit.SupplierPK == supplierPK).FirstOrDefault();
                    Accessory tmpAccessory2 = accessories.Where(unit => unit.Item == item).FirstOrDefault();
                    // check if item is duplicate to change rule of accID
                    if (tmpAccessory2 != null)
                    {
                        tmpAccessory = tmpAccessory2;
                        tempStr = tmpAccessory.AccessoryID.Substring(7, 5);
                        tempInt = Int32.Parse(tempStr);
                    }
                    else
                    {
                        tempStr = tmpAccessory.AccessoryID.Substring(7, 5);
                        tempInt = Int32.Parse(tempStr) + 1;
                    }


                    tempStr = tempInt + "";
                    if (tempStr.Length == 1) tempStr = "0000" + tempStr;
                    if (tempStr.Length == 2) tempStr = "000" + tempStr;
                    if (tempStr.Length == 3) tempStr = "00" + tempStr;
                    if (tempStr.Length == 4) tempStr = "0" + tempStr;
                    accessoryID = accessoryType.Abbreviation + "-" + customer.CustomerCode + "-" + tempStr + "-" + supplier.SupplierCode;
                }
                // create accessory
                Accessory accessory = new Accessory(accessoryID, description, item, art, color, comment, accessoryTypePK, supplierPK, customerPK);
                db.Accessories.Add(accessory);

                // lưu activity create
                Activity activity = new Activity("create", accessory.AccessoryID, "Accessory", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateAccessories(List<Accessory> inputAccessories, string userID)
        {
            try
            {
                HashSet<string> hsCheckID = new HashSet<string>();
                foreach (var a in inputAccessories)
                {
                    if (a.Color == null) a.Color = "";
                    if (a.Art.Length <= 25 && a.Color.Length <= 25 && a.AccessoryDescription.Length <= 25
                        && !PrimitiveType.isValidComment(a.Comment) && a.Item.Length <= 25 && a.Item != null && a.Item != "")
                    {
                        throw new Exception(SystemMessage.NotPassPrimitiveType);
                    }
                    if (a.Art == "undefined") a.Art = "";
                    if (a.Comment == "undefined") a.Comment = "";
                    if (a.Color == "undefined") a.Color = "";
                    string accessoryID = "";

                    // check if accessoryType or customer or supplier is exist
                    AccessoryType accessoryType = db.AccessoryTypes.Find(a.AccessoryTypePK);
                    Customer customer = db.Customers.Find(a.CustomerPK);
                    Supplier supplier = db.Suppliers.Find(a.SupplierPK);
                    if (accessoryType == null || customer == null || supplier == null)
                    {
                        throw new Exception("THÔNG TIN PHỤ LIỆU KHÔNG HỢP LỆ~AST-ERR~");
                    }

                    // tạo ID cho phụ liệu
                    List<Accessory> accessories = (from acc in db.Accessories.OrderByDescending(unit => unit.AccessoryPK)
                                                   where acc.AccessoryTypePK == a.AccessoryTypePK && acc.CustomerPK == a.CustomerPK && acc.SupplierPK == supplier.SupplierPK
                                                   select acc).ToList();
                    if (accessories.Count == 0)
                    {
                        accessoryID = accessoryType.Abbreviation + "-" + customer.CustomerCode + "-" + supplier.SupplierCode + "-" + "00001";
                    }
                    else
                    {
                        string tempStr;
                        Int32 tempInt;

                        tempStr = accessories[0].AccessoryID.Substring(accessories[0].AccessoryID.Length - 5);
                        tempInt = Int32.Parse(tempStr) + 1;

                        tempStr = tempInt + "";
                        if (tempStr.Length == 1) tempStr = "0000" + tempStr;
                        if (tempStr.Length == 2) tempStr = "000" + tempStr;
                        if (tempStr.Length == 3) tempStr = "00" + tempStr;
                        if (tempStr.Length == 4) tempStr = "0" + tempStr;
                        accessoryID = accessoryType.Abbreviation + "-" + customer.CustomerCode + "-" + supplier.SupplierCode + "-" + tempStr;
                    }

                    while (hsCheckID.Contains(accessoryID))
                    {
                        string tempStr;
                        Int32 tempInt;

                        tempStr = accessoryID.Substring(accessoryID.Length - 5);
                        tempInt = Int32.Parse(tempStr) + 1;

                        tempStr = tempInt + "";
                        if (tempStr.Length == 1) tempStr = "0000" + tempStr;
                        if (tempStr.Length == 2) tempStr = "000" + tempStr;
                        if (tempStr.Length == 3) tempStr = "00" + tempStr;
                        if (tempStr.Length == 4) tempStr = "0" + tempStr;
                        accessoryID = accessoryType.Abbreviation + "-" + customer.CustomerCode + "-" + supplier.SupplierCode + "-" + tempStr;
                    }

                    // create accessory
                    a.AccessoryID = accessoryID;
                    db.Accessories.Add(a);
                    hsCheckID.Add(accessoryID);

                    // lưu activity create
                    Activity activity = new Activity("create", a.AccessoryID, "Accessory", userID);
                    db.Activities.Add(activity);
                }

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public void UpdateAccessory(int accessoryPK, string comment, string userID)
        {
            try
            {
                // update accessory
                Accessory accessory = db.Accessories.Find(accessoryPK);
                accessory.Comment = comment;
                db.Entry(accessory).State = EntityState.Modified;

                // lưu activity update
                Activity activity = new Activity("update", accessory.AccessoryID, "Accessory", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void LinkConception(int accessoryPK, int conceptionPK, string userID)
        {
            try
            {
                // link conception
                Conception conception = db.Conceptions.Find(conceptionPK);
                Accessory accessory = db.Accessories.Find(accessoryPK);
                if (conception.CustomerPK != accessory.CustomerPK)
                {
                    throw new Exception("MÃ HÀNG VÀ ACCESSORY KHÔNG CÙNG CUSTOMER~AST-ERR~");
                }
                ConceptionAccessory conceptionAccessory = new ConceptionAccessory(conceptionPK, accessoryPK);
                db.ConceptionAccessories.Add(conceptionAccessory);

                // lưu activity link
                Activity activity1 = new Activity("link", conception.ConceptionCode, "Conception", userID);
                Activity activity2 = new Activity("link", accessory.AccessoryID, "Accessory", userID);
                db.Activities.Add(activity1);
                db.Activities.Add(activity2);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void LinkConception(List<int> accessoryPKs, int conceptionPK, string userID)
        {
            try
            {
                // link conception
                Conception conception = db.Conceptions.Find(conceptionPK);
                foreach (var accessoryPK in accessoryPKs)
                {
                    Accessory accessory = db.Accessories.Find(accessoryPK);
                    if (conception.CustomerPK != accessory.CustomerPK)
                    {
                        throw new Exception("MÃ HÀNG VÀ ACCESSORY KHÔNG CÙNG CUSTOMER~AST-ERR~");
                    }
                    ConceptionAccessory conceptionAccessory = new ConceptionAccessory(conceptionPK, accessoryPK);
                    db.ConceptionAccessories.Add(conceptionAccessory);

                    // lưu activity link
                    Activity activity1 = new Activity("link", conception.ConceptionCode, "Conception", userID);
                    Activity activity2 = new Activity("link", accessory.AccessoryID, "Accessory", userID);
                    db.Activities.Add(activity1);
                    db.Activities.Add(activity2);
                }

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UnlinkConception(int accessoryPK, int conceptionPK, string userID)
        {
            try
            {
                // unlink conception
                Conception conception = db.Conceptions.Find(conceptionPK);
                Accessory accessory = db.Accessories.Find(accessoryPK);
                if (conception.CustomerPK != accessory.CustomerPK)
                {
                    throw new Exception("MÃ HÀNG VÀ PHỤ LIỆU KHÔNG CÙNG KHÁCH HÀNG~AST-ERR~");
                }
                List<DemandedItem> demandedItems = (from dI in db.DemandedItems
                                                    where dI.AccessoryPK == accessoryPK
                                                    select dI).ToList();
                foreach (var item in demandedItems)
                {
                    Demand demand = db.Demands.Find(item.DemandPK);
                    if (demand.ConceptionPK == conception.ConceptionPK)
                    {
                        throw new Exception("MÃ HÀNG VÀ PHỤ LIỆU ĐÃ ĐƯỢC LIÊN KẾT VÀ SỬ DỤNG~AST-ERR~");
                    }
                }

                ConceptionAccessory conceptionAccessory = (from ca in db.ConceptionAccessories
                                                           where ca.AccessoryPK == accessoryPK && ca.ConceptionPK == conceptionPK
                                                           select ca).FirstOrDefault();
                db.ConceptionAccessories.Remove(conceptionAccessory);

                // lưu activity unlink
                Activity activity1 = new Activity("unlink", conception.ConceptionCode, "Conception", userID);
                Activity activity2 = new Activity("unlink", accessory.AccessoryID, "Accessory", userID);
                db.Activities.Add(activity1);
                db.Activities.Add(activity2);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeactiveAccessory(int accessoryPK, string userID)
        {
            try
            {
                // deactive accessory
                Accessory accessory = db.Accessories.Find(accessoryPK);
                accessory.IsActive = false;
                db.Entry(accessory).State = EntityState.Modified;

                // lưu activity deactive
                Activity activity = new Activity("deactive", accessory.AccessoryID, "Accessory", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ReactiveAccessory(int accessoryPK, string userID)
        {
            try
            {
                // active accessory
                Accessory accessory = db.Accessories.Find(accessoryPK);
                accessory.IsActive = true;
                db.Entry(accessory).State = EntityState.Modified;

                // lưu activity active
                Activity activity = new Activity("reactive", accessory.AccessoryID, "Accessory", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteAccessory(int accessoryPK, string userID)
        {
            try
            {
                // delete accessory
                Accessory accessory = db.Accessories.Find(accessoryPK);
                db.Accessories.Remove(accessory);

                // lưu activity delete
                Activity activity = new Activity("delete", accessory.AccessoryID, "Accessory", userID);
                db.Activities.Add(activity);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}

