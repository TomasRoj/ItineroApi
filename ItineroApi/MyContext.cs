using ItineroApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ItineroApi
{
    public class MyContext : DbContext
    {
        public DbSet<Trip> Trips { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Attraction> Attractions { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<CustomDestination> CustomDestinations { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategory { get; set; }
        public DbSet<ExpenseSplit> ExpenseSplit { get; set; }
        public DbSet<ItineraryDay> ItineraryDay { get; set; }
        public DbSet<ItineraryItem> ItineraryItem { get; set; }
        public DbSet<TripMember> TripMembers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=mysqlstudenti.litv.sssvt.cz;database=3b1_rojtomas_db1;user=rojtomas;password=123456;SslMode=none");
        }
    }
}
