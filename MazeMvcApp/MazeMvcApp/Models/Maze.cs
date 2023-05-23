namespace MazeMvcApp.Models
{
    public class Maze
    {
        private MazeCell[][] Cells { get; set; }
        private readonly int _nRow;
        private readonly int _nCol;

        public Maze(int nRow, int nCol)
        {
            // initialize cells in our maze
            for (int i = 0; i < nRow; i++)
            {
                Cells[i] = new MazeCell[nCol];

                for (int j = 0; j < nCol; j++)
                {
                    Cells[i][j] = new MazeCell(i, j);
                }
            }

            _nRow = nRow;
            _nCol = nCol;

            // need to populate cell neighbours here too
        }
        
        public MazeCell GetRandomCell()
        {
            var random = new Random();

            return Cells[random.Next(_nRow)][random.Next(_nCol)];
        }


    }
}
