using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace problem4.Models
{
    public class BtcContext : DbContext
    {
        public BtcContext(DbContextOptions<BtcContext> options) : base(options)
        {
        }
        // 

        public DbSet<BtcPriceInfo> TodoItems { get; set; }
    }
}
