namespace MazeMvcApp.Models.MazeSolverAlgos
{
    public class BreadthFirstSearch : IMazeSolver
    {
        private readonly Maze _maze;
        public List<MazeCell> ValidPath { get; set; } = new List<MazeCell>();
        public Queue<MazeCell> VisitedCells { get; set; } = new Queue<MazeCell>();
        public Dictionary<MazeCell, double> AlgorithmDisplayMap { get; set; } = new Dictionary<MazeCell, double>();
        // This dictionary keeps track of how we arrived at each cell when travelling through the maze
        // We use it to reconstitute the valid path at the end
        private Dictionary<MazeCell, MazeCell> _cellCameFrom = new Dictionary<MazeCell, MazeCell>();

        public BreadthFirstSearch(Maze maze)
        {
            _maze = maze;
        }

        public List<MazeCell> FindValidPath()
        {
            var startingCell = _maze.StartCell;

            _cellCameFrom[startingCell] = null;
            Queue<MazeCell> queue = new Queue<MazeCell>();
            // Note that VisitedCells is only for the purpose of the Algorithm Display Mapping
            queue.Enqueue(startingCell);
            VisitedCells.Enqueue(startingCell);
            var currentCell = startingCell;

            while (currentCell != _maze.EndCell)
            {
                // So that I don't get stuck, I need to Enqueue all neighbours of current node before further processing
                // See Obsidian notes for why this algo was getting stuck sometimes compared to DFS
                foreach (MazeCell neighbourCell in currentCell.Neighbours)
                {
                    if ( (!_cellCameFrom.ContainsKey(neighbourCell))
                      && (currentCell.IsConnectedTo(neighbourCell)) )
                    {
                        _cellCameFrom[neighbourCell] = currentCell;
                        queue.Enqueue(neighbourCell);

                        if(!VisitedCells.Contains(neighbourCell))
                        {
                            VisitedCells.Enqueue(neighbourCell);
                        }
                    }
                }

                if (IsMovePossible(currentCell, out MazeCell? nextCell) && (!queue.Contains(nextCell)) )
                {
                    currentCell = nextCell;
                    queue.Enqueue(currentCell);

                    if (!VisitedCells.Contains(currentCell))
                    {
                        VisitedCells.Enqueue(currentCell);
                    }
                }
                else
                {
                    queue.Dequeue();
                    currentCell = queue.Peek();
                }
            }

            ReconstructPath();
            MapAlgorithmDisplay();
            _maze.PopulateFinalDisplayTimer();

            return ValidPath;
        }

        private bool IsMovePossible(MazeCell currentCell, out MazeCell? nextCell)
        {
            foreach (MazeCell neighbourCell in currentCell.Neighbours)
            {
                if (currentCell.IsConnectedTo(neighbourCell))
                {
                    nextCell = neighbourCell;
                    return true;
                }
            }
            nextCell = null;
            return false;
        }

        private void ReconstructPath()
        {
            // Start from the final cell and work our way back up
            MazeCell current = _maze.EndCell;

            while (current != _maze.StartCell)
            {
                ValidPath.Add(current);
                current = _cellCameFrom[current];
            }

            // At the end, we add the sourceCell on index 0
            ValidPath.Insert(0, _maze.StartCell);
        }

        private void MapAlgorithmDisplay()
        {
            if (AlgorithmDisplayMap.Count == 0)
            {
                double delay = 0.1d;

                // Should be ordered the way I want since its a Queue
                foreach (MazeCell cell in VisitedCells)
                {
                    if (!AlgorithmDisplayMap.ContainsKey(cell))
                    {
                        AlgorithmDisplayMap.Add(cell, delay);
                        delay += 0.05;
                    }
                    else
                    {
                        delay += 0.02; // adding this extra delay makes branch transitions smoother
                    }
                }
                _maze.AlgorithmDisplayMap = AlgorithmDisplayMap;
            }
        }

    }
}
