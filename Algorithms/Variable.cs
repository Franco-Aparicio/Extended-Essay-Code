using System.Collections.Generic;

namespace Algorithms {
    
    public class Variable {

        public int Value;
        public int[,] Domain;
        public int Index;
        public List<Variable> Peers;

        /// Constructor 
        public Variable(int value, int[,] domain, int index) {
            Value = value;
            Domain = domain;
            Index = index;
        }

        /// Constructor 
        public Variable(int value) {
            Value = value;
        }

        /// Sets the list of Peers (constraining Variables) 
        public void SetPeers(List<Variable> peers) {
            Peers = peers;
        }

        /// Returns the number of unmarked domain values remaining 
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