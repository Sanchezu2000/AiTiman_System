using AiTiman_System.Entities;
using AiTiman_System.Models;
using AiTiman_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiTiman_System.Controllers
{
    public class HealthProviderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult HealthProviderDashboard()
        {
            return View();
        }
        public IActionResult Appointments()
        {
            return View();
        }
        //        public async Task<IActionResult> Create(AppointmentModel model)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                var success = await _appointmentService.CreateAppointmentAsync(model);
        //                if (success)
        //                {
        //                    return RedirectToAction("Appointment"); // Redirect to a suitable page
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Failed to create appointment.");
        //                }
        //            }
        //            return View(model);
        //        }

        //        [HttpGet]
        //        public async Task<IActionResult> Edit(string id)
        //        {
        //            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
        //            if (appointment == null)
        //            {
        //                return NotFound();
        //            }
        //            return View(appointment);
        //        }

        //        [HttpPost]
        //        public async Task<IActionResult> Edit(string id, AppointmentModel model)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                var success = await _appointmentService.UpdateAppointmentAsync(id, model);
        //                if (success)
        //                {
        //                    return RedirectToAction("Appointment");
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Failed to update appointment.");
        //                }
        //            }
        //            return View(model);
        //        }

        //        [HttpPost]
        //        public async Task<IActionResult> Delete(string id)
        //        {
        //            var success = await _appointmentService.DeleteAppointmentAsync(id);
        //            if (success)
        //            {
        //                return RedirectToAction("Appointment");
        //            }
        //            else
        //            {
        //                return NotFound();
        //            }
        //        }

        //        public async Task<IActionResult> Index()
        //        {
        //            var appointments = await _appointmentService.GetAllAppointmentsAsync();
        //            return View(appointments);
        //        }
        //    }
        //}
    }
}