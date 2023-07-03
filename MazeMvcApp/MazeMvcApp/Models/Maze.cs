namespace MazeMvcApp.Models
{
    public class Maze
    {
        public MazeCell[][] Cells { get; set; }

        // See https://stackoverflow.com/questions/3917796/how-to-implement-a-read-only-property
        // Goal here was avoiding anyone changing NRow or NCol after Maze creation
        public int NRow { get; set; } = 0;
        public int NCol { get; set; } = 0;
        public MazeCell StartCell { get; set; }
        public MazeCell EndCell { get; set; }
        public bool IsSolved { get; set; } = false;
        public List<MazeCell> ValidPath { get; set; }
        public Dictionary<MazeCell, double> ValidPathDelayMap { get; set; } = new Dictionary<MazeCell, double>();
        public Dictionary<MazeCell, double> AlgorithmDisplayMap { get; set; } = new Dictionary<MazeCell, double>();
        // FinalDisplayTimer stores the highest double value from AlgorithmDisplayMap and is used to trigger
        // the valid path display once the algorithm display finishes
        public double FinalDisplayTimer { get; set; } 

        public Maze(int nRow, int nCol)
        {
            NRow = nRow;
            NCol = nCol;

            // initialize cells in our maze
            MazeCell[][] cells = new MazeCell[nRow][];

            for (int i = 0; i < nRow; i++)
            {
                cells[i] = new MazeCell[nCol];

                for (int j = 0; j < nCol; j++)
                {
                    cells[i][j] = new MazeCell(i, j);
                }
            }

            Cells = cells;

            // Populate Cell Neighbours
            for (int i = 0; i < NRow; i++)
            {
                for (int j = 0; j < NCol; j++)
                {
                    MapCellNeighbours(Cells[i][j]);
                }
            }

            StartCell = Cells[0][0];
            StartCell.TopEdge = false;
            EndCell = Cells[NRow - 1][NCol - 1];
            EndCell.BottomEdge = false;
        }

        public Maze()
        {

        }

        public MazeCell GetRandomCell()
        {
            var random = new Random();

            return Cells[random.Next(NRow)][random.Next(NCol)];
        }

        /*
        // This method is because we don't want some of the edges to be displayed twice visually
        public void MapEdgeDisplays()
        {
            for (int i = 0; i < NRow; i++)
            {
                for (int j = 0; j < NCol; j++)
                {
                    // For each possible edge, check if there is a visited neighbour
                    // Since we move row by row left to right, we should just need to check Top & left edge
                    MazeCell currentCell = Cells[i][j];
                    currentCell.DisplayTopEdge = currentCell.TopEdge;
                    currentCell.DisplayLeftEdge = currentCell.LeftEdge;

                    // The cell above displays its bottom edge, so don't display top edge for currentCell
                    // Remember that currentCell.TopEdge will match cellAbove.BottomEdge value
                    if (currentCell.Y > 0 && currentCell.TopEdge == true)
                    {
                        currentCell.DisplayTopEdge = false;
                    }

                    // The cell to the left displays its right edge, so don't display top edge for currentCell
                    if (currentCell.X > 0 && currentCell.LeftEdge == true)
                    {
                        currentCell.DisplayLeftEdge = false;
                    }
                }
            }

            // MazeCell topNeighbour = currentCell.Neighbours.FirstOrDefault(cell => cell.Y == currentCell.Y - 1);
            // MazeCell leftNeighbour = currentCell.Neighbours.FirstOrDefault(cell => cell.X == currentCell.X - 1);
        }*/

        public void MapDisplayDelay()
        {
            double delay = 0.1d;
            Dictionary<MazeCell, double> validPathDelayMap = new();

            // Map valid path cells to transition-delay values
            foreach (var cell in ValidPath)
            {
                validPathDelayMap.Add(cell, delay);
                delay += 0.05;
            }

            ValidPathDelayMap = validPathDelayMap;
        }

        public void PopulateFinalDisplayTimer()
        {
            if(AlgorithmDisplayMap.Count > 0)
            {
                // Note that taking last value didn't work for all algos
                double max = 0;

                foreach(KeyValuePair<MazeCell, double> kvp in AlgorithmDisplayMap)
                {
                    max = Math.Max(max, kvp.Value);
                }
                FinalDisplayTimer = max;
            }
        }

        private void MapCellNeighbours(MazeCell mazeCell)
        {
            // Cell to the right
            if (mazeCell.X + 1 < NCol)
            {
                mazeCell.RandomizedNeighbours.Add(Cells[mazeCell.Y][mazeCell.X + 1]);
                mazeCell.Neighbours.Add(Cells[mazeCell.Y][mazeCell.X + 1]);
            }

            // Cell to the left
            if (mazeCell.X - 1 >= 0)
            {
                mazeCell.RandomizedNeighbours.Add(Cells[mazeCell.Y][mazeCell.X - 1]);
                mazeCell.Neighbours.Add(Cells[mazeCell.Y][mazeCell.X - 1]);
            }

            // Cell below
            if (mazeCell.Y - 1 >= 0)
            {
                mazeCell.RandomizedNeighbours.Add(Cells[mazeCell.Y - 1][mazeCell.X]);
                mazeCell.Neighbours.Add(Cells[mazeCell.Y - 1][mazeCell.X]);
            }

            // Cell above
            if (mazeCell.Y + 1 < NRow)
            {
                mazeCell.RandomizedNeighbours.Add(Cells[mazeCell.Y + 1][mazeCell.X]);
                mazeCell.Neighbours.Add(Cells[mazeCell.Y + 1][mazeCell.X]);
            }

            // Shuffle for RandomizedNeighbours to randomize access order by Hunt and Kill methods
            mazeCell.RandomizedNeighbours = Utils.ShuffleList(mazeCell.RandomizedNeighbours);
        }

    }
}
