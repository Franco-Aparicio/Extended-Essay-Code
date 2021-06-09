using System;
using System.Linq;
using System.Text;

namespace ExtendedEssayCode {
    class Grid {
        //static Grid Instance;

        string PossibleValues = Program.valueList;
        public int BoxSideLength;

        public static Random Rand = new Random();

        public Group[] Rows;
        public Group[] Columns;
        public Group[] Boxes;
        public Cell[] Cells;
        public int[,] Board;

        protected static bool CompletedGrid = false;

        /**
         * Constructor
         */
        public Grid (int boxSideLength) {
            int sideLength = boxSideLength * boxSideLength;

            BoxSideLength = boxSideLength;
            PossibleValues = PossibleValues.Substring(0, sideLength);

            Rows = new Group[sideLength];
            Columns = new Group[sideLength];
            Boxes = new Group[sideLength];

            // instantiate the groups
            for (int i=0; i<sideLength; i++) {
                Rows[i] = new Group(sideLength);
                Columns[i] = new Group(sideLength);
                Boxes[i] = new Group(sideLength);
            }

            Cells = new Cell[sideLength * sideLength];

            // instantiate the cells
            for (int y=0; y<sideLength; y++) {
                for (int x=0; x<sideLength; x++) {
                    int boxIndex = (x / boxSideLength) + (y / boxSideLength) * boxSideLength;

                    Cells[x + y * sideLength] = new Cell(x, y, Rows[y], Columns[x], Boxes[boxIndex], sideLength);
                }
            }

            // start building the grid
            // Assign the cell values
            while (!PopulateChar(PossibleValues[0])) {
                // reset Rng if complete failure occurs
                foreach (Cell cell in Cells) {
                    cell.ReseedRng();
                }
            }

            // first completed grid
            if (!CompletedGrid) {
                CompletedGrid = true;
                // Draw();
                Board = MakeUseful();
                // PrintBoard(Board);
            }
        }

        /** 
         * Used to recursively feed values into the AssignValues method
         */
        protected bool PopulateChar(char value) {
            //Console.SetCursorPosition(0, 0);
            //Draw();

            // check for completed grid, end processing
            if (CompletedGrid) {
                return true;
            }

            return AssignValues(Boxes[0], value);
        }

        /**
         * Used to recursively assign the given value to a cell in each box group
         */
        protected bool AssignValues(Group box, char value) {

            var candidates = box.GetCandidates(value);

            if (candidates.Count > 0) {

                foreach (Cell cell in candidates) {
                    // check for completed grid, end processing
                    if (CompletedGrid) {
                        return true;
                    }

                    // save current state of grid
                    State[] states = new State[Cells.Length];
                    for (int i=0; i<Cells.Length; i++) {
                        states[i] = new State(Cells[i].Value, Cells[i].PossibleValues);
                    }

                    cell.AssignValue(value);

                    // determine if this cell will cause the next box to error
                    int index = Array.IndexOf(Boxes, box);
                    int gridRowIndex = index / BoxSideLength;
                    int gridColIndex = index % BoxSideLength;

                    bool causesError = false;
                    for (int i = index + 1; i < Boxes.Length; i++) {
                        if (/*i > BoxSideLength * 2 &&*/ gridRowIndex != i / BoxSideLength || gridColIndex != i % BoxSideLength) continue;

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

                    // move on to next box if no error
                    if (!causesError) {
                        int nextBoxIndex = index + 1;

                        if (nextBoxIndex == Boxes.Length) {
                            // start assigning next character
                            int indexOfNextChar = PossibleValues.IndexOf(value) + 1;

                            // Check for grid completion
                            if (indexOfNextChar == PossibleValues.Length) return true;

                            // move on to next char
                            if (PopulateChar(PossibleValues[indexOfNextChar])) return true;
                        }
                        else {
                            // recurse through next box
                            if (AssignValues(Boxes[nextBoxIndex], value)) return true;
                        }
                    }

                    // undo changes made in this recursion layer
                    for (int i = 0; i < Cells.Length; i++) {
                        Cells[i].Value = states[i].Value;
                        Cells[i].PossibleValues = states[i].PossibleValues;
                    }
                }
            }
            return false; // no viable options, go back to previous box or previous character
        }

        /**
         * Output the grid to console
         */
        public void Draw() {
            int rowCounter = 0;

            foreach (Group row in Rows) {
                StringBuilder rowString = new StringBuilder();
                foreach (Cell cell in row.Cells) {
                    rowString.Append(cell.Value);
                    rowString.Append(' ', 2);
                }

                rowCounter++;
                if (rowCounter == BoxSideLength) {
                    rowCounter = 0;
                    Console.WriteLine(rowString.Append('\n').Append('-', Rows.Length*3));
                }
                else
                    Console.WriteLine(rowString + "\n");
            }
        }

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
        
        public static void PrintBoard(int[,] b) {
            int n = (int) Math.Sqrt((double) b.GetLength(0));
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