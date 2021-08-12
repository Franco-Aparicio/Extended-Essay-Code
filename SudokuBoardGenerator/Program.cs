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
            int n = 5;
            int[,] answer = null;
            var tasks = new[] {
                Task.Factory.StartNew(() => new Grid(n).Board),
                Task.Factory.StartNew(() => new Grid(n).Board),
                Task.Factory.StartNew(() => new Grid(n).Board),
                Task.Factory.StartNew(() => new Grid(n).Board)}; 
            var things = Task.WhenAll(tasks);
            foreach (var res in things.Result) {
                if (res != null) {
                    answer = (int[,]) res.Clone();
                }
            }
            Grid.PrintBoard(answer);
            int[,] board = LeaveClues(answer);
            Grid.PrintBoard(board);
            SaveBoard(board);
        }

        public static int[,] LeaveClues(int[,] answer) {
            Random r = new Random();
            int squares = answer.GetLength(0)*answer.GetLength(1);
            int n = (int) Math.Sqrt(answer.GetLength(0));
            int empties = squares * (n - 1) / (n + 1);
            foreach (int c in Enumerable.Range(0, squares).OrderBy(x=>r.Next()).Take(empties)) {
                answer[c / answer.GetLength(0), c % answer.GetLength(1)] = 0;
            }
            return answer;
        }
        
        public static void SaveBoard(int[,] board) {
            string bnum = $"{Math.Sqrt(board.GetLength(0))}";
            string path = Path.GetFullPath("./");
            if (path.Substring(path.Length - 21, 20) != "SudokuBoardGenerator") {
                path += @"../../../";
            }
            path += "boards";
            path = Path.GetFullPath(path);
            string[] boards = Directory.GetFiles(path);
            int count = boards.Count(b => b[path.Length+6] == bnum[0]);
            bnum += $".{count+1}";
            path += $"/board{bnum}.txt";
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
