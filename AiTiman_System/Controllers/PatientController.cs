using Microsoft.AspNetCore.Mvc;

namespace AiTiman_System.Controllers
{
    public class PatientController : Controller
    {
        public IActionResult PatientDashboard()
        {
            return View();
        }
         public IActionResult BookAppointment()
        {
            { return View(); }
        }
        
        public IActionResult Community()
        {
            return View();
        }
        public IActionResult Feedback()
        {
            return View();
        }
       
        public IActionResult MedicalRecord()
        {
            return View();
        }
            
        public  IActionResult Reports()
        {
            return View();
        }

        public IActionResult Medicines()
        {
            return View();

        }
        public IActionResult DataAnalytics()
        {
            return View();
        }
    }
}
