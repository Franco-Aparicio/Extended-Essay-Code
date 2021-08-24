using System;

namespace SudokuBoardGenerator {
    class Cell : IComparable<Cell> {
        // Cell coordinates
        public int X;
        public int Y;

        // Containing groups
        public Group Row;
        public Group Column;
        public Group Box;

        // Possible values
        public string PossibleValues = Program.valueList;

        // Display value
        public char Value = 'x';

        // Use this to randomize sort order
        int I = Grid.Rand.Next();
        
        /// Constructor 
        public Cell(int x, int y, Group row, Group column, Group box, int numValues) {
            X = x;
            Y = y;
            Row = row;
            Column = column;
            Box = box;

            // Assign to groups
            row.AddCell(this);
            column.AddCell(this);
            box.AddCell(this);

            // Init possible values
            PossibleValues = PossibleValues.Substring(0, numValues);
        }

        /// Assign a value to this cell while removing it from possible values of related cells 
        public void AssignValue(char value) {
            if (PossibleValues.Length > 0 && PossibleValues.IndexOf(value) != -1) {
                RemoveValueFromGroups(value);

                Value = value;
            }
        }
        
        /// Remove a value from possible values 
        protected void RemoveValue(char value) {
            int index = PossibleValues.IndexOf(value);

            // Remove value if exists in possible values
            if (index != -1) {
                PossibleValues = PossibleValues.Remove(index, 1);
            }
        }

        /// Remove a value from all related group members 
        protected void RemoveValueFromGroups(char value) {
            for (int i = 0; i < Row.Cells.Length; i++) {
                Row.Cells[i].RemoveValue(value);
                Column.Cells[i].RemoveValue(value);
                Box.Cells[i].RemoveValue(value);
            }
        }

        /// Used to sort cells randomly using their assigned RNG value I 
        public int CompareTo(Cell c) {
            if (c == null) return 0;

            return I.CompareTo(c.I);
        }

        /// Regenerate the RNG value 
        public void ReseedRng() {
            I = Grid.Rand.Next();
        }
    }
}