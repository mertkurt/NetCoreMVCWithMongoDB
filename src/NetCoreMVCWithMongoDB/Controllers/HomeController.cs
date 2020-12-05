using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCoreMVCWithMongoDB.Models;

namespace NetCoreMVCWithMongoDB.Controllers
{
    public class HomeController : Controller
    {

        KullaniciDAL KullaniciDB;

        public HomeController(KullaniciDAL kulDAL)
        {
            KullaniciDB = kulDAL;
        }
        public IActionResult Index()
        {
            var kullanicilar = KullaniciDB.KullaniciListesi();
            return View(kullanicilar);
        }

        [HttpGet]
        public IActionResult KullaniciEkle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KullaniciEkle(Kullanici model)
        {
            KullaniciDB.KullaniciEkle(model);

            return View("Index");
        }


        [HttpGet]
        public IActionResult KullaniciGuncelle(string id)
        {
            Kullanici guncellenecekKayit = KullaniciDB.KullaniciGetir(id);

            return View(guncellenecekKayit);
        }

        [HttpPost]
        public IActionResult KullaniciGuncelle(string id, Kullanici model)
        {
            Kullanici guncellenecekKayit = KullaniciDB.KullaniciGetir(id);

            guncellenecekKayit.Ad = model.Ad;
            guncellenecekKayit.Soyad = model.Soyad;
            guncellenecekKayit.KullaniciAdi = model.KullaniciAdi;
            guncellenecekKayit.Mail = model.Mail;
            guncellenecekKayit.Sifre = model.Sifre;

            KullaniciDB.KullaniciGuncelle(id, model);

            return View("Index");
        }

        public IActionResult KullaniciSil(string id)
        {
            KullaniciDB.KullaniciSil(id);

            return View("Index");
        }
    }
}
