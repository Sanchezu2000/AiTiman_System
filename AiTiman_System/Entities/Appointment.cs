using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AiTiman_System.Entities
{
    public class Appointments
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Appointment Name")]
        public string AppointmentName { get; set; }

        [BsonElement("Schedule Date")]
        public DateTime ScheduleDate { get; set; }

        [BsonElement("Schedule Time")]
        public TimeSpan ScheduleTime { get; set; }

        [BsonElement("Number of Slots")]
        public int NumberOfSlots { get; set; }

        [BsonElement("Appointment Status")]
        public string AppointmentStatus { get; set; }

        [BsonElement("Doctor In Charge")]
        public string DoctorInCharge { get; set; }

        [BsonElement("Appointment Setter")]
        public string AppointmentSetter { get; set; }

        [BsonElement("Date Created")]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        [BsonElement("Date Updated")]
        public DateTime DateUpdated { get; set; }
    }
}
