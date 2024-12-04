using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Schema.DbContexts;

public class SchemaDbContextFactory : IDesignTimeDbContextFactory<SchemaDbContext>
{
	public SchemaDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<SchemaDbContext>();
		//  Windows.
		// optionsBuilder.UseSqlServer("Server=localhost;Database=SchemaDbContext;Integrated Security=True;MultipleActiveResultSets=True");
            
		//  Docker - you will need to add, name and set your own: container and credentials.
		optionsBuilder.UseSqlServer("Server=tcp:localhost,1433;Initial Catalog=SchemaDbContext;Persist Security Info=False;User ID=SA;Password=Local@DevPa55word;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");

		return new SchemaDbContext(optionsBuilder.Options);
	}
}