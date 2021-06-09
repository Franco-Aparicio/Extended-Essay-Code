using System.Threading;

namespace ExtendedEssayCode {
    class Program {
        // characters used in the grid
        public const string valueList = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ?";

        static void Main(string[] args) {
            // start generating grid
            int n = 4;
            for (int i = 0; i < 4; i++) {
                Thread t = new Thread(() => { new Grid(n); });
                t.Priority = ThreadPriority.Highest;
                t.Start();
            }
            
        }
    }
}