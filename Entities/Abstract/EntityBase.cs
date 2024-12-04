using System.Runtime.Serialization;

namespace Entities.Abstract;

[DataContract]
public class EntityBase : IEntity
{
	[DataMember]
	public Guid Id { get; set; }
	
	[DataMember]
	public DateTimeOffset Created { get; set; }
	
	[DataMember]
	public DateTimeOffset Updated { get; set; }
}