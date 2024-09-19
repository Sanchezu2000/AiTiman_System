using AiTiman_System.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using AiTiman_System.Entities;

namespace AiTiman_System.Services
{
    public class AppointmentService
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public AppointmentService(IMongoDatabase database)
        {
            _appointments = database.GetCollection<Appointment>("Appointments");
        }

        public async Task CreateAppointmentAsync(Appointment appointment)
        {
            await _appointments.InsertOneAsync(appointment);
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _appointments.Find(_ => true).ToListAsync();
        }

        public async Task<Appointment> GetAppointmentByIdAsync(string id)
        {
            return await _appointments.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAppointmentAsync(string id, Appointment updatedAppointment)
        {
            await _appointments.ReplaceOneAsync(a => a.Id == id, updatedAppointment);
        }

        public async Task DeleteAppointmentAsync(string id)
        {
            await _appointments.DeleteOneAsync(a => a.Id == id);
        }
    }
}
