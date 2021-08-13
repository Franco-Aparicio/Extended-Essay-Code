using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms {
    public class ImprovedMaintainingArcConsistency {

        public List<int[]> Solution;
        private List<Variable[]> DeletionStream;
        private Dictionary<string, List<int>> Unchecked = new ();
        private Dictionary<string, List<int>> Support = new ();
        private Dictionary<string, List<Variable[]>> MinSupport = new();
        private Random r = new Random();

        public bool MAC(Variable[] vars) {
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    var.Domain[1, i] = -1;
                    MinSupport[GetKey(var.Index, i)] = new List<Variable[]>();
                    foreach (Variable v in var.Peers) {
                        Support[GetKey(var.Index, v.Index, i)] = new List<int>();
                        Unchecked[GetKey(var.Index, v.Index, i)] = new List<int> {0};
                        if (v.Domain.GetLength(1) > 1) {
                            for (int j = 1; j < v.Domain.GetLength(1); j++) {
                                Unchecked[GetKey(var.Index, v.Index, i)].Add(j);
                            }
                        }
                    }
                }
            }
            Solution = new List<int[]>();
            DeletionStream = new List<Variable[]>();
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
            if (PropagateAC(vars, 0)) return Search(vars, 1);
            return false;
        }

        private bool PropagateAC(Variable[] vars, int level) {
            while (DeletionStream.Count > 0) {
                Variable[] arc = DeletionStream[0];
                DeletionStream.Remove(arc);
                for (int p = 0; p < MinSupport[GetKey(arc[0].Index, arc[1].Value)].Count; p++) {
                    Variable[] pair = MinSupport[GetKey(arc[0].Index, arc[1].Value)][p];
                    if (pair[0].Domain[1, pair[1].Value] != -1 || vars.All(x => x.Index != pair[0].Index)) continue;
                    if (SeekSupport(pair[0], arc[0], pair[1].Value)) {
                        MinSupport[GetKey(arc[0].Index, arc[1].Value)].Remove(pair);
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
                if (v.Domain[1, val] != -1) continue;
                MinSupport[GetKey(v.Index, val)].Add(new [] {var, new Variable(valIndex)});
                return true;
            }
            for (int value = 0; value < Unchecked[GetKey(var.Index, v.Index, valIndex)].Count; value++) {
                int val = Unchecked[GetKey(var.Index, v.Index, valIndex)][value];
                if (v.Domain[1, val] != -1) continue;
                Unchecked[GetKey(var.Index, v.Index, valIndex)].Remove(val);
                if (var.Domain[0, valIndex] == v.Domain[0, val]) continue;
                Support[GetKey(v.Index, var.Index, val)].Add(valIndex);
                Support[GetKey(var.Index, v.Index, valIndex)].Add(val);
                MinSupport[GetKey(v.Index, val)].Add(new [] {var, new Variable(valIndex)});
                return true;
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
        
        private string GetKey(int var1, int val) {
            return $"{var1},{val}";
        }
    }
}