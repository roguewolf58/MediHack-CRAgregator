using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularJSWebApiEmpty.Models {
	public class AuditDTO {
		public string QueryStatus { get; set; }
		public string QueryRepeatKey { get; set; }
		public string ItemOID { get; set; }
		public string RecordID { get; set; }
		public string DataPageID { get; set; }
		public string InstanceID { get; set; }
		public string SubjectKey { get; set; }
		public string SiteRef { get; set; }
		public string Study { get; set; }

		public string QueryKey {
			get {
				return string.Join(":", Study, SubjectKey, InstanceID, DataPageID, RecordID, ItemOID, QueryRepeatKey);
			}
		}
	}
}