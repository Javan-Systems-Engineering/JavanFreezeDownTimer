using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity.ModelConfiguration.Conventions;
using FreezeDownTimer.Models;
using Models;

public partial class FreezeDownContext : DbContext
{
    public FreezeDownContext()
        : base("name=FreezeDownContext")
    {
    }

    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Docking> Dockings { get; set; }


    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        modelBuilder.Types().Configure(c => c.Ignore("IsDirty"));

        modelBuilder.Entity<Permission>()
            .HasMany(e => e.Roles)
            .WithMany(e => e.Permissions)
            .Map(m => m.ToTable("RolePermission").MapLeftKey("PermissionID").MapRightKey("RoleID"));

        modelBuilder.Entity<Role>()
            .HasMany(e => e.Users)
            .WithMany(e => e.Roles)
            .Map(m => m.ToTable("UserRole").MapLeftKey("RoleID").MapRightKey("UserID"));
    }
}