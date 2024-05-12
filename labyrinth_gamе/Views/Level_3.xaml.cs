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
        public enum TileType2 { Wall, Path, Start, End, Diamond }
        private MazeGenerator mazeGenerator;
        private MazeRenderer mazeRenderer;
        private Rectangle playerRect;
        private readonly int tileSize = 32;
        private Dictionary<Key, ICommand> _commands;
        private DispatcherTimer timer;
        private int timeElapsed;
        private bool isPaused = false;
        private int timeTaken;

        public Level_3()
        {
            InitializeComponent();
            _commands = new Dictionary<Key, ICommand>
            {
                { Key.Left, new MoveLeftCommand(new MovePlayer(this)) },
                { Key.Right, new MoveRightCommand(new MovePlayer(this)) },
                { Key.Up, new MoveUpCommand(new MovePlayer(this)) },
                { Key.Down, new MoveDownCommand(new MovePlayer(this)) },
                { Key.A, new MoveLeftCommand(new MovePlayer(this)) },
                { Key.D, new MoveRightCommand(new MovePlayer(this)) },
                { Key.W, new MoveUpCommand(new MovePlayer(this)) },
                { Key.S, new MoveDownCommand(new MovePlayer(this)) }
            };
            mazeGenerator = new MazeGenerator(19, 39);
            mazeRenderer = new MazeRenderer(canvas_1, mazeGenerator.Maze, tileSize);
            mazeRenderer.DrawMaze();
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

        private void DrawPlayer()
        {
            playerRect = new Rectangle();
            playerRect.Width = tileSize;
            playerRect.Height = tileSize;
            playerRect.Fill = Brushes.MediumVioletRed;
            int startRow = mazeGenerator.Maze.GetLength(0) / 2;
            int startCol = 0;
            playerRect.SetValue(Canvas.LeftProperty, (double)startCol * tileSize);
            playerRect.SetValue(Canvas.TopProperty, (double)startRow * tileSize);
            Canvas.SetTop(playerRect, mazeGenerator.EntranceRow * tileSize);
            Canvas.SetLeft(playerRect, mazeGenerator.EntranceCol * tileSize);
            canvas_1.Children.Add(playerRect);
        }

        private class MazeGenerator
        {
            private readonly int[,] maze;
            private readonly int entranceRow;
            private readonly int entranceCol;
            private readonly int exitRow;
            private readonly int exitCol;
            private readonly Random rand = new Random();

            public int[,] Maze => maze;
            public int EntranceRow => entranceRow;
            public int EntranceCol => entranceCol;
            public int ExitRow => exitRow;
            public int ExitCol => exitCol;

            public MazeGenerator(int rows, int cols)
            {
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
        }
        private class MazeRenderer
        {
            private readonly Canvas canvas;
            private readonly int tileSize;
            private readonly int[,] maze;
            public MazeRenderer(Canvas canvas, int[,] maze, int tileSize)
            {
                this.canvas = canvas;
                this.maze = maze;
                this.tileSize = tileSize;
            }
            public void DrawMaze()
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
                                rect.Fill = Brushes.DarkMagenta;
                                break;
                            case TileType2.Path:
                                rect.Fill = Brushes.Plum;
                                break;
                            case TileType2.Start:
                                rect.Fill = Brushes.MediumPurple;
                                break;
                            case TileType2.End:
                                rect.Fill = Brushes.MediumVioletRed;
                                break;
                            default:
                                break;
                        }
                        if ((TileType2)maze[row, col] == TileType2.Start)
                        {
                            rect.Fill = Brushes.MediumPurple;
                        }
                        canvas.Children.Add(rect);
                    }
                }
            }
        }
        private class MovePlayer
        {
            public readonly Level_3 _level;
            public MovePlayer(Level_3 level)
            {
                _level = level;
            }
            public void Execute(int deltaX, int deltaY)
            {
                double newX = Canvas.GetLeft(_level.playerRect) + deltaX;
                double newY = Canvas.GetTop(_level.playerRect) + deltaY;

                int newCol = (int)(newX / _level.tileSize);
                int newRow = (int)(newY / _level.tileSize);
                if (newRow >= 0 && newRow < _level.mazeGenerator.Maze.GetLength(0) && newCol >= 0 && newCol < _level.mazeGenerator.Maze.GetLength(1))
                {
                    if (_level.mazeGenerator.Maze[newRow, newCol] != (int)TileType2.Wall)
                    {
                        if ((Math.Abs(newRow - (int)(Canvas.GetTop(_level.playerRect) / _level.tileSize)) == 1 && newCol == (int)(Canvas.GetLeft(_level.playerRect) / _level.tileSize) && _level.mazeGenerator.Maze[newRow, newCol] != (int)TileType2.Wall)
                            || (Math.Abs(newCol - (int)(Canvas.GetLeft(_level.playerRect) / _level.tileSize)) == 1 && newRow == (int)(Canvas.GetTop(_level.playerRect) / _level.tileSize) && _level.mazeGenerator.Maze[newRow, newCol] != (int)TileType2.Wall))
                        {
                            _level.playerRect.SetValue(Canvas.LeftProperty, newX);
                            _level.playerRect.SetValue(Canvas.TopProperty, newY);

                            if (newRow == _level.mazeGenerator.ExitRow && newCol == _level.mazeGenerator.ExitCol)
                            {
                                _level.timer.Stop();
                                _level.timeTaken = _level.timeElapsed;
                                CustomMessageBox messageBox = new CustomMessageBox("");
                                Record record = new Record
                                {
                                    UserId = User.CurrentUser.UserId,
                                    UserName = User.CurrentUser.UserName,
                                    Level = 3,
                                    Time = _level.timeTaken + " сек"
                                };
                                using (var db = new DataBaseContext())
                                {
                                    db.Records.Add(record);
                                    db.SaveChanges();
                                }
                                messageBox.messageBoxText.Text = $"Вітаємо, ви виграли! Витрачено часу: {_level.timeTaken} секунд.";
                                messageBox.ShowDialog();
                                _level.Close();
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
        private class MoveUpCommand : ICommand
        {
            private readonly MovePlayer _movePlayer;

            public MoveUpCommand(MovePlayer movePlayer)
            {
                _movePlayer = movePlayer;
            }

            public void Execute()
            {
                _movePlayer.Execute(0, -_movePlayer._level.tileSize);
            }
        }
        private class MoveDownCommand : ICommand
        {
            private readonly MovePlayer _movePlayer;

            public MoveDownCommand(MovePlayer movePlayer)
            {
                _movePlayer = movePlayer;
            }
            public void Execute()
            {
                _movePlayer.Execute(0, _movePlayer._level.tileSize);
            }
        }
        private class MoveLeftCommand : ICommand
        {
            private readonly MovePlayer _movePlayer;
            public MoveLeftCommand(MovePlayer movePlayer)
            {
                _movePlayer = movePlayer;
            }

            public void Execute()
            {
                _movePlayer.Execute(-_movePlayer._level.tileSize, 0);
            }
        }
        private class MoveRightCommand : ICommand
        {
            private readonly MovePlayer _movePlayer;

            public MoveRightCommand(MovePlayer movePlayer)
            {
                _movePlayer = movePlayer;
            }

            public void Execute()
            {
                _movePlayer.Execute(_movePlayer._level.tileSize, 0);
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_commands.ContainsKey(e.Key))
            {
                _commands[e.Key].Execute();
            }
        }
        private void Image_MouseDown_18(object sender, MouseButtonEventArgs e)
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

        private void Image_MouseDown_9(object sender, RoutedEventArgs e)
        {
            IUserFactory userFactory = new UserFactory();
            Frame frame = new Frame();
            Window mainWindow = new MainWindow();
            frame.Navigate(new Labyrinth(new UserFactory()));
            mainWindow.Content = frame;
            mainWindow.Show();
            this.Close();
        }

        private void Image_MouseDown_12(object sender, MouseButtonEventArgs e)
        {
            Frame frame = new Frame();
            Window mainWindow = new MainWindow();
            frame.Navigate(new Levels());
            mainWindow.Content = frame;
            mainWindow.Show();
            this.Close();
        }
    }
}