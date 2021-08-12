using System.Collections.Generic;

namespace Algorithms {
    
    public class Variable {

        public int Value;
        public int[,] Domain;
        public int Index;
        public List<Variable>[] Units = new List<Variable>[3];
        public List<Variable> Peers;

        public Variable(int value, int[,] domain, int index) {
            Value = value;
            Domain = domain;
            Index = index;
        }

        public void SetUnitsAndPeers(List<Variable>[] units, List<Variable> peers) {
            Units = units;
            Peers = peers;
        }

        public int GetLeft() {
            int count = 0;
            for (int i = 0; i < Domain.GetLength(1); i++) {
                if (Domain[1, i] == -1) {
                    count++;
                }
            }
            return count;
        }
    }
}