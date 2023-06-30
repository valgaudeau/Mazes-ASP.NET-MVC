namespace MazeMvcApp.Models.MazeSolverAlgos
{
    public class BiDirectionalBFS : IMazeSolver
    {
        private readonly Maze _maze;
        public List<MazeCell> ValidPath { get; set; } = new List<MazeCell>();
        public Stack<MazeCell> PathTopStart { get; set; } = new Stack<MazeCell>();
        public Stack<MazeCell> PathBottomStart { get; set; } = new Stack<MazeCell>();
        public Queue<MazeCell> VisitedCellsTopPath { get; set; } = new Queue<MazeCell>();
        public Queue<MazeCell> VisitedCellsBottomPath { get; set; } = new Queue<MazeCell>();
        public Dictionary<MazeCell, double> AlgorithmDisplayMap { get; set; } = new Dictionary<MazeCell, double>();

        public BiDirectionalBFS(Maze maze)
        {
            _maze = maze;
        }

        public List<MazeCell> FindValidPath()
        {
            MazeCell startCellTopPath = _maze.StartCell;
            MazeCell startCellBottomPath = _maze.EndCell;

            PathTopStart.Push(startCellTopPath);
            PathBottomStart.Push(startCellBottomPath);
            VisitedCellsTopPath.Enqueue(startCellTopPath);
            VisitedCellsBottomPath.Enqueue(startCellBottomPath);

            MazeCell currentCellTopPath = startCellTopPath;
            MazeCell currentCellBottomPath = startCellBottomPath;
            bool turn = true; // true for topStart's turn, false for bottomStart

            double delay = 0.1d;
            AlgorithmDisplayMap.Add(currentCellTopPath, delay);
            AlgorithmDisplayMap.Add(currentCellBottomPath, delay);
            delay += 0.02;

            while (!IsIntersecting(out MazeCell? intCell))
            {
                if (turn == true)
                {
                    if (IsMovePossible(currentCellTopPath, turn, out MazeCell? nextCell))
                    {
                        currentCellTopPath = nextCell;
                        PathTopStart.Push(currentCellTopPath);
                        VisitedCellsTopPath.Enqueue(currentCellTopPath);
                        turn = false;
                    }
                    else
                    {
                        PathTopStart.Pop();
                        currentCellTopPath = PathTopStart.Peek();
                    }
                }

                if (turn == false)
                {
                    if (IsMovePossible(currentCellBottomPath, turn, out MazeCell? nextCell))
                    {
                        currentCellBottomPath = nextCell;
                        PathBottomStart.Push(currentCellBottomPath);
                        VisitedCellsBottomPath.Enqueue(currentCellBottomPath);
                        turn = true;
                    }
                    else
                    {
                        PathBottomStart.Pop();
                        currentCellBottomPath = PathBottomStart.Peek();
                    }
                }

                // This needs to be here for Bi-dir, not at the top, otherwise the last
                // added cell does not get included in the display mapping
                // Also not adding extra delays between branch swaps for BiDir
                if (!AlgorithmDisplayMap.ContainsKey(currentCellTopPath))
                {
                    AlgorithmDisplayMap.Add(currentCellTopPath, delay);
                }

                if (!AlgorithmDisplayMap.ContainsKey(currentCellBottomPath))
                {
                    AlgorithmDisplayMap.Add(currentCellBottomPath, delay);
                }

                delay += 0.02;
            }

            ValidPath = new List<MazeCell>(PathTopStart.Union(PathBottomStart));
            ValidPath.Reverse(); // reverse because path is a stack
            _maze.AlgorithmDisplayMap = AlgorithmDisplayMap;
            _maze.PopulateFinalDisplayTimer();

            return ValidPath;
        }

        private bool IsIntersecting(out MazeCell? intersectionCell)
        {
            // Careful here that if I manipulate the stacks, it will affect the actual references used 
            // Using Paths not VisitedCells because we know there is only 1 valid path through the maze
            MazeCell[] pathTopStartArr = PathTopStart.ToArray();
            MazeCell[] pathBottomStartArr = PathBottomStart.ToArray();

            foreach (MazeCell topPathCell in pathTopStartArr)
            {
                foreach (MazeCell bottomPathCell in pathBottomStartArr)
                {
                    // Logic: The paths cannot hold the same cell with the current logic since if a cell has been marked
                    // as traversed, it can't be added to the path. So we check if topPathCell has bottomPathCell as a
                    // neighbour, and if path is open between them
                    // May rework this logic later and not have Traversed property in MazeCell
                    if ((topPathCell.Neighbours.Contains(bottomPathCell))
                    && (topPathCell.IsConnectedTo(bottomPathCell)))
                    {
                        intersectionCell = topPathCell;
                        return true;
                    }
                }
            }
            intersectionCell = null;
            return false;
        }

        private bool IsMovePossible(MazeCell currentCell, bool turn, out MazeCell? nextCell)
        {
            // For Bi-Directional algo, I also need to tell the function which path is making the call
            // so that I can use the correct VisitedCells(Top/Bottom)Path check
            var VisitedCellsToCheck = turn == true ? VisitedCellsTopPath : VisitedCellsBottomPath;

            foreach (MazeCell neighbourCell in currentCell.Neighbours)
            {
                if ((!VisitedCellsToCheck.Contains(neighbourCell)) && (currentCell.IsConnectedTo(neighbourCell)))
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
