using Google.Cloud.Firestore;

namespace InternetShop.Models
{
    [FirestoreData]
    public class Category
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string ImageUrl { get; set; }
    }
}
