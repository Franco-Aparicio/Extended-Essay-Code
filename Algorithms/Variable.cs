using System.Collections.Generic;

namespace Algorithms {
    
    public class Variable {

        public int Value;
        public int[,] Domain;
        public int Index;
        // public bool Assigned = false;
        public List<Variable>[] Units = new List<Variable>[3];

        public Variable(int value, int[,] domain, int index) {
            Value = value;
            Domain = domain;
            Index = index;
        }

        public void SetUnits(List<Variable>[] units) {
            Units = units;
        }
    }
}