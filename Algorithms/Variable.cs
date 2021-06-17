namespace Algorithms {
    
    public class Variable {

        public int Value;
        public int[,] Domain;
        public int Index;

        public Variable(int value, int[,] domain, int index) {
            Value = value;
            Domain = domain;
            Index = index;
        }
    }
}