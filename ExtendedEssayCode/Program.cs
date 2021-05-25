using System;

namespace ExtendedEssayCode {

    class Program {

        static void Main(string[] args) {
            Sudoku problem = new Sudoku(3);
            problem.PrintBoard();
        }
    }
}