using System.Diagnostics;
using CodeSparkNET.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeSparkNET.WEB.Controllers
{
    public class SupportController : Controller
    {
        private readonly ILogger<SupportController> _logger;

        public SupportController(ILogger<SupportController> logger)
        {
            _logger = logger;
        }

        public IActionResult Help()
        {
            return View();
        }

        // TODO: create metod
        // public IActionResult GetEmail() {

        // }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
