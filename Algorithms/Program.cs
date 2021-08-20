using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Algorithms {
    
    class Program {
        
        static void Main(string[] args) {
            bool fc = args[0] == "f";
            int n = int.Parse(args[1]);
            int bnum = int.Parse(args[2]);
            int trial = int.Parse(args[3]);
            int[,] board = LoadBoard(n, bnum);
            if (board == null) return;
            Variable[] vars = new Variable[n * n * n * n];
            int[,] defaultDomain = new int[2, n * n];
            for (int i = 0; i < n * n; i++) {
                defaultDomain[0, i] = i + 1;
                defaultDomain[1, i] = -1;
            }
            for (int i = 0; i < n * n; i++) {
                for (int j = 0; j < n * n; j++) {
                    int val = board[i, j];
                    vars[i * n * n + j] = new Variable(val, val != 0 ? 
                        new int[,] {{val}, {-1}} : (int[,]) defaultDomain.Clone(), i * n * n + j);
                }
            }
            SetPeers(vars, n);
            List<int[]> solution = null;
            int nodes = 0;
            int checks = 0;
            int backs = 0;
            double time = 0;
            if (fc) {
                ForwardChecking FC = new ForwardChecking();
                FC.FC(vars);
                solution = FC.Solution;
                nodes = FC.Nodes;
                checks = FC.Checks;
                backs = FC.Backs;
                time = FC.Time;
            }
            else {
                MaintainingArcConsistency MAC = new MaintainingArcConsistency();
                MAC.MAC(vars);
                solution = MAC.Solution;
                nodes = MAC.Nodes;
                checks = MAC.Checks;
                backs = MAC.Backs;
                time = MAC.Time;
            }
            foreach (int[] action in solution) {
                vars[action[0]].Value = action[1];
            }
            for (int i = 0; i < n * n; i++) {
                for (int j = 0; j < n * n; j++) {
                    board[i, j] = vars[i * n * n + j].Value;
                }
            }
            // PrintBoard(board);
            SaveData(args, nodes, checks, backs, time);
        }
        
        private static void SetPeers(Variable[] vars, int n) {
            int ii = 0;
            for (int i = 0; i < n * n; i++) {
                int corrector = 0;
                int jj = 0;
                for (int j = 0; j < n * n; j++) {
                    int kk = i / n * n * n * n;
                    List<Variable> peers = new List<Variable>();
                    Variable v1 = vars[i * n * n + j];
                    for (int k = 0; k < n * n; k++) {
                        Variable v2 = vars[i * n * n + k];
                        if (v1.Index != v2.Index) {
                            if (peers.All(x => x.Index != v2.Index)) {
                                peers.Add(v2);
                            }
                        }
                        v2 = vars[k * n * n + j];
                        if (v1.Index != v2.Index) {
                            if (peers.All(x => x.Index != v2.Index)) {
                                peers.Add(v2);
                            }
                        }
                        corrector = j % n == 0 && j != 0 ? n * (j / n): corrector;
                        v2 = vars[kk + corrector];
                        if (v1.Index != v2.Index) {
                            if (peers.All(x => x.Index != v2.Index)) {
                                peers.Add(v2);
                            }
                        }
                        kk = k % n == n - 1 ? kk + n * (n - 1) + 1 : kk + 1;
                    }
                    v1.SetPeers(peers);
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
            path += @"../SudokuBoardGenerator/boards/";
            path = Path.GetFullPath(path);
            string[] boards = Directory.GetFiles(path);
            string bname = $"board{n}.{bnum}.txt";
            // try {
            //     bname = boards.Single(b => b.Substring(path.Length + 6, 3) == $"{n}.{bnum}");
            // }
            // catch (Exception e) {
            //     if (e is ArgumentNullException or InvalidOperationException) {
            //         Console.WriteLine($"\n\nNo available boards of order {n} in {path}");
            //         return null;
            //     }
            //     throw;
            // }
            int[,] board = new int[n*n, n*n];
            using (StreamReader reader = new StreamReader(path + bname)) {
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

        private static void PrintBoard(int[,] b) {
            int n = (int) Math.Sqrt((double) b.GetLength(0));
            // Console.WriteLine("\n\x1B[4m" + new String(' ', n * n + n + 1) + "\x1B[0m");
            // Console.WriteLine(new String('-', b.GetLength(0)*3+5));
            for (int i = 0; i < b.GetLength(0); i++) {
                if (i % n == 0) {
                    Console.WriteLine(new String('-', b.GetLength(0)*3+2*n));
                }
                Console.Write("|");
                for (int j = 0; j < b.GetLength(1); j++) {
                    Console.Write("{0, -3}", b[i, j]);
                    if ((j + 1) % n == 0) {
                        Console.Write("{0, -2}", "|");
                    }
                }
                // Console.WriteLine("\x1B[0m");
                Console.WriteLine();
            }
            Console.WriteLine(new String('-', b.GetLength(0)*3+2*n));
        }

        private static async void SaveData(string[] args, int nodes, int checks, int backs, double time) {
            // Console.WriteLine($"Nodes visited: {nodes}");
            // Console.WriteLine($"Constraint checks: {checks}");
            // Console.WriteLine($"Backtracks: {backs}");
            // Console.WriteLine($"Time taken (ms): {time}");
            string path = Path.GetFullPath("./");
            if (path.Substring(path.Length - 11, 10) != "Algorithms") path += @"../../../";
            path += @$"../Algorithms/data/raw/{args[0]}.{args[1]}.{args[2]}.{args[3]}.txt";
            path = Path.GetFullPath(path);
            string[] lines = new [] {$"{nodes}", $"{checks}", $"{backs}", $"{time}"};
            Console.WriteLine($"Saving data in: {path}");
            await File.WriteAllLinesAsync(path, lines);
        }
    }
}
