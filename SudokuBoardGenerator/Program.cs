using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SudokuBoardGenerator {
    class Program {
        // characters used in the grid
        public const string valueList = "123456789ABCDEFGHIJKLMNOP";

        static void Main(string[] args) {
            // start generating grid
            int n = 3;
            int[,] board = new int[n, n];
            var tasks = new[] {
                Task.Factory.StartNew(() => new Grid(n).Board),
                Task.Factory.StartNew(() => new Grid(n).Board),
                Task.Factory.StartNew(() => new Grid(n).Board),
                Task.Factory.StartNew(() => new Grid(n).Board)}; 
            var things = Task.WhenAll(tasks);
            foreach (var res in things.Result) {
                if (res != null) {
                    board = (int[,]) res.Clone();
                }
            }
            Grid.PrintBoard(board);
            SaveBoard(board);
        }

        public static void SaveBoard(int[,] board) {
            string bnum = $"{Math.Sqrt(board.GetLength(0))}";
            string path = Path.GetFullPath("./");
            if (path.Substring(path.Length - 18, 17) != "SudokuBoardGenerator") {
                path = String.Concat(path, @"../../../boards");
            }
            path = Path.GetFullPath(path);
            string[] boards = Directory.GetFiles(path);
            int count = boards.Count(b => b[path.Length+6] == bnum[0]);
            bnum += $".{count+1}";
            path += $"/board{bnum}";
            using (StreamWriter writer = new StreamWriter(path)) {
                for (int i = 0; i < board.GetLength(0); i++) {
                    for (int j = 0; j < board.GetLength(1); j++) {
                        writer.Write($"{board[i, j]} ");
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}