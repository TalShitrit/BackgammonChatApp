using Microsoft.EntityFrameworkCore;
using Models;

namespace DAL
{
    public class Data : DbContext
    {

        public Data(DbContextOptions<Data> options)
          : base(options)
        {

        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ChatMsg> ChatMsgs { get; set; }
    }
}
