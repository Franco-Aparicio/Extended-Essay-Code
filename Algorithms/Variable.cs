using System.Collections.Generic;

namespace Algorithms {
    
    public class Variable {

        public int Value;
        public int[,] Domain;
        public int Index;
        public List<Variable> Peers;

        public Variable(int value, int[,] domain, int index) {
            Value = value;
            Domain = domain;
            Index = index;
        }

        public Variable(int value) {
            Value = value;
        }

        public void SetPeers(List<Variable> peers) {
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