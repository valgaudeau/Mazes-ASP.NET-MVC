namespace MazeMvcApp.Models.MazeGenerationAlgos
{
    /*
    * Hunt & Kill Pseudo Algorithm, which we use to open edges and create a perfect maze.
    * A perfect maze is a maze with no loop areas and no unreachable areas. In a perfect maze, 
    * every cell is connected to another cell, and there is always one unique path between any 
    * two cells. STEPS:
    * 1) Choose a random starting cell.
    * 2) Enter the “kill” mode.Randomly select an unvisited neighbour from the current cell. 
    * Remove the edges between the current cell and the selected neighbour, and the selected 
    * neighbour becomes the current cell. Repeat until the current cell has no unvisited neighbours.
    * 3) Enter the “hunt” mode. Scan the grid to look for an unvisited cell that is adjacent to 
    * a visited cell. If found, then connect the two cells by removing their adjacent edges, and 
    * let the formerly unvisited cell be the new starting cell.
    * 4) Repeat steps 2 and 3 until the “hunt” mode scans the entire grid and finds no unvisited cells.
    */

    public class HuntAndKill : IMazeGenerator
    {
        // DECISION: Should HuntAndKill have a Maze field or do we pass Maze in the method?
        private readonly Maze _maze;

        public HuntAndKill(Maze maze)
        {
            _maze = maze;
        }

        public void GenerateMaze()
        {
            MazeCell startingCell = _maze.GetRandomCell();

            while (startingCell != null)
            {
                Kill(startingCell);
                startingCell = Hunt();
            }

        }

        private void Kill(MazeCell currentCell)
        {
            MazeCell nextCell = currentCell.GetUnvisitedNeighbour();

            if (nextCell != null)
            {
                currentCell.ConnectTo(nextCell);
                Kill(nextCell);
            }
        }

        private MazeCell Hunt()
        {
            // Need to randomize order in which we "hunt" because if we always scan
            // the grid from top-left to bottom-right, then there is a good chance
            // that the solutions generated for our maze will favor top rows because
            // they will be connected at an earlier time in the hunt & kill recursion
            // To address this, generate random arrays for col & row
            int[] randomRowNumbers = Utils.GenerateRandomArray(_maze.NRow);
            int[] randomColNumbers = Utils.GenerateRandomArray(_maze.NCol);

            for (int i = 0; i < randomRowNumbers.Length; i++)
            {
                for (int j = 0; j < randomColNumbers.Length; j++)
                {
                    var currentCell = _maze.Cells[i][j];

                    if (currentCell.Visited)
                    {
                        continue;
                    }

                    var nextCell = currentCell.RandomizedNeighbours.Find(c => c.Visited);

                    if (nextCell != null)
                    {
                        currentCell.ConnectTo(nextCell);
                        return currentCell;
                    }
                }
            }
            return null;
        }

    }
}
