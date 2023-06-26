namespace MazeMvcApp.Models.MazeSolverAlgos
{
    public class DepthFirstSearch : IMazeSolver
    {
        private readonly Maze _maze;
        public Dictionary<MazeCell, double> AlgorithmDisplayMap { get; set; } = new Dictionary<MazeCell, double>();

        public DepthFirstSearch(Maze maze)
        {
            _maze = maze;
        }

        // Return ordered list of cells that represent a valid path through the Maze
        // See https://varsubham.medium.com/maze-path-finding-using-dfs-e9c5fa14106f 
        public List<MazeCell> FindValidPath()
        {
            double delay = 0.1d; // for the display mapping - Don't like mixing it here but not sure how to avoid it
            var startingCell = _maze.StartCell;
            Stack<MazeCell> path = new Stack<MazeCell>();
            path.Push(startingCell);
            startingCell.Traversed = true;
            var currentCell = startingCell;

            while (currentCell != _maze.EndCell)
            {
                if(!AlgorithmDisplayMap.ContainsKey(currentCell))
                {
                    AlgorithmDisplayMap.Add(currentCell, delay);
                    delay += 0.02;
                }
                else
                {
                    delay += 0.02; // I like adding this extra delay, makes transition smoother when moving to another search branch
                }

                MazeCell nextCell = new MazeCell();

                if (IsMovePossible(currentCell, out nextCell))
                {
                    currentCell = nextCell;
                    currentCell.Traversed = true;
                    path.Push(currentCell);
                }
                else
                {
                    path.Pop();
                    currentCell = path.Peek();
                }
            }

            List<MazeCell> result = new List<MazeCell>(path);
            result.Reverse(); // reverse because path is a stack
            // We need this to reset all cells to untraversed once we've found valid path as we may want to apply different
            // search algorithms on the same maze for display algorithm purposes and need cells to be untraversed for it to work
            _maze.UntraverseAllCells(); 

            return result;
        }

        public Dictionary<MazeCell, double> GetAlgorithmSearchDisplayMap()
        {
            if(AlgorithmDisplayMap.Count > 1) // check if dictionary already populated
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
