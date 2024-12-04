using Entities;
using Microsoft.EntityFrameworkCore;

namespace Schema.DbContexts;

public class SchemaDbContext : DbContext
{
	public SchemaDbContext(DbContextOptions<SchemaDbContext> options)
		: base (options) { }
	
	public SchemaDbContext() { }
	
	public DbSet<Person> People { get; set; }
	public DbSet<Faculty> Faculties { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Person>(entity =>
		{
			entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
			entity.Property(e => e.Created).HasDefaultValueSql("SYSDATETIME()");
			entity.Property(e => e.Updated).HasDefaultValueSql("SYSDATETIME()");
			entity.HasIndex(e => e.FirstName);
			entity.HasIndex(e => e.LastName);

			//  Configure Change / History tracking.
			entity
				.ToTable("People", b => b.IsTemporal());
		});
		
		modelBuilder.Entity<Faculty>(entity =>
		{
			entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
			entity.Property(e => e.Created).HasDefaultValueSql("SYSDATETIME()");
			entity.Property(e => e.Updated).HasDefaultValueSql("SYSDATETIME()");
			entity.HasIndex(e => e.Name)
				.IsUnique();
			

			//  Configure Change / History tracking.
			entity
				.ToTable("Faculties", b => b.IsTemporal());
		});
	}
}