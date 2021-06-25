using System.Collections.Generic;
using System;
using System.Linq;

namespace Algorithms {
    
    public class ForwardChecking {
        
        private Dictionary<int[], List<int[]>> allowed;
        public Dictionary<int[], List<int[]>>.KeyCollection Keys;
        public List<int[]> Solution;
        
        public ForwardChecking(Dictionary<int[], List<int[]>> allowed) {
            this.allowed = allowed;
            Keys = allowed.Keys;
        }

        public bool FC(Variable[] vars) {
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    var.Domain[1, i] = 0;
                }
            }
            Solution = new List<int[]>();
            return Search(vars, 1);
        }

        private bool Search(Variable[] vars, int level) {
            Random r = new Random();
            Variable var = vars[r.Next(vars.Length)];
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] != 0) continue;
                Solution.Add(new int[] {var.Index, var.Domain[0, i]});
                if (vars.Length == 1) {
                    return true;
                }
                if (CheckForward(vars.Where(x => x.Index != var.Index).ToArray(), level, var, var.Domain[0, i]) &&
                    Search(vars.Where(x => x.Index != var.Index).ToArray(), level + 1)) return true;
                Solution.Remove(new int[] {var.Index, var.Domain[0, i]});
                Restore(vars.Where(x => x.Index != var.Index).ToArray(), level);
            }
            return false;
        }
        
        private bool CheckForward(Variable[] vars, int level, Variable var, int val) {
            foreach (Variable variable in vars) {
                int[] k = var.Index < variable.Index ? new [] {var.Index, variable.Index} : new [] {variable.Index, var.Index};
                var key = Keys.ToList().Find(x => x.SequenceEqual(k));
                if (key != null) {
                    for (int i = 0; i < variable.Domain.GetLength(1); i++) {
                        if (variable.Domain[1, i] == 0 &&
                            allowed[key].Find(x=>x.SequenceEqual(new int[] {val, variable.Domain[0, i]})) == null) {
                            variable.Domain[1, i] = level;
                        }
                    }
                    if (DWO(variable)) return false;
                }
            }
            return true;
        }
        
        // Checks for a Domain Wipe-Out due to constraint propagation
        private bool DWO(Variable var) {
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] == 0) {
                    return true;
                }
            }
            return false;
        }

        // Restores domains to previous state
        private void Restore(Variable[] vars, int level) {
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    if (var.Domain[1, i] == level) {
                        var.Domain[1, i] = 0;
                    }
                }
            }
        }
    }
}