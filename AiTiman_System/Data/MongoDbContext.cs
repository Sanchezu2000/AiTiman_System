using AiTiman_System.Entities;
using AiTiman_System.Models;
using AiTIman_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace AiTIman_System.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        // Constructor to initialize the MongoDB client and get the required database
        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        // Method to expose the IMongoDatabase instance
        public IMongoDatabase GetDatabase()
        {
            return _database;
        }

        // Property to get the "Users" collection from the database
        public IMongoCollection<AiTimanUser> Users => _database.GetCollection<AiTimanUser>("Users");

        // Property to get the "Roles" collection from the database
        public IMongoCollection<IdentityRole> Roles => _database.GetCollection<IdentityRole>("Roles");

        // Property to get the "UserProfiles" collection from the database
        public IMongoCollection<UserProfile> UserProfiles => _database.GetCollection<UserProfile>("UserProfiles");

        //Property to get the "Appointments" collection from the database
        public IMongoCollection<Appointment> GetAppointments() => _database.GetCollection<Appointment>("Appointment");
    }
}