using System;
using System.Linq;
using System.Text;

namespace SudokuBoardGenerator {
    class Grid {
        // Static Grid Instance 

        string PossibleValues = Program.valueList; // Domain of possible values 
        public int BoxSideLength; // Board order 

        public static Random Rand = new Random();

        public Group[] Rows;
        public Group[] Columns;
        public Group[] Boxes;
        public Cell[] Cells;
        public int[,] Board;

        protected static bool CompletedGrid = false;

        /// Constructor 
        public Grid (int boxSideLength) {
            int sideLength = boxSideLength * boxSideLength;

            BoxSideLength = boxSideLength;
            PossibleValues = PossibleValues.Substring(0, sideLength);

            Rows = new Group[sideLength];
            Columns = new Group[sideLength];
            Boxes = new Group[sideLength];

            // Instantiate the groups
            for (int i=0; i<sideLength; i++) {
                Rows[i] = new Group(sideLength);
                Columns[i] = new Group(sideLength);
                Boxes[i] = new Group(sideLength);
            }

            Cells = new Cell[sideLength * sideLength];

            // Instantiate the cells
            for (int y=0; y<sideLength; y++) {
                for (int x=0; x<sideLength; x++) {
                    int boxIndex = (x / boxSideLength) + (y / boxSideLength) * boxSideLength;
                    Cells[x + y * sideLength] = new Cell(x, y, Rows[y], Columns[x], Boxes[boxIndex], sideLength);
                }
            }

            // Start building the grid and assign the cell values
            while (!PopulateChar(PossibleValues[0])) {
                // Reset Rng if complete failure occurs
                foreach (Cell cell in Cells) {
                    cell.ReseedRng();
                }
            }

            // First completed grid
            if (!CompletedGrid) {
                CompletedGrid = true;
                Board = MakeUseful(); // Make board in terms of integers 
            }
        }

        /// Used to recursively feed values into the AssignValues method 
        protected bool PopulateChar(char value) {
            // Check for completed grid, end processing
            if (CompletedGrid) {
                return true;
            }
            return AssignValues(Boxes[0], value);
        }

        /// Used to recursively assign the given value to a cell in each box group
        protected bool AssignValues(Group box, char value) {

            var candidates = box.GetCandidates(value);

            if (candidates.Count > 0) {

                foreach (Cell cell in candidates) {
                    // Check for completed grid, end processing
                    if (CompletedGrid) {
                        return true;
                    }

                    // Save current state of grid
                    State[] states = new State[Cells.Length];
                    for (int i=0; i<Cells.Length; i++) {
                        states[i] = new State(Cells[i].Value, Cells[i].PossibleValues);
                    }

                    cell.AssignValue(value);

                    // Determine if this cell will cause the next box to error
                    int index = Array.IndexOf(Boxes, box);
                    int gridRowIndex = index / BoxSideLength;
                    int gridColIndex = index % BoxSideLength;

                    bool causesError = false;
                    for (int i = index + 1; i < Boxes.Length; i++) {
                        if (gridRowIndex != i / BoxSideLength || gridColIndex != i % BoxSideLength) continue;

                        bool hasFreeCell = false;
                        foreach (Cell testCell in Boxes[i].Cells) {
                            if (testCell.PossibleValues.Contains(value)) {
                                hasFreeCell = true;
                                break;
                            }
                        }
                        if (!hasFreeCell) {
                            causesError = true;
                            break;
                        }
                    }

                    // Move on to next box if no error
                    if (!causesError) {
                        int nextBoxIndex = index + 1;

                        if (nextBoxIndex == Boxes.Length) {
                            // Start assigning next character
                            int indexOfNextChar = PossibleValues.IndexOf(value) + 1;

                            // Check for grid completion
                            if (indexOfNextChar == PossibleValues.Length) return true;

                            // Move on to next char
                            if (PopulateChar(PossibleValues[indexOfNextChar])) return true;
                        }
                        else {
                            // Recurse through next box
                            if (AssignValues(Boxes[nextBoxIndex], value)) return true;
                        }
                    }

                    // Undo changes made in this recursion layer
                    for (int i = 0; i < Cells.Length; i++) {
                        Cells[i].Value = states[i].Value;
                        Cells[i].PossibleValues = states[i].PossibleValues;
                    }
                }
            }
            return false; // No viable options, go back to previous box or previous character
        }

        /// Makes the board in terms of integer values 
        private int[,] MakeUseful() {
            int[,] board = new int[Rows.Length, Rows.Length];
            for (int i = 0; i < Rows.Length; i++) {
                for (int j = 0; j < Rows[i].Cells.Length; j++) {
                    char val = Rows[i].Cells[j].Value;
                    if (Enumerable.Range(49, 9).Contains(val)) {
                        board[i, j] = val - 48;
                        continue;
                    }
                    board[i, j] = val - 55;
                }
            }
            return board;
        }
        
        /// Displays board on console 
        public static void PrintBoard(int[,] b) {
            int n = (int) Math.Sqrt((double) b.GetLength(0));
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
                Console.WriteLine();
            }
            Console.WriteLine(new String('-', b.GetLength(0)*3+2*n));
        }
    }
}