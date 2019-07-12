using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            //Declare List Models
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {

                //Intialize the List
                categoryVMList = db.Categories.ToArray().
                                    OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            //Return View with List
            return View(categoryVMList);
        }

        //POST: Admin/Shop/AddNewCategory

        [HttpPost]
        public string AddNewCategory(string catName)
        {

            //Declare id
            string id;

            using (Db db = new Db())
            {

                //Check the category name is unique
                if(db.Categories.Any(x=>x.Name == catName))
                {
                    return "titletaken";
                }

                //Initialize DTO
                CategoryDTO dto = new CategoryDTO();

                //Add to DTO
                dto.Name = catName;
                dto.Slug = catName.Trim().Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                //save DTO
                db.Categories.Add(dto);
                db.SaveChanges();

                //Get the id
                id = dto.Id.ToString();
            }

            //Return the id

            return id;
        }

        //POST: Admin/Shop/ReorderCategories

        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {

                //set intial count
                int cout = 1;

                //Declare CatergoryDTO
                CategoryDTO dto;

                //set sort for each page
                foreach (var catid in id)
                {
                    dto = db.Categories.Find(catid);
                    dto.Sorting = cout;

                    db.SaveChanges();

                    cout++;
                }
            }
        }


        //GET : Admin/Shop/DeleteCategory/id

        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {

                //Get Category
                CategoryDTO dto = db.Categories.Find(id);

                //Remove Page
                db.Categories.Remove(dto);

                //Save
                db.SaveChanges();

            }

            //redirect
            return RedirectToAction("Categories");
        }

        //POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public string RenameCategory(string newCatName,int id)
        {
            using (Db db = new Db())
            {
                //check category name unique
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                //get DTO
                CategoryDTO dto = db.Categories.Find(id);

                //Edit DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                //Save
                db.SaveChanges();
            }

            //Return
            return "success";
        }

    }
}