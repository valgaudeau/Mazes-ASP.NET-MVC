namespace MazeMvcApp.Models
{
    public static class Utils
    {
        private static readonly Random _random = new Random();

        public static int[] GenerateRandomArray(int max)
        {
            int[] result = new int[max];

            for (int i = 0; i < max; i++)
            {
                result[i] = _random.Next(0, max);
            }
            return result;
        }

        public static List<MazeCell> ShuffleList(List<MazeCell> neighbours)
        {
            // See https://stackoverflow.com/questions/273313/randomize-a-listt
            for (int i = neighbours.Count; i > 0; i--)
            {
                Swap(neighbours, i - 1, _random.Next(0, i));
            }
            return neighbours;
        }

        private static void Swap(List<MazeCell> neighbours, int i, int j)
        {
            var temp = neighbours[i];
            neighbours[i] = neighbours[j];
            neighbours[j] = temp;
        }

    }
}
