using Google.Cloud.Firestore;
using System;
using System.ComponentModel.DataAnnotations;

namespace InternetShop.Models
{
    [FirestoreData]
    public class Review
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        [Required]
        public string ProductId { get; set; }

        [FirestoreProperty]
        [Required]
        public string UserEmail { get; set; }

        [FirestoreProperty]
        [Required]
        [Range(0, 10)]
        public int Rating { get; set; }

        [FirestoreProperty]
        [StringLength(1000)]
        public string Comment { get; set; }

        [FirestoreProperty]
        public DateTime Date { get; set; }
    }
}
