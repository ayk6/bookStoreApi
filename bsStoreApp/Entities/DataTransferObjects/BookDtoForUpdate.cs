using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
	public record BookDtoForUpdate : BookDtoManipulation
	{
		[Required]
        public int Id { get; set; }

		[Required(ErrorMessage = "Category id required")]
		public int CategoryId { get; init; }
	}
}
