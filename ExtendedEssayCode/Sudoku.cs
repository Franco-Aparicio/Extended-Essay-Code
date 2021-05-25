using System;
using System.Linq;
using System.Collections.Generic;

namespace ExtendedEssayCode {
    
    public class Sudoku {

        private readonly int n;
        private readonly int side;
        private int[][] board;
        private Random r = new Random();
        
        public Sudoku(int n) {
            this.n = n;
            side = n * n;
            board = new int[side][]; 
           BoardGen();
        }

        private void BoardGen() {
            int[] baseNums = Enumerable.Range(0, n).ToArray();
            List<int> rows = new List<int>();
            List<int> cols = new List<int>();
            List<List<int>> b = new List<List<int>>();
            foreach (int r in Shuffle(baseNums)) {
                foreach (int i in Shuffle(baseNums)) {
                    rows.Add(i * n + r);
                }
            }
            foreach (int c in Shuffle(baseNums)) {
                foreach (int i in Shuffle(baseNums)) {
                    cols.Add(i * n + c);
                }
            }
            int[] nums = Shuffle(Enumerable.Range(1, side).ToArray());
            foreach (int r in rows) {
                List<int> temp = new List<int>();
                foreach (int c in cols) {
                    temp.Add(nums[Pattern(r, c)]);
                }
                b.Add(temp);
            }
            board = b.Select(x=>x.ToArray()).ToArray();
            PrintBoard();
            int square = side * side;
            int empties = square * 3 / 4;
            foreach (int cell in Shuffle(Enumerable.Range(0, square-1).ToArray()).Take(empties)) {
                board[cell / side][cell % side] = 0;
            }
        }

        private int[] Shuffle(int[] arr) {
            return arr.OrderBy(x => r.Next()).ToArray();
        }

        // Used for baseline pattern for valid solution
        private int Pattern(int r, int c) {
            return (n * (r % n) + r / n + c) % side;
        }
        
        public void PrintBoard() {
            Console.WriteLine("\n\x1B[4m" + new String(' ', n * n + n + 1) + "\x1B[0m");
            for (int i = 0; i < board.Length; i++) {
                if ((i + 1) % n == 0) {
                    Console.Write("\x1B[4m");
                }
                Console.Write("|");
                for (int j = 0; j < board[0].Length; j++) {
                    Console.Write(board[i][j]);
                    if ((j + 1) % n == 0) {
                        Console.Write("|");
                    }
                }
                Console.WriteLine("\x1B[0m");
            }
        }
    }
}