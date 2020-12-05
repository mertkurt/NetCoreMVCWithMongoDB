Merhaba Arkadaşlar,

 

Bu yazımızda sizlere .Net Core MVC projesi ile MongoDB kullanarak veritabanı işlemleri (Kayıt Ekleme, Silme, Güncelleme ve Listeleme) yapmanın kolay ve hızlı bir yöntemini kodlayarak göstereceğim.

 

İlk olarak Visual Studio programını açarak .Net Core bölümünden boşbir proje başlatıyorum. Projeyi başlattıktan sonra ilk işlem olarak Solution Explorer üzerinden projemize sağ tıklayarak Manage Nuget Pakages üzerinden MongoDB.Driver yazarak arama yapıp aşağıda görseldeki gibi gelen kütüphaneyi install ederek projemize dahil ediyoruz.

 

Manage Nuget Packages - MongoDB.Driver 

 

Projemize eklediğimiz kütüphane sayesinde kullanacağımız MongoDB hakkında biraz bilgi vermek gerekirse;

MongoDB 2009 yılında geliştirilmiş açık kaynak kodlu bir NoSQL veritabanı sistemidir.MongoDB de eklenen herbir kayıt document olarak ifade edilir.Documentlar MongoDB de json formatında saklanır.Veri tabanlarında tablo olarak adlandırdığımız  yapılar ise MongoDB de collection, satır yapıları document ve sütun yapıları ise field olarak kullanılır.

 

 

Bilgilendirmeden sonra projemiz de ben bir kullanıcı kayıt sistemi kodlayacağım için ilk olarak bu sisteme uygun bir model oluşturuyorum ve modeli aşağıdaki gibi kodluyorum.

 

    public class Kullanici

    {

        public ObjectId Id { get; set; }

        [BsonElement("kullaniciAdi")]

        public string KullaniciAdi { get; set; }

        [BsonElement("Ad")]

        public string Ad { get; set; }

        [BsonElement("Soyad")]

        public string Soyad { get; set; }

        [BsonElement("Mail")]

        public string Mail { get; set; }

        [BsonElement("Sifre")]

        public string Sifre { get; set; }

 

    }

 

Modelimizi oluşturduktan sonra ise MongoDB üzerinde işlemlerimizi daha hızlı ve tek yerden yönetebilmek için bir class oluşturup veri üzerindeki bütün işlemlerimi burada gerçekleştiriyorum.Bu tür oluşturulan yapılara genel isim olarak Veri Erişim Katmanı (Data Access Layer - DAL) ismi veriliyor. Bende bu ismin kısa ismini oluşturduğun class'ın ismine yazıyorumki oluşturduğumuz yapıyı daha sonra baktığımızda rahatlıkla anlayabilelim.

KullaniciDAL adında oluşturduğum class'ı aşağıdaki gibi kodluyorum.

 

//Kullanici Data Access Layer - Kullanıcılar ile ilgili bütün MongoDB işlemleri burada yapılıyor.

    public class KullaniciDAL

    {

        private readonly IMongoCollection<Kullanici> mongoDb;

 

        public KullaniciDAL(string connectionString, string dbName, string tableName)

        {

            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(dbName);

            mongoDb = database.GetCollection<Kullanici>(tableName);

        }

 

        // Butun Kullanıcıları Listeleme

        public List<Kullanici> KullaniciListesi()

        {

            return mongoDb.Find(kul => true).ToList();

        }

 

        // Bir Kullanıcı Getirme

        public Kullanici KullaniciGetir(string id)

        {

            var objectID = new ObjectId(id);

            return mongoDb.Find<Kullanici>(m => m.Id == objectID).FirstOrDefault();

        }

 

        // Kullanıcı Ekleme

        public Kullanici KullaniciEkle(Kullanici model)

        {

            mongoDb.InsertOne(model);

            return model;

        }

 

        // Kullanıcı Guncelleme

        public void KullaniciGuncelle(string id, Kullanici model)

        {

            var objectID = new ObjectId(id);

            mongoDb.ReplaceOne(m => m.Id == objectID, model);

        }

 

        // Kullanıcı Silme

        public void KullaniciSil(string id)

        {

            var objectID = new ObjectId(id);

            mongoDb.DeleteOne(m => m.Id == objectID);

        }

 

    }

 

Yukarıdaki kodu incelediğinizde zaten oluşturulan metot isimleri ve yazılan açıklama satırları sayesinde işlemlerin hangi bölümlerde ve nasıl yapıldığı konusunda bilgileriniz oluşacaktır.

 

Buraya kadar bütün işlemlerimiz tamam ise şimdi gelelim artık MongoDB bağlantımızı gerçekleştirdiğimiz aşamaya. Bilgisayarınızda MongoDB ile ilgili kurulumlar yapılı değilse aşağıdaki adresten kurulum dosyasını indirip bilgisayarınıza kurulum işlemlerini gerçekleştirebilirsiniz.

 

ADRES: https://www.mongodb.com/

 

Biz kurulumun yapıldığını ve MongoDB ile iletişime geçmek için ayarlarımızı yaparak devam ediyoruz. Bu işlem için projenizde appsettings.json isimli bir dosyanın olması gerekiyor. Eğer böyle bir dosya yoksa projenize sağ tıklayıp New Item bölümünden bir json dosyası projenize ekleyip ismini appsettings.json olarak belirleyebilirsiniz.

appsettings.json dosyasının içeriğini ise aşağıdaki gidi kodluyoruz.

 

{

  "ApplicationInsights": {

    "InstrumentationKey": ""

  },

  "Logging": {

    "IncludeScopes": false,

    "LogLevel": {

      "Default": "Debug",

      "System": "Information",

      "Microsoft": "Information"

    }

  },

  "AllowedHosts": "*",

  "ConnectionStrings": {

    "MongoConnectionString": "mongodb://localhost:10010"

  }

}

 

 

Dosyamız içerisine yazdığımız "MongoConnectionString": "mongodb://localhost:10010" satırındaki bilgiyi bilgisayarınızda kurulu olan MongoDB adresi ile değiştirmeniz gerekecektir. Bunu atlamazsanız MongoDB ile bağlantı kurulurken sorun yaşamazsınız.

 

Bağlantı ayarlarımızıda halletikten sonra artık MongoDB ile iletişime geçmek için projemizde bulunan ' Startup.cs ' dosyamız içerisindeki ' ConfigureServices ' metodumuz içerisine gerekli olan bağlantı ayarlarımızı aşağıdaki gibi yazıyoruz.

 

string mongoConnectionString = this.Configuration.GetConnectionString("MongoConnectionString");

 

services.AddTransient(at => new KullaniciDAL(mongoConnectionString, "MongoDBKullanici", "Kullanicilar"));

 

Artık bağlantı ayarlarımızıda bitirdiğimize göre gelelim Controlller ımızı oluşturmaya. Projemize ben HomeController isimli bir Controller ekliyorum ve içerisini aşağıdaki gibi kodluyorum.

   

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

 

Controllerımızı bu şekilde oluşturduktan sonra Controller içerisinde yazdığımız ilk View'ımız olan Index View'ımızı aşağıdaki gibi kodluyorum. 

 

@{

    ViewData["Title"] = "Home Page";

}

 

@model List<Models.Kullanici>

 

<div class="row">

    <div class="col-md-12">

        <h2>Kullanıcı Listesi</h2>

        <table>

            <thead>

                <tr>

                    <th>ID</th>

                    <th>Ad Soyad</th>

                    <th>Kullanıcı Adı</th>

                    <th>Mail</th>

                    <th>İşlemler</th>

                </tr>

            </thead>

            <tbody>

               

                @foreach (Models.Kullanici item in Model)

                {

                    <tr>

                        <td>@item.Id</td>

                        <td>@item.Ad @item.Soyad</td>

                        <td>@item.KullaniciAdi</td>

                        <td>@item.Mail</td>

                        <td>

                            <a href="~/Home/KullaniciGuncelle/@item.Id">Güncelle</a>

                            &nbsp;

                            <a href="~/Home/KullaniciSil/@item.Id">Sil</a>

                        </td>

                    </tr>

                }

 

            </tbody>

        </table>

    </div>

</div>

 

Yukarıdaki kodumuzda HomeController içerisinden List şeklinde gönderdiğimiz modeli sayfamıza alıp basit bir tablo halinde Viewda gösterimini sağlıyoruz.

 

Index viewından sonra HomeController içerisinde yazdığımız KullaniciEkle isimli viewımızı oluşturup içerisini aşağıdaki gibi kodluyorum.

 

@{

    ViewData["Title"] = "Kullanıcı Ekle";

}

@model Models.Kullanici

<div class="row">

    <div class="col-md-12">

        <h2>Kullanıcı Ekle</h2>

        <form asp-controller="Home" asp-action="KullaniciEkle" method="post" enctype="multipart/form-data">

            <input type="text" asp-for="KullaniciAdi" placeholder="Kullanıcı Adı" />

            <br />

            <input type="text" asp-for="Ad" placeholder="Ad" />

            <br />

            <input type="text" asp-for="Soyad" placeholder="Soyad" />

            <br />

            <input type="text" asp-for="Mail" placeholder="Mail" />

            <br />

            <input type="text" asp-for="Sifre" placeholder="Sifre" />

            <br />

            <button type="submit">EKLE</button>

        </form>

    </div>

</div>

 

 

 

Kullanıcı güncellemek için KullaniciGuncelle isimli bir view oluşturup aşağıdaki gibi kodluyorum.

 

@{

    ViewData["Title"] = "Kullanıcı Güncelle";

}

@model Models.Kullanici

<div class="row">

    <div class="col-md-12">

        <h2>Kullanıcı Ekle</h2>

        <form asp-controller="Home" asp-action="KullaniciGuncelle" method="post" enctype="multipart/form-data">

            <input type="text" asp-for="KullaniciAdi" value="@Model.KullaniciAdi" placeholder="Kullanıcı Adı" />

            <br />

            <input type="text" asp-for="Ad" value="@Model.Ad" placeholder="Ad" />

            <br />

            <input type="text" asp-for="Soyad" value="@Model.Soyad" placeholder="Soyad" />

            <br />

            <input type="text" asp-for="Mail" value="@Model.Mail" placeholder="Mail" />

            <br />

            <input type="text" asp-for="Sifre" value="@Model.Sifre" placeholder="Sifre" />

            <br />

            <button type="submit">GÜNCELLE</button>

        </form>

    </div>

 

</div>

 

 

Index View'ında Silme işlemi olarak link verdiğim ve HomeController içerisinde de KullaniciSil olarak kodladığım metot için ise bir View oluşturmuyorum. Bu metot için yazdığımız kodlara bakarsanız işlemimizi yaptıktan sonra View olarak Index yani listeleme işlemini yaptığımız View'a yönlendiğini göreceksiniz. O yüzden silme işlemi için bu metot tetiklendiğinde işlemimizi gerçekleştirip direk Listeleme işlemini yaptığımız Index View'ımız açılacaktır.

 

Kodlamalarımızı bu şekilde bitirdikten sonra artık projemizi derleyip çalıştırabiliriz. İlk baştada bahsettiğimiz gibi projemizi çalıştırdığımızda bütün veri tabanı işlemlerini gerçekleştirebilirsiniz. Bir kullanıcı kaydı ekleyip sonra o kaydı güncelleyip gerekli değil ise silebilirsiniz. İşlemleri deneme sırası sizin insiyatifinizde. 

 

 
