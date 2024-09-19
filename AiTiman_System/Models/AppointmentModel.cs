using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AiTiman_System.Entities
{
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Appointment Type")]
        public string AppointmentType { get; set; }

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
    }
}
