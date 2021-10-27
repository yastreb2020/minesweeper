using System;
using System.Collections.Generic;
using System.Text;

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
        ConsoleColor mainColor = new ConsoleColor();

        int numOfOpenCells = 0;

        // user coordinates
        int lastCoorI = 0, lastCoorJ = 0;
        int coorI = 0, coorJ = 0;

        /// <summary>
        /// Starts a game
        /// </summary>
        public void StartGame()
        {
            StartMenu();

            game = new string[rows, cols];
            CreateGame();
            CreateField();
            UpdateField();
            Console.SetCursorPosition(0, 0);

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
                        }
                        else if (field[coorI, coorJ] == userSign)
                        {
                            field[coorI, coorJ] = markSign;
                        }
                        else break; // you won't see any changes if you hit a spacebar on an opened cell
                        UpdateField();
                        break;
                    // here's the logic of openening a cell a user is on, after he/she hits an enter
                    case ConsoleKey.Enter:
                        field[coorI, coorJ] = game[coorI, coorJ];
                        numOfOpenCells++; //we opened one more cell!
                        UpdateField();
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
                    case ConsoleKey.X:
                    case ConsoleKey.Escape:
                        //exit from the game, close the window
                        break;

                }
                
                if(numOfOpenCells == rows * cols - GetNumOfBombs())
                {
                    GameWon();
                }

                // is the number of opened cells equals the number of cells - number of bombs - number of marksignes?
                /*if (rows*cols - NumOfOpenCells(bombSign) - NumOfOpenCells(markSign) == NumOfOpenCells())
                {
                    GameWon();
                }
                */
            }
        }

        /*
        /// <summary>
        /// checkS the numBER of opened cells of particular type(constants) on the field
        /// </summary>
        //do i need this method?
        int NumOfOpenCells(string type)
        {
            int cnt = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (field[i, j] == type)
                    {
                        cnt++;
                    }
                }
            }

            return cnt;
        }

        /// <summary>
        /// counts the number of cells opened by a user at the moment
        /// </summary>
        /// <returns></returns>
        int NumOfOpenCells()
        {
            int cnt = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // we 
                    if (field[i, j] != markSign && field[i, j] != voidSign && field[i, j] != userSign)
                    {
                        cnt++;
                    }
                }
            }

            return cnt;
        }
        */

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
            Console.WriteLine("Choose your mode:\n1 - Easy\n2 - Normal\n3 - Hard\n(For exit press Esc)");
            switch (Console.ReadLine())
            {
                default:
                case "1":
                    rows = 5; cols = 5;
                    bombs = 5;
                    mode = "Easy";
                    break;
                case "2":
                    rows = 10; cols = 10;
                    bombs = 20;
                    mode = "Normal";
                    break;
                case "3":
                    rows = 25; cols = 50;
                    bombs = 100;
                    mode = "Hard";
                    break;
            }
            Console.WriteLine($"You have chosen {mode} mode. Press any key to continue.");
            //user can rechoose
            Console.ReadKey();
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
                        case userSign:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
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
                        mainColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(field[i, j] + " ");
                        Console.ForegroundColor = mainColor;
                        continue;
                    }

                    // for all other cells we just need to print them
                    Console.Write(field[i, j] + " ");
                }
                Console.WriteLine();
            }
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


        /*ЧТО НЕ ТАК:
         * неверно расставились числа +
         * хочу чтобы как-то отражалось, где я нахожусь, когда на числах (можно настроить цвет?)
         * написать правила игры, описание клавиши
         * добавить отметку потенциальных бомб пользователем +
         * добавить выход в главное меню + (только в конце игры)
         * режим паузы в перспективе
         * добавить возможность изменить выбор режима игры
         * добавить время игры
         * добавить систему очков
         * сохранение очков в перспективе
         * выводить номер бомб
         * номер найденных бомб
         * настроить победу +
         * сделать победу красивой
         * начальная позиция поля: сделать отметку курсора
         * обработка ошибок
         * Console.Beep() при проигрыше и выигрыше :))))
         * журнализация?
         * добавить выход из меню
         * что такое гейм что такое филд, пересмотреть все методы
         * победа не работает, когда ? больше чем нужно +
         * нули сами открывались чтобы автоматически, когда на один из них зашел
         * число бомб рандомно не всегда совпадает с заданным
        */


    }
}
