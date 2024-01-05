namespace Entities.Exceptions
{
	public sealed class CategoryNotFoundException : NotFoundException
	{
		public CategoryNotFoundException(int id) : base($"Category id:{id} not found")
		{
		}
	}
}
