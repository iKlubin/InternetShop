using Google.Cloud.Firestore;
using InternetShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternetShop.Services
{
    public class ReviewService
    {
        private readonly FirestoreDb _firestoreDb;

        public ReviewService()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "credentials.json");
            _firestoreDb = FirestoreDb.Create("internetshop-d9b58");
        }

        public async Task AddReviewAsync(Review review)
        {
            var collection = _firestoreDb.Collection("reviews");
            await collection.AddAsync(review);
        }

        public async Task<List<Review>> GetReviewsByProductIdAsync(string productId)
        {
            var reviews = new List<Review>();
            var query = _firestoreDb.Collection("reviews").WhereEqualTo("ProductId", productId);
            var snapshot = await query.GetSnapshotAsync();

            if (snapshot != null)
            {
                foreach (var document in snapshot.Documents)
                {
                    var review = document.ConvertTo<Review>();
                    reviews.Add(review);
                }
            }

            return reviews;
        }

        public async Task<Review> GetReviewByIdAsync(string reviewId)
        {
            DocumentReference docRef = _firestoreDb.Collection("reviews").Document(reviewId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<Review>();
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteReviewAsync(string reviewId)
        {
            DocumentReference docRef = _firestoreDb.Collection("reviews").Document(reviewId);
            var snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                var review = snapshot.ConvertTo<Review>();
                await docRef.DeleteAsync();
                // Update the product's average rating and reviews after deletion
                await UpdateProductRatingAsync(review.ProductId);
            }
        }

        private async Task UpdateProductRatingAsync(string productId)
        {
            // Get all reviews for the product
            var reviews = await GetReviewsByProductIdAsync(productId);

            // Calculate the average rating
            double averageRating = 0;
            if (reviews.Any())
            {
                averageRating = reviews.Average(r => r.Rating);
            }

            // Get the product and update its rating
            DocumentReference productRef = _firestoreDb.Collection("products").Document(productId);
            await productRef.UpdateAsync(new Dictionary<string, object>
            {
                { "AverageRating", averageRating },
                { "Reviews", reviews }
            });
        }
    }
}
