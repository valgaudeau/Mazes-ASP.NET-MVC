namespace MazeMvcApp.Models.MazeSolverAlgos
{
    public class Dijkstra : IMazeSolver
    {
        private readonly Maze _maze;
        public List<MazeCell> ValidPath { get; set; } = new List<MazeCell>();
        public Queue<MazeCell> VisitedCells { get; set; } = new Queue<MazeCell>();
        public Dictionary<MazeCell, double> AlgorithmDisplayMap { get; set; } = new Dictionary<MazeCell, double>();

        public Dijkstra(Maze maze)
        {
            _maze = maze;
        }

        public List<MazeCell> FindValidPath()
        {
            InitializeGraph();

            PriorityQueue<MazeCell> priorityQueue = new PriorityQueue<MazeCell>();
            priorityQueue.Enqueue(_maze.StartCell, 0);

            while (priorityQueue.Count > 0)
            {
                MazeCell currentCell = priorityQueue.Dequeue();

                if (currentCell == _maze.EndCell)
                {
                    break;
                }

                VisitedCells.Enqueue(currentCell);

                foreach (MazeCell neighbour in currentCell.Neighbours)
                {
                    if( (!VisitedCells.Contains(neighbour)) && (currentCell.IsConnectedTo(neighbour)) )
                    {
                        int newDistance = currentCell.Distance + 1; // Assume unweighted graph

                        if (newDistance < neighbour.Distance)
                        {
                            neighbour.Distance = newDistance;
                            priorityQueue.Enqueue(neighbour, newDistance);
                        }
                    }
                }
            }

            ReconstructPath();
            MapAlgorithmDisplay();
            _maze.PopulateFinalDisplayTimer();

            return ValidPath;
        }

        private void InitializeGraph()
        {
            for(int i = 0; i < _maze.Cells.Length; i++) 
            { 
                for(int j = 0; j < _maze.Cells[i].Length; j++)
                {
                    MazeCell currCell = _maze.Cells[i][j];
                    currCell.Distance = int.MaxValue;
                }
            }

            _maze.StartCell.Distance = 0;
        }

        private void ReconstructPath()
        {
            MazeCell current = _maze.EndCell;

            while (current != _maze.StartCell)
            {
                ValidPath.Insert(0, current);
                current = GetPreviousCell(current);
            }

            ValidPath.Reverse();
            // ValidPath.Insert(0, _maze.EndCell);
        }

        private MazeCell GetPreviousCell(MazeCell cell)
        {
            // Find and return the previous cell based on the shortest distance
            foreach (MazeCell neighbour in cell.Neighbours)
            {
                if ( (neighbour.Distance == cell.Distance - 1) && (cell.IsConnectedTo(neighbour)) )
                {
                    return neighbour;
                }
            }

            return null;
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
                        delay += 0.05;
                    }
                    else
                    {
                        delay += 0.02; // adding this extra delay makes branch transitions smoother
                    }
                }
                _maze.AlgorithmDisplayMap = AlgorithmDisplayMap;
            }
        }

    }
}
