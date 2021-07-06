using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms {
    
    public class MaintainingArcConsistency {

        public List<int[]> Solution;
        private List<int[]> Q;
        private Random r = new Random();

        // public MaintainingArcConsistency() {
            // foreach (Variable var in vars) {
            //     var.Peers.ForEach(x=>Q.Add(new []{var.Index, x.Index}));
            // }
            // foreach (int[] pair in Q) {
            //     Console.WriteLine($"({pair[0]}, {pair[1]})");
            // }
        // }

        public bool MAC(Variable[] vars) {
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    var.Domain[1, i] = 0;
                }
            }
            Solution = new List<int[]>();
            Q = new List<int[]>();
            foreach (Variable var in vars) {
                foreach (Variable v in vars) {
                    if (var.Index == v.Index) continue;
                    if (var.Peers.Any(x => x.Index == v.Index)) {
                        Q = Q.Union(new List<int[]>{new int[]{var.Index, v.Index}}).ToList();
                    }
                }
            }
            return PropagateAC(vars, 0) && Search(vars, 1);
        }

        private bool Revise(Variable var, Variable v, int level) {
            bool deleted = false;
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] != 0) continue;
                bool found = false;
                for (int j = 0; j < v.Domain.GetLength(1); j++) {
                    if (v.Domain[1, j] != 0) continue;
                    if (var.Domain[0, i] != v.Domain[0, j]) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    var.Domain[1, i] = level;
                    deleted = true;
                }
            }
            return deleted;
        }

        private bool PropagateAC(Variable[] vars, int level) {
            // Console.WriteLine(level);
            while (Q.Count > 0) {
                Variable[] arc = SelectArc(vars);
                if (Revise(arc[0], arc[1], level)) {
                    if (DWO(arc[0])) return false;
                    foreach (Variable var in vars) {
                        if (var.Index == arc[1].Index) continue;
                        if (arc[0].Peers.Any(x => x.Index == var.Index)) {
                            Q = Q.Union(new List<int[]> {new int[] {var.Index, arc[0].Index}}).ToList();
                        }
                    }
                }
            }
            return true;
        }

        private bool Search(Variable[] vars, int level) {
            Variable var = SelectVar(vars);
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] != 0) continue;
                var item = new int[] {var.Index, var.Domain[0, i]};
                Solution.Add(item);
                if (vars.Length == 1) return true;
                var.RemoveDomain(i);
                for (int j = 0; j < var.Domain.GetLength(1); j++) {
                    if (var.Domain[1, i] != 0) continue;
                    var.Domain[1, i] = level;
                }
                foreach (Variable v in vars) {
                    if (v.Peers.Any(x => x.Index == var.Index)) {
                        Q = Q.Union(new List<int[]>{new int[]{v.Index, var.Index}}).ToList();
                    }
                }
                vars = vars.Where(x => x.Index != var.Index).ToArray();
                if (PropagateAC(vars, level) && Search(vars, level + 1)) return true;
                Solution.Remove(item);
                Restore(vars, level);
            }
            return false;
        }
        
        private Variable[] SelectArc(Variable[] vars) {
            Variable var = vars.OrderBy(x=> x.GetLeft()).ThenBy(x=>r.Next()).First();
            List<int[]> arcs = Q.FindAll(x => x[0] == var.Index);
            int[] arc = arcs.OrderBy(x=>r.Next()).First();
            Q.Remove(arc);
            return new []{vars[arc[0]], vars[arc[1]]};
        }
        
        private Variable SelectVar(Variable[] vars) {
            Variable var = vars.OrderBy(x=> x.GetLeft()).ThenBy(x=>r.Next()).First();
            return var;
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

        // private void MakeQ(Variable[] vars) {
        //     foreach (Variable var in vars) {
        //         var.Peers.ForEach(x=>Q.Add(new []{var.Index, x.Index}));
        //     }
        // }
    }
}