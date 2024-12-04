using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Entities.Abstract;

namespace Entities;

public class Faculty : EntityBase
{
	[DataMember]
	[Required(ErrorMessage = "Name is required")]
	[StringLength(150, ErrorMessage = "Name cannot be longer than 150 characters")]
	public string Name { get; set; } = default!;

	[DataMember]
	public List<Person> People { get; set; } = new();
}