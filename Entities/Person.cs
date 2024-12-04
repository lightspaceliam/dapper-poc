using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Entities.Abstract;

namespace Entities;

public class Person : EntityBase
{
	[NotMapped]
	public string Name => $"{FirstName}  {LastName}";
	
	[DataMember]
	[Required(ErrorMessage = "First name is required")]
	[StringLength(150, ErrorMessage = "First name cannot be longer than {1} characters")]
	public string FirstName { get; set; } = default!;

	[DataMember]
	[Required(ErrorMessage = "Last name is required")]
	[StringLength(150, ErrorMessage = "Last name cannot be longer than {1} characters")]
	public string LastName { get; set; } = default!;
	
	[DataMember]
	[Required(ErrorMessage = "Date of birth is required")]
	public DateOnly DateOfBirth { get; set; }
	
	public Guid FacultyId { get; set; } 
	public Faculty Faculty { get; set; }
}