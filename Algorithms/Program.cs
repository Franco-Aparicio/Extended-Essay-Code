using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Algorithms {
    
    class Program {
        
        static void Main(string[] args) {
            int n = 2;
            int bnum = 1;
            int[,] board = LoadBoard(n, bnum);
            if (board == null) {
                Console.WriteLine($"\n\nNo available boards of order {n} in {@"/home/noname/school/EE/ExtendedEssayCode/SudokuBoardGenerator/boards"}");
                return;
            }
            Variable[] vars = new Variable[n * n * n * n];
            List<int[]> defaultAllowed = new List<int[]>();
            int[,] defaultDomain = new int[2, n * n];
            for (int i = 0; i < n * n; i++) {
                defaultDomain[0, i] = i + 1;
                for (int j = 1; j <= n * n; j++) {
                    if (i + 1 == j) continue;
                    defaultAllowed.Add(new int[] {i + 1, j});
                }
            }
            for (int i = 0; i < n * n; i++) {
                for (int j = 0; j < n * n; j++) {
                    int val = board[i, j];
                    vars[i * n * n + j] = new Variable(val, val != 0 ? 
                        new int[,] {{val}, {0}} : defaultDomain, i * n * n + j);
                }
            }
            Dictionary<int[], List<int[]>> allowed = GetAllowed(vars, defaultAllowed, n);
            SetUnitsAndPeers(vars, n);
            // foreach (var unit in vars[1].Units) {
            //     foreach (var cell in unit) {
            //         Console.WriteLine($"({vars[1].Index}, {cell.Index})");
            //     }
            // }
            // var keys = allowed.Keys.ToList();
            // int[] k = new [] {0, 2};
            // var key = keys.Find(x => x.SequenceEqual(k));
            // for (int i = 0; i < vars[2].Domain.GetLength(1); i++) {
            //     Console.WriteLine(!allowed[key].Any(x => x.SequenceEqual(new int[] {3, vars[2].Domain[0, i]})));
            // }
        ForwardChecking FC = new ForwardChecking(allowed);
        //     // FC.FC(vars.Where(x=>x.Value == 0).ToArray());
        //
        vars = FC.FC(vars);
        //     foreach (int[] action in FC.Solution) {
        //         vars[action[0]].Value = action[1];
        //     }
        for (int i = 0; i < n * n; i++) {
            for (int j = 0; j < n * n; j++) {
                board[i, j] = vars[i * n * n + j].Value;
            }
        }
        PrintBoard(board);
        }

        private static Dictionary<int[], List<int[]>> GetAllowed(Variable[] vars, List<int[]> defaultAllowed, int n) {
            var allowed = new Dictionary<int[], List<int[]>>();
            int ii = 0;
            for (int i = 0; i < n * n; i++) {
                int jj = 0;
                for (int j = 0; j < n * n; j++) {
                    int kk = ii + jj + 1;
                    for (int k = j + 1; k < n * n; k++) {
                        int corrector = j % n == n - 1 ? (n - 1) * n : 0;
                        Variable v1 = vars[i * n * n + j];
                        Variable v2 = vars[i * n * n + k];
                        if (!allowed.Keys.ToList().Any(x=>x.SequenceEqual(new [] {v1.Index, v2.Index}))) {
                            if (v1.Domain.GetLength(1) == 1 || v2.Domain.GetLength(1) == 1) {
                                allowed.TryAdd(new int[] {v1.Index, v2.Index}, GetSpecial(v1, v2));
                            }
                            else {
                                allowed.TryAdd(new int[] {v1.Index, v2.Index}, defaultAllowed);
                            }
                        }
                        // // Console.WriteLine($"({i * n * n + j}, {i * n * n + k})");
                        v1 = vars[j * n * n + i];
                        v2 = vars[k * n * n + i];
                        if (!allowed.Keys.ToList().Any(x=>x.SequenceEqual(new [] {v1.Index, v2.Index}))) {
                            if (v1.Domain.GetLength(1) == 1 || v2.Domain.GetLength(1) == 1) {
                                allowed.TryAdd(new int[] {v1.Index, v2.Index}, GetSpecial(v1, v2));
                            }
                            else {
                                allowed.TryAdd(new int[] {v1.Index, v2.Index}, defaultAllowed);
                            }
                        }
                        // // Console.WriteLine($"({j * n * n + i}, {k * n * n + i})");
                        v1 = vars[ii + jj];
                        v2 = vars[kk + corrector];
                        if (!allowed.Keys.ToList().Any(x=>x.SequenceEqual(new [] {v1.Index, v2.Index}))) {
                            if (v1.Domain.GetLength(1) == 1 || v2.Domain.GetLength(1) == 1) {
                                allowed.TryAdd(new int[] {v1.Index, v2.Index}, GetSpecial(v1, v2));
                            }
                            else {
                                allowed.TryAdd(new int[] {v1.Index, v2.Index}, defaultAllowed);
                            }
                        }
                        // // Console.WriteLine($"({ii + jj}, {kk + corrector})");
                        kk = k % n == n - 1 ? kk + n * (n - 1) + 1 : kk + 1;
                    }
                    jj = j % n == n - 1 ? jj + n * (n - 1) + 1 : jj + 1;
                }
                ii = i % n == n - 1 ? ii + n * n * (n - 1) + n: ii + n;
                return allowed;
            }
            return allowed;
        }

        private static void SetUnitsAndPeers(Variable[] vars, int n) {
            for (int i = 0; i < n * n; i++) {
                for (int j = 0; j < n * n; j++) {
                    List<Variable> peers = new List<Variable>();
                    List<Variable>[] units = new List<Variable>[3];
                    List<Variable> row = new List<Variable>();
                    List<Variable> col = new List<Variable>();
                    List<Variable> box = new List<Variable>();
                    Variable v1 = vars[i * n * n + j];
                    int correct = 0;
                    for (int k = 0; k < n * n; k++) {
                        Variable v2 = vars[i * n * n + k];
                        if (i * n * n + j != i * n * n + k) {
                            row.Add(v2);
                            if (peers.All(x => x.Index != v2.Index)) {
                                peers.Add(v2);
                            }
                        }
                        v2 = vars[k * n * n + j];
                        if (i * n * n + j != k * n * n + j) {
                            col.Add(v2);
                            if (peers.All(x => x.Index != v2.Index)) {
                                peers.Add(v2);
                            }
                        }
                        int r = (i - i % n) * n * n;
                        int c = (j - j % n) * (n - 1);
                        correct = k != 0 && k % n == 0 ? n * (n - 1): correct;
                        v2 = vars[r + c + k + correct];
                        if (i * n * n + j != r + c + k + correct) {
                            box.Add(v2);
                            if (peers.All(x => x.Index != v2.Index)) {
                                peers.Add(v2);
                            }
                        }
                    }
                    units[0] = row;
                    units[1] = col;
                    units[2] = box;
                    v1.SetUnitsAndPeers(units, peers);
                }
            }
        }
        
        private static List<int[]> GetSpecial(Variable v1, Variable v2) {
            var product = from val1 in Enumerable.Range(0, v1.Domain.GetLength(1))
                    .Select(x => v1.Domain[0, x])
                from val2 in Enumerable.Range(0, v2.Domain.GetLength(1)).Select(x => v2.Domain[0, x])
                where val1 != val2
                select new {val1, val2};
            return product.Select(x => new int[] {x.val1, x.val2}).ToList();
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
    }
}