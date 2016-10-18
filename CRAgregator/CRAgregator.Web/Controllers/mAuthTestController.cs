using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Medidata.MAuth.Core;

namespace AngularJSWebApiEmpty.Controllers {
    public class mAuthTestController : ApiController {
		private MAuthSigningHandler _mAuthSigningHdlr;
		private KeyValuePair<string, string> _mAuthUserHdr = new KeyValuePair<string, string>("MCC-Impersonate", "com:mdsol:users:14afae0c-9fe2-11df-a531-12313900d531");

		public mAuthTestController() {

			_mAuthSigningHdlr = new MAuthSigningHandler(new MAuthOptions() {
				ApplicationUuid = new Guid(ConfigurationManager.AppSettings["MAuth_Uuid1"]),
				PrivateKey = File.ReadAllText(ConfigurationManager.AppSettings["MAuth_PKeyFile1"]),
				MAuthServiceUrl = new Uri(ConfigurationManager.AppSettings["MAuth_Uri"])
			});

		}

		public string GetStudy() {
			//-- example Get Data for a study 
			var request = new HttpRequestMessage(HttpMethod.Get, "https://plinth-innovate.imedidata.com/v1/studies/4251b676-938f-4f7e-b7a4-ee09cd710ad6");
			request.Headers.Add("MCC-Impersonate", "com:mdsol:users:68b1275e-6d66-11e0-8a85-12313b0a71bc");

			using (var client = new HttpClient(_mAuthSigningHdlr)) {
				var str = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result.ToString();



				//return new List<CountDTO> { new CountDTO { StudyId = 1, StudyName = str, SiteName = querystate.ToString("G") + " Site", SponsorName = querystate.ToString("G")+" Sponsor", Count = 0 } }; }
				return str;
			}
		}
	}
}
