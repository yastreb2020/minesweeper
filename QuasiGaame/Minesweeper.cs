using System;
using System.Collections.Generic;
using System.Text;

namespace QuasiGaame
{
    class Minesweeper
    {
        /// <summary>
        /// field size, n - rows, m - columns
        /// </summary>
        int n, m;
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
        int rows, cols;
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
        /// Starts a game
        /// </summary>
        public void StartGame()
        {
            StartMenu();

            game = new string[n, m];
            CreateGame();
            field = CreateField();
            rows = field.GetUpperBound(0) + 1;
            cols = field.Length / rows;
            //Console.Clear();
            UpdateField();
            Console.SetCursorPosition(0, 0);


            // user coordinates
            int lastCoorI = 0, lastCoorJ = 0;
            int coorI = 0, coorJ = 0;


            ConsoleKeyInfo keyinfo;

            while (true)
            {
                keyinfo = Console.ReadKey();
                lastCoorI = coorI;
                lastCoorJ = coorJ;
                switch (keyinfo.Key)
                {
                    case ConsoleKey.Spacebar:
                        if (field[coorI, coorJ] == markSign)
                        {
                            field[coorI, coorJ] = voidSign;
                        }
                        else
                        {
                            field[coorI, coorJ] = markSign;
                        }
                        UpdateField();
                        break;
                    case ConsoleKey.Enter:
                        field[coorI, coorJ] = game[coorI, coorJ];
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
                            Move(coorI, coorJ, lastCoorI, lastCoorJ);
                        }
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        if (coorJ < m - 1)
                        {
                            coorJ++;
                            Move(coorI, coorJ, lastCoorI, lastCoorJ);
                        }
                        break;
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        if (coorI > 0)
                        {
                            coorI--;
                            Move(coorI, coorJ, lastCoorI, lastCoorJ);
                        }
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        if (coorI < n - 1)
                        {
                            coorI++;
                            Move(coorI, coorJ, lastCoorI, lastCoorJ);
                        }
                        break;
                    case ConsoleKey.X:
                    case ConsoleKey.Escape:
                        break;

                }
                // is the number of opened cells equals the number of cells - number of bombs - number of marksignes?
                if (n*m - NumOfOpenCells(bombSign) - NumOfOpenCells(markSign) == NumOfOpenCells())
                {
                    GameWon();
                }
            }
        }

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
            Console.WriteLine("Congrats! You won! Wanna show off again?");
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
                    n = 5; m = 5;
                    bombs = 5;
                    mode = "Easy";
                    break;
                case "2":
                    n = 10; m = 10;
                    bombs = 20;
                    mode = "Normal";
                    break;
                case "3":
                    n = 25; m = 50;
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
        /// <param name="field"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        private void UpdateField()
        {
            Console.Clear();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(field[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Shows the starting game field
        /// </summary>
        string[,] CreateField()
        {
            string[,] field = new string[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    field[i, j] = voidSign;
                }
            }
            return field;
        }

        /// <summary>
        /// Displays user moves on the field when he/she goes from one dot to another
        /// </summary>
        /// <param name="coorI"></param>
        /// <param name="coorJ"></param>
        /// <param name="lastCoorI"></param>
        /// <param name="lastCoorJ"></param>
        void Move(int coorI, int coorJ, int lastCoorI, int lastCoorJ)
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
            int rows = field.GetUpperBound(0) + 1;
            int columns = field.Length / rows;

            Random rand = new Random();

            for (int i = 0; i < bombs; i++)
            {
                field[rand.Next(0, rows - 1), rand.Next(0, columns - 1)] = bombSign;
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (field[i, j] == bombSign) continue;
                    int sum = 0;
                    if (i > 0 && field[i - 1, j] == bombSign) sum++;
                    if (i < rows - 1 && field[i + 1, j] == bombSign) sum++;
                    if (j > 0 && field[i, j - 1] == bombSign) sum++;
                    if (j < columns - 1 && field[i, j + 1] == bombSign) sum++;
                    if (i > 0 && j > 0 && field[i - 1, j - 1] == bombSign) sum++;
                    if (i < rows - 1 && j < columns - 1 && field[i + 1, j + 1] == bombSign) sum++;
                    if (i > 0 && j < columns - 1 && field[i - 1, j + 1] == bombSign) sum++;
                    if (i < rows - 1 && j > 0 && field[i + 1, j - 1] == bombSign) sum++;
                    field[i, j] = sum.ToString();
                }
            }
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
        */


    }
}
