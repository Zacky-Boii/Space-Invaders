using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Space_Invaders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool gameOver = false;
        private Image gameOverScreen = new Image();
        private string gameOverScreenPath = "Images\\gameover.png";
        private Button restart = new Button();

        private Button nextLevelButton = new Button();
        private Image nextLevelScreen = new Image();
        private string nextLevelScreenPath = "Images\\levelup.png";

        private TextBox scoreDisplay = new TextBox();
        private TextBox levelDisplay = new TextBox();
        private int levelNumber = 1;

        Enemy1 exampleOne;
        Enemy2 exampleTwo;
        Enemy3 exampleThree;
        Image playerShipExample;

        private Button mainMenuButton = new Button();
        TextBox mainTitle = new TextBox();
        private Button play = new Button();

        private Button hiScores = new Button();
        private string hiScoreFileName = "Hi-Scores/ScoreList.txt";
        private List<string> scoresList = new List<string>();
        private TextBox hiScoreTextBox;

        private bool isGameLoopRunning = false;

        private Button rules = new Button();
        Image rulesImage;
        private string rulesImagePath = "Images/rules.png";
        private Button removeRules;
        private Canvas rulesCanvas = new Canvas();

        private Canvas maincanvas;
        private Image playerShip = new Image();
        private Image laser;

        private string playerShipPath = "Images/Player.png";
        private string laserPath = "Images/laser.png";

        private double playerShipX = 0;
        private double playerShipY = 0;
        private double speed = 300; //speed of game

        private int laserDelay;
        private int laserDelayAmount = 1; // in frames
        private int renderDelay;
        private int renderDelayAmount = 1; // in frames

        HashSet<Key> keysPressed = new HashSet<Key>();

        List<Laser> activeLasers = new List<Laser>();

        List<Enemy1> enemy1s = new List<Enemy1>();
        private int enemy1Count = 30;

        List<Enemy2> enemy2s = new List<Enemy2>();
        private int enemy2Count = 30;

        List<Enemy3> enemy3s = new List<Enemy3>();
        private int enemy3Count = 15;

        private int score;
        private int finalScore;

        public static int TitleHeight() => 30;

        private TextBox nameInput;
        private Button submitName;
        private string name = "";

        public MainWindow()
        {
            InitializeComponent();

            this.KeyDown += Key_Down;
            this.KeyUp += Key_Up;

            SetupCanvas();
            MainMenu();
        }

        private void MainMenu()
        {
            mainTitle.Text = "SPACE INVADERS";
            mainTitle.Width = 440;
            mainTitle.Background = Brushes.Black;
            mainTitle.BorderBrush = Brushes.Black;
            mainTitle.Foreground = Brushes.White;
            mainTitle.FontSize = 50;
            mainTitle.FontWeight = FontWeights.ExtraBold;
            mainTitle.IsReadOnly = true;
            mainTitle.IsHitTestVisible = false;
            Canvas.SetLeft(mainTitle, (maincanvas.Width / 2) - (mainTitle.Width / 2));
            maincanvas.Children.Add(mainTitle);

            //these are for visuals
            exampleOne = new Enemy1(maincanvas);
            exampleOne.enemy.Height = 50;
            exampleOne.enemy.Width = 50; 
            exampleTwo = new Enemy2(maincanvas);
            exampleTwo.enemy.Height = 50;
            exampleTwo.enemy.Width = 50;
            exampleThree = new Enemy3(maincanvas);
            exampleThree.enemy.Height = 50;
            exampleThree.enemy.Width = 50;
            playerShipExample = new Image();
            playerShipExample.Source = new BitmapImage(new Uri(playerShipPath, UriKind.Relative));
            playerShipExample.Height = 60;
            playerShipExample.Width = 60;

            //enemy 1 - left
            Canvas.SetLeft(exampleOne.enemy, 125);
            Canvas.SetTop(exampleOne.enemy, 100);
            //enemy2 - right
            Canvas.SetLeft(exampleTwo.enemy, 425);
            Canvas.SetTop(exampleTwo.enemy, 100);
            //enemy3 - middle
            Canvas.SetLeft(exampleThree.enemy, 275);
            Canvas.SetTop(exampleThree.enemy, 100);
            //player ship - middle
            Canvas.SetLeft(playerShipExample, 275);
            Canvas.SetTop(playerShipExample, 300);
            maincanvas.Children.Add(playerShipExample);

            Enemies.enemyCount = 0;

            play.Content = "Play!";
            play.Height = 50;
            play.Width = 200;
            Canvas.SetLeft(play, (maincanvas.Width / 2) - (play.Width / 2));
            Canvas.SetTop(play, (2*maincanvas.Height/3) - (play.Height / 2));
            maincanvas.Children.Add(play);

            hiScores.Content = "Hi-Scores";
            hiScores.Height = 50;
            hiScores.Width = 200;
            Canvas.SetLeft(hiScores, (maincanvas.Width / 2) - (hiScores.Width / 2));
            Canvas.SetTop(hiScores, (2 * maincanvas.Height / 3 + (play.Height + 20)) - (hiScores.Height / 2));
            maincanvas.Children.Add(hiScores);

            rules.Content = "Rules";
            rules.Height = 50;
            rules.Width = 200;
            Canvas.SetLeft(rules, (maincanvas.Width / 2) - (rules.Width / 2));
            Canvas.SetTop(rules, (2 * maincanvas.Height / 3 + (play.Height + hiScores.Height + 40)) - (rules.Height / 2));
            maincanvas.Children.Add(rules);

            play.Click += playButton_Click;
            hiScores.Click += hiScoresButton_Click;
            rules.Click += rulesButton_Click;
        }
        private void RemoveMainMenuItems()
        {
            maincanvas.Children.Remove(mainTitle);
            maincanvas.Children.Remove(exampleOne.enemy);
            maincanvas.Children.Remove(exampleTwo.enemy);
            maincanvas.Children.Remove(exampleThree.enemy);
            maincanvas.Children.Remove(playerShipExample);
            maincanvas.Children.Remove(play);
            maincanvas.Children.Remove(hiScores);
            maincanvas.Children.Remove(rules);
        }

        private void mainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnToMainMenu();
        }
        private void ReturnToMainMenu()
        {
            mainMenuButton.Click -= mainMenuButton_Click;
            if (isGameLoopRunning)
            {
                CompositionTarget.Rendering -= GameLoop_Call;
                isGameLoopRunning = false;
            }

            maincanvas.Children.Clear();

            ClearScreen();

            MainMenu();
        }

        private void AddGameInfo()
        {
            scoreDisplay = new TextBox();
            scoreDisplay.Text = "Score: " + score;
            scoreDisplay.Height = 16;
            scoreDisplay.Width = maincanvas.Width;
            scoreDisplay.Background = Brushes.Black;
            scoreDisplay.BorderBrush = Brushes.Black;
            scoreDisplay.Foreground = Brushes.White;
            scoreDisplay.FontWeight = FontWeights.Bold;
            scoreDisplay.IsReadOnly = true;
            scoreDisplay.IsHitTestVisible = false;
            maincanvas.Children.Add(scoreDisplay);

            TextBox title = new TextBox();
            title.Text = "SPACE INVADERS";
            title.Height = TitleHeight();
            title.Width = 200;
            title.Background = Brushes.Black;
            title.BorderBrush = Brushes.Black;
            title.Foreground = Brushes.White;
            title.FontSize = 22.5;
            title.FontWeight = FontWeights.ExtraBold;
            title.IsReadOnly = true;
            title.IsHitTestVisible = false;
            Canvas.SetLeft(title, (maincanvas.Width / 2) - (title.Width / 2));
            maincanvas.Children.Add(title);

            levelDisplay = new TextBox();
            levelDisplay.Text = "Level: " + levelNumber;
            levelDisplay.Height = 18;
            levelDisplay.Width = 60;
            levelDisplay.Background = Brushes.Black;
            levelDisplay.BorderBrush = Brushes.Black;
            levelDisplay.Foreground = Brushes.White;
            levelDisplay.FontWeight = FontWeights.Bold;
            levelDisplay.IsReadOnly = true;
            levelDisplay.IsHitTestVisible = false;
            Canvas.SetLeft(levelDisplay, maincanvas.Width - levelDisplay.Width);
            maincanvas.Children.Add(levelDisplay);
        }

        DateTime lastUpdate = DateTime.Now;
        private void GameLoop()
        {
            levelDisplay.Text = "Level: " + levelNumber;

            //use delta time so it doesnt matter what monitor refresh rate is
            var now = DateTime.Now;
            double deltaTime = (now - lastUpdate).TotalSeconds;
            lastUpdate = now;
            if (laserDelay > 0) laserDelay -= 1;
            if (renderDelay > 0) renderDelay -= 1;

            if ((keysPressed.Contains(Key.W) || keysPressed.Contains(Key.Up)) && playerShipY + (speed * deltaTime) + playerShip.Height < 600)
            {
                Canvas.SetBottom(playerShip, playerShipY += (speed * deltaTime));
            }
            else if ((keysPressed.Contains(Key.S) || keysPressed.Contains(Key.Down)) && playerShipY - (speed * deltaTime) > 0)
            {
                Canvas.SetBottom(playerShip, playerShipY -= (speed * deltaTime));
            }
            if ((keysPressed.Contains(Key.A) || keysPressed.Contains(Key.Left)) && playerShipX - (speed * deltaTime) > 0)
            {
                Canvas.SetLeft(playerShip, playerShipX -= (speed * deltaTime));
            }
            else if ((keysPressed.Contains(Key.D) || keysPressed.Contains(Key.Right)) && playerShipX + (speed * deltaTime) + playerShip.Width < 600)
            {
                Canvas.SetLeft(playerShip, playerShipX += (speed * deltaTime));
            }
            if (laserDelay == 0 && keysPressed.Contains(Key.Space))
            {
                FireLaser();
                laserDelay = laserDelayAmount;
            }

            RemoveLaser(); // remove it when laser goes off screen

            CheckIfAtBottom();

            DetectHit();

            if (renderDelay == 0)
            {
                DrawEnemies(deltaTime);
                renderDelay = renderDelayAmount;
            }

            CheckGameState();
            Debug.WriteLine(name);
        }
        private void GameLoop_Call(object sender, EventArgs e)
        {
           GameLoop();
        }
        private void CheckGameState()
        {
            gameOver = false;
            //check if any ships are touching the player
            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                if (enemy1s[i].TouchingPlayer(playerShipX, playerShipY, playerShip))
                {
                    gameOver = true;
                }
            }

            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                if (enemy2s[i].TouchingPlayer(playerShipX, playerShipY, playerShip))
                {
                    gameOver = true;
                }
            }

            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                if (enemy3s[i].TouchingPlayer(playerShipX, playerShipY, playerShip))
                {
                    gameOver = true;
                }
            }

            //check if all enemies are dead
            if (enemy1s.Count == 0 && enemy2s.Count == 0 && enemy3s.Count == 0)
            {
                NextLevel();
            }

            if (gameOver)
            {
                GameOver();
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            InitialiseGame();
        }
        private void InitialiseGame()
        {
            play.Click -= playButton_Click;

            Enemies.enemyCount = 0;

            RemoveMainMenuItems();
            ClearScreen();

            InitialiseSprites();
            AddGameInfo();



            Canvas.SetLeft(playerShip, playerShipX);
            Canvas.SetBottom(playerShip, 0);

            //similar to vsync so it calls every time monitor refreshes

            if (!isGameLoopRunning)
            {
                CompositionTarget.Rendering += GameLoop_Call;
                isGameLoopRunning = true;
            }
        }

        private void hiScoresButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayHiScores();
        }
        private void DisplayHiScores()
        {
            hiScores.Click -= hiScoresButton_Click;

            RemoveMainMenuItems();

            ReadFromHiScores();

            List<KeyValuePair<string, int>> scores = new List<KeyValuePair<string, int>>();

            for (int i = 0; i < scoresList.Count; i++)
            {
                string[] split = scoresList[i].Split(' ');
                //0 is name and 1 is score
                string name = split[0];
                if (split.Length == 1) break;
                int score = int.Parse(split[1]);
                scores.Add(new KeyValuePair<string, int>(name, score));
            }

            scores.Sort(CompareScoreDescending);

            string finalOutput = "HI SCORES:\n\n";
            for (int i = 0; i < scores.Count; i++)
            {
                string name = scores[i].Key;
                int score = scores[i].Value;

                finalOutput += name + " " + score + "\n";
            }



            hiScoreTextBox = new TextBox();
            hiScoreTextBox.Width = maincanvas.Width;
            hiScoreTextBox.Text = finalOutput;
            hiScoreTextBox.IsReadOnly = true;
            hiScoreTextBox.HorizontalAlignment = HorizontalAlignment.Center;
            hiScoreTextBox.Background = Brushes.Black;
            hiScoreTextBox.BorderBrush = Brushes.Black;
            hiScoreTextBox.Foreground = Brushes.White;
            hiScoreTextBox.FontSize = 50;
            hiScoreTextBox.IsHitTestVisible = false;
            maincanvas.Children.Add(hiScoreTextBox);

            mainMenuButton.Content = "Main Menu";
            mainMenuButton.Height = 50;
            mainMenuButton.Width = 200;
            Canvas.SetRight(mainMenuButton, 0);
            Canvas.SetTop(mainMenuButton, 0);

            maincanvas.Children.Add(mainMenuButton);

            mainMenuButton.Click += mainMenuButton_Click;
        }

        private int CompareScoreDescending(KeyValuePair<string, int> a, KeyValuePair<string, int> b)
        {
            return b.Value.CompareTo(a.Value);
        }

        private void WriteToHiScores(int finalScore)
        {
            string playerName = name;
            hiScoreFileName = "Hi-Scores/ScoreList.txt";
            string relativePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, hiScoreFileName);
            string toWrite = name + " " + finalScore;

            File.AppendAllText(relativePath, toWrite + Environment.NewLine);
        }
        private void ReadFromHiScores()
        {
            string relativePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, hiScoreFileName);

            StreamReader sr = new StreamReader(relativePath);
           
            string? line = sr.ReadLine();

            while (line != null)
            {
                scoresList.Add(line);
                line = sr.ReadLine();
            }
            
            sr.Close();
        }
        private void GetName()
        {
            string defaultText = "Enter name to save score...";
            nameInput = new TextBox();
            nameInput.Height = 38;
            nameInput.Width = 370;
            nameInput.Text = defaultText;
            nameInput.FontSize = 30;
            nameInput.Foreground = Brushes.Gray;
            nameInput.TextAlignment = TextAlignment.Center;
            Canvas.SetTop(nameInput, 5 * maincanvas.Width / 10);
            Canvas.SetLeft(nameInput, maincanvas.Width / 2 - nameInput.Width / 2);
            maincanvas.Children.Add(nameInput);

            nameInput.GotFocus += (s, e) =>
            {
                if (nameInput.Text == defaultText)
                {
                    nameInput.Text = "";
                    nameInput.Foreground = Brushes.Black;
                }
            };

            nameInput.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(nameInput.Text))
                {
                    nameInput.Text = defaultText;
                    nameInput.Foreground = Brushes.Gray;
                }
            };

            submitName = new Button();
            submitName.Height = 38;
            submitName.Width = 150;
            submitName.Content = "Submit";
            submitName.FontSize = 24;
            Canvas.SetLeft(submitName, maincanvas.Width/2 - submitName.Width/2);
            Canvas.SetTop(submitName, 6 * maincanvas.Width / 10);
            maincanvas.Children.Add(submitName);

            submitName.Click += (s, e) =>
            {
                if (nameInput.Text != defaultText)
                {
                    name = nameInput.Text;
                    defaultText = "Score submitted!";
                    WriteToHiScores(finalScore);
                    nameInput.Foreground = Brushes.Gray;
                    nameInput.Text = defaultText;
                    Canvas.SetTop(nameInput, 5 * maincanvas.Width / 10);
                    Canvas.SetLeft(nameInput, maincanvas.Width / 2 - nameInput.Width / 2);
                }        
            };
        }

        private void rulesButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayRules();
        }
        private void DisplayRules()
        {
            rules.Click -= rulesButton_Click;
            if (isGameLoopRunning)
            {
                CompositionTarget.Rendering -= GameLoop_Call;
                isGameLoopRunning = false;
            }

            InitialiseRulesCanvas();
            this.Content = rulesCanvas; 

            rulesImage = new Image();
            rulesImage.Source = new BitmapImage(new Uri(rulesImagePath, UriKind.Relative));
            rulesImage.Height = maincanvas.Height;
            rulesImage.Width = maincanvas.Width;
            rulesCanvas.Children.Add(rulesImage);

            removeRules = new Button();
            removeRules.Content = "Main Menu";
            removeRules.Height = 50;
            removeRules.Width = 200;
            Canvas.SetLeft(removeRules, 375);
            Canvas.SetTop(removeRules, 25);
            rulesCanvas.Children.Add(removeRules);

            //these are for visuals
            Image exampleOneRules = new Image();
            exampleOneRules.Height = 50;
            exampleOneRules.Width = 50;
            Image exampleTwoRules = new Image();
            exampleTwoRules.Height = 50;
            exampleTwoRules.Width = 50;
            Image exampleThreeRules = new Image();
            exampleThreeRules.Height = 50;
            exampleThreeRules.Width = 50;
            exampleOneRules.Source = new BitmapImage(new Uri("Images/Enemy1.png", UriKind.Relative));
            exampleTwoRules.Source = new BitmapImage(new Uri("Images/Enemy2.png", UriKind.Relative));
            exampleThreeRules.Source = new BitmapImage(new Uri("Images/Enemy3.png", UriKind.Relative));

            //enemy 1 - left
            Canvas.SetLeft(exampleOneRules, 65);
            Canvas.SetTop(exampleOneRules, 225);
            //enemy2 - middle
            Canvas.SetLeft(exampleTwoRules, 283);
            Canvas.SetTop(exampleTwoRules, 225);
            //enemy3 - right
            Canvas.SetLeft(exampleThreeRules, 465);
            Canvas.SetTop(exampleThreeRules, 225);

            rulesCanvas.Children.Add(exampleOneRules);
            rulesCanvas.Children.Add(exampleTwoRules);
            rulesCanvas.Children.Add(exampleThreeRules);

            removeRules.Click += removeRulesButton_Click;
        }
        private void InitialiseRulesCanvas()
        {
            rulesCanvas.Height = 600;
            rulesCanvas.Width = 600;
            rulesCanvas.Background = Brushes.Black;
        }
        private void removeRulesButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveRules();
        }
        private void RemoveRules()
        {
            removeRules.Click -= removeRulesButton_Click;

            rulesCanvas.Children.Clear();

            this.Content = maincanvas;
            rules.Click += rulesButton_Click;
        }

        private void CheckIfAtBottom()
        {
            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                if (enemy1s[i].enemyY < 0)
                {
                    score -= enemy1s[i].AddPoints(levelNumber);
                    scoreDisplay.Text = "Score: " + score;
                    maincanvas.Children.Remove(enemy1s[i].enemy);
                    enemy1s.Remove(enemy1s[i]);
                }
            }

            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                if (enemy2s[i].enemyY < 0)
                {
                    score -= enemy2s[i].AddPoints(levelNumber);
                    scoreDisplay.Text = "Score: " + score;
                    maincanvas.Children.Remove(enemy2s[i].enemy);
                    enemy2s.Remove(enemy2s[i]);
                }
            }

            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                if (enemy3s[i].enemyY < 0)
                {
                    score -= enemy3s[i].AddPoints(levelNumber);
                    scoreDisplay.Text = "Score: " + score;
                    maincanvas.Children.Remove(enemy3s[i].enemy);
                    enemy3s.Remove(enemy3s[i]);
                }
            }
        }

        private void NextLevel()
        {
            if (isGameLoopRunning)
            {
                CompositionTarget.Rendering -= GameLoop_Call;
                isGameLoopRunning = false;
            }

            ClearScreen();

            nextLevelScreen.Source = new BitmapImage(new Uri(nextLevelScreenPath, UriKind.Relative));
            nextLevelScreen.Height = 300;
            nextLevelScreen.Width = 600;
            maincanvas.Children.Add(nextLevelScreen);

            nextLevelButton.Content = "Next Level!";
            nextLevelButton.Height = 50;
            nextLevelButton.Width = 200;
            Canvas.SetLeft(nextLevelButton, (maincanvas.Width / 2) - (nextLevelButton.Width / 2));
            Canvas.SetTop(nextLevelButton, (maincanvas.Height / 2) - (nextLevelButton.Height / 2));

            maincanvas.Children.Add(nextLevelButton);

            nextLevelButton.Click += NextLevelButton_Click;
        }
        private void NextLevelButton_Click(object sender, RoutedEventArgs e)
        {
            InitialiseNextLevel();
        }
        private void InitialiseNextLevel()
        {
            nextLevelButton.Click -= NextLevelButton_Click;

            levelNumber++;

            ClearScreen();

            maincanvas.Children.Remove(nextLevelScreen);
            maincanvas.Children.Remove(nextLevelButton);

            Enemies.enemyCount = 0;
            playerShipX = ((maincanvas.Width / 2) - (playerShip.Width / 2));
            playerShipY = 0;

            InitialiseSprites();

            if (!isGameLoopRunning)
            {
                CompositionTarget.Rendering += GameLoop_Call;
                isGameLoopRunning = true;
            }
        }

        private void GameOver()
        {
            if (isGameLoopRunning)
            {
                CompositionTarget.Rendering -= GameLoop_Call;
                isGameLoopRunning = false;
            }

            ClearScreen();

            Enemies.enemyCount = 0;
            finalScore = score;
            score = 0;
            levelNumber = 1;

            gameOverScreen.Source = new BitmapImage(new Uri(gameOverScreenPath, UriKind.Relative));
            gameOverScreen.Height = 150;
            gameOverScreen.Width = 300;
            maincanvas.Children.Add(gameOverScreen);
            Canvas.SetTop(gameOverScreen, (maincanvas.Height/3) - (gameOverScreen.Height/2));
            Canvas.SetLeft(gameOverScreen, (maincanvas.Width/2) - (gameOverScreen.Width/2));

            restart.Content = "Play Again?";
            restart.Height = 50;
            restart.Width = 200;
            Canvas.SetLeft(restart, (maincanvas.Width/4) - (restart.Width / 2));
            Canvas.SetTop(restart, 450);

            mainMenuButton.Content = "Main Menu";
            mainMenuButton.Height = 50;
            mainMenuButton.Width = 200;
            Canvas.SetLeft(mainMenuButton, (3*maincanvas.Width / 4) - (mainMenuButton.Width / 2));
            Canvas.SetTop(mainMenuButton, 450);



            maincanvas.Children.Add(restart);
            maincanvas.Children.Add(mainMenuButton);

            GetName();

            restart.Click += RestartButton_Click;
            mainMenuButton.Click += mainMenuButton_Click;


        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            Restart();
        }
        private void Restart()
        {
            restart.Click -= RestartButton_Click;
            if (isGameLoopRunning)
            {
                CompositionTarget.Rendering -= GameLoop_Call;
                isGameLoopRunning = false;
            }

            maincanvas.Children.Remove(gameOverScreen);
            maincanvas.Children.Remove(restart);
            maincanvas.Children.Remove(mainMenuButton);
            maincanvas.Children.Remove(nameInput);
            maincanvas.Children.Remove(submitName);

            ClearScreen();

            scoreDisplay.Text = "Score: " + score;

            Enemies.enemyCount = 0;
            playerShipX = ((maincanvas.Width / 2) - (playerShip.Width / 2));
            playerShipY = 0;

            gameOver = false;

            InitialiseSprites();
            if (!isGameLoopRunning)
            {
                CompositionTarget.Rendering += GameLoop_Call;
                isGameLoopRunning = true;
            }
        }

        private void ClearScreen()
        {
            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                maincanvas.Children.Remove(enemy1s[i].enemy);
                enemy1s.Remove(enemy1s[i]);
            }

            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                maincanvas.Children.Remove(enemy2s[i].enemy);
                enemy2s.Remove(enemy2s[i]);
            }

            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                maincanvas.Children.Remove(enemy3s[i].enemy);
                enemy3s.Remove(enemy3s[i]);
            }

            for (int j = activeLasers.Count - 1; j >= 0; j--)
            {
                maincanvas.Children.Remove(activeLasers[j].laser);
                activeLasers.Remove(activeLasers[j]);
            }
                maincanvas.Children.Remove(playerShip);
        }

        private void DrawEnemies(double deltaTime)
        {
            int enemiesRemaining = enemy1s.Count + enemy2s.Count + enemy3s.Count;
            bool addRow = false;

            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                enemy1s[i].Redraw(enemiesRemaining, levelNumber, deltaTime);
            }
            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                enemy2s[i].Redraw(enemiesRemaining, levelNumber, deltaTime);
            }
            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                enemy3s[i].Redraw(enemiesRemaining, levelNumber, deltaTime);
            }

            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                char border = enemy1s[i].TouchingBorder();
                if (border != ' ')
                {
                    UndoLastMovement(border);
                    IncrementRow();
                    return;
                }
            }
            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                char border = enemy2s[i].TouchingBorder();
                if (border != ' ')
                {
                    UndoLastMovement(border);
                    IncrementRow();
                    return;
                }
            }
            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                char border = enemy3s[i].TouchingBorder();
                if (border != ' ')
                {
                    UndoLastMovement(border);
                    IncrementRow();                  
                    return;
                }
            }
        }

        private void UndoLastMovement(char border)
        {
            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                if (border == 'l') enemy1s[i].enemyX += enemy1s[i].MoveBy;
                else enemy1s[i].enemyX -= enemy1s[i].MoveBy;
            }
            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                if (border == 'l') enemy2s[i].enemyX += enemy2s[i].MoveBy;
                else enemy2s[i].enemyX -= enemy2s[i].MoveBy;
            }
            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                if (border == 'l') enemy3s[i].enemyX += enemy3s[i].MoveBy;
                else enemy3s[i].enemyX -= enemy3s[i].MoveBy;
            }
        }

        private void IncrementRow()
        {
            //enemy 1s
            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                enemy1s[i].AddRow();
            }

            //enemy 2s
            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                enemy2s[i].AddRow();
            }

            //enemy 3s
            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                enemy3s[i].AddRow();
            }

        }

        private void RemoveLaser()
        {
            for (int i = 0; i < activeLasers.Count; i++)
            {
                if (!activeLasers[i].IsActive())
                {
                    //if it goes off the screen remove it to avoid clutter
                    maincanvas.Children.Remove(activeLasers[i].laser);
                    activeLasers.Remove(activeLasers[i]);
                }
            }
        }

        private void DetectHit()
        {
            for (int i = enemy1s.Count - 1; i >= 0; i--)
            {
                for (int j = activeLasers.Count - 1; j >= 0; j--)
                {
                    if (enemy1s[i].TouchingLaser(activeLasers[j].LaserX, activeLasers[j].LaserY))
                    {
                        score += enemy1s[i].AddPoints(levelNumber);
                        scoreDisplay.Text = "Score: " + score;
                        //remove enemy once touched by laser
                        maincanvas.Children.Remove(enemy1s[i].enemy);
                        enemy1s.Remove(enemy1s[i]);

                        //remove laser once enemy has been killed so it doesnt kill anyone else
                        maincanvas.Children.Remove(activeLasers[j].laser);
                        activeLasers.Remove(activeLasers[j]);

                        //no need to check for other lasers once this enemy has been removed
                        break;
                    }
                }
            }

            for (int i = enemy2s.Count - 1; i >= 0; i--)
            {
                for (int j = activeLasers.Count - 1; j >= 0; j--)
                {
                    if (enemy2s[i].TouchingLaser(activeLasers[j].LaserX, activeLasers[j].LaserY))
                    {
                        score += enemy2s[i].AddPoints(levelNumber);
                        scoreDisplay.Text = "Score: " + score;
                        //remove enemy once touched by laser
                        maincanvas.Children.Remove(enemy2s[i].enemy);
                        enemy2s.Remove(enemy2s[i]);

                        //remove laser once enemy has been killed so it doesnt kill anyone else
                        maincanvas.Children.Remove(activeLasers[j].laser);
                        activeLasers.Remove(activeLasers[j]);

                        //no need to check for other lasers once this enemy has been removed
                        break;
                    }
                }
            }

            for (int i = enemy3s.Count - 1; i >= 0; i--)
            {
                for (int j = activeLasers.Count - 1; j >= 0; j--)
                {
                    if (enemy3s[i].TouchingLaser(activeLasers[j].LaserX, activeLasers[j].LaserY))
                    {
                        score += enemy3s[i].AddPoints(levelNumber);
                        scoreDisplay.Text = "Score: " + score;
                        //remove enemy once touched by laser
                        maincanvas.Children.Remove(enemy3s[i].enemy);
                        enemy3s.Remove(enemy3s[i]);

                        //remove laser once enemy has been killed so it doesnt kill anyone else
                        maincanvas.Children.Remove(activeLasers[j].laser);
                        activeLasers.Remove(activeLasers[j]);

                        //no need to check for other lasers once this enemy has been removed
                        break;
                    }
                }
            }
        }

        private void InitialiseSprites()
        {
            playerShip.Height = 57;
            playerShip.Width = 57;
            playerShipX = ((maincanvas.Width / 2) - (playerShip.Width / 2));
            playerShipY = 0;
            Canvas.SetLeft(playerShip, playerShipX);
            Canvas.SetBottom(playerShip, playerShipY);
            maincanvas.Children.Add(playerShip);

            //enemy 3s
            for (int i = 0; i < enemy3Count; i++)
            {
                enemy3s.Add(new Enemy3(maincanvas));
            }

            //enemy 2s
            for (int i = 0; i < enemy2Count; i++)
            {
                enemy2s.Add(new Enemy2(maincanvas));
            }

            //enemy 1s
            for (int i = 0; i < enemy1Count; i++)
            {
                enemy1s.Add(new Enemy1(maincanvas));
            }
        }

        private void FireLaser()
        {
            laser = new Image();
            laser.Source = new BitmapImage(new Uri(laserPath, UriKind.Relative));
            maincanvas.Children.Add(laser);
            double laserY = playerShipY + 55;
            double laserX = playerShipX + 27;
            Canvas.SetBottom(laser, laserY);
            Canvas.SetLeft(laser, laserX);

            activeLasers.Add(new Laser(laserY, laserX, laser, maincanvas));
        }

        private void Key_Down(object sender, KeyEventArgs e)
        {
            keysPressed.Add(e.Key);
        }

        private void Key_Up(object sender, KeyEventArgs e)
        {
            keysPressed.Remove(e.Key);
        }
        
        private void SetupCanvas()
        {
            this.SizeToContent = SizeToContent.WidthAndHeight;

            maincanvas = new Canvas();

            this.Content = maincanvas;
            this.Height = 600;
            this.Width = 600;

            playerShip.Source = new BitmapImage(new Uri(playerShipPath, UriKind.Relative));

            maincanvas.Height = 600;
            maincanvas.Width = 600;
            maincanvas.Background = Brushes.Black;
        }
    }
}