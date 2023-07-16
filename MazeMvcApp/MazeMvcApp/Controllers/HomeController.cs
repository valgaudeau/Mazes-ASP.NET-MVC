using MazeMvcApp.Models;
using MazeMvcApp.Models.MazeGenerationAlgos;
using MazeMvcApp.Models.MazeSolverAlgos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Net;
using System.Reflection;

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
            if( (nRow  < 3) || (nRow > 80) || (nCol < 3) || (nCol > 80) )
            {
                return RedirectToAction(nameof(Index));
            }

            _maze = new Maze(nRow, nCol);
            
            // Create perfect maze
            IMazeGenerator mazeGenerator = new HuntAndKill(_maze);
            mazeGenerator.GenerateMaze();
            // _maze.MapEdgeDisplays();

            IMazeSolver mazeSolver = new DepthFirstSearch(_maze);
            List<MazeCell> validPath = mazeSolver.FindValidPath();
            _maze.ValidPath = validPath;
            _maze.IsSolved = true;
            _maze.SetMazeSolver(mazeSolver);
            _maze.MapDisplayDelay();

            return RedirectToAction(nameof(Index));
        }

        /*
        public IActionResult EraseMaze()
        {
            _maze = new Maze();
            return RedirectToAction(nameof(Index));
        }*/

        public IActionResult ChooseAlgorithm(string selectedAlgorithm)
        {
            // I would like to store the scroll position so page reloads on same position even if it hits controller
            // TempData["ScrollPosition"] = Request["scrollPosition"];
            IMazeSolver mazeSolver;

            mazeSolver = selectedAlgorithm == "DFS" ? new DepthFirstSearch(_maze)
                       : selectedAlgorithm == "BFS" ? new BreadthFirstSearch(_maze)
                       : selectedAlgorithm == "bidir-DFS" ? new BiDirectionalDFS(_maze)
                       : selectedAlgorithm == "bidir-BFS" ? new BiDirectionalBFS(_maze)
                       : selectedAlgorithm == "dijkstras" ? new Dijkstra(_maze)
                       : new DepthFirstSearch(_maze);

            _maze.ValidPath = mazeSolver.FindValidPath();
            _maze.SetMazeSolver(mazeSolver);
            // MapDisplayDelay method if you want to test the valid path of your IMazeSolver
            // Without calling that method, the valid path found by the IMazeSolver isn't
            // mapped to the visual display - Can comment it out for now all algos working
            // It would only be relevant if there were multiple possible paths, in which case
            // the different pathfinding algorithms may find different valid paths 
            // _maze.MapDisplayDelay();
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}