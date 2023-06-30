﻿using MazeMvcApp.Models;
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
            // I would like to store the scroll position so page reloads on same position even if it hits controller
            // TempData["ScrollPosition"] = Request["scrollPosition"];

            if(selectedAlgorithm == "DFS")
            {
                // do nothing since this is what we've got implemented by default
                IMazeSolver mazeSolver = new DepthFirstSearch(_maze);
                mazeSolver.FindValidPath();
                return RedirectToAction(nameof(Index));
            }
            else if(selectedAlgorithm == "BFS")
            {
                IMazeSolver mazeSolver = new BreadthFirstSearch(_maze);
                // This shouldn't be in the loop, fix it later not sure how tho
                mazeSolver.FindValidPath();
                return RedirectToAction(nameof(Index));
            }
            else if(selectedAlgorithm == "bidir-DFS")
            {
                IMazeSolver mazeSolver = new BiDirectionalDFS(_maze);
                mazeSolver.FindValidPath();
                return RedirectToAction(nameof(Index));
            }
            else if (selectedAlgorithm == "bidir-BFS")
            {
                /*
                IMazeSolver mazeSolver = new BiDirectionalBFS(_maze);
                mazeSolver.FindValidPath();
                return RedirectToAction(nameof(Index));
                */
            }
            else if (selectedAlgorithm == "aStar")
            {
                // IMazeSolver mazeSolver = new AStarSearch(_maze);
            }

            // _maze.AlgorithmDisplayMap = mazeSolver.GetAlgorithmSearchDisplayMap();
            // _maze.UntraverseAllCells();

            // May want to change how I'm displaying valid path & algorithm at work, meaning not creating new class for
            // validPath and algorithmDisplay, but instead just in the cell

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}