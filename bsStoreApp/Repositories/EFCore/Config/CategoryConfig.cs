using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.EFCore.Config
{
	public class CategoryConfig : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			builder.HasKey(c => c.CategoryId); // pk
			builder.Property(c => c.CategoryName).IsRequired();

			builder.HasData(
				new Category { CategoryId = 1, CategoryName = "Art", },
				new Category { CategoryId = 2, CategoryName = "Fiction" },
				new Category { CategoryId = 3, CategoryName = "Science" }
				);
		}
	}
}
