using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Cryptographic;

namespace Azyre.Utils
{
	// Token: 0x0200002E RID: 46
	public class api
	{
		// Token: 0x060001ED RID: 493
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

		// Token: 0x060001EE RID: 494
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetCurrentProcess();

		// Token: 0x060001EF RID: 495
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern ushort GlobalAddAtom(string lpString);

		// Token: 0x060001F0 RID: 496
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern ushort GlobalFindAtom(string lpString);

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000D411 File Offset: 0x0000B611
		// (set) Token: 0x060001F2 RID: 498 RVA: 0x0000D418 File Offset: 0x0000B618
		public static api Static { get; set; }

		// Token: 0x060001F3 RID: 499 RVA: 0x0000D420 File Offset: 0x0000B620
		public api(string name, string ownerid, string version, string path = null)
		{
			api.Static = this;
			this.name = name;
			this.ownerid = ownerid;
			this.version = version;
			this.path = path;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000D488 File Offset: 0x0000B688
		public void init()
		{
			Random random = new Random();
			int num = random.Next(5, 51);
			StringBuilder stringBuilder = new StringBuilder(num);
			for (int i = 0; i < num; i++)
			{
				char value = (char)random.Next(32, 127);
				stringBuilder.Append(value);
			}
			this.seed = stringBuilder.ToString();
			this.checkAtom();
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = "init";
			nameValueCollection["ver"] = this.version;
			nameValueCollection["hash"] = api.checksum(Process.GetCurrentProcess().MainModule.FileName);
			nameValueCollection["name"] = this.name;
			nameValueCollection["ownerid"] = this.ownerid;
			NameValueCollection nameValueCollection2 = nameValueCollection;
			if (!string.IsNullOrEmpty(this.path))
			{
				nameValueCollection2.Add("token", File.ReadAllText(this.path));
				nameValueCollection2.Add("thash", api.TokenHash(this.path));
			}
			string text = api.req(nameValueCollection2);
			if (text == "KeyAuth_Invalid")
			{
				api.error("Application not found");
				api.TerminateProcess(api.GetCurrentProcess(), 1U);
			}
			api.response_structure response_structure = this.response_decoder.string_to_generic<api.response_structure>(text);
			if (response_structure.ownerid == this.ownerid)
			{
				this.load_response_struct(response_structure);
				if (response_structure.success)
				{
					api.sessionid = response_structure.sessionid;
					this.initialized = true;
					return;
				}
				if (response_structure.message == "invalidver")
				{
					this.app_data.downloadLink = response_structure.download;
					return;
				}
			}
			else
			{
				api.TerminateProcess(api.GetCurrentProcess(), 1U);
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000D62B File Offset: 0x0000B82B
		public bool IsInitialized()
		{
			return this.initialized;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000D634 File Offset: 0x0000B834
		public string webhook(string webid, string param, string body = "", string conttype = "application/json")
		{
			this.CheckInit();
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = "webhook";
			nameValueCollection["webid"] = webid;
			nameValueCollection["params"] = param;
			nameValueCollection["body"] = body;
			nameValueCollection["conttype"] = conttype;
			nameValueCollection["sessionid"] = api.sessionid;
			nameValueCollection["name"] = this.name;
			nameValueCollection["ownerid"] = this.ownerid;
			string json = api.req(nameValueCollection);
			api.response_structure response_structure = this.response_decoder.string_to_generic<api.response_structure>(json);
			this.load_response_struct(response_structure);
			if (response_structure.success)
			{
				return response_structure.contents;
			}
			return null;
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000D6EC File Offset: 0x0000B8EC
		public void setvar(string varid, string data)
		{
			this.CheckInit();
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = "setvar";
			nameValueCollection["var"] = varid;
			nameValueCollection["data"] = data;
			nameValueCollection["sessionid"] = api.sessionid;
			nameValueCollection["name"] = this.name;
			nameValueCollection["ownerid"] = this.ownerid;
			string json = api.req(nameValueCollection);
			api.response_structure data2 = this.response_decoder.string_to_generic<api.response_structure>(json);
			this.load_response_struct(data2);
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000D778 File Offset: 0x0000B978
		private void checkAtom()
		{
			new Thread(delegate()
			{
				for (;;)
				{
					Thread.Sleep(60000);
					if (api.GlobalFindAtom(this.seed) == 0)
					{
						api.TerminateProcess(api.GetCurrentProcess(), 1U);
					}
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000D798 File Offset: 0x0000B998
		public static string TokenHash(string tokenPath)
		{
			string result;
			using (SHA256 sha = SHA256.Create())
			{
				using (FileStream fileStream = File.OpenRead(tokenPath))
				{
					result = BitConverter.ToString(sha.ComputeHash(fileStream)).Replace("-", string.Empty);
				}
			}
			return result;
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000D804 File Offset: 0x0000BA04
		public void CheckInit()
		{
			if (!this.initialized)
			{
				api.error("Unable to initialize Auth");
				api.TerminateProcess(api.GetCurrentProcess(), 1U);
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000D824 File Offset: 0x0000BA24
		public void register(string username, string pass, string key, string email = "")
		{
			this.CheckInit();
			string hwid = Program.GetHwid();
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = "register";
			nameValueCollection["username"] = username;
			nameValueCollection["pass"] = pass;
			nameValueCollection["key"] = key;
			nameValueCollection["email"] = email;
			nameValueCollection["hwid"] = hwid;
			nameValueCollection["sessionid"] = api.sessionid;
			nameValueCollection["name"] = this.name;
			nameValueCollection["ownerid"] = this.ownerid;
			string json = api.req(nameValueCollection);
			api.response_structure response_structure = this.response_decoder.string_to_generic<api.response_structure>(json);
			if (response_structure.ownerid == this.ownerid)
			{
				api.GlobalAddAtom(this.seed);
				api.GlobalAddAtom(this.ownerid);
				this.load_response_struct(response_structure);
				if (response_structure.success)
				{
					this.load_user_data(response_structure.info);
					return;
				}
			}
			else
			{
				api.TerminateProcess(api.GetCurrentProcess(), 1U);
			}
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000D928 File Offset: 0x0000BB28
		public void login(string username, string pass)
		{
			this.CheckInit();
			string hwid = Program.GetHwid();
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = "login";
			nameValueCollection["username"] = username;
			nameValueCollection["pass"] = pass;
			nameValueCollection["hwid"] = hwid;
			nameValueCollection["sessionid"] = api.sessionid;
			nameValueCollection["name"] = this.name;
			nameValueCollection["ownerid"] = this.ownerid;
			string json = api.req(nameValueCollection);
			api.response_structure response_structure = this.response_decoder.string_to_generic<api.response_structure>(json);
			if (response_structure.ownerid == this.ownerid)
			{
				api.GlobalAddAtom(this.seed);
				api.GlobalAddAtom(this.ownerid);
				this.load_response_struct(response_structure);
				if (response_structure.success)
				{
					this.load_user_data(response_structure.info);
					return;
				}
			}
			else
			{
				api.TerminateProcess(api.GetCurrentProcess(), 1U);
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000DA14 File Offset: 0x0000BC14
		public string var(string varid)
		{
			this.CheckInit();
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = "var";
			nameValueCollection["varid"] = varid;
			nameValueCollection["sessionid"] = api.sessionid;
			nameValueCollection["name"] = this.name;
			nameValueCollection["ownerid"] = this.ownerid;
			string json = api.req(nameValueCollection);
			api.response_structure response_structure = this.response_decoder.string_to_generic<api.response_structure>(json);
			if (response_structure.ownerid == this.ownerid)
			{
				this.load_response_struct(response_structure);
				if (response_structure.success)
				{
					return response_structure.message;
				}
			}
			else
			{
				api.TerminateProcess(api.GetCurrentProcess(), 1U);
			}
			return null;
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000DAC4 File Offset: 0x0000BCC4
		public byte[] download(string fileid)
		{
			this.CheckInit();
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = "file";
			nameValueCollection["fileid"] = fileid;
			nameValueCollection["sessionid"] = api.sessionid;
			nameValueCollection["name"] = this.name;
			nameValueCollection["ownerid"] = this.ownerid;
			string json = api.req(nameValueCollection);
			api.response_structure response_structure = this.response_decoder.string_to_generic<api.response_structure>(json);
			this.load_response_struct(response_structure);
			if (response_structure.success)
			{
				return encryption.str_to_byte_arr(response_structure.contents);
			}
			return null;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000DB5C File Offset: 0x0000BD5C
		public void log(string message)
		{
			this.CheckInit();
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = "log";
			nameValueCollection["pcuser"] = Environment.UserName;
			nameValueCollection["message"] = message;
			nameValueCollection["sessionid"] = api.sessionid;
			nameValueCollection["name"] = this.name;
			nameValueCollection["ownerid"] = this.ownerid;
			api.req(nameValueCollection);
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000DBD8 File Offset: 0x0000BDD8
		public void changeUsername(string username)
		{
			this.CheckInit();
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["type"] = "changeUsername";
			nameValueCollection["newUsername"] = username;
			nameValueCollection["sessionid"] = api.sessionid;
			nameValueCollection["name"] = this.name;
			nameValueCollection["ownerid"] = this.ownerid;
			string json = api.req(nameValueCollection);
			api.response_structure data = this.response_decoder.string_to_generic<api.response_structure>(json);
			this.load_response_struct(data);
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000DC58 File Offset: 0x0000BE58
		public static string checksum(string filename)
		{
			string result;
			using (MD5 md = MD5.Create())
			{
				using (FileStream fileStream = File.OpenRead(filename))
				{
					result = BitConverter.ToString(md.ComputeHash(fileStream)).Replace("-", "").ToLowerInvariant();
				}
			}
			return result;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000DCC8 File Offset: 0x0000BEC8
		public static void error(string message)
		{
			try
			{
				if (api.Static.initialized)
				{
					api.Static.log("KeyauthERROR - " + message);
				}
			}
			catch
			{
			}
			Process.Start(new ProcessStartInfo("cmd.exe", "/c start cmd /C \"color b && title Error && echo " + message + " && timeout /t 5\"")
			{
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false
			});
			api.TerminateProcess(api.GetCurrentProcess(), 1U);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000DD54 File Offset: 0x0000BF54
		private static string req(NameValueCollection post_data)
		{
			string result;
			try
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.Proxy = null;
					ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(api.assertSSL));
					byte[] bytes = webClient.UploadValues("https://keyauth.win/api/1.3/ ", post_data);
					ServicePointManager.ServerCertificateValidationCallback = ((object <p0>, X509Certificate <p1>, X509Chain <p2>, SslPolicyErrors <p3>) => true);
					api.sigCheck(Encoding.UTF8.GetString(bytes), webClient.ResponseHeaders, post_data.Get(0));
					result = Encoding.Default.GetString(bytes);
				}
			}
			catch (WebException ex)
			{
				if (((HttpWebResponse)ex.Response).StatusCode == (HttpStatusCode)429)
				{
					api.error("You're connecting too fast to loader, slow down.");
					api.TerminateProcess(api.GetCurrentProcess(), 1U);
					result = "";
				}
				else
				{
					api.error("Connection failure. Please try again, or contact us for help.");
					api.TerminateProcess(api.GetCurrentProcess(), 1U);
					result = "";
				}
			}
			return result;
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000DE64 File Offset: 0x0000C064
		private static bool assertSSL(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if ((!certificate.Issuer.Contains("Google Trust Services") && !certificate.Issuer.Contains("Let's Encrypt")) || sslPolicyErrors != SslPolicyErrors.None)
			{
				api.error("SSL assertion fail, make sure you're not debugging Network. Disable internet firewall on router if possible. & echo: & echo If not, ask the developer of the program to use custom domains to fix this.");
				return false;
			}
			return true;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000DE9C File Offset: 0x0000C09C
		private static void sigCheck(string resp, WebHeaderCollection headers, string type)
		{
			if (type == "log" || type == "file")
			{
				return;
			}
			try
			{
				string hex = headers["x-signature-ed25519"];
				string text = headers["x-signature-timestamp"];
				long seconds;
				if (!long.TryParse(text, out seconds))
				{
					api.TerminateProcess(api.GetCurrentProcess(), 1U);
				}
				DateTime utcDateTime = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
				if ((DateTime.UtcNow - utcDateTime).TotalSeconds > 20.0)
				{
					api.TerminateProcess(api.GetCurrentProcess(), 1U);
				}
				byte[] signature = encryption.str_to_byte_arr(hex);
				byte[] publicKey = encryption.str_to_byte_arr("5586b4bc69c7a4b487e4563a4cd96afd39140f919bd31cea7d1c6a1e8439422b");
				string s = text + resp;
				byte[] bytes = Encoding.Default.GetBytes(s);
				if (!Ed25519.CheckValid(signature, bytes, publicKey))
				{
					api.error("Signature checksum failed. Request was tampered with or session ended most likely. & echo: & echo Response: " + resp);
					api.TerminateProcess(api.GetCurrentProcess(), 1U);
				}
			}
			catch
			{
				api.error("Signature checksum failed. Request was tampered with or session ended most likely. & echo: & echo Response: " + resp);
				api.TerminateProcess(api.GetCurrentProcess(), 1U);
			}
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000DFAC File Offset: 0x0000C1AC
		private void load_user_data(api.user_data_structure data)
		{
			this.user_data.username = data.username;
			this.user_data.ip = data.ip;
			this.user_data.hwid = data.hwid;
			this.user_data.createdate = data.createdate;
			this.user_data.lastlogin = data.lastlogin;
			this.user_data.subscriptions = data.subscriptions;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000E01F File Offset: 0x0000C21F
		private void load_response_struct(api.response_structure data)
		{
			this.response.success = data.success;
			this.response.message = data.message;
		}

		// Token: 0x0400014F RID: 335
		public string name;

		// Token: 0x04000150 RID: 336
		public string ownerid;

		// Token: 0x04000151 RID: 337
		public string version;

		// Token: 0x04000152 RID: 338
		public string path;

		// Token: 0x04000153 RID: 339
		public string seed;

		// Token: 0x04000155 RID: 341
		private static string sessionid;

		// Token: 0x04000156 RID: 342
		private static string enckey;

		// Token: 0x04000157 RID: 343
		private bool initialized;

		// Token: 0x04000158 RID: 344
		public api.app_data_class app_data = new api.app_data_class();

		// Token: 0x04000159 RID: 345
		public api.user_data_class user_data = new api.user_data_class();

		// Token: 0x0400015A RID: 346
		public api.response_class response = new api.response_class();

		// Token: 0x0400015B RID: 347
		private json_wrapper response_decoder = new json_wrapper(new api.response_structure());

		// Token: 0x0200002F RID: 47
		[DataContract]
		private class response_structure
		{
			// Token: 0x170000B1 RID: 177
			// (get) Token: 0x06000209 RID: 521 RVA: 0x0000E069 File Offset: 0x0000C269
			// (set) Token: 0x0600020A RID: 522 RVA: 0x0000E071 File Offset: 0x0000C271
			[DataMember]
			public bool success { get; set; }

			// Token: 0x170000B2 RID: 178
			// (get) Token: 0x0600020B RID: 523 RVA: 0x0000E07A File Offset: 0x0000C27A
			// (set) Token: 0x0600020C RID: 524 RVA: 0x0000E082 File Offset: 0x0000C282
			[DataMember]
			public string sessionid { get; set; }

			// Token: 0x170000B3 RID: 179
			// (get) Token: 0x0600020D RID: 525 RVA: 0x0000E08B File Offset: 0x0000C28B
			// (set) Token: 0x0600020E RID: 526 RVA: 0x0000E093 File Offset: 0x0000C293
			[DataMember]
			public string contents { get; set; }

			// Token: 0x170000B4 RID: 180
			// (get) Token: 0x0600020F RID: 527 RVA: 0x0000E09C File Offset: 0x0000C29C
			// (set) Token: 0x06000210 RID: 528 RVA: 0x0000E0A4 File Offset: 0x0000C2A4
			[DataMember]
			public string message { get; set; }

			// Token: 0x170000B5 RID: 181
			// (get) Token: 0x06000211 RID: 529 RVA: 0x0000E0AD File Offset: 0x0000C2AD
			// (set) Token: 0x06000212 RID: 530 RVA: 0x0000E0B5 File Offset: 0x0000C2B5
			[DataMember]
			public string ownerid { get; set; }

			// Token: 0x170000B6 RID: 182
			// (get) Token: 0x06000213 RID: 531 RVA: 0x0000E0BE File Offset: 0x0000C2BE
			// (set) Token: 0x06000214 RID: 532 RVA: 0x0000E0C6 File Offset: 0x0000C2C6
			[DataMember]
			public string download { get; set; }

			// Token: 0x170000B7 RID: 183
			// (get) Token: 0x06000215 RID: 533 RVA: 0x0000E0CF File Offset: 0x0000C2CF
			// (set) Token: 0x06000216 RID: 534 RVA: 0x0000E0D7 File Offset: 0x0000C2D7
			[DataMember(IsRequired = false, EmitDefaultValue = false)]
			public api.user_data_structure info { get; set; }
		}

		// Token: 0x02000030 RID: 48
		[DataContract]
		private class user_data_structure
		{
			// Token: 0x170000B8 RID: 184
			// (get) Token: 0x06000218 RID: 536 RVA: 0x0000E0E0 File Offset: 0x0000C2E0
			// (set) Token: 0x06000219 RID: 537 RVA: 0x0000E0E8 File Offset: 0x0000C2E8
			[DataMember]
			public string username { get; set; }

			// Token: 0x170000B9 RID: 185
			// (get) Token: 0x0600021A RID: 538 RVA: 0x0000E0F1 File Offset: 0x0000C2F1
			// (set) Token: 0x0600021B RID: 539 RVA: 0x0000E0F9 File Offset: 0x0000C2F9
			[DataMember]
			public string ip { get; set; }

			// Token: 0x170000BA RID: 186
			// (get) Token: 0x0600021C RID: 540 RVA: 0x0000E102 File Offset: 0x0000C302
			// (set) Token: 0x0600021D RID: 541 RVA: 0x0000E10A File Offset: 0x0000C30A
			[DataMember]
			public string hwid { get; set; }

			// Token: 0x170000BB RID: 187
			// (get) Token: 0x0600021E RID: 542 RVA: 0x0000E113 File Offset: 0x0000C313
			// (set) Token: 0x0600021F RID: 543 RVA: 0x0000E11B File Offset: 0x0000C31B
			[DataMember]
			public string createdate { get; set; }

			// Token: 0x170000BC RID: 188
			// (get) Token: 0x06000220 RID: 544 RVA: 0x0000E124 File Offset: 0x0000C324
			// (set) Token: 0x06000221 RID: 545 RVA: 0x0000E12C File Offset: 0x0000C32C
			[DataMember]
			public string lastlogin { get; set; }

			// Token: 0x170000BD RID: 189
			// (get) Token: 0x06000222 RID: 546 RVA: 0x0000E135 File Offset: 0x0000C335
			// (set) Token: 0x06000223 RID: 547 RVA: 0x0000E13D File Offset: 0x0000C33D
			[DataMember]
			public List<api.Data> subscriptions { get; set; }
		}

		// Token: 0x02000031 RID: 49
		public class app_data_class
		{
			// Token: 0x170000BE RID: 190
			// (get) Token: 0x06000225 RID: 549 RVA: 0x0000E146 File Offset: 0x0000C346
			// (set) Token: 0x06000226 RID: 550 RVA: 0x0000E14E File Offset: 0x0000C34E
			public string downloadLink { get; set; }
		}

		// Token: 0x02000032 RID: 50
		public class user_data_class
		{
			// Token: 0x170000BF RID: 191
			// (get) Token: 0x06000228 RID: 552 RVA: 0x0000E157 File Offset: 0x0000C357
			// (set) Token: 0x06000229 RID: 553 RVA: 0x0000E15F File Offset: 0x0000C35F
			public string username { get; set; }

			// Token: 0x170000C0 RID: 192
			// (get) Token: 0x0600022A RID: 554 RVA: 0x0000E168 File Offset: 0x0000C368
			// (set) Token: 0x0600022B RID: 555 RVA: 0x0000E170 File Offset: 0x0000C370
			public string ip { get; set; }

			// Token: 0x170000C1 RID: 193
			// (get) Token: 0x0600022C RID: 556 RVA: 0x0000E179 File Offset: 0x0000C379
			// (set) Token: 0x0600022D RID: 557 RVA: 0x0000E181 File Offset: 0x0000C381
			public string hwid { get; set; }

			// Token: 0x170000C2 RID: 194
			// (get) Token: 0x0600022E RID: 558 RVA: 0x0000E18A File Offset: 0x0000C38A
			// (set) Token: 0x0600022F RID: 559 RVA: 0x0000E192 File Offset: 0x0000C392
			public string createdate { get; set; }

			// Token: 0x170000C3 RID: 195
			// (get) Token: 0x06000230 RID: 560 RVA: 0x0000E19B File Offset: 0x0000C39B
			// (set) Token: 0x06000231 RID: 561 RVA: 0x0000E1A3 File Offset: 0x0000C3A3
			public string lastlogin { get; set; }

			// Token: 0x170000C4 RID: 196
			// (get) Token: 0x06000232 RID: 562 RVA: 0x0000E1AC File Offset: 0x0000C3AC
			// (set) Token: 0x06000233 RID: 563 RVA: 0x0000E1B4 File Offset: 0x0000C3B4
			public List<api.Data> subscriptions { get; set; }
		}

		// Token: 0x02000033 RID: 51
		public class Data
		{
			// Token: 0x170000C5 RID: 197
			// (get) Token: 0x06000235 RID: 565 RVA: 0x0000E1BD File Offset: 0x0000C3BD
			// (set) Token: 0x06000236 RID: 566 RVA: 0x0000E1C5 File Offset: 0x0000C3C5
			public string subscription { get; set; }

			// Token: 0x170000C6 RID: 198
			// (get) Token: 0x06000237 RID: 567 RVA: 0x0000E1CE File Offset: 0x0000C3CE
			// (set) Token: 0x06000238 RID: 568 RVA: 0x0000E1D6 File Offset: 0x0000C3D6
			public string expiry { get; set; }

			// Token: 0x170000C7 RID: 199
			// (get) Token: 0x06000239 RID: 569 RVA: 0x0000E1DF File Offset: 0x0000C3DF
			// (set) Token: 0x0600023A RID: 570 RVA: 0x0000E1E7 File Offset: 0x0000C3E7
			public string timeleft { get; set; }
		}

		// Token: 0x02000034 RID: 52
		public class response_class
		{
			// Token: 0x170000C8 RID: 200
			// (get) Token: 0x0600023C RID: 572 RVA: 0x0000E1F0 File Offset: 0x0000C3F0
			// (set) Token: 0x0600023D RID: 573 RVA: 0x0000E1F8 File Offset: 0x0000C3F8
			public bool success { get; set; }

			// Token: 0x170000C9 RID: 201
			// (get) Token: 0x0600023E RID: 574 RVA: 0x0000E201 File Offset: 0x0000C401
			// (set) Token: 0x0600023F RID: 575 RVA: 0x0000E209 File Offset: 0x0000C409
			public string message { get; set; }
		}
	}
}
