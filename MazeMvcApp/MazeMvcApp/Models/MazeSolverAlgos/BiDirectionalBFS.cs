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
            AlgorithmDisplayMap.Add(topPathCurrentCell, delay);
            AlgorithmDisplayMap.Add(bottomPathCurrentCell, delay);
            delay += 0.02;

            bool turn = true; // true for topStart's turn, false for bottomStart

            while (true)
            {
                if(!IsIntersecting(out MazeCell? intersectionCell))
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

                    // Not adding extra delays between branch swaps for BiDir
                    if (!AlgorithmDisplayMap.ContainsKey(topPathCurrentCell))
                    {
                        AlgorithmDisplayMap.Add(topPathCurrentCell, delay);
                    }

                    if (!AlgorithmDisplayMap.ContainsKey(bottomPathCurrentCell))
                    {
                        AlgorithmDisplayMap.Add(bottomPathCurrentCell, delay);
                    }

                    delay += 0.02;
                }
                else
                {
                    _intersectionCell = intersectionCell;
                    if (!AlgorithmDisplayMap.ContainsKey(_intersectionCell))
                    {
                        AlgorithmDisplayMap.Add(_intersectionCell, delay);
                    }

                    // Now Connect intersection cell to bottom path
                    foreach(MazeCell neighbour in intersectionCell.Neighbours)
                    {
                        if(intersectionCell.IsConnectedTo(neighbour) && (_cellCameFromBottom.ContainsKey(neighbour)) )
                        {
                            _cellCameFromBottom[intersectionCell] = neighbour;
                        }
                    }

                    break;
                }

            }

            // ReconstructPath();
            _maze.AlgorithmDisplayMap = AlgorithmDisplayMap;
            _maze.PopulateFinalDisplayTimer();

            return ValidPath;
        }

        private bool IsIntersecting(out MazeCell? intersectionCell)
        {
            // Careful here that if I manipulate the stacks, it will affect the actual references used 
            // Using Paths not VisitedCells because we know there is only 1 valid path through the maze
            MazeCell[] pathTopStartArr = VisitedCellsTopPath.ToArray();
            MazeCell[] pathBottomStartArr = VisitedCellsBottomPath.ToArray();

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

        /*
        private void ReconstructPath()
        {
            // To find Valid Path for my bidir-BFS setup, What I need is to travel from the intersection cell
            // to a cell which connects with end cell. Then I can add end cell to the dictionary linking to that
            // connecting cell, and I can find the way to the start by checking to which cell intersection cell
            // connects with from the top path and linking it to that in the dictionary. This is ridiculous though
            // Actually I do need 2 dictionaries to check if the neighbour cell of intersection cell comes from
            // top or bottom path...
            // Here I first need to travel from intersection cell to a cell which is connected to endCell
            MazeCell current = _intersectionCell;

            while (current.Neighbours _maze.StartCell)
            {

            }

            ValidPath.Insert(0, _maze.StartCell);
        }*/

    }
}
