using Entities;
namespace Stub.Seed;

public static class FacultiesPeopleSeed
{
	public static List<Faculty> FacilitiesData = new List<Faculty>
	{
		new Faculty
		{
			Name = "Visual Communication Design",
			People = new List<Person>
			{
				new Person { FirstName = "Luke", LastName = "Skywalker", DateOfBirth = new DateOnly(2010, 10, 14) },
				new Person { FirstName = "Han", LastName = "Solo", DateOfBirth = new DateOnly(2010, 01, 03) },
			}
		},
		new Faculty
		{
			Name = "Computers Information Technology",
			People = new List<Person>
			{
				new Person { FirstName = "Leia", LastName = "Organa", DateOfBirth = new DateOnly(2010, 05, 07) },
			}
		},
		new Faculty
		{
			Name = "English",
		},
		new Faculty
		{
			Name = "Mathematics",
			People = new List<Person>
			{
				new Person { FirstName = "Ben", LastName = "Solo", DateOfBirth = new DateOnly(2010, 11, 24) },
			}
		},
	};
}