using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            //Initialize the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Check if cart is empty
            if(cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty!";
                return View();
            }

            //Calculate total and save in viewbag

            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            //Return view with list
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            //Initialize CartVM
            CartVM model = new CartVM();

            //Initialize Quantity
            int quantity = 0;

            //Initialize Price
            decimal price = 0m;

            //Check for cart session
            if(Session["cart"] != null)
            {
                var list = (List<CartVM>)Session["cart"];

                //Get total quantity and price
                foreach (var item in list)
                {
                    quantity += item.Quantity;
                    price += item.Quantity * item.Price;
                }
                model.Quantity = quantity;
                model.Price = price;
            }
            else
            {
                //Or set quantity and price 0
                model.Quantity = 0;
                model.Price = 0;
            }



            //Return partial View with model
            return PartialView(model);
        }

        public ActionResult AddToPartial(int id)
        {
            //Initialize CartVM list
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Intialize cartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                //Get the Product
                ProductDTO product = db.Products.Find(id);

                //Check if the product is already there in cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                //If not there, add new
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    //If it is increement
                    productInCart.Quantity++;
                }
            }

            //Get total quantity and add to model
            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            //Save cart back to session
            Session["cart"] = cart;

            //Return partial view with model
            return PartialView(model);
        }

        //GET: /cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            //Initialize cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Get cartVM from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Increement quantity
                model.Quantity++;

                //store the data
                var result = new { qty = model.Quantity, price = model.Price };

                //return json with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            
        }

        //GET: /cart/DecrementProduct

        public JsonResult DecrementProduct(int productId)
        {
            //Initialize cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Get cartVM from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Decrement quantity
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }
                

                //store the data
                var result = new { qty = model.Quantity, price = model.Price };

                //return json with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //GET: /cart/RemoveProduct

        public void RemoveProduct(int productId)
        {
            //Initialize cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Get model from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Remove model from list
                cart.Remove(model);

            }
        }

    }    
}