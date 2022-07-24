using BeforeWatch.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BeforeWatch.Web.Controllers
{
    [Authorize]
    public class KullaniciController : Controller
    {
        //ado.netten db nesnesi üretiyoruz
        BeforeWatchEntities db = new BeforeWatchEntities();

        [AllowAnonymous]
        public ActionResult Giris()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GirisYap(string Email, string Password)
        {
            User kullanici = new User();
            kullanici = db.User.Where(w => w.Email == Email && w.Password == Password).FirstOrDefault();
            if (kullanici != null)
            {
                //resetleyelim daha önceden giriş yapmış olan varsa
                AnaController.SuankiKullanicininTamAdi = null;
                AnaController.SuankiKullanicininEmaili = null;
                //yenisini setleyim
                FormsAuthentication.SetAuthCookie(kullanici.Email, false);

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Giris");
        }

        [AllowAnonymous]
        public ActionResult Kayit()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult KayitOl(User kullanici)
        {
            kullanici.IsActive = true;
            db.User.Add(kullanici);
            //kaydediyoruz
            db.SaveChanges();
            return RedirectToAction("Giris");
        }

        public ActionResult Cikis()
        {
            //Her şeyi siler
            string[] myCookies = Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
            return RedirectToAction("Giris");
        }

        public class GecmisViewModel
        {
            public List<Comment> YorumListesi { get; set; }
        }

        public ActionResult Gecmis()
        {
            GecmisViewModel gecmisViewModel = new GecmisViewModel();
            //sondan başa doğru kullanıcının ilgili yorumlarını getiriyoruz
            gecmisViewModel.YorumListesi = db.Comment.Where(w => w.UserID == AnaController.SuankiKullanicininIDsi).OrderByDescending(o => o.ID).ToList();
            //view e bu yorumları dönüyoruz
            return View(gecmisViewModel);
        }

        public ActionResult YorumSil(int id)
        {
            //silinecek yorumu seçiyoruz
            Comment yorum = db.Comment.Where(w => w.ID == id).FirstOrDefault();
            //db'den kaldırıyoruz
            db.Comment.Remove(yorum);
            //db'ye kaydediyoruz
            db.SaveChanges();

            //yorumları listeleyecek controllera yönlendiriyoruz
            return RedirectToAction("Gecmis");
        }

        public ActionResult YorumDetayi(int id)
        {
            //getirilecek yorumu seçiyoruz
            Comment yorum = db.Comment.Where(w => w.ID == id).FirstOrDefault();

            //yorumları listeleyecek controllera yönlendiriyoruz
            return View(yorum);
        }

        [HttpPost]
        public ActionResult YorumDegistir(int id, string yeniYorum)
        {
            //getirilecek yorumu seçiyoruz
            Comment yorum = db.Comment.Where(w => w.ID == id).FirstOrDefault();
            //yorum değiştiği için aktiflikten çıkıyor ve admin onayına gidiyor film sayfasında artık gözükmeyecek
            yorum.IsActive = false;
            //yeni yorumu db'ye kaydediyoruz
            yorum.Comment1 = yeniYorum;

            db.SaveChanges();

            //yorumları listeleyecek controllera yönlendiriyoruz
            return RedirectToAction("Gecmis");
        }

        public ActionResult Profil()
        {
            //ilgili kullanıcının profil sayfasını getiriyoruz
            User profil = db.User.Where(w => w.ID == AnaController.SuankiKullanicininIDsi).FirstOrDefault();

            return View(profil);
        }

        [HttpPost]
        public ActionResult Profil(User user)
        {
            User profil = db.User.Where(w => w.ID == AnaController.SuankiKullanicininIDsi).FirstOrDefault();

            profil.Birthday = user.Birthday;
            profil.Email = user.Email;
            profil.Firstname = user.Firstname;
            profil.Lastname = user.Lastname;
            profil.Password = user.Password;
            profil.IsActive = true;

            //db kayıt işlemi
            db.SaveChanges();
            //profil sayfasına dön
            return RedirectToAction("Profil");
    }

}
}
