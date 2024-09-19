using MongoDB.Driver;
using AiTiman_System.Entities;
using System.Threading.Tasks;
using System;
using MongoDB.Bson;

namespace AiTiman_System.Services
{
    public class UserProfileService

    {

        private readonly IMongoCollection<UserProfile> _userProfilesCollection;

        public UserProfileService(IMongoDatabase database)
        {
            _userProfilesCollection = database.GetCollection<UserProfile>("UserProfiles");

        }

        public async Task<bool> CreateUserProfileAsync(UserProfile profile)
        {
            try
            {
                await _userProfilesCollection.InsertOneAsync(profile);
                return true;
            }
            catch (MongoException mongoEx)
            {
                Console.WriteLine($"MongoDB error: {mongoEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user profile: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateUserProfile(UserProfile profile)
        {
            try
            {
                var result = await _userProfilesCollection.ReplaceOneAsync(
                    filter: u => u.Id == profile.Id,
                    replacement: profile,
                    options: new ReplaceOptions { IsUpsert = true });

                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch (MongoException mongoEx)
            {
                Console.WriteLine($"MongoDB error: {mongoEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user profile: {ex.Message}");
                return false;
            }
        }

        //public async Task<UserProfile> GetUserProfileById(string userName)
        //{
        //    try
        //    {
        //        // Convert the string userId to an ObjectId
        //        if (!ObjectId.TryParse(userName, out var objectId))
        //        {
        //            throw new ArgumentException("Invalid user ID format.");
        //        }

        //        // Find the user by ObjectId
        //        //return await _userProfilesCollection.Find(u => u.Id == objectId).FirstOrDefaultAsync();
        //        return await _userProfilesCollection.Find(x => x.UserName == userName).FirstOrDefaultAsync();

        //    }
        //    catch (MongoException mongoEx)
        //    {
        //        Console.WriteLine($"MongoDB error: {mongoEx.Message}");
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error retrieving user profile: {ex.Message}");
        //        return null;
        //    }
        //}

        public async Task<UserProfile> GetUserProfileByUserName(string userName)
        {
            var filter = Builders<UserProfile>.Filter.Eq(u => u.UserName, userName);
            return await _userProfilesCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<UserProfile> GetUserProfile(string userName)
        {
            try
            {
                return await _userProfilesCollection.Find(u => u.UserName == userName).FirstOrDefaultAsync();
            }
            catch (MongoException mongoEx)
            {
                Console.WriteLine($"MongoDB error: {mongoEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user profile: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteUserProfileAsync(string profileId)
        {
            try
            {
                if (!ObjectId.TryParse(profileId, out var objectId))
                    throw new ArgumentException("Invalid profile ID format.");

                var result = await _userProfilesCollection.DeleteOneAsync(u => u.Id == objectId);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch (MongoException mongoEx)
            {
                Console.WriteLine($"MongoDB error: {mongoEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user profile: {ex.Message}");
                return false;
            }
        }
    }
}
