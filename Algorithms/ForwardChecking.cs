using System.Collections.Generic;
using System;
using System.Linq;

namespace Algorithms {
    
    public class ForwardChecking {
        
        private Dictionary<int[], List<int[]>> allowed;
        public List<int[]> Keys;
        public List<int[]> Solution;
        
        public ForwardChecking(Dictionary<int[], List<int[]>> allowed) {
            this.allowed = allowed;
            Keys = allowed.Keys.ToList();
        }

        public bool FC(Variable[] vars) {
            foreach (Variable var in vars) {
                for (int i = 0; i < var.Domain.GetLength(1); i++) {
                    var.Domain[1, i] = 0;
                }
            }
            Solution = new List<int[]>();
            bool res = Search(vars, 1);
            Console.WriteLine(res);
            foreach (var s in Solution) {
                Console.WriteLine($"({s[0]}, {s[1]})");
            }
            return res;
        }

        private bool Search(Variable[] vars, int level) {
            Random r = new Random();
            Variable var = vars[r.Next(vars.Length)];
            // Console.WriteLine(vars.Length);
            // Console.WriteLine(var.Index);
            for (int i = 0; i < var.Domain.GetLength(1); i++) {
                if (var.Domain[1, i] != 0) continue;
                var item = new int[] {var.Index, var.Domain[0, i]};
                // Console.WriteLine($"{var.Index}, {var.Domain[0, i]}");
                Solution.Add(item);
                if (vars.Length == 1) {
                    return true;
                }
                var temp = vars.Where(x => x.Index != var.Index).ToArray();
                if (CheckForward(temp, level, var, var.Domain[0, i]) &&
                    Search(temp, level + 1)) return true;
                Solution.Remove(item);
                Restore(temp, level);
            }
            // Console.WriteLine(Solution.Count);
            return false;
        }
        
        private bool CheckForward(Variable[] vars, int level, Variable var, int val) {
            foreach (Variable v in vars) {
                int[] k = var.Index < v.Index ? new [] {var.Index, v.Index} : new [] {v.Index, var.Index};
                var key = Keys.Find(x => x.SequenceEqual(k));
                if (key == null) continue;
                    for (int i = 0; i < v.Domain.GetLength(1); i++) {
                        if (v.Domain[1, i] != 0) continue;
                        if (!allowed[key].Any(x=>x.SequenceEqual(new int[] {val, v.Domain[0, i]}))) {
                            v.Domain[1, i] = level;
                        }
                        // Console.WriteLine($"Level: {level} -- {var.Index}, {v.Index} ({val}, {v.Domain[0, i]}), {v.Domain[1, i]}");
                        // Console.WriteLine(DWO(v));
                        // if (DWO(v)) return false;
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