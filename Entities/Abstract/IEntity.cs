namespace Entities.Abstract;

public interface IEntity
{
	Guid Id { get; set; }
	DateTimeOffset Created { get; set; }
	DateTimeOffset Updated { get; set; }
}