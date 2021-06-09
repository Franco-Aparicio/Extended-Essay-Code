namespace ExtendedEssayCode {
    struct State {
        public char Value;
        public string PossibleValues;

        public State(char value, string possibleValues)
        {
            Value = value;
            PossibleValues = possibleValues;
        }
    }
}