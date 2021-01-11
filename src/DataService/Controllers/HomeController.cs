using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;



namespace DataService.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [Route("")]
        [HttpGet]
        public ActionResult Index()
        {
            Models.IndexModel model = new Models.IndexModel(_env);
            return View(model);
        }
    }
}