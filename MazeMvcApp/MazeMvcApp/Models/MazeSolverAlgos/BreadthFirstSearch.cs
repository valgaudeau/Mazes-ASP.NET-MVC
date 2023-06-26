namespace MazeMvcApp.Models.MazeSolverAlgos
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
            double delay = 0.1d;
            var startingCell = _maze.StartCell;
            Queue<MazeCell> path = new();
            path.Enqueue(startingCell);
            startingCell.Traversed = true;
            var currentCell = startingCell;

            while (currentCell != _maze.EndCell)
            {
                if (!AlgorithmDisplayMap.ContainsKey(currentCell))
                {
                    AlgorithmDisplayMap.Add(currentCell, delay);
                    delay += 0.02;
                }
                else
                {
                    delay += 0.02;
                }

                MazeCell nextCell = new MazeCell();

                if (IsMovePossible(currentCell, out nextCell))
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

        private bool IsMovePossible(MazeCell currentCell, out MazeCell nextCell)
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
