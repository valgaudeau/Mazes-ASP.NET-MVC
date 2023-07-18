## Project Overview

An ASP.NET MVC application which allows for the creation of customized mazes. It also lets users see how different pathfinding algorithms will work their way through the created mazes. Those algorithms include:
- Depth First Search
- Breadth First Search
- Dijkstra's algorithm
- Bidirectional Depth First Search
- Bidirectional Breadth First Search

This project is hosted on Azure at https://mazegenerator.azurewebsites.net/ .

## How To Use The App

- Once you have entered how many rows and columns you want, click on the <b>Generate Maze</b> button. Note that if there is already a maze, it will be replaced.
- Choose the pathfinding algorithm you want to see from the dropdown, click <b>Confirm</b>, and then click <b>Show Algorithm</b> to see it in action in your maze.
- Hold the left mouse-button down to draw your path, hold the right mouse-button down to eraze it.
- To restore the maze to its initial state, click on <b>Clear Maze</b>. If you have drawn your own path through the maze, you will need to clear it before you use <b>Show Algorithm</b>.

## More Info

- The <em>Hunt And Kill</em> algorithm is used as part of the maze creation process. If you want to learn more about this topic, see [Jamis Buck's blog](http://weblog.jamisbuck.org/2011/1/24/maze-generation-hunt-and-kill-algorithm) .
- The generated mazes are <b>Perfect Mazes</b>. A perfect maze is a maze with no loop areas and no unreachable areas. In a perfect maze, every cell is connected to another cell, and there is always one unique path between any two cells.
- All of the generated mazes are unweighted, meaning that the edges connecting the vertices (vertices being the individual mazecells here) do not have any associated numerical weights or costs. In this case, we set all of the weights to 1, which is why Dijkstra's algorithm performs the same as BFS.

## Technical Info
- ASP.NET MVC App
- .NET v.7.0
- C# v.11








