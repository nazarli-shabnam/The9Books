using System;
using System.Threading;

namespace The9Books
{
    public interface IRandom
    {
        int RandPositive(int max = int.MaxValue);
    }

    public class RandomGenerator : IRandom
    {
        private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => 
            new Random(Interlocked.Increment(ref _seedCounter)));

        private static int _seedCounter = Environment.TickCount;

        public int RandPositive(int max = int.MaxValue)
        {
            return _random.Value.Next(1, max + 1);
        }
    }
}