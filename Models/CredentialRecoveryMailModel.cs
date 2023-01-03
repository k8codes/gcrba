using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GCRBA.Models {
	public class CredentialRecoveryMailModel {
		public string Subject = "Username/Password Request";
		public string From = "GCRBAWebApp@donotreply";
		public string Title = "Return to Login Portal";
		public string Url = "\\Profile\\Login";
		public string Description = "If you did not request this information, your information may be compromised, please reset username/password";
		public string Recipient { get; set; }
		public string Username { get; set; }
		public string TemporaryCode { get; set; }
	}


}