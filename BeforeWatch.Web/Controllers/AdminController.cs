using BeforeWatch.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static BeforeWatch.Web.Controllers.HomeController;

namespace BeforeWatch.Web.Controllers
{
    public class AdminController : Controller
    {
        //ado.netten db nesnesi üretiyoruz
        BeforeWatchEntities db = new BeforeWatchEntities();

        //admin giriş sayfası
        public ActionResult Giris()
        {
            return View();
        }

        //admin girişi butonuna tıkladığımızda gidecek olan post fonksiyonumuz
        [HttpPost]
        public ActionResult Giris(string Email, string Password)
        {
            Admin admin = db.Admin.Where(w => w.Email == Email && w.Password == Password).FirstOrDefault();
            if (admin != null)
            {
                return RedirectToAction("FilmEkle");
            }
            return RedirectToAction("Giris");
        }

        #region film işlemleri
        //view e döneceğimiz açılır liste (dropdown list) için model oluşturuyoruz
        public class FilmEkleViewModel
        {
            public List<FilmSeries> FilmSeries;
            public List<SelectListItem> SecilebilirListe { get; set; }
            public string SecilenTip { get; set; }
        }
        
        //film ekle sayfamız
        public ActionResult FilmEkle()
        {
            //oluşturduğumuz modelden bir nesne türetiyoruz
            FilmEkleViewModel filmEkleViewModel = new FilmEkleViewModel();

            //tip tablosunu liste halinde çekip ön tarafa döneceğimiz modeldeki SecilebilirListe propertysine atıyoruz
            filmEkleViewModel.SecilebilirListe = db.Type.Select(s => new SelectListItem
            {
                //ismini text e atıyoruz
                Text = s.Name,
                //id sini value ye atıyoruz 
                Value = s.ID.ToString()
            })
            //listeye çeviriyoruz
            .ToList();

            //View e modeli dönüyoruz
            return View(filmEkleViewModel);
        }

        [HttpPost]
        public ActionResult FilmEkle(FilmSeries filmSeries)
        {

            //Post işleminden sonra gelen modeldeki değerleri filmseries tablosuna ekliyoruz
            db.FilmSeries.Add(new FilmSeries
            {
                Cast = filmSeries.Cast,
                Comment = filmSeries.Comment,
                Country = filmSeries.Country,
                Detail = filmSeries.Detail,
                Director = filmSeries.Director,
                Duration = filmSeries.Duration,
                YoutubeFragman = filmSeries.YoutubeFragman,
                Name = filmSeries.Name,
                ReleaseYear = filmSeries.ReleaseYear,
                IsActive = true,
                TypeID = filmSeries.TypeID,
                FilmOrSeries = filmSeries.FilmOrSeries,
            });

            //ekleme işlemini kaydediyoruz
            db.SaveChanges();
            //eğer fotoğraf yükleme talebi gelmişse
            if (Request.Files.Count > 0)
            {
                try
                {
                    //tablodan son kaydı getiriyoruz
                    var kayit = db.FilmSeries.OrderByDescending(o => o.ID).FirstOrDefault();
                    HttpPostedFileBase file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        //kaydın id si ile bir fotoğraf ismi oluşturuyoruz ki benzersiz olsun
                        string fileName = string.Concat(kayit.ID, ".jpg");
                        //fotoğrafı kaydedeceğimiz yolu belirtiyoruz
                        string path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        
                        //fotoğrafı o yola kaydediyoruz
                        file.SaveAs(path);
                        //kaydın PosterName kolununa dosya adımızı ekliyoruz
                        kayit.PosterName = fileName;
                        //db işlemlerini kaydediyoruz
                        db.SaveChanges();
                    }
                }
                //eğer hata yakalarsa buraya düşüyor
                catch (Exception exp)
                {

                }
            }

            //ekleme işlemi başarılı olduğu vakit buraya geliyor ve view e viewbag e benzer şekilde view data ile sonuç başarılı değerini dönüyoruz
            TempData["EklemeIslemi"] = "Sonuç Başarılı";

            //tekrardan film ekleme sayfasına yönlendiriyoruz
            return RedirectToAction("FilmEkle");
        }

        public ActionResult FilmDuzenle(int id)
        {
            //oluşturduğumuz modelden bir nesne türetiyoruz
            FilmEkleViewModel filmEkleViewModel = new FilmEkleViewModel();

            //tip tablosunu liste halinde çekip ön tarafa döneceğimiz modeldeki SecilebilirListe propertysine atıyoruz
            filmEkleViewModel.SecilebilirListe = db.Type.Select(s => new SelectListItem
            {
                //ismini text e atıyoruz
                Text = s.Name,
                //id sini value ye atıyoruz 
                Selected = true,
                Value = s.ID.ToString(),
            })
            //listeye çeviriyoruz
            .ToList();
            //tablodan düzenlenecek filmi çekiyoruz
            var FilmSeries = db.FilmSeries.Where(w => w.ID == id).FirstOrDefault();
            //eğer bu id ile bir film varsa yani çekilmişse yeni değerleri view e veri göndreceğimiz tempdata değişkenlerimize atıyoruz
            if (FilmSeries != null)
            {
                TempData["ID"] = FilmSeries.ID.ToString();
                TempData["IsActive"] = FilmSeries.IsActive;
                TempData["Cast"] = FilmSeries.Cast;
                TempData["Country"] = FilmSeries.Country;
                TempData["Detail"] = FilmSeries.Detail;
                TempData["Director"] = FilmSeries.Director;
                TempData["Duration"] = FilmSeries.Duration;
                TempData["Name"] = FilmSeries.Name;
                TempData["FilmOrSeries"] = FilmSeries.FilmOrSeries;
                TempData["TypeID"] = FilmSeries.TypeID;
                TempData["YoutubeFragman"] = FilmSeries.YoutubeFragman;
                TempData["ReleaseYear"] = FilmSeries.ReleaseYear;
                TempData["PosterName"] = FilmSeries.PosterName;

                //View e modeli dönüyoruz
                return View(filmEkleViewModel);
            }
            return RedirectToAction("FilmDuzenle");
        }

        [HttpPost]
        public ActionResult FilmDuzenle(FilmSeries filmSeries)
        {
            var duzenlenecekFilm = db.FilmSeries.Where(w => w.ID == filmSeries.ID).FirstOrDefault();

            //Post işleminden sonra gelen modeldeki değerleri duzenlenecekFilm kaydındakilerle değiştiriyoruz
            duzenlenecekFilm.Cast = filmSeries.Cast;
            duzenlenecekFilm.Comment = filmSeries.Comment;
            duzenlenecekFilm.Country = filmSeries.Country;
            duzenlenecekFilm.Detail = filmSeries.Detail;
            duzenlenecekFilm.Director = filmSeries.Director;
            duzenlenecekFilm.Duration = filmSeries.Duration;
            duzenlenecekFilm.YoutubeFragman = filmSeries.YoutubeFragman;
            duzenlenecekFilm.Name = filmSeries.Name;
            duzenlenecekFilm.ReleaseYear = filmSeries.ReleaseYear;
            duzenlenecekFilm.IsActive = true;
            duzenlenecekFilm.TypeID = filmSeries.TypeID;
            duzenlenecekFilm.FilmOrSeries = filmSeries.FilmOrSeries;

            //ekleme işlemini kaydediyoruz
            db.SaveChanges();

            //foto yükleme talebi var ise
            if (Request.Files.Count > 0)
            {
                try
                {
                    //ilgili kaydı db den getiyoruz
                    var kayit = db.FilmSeries.Where(w => w.ID == filmSeries.ID).FirstOrDefault();
                    //dosya ismini kaydımızın id'si .jpg şeklinde veriyoruz
                    string fileName = string.Concat(kayit.ID, ".jpg");
                    //nereye kaydedeceğimizi seçiyoruz
                    string path = Path.Combine(Server.MapPath("~/Images/"), fileName);

                    //istekteki dosyayı alır file a atar
                    HttpPostedFileBase file = Request.Files[0];

                    //dosya varsa
                    if (file != null && file.ContentLength > 0)
                    {
                        //eğer daha önceden afiş eklenmişse
                        if (System.IO.File.Exists(path))
                        {
                            //eskisini siler
                            System.IO.File.Delete(path);
                        }
                        //tablodan son kaydı getiriyoruz
                        //db'deki kaydın postername kolununa dosya ismimizi ekliyoruz
                        kayit.PosterName = fileName;

                        //afişi klasöre ekler
                        file.SaveAs(path);

                        db.SaveChanges();
                    }
                }
                catch (Exception exp)
                {

                }
            }

            //ekleme işlemi başarılı olduğu vakit buraya geliyor ve view e viewbag e benzer şekilde view data ile sonuç başarılı değerini dönüyoruz
            TempData["EklemeIslemi"] = "Sonuç Başarılı";

            //tekrardan film ekleme sayfasına yönlendiriyoruz
            return RedirectToAction("FilmDuzenle", new { id =filmSeries.ID });
        }

        //view e döneceğimiz açılır liste (dropdown list) için model oluşturuyoruz
        public class FilmDiziViewModel
        {
            public List<FilmSeries> FilmSeries;
            public List<SelectListItem> SecilebilirListe { get; set; }
            public string SecilenDiziFilm { get; set; }
        }

        public ActionResult DuzenlenecekFilm()
        {
            //nesne türettik içine ulaşabilmek için
            FilmDiziViewModel filmDiziViewModel = new FilmDiziViewModel();

            //filmler ve diziler tablosunu liste halinde çekip ön tarafa döneceğimiz modeldeki SecilebilirListe propertysine atıyoruz
            filmDiziViewModel.SecilebilirListe = db.FilmSeries.Select(s => new SelectListItem
            {
                Text = string.Concat("Adı: ", s.Name, " | Tipi: ", s.FilmOrSeries, " | Türü: ", s.Type.Name, " | Oyuncular: " + s.Cast),
                Value = s.ID.ToString()
            }).ToList();

            //View e modeli dönüyoruz
            return View(filmDiziViewModel);
        }

        public ActionResult FilmSil(int id)
        {
            var silinecekFilm = db.FilmSeries.Where(w => w.ID == id).FirstOrDefault();

            db.FilmSeries.Remove(silinecekFilm);

            db.SaveChanges();

            //düzenlenecek film sayfasına gönderiyoruz
            return RedirectToAction("DuzenlenecekFilm");
        }
        #endregion

        #region yorum işlemleri
        public ActionResult OnaysizYorumlar()
        {
            List<Comment> yorumlar = db.Comment.Where(w => w.IsActive == false).ToList();

            //View e modeli dönüyoruz
            return View(yorumlar);
        }

        public ActionResult YorumOnayla(int id)
        {
            //aktif edilecek yorumu seçiyoruz
            Comment yorum = db.Comment.Where(w => w.ID == id).FirstOrDefault();
            yorum.IsActive = true;
            db.SaveChanges();

            //onaysiz yorumları listeleyecek controllera yönlendiriyoruz
            return RedirectToAction("OnaysizYorumlar");
        }

        public ActionResult OnayliYorumlar()
        {
            List<Comment> yorumlar = db.Comment.Where(w => w.IsActive == true).ToList();

            //View e modeli dönüyoruz
            return View(yorumlar);
        }

        public ActionResult YorumSil(int id)
        {
            //silenecek yorumu seçiyoruz
            Comment yorum = db.Comment.Where(w => w.ID == id).FirstOrDefault();
            db.Comment.Remove(yorum);
            db.SaveChanges();

            //onaysiz yorumları listeleyecek controllera yönlendiriyoruz
            return RedirectToAction("OnayliYorumlar");
        }
        #endregion

        #region admin işlemleri
        //buradaki metodlar mvc nin otomatik iskeletleme (scaffolding) özelliği ile oluşturuldu
        public ActionResult List()
        {
            return View(db.Admin.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admin.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }
        
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Firstname,Lastname,Email,Password,Birthday,IsActive")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Admin.Add(admin);
                db.SaveChanges();
                return RedirectToAction("List");
            }

            return View(admin);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admin.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Firstname,Lastname,Email,Password,Birthday,IsActive")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("List");
            }
            return View(admin);
        }
        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admin.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Admin admin = db.Admin.Find(id);
            db.Admin.Remove(admin);
            db.SaveChanges();
            return RedirectToAction("List");
        }
        #endregion

        #region kullanıcı işlemleri
        //buradaki metodlar mvc nin otomatik iskeletleme (scaffolding) özelliği ile oluşturuldu
        public ActionResult ListUser()
        {
            return View(db.User.ToList());
        }

        public ActionResult DetailsUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser([Bind(Include = "ID,Firstname,Lastname,Email,Password,Birthday,IsActive")] User user)
        {
            if (ModelState.IsValid)
            {
                db.User.Add(user);
                db.SaveChanges();
                return RedirectToAction("ListUser");
            }

            return View(user);
        }

        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "ID,Firstname,Lastname,Email,Password,Birthday,IsActive")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListUser");
            }
            return View(user);
        }

        public ActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedUser(int id)
        {
            User user = db.User.Find(id);
            db.User.Remove(user);
            db.SaveChanges();
            return RedirectToAction("ListUser");
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
        #endregion
    }
}