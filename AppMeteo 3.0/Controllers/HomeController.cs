using AppMeteo_3._0.Models;
using AppMeteo_3._0.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AppMeteo_3._0.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DbServices _services;

        private Utente utente;


        public HomeController(ILogger<HomeController> logger, DbServices services)
        {
            _logger = logger;
            _services = services;
        }


        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Meteo()
        {
            return View();
        }

        public IActionResult Accesso()
        {

            return View();

        }

        public IActionResult Registrazione()
        {
            return View();
        }

        public IActionResult CambioPassword()
        {
            return View();
        }

        public IActionResult Amministratore()
        {
            List<Utente> listaUtenti = _services.OttieniListaUtenti();
            List<InfoMeteo> listaInfo = _services.OttieniListaInformazioni();

            var viewModel = new AmministratoreViewModel
            {
                ListaUtenti = listaUtenti,
                ListaInfo = listaInfo
            };

            return View(viewModel);

        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
