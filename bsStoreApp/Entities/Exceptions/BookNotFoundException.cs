namespace Entities.Exceptions
{
	public sealed class BookNotFoundException : NotFoundException
	{
		public BookNotFoundException(int id) : base($"Book id:{id} not found")
		{
		}
	}
}
