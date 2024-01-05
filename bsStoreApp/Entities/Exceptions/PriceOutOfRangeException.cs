namespace Entities.Exceptions
{
	public class PriceOutOfRangeException : BadRequestException
	{
		public PriceOutOfRangeException() : base("max price should be min 10, max 1000")
		{

		}
	}
}
