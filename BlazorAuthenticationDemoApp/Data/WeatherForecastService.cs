using Microsoft.EntityFrameworkCore;

namespace BlazorAuthenticationDemoApp.Data
{
    public class WeatherForecastService
    {
        private readonly IDbContextFactory<DataContext> _ctxFactory;

        public WeatherForecastService(IDbContextFactory<DataContext> ctxFactory)
        {
            _ctxFactory = ctxFactory;
        }

        public async Task<WeatherForecast[]> GetForecastAsync()
        {
            using (var ctx = _ctxFactory.CreateDbContext()) 
            {
                return await ctx.Forecasts.ToArrayAsync();
            }
        }
    }
}