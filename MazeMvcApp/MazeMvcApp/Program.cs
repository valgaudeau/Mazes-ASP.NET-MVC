using MazeMvcApp.Models.MazeGenerationAlgos;
using MazeMvcApp.Models.MazeSolverAlgos;
using MazeMvcApp.Models;

namespace MazeMvcApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /*
            Maze maze = new Maze(5, 5);
            IMazeGenerator mazeGenerator = new HuntAndKill(maze);
            mazeGenerator.GenerateMaze();
            // Temporal dependence to consider: Maze has to be "perfect" before used in DFS class
            IMazeSolver mazeSolver = new DepthFirstSearch(maze);
            List<MazeCell> validPath = mazeSolver.FindValidPath();
            */

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}