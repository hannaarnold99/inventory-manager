using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InventoryManager.Models;

namespace InventoryManager.Data
{
    public class InventoryManagerContext : DbContext
    {
        public InventoryManagerContext (DbContextOptions<InventoryManagerContext> options)
            : base(options)
        {
        }

        public DbSet<InventoryManager.Models.StockProduct> StockProduct { get; set; } = default!;

    }
}
