using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Cloud.Firestore;

namespace InternetShop.Models
{
    [FirestoreData]
    public class Product
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }

        [FirestoreProperty]
        public string ExtendedDescription { get; set; }

        [FirestoreProperty]
        public int Price { get; set; }

        [FirestoreProperty]
        public string ImageUrl { get; set; }

        [FirestoreProperty]
        public string Category { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Tags { get; set; }

        [FirestoreProperty]
        public int PurchaseCount { get; set; }

        [FirestoreProperty]
        public int ViewCount { get; set; }
    }
}
