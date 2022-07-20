using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthenticationDemoApp.Data
{
    public class DataContext : IdentityDbContext
    {
        public virtual DbSet<WeatherForecast> Forecasts { get; set; }

        public DataContext()
        {
        }

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=vdpam-007760;Initial Catalog=BlazerAuth;Integrated Security=True");
        //}
    }
}
