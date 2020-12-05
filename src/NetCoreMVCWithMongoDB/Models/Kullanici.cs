using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMVCWithMongoDB.Models
{
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
}
