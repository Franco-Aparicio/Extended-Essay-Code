using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms {
    
    public class MaintainingArcConsistency {

        public List<int[]> Solution;
        private List<Variable[]> Q;
        private Random r = new Random();

        // public MaintainingArcConsistency(Variable[] vars) {
        //     foreach (Variable var in vars) {
        //         var.Peers.ForEach(x=>Q.Add(new []{var, x}));
        //     }
        //     foreach (Variable[] pair in Q) {
        //         Console.WriteLine($"({pair[0].Index}, {pair[1].Index})");
        //     }
        // }

        public bool MAC(Variable[] vars) {
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    var.Domain[1, i] = -1;
                }
            }
            Solution = new List<int[]>();
            Q = new List<Variable[]>();
            foreach (Variable var in vars) {
                foreach (Variable v in vars) {
                    if (var.Index == v.Index) continue;
                    if (var.Peers.Any(x => x.Index == v.Index)) {
                        // Q = Q.Union(new List<Variable[]>{new []{var, v}}).ToList();
                        AddToQ(new []{var, v});
                    }
                }
            }
            return PropagateAC(vars, 0) && Search(vars, 1);
        }

        private bool Revise(Variable var, Variable v, int level) {
            bool deleted = false;
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] != -1) continue;
                bool found = false;
                for (int j = 0; j < v.Domain.GetLength(1); j++) {
                    if (v.Domain[1, j] != -1) continue;
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
            // Console.WriteLine($"Level: {level}\tQ: {Q.Count}");
            while (Q.Count > 0) {
                // Variable[] arc = SelectArc(vars);
                Variable[] arc = Q[0];
                Q.Remove(arc);
                if (Revise(arc[0], arc[1], level)) {
                    if (DWO(arc[0])) return false;
                    foreach (Variable var in arc[0].Peers) {
                        if (var.Index == arc[1].Index) continue;
                        AddToQ(new []{var, arc[0]});
                    }
                }
            }
            return true;
        }

        private bool Search(Variable[] vars, int level) {
            Variable var = SelectVar(vars);
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                // Console.WriteLine(vars.Length);
                if (var.Domain[1, i] != -1) continue;
                var item = new [] {var.Index, var.Domain[0, i]};
                Solution.Add(item);
                // Console.WriteLine(vars.Length);
                if (vars.Length == 1) return true;
                // var.RemoveDomain(i);
                for (int j = 0; j < var.Domain.GetLength(1); j++) {
                    if (var.Domain[1, j] != -1 || j == i) continue;
                    var.Domain[1, j] = level;
                }
                foreach (Variable v in vars) {
                    if (v.Peers.Any(x => x.Index == var.Index)) {
                        // Q = Q.Union(new List<Variable[]>{new []{v, var}}).ToList();
                        AddToQ(new []{v, var});
                    }
                }
                // vars = vars.Where(x => x.Index != var.Index).ToArray();
                Variable[] temp = vars.Where(x => x.Index != var.Index).ToArray();
                if (PropagateAC(temp, level) && Search(temp, level + 1)) return true;
                // bool thing1 = PropagateAC(vars, level);
                // bool thing2 = Search(vars, level + 1);
                // Console.WriteLine($"Level: {level}\tProp: {thing1}\tSearch: {thing2}");
                // if (thing1 && thing2) return true;
                Solution.Remove(item);
                vars = Restore(vars, level);
                // Console.WriteLine(vars.Length);
            }
            return false;
        }
        
        private Variable[] SelectArc(Variable[] vars) {
            Variable[] arc = Q[0];
            Q.Remove(arc);
            return new []{arc[0], arc[1]};
        }
        
        private Variable SelectVar(Variable[] vars) {
            Variable var = vars.OrderBy(x => x.GetLeft()).ThenBy(x=>r.Next()).First();
            // Console.WriteLine(var.Index);
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
            // Console.WriteLine("Restore");
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    if (var.Domain[1, i] == level) {
                        var.Domain[1, i] = -1;
                    }
                }
            }
            return vars;
        }

        private void AddToQ(Variable[] toAdd) {
            if (Q.Any(pair => pair[0].Index == toAdd[0].Index && pair[1].Index == toAdd[1].Index)) {
                return;
            }
            Q.Add(toAdd);
        }

        // private void MakeQ(Variable[] vars) {
        //     foreach (Variable var in vars) {
        //         var.Peers.ForEach(x=>Q.Add(new []{var.Index, x.Index}));
        //     }
        // }
    }
}