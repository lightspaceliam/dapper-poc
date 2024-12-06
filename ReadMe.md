# Dapper POC

After many conversations with people from multiple industry types using .NET, it seemed a lot of people where using Dapper. 

Whilst I’ve always been able to achieve consistent and performant results with Entity Framework, I decided to investigate Dapper to see how easy it is to implement and further explore if it could complement and work in conjunction with Entity Framework.

This POC is designed to do just that. 

1. Mitigate database schema migrations and seeding data to Entity Framework (Code First)
2. Initial exploration into simple and slightly more complex queries to experience how Dapper handles searching for single records and moving on to more complex responses that leverage navigational properties. 
3. Stored procedure compatibility with both Entity Framework and Dapper

##  Setup

This solution is designed to work with Docker running an SQL Server. This solution is compatible with MacOS and Windows. If you don't want to run Docker, you will simply need to update the connection strings:

- `/dapper-poc/Stub/appsettings.json`
- `/dapper-poc/Schema/DbContexts/SchemaDbContextFactory.cs` *Only if you want to run the migrations manually*

**What you will require is:**

1. Docker or SQL Server
2. .NET 8 https://dotnet.microsoft.com/en-us/download/dotnet

**Process:**

1. Clone the solution: https://github.com/lightspaceliam/dapper-poc.git
2. Open the `dapper-poc` solution in VSCode | Visual Studio | Ryder
3. Select the `Stub` project as the one to run
4. If running with:
    - Visual Studio or Rider: Press `Play` 
    - .NET CLI `dotnet run` or `dotnet run --environment Development` comment out the setting of the base path in: `/dapper-poc/Stub/Program.cs` - `config.SetBasePath(projectPath);`

Running the Stub console project will:

1. Create the required database: SchemaDbContext
2. Run database schema migration scripts
3. Seed the database with minimal data
4. Execute queries via Entity Framework and Dapper.

Ensure the currently set local connection strings work with your local environment.

## Project 

```
| --Solution-(dapper-poc)-
|
|   Entities
|     Person
|     Faculty
|   Schema 
|     DbContexts
|     Migrations
|   Stub
|     Program
|
| ------------------------
```
**Entities:**

Class library. Designed to house all the Entities for use with both: 

1. Entity Framework
2. Dapper backing models.

**Schema:**

Class library. Designed to handle database schema migration with Entity Framework (Code First).

**Stub:**

1. Brief concrete implementer. Calls Entity Framework to apply schema migrations and data seeds if required.
2. Implements Dapper to execute the following queries:
    - A simple find person by name
    - Slightly more complex `INNER JOIN` query to explore how navigational properties can still be leveraged  
3. Executes the same stored procedure `[dbo].[uspGetPeopleByFaculty]` via Entity Framework and Dapper.

## Conclusion

Dapper seems to be easy to use. Very similar to ADO.NET (apparently it's built on top of ADO.NET). From what I can guess, it must use reflection to map the query response/s to the backing model / entity. So no need to manually write:

```C#
var myModels = new List<MyModel>();

foreach(DataRow row in YourDataTable.Rows)
{ 
  myModels.Add(new MyModel {
    name = row["name"].ToString(),
    ...
  });
}
```

My only concern is refactoring. If you're writing TSQL in C# like so:

```c#
var person = await sqlConnection.QueryFirstOrDefaultAsync<Person>(@"
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
```

and you need to update a database table or column name, how do you refactor? You're totally reliant on a text search throughout the entire code base right? Where as end to end Entity Framework (Code First or Database First) provides full code look through / intellisense. However, if the added performance boost is what you're after, you do what you have to do.

**Side Note:**

Entity Framework allows you to execute raw TSQL similar to Dapper and has SQL Injection protection built in. I would assume both implement reflection to map results to the backing model. From what I have read the benefits of Dapper over Entity Framework include:

1. Performance (what I've read but not experienced)
2. Dapper has Unit Testing build in. Whist I didn't use it, thats a bonus. About 3 or 4 years ago I investigated Entity Framework with Stored Procedures. When doing so, there was no longer Unit Testing support.


At the very least, if your database is not already backed with Entity Framework, Dapper is a massive time saver for mapping query responses to classes. If the additional performance is real, as in you can quantify it, then this is yet another reason to make a change.   

## References:

- https://github.com/DapperLib/Dapper
- https://www.learndapper.com/relationships
- https://www.learndapper.com/stored-procedures
- https://learn.microsoft.com/en-us/ef/core/querying/sql-queries?tabs=sqlserver