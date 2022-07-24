using BeforeWatch.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BeforeWatch.Web.Controllers
{
    public class AnaController : Controller
    {
        BeforeWatchEntities db = new BeforeWatchEntities();

        User suankiKullanici = new User();

        public static string SuankiKullanicininTamAdi { get; set; }
        public static string SuankiKullanicininEmaili { get; set; }
        public static int SuankiKullanicininIDsi { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)

        {
            if (HttpContext.Request.IsAuthenticated)
            {

                var suankiKullanici = db.User.Where(w => w.IsActive == true && w.Email == /*FormsAuthentication.SetAuthCookie(kullanici.Email, false); bunu alır*/ filterContext.HttpContext.User.Identity.Name/*Email i getiriyor*/).FirstOrDefault();

                if (suankiKullanici != null)
                {
                    SuankiKullanicininTamAdi = string.Concat(suankiKullanici.Firstname, " ", suankiKullanici.Lastname);

                    SuankiKullanicininEmaili = suankiKullanici.Email;

                    SuankiKullanicininIDsi = suankiKullanici.ID;
                }
            }
        }

    }
}