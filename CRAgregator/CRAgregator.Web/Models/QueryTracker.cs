using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularJSWebApiEmpty.Models {
	public class QueryTracker {
		public string SiteRef { get; set; }
		public string Study { get; set; }

		public string UniqueID { get; set; }

		public bool Open { get; set; }
		public bool Answered { get; set; }
	}
}