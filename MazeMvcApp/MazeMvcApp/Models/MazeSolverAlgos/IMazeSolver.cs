namespace MazeMvcApp.Models.MazeSolverAlgos
{
    public interface IMazeSolver
    {
        List<MazeCell> FindValidPath();
        Dictionary<MazeCell, double> GetAlgorithmSearchDisplayMap();
    }
}
