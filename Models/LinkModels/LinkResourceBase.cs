using System.Collections.Generic;

namespace CompanyEmployees.Models.LinkModels
{
	public class LinkResourceBase
	{
		public LinkResourceBase() { }

		public List<Link> Links { get; set; } = new List<Link>();
	}
}