using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Surveys.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Surveys.Controllers
{
    /// <summary>
    /// Klasa zawierająca logikę związaną ze stroną główną
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Strona startowa
        /// </summary>
        /// <returns>Widok strony głównej</returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Strona z polityką prywatności
        /// </summary>
        /// <returns>Widok strony polityki prywatności</returns>
        public IActionResult Privacy()
        {
            return View();
        }
        /// <summary>
        /// Generyczna strona błędu
        /// </summary>
        /// <returns>Widok strony z błędem</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
