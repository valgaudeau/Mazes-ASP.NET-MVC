namespace MazeMvcApp.Models.MazeSolverAlgos
{
    public class BreadthFirstSearch : IMazeSolver
    {
        private readonly Maze _maze;
        public Queue<MazeCell> ValidPath { get; set; } = new Queue<MazeCell>();
        public Queue<MazeCell> VisitedCells { get; set; } = new Queue<MazeCell>();
        public Dictionary<MazeCell, double> AlgorithmDisplayMap { get; set; } = new Dictionary<MazeCell, double>();

        public BreadthFirstSearch(Maze maze)
        {
            _maze = maze;
        }

        public List<MazeCell> FindValidPath()
        {
            var startingCell = _maze.StartCell;

            ValidPath.Enqueue(startingCell);
            VisitedCells.Enqueue(startingCell);

            var currentCell = startingCell;

            while (currentCell != _maze.EndCell)
            {
                // So that I don't get stuck, I need to Enqueue all neighbours of current node before further processing
                // See Obsidian notes for why this algo was getting stuck sometimes compared to DFS
                foreach (MazeCell neighbourCell in currentCell.Neighbours)
                {
                    if ((!VisitedCells.Contains(neighbourCell)) && (currentCell.IsConnectedTo(neighbourCell)))
                    {
                        ValidPath.Enqueue(neighbourCell);
                    }
                }

                if (IsMovePossible(currentCell, out MazeCell? nextCell))
                {
                    currentCell = nextCell;
                    ValidPath.Enqueue(currentCell);
                    VisitedCells.Enqueue(currentCell);
                }
                else
                {
                    ValidPath.Dequeue();
                    currentCell = ValidPath.Peek();
                }
            }

            List<MazeCell> result = new List<MazeCell>(ValidPath);
            MapAlgorithmDisplay();
            _maze.PopulateFinalDisplayTimer();

            return result;
        }

        private bool IsMovePossible(MazeCell currentCell, out MazeCell? nextCell)
        {
            foreach (MazeCell neighbourCell in currentCell.Neighbours)
            {
                if ((!VisitedCells.Contains(neighbourCell)) && (currentCell.IsConnectedTo(neighbourCell)))
                {
                    nextCell = neighbourCell;
                    return true;
                }
            }
            nextCell = null;
            return false;
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
                        delay += 0.02;
                    }
                    else
                    {
                        delay += 0.01; // adding this extra delay makes branch transitions smoother
                    }
                }
                _maze.AlgorithmDisplayMap = AlgorithmDisplayMap;
            }
        }

    }
}
