using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms {
    public class ImprovedMaintainingArcConsistency {

        public List<int[]> Solution;
        private List<Variable[]> DeletionStream;
        private Dictionary<string, List<int>> Support = new ();
        private Random r = new Random();

        public bool MAC(Variable[] vars) {
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    var.Domain[1, i] = -1;
                    foreach (Variable v in vars) {
                        if (var.Peers.Any(x => x.Index == v.Index)) {
                            Support[GetKey(var.Index, v.Index, i)] = new List<int>();
                            // Support.Add(GetKey(var.Index, v.Index, i), new List<int>());
                        }
                    }
                }
            }
            Solution = new List<int[]>();
            DeletionStream = new List<Variable[]>();
            foreach (Variable var in vars) {
                foreach (Variable v in vars) {
                    if (var.Peers.Any(x => x.Index == v.Index)) {
                        for (int i = 0; i < var.Domain.GetLength(1); i++) {
                            if (var.Domain[1, i] != -1) continue;
                            int total = 0;
                            for (int j = 0; j < v.Domain.GetLength(1); j++) {
                                if (v.Domain[1, j] != -1) continue;
                                if (var.Domain[0, i] != v.Domain[0, j]) {
                                    total++;
                                    Support[GetKey(v.Index, var.Index, j)].Add(i);
                                }
                            }
                            if (total == 0) {
                                var.Domain[1, i] = 0;
                                DeletionStream.Add(new [] {var, new Variable(i)});
                                if (DWO(var)) return false;
                            }
                        }
                    }
                }
            }
            if (PropagateAC(vars, 0)) return Search(vars, 1);
            return false;
        }

        private bool PropagateAC(Variable[] vars, int level) {
            while (DeletionStream.Count > 0) {
                Variable[] arc = DeletionStream[0];
                DeletionStream.Remove(arc);
                foreach (Variable var in vars) {
                    if (arc[0].Peers.Any(x => x.Index == var.Index)) {
                        foreach (int val in Support[GetKey(arc[0].Index, var.Index, arc[1].Value)]) {
                            if (var.Domain[1, val] != -1 || SeekSupport(var, arc[0], val)) continue;
                            // if (SeekSupport(var, arc[0], val)) continue;
                            var.Domain[1, val] = level;
                            DeletionStream.Add(new [] {var, new Variable(val)});
                            if (DWO(var)) return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool Search(Variable[] vars, int level) {
            Variable var = SelectVar(vars);
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] != -1) continue;
                var item = new [] {var.Index, var.Domain[0, i]};
                Solution.Add(item);
                if (vars.Length == 1) return true;
                for (int j = 0; j < var.Domain.GetLength(1); j++) {
                    if (var.Domain[1, j] != -1 || j == i) continue;
                    var.Domain[1, j] = level;
                    DeletionStream.Add(new [] {var, new Variable(j)});
                }
                Variable[] temp = vars.Where(x => x.Index != var.Index).ToArray();
                if (PropagateAC(temp, level) && Search(temp, level + 1)) return true;
                Solution.Remove(item);
                vars = Restore(vars, level);
            }
            return false;
        }

        private bool SeekSupport(Variable var, Variable v, int valIndex) {
            foreach (int val in Support[GetKey(var.Index, v.Index, valIndex)]) {
                if (v.Domain[1, val] == -1) return true;
            }
            return false;
        }
        
        private Variable SelectVar(Variable[] vars) {
            Variable var = vars.OrderBy(x => x.GetLeft()).ThenBy(x=>r.Next()).First();
            return var;
        }

        // Checks for a Domain Wipe-Out due to constraint propagation
        private bool DWO(Variable var) {
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] == -1) {
                    return false;
                }
            }
            return true;
        }

        // Restores domains to previous state
        private Variable[] Restore(Variable[] vars, int level) {
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    if (var.Domain[1, i] == level) {
                        var.Domain[1, i] = -1;
                    }
                }
            }
            return vars;
        }

        private string GetKey(int var1, int var2, int val) {
            return $"{var1},{var2},{val}";
        }
    }
}