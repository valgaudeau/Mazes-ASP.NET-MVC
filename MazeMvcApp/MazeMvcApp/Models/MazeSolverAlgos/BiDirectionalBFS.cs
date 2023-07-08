namespace MazeMvcApp.Models.MazeSolverAlgos
{
    public class BiDirectionalBFS : IMazeSolver
    {
        private readonly Maze _maze;
        public List<MazeCell> ValidPath { get; set; } = new List<MazeCell>();
        public Queue<MazeCell> VisitedCellsTopPath { get; set; } = new Queue<MazeCell>();
        public Queue<MazeCell> VisitedCellsBottomPath { get; set; } = new Queue<MazeCell>();
        public Dictionary<MazeCell, double> AlgorithmDisplayMap { get; set; } = new Dictionary<MazeCell, double>();
        private Dictionary<MazeCell, MazeCell> _cellCameFromTop = new Dictionary<MazeCell, MazeCell>();
        private Dictionary<MazeCell, MazeCell> _cellCameFromBottom = new Dictionary<MazeCell, MazeCell>();
        private MazeCell _intersectionCell;

        public BiDirectionalBFS(Maze maze)
        {
            _maze = maze;
        }

        public List<MazeCell> FindValidPath()
        {
            var startCellTopPath = _maze.StartCell;
            var startCellBottomPath = _maze.EndCell;

            _cellCameFromTop[startCellTopPath] = null;
            Queue<MazeCell> topQueue = new Queue<MazeCell>();
            Queue<MazeCell> bottomQueue = new Queue<MazeCell>();

            topQueue.Enqueue(startCellTopPath);
            bottomQueue.Enqueue(startCellBottomPath);
            VisitedCellsTopPath.Enqueue(startCellTopPath);
            VisitedCellsBottomPath.Enqueue(startCellBottomPath);

            MazeCell topPathCurrentCell = startCellTopPath;
            MazeCell bottomPathCurrentCell = startCellBottomPath;

            double delay = 0.1d;

            bool turn = true; // true for topStart's turn, false for bottomStart

            while (true)
            {

                if (!AlgorithmDisplayMap.ContainsKey(topPathCurrentCell))
                {
                    AlgorithmDisplayMap.Add(topPathCurrentCell, delay);
                }

                if (!AlgorithmDisplayMap.ContainsKey(bottomPathCurrentCell))
                {
                    AlgorithmDisplayMap.Add(bottomPathCurrentCell, delay);
                }

                delay += 0.05;

                if ( (!IsIntersecting(topQueue, bottomQueue, out MazeCell ? intersectionCell)) 
                  && (topPathCurrentCell != _maze.EndCell) 
                  && (bottomPathCurrentCell != _maze.StartCell) )
                {

                    foreach (MazeCell neighbourCell in topPathCurrentCell.Neighbours)
                    {
                        if ((!_cellCameFromTop.ContainsKey(neighbourCell))
                         && (topPathCurrentCell.IsConnectedTo(neighbourCell)) )
                        {
                            _cellCameFromTop[neighbourCell] = topPathCurrentCell;
                            topQueue.Enqueue(neighbourCell);

                            if (!VisitedCellsTopPath.Contains(neighbourCell))
                            {
                                VisitedCellsTopPath.Enqueue(neighbourCell);
                            }
                        }
                    }

                    foreach (MazeCell neighbourCell in bottomPathCurrentCell.Neighbours)
                    {
                        if ((!_cellCameFromBottom.ContainsKey(neighbourCell))
                         && (bottomPathCurrentCell.IsConnectedTo(neighbourCell)) )
                        {
                            _cellCameFromBottom[neighbourCell] = bottomPathCurrentCell;
                            bottomQueue.Enqueue(neighbourCell);

                            if (!VisitedCellsBottomPath.Contains(neighbourCell))
                            {
                                VisitedCellsBottomPath.Enqueue(neighbourCell);
                            }
                        }
                    }

                    if (turn == true)
                    {
                        topQueue.Dequeue();
                        topPathCurrentCell = topQueue.Peek();
                        turn = false;
                    }

                    if (turn == false)
                    {
                        bottomQueue.Dequeue();
                        bottomPathCurrentCell = bottomQueue.Peek();
                        turn = true;
                    }
                }
                else
                {
                    _intersectionCell = intersectionCell;
                    break;
                }

            }

            ReconstructPath(topQueue, bottomQueue, delay);
            _maze.AlgorithmDisplayMap = AlgorithmDisplayMap;
            _maze.PopulateFinalDisplayTimer();

            return ValidPath;
        }

        private bool IsIntersecting(Queue<MazeCell> topQueue, Queue<MazeCell> bottomQueue, out MazeCell? intersectionCell)
        {
            // Careful here that if I manipulate the stacks, it will affect the actual references used 
            // Using Paths not VisitedCells because we know there is only 1 valid path through the maze
            MazeCell[] pathTopStartArr = topQueue.ToArray();
            MazeCell[] pathBottomStartArr = bottomQueue.ToArray();

            foreach (MazeCell topPathCell in pathTopStartArr)
            {
                foreach (MazeCell bottomPathCell in pathBottomStartArr)
                {
                    // If one of the paths contains cell of the other path, that's the intersection cell
                    if (topQueue.Contains(bottomPathCell))
                    {
                        intersectionCell = bottomPathCell;
                        return true;
                    }
                    else if (bottomQueue.Contains(topPathCell))
                    {
                        intersectionCell = topPathCell;
                        return true;
                    }
                }
            }
            intersectionCell = null;
            return false;
        }

        private void ReconstructPath(Queue<MazeCell> pathTopStart, Queue<MazeCell> pathBottomStart, double delay)
        {
            if (_intersectionCell == null)
            {
                if (pathTopStart.Contains(_maze.EndCell))
                {
                    ValidPath = pathTopStart.ToList();
                }
                else
                {
                    ValidPath = pathBottomStart.ToList();
                }
            }
            else
            {
                var currentCellBottom = _intersectionCell;

                while (currentCellBottom != _maze.EndCell)
                {
                    ValidPath.Add(currentCellBottom);
                    currentCellBottom = _cellCameFromBottom[currentCellBottom];
                }
                ValidPath.Add(_maze.EndCell);
                ValidPath.Reverse();

                var currentCellTop = _intersectionCell;
                // Move to cell after intersection cell not to add it twice to valid path
                currentCellTop = _cellCameFromTop[currentCellTop];

                while (currentCellTop != _maze.StartCell)
                {
                    ValidPath.Add(currentCellTop);
                    currentCellTop = _cellCameFromTop[currentCellTop];
                }
                ValidPath.Add(_maze.StartCell);

                // pass delay as argument and add any display cells missing here
                // Solves issue of 1 cell sometimes not being included in the display map
                foreach(var cell in ValidPath)
                {
                    if(!AlgorithmDisplayMap.ContainsKey(cell))
                    {
                        AlgorithmDisplayMap.Add(cell, delay);
                    }
                }
            }
        }

    }
}
