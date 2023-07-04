namespace MazeMvcApp.Models.MazeSolverAlgos
{
    public class DepthFirstSearch : IMazeSolver
    {
        private readonly Maze _maze;
        public Stack<MazeCell> ValidPath { get; set; } = new Stack<MazeCell>();
        // Make VisitedCells a Queue so I can easily populate AlgorithmDisplayMap AFTER FindValidPath
        public Queue<MazeCell> VisitedCells { get; set; } = new Queue<MazeCell>();
        public Dictionary<MazeCell, double> AlgorithmDisplayMap { get; set; } = new Dictionary<MazeCell, double>();

        public DepthFirstSearch(Maze maze)
        {
            _maze = maze;
        }

        // Return ordered list of cells that represent a valid path through the Maze
        // See https://varsubham.medium.com/maze-path-finding-using-dfs-e9c5fa14106f 
        public List<MazeCell> FindValidPath()
        {
            var startingCell = _maze.StartCell;

            ValidPath.Push(startingCell);
            VisitedCells.Enqueue(startingCell);

            var currentCell = startingCell;

            while (currentCell != _maze.EndCell)
            {
                if (IsMovePossible(currentCell, out MazeCell? nextCell))
                {
                    currentCell = nextCell;
                    ValidPath.Push(currentCell);
                    VisitedCells.Enqueue(currentCell);
                }
                else
                {
                    ValidPath.Pop();
                    currentCell = ValidPath.Peek();
                }
            }

            List<MazeCell> result = new List<MazeCell>(ValidPath);
            result.Reverse(); // reverse because path is a stack
            MapAlgorithmDisplay();
            _maze.PopulateFinalDisplayTimer();

            return result;
        }

        private bool IsMovePossible(MazeCell currentCell, out MazeCell? nextCell)
        {
            // Check if we can move to neighbour cells & if they are untraversed
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
            if(AlgorithmDisplayMap.Count == 0)
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
