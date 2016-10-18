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

namespace AngularJSWebApiEmpty.Controllers
{
	public class QueriesController : ApiController {
		private string _usr {get; set;}= "defuser";
		private string _pwd {get; set;} = "password";

		public QueriesController() { }

		public List<CountDTO> Get(QueryState querystate) {
			var studies = new string[] { "PickettStudy1(Prod)", "Symposium-2016(Prod)" };

			var list = new List<CountDTO>();

			//-- Pull queries
			var pageSize = 1000;
			var lastId = (int?)null;

			foreach (var stOID in studies) {
				//-- get list of open queries
				using (var client = new HttpClient()) {
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_usr}:{_pwd}")));
					var response = client.GetAsync(String.Format(@"https://hackathon2.mdsol.com/RaveWebServices/datasets/ClinicalAuditRecords.odm?studyoid={0}", stOID)).Result;

					var xmlDoc = new XmlDocument();
					xmlDoc.Load(response.Content.ReadAsStreamAsync().Result);

					var queryNodes = xmlDoc.SelectNodes("descendant::mdsol:Query");

					foreach (XmlNode node in queryNodes) {
						list.Add(new CountDTO {
							Count = Int32.Parse(node.Attributes["QueryRepeatKey"].Value),
							SiteName = node.Attributes["Value"].Value,
							SponsorName = node.Attributes["Status"].Value


						});
					}
				}
			}

			return list;

		}
    }
}
