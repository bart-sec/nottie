using Microsoft.EntityFrameworkCore;

namespace server
{
    public class TodoDb : DbContext
    {

        public TodoDb(DbContextOptions<TodoDb> dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<Todo> Todods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Todo>().ToTable(nameof(Todo));
        }
    }


}
