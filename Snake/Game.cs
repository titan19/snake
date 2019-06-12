using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Snake
{
    public class Game
    {
        private char[,] field = new char[15, 10];

        private Point headPrev = Point.None;
        private Point head = Point.None;
        private int points = 0;
        private List<Point> tail = new List<Point>();
        private int cherryCount = 0;
        private ConsoleKey prevKey;

        public void Start(char[,] map = null)
        {
            Console.CursorVisible = false;
            if (map == null)
                InitGame();
            else
                ParseField(map);
            Draw();

            ConsoleKey key;
            do
            {
                if (cherryCount == 0)
                    GenerateCherry();
                if (Console.KeyAvailable)
                {
                    key = Console.ReadKey().Key;
                    while (Console.KeyAvailable)
                    {
                        key = Console.ReadKey().Key;
                    }
                    KeyProcess(key);
                    if (key == ConsoleKey.S)
                    {
                        key = prevKey;
                        KeyProcess(key);
                    }
                    else
                        prevKey = key;
                }
                else
                {
                    key = prevKey;
                    KeyProcess(prevKey);
                }

                if (ValidateHead())
                    break;
                DrawField();
                Thread.Sleep(500);
            } while (key != ConsoleKey.Enter);

            if (key != ConsoleKey.Enter)
                Console.ReadKey();
        }

        private void ParseField(char[,] map)
        {
            field = map;
            for (var i = 0; i < map.Length; i++)
            {
                var x = i / map.GetLength(1);
                var y = i % map.GetLength(1);
                if (map[x, y] == 'O')
                {
                    head = new Point(x, y);
                }
                else if (map[x, y] == 'o')
                {
                    tail.Add(new Point(x, y));
                    points++;
                }
                else if (map[x, y] == 'V')
                {
                    cherryCount++;
                }
            }
        }

        private void GenerateCherry()
        {
            var index = new Random().Next(field.Length);
            while (field[index / field.GetLength(0), index % field.GetLength(1)] != '.')
            {
                index++;
                if (index >= field.Length)
                    index = 0;
            }

            Draw(index / field.GetLength(0), index % field.GetLength(1), 'V');
            cherryCount++;
        }

        private void InitGame()
        {
            DrawInit();
            head = new Point { x = 7, y = 4 };
        }

        private void DrawField()
        {
            Draw(headPrev, '.');
            Draw(head, 'O');
            foreach (var part in tail)
                Draw(part, 'o');
            DrawInConsole(0, field.GetLength(1) + 2, "Your points is: " + points);
        }

        void KeyProcess(ConsoleKey key)
        {
            headPrev = head;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    head.y--;
                    break;
                case ConsoleKey.DownArrow:
                    head.y++;
                    break;
                case ConsoleKey.LeftArrow:
                    head.x--;
                    break;
                case ConsoleKey.RightArrow:
                    head.x++;
                    break;
                case ConsoleKey.S:
                    SaveGame();
                    break;
            }
        }

        private void SaveGame()
        {
            if (!Directory.Exists("saved"))
                Directory.CreateDirectory("saved");
            var filename = $"saved\\{DateTime.Now:HH-mm-ss_dd-MM-yyyy}.map";
            using (StreamWriter wr = new StreamWriter(filename))
            {
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    for (int x = 0; x < field.GetLength(0); x++)
                    {
                        wr.Write(field[x, y]);
                    }
                    wr.WriteLine();
                }
            }
            DrawInConsole(0, field.GetLength(1) + 4, $"game was saved into '{filename}'");
        }

        private bool ValidateHead()
        {
            if (head.x < 0 || head.x >= field.GetLength(0))
                head.x = (field.GetLength(0) + head.x) % field.GetLength(0);
            if (head.y < 0 || head.y >= field.GetLength(1))
                head.y = (field.GetLength(1) + head.y) % field.GetLength(1);
            var ch = field[head.x, head.y];
            if (ch == 'X')
                head = headPrev;
            if (ch == 'V')
            {
                points++;
                cherryCount--;
                if (points > 20)
                {
                    DrawInConsole(0, field.GetLength(1) + 3, "You win!");
                    return true;
                }
            }
            if (ch == 'o')
            {
                DrawInConsole(0, field.GetLength(1) + 3, "You lose!");
                return true;
            }

            if (head == headPrev)
                return false;
            tail.Insert(0, headPrev);
            var result = tail.Take(points).ToList();
            foreach (var part in tail.Skip(points))
            {
                Draw(part, '.');
            }

            tail = result;
            return false;
        }

        void DrawInit()
        {
            for (int y = 0; y < field.GetLength(1); y++)
            {
                for (int x = 0; x < field.GetLength(0); x++)
                {
                    if (x == 0 || y == 0 || x == field.GetLength(0) - 1 || y == field.GetLength(1) - 1)
                        field[x, y] = 'X';
                    else
                        field[x, y] = '.';
                }
            }
        }

        void Draw()
        {
            Console.Clear();
            for (int y = 0; y < field.GetLength(1); y++)
            {
                for (int x = 0; x < field.GetLength(0); x++)
                {
                    Console.Write(field[x, y]);
                }
                Console.WriteLine();
            }
        }

        void Draw(Point p, char ch)
        {
            Draw(p.x, p.y, ch);
        }

        void Draw(int x, int y, char ch)
        {
            if (x < 0 || y < 0)
                return;
            field[x, y] = ch;
            DrawInConsole(x, y, ch.ToString());
        }

        void DrawInConsole(int x, int y, string s)
        {
            Console.CursorLeft = x;
            Console.CursorTop = y;
            Console.Write(s);

            Console.CursorLeft = 0;
            Console.CursorTop = 13;
        }
    }
}