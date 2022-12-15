using Microsoft.EntityFrameworkCore;

namespace ToDoAPI.Models {
    public class ToDoContext : DbContext {
        public ToDoContext( DbContextOptions options ) : base( options ) {
        }

        public DbSet<ToDoItem> ToDoItems { get; set; }
    }
}
