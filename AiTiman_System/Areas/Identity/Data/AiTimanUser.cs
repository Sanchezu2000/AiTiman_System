using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AiTIman_System.Areas.Identity.Data
{
    public class AiTimanUser : IdentityUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnore]
        public override string Id { get; set; }

        [BsonElement("birthdate")]
        public DateTime Birthdate { get; set; }

        [BsonElement("address")]
        public string Address { get; set; }

        [BsonElement("roles")]
        public List<string> Roles { get; set; } = new List<string>();

        [BsonElement("licenseNumber")]
        [BsonIgnoreIfNull] // Ignore field if null
        public string? LicenseNumber { get; set; }

        [BsonElement("employeeNumber")]
        [BsonIgnoreIfNull] // Ignore field if null
        public string? EmployeeNumber { get; set; }

        [BsonElement("age")]
        public int Age { get; set; }

        public string VerificationCode { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public string UnverifiedRole { get; set; }
    }

}
