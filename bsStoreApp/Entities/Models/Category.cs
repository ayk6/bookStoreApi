namespace Entities.Models
{
	public class Category
	{
        public int CategoryId { get; set; }
        public String? CategoryName { get; set; }

        // Ref: navigation property
        //public ICollection<Book> Books { get; set; }
    }
}
