using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace QuasiGaame
{
    class Minesweeper
    {
        /// <summary>
        /// field size, rows and columns
        /// </summary>
        int rows, cols;
        /// <summary>
        /// number of bombs
        /// </summary>
        int bombs;
        /// <summary>
        /// game mode name
        /// </summary>
        string mode = "";
        /// <summary>
        /// array with info about bombs and nums - logic of game
        /// </summary>
        string[,] game;
        /// <summary>
        /// array with graphical representation of game - just the output
        /// </summary>
        string[,] field;
        
        /// <summary>
        /// what you see a bomb like when you play (and what it is in the game array)
        /// </summary>
        const string bombSign = "X";
        /// <summary>
        /// what user looks like when he/she moves around the field
        /// </summary>
        const string userSign = "*";
        /// <summary>
        /// what the cells a user haven't opened yet look like
        /// </summary>
        const string voidSign = ".";
        /// <summary>
        /// what user see when he/she wants to mark a cell like a potential bomb place
        /// </summary>
        const string markSign = "?";

        /// <summary>
        /// used to save a foreground color before changing and return to it after
        /// </summary>
        //ConsoleColor mainColor = new ConsoleColor();

        /// <summary>
        /// the number of cells that a user have opened so far
        /// </summary>
        int numOfOpenCells;

        int numOfBombs;
        int numOfMarks;

        /// <summary>
        /// prevoius user coordinates on a field
        /// </summary>
        int lastCoorI, lastCoorJ;
        /// <summary>
        /// current user coordinates on a field
        /// </summary>
        int coorI, coorJ;

        /// <summary>
        /// initialization of game variables
        /// </summary>
        void GameInit()
        {
            game = new string[rows, cols];
            CreateGame();
            coorI = 0;
            coorJ = 0;
            lastCoorI = 0;
            lastCoorJ = 0;
            numOfOpenCells = 0;
            numOfBombs = GetNumOfBombs();
            numOfMarks = 0;
            CreateField();
            UpdateField();
            Console.SetCursorPosition(0, 0);
        }

        /// <summary>
        /// Starts a game
        /// </summary>
        public void StartGame()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Ah, there you are!");


            StartMenu();
            GameInit();

            ConsoleKeyInfo keyinfo;

            while (true)
            {
                keyinfo = Console.ReadKey();
                lastCoorI = coorI;
                lastCoorJ = coorJ;
                switch (keyinfo.Key)
                {
                    // here's the logic of marking potential bombs on hitting spacebar
                    case ConsoleKey.Spacebar:
                        if (field[coorI, coorJ] == markSign)
                        {
                            field[coorI, coorJ] = userSign;
                            numOfMarks--;
                        }
                        else if (field[coorI, coorJ] == userSign)
                        {
                            field[coorI, coorJ] = markSign;
                            numOfMarks++;
                        }
                        else break; // you won't see any changes if you hit a spacebar on an opened cell
                        UpdateField();
                        break;

                    // here's the logic of openening a cell a user is on, after he/she hits an enter
                    case ConsoleKey.Enter:
                        //if it was markSign before we need to decrease their counter
                        if (field[coorI, coorJ] == markSign)
                        {
                            numOfMarks--;
                        }

                        field[coorI, coorJ] = game[coorI, coorJ];
                        numOfOpenCells++; //we opened one more cell!
                        UpdateField();
                        // automatic zero cells opening
                        if (field[coorI, coorJ] == "0")
                        {
                            OpenedZeroCell(coorI, coorJ);
                            //update once all the cells around
                            UpdateField();
                        }

                        if (field[coorI, coorJ] == bombSign)
                        {
                            Console.WriteLine("Loser.");
                            // + stop counts
                            GameLost();
                        }
                        break;

                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        if (coorJ > 0)
                        {
                            coorJ--;
                            Move();
                        }
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        if (coorJ < cols - 1)
                        {
                            coorJ++;
                            Move();
                        }
                        break;
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        if (coorI > 0)
                        {
                            coorI--;
                            Move();
                        }
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        if (coorI < rows - 1)
                        {
                            coorI++;
                            Move();
                        }
                        break;
                    case ConsoleKey.P:
                    case ConsoleKey.X:
                    case ConsoleKey.Escape:
                        Pause();
                        break;

                }
                
                if(numOfOpenCells == rows * cols - numOfBombs)
                {
                    GameWon();
                }

            }
        }

        /*
         * i-1, j-1     i-1, j      i-1, j+1
         * 
         * i, j-1       [i, j]        i, j+1
         * 
         * i+1, j-1     i+1, j      i+1, j+1
         * */

        void OpenedZeroCell(int i, int j)
        {
            // preveting outside of bounds of the array error
            int lowI, highI, lowJ, highJ;
            if (i == 0) lowI = 0;
            else lowI = i - 1;
            if (i == rows - 1) highI = rows - 1;
            else highI = i + 1;

            if (j == 0) lowJ = 0;
            else lowJ = j - 1;
            if (j == cols - 1) highJ = cols - 1;
            else highJ = j + 1;

            for (int a = lowI; a <= highI; a++)
            {
                for (int b = lowJ; b <= highJ; b++)
                {
                    if (field[a, b] != voidSign)
                        continue;
                    //we don't need to check the cell we already know is zero
                    //if (a == i && b == j)
                    //    continue;


                    //for any cell around the known zero, we open it, check if it's a zero and if yes, repeat the same actions
                    field[a, b] = game[a, b];
                    numOfOpenCells++; //don't forget to increase the counter
                    if (field[a, b] == "0")
                        OpenedZeroCell(a, b);
                }
            }
        }

        // create a method for losing/winning
        // do i need those two?.. for the future?.. like keeping records?
        void GameLost()
        {
            Console.WriteLine("\nYou've lost! Don't get upset - you've fought like a real warrior. Try again.");
            Console.WriteLine("Press any key to escape to the main menu.");
            Console.ReadKey();
            Console.Clear();
            StartGame();
        }

        void GameWon()
        {
            Console.WriteLine("Congrats! You've won! Wanna show off again?");
            Console.WriteLine("Press any key to escape to the main menu.");
            Console.ReadKey();
            Console.Clear();
            StartGame();
        }

        /// <summary>
        /// shows menu before and after game (in pause?)
        /// </summary>
        private void StartMenu()
        {
            Console.Clear();
            Console.WriteLine("Choose your mode:\n1 - Easy\n2 - Normal\n3 - Hard\n(For exit press Esc)");
            switch (Console.ReadKey().Key)
            {
                default:
                case ConsoleKey.D1:
                    rows = 5; cols = 5;
                    bombs = 5;
                    mode = "Easy";
                    break;
                case ConsoleKey.D2:
                    rows = 10; cols = 10;
                    bombs = 20;
                    mode = "Normal";
                    break;
                case ConsoleKey.D3:
                    rows = 25; cols = 50;
                    bombs = 100;
                    mode = "Hard";
                    break;
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
            }
            Console.WriteLine($"You have chosen {mode} mode. Goodluck!");
            System.Threading.Thread.Sleep(100);
            //user can rechoose ?
            //Console.ReadKey();
        }

        /// <summary>
        /// updates field after each user input: clears and then draws updated game field
        /// </summary>
        private void UpdateField()
        {
            Console.Clear();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    switch(field[i, j])
                    {
                        case markSign:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            break;
                        case "0":
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            break;
                        case "1":
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case "2":
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            break;
                        case "3":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case "4":
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case "5":
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case "6":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case "7":
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            break;
                        case "8":
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                    }

                    //special color for the place user is in
                    if(i == coorI && j == coorJ)
                    {
                        //mainColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(field[i, j] + " ");
                        //Console.ForegroundColor = mainColor;
                        continue;
                    }

                    // for all other cells we just need to print them
                    Console.Write(field[i, j] + " ");
                }
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Mines Total: {numOfBombs} \t Mines Marked: {numOfMarks}");
        }

        /// <summary>
        /// Shows the starting game field
        /// </summary>
        void CreateField()
        {
            field = new string[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    field[i, j] = voidSign;
                }
            }
        }

        /// <summary>
        /// Displays user moves on the field when he/she goes from one dot to another
        /// </summary>
        void Move()
        {
            // if you haven't opened any cell before moving, you'll leave a dot behind
            if (field[lastCoorI, lastCoorJ] == userSign)
            {
                field[lastCoorI, lastCoorJ] = voidSign;
            }

            // whem moving, you (almost!) always look like "*"
            if (field[coorI, coorJ] == voidSign)
                field[coorI, coorJ] = userSign;
            UpdateField();
        }

        
        /// <summary>
        /// create array game field with bombs
        /// </summary>
        void CreateGame()
        {
            Random rand = new Random();

            for (int i = 0; i < bombs; i++)
            {
                game[rand.Next(0, rows - 1), rand.Next(0, cols - 1)] = bombSign;
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (game[i, j] == bombSign) continue;
                    int sum = 0;
                    if (i > 0 && game[i - 1, j] == bombSign) sum++;
                    if (i < rows - 1 && game[i + 1, j] == bombSign) sum++;
                    if (j > 0 && game[i, j - 1] == bombSign) sum++;
                    if (j < cols - 1 && game[i, j + 1] == bombSign) sum++;
                    if (i > 0 && j > 0 && game[i - 1, j - 1] == bombSign) sum++;
                    if (i < rows - 1 && j < cols - 1 && game[i + 1, j + 1] == bombSign) sum++;
                    if (i > 0 && j < cols - 1 && game[i - 1, j + 1] == bombSign) sum++;
                    if (i < rows - 1 && j > 0 && game[i + 1, j - 1] == bombSign) sum++;
                    game[i, j] = sum.ToString();
                }
            }
        }

        int GetNumOfBombs()
        {
            int cnt = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (game[i, j] == bombSign)
                        cnt++;
                }
            }
            return cnt;
        }

        string pauseSign = "*";

        void Pause()
        {
            Console.Clear();
            for(int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(pauseSign + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\nYou're in a pause mode. Press Enter to continue.");
            Console.WriteLine("To escape to the main menu and finish the game press Esc.");

            switch(Console.ReadKey().Key)
            {
                case ConsoleKey.Enter:
                    UpdateField();
                    break;
                case ConsoleKey.Escape:
                    StartMenu();
                    break;
            }
        }

        /*ЧТО НЕ ТАК:
         * написать правила игры, описание клавиши
         * режим паузы в перспективе
         * добавить возможность изменить выбор режима игры
         * добавить время игры
         * добавить систему очков
         * сохранение очков в перспективе
         * сделать победу красивой
         * обработка ошибок
         * Console.Beep() при проигрыше и выигрыше :))))
         * журнализация?
         * добавить выход из меню
         * что такое гейм что такое филд, пересмотреть все методы
         * число бомб рандомно не всегда совпадает с заданным
        */


    }
}
