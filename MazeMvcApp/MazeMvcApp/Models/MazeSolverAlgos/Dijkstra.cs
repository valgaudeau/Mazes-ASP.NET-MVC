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
                    if(!VisitedCells.Contains(neighbour))
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

            ValidPath.Insert(0, _maze.EndCell);
        }

        private MazeCell GetPreviousCell(MazeCell cell)
        {
            // Find and return the previous cell based on the shortest distance
            foreach (MazeCell neighbour in cell.Neighbours)
            {
                if (neighbour.Distance == cell.Distance - 1)
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

    public class PriorityQueue<T>
    {
        private SortedDictionary<int, Queue<T>> dictionary;

        public int Count { get; private set; }

        public PriorityQueue()
        {
            dictionary = new SortedDictionary<int, Queue<T>>();
            Count = 0;
        }

        public void Enqueue(T item, int priority)
        {
            if (!dictionary.ContainsKey(priority))
            {
                dictionary[priority] = new Queue<T>();
            }

            dictionary[priority].Enqueue(item);
            Count++;
        }

        public T Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }
                
            var queue = dictionary.First().Value;
            T item = queue.Dequeue();
            if (queue.Count == 0)
            {
                dictionary.Remove(dictionary.First().Key);
            }
            Count--;

            return item;
        }
    }
}
