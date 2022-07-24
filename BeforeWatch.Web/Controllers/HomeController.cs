using BeforeWatch.Web.Models;
using BeforeWatch.Web.Eklentiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BeforeWatch.Web.Controllers
{
    [Authorize]
    public class HomeController : AnaController
    {
        BeforeWatchEntities db = new BeforeWatchEntities();

        //view e döneceğimiz açılır liste (dropdown list) için model oluşturuyoruz
        public class FilmDiziViewModel
        {
            public List<FilmSeries> FilmSeries;
            public List<SelectListItem> SecilebilirListe { get; set; }
            public string SecilenDiziFilm { get; set; }
        }

        public ActionResult Index()
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

        //film detayları (search) sayfası için kullanacağımız view modeli oluşturduk
        public class SearchViewModel
        {
            public FilmSeries secilenFilm { get; set; }
            public Comment[] oyVerenKullaniciSayisi { get; set; }
            public List<Comment> YorumListesi { get; set; }
        }

        //filmid parametresi ile filmin detaylarını ve yorumlarını getiren fonksiyonumuz
        public ActionResult Search(int filmId)
        {
            //modelimizden obje türettik
            SearchViewModel searchViewModel = new SearchViewModel();

            //modelin secilenFilm property sine filmseries tablosunda filmidsi olan filmi atadık
            searchViewModel.secilenFilm = db.FilmSeries.Where/*linq sorgusu*/(nerede => nerede.ID == filmId).FirstOrDefault();

            //yorumlar tablosundaki filme ait yorumları modelimizin property sine atadık
            searchViewModel.oyVerenKullaniciSayisi = db.Comment.Where(nerede => nerede.FilmSeriesID == filmId).ToArray();

            //yorumları saydık
            int toplamOySayisi = searchViewModel.oyVerenKullaniciSayisi.Count();

            //default değişken
            int toplam = 0;
            //filme ait toplam oy sayısını bulduk
            for (int i = 0; i < toplamOySayisi; i++)
            {
                toplam = toplam + searchViewModel.oyVerenKullaniciSayisi[i].Score;
            }
            //bunu viewbag ile ön tarafa gönderdik
            ViewBag.oyVerenKullaniciSayisi = toplamOySayisi;

            //oy yoksa ortalaması 0 dır 0 'a bölüm hata vereceği için bunu yazmak zorunda kaldık
            if (toplam == 0 || toplamOySayisi == 0)
            {
                ViewBag.ortalamaSkor = 0;
            }
            //toplam oy sıfırdan farklı ise ortalama oyu hesapladık
            else
            {
                ViewBag.ortalamaSkor = (toplam / toplamOySayisi);
            }

            //yorumlar tablosundaki filme ait yorumları modelimizin property sine liste şeklinde atadık view de bunu kullnacağız
            searchViewModel.YorumListesi = db.Comment.Where(w => w.FilmSeriesID == filmId && w.IsActive == true).OrderByDescending(o => o.ID).ToList();

            return View(searchViewModel);
        }

        [HttpPost]
        public JsonResult YorumGonder(Comment comment)
        {
            try
            {
                Comment comment1 = new Comment();
                comment1.Comment1 = comment.Comment1;
                comment1.Score = comment.Score;
                comment1.UserID = AnaController.SuankiKullanicininIDsi;
                comment1.IsActive = false;
                comment1.FilmSeriesID = comment.FilmSeriesID;

                db.Comment.Add(comment1);
                db.SaveChanges();

                return Json(new { SonucTipi = 1});
            }
            catch (Exception)
            {
                return Json(new { SonucTipi = 0 });
            }

        }

        public ActionResult RastgeleFilm()
        {
            Random random = new Random();

            //tüm filmler diziye atılır
            int[] tumIDler = db.FilmSeries
                //eğer favori tür varsa bu türde filmleri getir
                .Select(s => s.ID).ToArray();

            //rastgele bir dizi elemanı seçilerek filmin id si alınır
            int rastgeleFilmID = tumIDler[random.Next(tumIDler.Length)];

            //View e modeli dönüyoruz
            return RedirectToAction("Search", new { filmId = rastgeleFilmID });
        }

        public ActionResult FilmOner()
        {
            Random random = new Random();

            //son verilen 7 ve 7'den büyük puana sahip yoruma ait filmin türünü getirir
            int? favoriTürü = db.Comment.Where(w => w.Score >= 7 && w.UserID == SuankiKullanicininIDsi).OrderByDescending(o => o.ID).Select(s => s.FilmSeries.TypeID).Take(1).SingleOrDefault();

            //bu türdeki tüm filmler diziye atılır
            int[] tumIDler = db.FilmSeries
                //eğer favori tür varsa bu türde filmleri getir
                .WhereIf(favoriTürü > 0, w => w.TypeID == favoriTürü)
                .Select(s => s.ID).ToArray();

            //rastgele bir dizi elemanı seçilerek filmin id si alınır
            int rastgeleFilmID = tumIDler[random.Next(tumIDler.Length)];

            //View e modeli dönüyoruz
            return RedirectToAction("Search", new { filmId = rastgeleFilmID });
        }

    }
}