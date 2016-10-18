using System;
using System.Collections.Generic;
using System.Data;
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
			_nsMgr.AddNamespace(string.Empty, @"http://www.cdisc.org/ns/odm/v1.3");
			_nsMgr.AddNamespace("mdsol", @"http://www.mdsol.com/ns/odm/metadata");
		}

		public List<CountDTO> Get(QueryState querystate = QueryState.Open) {
			var studies = new string[] { "PickettStudy1(Prod)", "Symposium-2016(Prod)" };
			var sponsors = new string[] { "Drug, Inc.", "MedCo." };

			var list = new List<CountDTO>();

			//-- Pull queries
			var pageSize = 1000;
			var lastId = (int?)null;
			//var openTracker = new  List<MutableKeyValuePair<string, int>>(); var ansTracker = new List<MutableKeyValuePair<string, int>>();
			var trackers = new List<QueryTracker>();

			foreach (var stOID in studies) {
				//-- get list of open queries
				using (var client = new HttpClient()) {
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", _usr, _pwd))));
					var response = client.GetAsync(String.Format(@"https://hackathon2.mdsol.com/RaveWebServices/datasets/ClinicalAuditRecords.odm?studyoid={0}", stOID)).Result;

					var xmlDoc = new XmlDocument();
					xmlDoc.Load(response.Content.ReadAsStreamAsync().Result);

					DataSet ds = new DataSet();
					XmlNodeReader xmlReader = new XmlNodeReader(xmlDoc);
					ds.ReadXml(xmlReader);

					var queries = ds.Tables["Query"].AsEnumerable();

					var nodeqry = ds.Tables["Query"].AsEnumerable()
						.Where(q => q.Field<string>("Recipient").Equals("Site from CRA"))
						.Select(q => new { QueryStatus = q.Field<string>("Status"), QueryRepeatKey = q.Field<string>("QueryRepeatKey"), ID_Id = q.Field<int>("ItemData_Id") })
						.Join(ds.Tables["ItemData"].AsEnumerable(), rslt =>rslt.ID_Id, id => id.Field<int>("ItemData_Id"),
							(rslt, id) => new { rslt.QueryStatus, rslt.QueryRepeatKey, ItemOID = id.Field<string>("ItemOID"), ID_Id = id.Field<int>("ItemData_Id"), IGD_Id = id.Field<int>("ItemGroupData_Id") })
						.Join(ds.Tables["ItemGroupData"].AsEnumerable(), rslt => rslt.IGD_Id, igd => igd.Field<int>("ItemGroupData_Id"),
							(rslt, igd) => new { rslt.QueryStatus, rslt.QueryRepeatKey, rslt.ItemOID, RecordId = igd.Field<string>("RecordId"), FD_Id = igd.Field<int>("FormData_Id") })
						.Join(ds.Tables["FormData"].AsEnumerable(), rslt => rslt.FD_Id, fd => fd.Field<int>("FormData_Id"),
							(rslt, fd) => new { rslt.QueryStatus, rslt.QueryRepeatKey, rslt.ItemOID, rslt.RecordId, DPG_Id = fd.Field<string>("DataPageId"), SED_Id = fd.Field<int>("StudyEventData_Id") })
						.Join(ds.Tables["StudyEventData"].AsEnumerable(), rslt => rslt.SED_Id, sed => sed.Field<int>("StudyEventData_Id"),
							(rslt, sed) => new { rslt.QueryStatus, rslt.QueryRepeatKey, rslt.ItemOID, rslt.RecordId, rslt.DPG_Id, InsatnceId = sed.Field<string>("InstanceId"), SD_Id = sed.Field<int>("SubjectData_Id") })
						.Join(ds.Tables["SiteRef"].AsEnumerable(), rslt => rslt.SD_Id, sr => sr.Field<int?>("SubjectData_Id"),
							(rslt, sr) => new { rslt.QueryStatus, rslt.QueryRepeatKey, rslt.ItemOID, SiteRef = sr.Field<string>("LocationOID"),rslt.SD_Id, rslt.RecordId, rslt.DPG_Id, rslt.InsatnceId })
						.Join(ds.Tables["SubjectData"].AsEnumerable(), rslt => rslt.SD_Id, sd => sd.Field<int>("SubjectData_Id"),
							(rslt, sd) => new { rslt.QueryStatus, rslt.QueryRepeatKey, rslt.ItemOID, rslt.SiteRef, rslt.RecordId, rslt.DPG_Id, rslt.InsatnceId, SbjKey = sd.Field<string>("SubjectKey"), CD_Id = sd.Field<int>("ClinicalData_Id") })
						.Join(ds.Tables["ClinicalData"].AsEnumerable(), rslt => rslt.CD_Id, cd => cd.Field<int>("ClinicalData_Id"),
							(rslt, cd) => new AuditDTO {
								QueryStatus = rslt.QueryStatus,
								QueryRepeatKey =  rslt.QueryRepeatKey,
								SiteRef = rslt.SiteRef,
								ItemOID = rslt.ItemOID,
								RecordID = rslt.RecordId,
								DataPageID = rslt.DPG_Id,
								InstanceID = rslt.InsatnceId,
								SubjectKey =  rslt.SbjKey,
								Study = cd.Field<string>("StudyOID")
							});


					//XmlNode root = xmlDoc.DocumentElement;

					//foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes) {

					var queryNodes = nodeqry.ToList(); // xmlDoc.SelectNodes("./ClinicalData[./SubjectData/StudyEventData/FormData/ItemGroupData/ItemData/mdsol:Query[@Recipient ='Site from CRA']]", _nsMgr);
					for(int i =0; i< queryNodes.Count; i++) {// (var node in queryNodes) {
						var node = queryNodes[i];

						//Console.WriteLine(node.Name);
						//try {
						//if (node["SubjectData"]["StudyEventData"]["FormData"]["ItemGroupData"]["ItemData"]["mdsol:Query"].Attributes["Recipient"].InnerText.Contains("Site from CRA")) {
						//This is your node
						//var key = string.Join(":", node.Study);
						//node.Attributes["StudyOID"].Value,
						//node.SelectSingleNode("SubjectData").Attributes["SubjectKey"].Value,
						//node.SelectSingleNode("StudyEventData").Attributes["mdsol:InstanceId"].Value,
						//node.SelectSingleNode("FormData").Attributes["mdsol:DataPageId"].Value,
						//node.SelectSingleNode("ItemGroupData").Attributes["mdsol:RecordId"].Value,
						//node.SelectSingleNode("ItemData").Attributes["ItemOID"].Value,
						//node.SelectSingleNode("mdsol:Query").Attributes["QueryRepeatKey"].Value
						//);

						//var qryStatus = "";// node.SelectSingleNode("mdsol:Query").Attributes["Status"].Value;



						//if (!openTracker.Any(t => t.Key.Equals(node.QueryKey))) openTracker.Add(new MutableKeyValuePair<string, int> { Key = node.QueryKey, Value = 0 });
						//if (!ansTracker.Any(t => t.Key.Equals(node.QueryKey))) ansTracker.Add(new MutableKeyValuePair<string, int> { Key = node.QueryKey, Value = 0 });

						//var ot = openTracker.SingleOrDefault(t => t.Key.Equals(node.QueryKey));
						//var at = ansTracker.SingleOrDefault(t => t.Key.Equals(node.QueryKey));

						if (!trackers.Any(t => t.UniqueID.Equals(node.QueryKey))) trackers.Add(new QueryTracker { Study = node.Study, SiteRef = node.SiteRef, UniqueID = node.QueryKey });
						var ct = trackers.SingleOrDefault(t => t.UniqueID.Equals(node.QueryKey));

								switch (node.QueryStatus) {
									case "Open": ct.Open = true; ct.Answered = false; break;
									case "Closed": ct.Open = false; ct.Answered = false; break;
									case "Answered": ct.Open = false; ct.Answered = true; break;
									default:
									break;
								}
							//}

						//} catch { }
					}

					//-- Group to the site
					trackers.GroupBy(t => new { t.Study, t.SiteRef },
						(key, data) => new CountDTO {
							StudyName = key.Study,
							SponsorName = sponsors[Array.IndexOf(studies,key.Study)],
							SiteName = key.SiteRef,
							OpenCount = data.Count(d => d.Open),
							AnsweredCount = data.Count(d => d.Answered)
						}).Distinct()
						.ToList()
						.ForEach(cdto => list.Add(cdto));

					//foreach (var kvp in openTracker) {
					//	var kAry = kvp.Key.Split(':');

					//	list.Add(new CountDTO {
					//		StudyName = kAry[0],
					//		SponsorName = "TODO",
					//		SiteName = "TODO",
					//		OpenCount = kvp.Value,
					//		AnsweredCount = ansTracker.SingleOrDefault(t => t.Key.Equals(kvp.Key)).Value
					//	});
					//}
				}
			}

			return list;

		}
    }
}
