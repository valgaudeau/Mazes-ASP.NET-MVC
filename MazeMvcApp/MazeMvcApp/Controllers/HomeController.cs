using MazeMvcApp.Models;
using MazeMvcApp.Models.MazeGenerationAlgos;
using MazeMvcApp.Models.MazeSolverAlgos;
using Microsoft.AspNetCore.Mvc;
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
            if( (nRow  < 3) || (nRow > 100) || (nCol < 3) || (nCol > 100) )
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
            // I would like to store the scroll position so page reloads on same position even if it hits controller
            // TempData["ScrollPosition"] = Request["scrollPosition"];
            IMazeSolver mazeSolver;

            mazeSolver = selectedAlgorithm == "DFS" ? new DepthFirstSearch(_maze)
                       : selectedAlgorithm == "BFS" ? new BreadthFirstSearch(_maze)
                       : selectedAlgorithm == "bidir-DFS" ? new BiDirectionalDFS(_maze)
                       : selectedAlgorithm == "bidir-BFS" ? new BiDirectionalBFS(_maze)
                       : new DepthFirstSearch(_maze); // default

            // I wasn't using MapDisplayDelay(), which is why ValidPath display was still fine even though
            // BFS not implemented properly - The algorithm display is fine, but not the valid path
            // Issue: One is that I'm adding the same cell multiple time into the queue just to start with
            // That's why algorithm display was slow, trying to add same cell multiple times in display
            // Its somewhat fixed, problem now is that my ValidPath includes all the nodes we've traversed!
            // Need to change the logic so that all the "wrong" nodes are dequeue'd or something - That's the next step
            // Check https://www.youtube.com/watch?v=TIbUeeksXcI&t=650s - NEED TO REVIEW WHOLE BFS LOGIC ITS JUST WEIRD ATMO
            // Also add timer for algorithm display speed, should be very easy just need a variable in controller which
            // we pass to IMazeSolver somewhere. Could also change from double to decimal for more precise but meh
            // COME BACK TO THIS LATER, BRAIN IS TOO FRIED - WORK ON OTHER ISSUES
            _maze.ValidPath = mazeSolver.FindValidPath();
            _maze.MapDisplayDelay();
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}