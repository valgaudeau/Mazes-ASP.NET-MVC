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

        public Maze(int nRow, int nCol)
        {
            NRow = nRow;
            NCol = nCol;

            // initialize cells in our maze
            MazeCell[][] cells = new MazeCell[nRow][];

            for (int i = 0; i < nRow; i++)
            {
                cells[i] = new MazeCell[nRow];

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
            EndCell = Cells[NRow - 1][NCol - 1];
        }

        public Maze()
        {

        }

        public MazeCell GetRandomCell()
        {
            var random = new Random();

            return Cells[random.Next(NRow)][random.Next(NCol)];
        }

        private void MapCellNeighbours(MazeCell mazeCell)
        {
            // Cell below
            if (mazeCell.Y - 1 >= 0)
            {
                mazeCell.Neighbours.Add(Cells[mazeCell.Y - 1][mazeCell.X]);
            }

            // Cell above
            if (mazeCell.Y + 1 < NRow)
            {
                mazeCell.Neighbours.Add(Cells[mazeCell.Y + 1][mazeCell.X]);
            }

            // Cell to the left
            if (mazeCell.X - 1 >= 0)
            {
                mazeCell.Neighbours.Add(Cells[mazeCell.Y][mazeCell.X - 1]);
            }

            // Cell to the right
            if (mazeCell.X + 1 < NCol)
            {
                mazeCell.Neighbours.Add(Cells[mazeCell.Y][mazeCell.X + 1]);
            }

            // Shuffle neighbours to randomize access order by Hunt and Kill methods
            mazeCell.Neighbours = Utils.ShuffleList(mazeCell.Neighbours);
        }

    }
}
