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
    public partial class Level_2 : Window
    {
        private readonly LevelBase _levelBase;
        private IGameState state;

        public Level_2()
        {
            _levelBase = new LevelBase()
            {
                tileSize = 32
            };
            InitializeComponent();
            state = new PlayingState(this);
            InitializeGame();
        }

        private void InitializeGame()
        {
            GenerateMaze();
            DrawMaze();
            DrawPlayer();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _levelBase.timer = new DispatcherTimer();
            _levelBase.timer.Interval = TimeSpan.FromSeconds(1);
            _levelBase.timer.Tick += Timer_Tick;
            _levelBase.timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!IsPaused())
            {
                _levelBase.timeElapsed++;

                int minutes = _levelBase.timeElapsed / 60;
                int seconds = _levelBase.timeElapsed % 60;
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
            _levelBase.maze = new int[rows, cols];
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

            GenerateMazeRecursive(_levelBase.entranceRow, _levelBase.entranceCol, rows, cols);
            _levelBase.maze[_levelBase.exitRow + 1, _levelBase.exitCol] = (int)TileType.Path;
            _levelBase.maze[_levelBase.entranceRow, _levelBase.entranceCol] = (int)TileType.Start;
            _levelBase.maze[_levelBase.exitRow, _levelBase.exitCol] = (int)TileType.End;
        }

        private void GenerateMazeRecursive(int row, int col, int totalRows, int totalCols)
        {
            _levelBase.maze[row, col] = (int)TileType.Path;
            List<int> directions = new List<int> { 0, 1, 2, 3 };
            directions = directions.OrderBy(x => _levelBase.rand.Next()).ToList();

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

                if (_levelBase.maze[newRow, newCol] == (int)TileType.Wall)
                {
                    _levelBase.maze[newRow, newCol] = (int)TileType.Path;
                    _levelBase.maze[(newRow + row) / 2, (newCol + col) / 2] = (int)TileType.Path;

                    GenerateMazeRecursive(newRow, newCol, totalRows, totalCols);
                }
            }
        }

        private void DrawPlayer()
        {
            _levelBase.playerRect = new Rectangle();
            _levelBase.playerRect.Width = _levelBase.tileSize;
            _levelBase.playerRect.Height = _levelBase.tileSize;
            _levelBase.playerRect.Fill = Brushes.MediumSlateBlue;
            int startRow = _levelBase.maze.GetLength(0) / 2;
            int startCol = 0;
            LevelBase.SetRectangleCoordinates(_levelBase.playerRect,
                (double)startCol * _levelBase.tileSize,
                (double)startRow * _levelBase.tileSize);
            canvas_1.Children.Add(_levelBase.playerRect);
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
                    LevelBase.SetRectangleCoordinates(rect,
                        (double)col * _levelBase.tileSize,
                        (double)row * _levelBase.tileSize);

                    switch ((TileType)_levelBase.maze[row, col])
                    {
                        case TileType.Wall:
                            rect.Fill = Brushes.DarkBlue;
                            break;
                        case TileType.Path:
                            rect.Fill = Brushes.AliceBlue;
                            break;
                        case TileType.Start:
                            rect.Fill = Brushes.LightSkyBlue;
                            break;
                        case TileType.End:
                            rect.Fill = Brushes.MediumSlateBlue;
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

                    canvas_1.Children.Add(rect);
                }
            }
        }

        private void MovePlayer(int deltaX, int deltaY)
        {
            double newX = Canvas.GetLeft(_levelBase.playerRect) + deltaX;
            double newY = Canvas.GetTop(_levelBase.playerRect) + deltaY;
            int newCol = (int)(newX / _levelBase.tileSize);
            int newRow = (int)(newY / _levelBase.tileSize);
            if (newRow >= 0 && newRow < _levelBase.maze.GetLength(0) && newCol >= 0 &&
                newCol < _levelBase.maze.GetLength(1))
            {
                if (_levelBase.maze[newRow, newCol] != (int)TileType.Wall)
                {
                    if (CheckExitCollision(newRow, newCol))
                    {
                        HandleExitCollision(newRow, newCol);
                    }
                    else if (CheckMovementValidity(newRow, newCol))
                    {
                        LevelBase.SetRectangleCoordinates(_levelBase.playerRect, newX, newY);
                    }
                }
            }
        }

        private bool CheckExitCollision(int newRow, int newCol)
        {
            return newRow == _levelBase.exitRow && newCol == _levelBase.exitCol;
        }

        private void HandleExitCollision(int newRow, int newCol)
        {
            _levelBase.timer.Stop();
            _levelBase.timeTaken = _levelBase.timeElapsed;
            CustomMessageBox messageBox = new CustomMessageBox("");
            Record record = new Record
            {
                UserId = User.CurrentUser.UserId,
                UserName = User.CurrentUser.UserName,
                Level = 2,
                Time = _levelBase.timeTaken + " сек"
            };
            using (var db = new DataBaseContext())
            {
                db.Records.Add(record);
                db.SaveChanges();
            }

            messageBox.messageBoxText.Text = $"Вітаємо, ви виграли! Витрачено часу: {_levelBase.timeTaken} секунд.";
            messageBox.ShowDialog();
            Close();
        }

        private bool CheckMovementValidity(int newRow, int newCol)
        {
            return (Math.Abs(newRow - (int)(Canvas.GetTop(_levelBase.playerRect) / _levelBase.tileSize)) == 1 &&
                    newCol == (int)(Canvas.GetLeft(_levelBase.playerRect) / _levelBase.tileSize) &&
                    _levelBase.maze[newRow, newCol] != (int)TileType.Wall)
                   ||
                   (Math.Abs(newCol - (int)(Canvas.GetLeft(_levelBase.playerRect) / _levelBase.tileSize)) == 1 &&
                    newRow == (int)(Canvas.GetTop(_levelBase.playerRect) / _levelBase.tileSize) &&
                    _levelBase.maze[newRow, newCol] != (int)TileType.Wall);
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
            _levelBase.timer.Stop();
            state = new PausedState(this);
            MessageBoxResult result = MessageBox.Show("Гра призупинена. Натисніть ОК, щоб продовжити гру.", "",
                MessageBoxButton.OK);
            if (result == MessageBoxResult.OK)
            {
                ResumeGame();
            }
        }

        private void ResumeGame()
        {
            state = new PlayingState(this);
            _levelBase.timer.Start();
        }

        private void RestartGame()
        {
            double x = _levelBase.entranceCol * _levelBase.tileSize;
            double y = _levelBase.entranceRow * _levelBase.tileSize;
            LevelBase.SetRectangleCoordinates(_levelBase.playerRect, x, y);
            GenerateMaze();
            DrawMaze();
            DrawPlayer();
            {
                _levelBase.timeElapsed = 0;
                _levelBase.timeTaken = 0;
                _levelBase.timer.Start();
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
            // Передача фабрики користувачів у конструктор Labyrinth
            NavigateToPage(new Labyrinth(userFactory));
        }

        private void Image_MouseDown_11(object sender, MouseButtonEventArgs e)
        {
            NavigateToPage(new Levels());
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