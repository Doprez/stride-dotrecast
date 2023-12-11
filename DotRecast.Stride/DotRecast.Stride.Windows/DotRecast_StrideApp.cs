using Stride.Engine;

namespace DotRecast.Stride
{
    class DotRecast_StrideApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
