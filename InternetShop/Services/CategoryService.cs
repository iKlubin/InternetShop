using Google.Cloud.Firestore;
using InternetShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternetShop.Services
{
    public class CategoryService
    {
        private readonly FirestoreDb _firestoreDb;

        public CategoryService()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "credentials.json");
            _firestoreDb = FirestoreDb.Create("internetshop-d9b58");
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            Query query = _firestoreDb.Collection("categories");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            List<Category> categories = snapshot.Documents.Select(d => d.ConvertTo<Category>()).ToList();
            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection("categories").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<Category>();
            }
            else
            {
                return null;
            }
        }

        public async Task AddCategoryAsync(Category category)
        {
            CollectionReference collection = _firestoreDb.Collection("categories");
            await collection.AddAsync(category);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var categoryRef = _firestoreDb.Collection("categories").Document(category.Id);
            await categoryRef.SetAsync(category, SetOptions.Overwrite);
        }

        public async Task DeleteCategoryAsync(string categoryId)
        {
            var categoryRef = _firestoreDb.Collection("categories").Document(categoryId);
            await categoryRef.DeleteAsync();
        }
    }
}
