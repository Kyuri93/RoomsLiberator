using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RoomsLiberatorEngine.ViewModel;

namespace RoomsLiberatorEngine
{
    public class DatabaseContext : DbContext
    {
       // private string connectionString = @"Data Source=.\FilenameRoomL.db";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("FilenameRoomL.db");
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "RoomL.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }
        public DbSet<EventLog> EventLogs { get; set; }
        public DbSet<DeviceState> DeviceStates { get; set; }
        public DbSet<User> Users { get; set; }
    }
}