using System;
using System.Linq;
using System.Collections.Generic;

namespace ExtendedEssayCode {
    
    public class Sudoku {

        private readonly int n;
        private readonly int side;
        private int[,] board;
        private int[,] solution;
        private Random r = new Random();
        
        public Sudoku(int n) {
            this.n = n;
            side = n * n;
            board = new int[side, side];
            solution = new int[side, side];
           MakeCompleted();
           PrintBoard(solution);
           // board = MakeCompleted().Select(x=>x.ToArray()).ToArray();
        }

        private void MakeCompleted() {
            int[] nums = Enumerable.Range(1, side).ToArray();
            while (true) {
                try {
                    List<List<int>> rows = Enumerable.Range(0, side).Select(x=>nums.ToList()).ToList();
                    List<List<int>> cols = Enumerable.Range(0, side).Select(x=>nums.ToList()).ToList();
                    List<List<int>> squares = Enumerable.Range(0, side).Select(x=>nums.ToList()).ToList();
                    for (int i = 0; i < side; i++) {
                        for (int j = 0; j < side; j++) {
                            List<int> choices = rows[i].Intersect(cols[j]).Intersect(squares[i / n * n + j / n]).ToList();
                            int choice = choices[r.Next(choices.Count)];
                            solution[i, j] = choice;
                            rows[i].Remove(choice);
                            cols[j].Remove(choice);
                            squares[i / n * n + j / n].Remove(choice);
                        }
                    }
                    return;
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }
        
        private int[] Shuffle(int[] arr) {
            return arr.OrderBy(x => r.Next()).ToArray();
        }
        
        public void PrintBoard(int[,] b=null) {
            if (b == null) {
                b = board;
            }
            Console.WriteLine("\n\x1B[4m" + new String(' ', n * n + n + 1) + "\x1B[0m");
            for (int i = 0; i < b.GetLength(0); i++) {
                if ((i + 1) % n == 0) {
                    Console.Write("\x1B[4m");
                }
                Console.Write("|");
                for (int j = 0; j < b.GetLength(1); j++) {
                    Console.Write(b[i, j]);
                    if ((j + 1) % n == 0) {
                        Console.Write("|");
                    }
                }
                Console.WriteLine("\x1B[0m");
            }
        }
    }
}