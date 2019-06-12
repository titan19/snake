using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] levels = null;
            string[] saved = null;
            if (Directory.Exists("saved"))
                saved = Directory.GetFiles("saved");
            if (Directory.Exists("levels"))
                levels = Directory.GetFiles("levels");

            var i = 0;
            if (saved != null)
            {
                Console.WriteLine("You have saved games: ");
                foreach (var save in saved)
                {
                    Console.WriteLine($"{i++}: {Path.GetFileName(save)}");
                }
            }
            if (levels != null)
            {
                Console.WriteLine("You have more levels: ");
                foreach (var level in levels)
                {
                    Console.WriteLine($"{i++}: {Path.GetFileName(level)}");
                }
            }

            char[,] map = null;

            if (levels != null || saved != null)
            {

                Console.WriteLine("Please enter a number of chosen variant (Enter for standard game)");
                var result = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(result))
                {
                    int variant;
                    if (int.TryParse(result, out variant) && variant >= 0 && variant < (levels?.Length ?? 0) + (saved?.Length ?? 0))
                    {
                        var paths = saved == null ? levels.ToList() : levels == null ? saved.ToList() : saved.Concat(levels).ToList();
                        var path = paths[variant];
                        map = LoadFromFile(path);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect variant. Starting standard game.");
                    }
                }
            }

            Console.CursorVisible = false;
            new Game().Start(map);
        }

        public static char[,] LoadFromFile(string file)
        {
            var lines = File.ReadAllLines(file);
            var columns = lines.FirstOrDefault()?.Length;
            if (columns == null)
                return null;
            var result = new char[columns.Value, lines.Length];
            for (var x = 0; x < columns; x++)
            {
                for (var y = 0; y < lines.Length; y++)
                {
                    result[x, y] = lines[y][x];
                }
            }

            return result;
        }
    }
}
