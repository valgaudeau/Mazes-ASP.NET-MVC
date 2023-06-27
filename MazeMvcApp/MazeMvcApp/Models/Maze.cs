namespace MazeMvcApp.Models
{
    public class Maze
    {
        public MazeCell[][] Cells { get; set; }

        // See https://stackoverflow.com/questions/3917796/how-to-implement-a-read-only-property
        // Goal here was avoiding anyone changing NRow or NCol after Maze creation
        public int NRow { get; set; } = 1;
        public int NCol { get; set; } = 1;
        public MazeCell StartCell { get; set; }
        public MazeCell EndCell { get; set; }
        public bool IsSolved { get; set; } = false;
        public List<MazeCell> ValidPath { get; set; }
        public Dictionary<MazeCell, double> ValidPathDelayMap { get; set; } = new Dictionary<MazeCell, double>();
        public Dictionary<MazeCell, double> AlgorithmDisplayMap { get; set; } = new Dictionary<MazeCell, double>();

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

                    if (currentCell.Y > 0 && currentCell.TopEdge == true)
                    {
                        currentCell.DisplayTopEdge = false;
                    }

                    if (currentCell.X > 0 && currentCell.LeftEdge == true)
                    {
                        currentCell.DisplayLeftEdge = false;
                    }
                }
            }
        }

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

        // Because we use a static maze instance in the controller, this method should be used after an algorithm
        // has finished solving the maze so that the cells property traversed is set back to false and we can then
        // solve the maze again with a different algorithm using that property once again
        public void UntraverseAllCells()
        {
            foreach(var cellRow in Cells)
            {
                foreach(var cell in cellRow)
                {
                    cell.Traversed = false;
                }
            }
        }

        private void MapCellNeighbours(MazeCell mazeCell)
        {
            // Cell to the right
            if (mazeCell.X + 1 < NCol)
            {
                mazeCell.Neighbours.Add(Cells[mazeCell.Y][mazeCell.X + 1]);
                mazeCell.RandomizedNeighbours.Add(Cells[mazeCell.Y][mazeCell.X + 1]);
            }

            // Cell to the left
            if (mazeCell.X - 1 >= 0)
            {
                mazeCell.Neighbours.Add(Cells[mazeCell.Y][mazeCell.X - 1]);
                mazeCell.RandomizedNeighbours.Add(Cells[mazeCell.Y][mazeCell.X - 1]);
            }

            // Cell below
            if (mazeCell.Y - 1 >= 0)
            {
                mazeCell.Neighbours.Add(Cells[mazeCell.Y - 1][mazeCell.X]);
                mazeCell.RandomizedNeighbours.Add(Cells[mazeCell.Y - 1][mazeCell.X]);
            }

            // Cell above
            if (mazeCell.Y + 1 < NRow)
            {
                mazeCell.Neighbours.Add(Cells[mazeCell.Y + 1][mazeCell.X]);
                mazeCell.RandomizedNeighbours.Add(Cells[mazeCell.Y + 1][mazeCell.X]);
            }

            // Shuffle for RandomizedNeighbours to randomize access order by Hunt and Kill methods
            mazeCell.RandomizedNeighbours = Utils.ShuffleList(mazeCell.RandomizedNeighbours);
        }

    }
}
