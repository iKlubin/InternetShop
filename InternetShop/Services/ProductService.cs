using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using InternetShop.Models;
using Google.Apis.Auth.OAuth2;

namespace InternetShop.Services
{
    public class ProductService
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public ProductService()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "credentials.json");

            _bucketName = "internetshop-d9b58.appspot.com";
            _firestoreDb = FirestoreDb.Create("internetshop-d9b58");
            _storageClient = StorageClient.Create(GoogleCredential.FromFile("credentials.json"));
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            Query query = _firestoreDb.Collection("products");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            List<Product> products = snapshot.Documents.Select(d => d.ConvertTo<Product>()).ToList();
            return products;
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string imageName)
        {
            var imageObject = await _storageClient.UploadObjectAsync(
                bucket: _bucketName,
                objectName: imageName,
                contentType: null,
                source: imageStream,
                options: new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead }
            );
            return $"https://storage.googleapis.com/{_bucketName}/{imageName}";
        }

        public async Task AddProductAsync(Product product)
        {
            CollectionReference collection = _firestoreDb.Collection("products");
            await collection.AddAsync(product);
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            CollectionReference collection = _firestoreDb.Collection("products");
            QuerySnapshot snapshot = await collection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Product>()).ToList();
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection("products").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<Product>();
            }
            else
            {
                return null;
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            var productRef = _firestoreDb.Collection("products").Document(product.Id);
            await productRef.SetAsync(product, SetOptions.Overwrite);
        }

        public async Task DeleteProductAsync(string productId)
        {
            var productRef = _firestoreDb.Collection("products").Document(productId);
            await productRef.DeleteAsync();
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            var categoryNames = new List<string>();
            var categoriesCollection = _firestoreDb.Collection("categories");
            var snapshot = await categoriesCollection.GetSnapshotAsync();

            foreach (var document in snapshot.Documents)
            {
                var category = document.ConvertTo<Category>();
                if (!string.IsNullOrEmpty(category.Name))
                {
                    categoryNames.Add(category.Name);
                }
            }

            return categoryNames;
        }
    }
}
