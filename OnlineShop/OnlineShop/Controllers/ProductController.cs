using OnlineShop.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class ProductController : Controller
    {
        private OnlineShopDbContext db = new OnlineShopDbContext();
        // GET: Product
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var model = db.Products.Where(x => x.Status == true).OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
            return View(model);
        }

        // GET: Admin/Product/Create
        public ActionResult Add()
        {
            ViewBag.lstCategory = db.Categories.ToList();
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Product entity, HttpPostedFileBase Image)
        {
            try
            {
                var pro = new Product();
                pro.Product_Name = entity.Product_Name;
                pro.Metatitle = Str_Metatitle(entity.Product_Name);
                pro.Promotion_Price = entity.Promotion_Price;
                pro.Price = entity.Price;
                pro.Category_ID = entity.Category_ID;
                pro.Quantity = entity.Quantity;
                pro.Status = true;
                //Thêm hình ảnh
                var path = Path.Combine(Server.MapPath("~/Assets/img/product"), Image.FileName);
                if (System.IO.File.Exists(path))
                {
                    string extensionName = Path.GetExtension(Image.FileName);
                    string filename = Image.FileName + DateTime.Now.ToString("ddMMyyyy") + extensionName;
                    path = Path.Combine(Server.MapPath("~/Assets/img/product/"), filename);
                    Image.SaveAs(path);
                    pro.Image = filename;
                }
                else
                {
                    Image.SaveAs(path);
                    pro.Image = Image.FileName;
                }

                db.Products.Add(pro);
                db.SaveChanges();
                TempData["add_success"] = "Thêm sản phẩm thành công.";
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Add");
            }
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(long ID)
        {
            ViewBag.product = db.Products.Find(ID);
            ViewBag.lstCategory = db.Categories.ToList();
            return View();
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Product entity, HttpPostedFileBase Image)
        {
            try
            {
                var pro = db.Products.Find(entity.ID);
                pro.Product_Name = entity.Product_Name;
                pro.Metatitle = Str_Metatitle(entity.Product_Name);
                pro.Promotion_Price = entity.Promotion_Price;
                pro.Price = entity.Price;
                pro.Category_ID = entity.Category_ID;
                pro.Quantity = entity.Quantity;
                try
                {
                    if (Image != null && pro.Image != Image.FileName)
                    {
                        //Xóa file cũ
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/img/product"), pro.Image));
                        //Thêm hình ảnh
                        var path = Path.Combine(Server.MapPath("~/Assets/img/product"), Image.FileName);
                        if (System.IO.File.Exists(path))
                        {
                            string extensionName = Path.GetExtension(Image.FileName);
                            string filename = Image.FileName + DateTime.Now.ToString("ddMMyyyy") + extensionName;
                            path = Path.Combine(Server.MapPath("~/Assets/img/product/"), filename);
                            Image.SaveAs(path);
                            pro.Image = filename;
                        }
                        else
                        {
                            Image.SaveAs(path);
                            pro.Image = Image.FileName;
                        }
                    }
                    db.SaveChanges();
                    TempData["add_success"] = "Sửa sản phẩm thành công.";
                    return RedirectToAction("Index");
                }
                catch
                {
                    db.SaveChanges();
                    TempData["add_success"] = "Sửa sản phẩm thành công.";
                    return RedirectToAction("Index");
                }




            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Product/Delete/5
        public JsonResult Delete(long ID)
        {
            try
            {
                var product = db.Products.Find(ID);
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/img/product"), product.Image));

                db.Products.Remove(product);
                db.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            catch
            {
                return Json(new
                {
                    status = false
                });
            }

        }

        public JsonResult ListName(string q)
        {
            var query = db.Products.Where(x => x.Product_Name.Contains(q)).Select(x => x.Product_Name);
            //var query = from pro in db.Products
            //            where pro.Product_Name.Contains(q)
            //            select new
            //            {
            //                pro.Product_Name,
            //                pro.Image
            //            };
            return Json(new
            {
                data = query,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string keyword, int page = 1, int pagesize = 5)
        {
            string[] key = keyword.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var product_name = new List<Product>();//Tìm theo tên sản phẩm
            foreach (var item in key)
            {
                product_name = (from b in db.Products
                                where b.Product_Name.Contains(item)
                                select b).ToList();
            }
            ViewBag.KeyWord = keyword;
            return View(product_name.ToPagedList(page, pagesize));
        }

        //Chuyển tên sản phẩm thành metatitle
        public string Str_Metatitle(string str)
        {
            string[] VietNamChar = new string[]
            {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ:/"
            };
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            string str1 = str.ToLower();
            string[] name = str1.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string meta = null;
            //Thêm dấu '-'
            foreach (var item in name)
            {
                meta = meta + item + "-";
            }
            return meta;
        }
    }
}