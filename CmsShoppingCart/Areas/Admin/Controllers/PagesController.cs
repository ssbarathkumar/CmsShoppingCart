﻿using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Declare list of Pages
            List<PageVM> pagesList;

            //Initialize the list
            using (Db db = new Db())
            {
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

                //Return view with list


                return View(pagesList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {

            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Check Model State
            if(! ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                //Declare slug
                string slug;

                //Initialize PageDTO
                PageDTO dto = new PageDTO();

                //DTO title
                dto.Title = model.Title;

                //Check and set slug if needed
                if(string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //check slug and title are unique
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "The Title or Slug is already present!");
                    return View(model);
                }

                //DTO rest
                
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //save DTO
                db.Pages.Add(dto);
                db.SaveChanges();

            }

            //set Temporary message
            TempData["SM"] = "You have added a new page.";

            //Redirect
            return RedirectToAction("AddPage");
        }

        // GET: Admin/Pages/EditPage/id
        public ActionResult EditPage(int id)
        {
            //Declare PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Get the page
                PageDTO dto =  db.Pages.Find(id);

                //Confirm the page exsists
                if(dto==null)
                {
                    return Content("The does not exsist.");
                }

                //Initialize PageVM
                model = new PageVM(dto);
            }
                //return view with Model
                return View(model);
        }
    }
}