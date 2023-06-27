namespace MazeMvcApp.Models
{
    public class MazeCell
    {
        // true -> Can't move through (Edge is present), false -> Can move through.
        public bool TopEdge { get; set; } = true;
        public bool LeftEdge { get; set; } = true;
        public bool RightEdge { get; set; } = true;
        public bool BottomEdge { get; set; } = true;
        public bool DisplayTopEdge { get; set; } = true;
        public bool DisplayLeftEdge { get; set; } = true;
        public int X { get; set; }
        public int Y { get; set; }
        // Use Neighbours in Pathfinding Algorithms to have always the same search behaviour
        public List<MazeCell> Neighbours { get; set; } = new();
        // Use RandomizedNeighbours in Hunt & Kill to avoid generating similar mazes
        public List<MazeCell> RandomizedNeighbours { get; set; } = new();

        // flag when using IMazeSolver implementation
        public bool Traversed { get; set; } = false;

        // flag when using IMazeGenerator implementation
        public bool Visited { get; private set; } = false;

        public MazeCell(int rowNumber, int columnNumber)
        {
            X = columnNumber;
            Y = rowNumber;
        }

        public MazeCell()
        {

        }

        public int GetNumberOfEdges()
        {
            return RandomizedNeighbours.Count;
        }

        public MazeCell GetUnvisitedNeighbour()
        {
            // INTERESTING: Even though I shuffle list of neighbours when I populate that list in the Maze constructor,
            // this method seems to always return neighbour cells in the same order. Must be something to do with LINQ 
            // return Neighbours.Find(c => c.Visited != true);
            foreach (MazeCell neighbour in RandomizedNeighbours)
            {
                if (!neighbour.Visited) return neighbour;
            }
            return null;
        }

        public void ConnectTo(MazeCell anotherCell)
        {
            if (anotherCell == null) return;

            if (Y == anotherCell.Y) // If same row, check if cells are neighbours
            {
                // anotherCell is to the left of the currentCell
                if (X - 1 == anotherCell.X)
                {
                    LeftEdge = false;
                    anotherCell.RightEdge = false;
                }
                // anotherCell is to the right of the currentCell
                else if (X + 1 == anotherCell.X)
                {
                    RightEdge = false;
                    anotherCell.LeftEdge = false;
                }
                else return;
            }
            else if (X == anotherCell.X) // if same column, check if cells are neighbours
            {
                // anotherCell is above currentCell
                if (Y - 1 == anotherCell.Y)
                {
                    TopEdge = false;
                    anotherCell.BottomEdge = false;
                }
                // anotherCell is below currentCell
                else if (Y + 1 == anotherCell.Y)
                {
                    BottomEdge = false;
                    anotherCell.TopEdge = false;
                }
                else return;
            }
            Visited = true;
            anotherCell.Visited = true;
        }

        // Checks if the path between the cell and its neighbour is open
        public bool IsConnectedTo(MazeCell neighbourCell)
        {
            if (neighbourCell == null) return false;

            if (Y == neighbourCell.Y) // Same row
            {
                if (X - 1 == neighbourCell.X) return !LeftEdge; // neighbour is to the left of current cell
                else if (X + 1 == neighbourCell.X) return !RightEdge; // neighbour is to the right of current cell
            }
            else if (X == neighbourCell.X) // same column
            {
                if (Y - 1 == neighbourCell.Y) return !TopEdge; // neighbour is above current cell
                else if (Y + 1 == neighbourCell.Y) return !BottomEdge; // neighbour is below current cell
            }
            return false;
        }

        public string ToEmptyString()
        {
            return " ";
        }
    }
}
