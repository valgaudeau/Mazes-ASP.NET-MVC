using System.IO;

namespace MazeMvcApp.Models.MazeSolverAlgos
{
    public class BiDirectionalDFS : IMazeSolver
    {
        private readonly Maze _maze;
        public List<MazeCell> ValidPath { get; set; } = new List<MazeCell>();
        public Queue<MazeCell> VisitedCellsTopPath { get; set; } = new Queue<MazeCell>();
        public Queue<MazeCell> VisitedCellsBottomPath { get; set; } = new Queue<MazeCell>();
        public Dictionary<MazeCell, double> AlgorithmDisplayMap { get; set; } = new Dictionary<MazeCell, double>();
        private Stack<MazeCell> _pathTopStart = new Stack<MazeCell>();
        private Stack<MazeCell> _pathBottomStart = new Stack<MazeCell>();
        private MazeCell _intersectionCell;

        public BiDirectionalDFS(Maze maze)
        {
            _maze = maze;
        }

        public List<MazeCell> FindValidPath()
        {
            MazeCell startCellTopPath = _maze.StartCell;
            MazeCell startCellBottomPath = _maze.EndCell;

            _pathTopStart.Push(startCellTopPath);
            _pathBottomStart.Push(startCellBottomPath);
            VisitedCellsTopPath.Enqueue(startCellTopPath);
            VisitedCellsBottomPath.Enqueue(startCellBottomPath);

            MazeCell currentCellTopPath = startCellTopPath;
            MazeCell currentCellBottomPath = startCellBottomPath;
            bool turn = true; // true for topStart's turn, false for bottomStart

            double delay = 0.1d;

            while (true) 
            {
                if (!AlgorithmDisplayMap.ContainsKey(currentCellTopPath))
                {
                    AlgorithmDisplayMap.Add(currentCellTopPath, delay);
                    delay += 0.05;
                }

                if (!AlgorithmDisplayMap.ContainsKey(currentCellBottomPath))
                {
                    AlgorithmDisplayMap.Add(currentCellBottomPath, delay);
                    delay += 0.05;
                }

                if ( (!IsIntersecting(out MazeCell? intersectionCell)) 
                  && (currentCellTopPath != _maze.EndCell) 
                  && (currentCellBottomPath != _maze.StartCell) )
                {
                    if (turn == true)
                    {
                        if (IsMovePossible(currentCellTopPath, turn, out MazeCell? nextCell))
                        {
                            currentCellTopPath = nextCell;
                            _pathTopStart.Push(currentCellTopPath);
                            VisitedCellsTopPath.Enqueue(currentCellTopPath);
                            turn = false;
                        }
                        else
                        {
                            _pathTopStart.Pop();
                            currentCellTopPath = _pathTopStart.Peek();
                        }
                    }

                    if (turn == false)
                    {
                        if (IsMovePossible(currentCellBottomPath, turn, out MazeCell? nextCell))
                        {
                            currentCellBottomPath = nextCell;
                            _pathBottomStart.Push(currentCellBottomPath);
                            VisitedCellsBottomPath.Enqueue(currentCellBottomPath);
                            turn = true;
                        }
                        else
                        {
                            _pathBottomStart.Pop();
                            currentCellBottomPath = _pathBottomStart.Peek();
                        }
                    }
                }
                else
                {
                    _intersectionCell = intersectionCell;
                    break;
                }

            }

            ReconstructPath();
            _maze.AlgorithmDisplayMap = AlgorithmDisplayMap;
            _maze.PopulateFinalDisplayTimer();

            return ValidPath;
        }

        private bool IsIntersecting(out MazeCell? intersectionCell)
        {
            MazeCell[] pathTopStartArr = _pathTopStart.ToArray();
            MazeCell[] pathBottomStartArr = _pathBottomStart.ToArray();
            
            foreach(MazeCell topPathCell in pathTopStartArr)
            {
                foreach(MazeCell bottomPathCell in pathBottomStartArr)
                {
                    // If one of the paths contains cell of the other path, that's the intersection cell
                    if(_pathTopStart.Contains(bottomPathCell))
                    {
                        intersectionCell = bottomPathCell;
                        return true;
                    }
                    else if(_pathBottomStart.Contains(topPathCell))
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

        private void ReconstructPath()
        {
            // if intersection cell is still null, that means either top or bottom path solved the maze,
            // and no intersection happened between the 2 paths
            // So I can check if top contains end else bottom contains start and return that as valid path
            if(_intersectionCell == null)
            {
                if(_pathTopStart.Contains(_maze.EndCell))
                {
                    ValidPath = _pathTopStart.ToList();
                }
                else
                {
                    ValidPath = _pathBottomStart.ToList();
                }
            }
            else
            {
                // if intersection cell isn't null, I can pop MazeCell entries from wherever the
                // intersection cell is in both of the paths stacks and that will be the valid path
                var currentCellBottom = _pathBottomStart.Peek();

                // Travel to intersection cell and then pop remaining nodes to valid path
                while (currentCellBottom != _intersectionCell)
                {
                    _pathBottomStart.Pop();
                    currentCellBottom = _pathBottomStart.Peek();
                }

                while (currentCellBottom != _maze.EndCell)
                {
                    ValidPath.Add(currentCellBottom);
                    _pathBottomStart.Pop();
                    currentCellBottom = _pathBottomStart.Peek();
                }

                // Reverse since we want to display path end -> Start, not intersection cell -> end
                ValidPath.Reverse();
                var currentCellTop = _pathTopStart.Peek();

                // Travel to intersection cell in _pathTopStart then pop remaining nodes to valid path
                while (currentCellTop != _intersectionCell)
                {
                    _pathTopStart.Pop();
                    currentCellTop = _pathTopStart.Peek();
                }

                // This is to avoid adding intersection cell to valid path again here
                _pathTopStart.Pop();
                currentCellTop = _pathTopStart.Peek();

                while (currentCellTop != _maze.StartCell)
                {
                    ValidPath.Add(currentCellTop);
                    _pathTopStart.Pop();
                    currentCellTop = _pathTopStart.Peek();
                }
            }

        }

    }
}
