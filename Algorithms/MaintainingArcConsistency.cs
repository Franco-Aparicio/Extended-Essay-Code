using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Algorithms {
    public class MaintainingArcConsistency {
        
        // Initialize data variables and stopwatch 
        public int Nodes = 0;
        public int Checks = 0;
        public int Backs = 0;
        public double Time = 0;
        private Stopwatch Watch = Stopwatch.StartNew();
        // Initialize data structures 
        public List<int[]> Solution;
        private List<Variable[]> DeletionStream;
        private Dictionary<string, List<int>> Unchecked = new ();
        private Dictionary<string, List<int>> Support = new ();
        private Dictionary<string, List<Variable[]>> MinSupport = new();
        private Random r = new Random();

        /// MAC7 method outlined in essay  
        public bool MAC(Variable[] vars) {
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    var.Domain[1, i] = -1; // Unmark every domain value 
                    // Initialize each entry in MinS 
                    MinSupport[GetKey(var.Index, i)] = new List<Variable[]>();
                    foreach (Variable v in var.Peers) {
                        // Initialize each entry in S and UnChecked 
                        Support[GetKey(var.Index, v.Index, i)] = new List<int>();
                        Unchecked[GetKey(var.Index, v.Index, i)] = new List<int> {0};
                        if (v.Domain.GetLength(1) > 1) {
                            for (int j = 1; j < v.Domain.GetLength(1); j++) {
                                // Sets up UnChecked 
                                Unchecked[GetKey(var.Index, v.Index, i)].Add(j);
                            }
                        }
                    }
                }
            }
            Solution = new List<int[]>(); // Initialize solution list 
            DeletionStream = new List<Variable[]>(); // Initialize DeletionStream 
            // Remove any values that do not support other values 
            foreach (Variable var in vars) {
                foreach (Variable v in var.Peers) {
                    for (int i = 0; i < var.Domain.GetLength(1); i++) {
                        if (var.Domain[1, i] != -1) continue;
                        if (!SeekSupport(var, v, i)) {
                            var.Domain[1, i] = 0;
                            DeletionStream.Add(new [] {var, new Variable(i)});
                            if (DWO(var)) return false;
                        }
                    }
                }
            }
            bool answer = PropagateAC(vars, 0) && Search(vars, 1);
            Time = Watch.ElapsedMilliseconds; // Records the time taken to return a solution 
            return answer;
        }

        /// Applies constraint propagation through the contents of DeletionStream  
        private bool PropagateAC(Variable[] vars, int level) {
            while (DeletionStream.Count > 0) {
                // Select and remove del from DeletionStream 
                Checks++;
                Variable[] del = DeletionStream[0];
                DeletionStream.Remove(del);
                int skip = 0;
                // Iterate through every unmarked pair in MinS and remove them and add marked to the deletion stream 
                foreach (int p in GetUnmarked(del[0], del[1].Value)) {
                    Variable[] pair = MinSupport[GetKey(del[0].Index, del[1].Value)][p - skip];
                    if (pair[0].Domain[1, pair[1].Value] != -1 || vars.All(x => x.Index != pair[0].Index)) continue;
                    if (SeekSupport(pair[0], del[0], pair[1].Value)) {
                        MinSupport[GetKey(del[0].Index, del[1].Value)].Remove(pair);
                        skip++;
                    }
                    else {
                        pair[0].Domain[1, pair[1].Value] = level;
                        DeletionStream.Add(pair);
                        if (DWO(pair[0])) return false;
                    }
                }
            }
            return true;
        }

        /// Main search method for MAC  
        private bool Search(Variable[] vars, int level) {
            Variable var = SelectVar(vars); // Selects next Variable to instantiate 
            // Iterates through each of the Variable's unmarked domain values 
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] != -1) continue;
                var item = new [] {var.Index, var.Domain[0, i]};
                Solution.Add(item); // Add instantiation to Solution 
                Nodes++;
                if (vars.Length == 1) return true; // If all Variables have been instantiated, return true
                // Otherwise mark all other values in this domain (to instantiate the Variable)
                for (int j = 0; j < var.Domain.GetLength(1); j++) {
                    if (var.Domain[1, j] != -1 || j == i) continue;
                    var.Domain[1, j] = level;
                    DeletionStream.Add(new [] {var, new Variable(j)});
                }
                // With var instantiated, recursively search through the rest of the Variables 
                Variable[] temp = vars.Where(x => x.Index != var.Index).ToArray();
                if (PropagateAC(temp, level) && Search(temp, level + 1)) return true;
                // If a dead end has been reached, remove instantiation from solution and restore previous state 
                Solution.Remove(item);
                vars = Restore(vars, level);
            }
            return false;
        }

        /// Looks for supporting values in S and adds to S and MinS from UnChecked if none found  
        private bool SeekSupport(Variable var, Variable v, int valIndex) {
            foreach (int val in Support[GetKey(var.Index, v.Index, valIndex)]) {
                if (v.Domain[1, val] != -1) continue;
                MinSupport[GetKey(v.Index, val)].Add(new [] {var, new Variable(valIndex)});
                return true;
            }
            int skip = 0;
            foreach (int value in GetUnmarked(var, v, valIndex)) {
                int val = Unchecked[GetKey(var.Index, v.Index, valIndex)][value - skip];
                if (v.Domain[1, val] != -1) continue;
                Unchecked[GetKey(var.Index, v.Index, valIndex)].Remove(val);
                skip++;
                if (var.Domain[0, valIndex] == v.Domain[0, val]) continue;
                Support[GetKey(v.Index, var.Index, val)].Add(valIndex);
                Support[GetKey(var.Index, v.Index, valIndex)].Add(val);
                MinSupport[GetKey(v.Index, val)].Add(new [] {var, new Variable(valIndex)});
                return true;
            }
            return false;
        }
        
        /// Uses the minimum remaining values heuristic to select the next variable for instantiation 
        private Variable SelectVar(Variable[] vars) {
            Variable var = vars.OrderBy(x => x.GetLeft()).ThenBy(x=>r.Next()).First();
            return var;
        }

        /// Checks for a Domain Wipe-Out (no remaining unmarked domain values) due to constraint propagation 
        private bool DWO(Variable var) {
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] == -1) {
                    return false;
                }
            }
            return true;
        }

        /// Restores domains to previous state
        private Variable[] Restore(Variable[] vars, int level) {
            Backs++;
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    if (var.Domain[1, i] == level) {
                        var.Domain[1, i] = -1;
                    }
                }
            }
            return vars;
        }

        /// Returns valid string key for S and UnChecked 
        private string GetKey(int var1, int var2, int val) {
            return $"{var1},{var2},{val}";
        }
        
        /// Returns valid string key for MinS 
        private string GetKey(int var1, int val) {
            return $"{var1},{val}";
        }

        /// Returns list of unmarked values from a given entry of UnChecked  
        private List<int> GetUnmarked(Variable var, Variable v, int val) {
            List<int> unmarked = new List<int>();
            int count = 0;
            foreach (int value in Unchecked[GetKey(var.Index, v.Index, val)]) {
                if (v.Domain[1, value] == -1) unmarked.Add(count);
                count++;
            }
            return unmarked;
        }
        
        /// Returns list of unmarked values from a given entry of MinS 
        private List<int> GetUnmarked(Variable var, int val) {
            List<int> unmarked = new List<int>();
            int count = 0;
            foreach (Variable[] pair in MinSupport[GetKey(var.Index, val)]) {
                if (pair[0].Domain[1, pair[1].Value] == -1) unmarked.Add(count);
                count++;
            }
            return unmarked;
        }
    }
}