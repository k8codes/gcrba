using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GCRBA.Classes;

namespace GCRBA.Models {
	public class Recovery {
		public String Email { get; set; }
		public String Username { get; set; }
		public String Hint { get; set; }
		public int TemporaryCode { get; set; }
		public String Password { get; set; }
		public DateTime TemporaryCodeCreated { get; set; }
		public String strTempCodeCreated { get; set; }
		public int UserId { get; set; }
		public byte[] Salt { get; set; }
		public byte[] btTemporaryCode { get; set; }
		public ActionTypes actionType { get; set; }

		public String GetUsername(string email) {
			String username = String.Empty;
			Models.Database db = new Database();
			username = db.GetUsernameRecovery(email);
			return username;
		}

		public int GetUserIdFromEmail(string email) {
			Models.Database db = new Database();
			UserId = db.GetUserIdFromEmail(email);
			return UserId;
		}

		public ActionTypes InsertTempCode() {
			Models.Database db = new Database();
			Classes.ObfuscatePwd obfuscator = new ObfuscatePwd();
			//obfuscator.ComplexObfuscateCredentialsForRecovery(this.TemporaryCode.ToString(), this);
			this.actionType = (ActionTypes)db.InsertTempCode(this);
			return actionType;
		}

		public void CheckTempCode() {
			Models.Database db = new Database();
			this.UserId = db.GetTempCode(this.TemporaryCode.ToString());
		}

		public enum ActionTypes {
			NoType = 0,
			Success = 1,
			NoEmailFound = 2,
			InsertFailed = 3
		}
	}
}