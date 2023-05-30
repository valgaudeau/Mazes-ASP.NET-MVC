using MazeMvcApp.Models;
using MazeMvcApp.Models.MazeGenerationAlgos;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MazeMvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private Maze _maze;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_maze);
        }

        public IActionResult GenerateMaze(Maze maze)
        {
            /*
            IMazeGenerator mazeGenerator = new HuntAndKill(maze);
            mazeGenerator.GenerateMaze();
            _maze = maze;
            */
            return RedirectToAction(nameof(Index));
            // Problem: If I return view, goese to different page
            // return View(maze);
        }

        public IActionResult Privacy()
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