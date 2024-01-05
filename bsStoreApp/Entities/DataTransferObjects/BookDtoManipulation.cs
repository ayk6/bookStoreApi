using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
	public abstract record BookDtoManipulation
	{
		[Required(ErrorMessage ="Title required")]
		[MinLength(2, ErrorMessage ="min 2 characters")]
		[MaxLength(100,ErrorMessage ="max 100 characters")]
		public string Title { get; set; }

		[Required(ErrorMessage ="Price required")]
		[Range(10, 1000)]
		public decimal Price { get; set; }
    }
}
