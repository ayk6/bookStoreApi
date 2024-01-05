using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
	public record BookDtoForInsertion : BookDtoManipulation
	{
		[Required(ErrorMessage = "Category id required")]
        public int CategoryId { get; init; }
    }
}
