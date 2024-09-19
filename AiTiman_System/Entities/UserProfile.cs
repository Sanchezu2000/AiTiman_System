using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AiTiman_System.Entities
{
    public class UserProfile
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string UserName { get; set; }
        public string FirstName { get; set; } = "First Name";
        public string LastName { get; set; } = "Last Name";

        public DateTime Birthdate { get; set; }

        public string CompleteAddress { get; set; } = "Complete Address";
        public string Religion { get; set; } = "Religion";
        public string PhoneNumber { get; set; } = "00000000000";
        public string Gender { get; set; } = "Not Specified";
        public string Status { get; set; } = "Single";

        // Age is computed from Birthdate, so it should not be stored
        [BsonIgnore]
        public string Age { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string UnverifiedRole { get; set; }

        [BsonIgnoreIfNull]
        public string GuardianName { get; set; } = "Guardian Name";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string ProfilePictureUrl { get; set; } = "/images/Timan/user.png";
        public string SignPictureUrl { get; set; } = "/images/Timan/dr_sign.png";

        // Age Group Classification is automatically computed based on age
        [BsonIgnore]
        public string AgeGroupClassification { get; set; }
    }
}
