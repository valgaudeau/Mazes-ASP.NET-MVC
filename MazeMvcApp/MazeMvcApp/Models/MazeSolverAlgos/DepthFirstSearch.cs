namespace MazeMvcApp.Models.MazeSolverAlgos
{
    public class DepthFirstSearch : IMazeSolver
    {
        private readonly Maze _maze;

        public DepthFirstSearch(Maze maze)
        {
            _maze = maze;
        }

        // Return ordered list of cells that represent a valid path through the Maze
        // See https://varsubham.medium.com/maze-path-finding-using-dfs-e9c5fa14106f 
        public List<MazeCell> FindValidPath()
        {
            var startingCell = _maze.StartCell;
            Stack<MazeCell> path = new Stack<MazeCell>();
            path.Push(startingCell);
            startingCell.Traversed = true;
            var currentCell = startingCell;

            while (currentCell != _maze.EndCell)
            {
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

            return result;
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
