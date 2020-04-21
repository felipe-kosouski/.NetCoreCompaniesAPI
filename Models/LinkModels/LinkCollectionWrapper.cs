using System.Collections.Generic;

namespace CompanyEmployees.Models.LinkModels
{
	public class LinkCollectionWrapper<T> : LinkResourceBase
	{
		public List<T> Value { get; set; } = new List<T>();

		public LinkCollectionWrapper() { }

		public LinkCollectionWrapper(List<T> value)
		{
			Value = value;
		}
	}
}