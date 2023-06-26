using MazeMvcApp.Models;
using MazeMvcApp.Models.MazeGenerationAlgos;
using MazeMvcApp.Models.MazeSolverAlgos;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MazeMvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // The Maze field has to be static or it resets - Investigate later
        private static Maze _maze = new Maze();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {        
            return View(_maze);
        }

        public IActionResult GenerateMaze(int nRow, int nCol)
        {
            if( (nRow  < 3) || (nRow > 60) || (nCol < 3) || (nCol > 60) )
            {
                return RedirectToAction(nameof(Index));
            }

            _maze = new Maze(nRow, nCol);
            
            // Create perfect maze
            IMazeGenerator mazeGenerator = new HuntAndKill(_maze);
            mazeGenerator.GenerateMaze();
            _maze.MapEdgeDisplays();

            IMazeSolver mazeSolver = new DepthFirstSearch(_maze);
            List<MazeCell> validPath = mazeSolver.FindValidPath();
            _maze.ValidPath = validPath;
            _maze.IsSolved = true;

            _maze.MapDisplayDelay();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult ClearMaze()
        {
            _maze = new Maze();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ChooseAlgorithm(string selectedAlgorithm)
        {
            Console.WriteLine(selectedAlgorithm);

            return RedirectToAction(nameof(Index));
        }

        /*
        // Method for debugging purposes
        public IActionResult DummyMethod()
        {
            Maze shitMaze = _maze;
            Console.WriteLine("ye");
            return View(_maze);
        }*/

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}