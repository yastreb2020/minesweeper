using System;
using Serilog;

/*namespace SerilogExample
{
    class Program
    {
        static void Main()
        {
            

            Log.Information("Hello, world!");

            int a = 10, b = 0;
            try
            {
                Log.Debug("Dividing {A} by {B}", a, b);
                Console.WriteLine(a / b);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
*/
namespace QuasiGaame
{
    class Program
    {
        static void Main(string[] args)
        {
            // logging configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var game = new Minesweeper();
            game.StartGame();
        }

        


        



    }
}
