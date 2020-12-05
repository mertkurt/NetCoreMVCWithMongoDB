using MongoDB.Bson;
using MongoDB.Driver;
using NetCoreMVCWithMongoDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMVCWithMongoDB
{
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
}
