using System.Diagnostics;
using CodeSparkNET.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeSparkNET.WEB.Controllers
{
    public class DocumentsController : Controller
    {
        // private readonly ILogger<DocumentsController> _logger;

        // public DocumentsController(ILogger<DocumentsController> logger)
        // {
        //     _logger = logger;
        // }

        public IActionResult Documentations()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
