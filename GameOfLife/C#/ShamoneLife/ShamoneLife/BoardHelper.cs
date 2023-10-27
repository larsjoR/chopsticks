using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShamoneLife;

public static class BoardHelper
{

    public static void SetState(IReadOnlyList<(int x, int y)> allCoordinates, (State State, Rectangle Rect)[,] board)
    {
        foreach (var coordinate in allCoordinates)
        {
            var state = board[coordinate.x,coordinate.y].State;

            if (state.ShouldBeActive)
            {
                SetCoordinateState(coordinate, board);
                continue;
            }

            SetCoordinateState(coordinate, board, kill:true);
        }
    }
    public static void UpdateNextState(IReadOnlyList<(int x, int y)> allCoordinates, (State State, Rectangle Rect)[,] board)
    {
        /*
            Any live cell with fewer than two live neighbors dies, as if by underpopulation.
            Any live cell with two or three live neighbors lives on to the next generation.
            Any live cell with more than three live neighbors dies, as if by overpopulation.
            Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
         */
        foreach (var coordinate in allCoordinates)
        {
            var neighborCount = GetNeighborCount(coordinate, board);

            if (board[coordinate.x, coordinate.y].State.IsActive && neighborCount == 2 || neighborCount == 3)
            {
                board[coordinate.x, coordinate.y].State.ShouldBeActive = true;
                continue;
            }

            if (board[coordinate.x, coordinate.y].State.IsActive &&  neighborCount < 2 || neighborCount > 3)
            {
                board[coordinate.x, coordinate.y].State.ShouldBeActive = false;
            }
        }
    }
    public static int GetNeighborCount((int x, int y) coordinate, (State state, Rectangle rect)[,] board)
    {
        var dimension = board.GetLength(0);
        var surroundingCells = new List<(int x, int y)>();
        var x = coordinate.x;
        var y = coordinate.y;

        var north = (x, y - 1);
        surroundingCells.Add(north);
        var northEast = (x + 1, y - 1);
        surroundingCells.Add(northEast);
        var east = (x + 1, y);
        surroundingCells.Add(east);
        var southEast = (x + 1, y + 1);
        surroundingCells.Add(southEast);
        var south = (x, y + 1);
        surroundingCells.Add(south);
        var southWest = (x - 1, y + 1);
        surroundingCells.Add(southWest);
        var west = (x - 1, y);
        surroundingCells.Add(west);
        var northWest = (x - 1, y - 1);
        surroundingCells.Add(northWest);

        var neighborCount = surroundingCells
            .Where(coordinate => !IsOutOfBounds((coordinate.x, coordinate.y), dimension))
            .Count(x => board[x.x,x.y].state.IsActive);

        return neighborCount;
    }

    public static bool IsOutOfBounds((int x, int y) coordinate, int dimension)
    {
        var x = coordinate.x;
        var y = coordinate.y;
        return x < 0 || x >= dimension || y < 0 || y >= dimension;
    }

    public static void SetCoordinateState((int x, int y) coordinate, (State State, Rectangle Rect)[,] board, bool kill = false)
    {
        if (kill)
        {
            board[coordinate.x, coordinate.y].Rect.Fill = Brushes.White;
            board[coordinate.x, coordinate.y].State.IsActive = false;
            board[coordinate.x, coordinate.y].State.ShouldBeActive = false;
            return;
        }

        board[coordinate.x, coordinate.y].Rect.Fill = Brushes.Black;
        board[coordinate.x, coordinate.y].State.IsActive = true;
        
    }

    public static void UpdateCoordinates(List<(int x,int y)> coordinates, (State, Rectangle)[,] board)
    {
        foreach (var coordinate in coordinates)
        {
            BoardHelper.SetCoordinateState(coordinate, board);
        }
    }

    public static void MakeGliderGun((State, Rectangle)[,] board)
    {
        var coordinates = new List<(int x, int y)>
        {
            (1, 5), (1, 6), (2, 5), (2, 6), (11, 5), (11, 6), (11, 7),
            (12, 4), (12, 8), (13, 3), (13, 9), (14, 3), (14, 9), (15, 6),
            (16, 4), (16, 8), (17, 5), (17, 6), (17, 7), (18, 6),
            (21, 3), (21, 4), (21, 5), (22, 3), (22, 4), (22, 5),
            (23, 2), (23, 6), (25, 1), (25, 2), (25, 6), (25, 7),
            (35, 3), (35, 4), (36, 3), (36, 4)
        };

        UpdateCoordinates(coordinates, board);
    }

    public static void MakeSpinner((State, Rectangle)[,] board)
    {
        var coordinates = new List<(int x, int y)>
        {
            (10, 10), (10, 11), (10, 12)
        };

        UpdateCoordinates(coordinates, board);
    }

    public static void MakeGlider((State, Rectangle)[,] board)
    {
        var coordinates = new List<(int x, int y)>
        {
               (4, 20), (4, 21), (4, 22), (5, 22), (6, 21)
        };

        UpdateCoordinates(coordinates, board);
    }


    public static void MakePulsar((State, Rectangle)[,] board)
    {

        var coordinates = new List<(int x, int y)>
        {
            (48, 48), (48, 49), (48, 50), (48, 51), (48, 52), (48, 53), (48, 54),
            (50, 48), (50, 49), (50, 50), (50, 51), (50, 52), (50, 53), (50, 54),
            (52, 48), (52, 49), (52, 50), (52, 51), (52, 52), (52, 53), (52, 54),
            (56, 48), (56, 49), (56, 50), (56, 51), (56, 52), (56, 53), (56, 54),
            (72, 48), (72, 49), (72, 50), (72, 51), (72, 52), (72, 53), (72, 54)
        };

        UpdateCoordinates(coordinates, board);
    }

    public static void MakeSimpkinGliderGun((State, Rectangle)[,] board)
    {

        var coordinates = new List<(int x, int y)>
        {
            (2, 12), (2, 13), (3, 12), (3, 13),
            (7, 12), (7, 13), (7, 14), (8, 11), (8, 15), (9, 10), (9, 16), (10, 10), (10, 16), (11, 11), (11, 15), (12, 12), (12, 13), (12, 14),
            (13, 11), (13, 15), (14, 10), (14, 16), (15, 10), (15, 16), (16, 11), (16, 15), (17, 12), (17, 13), (17, 14),
            (22, 12), (22, 13), (23, 12), (23, 13),
            (25, 12), (25, 13), (25, 14), (26, 11), (26, 15), (27, 10), (27, 16), (28, 10), (28, 16), (29, 11), (29, 15), (30, 12), (30, 13), (30, 14),
            (31, 11), (31, 15), (32, 10), (32, 16), (33, 10), (33, 16), (34, 11), (34, 15), (35, 12), (35, 13), (35, 14),
            (42, 12), (42, 13), (43, 12), (43, 13)
        };

        UpdateCoordinates(coordinates, board);
    }

    public static void MakePufferTrain((State, Rectangle)[,] board)
    {

        var coordinates = new List<(int x, int y)>
        {
            // Initial puffer
            (10, 50), (10, 51), (10, 52),
            (11, 49), (11, 52),
            (12, 49),
            (13, 49), (13, 52),
            (14, 49),
            (15, 50), (15, 51), (15, 52),

            // Debris trail
            (22, 49), (22, 50), (22, 51),
            (23, 48), (23, 51),
            (24, 48),
            (25, 48), (25, 51),
            (26, 48),
            (27, 49), (27, 50), (27, 51),
        };

        UpdateCoordinates(coordinates, board);
    }

    static void MakeRandomBoard((State State, Rectangle Rect)[,] board)
    {
        
    }
}
