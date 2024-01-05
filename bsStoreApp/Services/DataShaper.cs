using System.Dynamic;
using System.Reflection;
using Services.Contracts;

namespace Services
{
	public class DataShaper<T> : IDataShaper<T> where T : class
	{
		public PropertyInfo[] Properties { get; set; }
		public DataShaper()
		{
			Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
		}
		public IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString)
		{
			var requiredProperties = RequiredProperties(fieldsString);
			return FetchData(entities, requiredProperties);
		}

		public ExpandoObject ShapeData(T entity, string fieldsString)
		{
			var requiredProperties = RequiredProperties(fieldsString);
			return FetchDataForEntity(entity, requiredProperties);
		}

		private IEnumerable<PropertyInfo> RequiredProperties(string fieldsString)
		{
			var requiredProperties = new List<PropertyInfo>();
			if (!string.IsNullOrWhiteSpace(fieldsString))
			{
				var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
				foreach (var field in fields)
				{
					var property = Properties.FirstOrDefault(p =>
					p.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));
					if (property is null) continue;
					requiredProperties.Add(property);
				}
			}
			else
			{
				requiredProperties = Properties.ToList();
			}
			return requiredProperties;
		}

		private ExpandoObject FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
		{
			var shapedObject = new ExpandoObject();

			foreach (var property in requiredProperties)
			{
				var objectPropertyValue = property.GetValue(entity);
				shapedObject.TryAdd(property.Name, objectPropertyValue);
			}
			return shapedObject;
		}

		private IEnumerable<ExpandoObject> FetchData(IEnumerable<T> entities,
			IEnumerable<PropertyInfo> requiredroperties)
		{
			var shapedData = new List<ExpandoObject>();
			foreach (var entity in entities)
			{
				var shapedObject = FetchDataForEntity(entity, requiredroperties);
				shapedData.Add(shapedObject);
			}
			return shapedData;
		}
	}
}
