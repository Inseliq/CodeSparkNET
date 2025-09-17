using Microsoft.AspNetCore.Mvc;

namespace CodeSparkNET.Controllers
{
  public class CatalogController : Controller
  {

    public IActionResult Catalog()
    {
      return View();
    }
  }
}