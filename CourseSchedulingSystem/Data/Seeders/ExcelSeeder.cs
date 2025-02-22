using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Data.Seeders
{
    /// <summary>
    /// Runs seeders that import data from excel fixtures.
    /// </summary>
    public class ExcelSeeder : ISeeder
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;

        public ExcelSeeder(IServiceProvider provider, ILogger<DatabaseSeeder> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            await CallAsync<CourseSeeder>();
            await CallAsync<SchedulesSeeder>();
        }
        
        /// <summary>
        ///     Runs a seeder.
        /// </summary>
        /// <param name="seeder"></param>
        /// <returns></returns>
        private async Task CallAsync<T>() where T : ISeeder
        {
            var seeder = ActivatorUtilities.GetServiceOrCreateInstance<T>(_provider);
            _logger.LogInformation($"Running {typeof(T).Name}");
            await seeder.SeedAsync();
        }
    }
}