using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using WebBanlaptop.Models;
using PagedList;
using PagedList.Mvc;
using System.Collections.Generic;
using System.Data.Entity.Migrations;

namespace WebBanlaptop.Controllers
{
    public class AdminController : Controller
    {
        
        public ActionResult index()
        {
            if (!checkLogin()) return RedirectToAction("Login"); // session null thì bắt đăng nhập
            return View();
        }
        private bool checkLogin() // check đã có người dung đăng nhập chưa nếu chưa thì return false
        {
            NHANVIEN admin = Session["Admin"] as NHANVIEN;
            if (admin == null)
            {
                Session["Admin"] = null;
                return false;
            }
            return true;
        }
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        #region đăng nhập, đăng xuất
        [HttpGet]
        public ActionResult Login()
        {
            if (checkLogin()) return RedirectToAction("index"); // nếu có lưu session đăng nhập thì vào luôn web
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            QLBANLAPTOPEntities db = new QLBANLAPTOPEntities();
            var username = f["username"];
            var password = f["password"];
            var admin = db.NHANVIEN.SingleOrDefault(p => p.USERNAME == username);
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                ViewData["Error"] = "Vui Lòng Điền Đầy Đủ Nội Dung";
                return this.Login();
            }
            else if (admin == null)
            {
                ViewData["Error"] = "Sai Tài Khoản";
                return this.Login();
            }
            else if (!String.Equals(MD5Hash(password), admin.PASSWORD))
            {
                ViewData["Error"] = "Sai Mật Khẩu";
                return this.Login();
            }
            else if (!admin.TRANGTHAI)
            {
                ViewData["Error"] = "Tài khoản Admin này đang bị khóa. Vui lòng liên hệ chủ shop.";
                return this.Login();
            }
            else
            {
                Session["Admin"] = admin;
                return RedirectToAction("index","Admin");
            }
        }
        public ActionResult LogOut()
        {
            Session["Admin"] = null;
            return RedirectToAction("Login");
        }
        #endregion

        #region phần admins
        public ActionResult ListAdmin(int? page)
        {
            QLBANLAPTOPEntities db = new QLBANLAPTOPEntities();
            if (!checkLogin()) return RedirectToAction("Login");
            List<NHANVIEN> listAdmin = db.NHANVIEN.ToList();
            ViewBag.lstnv = listAdmin;
            return View();
        }
        [HttpGet]
        public ActionResult addAdmin()
        {
            if (!checkLogin()) return RedirectToAction("Login");
            return View();
        }
        [HttpPost]
        public ActionResult addAdmin(NHANVIEN nv)
        {
            QLBANLAPTOPEntities db = new QLBANLAPTOPEntities();
            var checkNV = db.NHANVIEN.FirstOrDefault(n => n.USERNAME == nv.USERNAME);
            if(checkNV != null)
            {
                ViewData["Error"] = "Tài khoản admin đã tồn tại";
                return this.addAdmin();
            }
            else
            {
                try
                {
                    nv.PASSWORD = MD5Hash(nv.PASSWORD);
                    nv.TRANGTHAI = true;
                    nv.BIGBOSS = false;
                    db.NHANVIEN.Add(nv);
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting  
                            // the current instance as InnerException  
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }

            }
            return RedirectToAction("ListAdmin","Admin");
        }
        public JsonResult LockOrUnlockAdmin(string id)
        {
            bool lockOrUnlock = true; // khóa hay mở khóa tài khoản (true: khóa, false: mở khóa)
            QLBANLAPTOPEntities db = new QLBANLAPTOPEntities();
            var admin = db.NHANVIEN.Single(p => p.USERNAME.Equals(id));
            bool Status = false;
            string err = string.Empty;
            if (admin != null)
            {
                if ((bool)admin.BIGBOSS) // check chủ sỡ hữu
                    return Json(new
                    {
                        status = false,
                        exit = "Không thể khóa chủ sở hữu"
                    });
                if (admin.TRANGTHAI) admin.TRANGTHAI = false; // nếu đang hoạt động thì khóa
                else
                {
                    admin.TRANGTHAI = true;
                    lockOrUnlock = false; // mở khóa tài khoản
                }

                try
                {
                    db.SaveChanges();
                    Status = true;
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                }
            }

            return Json(new
            {
                status = Status,
                errorMessage = err,
                lockStatus = lockOrUnlock
            });

        }
        [HttpGet]
        public ActionResult changePWAdmin(int id, bool type)
        {
            if (!checkLogin()) return RedirectToAction("Login");
            QLBANLAPTOPEntities db = new QLBANLAPTOPEntities();
            var checkNV = db.NHANVIEN.FirstOrDefault(n => n.MANV == id);
            if (type)
            {
                ViewBag.type = true;
                return View(checkNV);
            }
            else
            {
                ViewBag.type = false;
                return View(checkNV);
            }
        }
        [HttpPost]
        public ActionResult changePWAdmin(NHANVIEN nv, bool type,FormCollection f,int id)
        {
            QLBANLAPTOPEntities db = new QLBANLAPTOPEntities();
            var checkNV = db.NHANVIEN.FirstOrDefault(n => n.MANV == id);
            if (type)
            {
                try
                {
                    checkNV.PASSWORD = MD5Hash(nv.PASSWORD);
                    db.NHANVIEN.AddOrUpdate(checkNV);
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting  
                            // the current instance as InnerException  
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
            }
            else
            {
                var xnmk = f["XNPASSWORD"].ToString();
                if (!nv.PASSWORD.Equals(xnmk))
                {
                    ViewData["Error"] = "Tài khoản admin đã tồn tại";
                    return this.changePWAdmin(nv.MANV,type);
                }
                else
                {
                    try
                    {
                        checkNV.PASSWORD = MD5Hash(nv.PASSWORD);
                        db.NHANVIEN.AddOrUpdate(checkNV);
                        db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                                // raise a new exception nesting  
                                // the current instance as InnerException  
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                }
                
            }
                
            return RedirectToAction("ListAdmin", "Admin");
        }
        #endregion

        #region phần users

        #endregion
        #region phần quản lý góp ý
        public ActionResult ListGopy()
        { 
            QLBANLAPTOPEntities db = new QLBANLAPTOPEntities();
            if (!checkLogin()) return RedirectToAction("Login");
            List<GOPY> listgopy = db.GOPY.ToList();
            ViewBag.lstgy = listgopy;
            return View();
        }
        public JsonResult LockOrUnlockGopY(int? id)
        {
            bool lockOrUnlock = true; // khóa hay mở khóa tài khoản (true: khóa, false: mở khóa)
            QLBANLAPTOPEntities db = new QLBANLAPTOPEntities();
            var gopy = db.GOPY.Single(p => p.MAGOPY==id);
            bool Status = false;
            string err = string.Empty;
            if (gopy != null)
            {
                
                if (gopy.TRANGTHAI) gopy.TRANGTHAI = false; // nếu đang hoạt động thì khóa
                else
                {
                    gopy.TRANGTHAI = true;
                    lockOrUnlock = false; // mở khóa tài khoản
                }

                try
                {
                    db.SaveChanges();
                    Status = true;
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                }
            }

            return Json(new
            {
                status = Status,
                errorMessage = err,
                lockStatus = lockOrUnlock
            });

        }

        [HttpGet]
        public ActionResult phanhoiGopy()
        {
            return View();
        }


        #endregion
    }
}