using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Xml;
using AngularJSWebApiEmpty.Models;

namespace AngularJSWebApiEmpty.Controllers {
	public class MutableKeyValuePair<TKey, TValue> {
		public TKey Key { get; set; }
		public TValue Value { get; set; }
	}

	public class QueriesController : ApiController {
		private string _usr {get; set;} = "defuser";
		private string _pwd {get; set;} = "password";
		private XmlNamespaceManager _nsMgr;

		public QueriesController() {
			_nsMgr = new XmlNamespaceManager(new NameTable());
			_nsMgr.AddNamespace("mdsol", @"http://www.mdsol.com/ns/odm/metadata");
		}

		public List<CountDTO> Get(QueryState querystate = QueryState.Open) {
			var studies = new string[] { "PickettStudy1(Prod)", "Symposium-2016(Prod)" };

			var list = new List<CountDTO>();

			//-- Pull queries
			var pageSize = 1000;
			var lastId = (int?)null;
			var openTracker = new  List<MutableKeyValuePair<string, int>>(); var ansTracker = new List<MutableKeyValuePair<string, int>>();

			foreach (var stOID in studies) {
				//-- get list of open queries
				using (var client = new HttpClient()) {
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", _usr, _pwd))));
					var response = client.GetAsync(String.Format(@"https://hackathon2.mdsol.com/RaveWebServices/datasets/ClinicalAuditRecords.odm?studyoid={0}", stOID)).Result;

					var xmlDoc = new XmlDocument();
					xmlDoc.Load(response.Content.ReadAsStreamAsync().Result);

					var queryNodes = xmlDoc.SelectNodes("./ClinicalData[./SubjectData/StudyEventData/FormData/ItemGroupData/ItemData/mdsol:Query[@Recipient ='Site from CRA']]", _nsMgr);
					foreach (XmlNode node in queryNodes) {
						var key = string.Join(":",
							node.Attributes["StudyOID"].Value,
							node.SelectSingleNode("SubjectData").Attributes["SubjectKey"].Value,
							node.SelectSingleNode("StudyEventData").Attributes["mdsol:InstanceId"].Value,
							node.SelectSingleNode("FormData").Attributes["mdsol:DataPageId"].Value,
							node.SelectSingleNode("ItemGroupData").Attributes["mdsol:RecordId"].Value,
							node.SelectSingleNode("ItemData").Attributes["ItemOID"].Value,
							node.SelectSingleNode("mdsol:Query").Attributes["QueryRepeatKey"].Value
						);

						var qryStatus = node.SelectSingleNode("mdsol:Query").Attributes["Status"].Value;

						if (!openTracker.Any(t => t.Key.Equals(key))) openTracker.Add(new MutableKeyValuePair<string, int> { Key =key, Value = 0});
						if (!ansTracker.Any(t => t.Key.Equals(key))) ansTracker.Add(new MutableKeyValuePair<string, int> { Key = key, Value = 0 });

						var ot = openTracker.SingleOrDefault(t => t.Key.Equals(key));
						var at = ansTracker.SingleOrDefault(t => t.Key.Equals(key));

						switch (qryStatus) {
							case "Open": ot .Value++; break;
							case "Closed": ot.Value=0; at.Value = 0; break;
							case "Answered": at.Value++; break;
							default:
							break;
						}
					}

					foreach (var kvp in openTracker) {
						var kAry = kvp.Key.Split(':');

						list.Add(new CountDTO {
							StudyName = kAry[0],
							SponsorName = "TODO",
							SiteName = "TODO",
							OpenCount = kvp.Value,
							AnsweredCount = ansTracker.SingleOrDefault(t => t.Key.Equals(kvp.Key)).Value
						});
					}
				}
			}

			return list;

		}
    }
}
