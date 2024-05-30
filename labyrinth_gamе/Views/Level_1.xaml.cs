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
            InitializeWindow();
            InitializeGame();
        }

        private void InitializeGame()
        {
            GenerateMaze();
            DrawMaze();
            DrawPlayer();
            InitializeTimer();
        }

        private void InitializeWindow()
        {
            this.WindowState = WindowState.Maximized;
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                UpdateElapsedTime();
            }
        }

        private void UpdateElapsedTime()
        {
            timeElapsed++;
            int minutes = timeElapsed / 60;
            int seconds = timeElapsed % 60;
            TimeLabel.Content = $"Часу минуло: {minutes:D2}:{seconds:D2}";
        }

        private void GenerateMaze()
        {
            int rows = 15;
            int cols = 25;
            maze = new int[rows, cols];
            InitializeMaze(rows, cols);
            GenerateMazeRecursive(entranceRow, entranceCol, rows, cols);
            maze[exitRow + 1, exitCol] = (int)TileType.Path;
            maze[entranceRow, entranceCol] = (int)TileType.Start;
            maze[exitRow, exitCol] = (int)TileType.End;
        }

        private void InitializeMaze(int rows, int cols)
        {
            entranceRow = rows / 2;
            entranceCol = 0;
            maze[entranceRow, entranceCol] = (int)TileType.Start;
            exitRow = rand.Next(1, rows - 2);
            exitCol = cols - 1;
            maze[exitRow, exitCol] = (int)TileType.End;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    maze[i, j] = (int)TileType.Wall;
                }
            }
        }

        private void GenerateMazeRecursive(int row, int col, int totalRows, int totalCols)
        {
            maze[row, col] = (int)TileType.Path;
            List<int> directions = GetShuffledDirections();
            foreach (int direction in directions)
            {
                int newRow, newCol;
                if (TryGetNewPosition(row, col, direction, out newRow, out newCol, totalRows, totalCols))
                {
                    if (maze[newRow, newCol] == (int)TileType.Wall)
                    {
                        CarvePath(row, col, newRow, newCol);
                        GenerateMazeRecursive(newRow, newCol, totalRows, totalCols);
                    }
                }
            }
        }

        private List<int> GetShuffledDirections()
        {
            List<int> directions = new List<int> { 0, 1, 2, 3 };
            return directions.OrderBy(x => rand.Next()).ToList();
        }

        private bool TryGetNewPosition(int row, int col, int direction, out int newRow, out int newCol, int totalRows,
            int totalCols)
        {
            newRow = row;
            newCol = col;

            switch (direction)
            {
                case 0:
                    newRow -= 2;
                    break;
                case 1:
                    newCol += 2;
                    break;
                case 2:
                    newRow += 2;
                    break;
                case 3:
                    newCol -= 2;
                    break;
                default:
                    return false;
            }

            return newRow >= 0 && newRow < totalRows && newCol >= 0 && newCol < totalCols;
        }

        private void CarvePath(int row, int col, int newRow, int newCol)
        {
            maze[newRow, newCol] = (int)TileType.Path;
            maze[(newRow + row) / 2, (newCol + col) / 2] = (int)TileType.Path;
        }

        private void DrawPlayer()
        {
            playerRect = new Rectangle()
            {
                Width = tileSize,
                Height = tileSize,
                Fill = Brushes.GreenYellow
            };
            int startRow = maze.GetLength(0) / 2;
            int startCol = 0;
            SetPlayerRect((double)startCol * tileSize, (double)startRow * tileSize);
            canvas.Children.Add(playerRect);
        }

        private void SetPlayerRect(double xCoordinate, double yCoordinate)
        {
            playerRect.SetValue(Canvas.LeftProperty, xCoordinate);
            playerRect.SetValue(Canvas.TopProperty, yCoordinate);
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

            if (IsMoveValid(newRow, newCol))
            {
                UpdatePlayerPosition(newX, newY, newRow, newCol);
            }
        }

        private bool IsMoveValid(int newRow, int newCol)
        {
            return newRow >= 0 && newRow < maze.GetLength(0) &&
                   newCol >= 0 && newCol < maze.GetLength(1) &&
                   maze[newRow, newCol] != (int)TileType.Wall;
        }

        private void UpdatePlayerPosition(double newX, double newY, int newRow, int newCol)
        {
            if (newRow == exitRow && newCol == exitCol)
            {
                UpdatePlayerPositionAndEndGame(newX, newY);
            }
            else if (IsAdjacentCell(newRow, newCol))
            {
                SetPlayerRect(newX, newY);
            }
        }

        private void UpdatePlayerPositionAndEndGame(double newX, double newY)
        {
            SetPlayerRect(newX, newY);
            StopTimerAndEndGame();
        }

        private void StopTimerAndEndGame()
        {
            timer.Stop();
            timeTaken = timeElapsed;
            SaveRecordAndShowMessage();
            Close();
        }

        private void SaveRecordAndShowMessage()
        {
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
        }

        private bool IsAdjacentCell(int newRow, int newCol)
        {
            return Math.Abs(newRow - (int)(Canvas.GetTop(playerRect) / tileSize)) == 1 &&
                   newCol == (int)(Canvas.GetLeft(playerRect) / tileSize) &&
                   maze[newRow, newCol] != (int)TileType.Wall ||
                   Math.Abs(newCol - (int)(Canvas.GetLeft(playerRect) / tileSize)) == 1 &&
                   newRow == (int)(Canvas.GetTop(playerRect) / tileSize) &&
                   maze[newRow, newCol] != (int)TileType.Wall;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            HandleMovementKeys(e.Key);
        }

        private void HandleMovementKeys(Key key)
        {
            switch (key)
            {
                case Key.Left:
                case Key.A:
                    MovePlayer(-tileSize, 0);
                    break;
                case Key.Right:
                case Key.D:
                    MovePlayer(tileSize, 0);
                    break;
                case Key.Up:
                case Key.W:
                    MovePlayer(0, -tileSize);
                    break;
                case Key.Down:
                case Key.S:
                    MovePlayer(0, tileSize);
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

        private void RestartGame()
        {
            SetPlayerRect(entranceCol * tileSize, entranceRow * tileSize);
            GenerateMaze();
            DrawMaze();
            DrawPlayer();
            {
                timeElapsed = 0;
                timeTaken = 0;
                timer.Start();
            }
        }

        private void Image_MouseDown_13(object sender, MouseButtonEventArgs e)
        {
            RestartGame();
        }

        private void Image_MouseDown_10(object sender, MouseButtonEventArgs e)
        {
            NavigateToPage(new Levels());
        }

        private void Image_MouseDown_7(object sender, RoutedEventArgs e)
        {
            // Створення екземпляру фабрики користувачів
            IUserFactory userFactory = new UserFactory();
            // Передача фабрики користувачів у конструктор Labyrinth
            NavigateToPage(new Labyrinth(userFactory));
        }

        private void NavigateToPage(Page page)
        {
            Frame frame = new Frame();
            Window mainWindow = new MainWindow();
            frame.Navigate(page);
            mainWindow.Content = frame;
            mainWindow.Show();
            this.Close();
        }
    }
}