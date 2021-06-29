using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Surveys
{
    /// <summary>
    /// Klasa g³ówna z funkcj¹ main
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        /// <summary>
        /// Pomocnicza funkcja tworz¹ca hosta
        /// </summary>
        /// <param name="args">argumenty startowe programu</param>
        /// <returns>Zwraca nowego hosta</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
