using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace labyrinth_gamе.Views;

public class LevelBase
{
    public int[,] maze;
    public Rectangle playerRect;
    public int tileSize { get; init; }
    public Random rand = new Random();
    public int exitRow;
    public int exitCol;
    public int entranceRow;
    public int entranceCol;
    public int timeElapsed;
    public DispatcherTimer timer;
    public int timeTaken;

    public LevelBase()
    {
    }
    public static void SetRectangleCoordinates(Rectangle rectangle, double xCoordinate, double yCoordinate)
    {
        rectangle.SetValue(Canvas.LeftProperty, xCoordinate);
        rectangle.SetValue(Canvas.TopProperty, yCoordinate);
    }
}