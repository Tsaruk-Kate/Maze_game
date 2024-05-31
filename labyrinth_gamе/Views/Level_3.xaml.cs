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
    public partial class Level_3 : Window
    {
        private LevelBase _levelBase;
        private Dictionary<Key, ICommand> _commands;
        private List<Diamond> diamonds;
        private int collectedDiamonds = 0;

        public Level_3()
        {
            _levelBase = new LevelBase()
            {
                tileSize = 32
            };
            InitializeComponent();
            _commands = new Dictionary<Key, ICommand>
            {
                { Key.Left, new MoveLeftCommand(this) },
                { Key.Right, new MoveRightCommand(this) },
                { Key.Up, new MoveUpCommand(this) },
                { Key.Down, new MoveDownCommand(this) },
                { Key.A, new MoveLeftCommand(this) },
                { Key.D, new MoveRightCommand(this) },
                { Key.W, new MoveUpCommand(this) },
                { Key.S, new MoveDownCommand(this) }
            };
            GenerateMaze();
            DrawMaze();
            DrawPlayer();
            _levelBase.timer = new DispatcherTimer();
            _levelBase.timer.Interval = TimeSpan.FromSeconds(1);
            _levelBase.timer.Tick += Timer_Tick;
            _levelBase.timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                _levelBase.timeElapsed++;
                int minutes = _levelBase.timeElapsed / 60;
                int seconds = _levelBase.timeElapsed % 60;
                TimeLabel.Content = $"Часу минуло: {minutes:D2}:{seconds:D2}";
            }
        }

        private void GenerateMaze()
        {
            int rows = 19;
            int cols = 39;
            _levelBase.maze = new int[rows, cols];
            this._levelBase.entranceRow = rows / 2;
            this._levelBase.entranceCol = 0;
            _levelBase.maze[this._levelBase.entranceRow, this._levelBase.entranceCol] = (int)TileType.Start;
            this._levelBase.exitRow = _levelBase.rand.Next(1, rows - 2);
            this._levelBase.exitCol = cols - 1;
            _levelBase.maze[this._levelBase.exitRow, this._levelBase.exitCol] = (int)TileType.End;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    _levelBase.maze[i, j] = (int)TileType.Wall;
                }
            }

            GenerateMazeRecursive(_levelBase.entranceRow, _levelBase.entranceCol, rows, cols);
            _levelBase.maze[this._levelBase.exitRow + 1, this._levelBase.exitCol] = (int)TileType.Path;
            diamonds = new List<Diamond>();
            int diamondCount = 0;
            while (diamondCount < 10)
            {
                int randRow = _levelBase.rand.Next(1, rows - 1);
                int randCol = _levelBase.rand.Next(1, cols - 1);
                if (_levelBase.maze[randRow, randCol] == (int)TileType.Path &&
                    diamonds.Find(d => d.Row == randRow && d.Col == randCol) == null)
                {
                    diamonds.Add(new Diamond { Row = randRow, Col = randCol });
                    diamondCount++;
                }
            }

            _levelBase.maze[this._levelBase.exitRow + 1, this._levelBase.exitCol] = (int)TileType.Path;
            _levelBase.maze[this._levelBase.entranceRow, this._levelBase.entranceCol] = (int)TileType.Start;
            _levelBase.maze[this._levelBase.exitRow, this._levelBase.exitCol] = (int)TileType.End;
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
            _levelBase.playerRect.Fill = Brushes.MediumVioletRed;
            int startRow = _levelBase.maze.GetLength(0) / 2;
            int startCol = 0;
            LevelBase.SetRectangleCoordinates(_levelBase.playerRect,
                (double)startCol * _levelBase.tileSize,
                (double)startRow * _levelBase.tileSize);
            Canvas.SetTop(_levelBase.playerRect, _levelBase.entranceRow * _levelBase.tileSize);
            Canvas.SetLeft(_levelBase.playerRect, _levelBase.entranceCol * _levelBase.tileSize);
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
                            rect.Fill = Brushes.DarkMagenta;
                            break;
                        case TileType.Path:
                            rect.Fill = Brushes.Plum;
                            break;
                        case TileType.Start:
                            rect.Fill = Brushes.MediumPurple;
                            break;
                        case TileType.End:
                            rect.Fill = Brushes.MediumVioletRed;
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
                        rect.Fill = Brushes.MediumPurple;
                    }

                    canvas_1.Children.Add(rect);
                }
            }

            foreach (var diamond in diamonds)
            {
                if (!diamond.IsCollected)
                {
                    Image diamondImage = new Image();
                    diamondImage.Width = _levelBase.tileSize;
                    diamondImage.Height = _levelBase.tileSize;
                    var uri = new Uri("pack://application:,,,/Views/diamond.png", UriKind.Absolute);
                    diamondImage.Source = new BitmapImage(uri);

                    Canvas.SetLeft(diamondImage, diamond.Col * _levelBase.tileSize);
                    Canvas.SetTop(diamondImage, diamond.Row * _levelBase.tileSize);

                    canvas_1.Children.Add(diamondImage);
                    diamond.Image = diamondImage;
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
                    if ((Math.Abs(newRow - (int)(Canvas.GetTop(_levelBase.playerRect) / _levelBase.tileSize)) == 1 &&
                         newCol == (int)(Canvas.GetLeft(_levelBase.playerRect) / _levelBase.tileSize) &&
                         _levelBase.maze[newRow, newCol] != (int)Views.TileType.Wall)
                        || (Math.Abs(newCol - (int)(Canvas.GetLeft(_levelBase.playerRect) / _levelBase.tileSize)) ==
                            1 && newRow == (int)(Canvas.GetTop(_levelBase.playerRect) / _levelBase.tileSize) &&
                            _levelBase.maze[newRow, newCol] != (int)Views.TileType.Wall))
                    {
                        LevelBase.SetRectangleCoordinates(_levelBase.playerRect, newX, newY);

                        foreach (Diamond diamond in diamonds)
                        {
                            if (newRow == diamond.Row && newCol == diamond.Col)
                            {
                                _levelBase.maze[diamond.Row, diamond.Col] = (int)TileType.Path;
                                diamonds.Remove(diamond);
                                collectedDiamonds++;
                                diamondsTextBlock.Text = "Діаманти: " + collectedDiamonds;
                                canvas_1.Children.Remove(diamond.Image);

                                break;
                            }
                        }

                        if (newRow == _levelBase.exitRow && newCol == _levelBase.exitCol)
                        {
                            if (diamonds.Count == 0)
                            {
                                _levelBase.timer.Stop();
                                _levelBase.timeTaken = _levelBase.timeElapsed;
                                CustomMessageBox messageBox = new CustomMessageBox("");
                                Record record = new Record
                                {
                                    UserId = User.CurrentUser.UserId,
                                    UserName = User.CurrentUser.UserName,
                                    Level = 3,
                                    Time = _levelBase.timeTaken + " сек"
                                };
                                using (var db = new DataBaseContext())
                                {
                                    db.Records.Add(record);
                                    db.SaveChanges();
                                }

                                messageBox.messageBoxText.Text =
                                    $"Вітаємо, ви виграли! Витрачено часу: {_levelBase.timeTaken} секунд.";
                                messageBox.ShowDialog();
                                Close();
                            }
                            else
                            {
                                MessageBox.Show("Ви повинні зібрати всі діаманти до того, як дійдете до виходу.",
                                    "Не завершено");
                            }
                        }
                    }
                }
            }
        }

        public interface ICommand
        {
            void Execute();
        }

        public class MoveUpCommand : ICommand
        {
            private readonly Level_3 _level;

            public MoveUpCommand(Level_3 level)
            {
                _level = level;
            }

            public void Execute()
            {
                _level.MovePlayer(0, -_level._levelBase.tileSize);
            }
        }

        public class MoveDownCommand : ICommand
        {
            private readonly Level_3 _level;

            public MoveDownCommand(Level_3 level)
            {
                _level = level;
            }

            public void Execute()
            {
                _level.MovePlayer(0, _level._levelBase.tileSize);
            }
        }

        public class MoveLeftCommand : ICommand
        {
            private readonly Level_3 _level;

            public MoveLeftCommand(Level_3 level)
            {
                _level = level;
            }

            public void Execute()
            {
                _level.MovePlayer(-_level._levelBase.tileSize, 0);
            }
        }

        public class MoveRightCommand : ICommand
        {
            private readonly Level_3 _level;

            public MoveRightCommand(Level_3 level)
            {
                _level = level;
            }

            public void Execute()
            {
                _level.MovePlayer(_level._levelBase.tileSize, 0);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_commands.ContainsKey(e.Key))
            {
                _commands[e.Key].Execute();
            }
        }

        private bool isPaused = false;

        private void Image_MouseDown_18(object sender, MouseButtonEventArgs e)
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
            double x = _levelBase.entranceCol * _levelBase.tileSize;
            double y = _levelBase.entranceRow * _levelBase.tileSize;
            LevelBase.SetRectangleCoordinates(_levelBase.playerRect, x, y);
            collectedDiamonds = 0;
            diamondsTextBlock.Text = "Діаманти: " + collectedDiamonds;
            GenerateMaze();
            DrawMaze();
            DrawPlayer();
            if (!_levelBase.timer.IsEnabled)
            {
                _levelBase.timeElapsed = 0;
                _levelBase.timeTaken = 0;
                _levelBase.timer.Start();
            }
        }

        private void Image_MouseDown_17(object sender, MouseButtonEventArgs e)
        {
            RestartGame();
        }

        private void Image_MouseDown_9(object sender, RoutedEventArgs e)
        {
            // Створення екземпляру фабрики користувачів
            IUserFactory userFactory = new UserFactory();
            // Передача фабрики користувачів у конструктор Labyrinth
            NavigateToPage(new Labyrinth(userFactory));
        }

        private void Image_MouseDown_12(object sender, MouseButtonEventArgs e)
        {
            NavigateToPage(new Levels());
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