using System;

namespace AiTiman_System.Models
{
    public class ProfileModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; } = "First Name";
        public string LastName { get; set; } = "Last Name";

        public string CompleteAddress { get; set; } = "Complete Address";
        public string Religion { get; set; } = "Religion";
        public string PhoneNumber { get; set; } = "00000000000";
        public string Gender { get; set; } = "Not Specified";
        public string Status { get; set; } = "Single";

        // Age and AgeGroupClassification calculated from Birthdate
        public string Age => (DateTime.Now.Year - Birthdate.Year).ToString();
        public string AgeGroupClassification => (DateTime.Now.Year - Birthdate.Year) < 18 ? "Minor" : "Adult";

        public string Email { get; set; }
        public string Address { get; set; }
        public string UnverifiedRole { get; set; }

        public string GuardianName { get; set; } = "Guardian Name";
        public string ProfilePictureUrl { get; set; } = "/images/Timan/user.png";
        public string SignPictureUrl { get; set; } = "/images/Timan/dr_sign.png";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime Birthdate { get; set; }
    }
}
