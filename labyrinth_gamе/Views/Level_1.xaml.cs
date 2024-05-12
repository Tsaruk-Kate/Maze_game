using labyrinth_gamе.DataBase.Tables;
using labyrinth_gamе.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace labyrinth_gamе.Views
{
    public enum TileType { Wall, Path, Start, End }
    public partial class Level_1 : Window
    {
        private int[,] maze;
        private Rectangle playerRect;
        private readonly int tileSize = 45;
        private Random rand = new Random();
        private int exitRow;
        private int exitCol;
        private int entranceRow;
        private int entranceCol;
        private int timeElapsed;
        private DispatcherTimer timer;
        private int timeTaken;
        public Level_1()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            GenerateMaze();
            DrawMaze();
            DrawPlayer();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                timeElapsed++;

                int minutes = timeElapsed / 60;
                int seconds = timeElapsed % 60;
                TimeLabel.Content = $"Часу минуло: {minutes:D2}:{seconds:D2}";
            }
        }
        private void GenerateMaze()
        {
            int rows = 15;
            int cols = 25;
            maze = new int[rows, cols];
            this.entranceRow = rows / 2;
            this.entranceCol = 0;
            maze[this.entranceRow, this.entranceCol] = (int)TileType.Start;
            this.exitRow = rand.Next(1, rows - 2);
            this.exitCol = cols - 1;
            maze[this.exitRow, this.exitCol] = (int)TileType.End;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    maze[i, j] = (int)TileType.Wall;
                }
            }
            GenerateMazeRecursive(entranceRow, entranceCol, rows, cols);
            maze[this.exitRow + 1, this.exitCol] = (int)TileType.Path;
            maze[this.exitRow + 1, this.exitCol] = (int)TileType.Path;
            maze[this.entranceRow, this.entranceCol] = (int)TileType.Start;
            maze[this.exitRow, this.exitCol] = (int)TileType.End;
        }
        private void GenerateMazeRecursive(int row, int col, int totalRows, int totalCols)
        {
            maze[row, col] = (int)TileType.Path;
            List<int> directions = new List<int> { 0, 1, 2, 3 };
            directions = directions.OrderBy(x => rand.Next()).ToList();
            foreach (int direction in directions)
            {
                int newRow = row;
                int newCol = col;
                if (direction == 0)
                {
                    newRow -= 2;
                    if (newRow < 0)
                        continue;
                }
                else if (direction == 1)
                {
                    newCol += 2;
                    if (newCol >= totalCols)
                        continue;
                }
                else if (direction == 2)
                {
                    newRow += 2;
                    if (newRow >= totalRows)
                        continue;
                }
                else if (direction == 3)
                {
                    newCol -= 2;
                    if (newCol < 0)
                        continue;
                }
                if (maze[newRow, newCol] == (int)TileType.Wall)
                {
                    maze[newRow, newCol] = (int)TileType.Path;
                    maze[(newRow + row) / 2, (newCol + col) / 2] = (int)TileType.Path;

                    GenerateMazeRecursive(newRow, newCol, totalRows, totalCols);
                }
            }
        }
        private void DrawPlayer()
        {
            playerRect = new Rectangle();
            playerRect.Width = tileSize;
            playerRect.Height = tileSize;
            playerRect.Fill = Brushes.GreenYellow;
            int startRow = maze.GetLength(0) / 2;
            int startCol = 0;
            playerRect.SetValue(Canvas.LeftProperty, (double)startCol * tileSize);
            playerRect.SetValue(Canvas.TopProperty, (double)startRow * tileSize);
            canvas.Children.Add(playerRect);
        }
        private void DrawMaze()
        {
            for (int row = 0; row < maze.GetLength(0); row++)
            {
                for (int col = 0; col < maze.GetLength(1); col++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = tileSize;
                    rect.Height = tileSize;
                    rect.SetValue(Canvas.LeftProperty, (double)col * tileSize);
                    rect.SetValue(Canvas.TopProperty, (double)row * tileSize);

                    switch ((TileType)maze[row, col])
                    {
                        case TileType.Wall:
                            rect.Fill = Brushes.DarkGreen;
                            break;
                        case TileType.Path:
                            rect.Fill = Brushes.LightYellow;
                            break;
                        case TileType.Start:
                            rect.Fill = Brushes.LightBlue;
                            break;
                        case TileType.End:
                            rect.Fill = Brushes.Yellow;
                            exitRow = row;
                            exitCol = col;
                            break;
                        default:
                            break;
                    }
                    if ((TileType)maze[row, col] == TileType.Start)
                    {
                        entranceRow = row;
                        entranceCol = col;
                        rect.Fill = Brushes.LightBlue;
                    }
                    canvas.Children.Add(rect);
                }
            }
        }
        private void MovePlayer(int deltaX, int deltaY)
        {
            double newX = Canvas.GetLeft(playerRect) + deltaX;
            double newY = Canvas.GetTop(playerRect) + deltaY;

            int newCol = (int)(newX / tileSize);
            int newRow = (int)(newY / tileSize);
            if (newRow >= 0 && newRow < maze.GetLength(0) && newCol >= 0 && newCol < maze.GetLength(1))
            {
                if (maze[newRow, newCol] != (int)TileType.Wall)
                {

                    if (newRow == exitRow && newCol == exitCol)
                    {
                        playerRect.SetValue(Canvas.LeftProperty, newX);
                        playerRect.SetValue(Canvas.TopProperty, newY);

                        if (newRow == exitRow && newCol == exitCol)
                        {
                            timer.Stop();
                            timeTaken = timeElapsed;
                            CustomMessageBox messageBox = new CustomMessageBox("");
                            Record record = new Record
                            {
                                UserId = User.CurrentUser.UserId,
                                UserName = User.CurrentUser.UserName,
                                Level = 1,
                                Time = timeTaken + " сек"
                            };
                            using (var db = new DataBaseContext())
                            {
                                db.Records.Add(record);
                                db.SaveChanges();
                            }
                            messageBox.messageBoxText.Text = $"Вітаємо, ви виграли! Витрачено часу: {timeTaken} секунд.";
                            messageBox.ShowDialog();
                            Close();
                        }
                    }
                    else if ((Math.Abs(newRow - (int)(Canvas.GetTop(playerRect) / tileSize)) == 1 && newCol == (int)(Canvas.GetLeft(playerRect) / tileSize) && maze[newRow, newCol] != (int)TileType.Wall)
                          || (Math.Abs(newCol - (int)(Canvas.GetLeft(playerRect) / tileSize)) == 1 && newRow == (int)(Canvas.GetTop(playerRect) / tileSize) && maze[newRow, newCol] != (int)TileType.Wall))
                    {
                        playerRect.SetValue(Canvas.LeftProperty, newX);
                        playerRect.SetValue(Canvas.TopProperty, newY);
                    }
                }
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    MovePlayer(-45, 0);
                    break;

                case Key.Right:
                    MovePlayer(45, 0);
                    break;

                case Key.Up:
                    MovePlayer(0, -45);
                    break;

                case Key.Down:
                    MovePlayer(0, 45);
                    break;

                case Key.A:
                    MovePlayer(-45, 0);
                    break;

                case Key.D:
                    MovePlayer(45, 0);
                    break;

                case Key.W:
                    MovePlayer(0, -45);
                    break;

                case Key.S:
                    MovePlayer(0, 45);
                    break;
            }
        }
        private bool isPaused = false;
        private void Image_MouseDown_14(object sender, MouseButtonEventArgs e)
        {
            PauseGame();
        }
        private void PauseGame()
        {
            isPaused = true;
            timer.Stop();
            MessageBox.Show("Гра призупинена. Натисніть ОК, щоб продовжити гру.");
            isPaused = false;
            timer.Start();

        }

        private void Image_MouseDown_10(object sender, MouseButtonEventArgs e)
        {
            Frame frame = new Frame();
            Window mainWindow = new MainWindow();
            frame.Navigate(new Levels());
            mainWindow.Content = frame;
            mainWindow.Show();
            this.Close();

        }
        private void Image_MouseDown_7(object sender, RoutedEventArgs e)
        {
            // Створення екземпляру фабрики користувачів
            IUserFactory userFactory = new UserFactory();
            Frame frame = new Frame();
            Window mainWindow = new MainWindow();
            // Передача фабрики користувачів у конструктор Labyrinth
            frame.Navigate(new Labyrinth(new UserFactory()));
            mainWindow.Content = frame;
            mainWindow.Show();
            this.Close();
        }
    }
}
