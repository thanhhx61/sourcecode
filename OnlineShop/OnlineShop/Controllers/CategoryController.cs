using OnlineShop.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class CategoryController : Controller
    {
        private OnlineShopDbContext db = new OnlineShopDbContext();
        // GET: Category
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var model = db.Categories.Where(x => x.Status == true).OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
            ViewBag.lstProduct = db.Products.ToList();
            return View(model);
        }

        public JsonResult Delete(long ID)
        {

            try
            {
                var cate = db.Categories.Find(ID);
                db.Categories.Remove(cate);
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


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult addCate(Category entity)
        {
            try
            {
                var prv = new Category();
                prv.Name = entity.Name;
                prv.Link = Str_Metatitle(entity.Name);
                prv.Status = true;
                db.Categories.Add(prv);
                db.SaveChanges();
                TempData["add_success"] = "Thêm danh mục sản phẩm thành công";
                return RedirectToAction("Index");

            }
            catch
            {
                TempData["add_success"] = "Thêm danh mục sản phẩm KHÔNG thành công";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult editCate(Category entity)
        {
            try
            {
                var prv = db.Categories.Find(entity.ID);
                prv.Name = entity.Name;
                prv.Link = Str_Metatitle(entity.Name);
                db.SaveChanges();
                TempData["add_success"] = "Sửa danh mục sản phẩm thành công";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["add_success"] = "Sửa danh mục sản phẩm KHÔNG thành công";
                return RedirectToAction("Index");
            }
        }

        public JsonResult GetCateByID(long ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var cate = db.Categories.Find(ID);
            return Json(cate, JsonRequestBehavior.AllowGet);
        }


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