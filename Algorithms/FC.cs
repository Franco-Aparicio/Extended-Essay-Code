using System.Collections.Generic;
using System;
using System.Linq;

namespace Algorithms {
    
    public class FC {
        
        private Dictionary<Variable[], List<int[]>> allowed;
        private List<int[]> Solution = new List<int[]>();
        
        public FC(Dictionary<Variable[], List<int[]>> allowed) {
            this.allowed = allowed;
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
                Variable[] key = var.Index < variable.Index ? new Variable[] {var, variable} : new Variable[] {variable, var};
                if (allowed.ContainsKey(key)) {
                    for (int i = 0; i < variable.Domain.GetLength(1); i++) {
                        if (variable.Domain[1, i] == 0 &&
                            !allowed[key].Contains(new int[] {val, variable.Domain[0, i]})) {
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