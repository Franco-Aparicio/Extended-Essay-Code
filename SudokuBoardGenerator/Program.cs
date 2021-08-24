using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SudokuBoardGenerator {
    class Program {
        // Characters used in the grid
        public const string valueList = "123456789ABCDEFGHIJKLMNOP";

        static void Main(string[] args) {
            int n = int.Parse(args[0]); // Get argument of board order 
            int[,] answer = null;
            var tasks = new[] { // Start generating grid 
                Task.Factory.StartNew(() => new Grid(n).Board),
                Task.Factory.StartNew(() => new Grid(n).Board),
                Task.Factory.StartNew(() => new Grid(n).Board),
                Task.Factory.StartNew(() => new Grid(n).Board)};
            var things = Task.WhenAll(tasks);
            foreach (var res in things.Result) {  // Get resulting board from the task that is not null 
                if (res != null) {
                    answer = (int[,]) res.Clone();
                }
            }
            Grid.PrintBoard(answer); // Display completed grid 
            int[,] board = LeaveClues(answer); // Remove values to have a puzzle 
            Grid.PrintBoard(board); // Display puzzle 
            SaveBoard(board); // Save puzzle to file 
        }
        
        /// Removes values to leave only clues 
        public static int[,] LeaveClues(int[,] answer) {
            Random r = new Random();
            int squares = answer.GetLength(0)*answer.GetLength(1);
            int n = (int) Math.Sqrt(answer.GetLength(0));
            // Determine how many values to remove depending on the board order 
            int empties = n == 2 ? 10 : n == 3 ? 50 : n == 4 ? 130 : 280;
            foreach (int c in Enumerable.Range(0, squares).OrderBy(x=>r.Next()).Take(empties)) {
                answer[c / answer.GetLength(0), c % answer.GetLength(1)] = 0;
            }
            return answer;
        }
        
        /// Saves puzzle to .txt file 
        public static void SaveBoard(int[,] board) {
            string bnum = $"{Math.Sqrt(board.GetLength(0))}";
            string path = Path.GetFullPath("./");
            // Ensures the .txt file is being saved in the correct directory
            if (path.Substring(path.Length - 21, 20) != "SudokuBoardGenerator") {
                path += @"../../../";
            }
            path += "boards";
            path = Path.GetFullPath(path);
            string[] boards = Directory.GetFiles(path);
            // Gets the current number of boards of the desired order  
            int count = boards.Count(b => b[path.Length+6] == bnum[0]);
            bnum += $".{count+1}"; // Ensures writing new file by increasing the count by 1
            path += $"/board{bnum}.txt";
            // Writes each entry in the board separated by spaces and rows into the file
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
