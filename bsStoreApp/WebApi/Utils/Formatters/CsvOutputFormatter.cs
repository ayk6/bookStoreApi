using System.Text;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace WebApi.Utils.Formatters
{
	public class CsvOutputFormatter :TextOutputFormatter
	{
        public CsvOutputFormatter()
        {
			SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
			SupportedEncodings.Add(Encoding.UTF8);
			SupportedEncodings.Add(Encoding.Unicode);
		}

		protected override bool CanWriteType(Type? type)
		{
			if (typeof(BookDTO).IsAssignableFrom(type) ||
					typeof(IEnumerable<BookDTO>).IsAssignableFrom(type))
			{
				return base.CanWriteType(type);
			}
			return false;
		}

		public static void FormatCsv(StringBuilder builder, BookDTO bookDTO)
		{
			builder.AppendLine($"{bookDTO.Id}, {bookDTO.Title}, {bookDTO.Price}");
		}

		public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
		{
			var response = context.HttpContext.Response;
			var builder = new StringBuilder();

			if (context.Object is IEnumerable<BookDTO>) 
			{
				foreach(var book in (IEnumerable<BookDTO>)context.Object) 
				{
					FormatCsv(builder, book);
				}
			}
			else
			{
				FormatCsv(builder, (BookDTO)context.Object);
			}
			await response.WriteAsync(builder.ToString());
		}
	}
}
