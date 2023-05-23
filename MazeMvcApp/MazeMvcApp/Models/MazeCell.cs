namespace MazeMvcApp.Models
{
    public class MazeCell
    {
        // true -> Can't move through (Edge is present), false -> Can move through.
        private bool TopEdge { get; set; } = true;
        private bool LeftEdge { get; set; } = true;
        private bool RightEdge { get; set; } = true; 
        private bool BottomEdge { get; set; } = true;
        private int X { get; set; }
        private int Y { get; set; }
        private List<MazeCell> Neighbours { get; set; }

        // flag when finding path through the maze
        private bool Traversed { get; set; } = false;

        // flag when using hunt & kill algorithm
        private bool Visited { get; set; } = false;

        public MazeCell(int rowNumber, int columnNumber)
        {
            X = columnNumber;
            Y = rowNumber;
        }
        
        public int GetNumberOfEdges()
        {
            return Neighbours.Count;
        }

        public MazeCell GetUnvisitedNeighbour()
        {
            // Thinking we may have to return List of unvisited neighbours but we'll see
            return Neighbours.Find(c => c.Visited != true);
        }

        public void ConnectTo(MazeCell anotherCell)
        {
            if(anotherCell== null) return;
            
            if(Y == anotherCell.Y) // If same row, check if cells are neighbours
            {
                if(X - 1 == anotherCell.X)
                {
                    RightEdge = false;
                    anotherCell.LeftEdge = false;
                }
                else if(X + 1 == anotherCell.X) 
                {
                    LeftEdge = false;
                    anotherCell.RightEdge = false;
                }
                else
                {
                    return;
                }
            }
            else if(X == anotherCell.X) // if same column, check if cells are neighbours
            { 
                
            }

        }

    }
}
