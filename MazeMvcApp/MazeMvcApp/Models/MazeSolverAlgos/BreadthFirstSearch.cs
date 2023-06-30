﻿namespace MazeMvcApp.Models.MazeSolverAlgos
{
    public class BreadthFirstSearch : IMazeSolver
    {
        private readonly Maze _maze;
        public Dictionary<MazeCell, double> AlgorithmDisplayMap { get; set; } = new Dictionary<MazeCell, double>();

        public BreadthFirstSearch(Maze maze)
        {
            _maze = maze;
        }

        // Return ordered list of cells that represent a valid path through the Maze
        public List<MazeCell> FindValidPath()
        {
            var startingCell = _maze.StartCell;
            Queue<MazeCell> path = new();
            path.Enqueue(startingCell);
            startingCell.Traversed = true;
            var currentCell = startingCell;
            double delay = 0.1d;

            while (currentCell != _maze.EndCell)
            {
                if (!AlgorithmDisplayMap.ContainsKey(currentCell))
                {
                    AlgorithmDisplayMap.Add(currentCell, delay);
                    delay += 0.02;
                }
                else
                {
                    delay += 0.01;
                }

                // So that I don't get stuck, I need to Enqueue all neighbours of current node before further processing
                // See Obsidian notes for why this algo was getting stuck sometimes compared to DFS
                foreach (MazeCell neighbourCell in currentCell.Neighbours)
                {
                    if ((!neighbourCell.Traversed) && (currentCell.IsConnectedTo(neighbourCell)))
                    {
                        path.Enqueue(neighbourCell);
                    }
                }

                if (IsMovePossible(currentCell, out MazeCell? nextCell))
                {
                    currentCell = nextCell;
                    currentCell.Traversed = true;
                    path.Enqueue(currentCell);
                }
                else
                {
                    path.Dequeue();
                    currentCell = path.Peek();
                }
            }

            List<MazeCell> result = new List<MazeCell>(path);
            _maze.UntraverseAllCells();
            _maze.AlgorithmDisplayMap = AlgorithmDisplayMap;
            _maze.PopulateFinalDisplayTimer();

            return result;
        }

        public Dictionary<MazeCell, double> GetAlgorithmSearchDisplayMap()
        {
            if (AlgorithmDisplayMap.Count > 1) // check if dictionary already populated
            {
                return AlgorithmDisplayMap;
            }
            else
            {
                FindValidPath();
                return AlgorithmDisplayMap;
            }
        }

        private bool IsMovePossible(MazeCell currentCell, out MazeCell? nextCell)
        {
            // Check if we can move to neighbour cells & if they are untraversed
            foreach (MazeCell neighbourCell in currentCell.Neighbours)
            {
                if ((!neighbourCell.Traversed) && (currentCell.IsConnectedTo(neighbourCell)))
                {
                    nextCell = neighbourCell;
                    return true;
                }
            }
            nextCell = null;
            return false;
        }

    }
}
