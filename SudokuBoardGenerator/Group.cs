using System.Collections.Generic;

namespace SudokuBoardGenerator {
    class Group {
        public Cell[] Cells;

        protected int Index = 0;

        /// Constructor 
        public Group (int numCells) {
            Cells = new Cell[numCells];
        }

        /// Add a cell to the group 
        public void AddCell (Cell cell) {
            Cells[Index++] = cell;
        }

        /// Get a sorted set of all cells that can potential have the given value 
        public SortedSet<Cell> GetCandidates (char value) {
            SortedSet<Cell> candidates = new SortedSet<Cell>();

            // Add eligible cells
            foreach (Cell cell in Cells) {
                if (cell.Value == 'x' && cell.PossibleValues.Contains(value)) {
                    candidates.Add(cell);
                }
            }

            return candidates;
        }
    }
}