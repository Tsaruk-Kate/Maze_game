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
        private readonly LevelBase _levelBase;
        public Level_1()
        {
            _levelBase = new LevelBase()
            {
                tileSize = 45
            };
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
            _levelBase.timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _levelBase.timer.Tick += Timer_Tick;
            _levelBase.timer.Start();
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
            _levelBase.timeElapsed++;
            int minutes = _levelBase.timeElapsed / 60;
            int seconds = _levelBase.timeElapsed % 60;
            TimeLabel.Content = $"Часу минуло: {minutes:D2}:{seconds:D2}";
        }

        private void GenerateMaze()
        {
            int rows = 15;
            int cols = 25;
            _levelBase.maze = new int[rows, cols];
            InitializeMaze(rows, cols);
            GenerateMazeRecursive(_levelBase.entranceRow, _levelBase.entranceCol, rows, cols);
            _levelBase.maze[_levelBase.exitRow + 1, _levelBase.exitCol] = (int)TileType.Path;
            _levelBase.maze[_levelBase.entranceRow, _levelBase.entranceCol] = (int)TileType.Start;
            _levelBase.maze[_levelBase.exitRow, _levelBase.exitCol] = (int)TileType.End;
        }

        private void InitializeMaze(int rows, int cols)
        {
            _levelBase.entranceRow = rows / 2;
            _levelBase.entranceCol = 0;
            _levelBase.maze[_levelBase.entranceRow, _levelBase.entranceCol] = (int)TileType.Start;
            _levelBase.exitRow = _levelBase.rand.Next(1, rows - 2);
            _levelBase.exitCol = cols - 1;
            _levelBase.maze[_levelBase.exitRow, _levelBase.exitCol] = (int)TileType.End;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    _levelBase.maze[i, j] = (int)TileType.Wall;
                }
            }
        }

        private void GenerateMazeRecursive(int row, int col, int totalRows, int totalCols)
        {
            _levelBase.maze[row, col] = (int)TileType.Path;
            List<int> directions = GetShuffledDirections();
            foreach (int direction in directions)
            {
                int newRow, newCol;
                if (TryGetNewPosition(row, col, direction, out newRow, out newCol, totalRows, totalCols))
                {
                    if (_levelBase.maze[newRow, newCol] == (int)TileType.Wall)
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
            return directions.OrderBy(x => _levelBase.rand.Next()).ToList();
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
            _levelBase.maze[newRow, newCol] = (int)TileType.Path;
            _levelBase.maze[(newRow + row) / 2, (newCol + col) / 2] = (int)TileType.Path;
        }
        
        private void DrawPlayer()
        {
            _levelBase.playerRect = new Rectangle()
            {
                Width = _levelBase.tileSize,
                Height = _levelBase.tileSize,
                Fill = Brushes.GreenYellow
            };
            int startRow = _levelBase.maze.GetLength(0) / 2;
            int startCol = 0;
            LevelBase.SetRectangleCoordinates(_levelBase.playerRect,(double)startCol * _levelBase.tileSize, (double)startRow * _levelBase.tileSize);
            canvas.Children.Add(_levelBase.playerRect);
        }


        private void DrawMaze()
        {
            for (int row = 0; row < _levelBase.maze.GetLength(0); row++)
            {
                for (int col = 0; col < _levelBase.maze.GetLength(1); col++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = _levelBase.tileSize;
                    rect.Height = _levelBase.tileSize;
                    LevelBase.SetRectangleCoordinates(rect, (double)col * _levelBase.tileSize, (double)row * _levelBase.tileSize);

                    switch ((TileType)_levelBase.maze[row, col])
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
                            _levelBase.exitRow = row;
                            _levelBase.exitCol = col;
                            break;
                        default:
                            break;
                    }

                    if ((TileType)_levelBase.maze[row, col] == TileType.Start)
                    {
                        _levelBase.entranceRow = row;
                        _levelBase.entranceCol = col;
                        rect.Fill = Brushes.LightBlue;
                    }

                    canvas.Children.Add(rect);
                }
            }
        }

        private void MovePlayer(int deltaX, int deltaY)
        {
            double newX = Canvas.GetLeft(_levelBase.playerRect) + deltaX;
            double newY = Canvas.GetTop(_levelBase.playerRect) + deltaY;

            int newCol = (int)(newX / _levelBase.tileSize);
            int newRow = (int)(newY / _levelBase.tileSize);

            if (IsMoveValid(newRow, newCol))
            {
                UpdatePlayerPosition(newX, newY, newRow, newCol);
            }
        }

        private bool IsMoveValid(int newRow, int newCol)
        {
            return newRow >= 0 && newRow < _levelBase.maze.GetLength(0) &&
                   newCol >= 0 && newCol < _levelBase.maze.GetLength(1) && _levelBase.maze[newRow, newCol] != (int)TileType.Wall;
        }

        private void UpdatePlayerPosition(double newX, double newY, int newRow, int newCol)
        {
            if (newRow == _levelBase.exitRow && newCol == _levelBase.exitCol)
            {
                UpdatePlayerPositionAndEndGame(newX, newY);
            }
            else if (IsAdjacentCell(newRow, newCol))
            {
                LevelBase.SetRectangleCoordinates(_levelBase.playerRect, newX, newY);
            }
        }

        private void UpdatePlayerPositionAndEndGame(double newX, double newY)
        {
            LevelBase.SetRectangleCoordinates(_levelBase.playerRect, newX, newY);
            StopTimerAndEndGame();
        }

        private void StopTimerAndEndGame()
        {
            _levelBase.timer.Stop();
            _levelBase.timeTaken = _levelBase.timeElapsed;
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
                Time = _levelBase.timeTaken + " сек"
            };
            using (var db = new DataBaseContext())
            {
                db.Records.Add(record);
                db.SaveChanges();
            }

            messageBox.messageBoxText.Text = $"Вітаємо, ви виграли! Витрачено часу: {_levelBase.timeTaken} секунд.";
            messageBox.ShowDialog();
        }

        private bool IsAdjacentCell(int newRow, int newCol)
        {
            return Math.Abs(newRow - (int)(Canvas.GetTop(_levelBase.playerRect) / _levelBase.tileSize)) == 1 &&
                   newCol == (int)(Canvas.GetLeft(_levelBase.playerRect) / _levelBase.tileSize) && _levelBase.maze[newRow, newCol] != (int)TileType.Wall ||
                   Math.Abs(newCol - (int)(Canvas.GetLeft(_levelBase.playerRect) / _levelBase.tileSize)) == 1 &&
                   newRow == (int)(Canvas.GetTop(_levelBase.playerRect) / _levelBase.tileSize) && _levelBase.maze[newRow, newCol] != (int)TileType.Wall;
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
                    MovePlayer(-_levelBase.tileSize, 0);
                    break;
                case Key.Right:
                case Key.D:
                    MovePlayer(_levelBase.tileSize, 0);
                    break;
                case Key.Up:
                case Key.W:
                    MovePlayer(0, -_levelBase.tileSize);
                    break;
                case Key.Down:
                case Key.S:
                    MovePlayer(0, _levelBase.tileSize);
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
            _levelBase.timer.Stop();
            MessageBox.Show("Гра призупинена. Натисніть ОК, щоб продовжити гру.");
            isPaused = false;
            _levelBase.timer.Start();
        }

        private void RestartGame()
        {
            LevelBase.SetRectangleCoordinates(_levelBase.playerRect,
                _levelBase.entranceCol * _levelBase.tileSize,
                _levelBase.entranceRow * _levelBase.tileSize);
            GenerateMaze();
            DrawMaze();
            DrawPlayer();
            {
                _levelBase.timeElapsed = 0;
                _levelBase.timeTaken = 0;
                _levelBase.timer.Start();
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