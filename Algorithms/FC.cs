using System.Collections.Generic;

namespace Algorithms {
    
    public class FC {

        private static bool CheckForward(List<int[,]> vars, int level, int[,] var, int val) {
            foreach (int[,] variable in vars) {
                
            }
            return true;
        }
        
        // Checks for a Domain Wipe-Out due to constraint propagation
        private static bool DWO(int[,] var) {
            for (int i = 0; i < var.GetLength(0); i++) {
                if (var[i, 0] == -1) {
                    return true;
                }
            }
            return false;
        }

        // Restores domains to previous state
        private static void Restore(List<int[,]> vars, int level) {
            foreach (int[,] var in vars) {
                for (int i = 0; i < var.GetLength(0); i++) {
                    if (var[i, 0] == level) {
                        var[i, 0] = -1;
                    }
                }
            }
        }
    }
}