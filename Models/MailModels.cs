using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GCRBA.Models {
	public class MailModels {
		public String Subject { get; set; }
		public String From { get; set; }
		public String Recipient { get; set; }

		public String Title { get; set; }
		public String Url { get; set; }
		public String Description { get; set; }
	}

	public class CredentialRecovery : MailModels {
		public string Username { get; set; }
		public string TemporaryCode { get; set; }
		public CredentialRecovery() {
			Subject = "Username/Password Request";
			From = "GCRBAWebApp@donotreply";
			Title = "Return to Login Portal";
			Url = "\\Profile\\Login";
			Description = "If you did not request this information, your information may be compromised, please reset username/password";
		}
	}

	public class LocationAdminRequest : MailModels {
		public string UserFullName { get; set; }
		public string UserEmail { get; set; }
		public string UserTelephone { get; set; }
		public Models.LocationList Content { get; set; }

		public LocationAdminRequest() {
			Title = "Return to Admin Portal";
			Url = "http://localhost:62421/Profile/AdminLogin"; // { get; set; }
			Description = "Please review this new location request and approve/deny."; //{ get; set; }
			Recipient = "gcrbadata@gmail.com";
		}	
	}

	public class MemberAdminRequest : MailModels {
		public string UserFullName { get; set; }
		public string UserEmail { get; set; }
		public string UserTelephone { get; set; }
		public Models.User Content { get; set; }
		public MemberAdminRequest() {
			Subject = "New Member Request";
			From = "GCRBAWebApp@donotreply";
			Title = "Return to Admin Portal";// { get; set; }
			Url = "http://localhost:62421/Profile/AdminLogin"; // { get; set; }
			Description = "Please review this new member request and approve/deny."; //{ get; set; }
			Recipient = "gcrbadata@gmail.com";
		}
	}

	public class SendNewsletterRequest : MailModels {
		public Image Image { get; set; }
		public String FirstName { get; set; }
		public SendNewsletterRequest() {
			Subject = "GCRBA New Newsletter!";
			From = "GCRBAWebApp@donotreply";
			Description = "To unsubscribe from our newsletter, please modify the setting within your user profile";
		}
	}
}