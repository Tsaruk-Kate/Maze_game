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
        private Dictionary<Key, ICommand> _commands;
        private List<Diamond> diamonds;
        private int collectedDiamonds = 0;

        public Level_3()
        {
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
            int rows = 19;
            int cols = 39;
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
            diamonds = new List<Diamond>();
            int diamondCount = 0;
            while (diamondCount < 10)
            {
                int randRow = rand.Next(1, rows - 1);
                int randCol = rand.Next(1, cols - 1);
                if (maze[randRow, randCol] == (int)TileType.Path &&
                    diamonds.Find(d => d.Row == randRow && d.Col == randCol) == null)
                {
                    diamonds.Add(new Diamond { Row = randRow, Col = randCol });
                    diamondCount++;
                }
            }
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
            playerRect.Fill = Brushes.MediumVioletRed;
            int startRow = maze.GetLength(0) / 2;
            int startCol = 0;
            playerRect.SetValue(Canvas.LeftProperty, (double)startCol * tileSize);
            playerRect.SetValue(Canvas.TopProperty, (double)startRow * tileSize);
            Canvas.SetTop(playerRect, entranceRow * tileSize);
            Canvas.SetLeft(playerRect, entranceCol * tileSize);
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

                    switch ((TileType)maze[row, col])
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
                    diamondImage.Width = tileSize;
                    diamondImage.Height = tileSize;
                    var uri = new Uri("pack://application:,,,/Views/diamond.png", UriKind.Absolute);
                    diamondImage.Source = new BitmapImage(uri);

                    Canvas.SetLeft(diamondImage, diamond.Col * tileSize);
                    Canvas.SetTop(diamondImage, diamond.Row * tileSize);

                    canvas_1.Children.Add(diamondImage);
                    diamond.Image = diamondImage;
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
                    if ((Math.Abs(newRow - (int)(Canvas.GetTop(playerRect) / tileSize)) == 1 && newCol == (int)(Canvas.GetLeft(playerRect) / tileSize) && maze[newRow, newCol] != (int)Views.TileType.Wall)
                        || (Math.Abs(newCol - (int)(Canvas.GetLeft(playerRect) / tileSize)) == 1 && newRow == (int)(Canvas.GetTop(playerRect) / tileSize) && maze[newRow, newCol] != (int)Views.TileType.Wall))
                    {
                        playerRect.SetValue(Canvas.LeftProperty, newX);
                        playerRect.SetValue(Canvas.TopProperty, newY);

                        foreach (Diamond diamond in diamonds)
                        {
                            if (newRow == diamond.Row && newCol == diamond.Col)
                            {
                                maze[diamond.Row, diamond.Col] = (int)TileType.Path;
                                diamonds.Remove(diamond);
                                collectedDiamonds++;
                                diamondsTextBlock.Text = "Діаманти: " + collectedDiamonds;
                                canvas_1.Children.Remove(diamond.Image);

                                break;
                            }
                        }
                        if (newRow == exitRow && newCol == exitCol)
                        {
                            if (diamonds.Count == 0)
                            {
                                timer.Stop();
                                timeTaken = timeElapsed;
                                CustomMessageBox messageBox = new CustomMessageBox("");
                                Record record = new Record
                                {
                                    UserId = User.CurrentUser.UserId,
                                    UserName = User.CurrentUser.UserName,
                                    Level = 3,
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
                            else
                            {
                                MessageBox.Show("Ви повинні зібрати всі діаманти до того, як дійдете до виходу.", "Не завершено");
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
                _level.MovePlayer(0, -_level.tileSize);
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
                _level.MovePlayer(0, _level.tileSize);
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
                _level.MovePlayer(-_level.tileSize, 0);
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
                _level.MovePlayer(_level.tileSize, 0);
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
            timer.Stop();
            MessageBox.Show("Гра призупинена. Натисніть ОК, щоб продовжити гру.");
            isPaused = false;
            timer.Start();
        }
        private void RestartGame()
        {

            double x = entranceCol * tileSize;
            double y = entranceRow * tileSize;
            playerRect.SetValue(Canvas.LeftProperty, x);
            playerRect.SetValue(Canvas.TopProperty, y);
            collectedDiamonds = 0;
            diamondsTextBlock.Text = "Діаманти: " + collectedDiamonds;
            GenerateMaze();
            DrawMaze();
            DrawPlayer();
            if (!timer.IsEnabled)
            {
                timeElapsed = 0;
                timeTaken = 0;
                timer.Start();
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
            Frame frame = new Frame();
            Window mainWindow = new MainWindow();
            // Передача фабрики користувачів у конструктор Labyrinth
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