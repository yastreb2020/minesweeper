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
        /// Constructor which starts a game
        /// </summary>
        public Minesweeper()
        {
            StartMenu();

            string[,] game = new string[n, m]; //array with info about bombs and nums
            CreateGame(game, bombs);
            string[,] field = CreateField(n, m); // array with graphical representation of game
            //Console.Clear();
            UpdateField(field);
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
                    case ConsoleKey.Enter:
                        field[coorI, coorJ] = game[coorI, coorJ];
                        UpdateField(field);
                        if (field[coorI, coorJ] == "X")
                            Console.WriteLine("Loser.");
                        break;
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        if (coorJ > 0)
                        {
                            coorJ--;
                            Move(field, coorI, coorJ, lastCoorI, lastCoorJ);
                        }
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        if (coorJ < m - 1)
                        {
                            coorJ++;
                            Move(field, coorI, coorJ, lastCoorI, lastCoorJ);
                        }
                        break;
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        if (coorI > 0)
                        {
                            coorI--;
                            Move(field, coorI, coorJ, lastCoorI, lastCoorJ);
                        }
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        if (coorI < n - 1)
                        {
                            coorI++;
                            Move(field, coorI, coorJ, lastCoorI, lastCoorJ);
                        }
                        break;
                    case ConsoleKey.X:
                    case ConsoleKey.Escape:
                        break;

                }
            }
        }

        /// <summary>
        /// shows menu before and after game (in pause?)
        /// </summary>
        private void StartMenu()
        {
            Console.WriteLine("Choose your mode:\n1 - Easy\n2 - Normal\n3 - Hard");
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
        private void UpdateField(string[,] field)
        {
            Console.Clear();

            int rows = field.GetUpperBound(0) + 1;
            int columns = field.Length / rows;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Console.Write(field[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Shows the starting game field
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <param name="b"></param>
        string[,] CreateField(int n, int m)
        {
            string[,] field = new string[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    field[i, j] = ".";
                }
            }
            return field;
        }

        /// <summary>
        /// Displays user moves on the field when he/she goes from one dot to another
        /// </summary>
        /// <param name="field"></param>
        /// <param name="coorI"></param>
        /// <param name="coorJ"></param>
        /// <param name="lastCoorI"></param>
        /// <param name="lastCoorJ"></param>
        void Move(string[,] field, int coorI, int coorJ, int lastCoorI, int lastCoorJ)
        {
            // if you haven't opened any cell before moving, you'll leave a dot behind
            if (field[lastCoorI, lastCoorJ] == "*")
            {
                field[lastCoorI, lastCoorJ] = ".";
            }

            // whem moving, you (almost!) always look like "*"
            if (field[coorI, coorJ] == ".")
                field[coorI, coorJ] = "*";
            UpdateField(field);
        }

        
        /// <summary>
        /// create array game field with bombs
        /// </summary>
        /// <param name="field"></param>
        /// <param name="b"></param>
        void CreateGame(string[,] field, int b)
        {
            int rows = field.GetUpperBound(0) + 1;
            int columns = field.Length / rows;

            Random rand = new Random();

            for (int i = 0; i < b; i++)
            {
                field[rand.Next(0, rows - 1), rand.Next(0, columns - 1)] = "X";
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (field[i, j] == "X") continue;
                    int sum = 0;
                    if (i > 0 && field[i - 1, j] == "X") sum++;
                    if (i < rows - 1 && field[i + 1, j] == "X") sum++;
                    if (j > 0 && field[i, j - 1] == "X") sum++;
                    if (j < columns - 1 && field[i, j + 1] == "X") sum++;
                    if (i > 0 && j > 0 && field[i - 1, j - 1] == "X") sum++;
                    if (i < rows - 1 && j < columns - 1 && field[i + 1, j + 1] == "X") sum++;
                    if (i > 0 && j < columns - 1 && field[i - 1, j + 1] == "X") sum++;
                    if (i < rows - 1 && j > 0 && field[i + 1, j - 1] == "X") sum++;
                    field[i, j] = sum.ToString();
                }
            }
        }


        /*ЧТО НЕ ТАК:
         * неверно расставились числа -исправлено
         * хочу чтобы как-то отражалось, где я нахожусь, когда на числах (можно настроить цвет?)
         * написать правила игры, описание клавиши
         * добавить отметку потенциальных бомб пользователем
         * добавить выход в главное меню
         * режим паузы в перспективе
         * добавить возможность изменить выбор режима игры
         * добавить время игры
         * добавить систему очков
         * сохранение очков в перспективе
         * выводить номер бомб
         * номер найденных бомб
         * настроить победу
         * сделать победу красивой
         * начальная позиция поля: сделать отметку курсора
         * обработка ошибок
         * Console.Beep() при проигрыше и выигрыше :))))
         * журнализация?
        */

        //onClick when the user hits enter
        //if it's a bomb the game is stopped, if not, it shows
        /*
        static void OnClick(string[,] field, string[,] game, int coorI, int coorJ)
        {
            
        }
        */

        //when the user moves around the field

    }
}
