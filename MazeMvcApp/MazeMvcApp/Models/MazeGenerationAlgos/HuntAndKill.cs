namespace MazeMvcApp.Models.MazeGenerationAlgos
{
    /*
     * Hunt & Kill Pseudo Algorithm:
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
        // Should HuntAndKill have a Maze field or do we pass Maze in the method?

        public void GenerateMaze(Maze maze)
        {
            MazeCell startingCell = maze.GetRandomCell();
            
            while(startingCell != null) 
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

        }

    }
}
