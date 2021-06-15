using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Algorithms {
    
    class Program {
        
        static void Main(string[] args) {
            int n = 3;
            int bnum = 1;
            int[,] board = LoadBoard(n, bnum);
            if (board == null) {
                Console.WriteLine($"\n\nNo available boards of order {n} in {@"/home/noname/school/EE/ExtendedEssayCode/SudokuBoardGenerator/boards"}");
                return;
            }
            Variable[] vars = new Variable[n * n * n * n];
            List<Variable[]> constraints = new List<Variable[]>();
            for (int i = 0; i < n * n; i++) {
                for (int j = 0; j < n * n; j++) {
                    int val = board[i, j];
                    vars[i*n*n+j] = new Variable(val, val != 0 ? new int[] {-1, val} : Enumerable.Range(1, n * n).ToArray());
                }
            }
            for (int i = 0; i < n * n; i++) {
                for (int j = 0; j < n * n; j++) {
                    for (int k = 0; k < n * n; k++) {
                        if (k > j) {
                            // constraints.Add(new Variable[] {vars[i * n * n + j], vars[i * n * n + k]});
                            // Console.WriteLine($"({i * n * n + j}, {i * n * n + k})");
                        }
                        // constraints.Add(new Variable[] {vars[i], vars[j * n * n + i]});
                        // constraints.Add(new Variable[] {vars[i + j], vars[k * n * n + i]});
                        Console.WriteLine($"({i + j * n * n}, {k * n * n + j})");
                    }
                }
            }
            // for (int i = 0; i < vars.Length; i++) {
            //     Console.Write($"{vars[i].Value} ");
            //     if ((i + 1) % (n * n) == 0) {
            //         Console.WriteLine();
            //     }
            // }
            // for (int i = 0; i < n * n * n * n * 3 * (n * n - 1); i++) {
            //     for (int j = 0; j < 2; j++) {
            //         constraints[i, j] = vars[]
            //     }
            // }
        }

        private static int[,] LoadBoard(int n, int bnum) {
            string path = Path.GetFullPath("./");
            if (path.Substring(path.Length - 11, 10) != "Algorithms") {
                path += @"../../../";
            }
            path += @"../SudokuBoardGenerator/boards";
            path = Path.GetFullPath(path);
            string[] boards = Directory.GetFiles(path);
            string bname = "";
            try {
                bname = boards.Single(b => b.Substring(path.Length + 6, 3) == $"{n}.{bnum}");
            }
            catch (Exception e) {
                if (e is ArgumentNullException or InvalidOperationException) {
                    return null;
                }
                throw;
            }
            int[,] board = new int[n*n, n*n];
            using (StreamReader reader = new StreamReader(bname)) {
                string line = reader.ReadLine();
                for (int i = 0; i < n*n; i++) {
                    string[] nums = line.Split(' ').ToArray();
                    for (int j = 0; j < n*n; j++) {
                        board[i, j] = Int32.Parse(nums[j]);
                    }
                    line = reader.ReadLine();
                }
            }
            return board;
        }
    }
}