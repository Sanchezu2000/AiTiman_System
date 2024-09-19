using AiTiman_System.Entities;
using AiTiman_System.Models;
using AiTiman_System.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AiTiman_System.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                // Saving the appointment to the database
                await _appointmentService.CreateAppointmentAsync(appointment);
                TempData["Message"] = "Appointment saved successfully!";
                return RedirectToAction("BookAppointment");
            }

            // If the model state is not valid, re-render the form
            return View(appointment);
        }

        public IActionResult BookAppointment()
        {
            return View();
        }
    }
}
