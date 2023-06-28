using System.IO;

namespace MazeMvcApp.Models.MazeSolverAlgos
{
    public class BiDirectionalDFS : IMazeSolver
    {
        private readonly Maze _maze;
        public Dictionary<MazeCell, double> AlgorithmDisplayMap { get; set; } = new Dictionary<MazeCell, double>();

        public BiDirectionalDFS(Maze maze)
        {
            _maze = maze;
        }

        // Return ordered list of cells that represent a valid path through the Maze
        // See https://varsubham.medium.com/maze-path-finding-using-dfs-e9c5fa14106f 
        public List<MazeCell> FindValidPath()
        {
            MazeCell startCellTopPath = _maze.StartCell;
            MazeCell startCellBottomPath = _maze.EndCell;

            Stack<MazeCell> pathTopStart = new Stack<MazeCell>();
            Stack<MazeCell> pathBottomStart = new Stack<MazeCell>();

            pathTopStart.Push(startCellTopPath);
            pathBottomStart.Push(startCellBottomPath);

            startCellTopPath.Traversed = true;
            startCellBottomPath.Traversed = true;

            MazeCell currentCellTopPath = startCellTopPath;
            MazeCell currentCellBottomPath = startCellBottomPath;
            bool turn = true; // true for topStart's turn, false for bottomStart
            double delay = 0.1d; // for the display mapping - Don't like mixing it here, make method later with instance var

            while (!IsIntersecting(pathTopStart, pathBottomStart, out MazeCell? intCell))
            {
                // Don't add extra delays for Bi-Directional if it does ContainsKey
                if (!AlgorithmDisplayMap.ContainsKey(currentCellTopPath))
                {
                    AlgorithmDisplayMap.Add(currentCellTopPath, delay);
                }

                if (!AlgorithmDisplayMap.ContainsKey(currentCellBottomPath))
                {
                    AlgorithmDisplayMap.Add(currentCellBottomPath, delay);
                }

                if(turn == true)
                {
                    if (IsMovePossible(currentCellTopPath, out MazeCell? nextCell))
                    {
                        currentCellTopPath = nextCell;
                        currentCellTopPath.Traversed = true;
                        pathTopStart.Push(currentCellTopPath);
                        turn = false;
                    }
                    else
                    {
                        pathTopStart.Pop();
                        currentCellTopPath = pathTopStart.Peek();
                    }
                }
                
                if(turn == false)
                {
                    if (IsMovePossible(currentCellBottomPath, out MazeCell? nextCell))
                    {
                        currentCellBottomPath = nextCell;
                        currentCellBottomPath.Traversed = true;
                        pathBottomStart.Push(currentCellBottomPath);
                        turn = true;
                    }
                    else
                    {
                        pathBottomStart.Pop();
                        currentCellBottomPath = pathBottomStart.Peek();
                    }
                }
                delay += 0.02;
            }

            // Need to add this here for now since intersectionCell gets popped off stack
            // Very ugly, will need to refactor all this
            if(IsIntersecting(pathTopStart, pathBottomStart, out MazeCell? intersectionCell))
            {
                if(!AlgorithmDisplayMap.ContainsKey(intersectionCell))
                {
					AlgorithmDisplayMap.Add(intersectionCell, delay);
				}
			}

			List<MazeCell> result = new List<MazeCell>(pathTopStart.Union(pathBottomStart));
            result.Reverse(); // reverse because path is a stack
            _maze.UntraverseAllCells();
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

        private bool IsIntersecting(Stack<MazeCell> pathTopStart, Stack<MazeCell> pathBottomStart, out MazeCell? intersectionCell)
        {
            // Careful here that if I manipulate the stacks, it will affect the actual references used 
            // Maybe create 2 new objects
            MazeCell[] pathTopStartArr = pathTopStart.ToArray();
            MazeCell[] pathBottomStartArr = pathBottomStart.ToArray();
            
            foreach(MazeCell topPathCell in pathTopStartArr)
            {
                foreach(MazeCell bottomPathCell in pathBottomStartArr)
                {
                    // Logic: The paths cannot hold the same cell with the current logic since if a cell has been marked
                    // as traversed, it can't be added to the path. So we check if topPathCell has bottomPathCell as a
                    // neighbour, and if path is open between them
                    // May rework this logic later and not have Traversed property in MazeCell
                    if((topPathCell.Neighbours.Contains(bottomPathCell))
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
