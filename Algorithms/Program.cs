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
                    vars[i * n * n + j] = new Variable(val, val != 0 ? 
                        new int[] {val} : new int[] {-1}
                            .Concat(Enumerable.Range(1, n * n)).ToArray());
                }
            }
            int ii = 0;
            for (int i = 0; i < n * n; i++) {
                int jj = 0;
                for (int j = 0; j < n * n; j++) {
                    int kk = ii + jj + 1;
                    for (int k = j + 1; k < n * n; k++) {
                        int temp = j % n == n - 1 ? (n - 1) * n : 0;
                        constraints.Add(new Variable[] {vars[i * n * n + j], vars[i * n * n + k]});
                        // Console.WriteLine($"({i * n * n + j}, {i * n * n + k})");
                        constraints.Add(new Variable[] {vars[j * n * n + i], vars[k * n * n + i]});
                        // Console.WriteLine($"({j * n * n + i}, {k * n * n + i})");
                        constraints.Add(new Variable[] {vars[ii + jj], vars[kk + temp]});
                        // Console.WriteLine($"({ii + jj}, {kk + temp})");
                        kk = k % n == n - 1 ? kk + n * (n - 1) + 1 : kk + 1;
                    }
                    jj = j % n == n - 1 ? jj + n * (n - 1) + 1 : jj + 1;
                }
                ii = i % n == n - 1 ? ii + n * n * (n - 1) + n: ii + n;
            }
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