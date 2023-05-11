using Microsoft.AspNetCore.Mvc;

namespace MazeMvcApp.Controllers
{
    public class MazeController : Controller
    {
        /*
        public IActionResult Index()
        {
            return View();
        }
        */

        // GET: /Maze/GenerateMaze
        public ActionResult GenerateMaze()
        {
            return View();
        }

        // POST: /Maze/GenerateMaze
        [HttpPost]
        public ActionResult GenerateMaze(int nRow, int nCol)
        {
            // Generate a new maze with the specified dimensions
            Maze maze = new Maze(nRow, nCol);

            // Pass Maze to View with ViewBag?
        }

        // GET: /Maze/SolveMaze
        public ActionResult SolveMaze()
        {
            // Retrieve Maze (maybe pass as parameter from View)

            return View(maze);
        }

        // POST: /Maze/SolveMaze
        [HttpPost]
        public ActionResult SolveMaze(string solution)
        {
            // Retrieve the maze from the session
            Maze maze = (Maze)Session["maze"];

            // Validate the solution
            bool isCorrect = maze.Solve(solution);

            return Json(new { success = true, isCorrect = isCorrect });
        }

    }
}
