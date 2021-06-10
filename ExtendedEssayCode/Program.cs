using System.Threading.Tasks;

namespace ExtendedEssayCode {
    class Program {
        // characters used in the grid
        public const string valueList = "123456789ABCDEFGHIJKLMNOP";

        static void Main(string[] args) {
            // start generating grid
            int n = 2;
            int[,] board = new int[n, n];
            var tasks = new[] {
                Task.Factory.StartNew(() => new Grid(n).Board),
                Task.Factory.StartNew(() => new Grid(n).Board),
                Task.Factory.StartNew(() => new Grid(n).Board),
                Task.Factory.StartNew(() => new Grid(n).Board)}; 
            var things = Task.WhenAll(tasks);
            foreach (var task in things.Result) {
                if (task != null) {
                    board = (int[,]) task.Clone();
                }
            }
            Grid.PrintBoard(board);
        }
    }
}