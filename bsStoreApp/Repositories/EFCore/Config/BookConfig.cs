using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.EFCore.Config
{
	public class BookConfig: IEntityTypeConfiguration<Book>
	{
		public void Configure(EntityTypeBuilder<Book> builder)
		{
			builder.HasData(
				new Book { Id = 1, CategoryId=1, Title = "Divine Commedy", Price = 50 },
				new Book { Id = 2, CategoryId=2, Title = "1984", Price = 25 },
				new Book { Id = 3, CategoryId=2, Title = "The Brothers Karamazov", Price = 30 }
				);
		}
	}
}
