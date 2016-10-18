using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularJSWebApiEmpty.Models;
using Medidata.MAuth.Core;

namespace AngularJSWebApiEmpty.Controllers
{
    public class QueriesOldController : ApiController {
		private MAuthSigningHandler _mAuthSigningHdlr;
		private KeyValuePair<string, string> _mAuthUserHdr = new KeyValuePair<string, string>("MCC-Impersonate", "com:mdsol:users:14afae0c-9fe2-11df-a531-12313900d531");

		public QueriesOldController() {

			_mAuthSigningHdlr = new MAuthSigningHandler(new MAuthOptions() {
				ApplicationUuid = new Guid(ConfigurationManager.AppSettings["MAuth_Uuid1"]),
				PrivateKey = File.ReadAllText(ConfigurationManager.AppSettings["MAuth_PKeyFile1"]),
				MAuthServiceUrl = new Uri(ConfigurationManager.AppSettings["MAuth_Uri"])
			});

		}

		public List<CountDTO> Get(QueryState querystate = QueryState.Open) {
			var studyUuids = new string[] { "218da270-19a4-451e-a48b-05ef53183589" /*henserling*/, "4251b676-938f-4f7e-b7a4-ee09cd710ad6" /*Pickett*/, "7464bb54-5861-4e59-b72a-95cef298455f" /*Pickett*/};

			////-- example Get list of countries
			//var request = new HttpRequestMessage(HttpMethod.Get, "https://references-innovate.imedidata.com/v1/countries.json");

			////-- example Get lsit of studies
			//var request = new HttpRequestMessage(HttpMethod.Get, "https://plinth-innovate.imedidata.com/v1/studies?client_division_uuid=585379fb-d44d-4ac1-b9c2-9b502eb6730e");
			//request.Headers.Add("MCC-Impersonate", "com:mdsol:users:14afae0c-9fe2-11df-a531-12313900d531"); // Matthews UUID

			////-- example Get Data for a study 
			//var request = new HttpRequestMessage(HttpMethod.Get, "https://plinth-innovate.imedidata.com/v1/studies/4251b676-938f-4f7e-b7a4-ee09cd710ad6");
			//request.Headers.Add("MCC-Impersonate", "com:mdsol:users:68b1275e-6d66-11e0-8a85-12313b0a71bc");

			foreach (var uuid in studyUuids) {
				//-- Get Study Metadata (Name, Sponsor)
				var smd = StudyMetaData(uuid);
			}

			throw new NotImplementedException();
		}

		private List<StudyMetadataDTO> StudyMetaData(string uuid) {
			var req = new HttpRequestMessage(HttpMethod.Get, string.Format(@"https://plinth-innovate.imedidata.com/v1/studies/{0}", uuid));
			req.Headers.Add(_mAuthUserHdr.Key, _mAuthUserHdr.Value);

			using(var client = new HttpClient(_mAuthSigningHdlr)) {
				var json = client.SendAsync(req).Result.Content.ReadAsStringAsync();

			}
			return new List<StudyMetadataDTO> {
				new StudyMetadataDTO { StudyName = "PickettStudy", StudyUrl = "", StudyUuid="XXXXXXXX" },
				new StudyMetadataDTO { StudyName = "OtherStudy", StudyUrl = "", StudyUuid="XXXXXXXX" },
			};
		}
	}

	public enum QueryState { Open = 1, Answered = 2 }
}
