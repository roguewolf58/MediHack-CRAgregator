using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularJSWebApiEmpty.Models {
	public class CountDTO {

		public long StudyId { get; set; }

		public string SponsorName { get; set; }
		public string StudyName { get; set; }
		public string SiteName { get; set; }

		public long Count { get; set; }
	}
}