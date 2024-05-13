using labyrinth_gamе.DataBase.Tables;
using labyrinth_gamе.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
    public enum TileType2 { Wall, Path, Start, End }
    public partial class Level_2 : Window
    {
        private int[,] maze;
        private Rectangle playerRect;
        private readonly int tileSize = 32;
        private Random rand = new Random();
        private int exitRow;
        private int exitCol;
        private int entranceRow;
        private int entranceCol;
        private int timeElapsed;
        private DispatcherTimer timer;
        private int timeTaken;
        private IGameState state;
        public Level_2()
        {
            InitializeComponent();
            state = new PlayingState(this);
            GenerateMaze();
            DrawMaze();
            DrawPlayer();
            InitializeTimer();
        }
        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!IsPaused())
            {
                timeElapsed++;

                int minutes = timeElapsed / 60;
                int seconds = timeElapsed % 60;
                TimeLabel.Content = $"Часу минуло: {minutes:D2}:{seconds:D2}";
            }
        }
        private bool IsPaused()
        {
            return state is PausedState;
        }
        private void GenerateMaze()
        {
            int rows = 20;
            int cols = 35;
            maze = new int[rows, cols];
            entranceRow = rows / 2;
            entranceCol = 0;
            maze[entranceRow, entranceCol] = (int)TileType2.Start;
            exitRow = rand.Next(1, rows - 2);
            exitCol = cols - 1;
            maze[exitRow, exitCol] = (int)TileType2.End;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    maze[i, j] = (int)TileType2.Wall;
                }
            }
            GenerateMazeRecursive(entranceRow, entranceCol, rows, cols);
            maze[exitRow + 1, exitCol] = (int)TileType2.Path;
            maze[entranceRow, entranceCol] = (int)TileType2.Start;
            maze[exitRow, exitCol] = (int)TileType2.End;
        }
        private void GenerateMazeRecursive(int row, int col, int totalRows, int totalCols)
        {
            maze[row, col] = (int)TileType2.Path;
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
                if (maze[newRow, newCol] == (int)TileType2.Wall)
                {
                    maze[newRow, newCol] = (int)TileType2.Path;
                    maze[(newRow + row) / 2, (newCol + col) / 2] = (int)TileType2.Path;

                    GenerateMazeRecursive(newRow, newCol, totalRows, totalCols);
                }
            }
        }
        private void DrawPlayer()
        {
            playerRect = new Rectangle();
            playerRect.Width = tileSize;
            playerRect.Height = tileSize;
            playerRect.Fill = Brushes.MediumSlateBlue;
            int startRow = maze.GetLength(0) / 2;
            int startCol = 0;
            playerRect.SetValue(Canvas.LeftProperty, (double)startCol * tileSize);
            playerRect.SetValue(Canvas.TopProperty, (double)startRow * tileSize);
            canvas_1.Children.Add(playerRect);
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

                    switch ((TileType2)maze[row, col])
                    {
                        case TileType2.Wall:
                            rect.Fill = Brushes.DarkBlue;
                            break;
                        case TileType2.Path:
                            rect.Fill = Brushes.AliceBlue;
                            break;
                        case TileType2.Start:
                            rect.Fill = Brushes.LightSkyBlue;
                            break;
                        case TileType2.End:
                            rect.Fill = Brushes.MediumSlateBlue;
                            exitRow = row;
                            exitCol = col;
                            break;
                        default:
                            break;
                    }
                    if ((TileType2)maze[row, col] == TileType2.Start)
                    {
                        entranceRow = row;
                        entranceCol = col;
                        rect.Fill = Brushes.LightBlue;
                    }
                    canvas_1.Children.Add(rect);
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
                if (maze[newRow, newCol] != (int)TileType2.Wall)
                {
                    if (CheckExitCollision(newRow, newCol))
                    {
                        HandleExitCollision(newRow, newCol);
                    }
                    else if (CheckMovementValidity(newRow, newCol))
                    {
                        playerRect.SetValue(Canvas.LeftProperty, newX);
                        playerRect.SetValue(Canvas.TopProperty, newY);
                    }
                }
            }
        }
        private bool CheckExitCollision(int newRow, int newCol)
        {
            return newRow == exitRow && newCol == exitCol;
        }

        private void HandleExitCollision(int newRow, int newCol)
        {
            timer.Stop();
            timeTaken = timeElapsed;
            CustomMessageBox messageBox = new CustomMessageBox("");
            Record record = new Record
            {
                UserId = User.CurrentUser.UserId,
                UserName = User.CurrentUser.UserName,
                Level = 2,
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

        private bool CheckMovementValidity(int newRow, int newCol)
        {
            return (Math.Abs(newRow - (int)(Canvas.GetTop(playerRect) / tileSize)) == 1 && newCol == (int)(Canvas.GetLeft(playerRect) / tileSize) && maze[newRow, newCol] != (int)TileType.Wall)
                        || (Math.Abs(newCol - (int)(Canvas.GetLeft(playerRect) / tileSize)) == 1 && newRow == (int)(Canvas.GetTop(playerRect) / tileSize) && maze[newRow, newCol] != (int)TileType.Wall);
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            state.HandleKeyPress(e.Key);
        }
        private void Image_MouseDown_16(object sender, MouseButtonEventArgs e)
        {
            PauseGame();
        }
        private void PauseGame()
        {
            timer.Stop();
            state = new PausedState(this);
            MessageBoxResult result = MessageBox.Show("Гра призупинена. Натисніть ОК, щоб продовжити гру.", "", MessageBoxButton.OK);
            if (result == MessageBoxResult.OK)
            {
                ResumeGame();
            }
        }
        private void ResumeGame()
        {
            state = new PlayingState(this);
            timer.Start();
        }
        private void RestartGame()
        {
            double x = entranceCol * tileSize;
            double y = entranceRow * tileSize;
            playerRect.SetValue(Canvas.LeftProperty, x);
            playerRect.SetValue(Canvas.TopProperty, y);
            GenerateMaze();
            DrawMaze();
            DrawPlayer();
            {
                timeElapsed = 0;
                timeTaken = 0;
                timer.Start();
            }
        }
        private void Image_MouseDown_15(object sender, MouseButtonEventArgs e)
        {
            RestartGame();
        }
        private void Image_MouseDown_8(object sender, RoutedEventArgs e)
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
        private void Image_MouseDown_11(object sender, MouseButtonEventArgs e)
        {
            Levels levelsPage = new Levels();
            Window mainWindow = new MainWindow();
            Frame frame = new Frame();
            frame.NavigationService.Navigate(levelsPage);
            mainWindow.Content = frame;
            mainWindow.Show();
            this.Close();
        }
        public interface IGameState
        {
            void HandleKeyPress(Key key);
        }

        public class PlayingState : IGameState
        {
            private readonly Level_2 _context;

            public PlayingState(Level_2 context)
            {
                _context = context;
            }

            public void HandleKeyPress(Key key)
            {
                switch (key)
                {
                    case Key.Left:
                    case Key.A:
                        _context.MovePlayer(-32, 0);
                        break;
                    case Key.Right:
                    case Key.D:
                        _context.MovePlayer(32, 0);
                        break;
                    case Key.Up:
                    case Key.W:
                        _context.MovePlayer(0, -32);
                        break;
                    case Key.Down:
                    case Key.S:
                        _context.MovePlayer(0, 32);
                        break;
                }
            }
        }
        public class PausedState : IGameState
        {
            private readonly Level_2 _context;

            public PausedState(Level_2 context)
            {
                _context = context;
            }

            public void HandleKeyPress(Key key)
            {
                if (key == Key.Escape || key == Key.Enter)
                {
                    _context.ResumeGame();
                }
            }
        }

    }
}