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
                if (Domain[1, i] != 0) {
                    count++;
                }
            }
            return count;
        }

        public void RemoveDomain(int n) {
            int[,] newDom = new int[2,Domain.GetLength(1)-1];
            for (int i = 0; i < Domain.GetLength(1); i++) {
                if (i == n) continue;
                newDom[0, i] = Domain[0, i];
                newDom[1, i] = Domain[1, i];
            }
            Domain = newDom;
        }
    }
}