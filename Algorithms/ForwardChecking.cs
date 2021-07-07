using System.Collections.Generic;
using System;
using System.Linq;

namespace Algorithms {
    
    public class ForwardChecking {
        
        public List<int[]> Solution;
        private Random r = new Random();

        public bool FC(Variable[] vars) {
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    var.Domain[1, i] = 0;
                }
            }
            Solution = new List<int[]>();
            return Search(vars, 1);
        }
        
        private Variable SelectVar(Variable[] vars) {
            Variable var = vars.OrderBy(x=> x.GetLeft()).ThenBy(x=>r.Next()).First();
            return var;
        }

        private bool Search(Variable[] vars, int level) {
            Variable var = SelectVar(vars);
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] != 0) continue;
                var item = new int[] {var.Index, var.Domain[0, i]};
                Solution.Add(item);
                if (vars.Length == 1) return true;
                vars = vars.Where(x => x.Index != var.Index).ToArray();
                if (CheckForward(vars, level, var, var.Domain[0, i]) &&
                    Search(vars, level + 1)) return true;
                Solution.Remove(item);
                vars = Restore(vars, level);
            }
            return false;
        }
        
        private bool CheckForward(Variable[] vars, int level, Variable var, int val) {
            foreach (Variable v in vars) {
                if (var.Peers.All(x => x.Index != v.Index)) continue;
                for (int i = 0; i < v.Domain.GetLength(1); i++) {
                    if (v.Domain[1, i] != 0) continue;
                    v.Domain[1, i] = val == v.Domain[0, i] ? level : 0;
                }
                if (DWO(v)) return false;
            }
            return true;
        }
        
        // Checks for a Domain Wipe-Out due to constraint propagation
        private bool DWO(Variable var) {
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] == 0) {
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
                        var.Domain[1, i] = 0;
                    }
                }
            }
            return vars;
        }
    }
}