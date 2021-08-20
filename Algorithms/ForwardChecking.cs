using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;

namespace Algorithms {
    
    public class ForwardChecking {

        public int Nodes = 0;
        public int Checks = 0;
        public int Backs = 0;
        public double Time = 0;
        private Stopwatch Watch = Stopwatch.StartNew();
        public List<int[]> Solution;
        private Dictionary<string, List<Variable[]>> NotSupport = new();
        private Random r = new Random();

        public bool FC(Variable[] vars) {
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    var.Domain[1, i] = -1;
                    NotSupport[GetKey(var.Index, i)] = new List<Variable[]>();
                }
            }
            Solution = new List<int[]>();
            foreach (Variable var in vars) {
                foreach (Variable v in var.Peers) {
                    if (v.Index < var.Index) continue;
                    for (int i = 0; i < var.Domain.GetLength(1); i++) {
                        for (int j = 0; j < v.Domain.GetLength(1); j++) {
                            if (var.Domain[0, i] != v.Domain[0, j]) continue;
                            NotSupport[GetKey(v.Index, j)].Add(new [] {var, new Variable(i)});
                            NotSupport[GetKey(var.Index, i)].Add(new [] {v, new Variable(j)});
                        }
                    }
                }
            }
            bool answer = Search(vars, 1);
            Time = Watch.ElapsedMilliseconds;
            return answer;
        }
        
        private Variable SelectVar(Variable[] vars) {
            Variable var = vars.OrderBy(x=> x.GetLeft()).ThenBy(x=>r.Next()).First();
            return var;
        }

        private bool Search(Variable[] vars, int level) {
            Variable var = SelectVar(vars);
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] != -1) continue;
                var item = new int[] {var.Index, var.Domain[0, i]};
                Solution.Add(item);
                Nodes++;
                if (vars.Length == 1) return true;
                vars = vars.Where(x => x.Index != var.Index).ToArray();
                if (CheckForward(vars, level, var, i) &&
                    Search(vars, level + 1)) return true;
                Solution.Remove(item);
                vars = Restore(vars, level);
            }
            return false;
        }
        
        private bool CheckForward(Variable[] vars, int level, Variable var, int val) {
            foreach (Variable[] pair in NotSupport[GetKey(var.Index, val)]) {
                if (pair[0].Domain[1, pair[1].Value] != -1 || vars.All(x => x.Index != pair[0].Index)) continue;
                Checks++;
                pair[0].Domain[1, pair[1].Value] = level;
                if (DWO(pair[0])) return false;
            }
            return true;
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
        
        private string GetKey(int var1, int val) {
            return $"{var1},{val}";
        }
    }
}