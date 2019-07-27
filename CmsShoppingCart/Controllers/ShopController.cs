using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            //Declare list of CategoryVM
            List<CategoryVM> categoryVMList;

            //Initialize the list
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            //Return view with List
            return PartialView(categoryVMList);
        }

        // GET: /shop/category/name
        public ActionResult Category(string name)
        {
            //Declare a list of ProductVM
            List<ProductVM> productVMList;

            using (Db db = new Db())
            {
                //Get Category Id
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int id = categoryDTO.Id;

                //Initialize the list
                productVMList = db.Products.ToArray().Where(x => x.CategoryId == id).Select(x => new ProductVM(x)).ToList();

                //Get the Category Name
                var productCat = db.Products.Where(x => x.CategoryId == id).FirstOrDefault();
                ViewBag.CategoryName = productCat.CategoryName;
            }
            //Return view with List
            return View(productVMList);
        }

        // GET: /shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            //Declare the VM and DTO
            ProductVM model;
            ProductDTO dto;

            //Intialize product Id
            int id = 0;

            using (Db db = new Db())
            {
                //Check product exists
                if(! db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Shop", "Index");
                }

                //Initialize ProductDTO
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                //Get inserted Id
                id = dto.Id;

                //Initialize model
                model = new ProductVM(dto);
            }
            //Get gallery image
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Image/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs"))
                                            .Select(fn => Path.GetFileName(fn));

            //Return view with model
            return View("ProductDetails", model);
        }
    }
}