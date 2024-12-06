// See https://aka.ms/new-console-template for more information

using Dapper;
using Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Schema.DbContexts;
using Stub.Seed;

var connectionString = string.Empty;
const string facultyName = "Visual Communication Design";

var host = Host.CreateDefaultBuilder(args)
	.ConfigureAppConfiguration((context, config) =>
	{
		/*
		 HACK: override the project base path for simple use of reading the appsettings file.
		 Normally sensitive configuration properties stored in UserSecrets or a Key Vault.
		 */
		// config.AddUserSecrets<Program>();
		
    //  Comment out if running with CLI command: dotnet run or dotnet run --environment Development
    //  Leave uncommented if running in Visual Studio / Rider and you have your local ASPNETCORE_ENVIRONMENT set to Development 
		var projectPath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName).FullName;
		config.SetBasePath(projectPath);
	

		//  Pick up configuration settings.
		config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);	
	})
	.ConfigureLogging(logging =>
	{
		logging.AddConsole();
	})
	.ConfigureServices((hostContext, services) =>
	{
		services.AddLogging();
		connectionString = hostContext.Configuration["ConnectionStrings:DockerConnection"] ?? "What tha!";
		services.AddDbContext<SchemaDbContext>(options =>
		{
			options.UseSqlServer(
				hostContext.Configuration["ConnectionStrings:DockerConnection"],
				optionsBuilder =>
				{
					optionsBuilder.ExecutionStrategy(
						context => new SqlServerRetryingExecutionStrategy(context, 10, TimeSpan.FromMilliseconds(200), null));
				});
		});
	})
	.Build();

/*
 * Set Up
 * Get access to the database context, run schema migrations and seed data.
 * Mitigate this responsibility to Entity Framework.
 *
 * Use older 'using' syntax to clearly define where use of EF starts and finishes.  
 */
using (var scope = host.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<SchemaDbContext>();
	await context.Database.MigrateAsync();

	var hasFaculties = await context.Faculties.AnyAsync();
	var hasPeople = await context.People.AnyAsync();

	if (!hasFaculties && !hasPeople)
	{
		context.Faculties.AddRange(FacultiesPeopleSeed.FacilitiesData);
		await context.SaveChangesAsync();
	}

	/*
	 * Execute stored procedure via Entity Framework.
	 */
	var peopleDerivedFromStoredProcedure = await context.People
		.FromSqlInterpolated($"EXEC [dbo].[uspGetPeopleByFaculty] {facultyName}")
		.ToListAsync();
	
	Console.WriteLine("\nStored Procedure Execution:\n");
	
	peopleDerivedFromStoredProcedure.ForEach(p =>
	{
		Console.WriteLine($"Person derived form SP with Entity Framework - Name: {p.Name}");
	});
}

/*
 * Initiate database connection for use with Dapper.
 */
await using (var sqlConnection = new SqlConnection(connectionString ?? throw new InvalidOperationException()))
{
	await sqlConnection.OpenAsync();
	
	//  Inline query searching for a single record in the database. Apparently you're not required to define each table property.
	var luke = await sqlConnection.QueryFirstOrDefaultAsync<Person>(@"
		SELECT	Id
				, FirstName
				, LastName
				, DateOfBirth
				, FacultyId
				, Created
				, Updated
		FROM 	People
		WHERE 	FirstName = 'Luke';
	");

	/*
	 * Up the complexity. Simple INNER JOIN to investigate how to still take advantage of navigational properties. 
	 */
	var joinQuery = @"
		SELECT  *
		FROM    People AS P
		        INNER JOIN Faculties AS F
		            ON  P.FacultyId = F.Id  
		--WHERE   F.Name = 'Visual Communication Design'
	";
	
	var people = (await sqlConnection.QueryAsync<Person, Faculty, Person>(joinQuery, (person, faculty) =>
	{
		person.Faculty = faculty;
		return person;
	//}, splitOn: "FacultyId")); //  why is this recommended. Works without it?
	})).ToList();
	
	//  Same Stored Procedure executed via Dapper.
	var dapperSpPeople = (await sqlConnection.QueryAsync(
			"EXEC [dbo].[uspGetPeopleByFaculty] @FacultyName", 
			new { FacultyName = facultyName }))
		.ToList();
	
	await sqlConnection.CloseAsync();
	
	Console.WriteLine("\nPrint Person result:\n");
	Console.WriteLine($"Person - Name: {luke.Name}, Dob: {luke.DateOfBirth}");
	
	Console.WriteLine("\nPrint Person to Faculty results:\n");
	people
		.ForEach(p =>
	{
		Console.WriteLine($"Person - Name: {p.Name}\nFaculty - name: {p.Faculty.Name}");
	});

	var grouping = people
		.GroupBy(person => person.Faculty.Name)
		.ToList();
	
	Console.WriteLine("\nPrint Faculty aggregation by person results:\n");
	grouping.ForEach(p =>
	{
		Console.WriteLine($"Faculty - name: {p.Key}, Has this many people: {p.Count()}");
	});
	
	Console.WriteLine("\nPrint People derived from stored procedure:\n");
	
	dapperSpPeople.ForEach(p =>
	{
		Console.WriteLine($"Person derived form SP with Dapper - Name: {p.Name}");
	});
}

Console.WriteLine("Hello, Dapper!");