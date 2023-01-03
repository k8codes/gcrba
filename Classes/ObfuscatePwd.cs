using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Konscious.Security.Cryptography;

namespace GCRBA.Classes {
	public class ObfuscatePwd {
		private static String obfuscateId(String id, int shift) {
			int maxChar = Char.MaxValue;
			int minChar = Char.MinValue;
			char[] buffer = id.ToCharArray();
			for (int i = 0; i < buffer.Length; i++) {
				int newEncodedChar = buffer[i] + shift;
				if (newEncodedChar > maxChar)
					newEncodedChar -= maxChar;
				else if (newEncodedChar < minChar)
					newEncodedChar += maxChar;
				if (newEncodedChar == 39)
					newEncodedChar = 32;
				buffer[i] = (char)newEncodedChar;
			}
			return new String(buffer);
		}
		private byte[] CreateSalt() {
			var buffer = new byte[24];
			var rng = new RNGCryptoServiceProvider();
			rng.GetBytes(buffer);
			return buffer;
		}

		private byte[] HashPassword(string password, byte[] salt) {
			var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

			argon2.Salt = salt;
			argon2.DegreeOfParallelism = 2; // 2 cores
			argon2.Iterations = 2;
			argon2.MemorySize = 564 * 564; // 1 GB

			return argon2.GetBytes(24);
		}

		private bool VerifyHash(string password, byte[] salt, byte[] hash) {
			var newHash = HashPassword(password, salt);
			return hash.SequenceEqual(newHash);
		}

		public Models.User ComplexObfuscateCredentials(String id, Models.User u) {
			String obfuscatedId = obfuscateId(id, -7);
			var salt = CreateSalt();
			var hash = HashPassword(obfuscatedId, salt);
			u.salt = salt;
			u.Password = hash;
			return u;
		}

		public Models.Recovery ComplexObfuscateCredentialsForRecovery(String id, Models.Recovery r) {
			String obfuscatedId = obfuscateId(id, -7);
			var salt = CreateSalt();
			var hash = HashPassword(obfuscatedId, salt);
			r.Salt = salt;
			r.btTemporaryCode = hash;
			return r;
		}

		public String SimpleObfuscateCredentials(String id) {
			String obfuscatedId = obfuscateId(id, -7);
			return obfuscatedId;
		}

		public bool CheckPassword(String password, byte[] hash, byte[] salt) { /*String strSalt*/
			bool validated = false;
			String obfuscatedId = obfuscateId(password, -7);
			validated = VerifyHash(obfuscatedId, salt, hash);
			return validated; 
		}

		public String SimpleDecrypt(String id) {
			String decryptedId = String.Empty;
			decryptedId = obfuscateId(id, 7);
			return decryptedId;
		}
	}
}