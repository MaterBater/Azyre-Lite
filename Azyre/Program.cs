using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Azyre.Prot;
using Azyre.Utils;
using Microsoft.Win32;

namespace Azyre
{
	// Token: 0x02000029 RID: 41
	internal static class Program
	{
		// Token: 0x060001D2 RID: 466 RVA: 0x0000C6A0 File Offset: 0x0000A8A0
		public static string GetHwid()
		{
			string input = Program.<GetHwid>g__GetStableWmiValue|0_2("Win32_ComputerSystemProduct", "UUID");
			string input2 = Program.<GetHwid>g__GetStableWmiValue|0_2("Win32_Processor", "ProcessorId");
			string input3 = Program.<GetHwid>g__GetWmiValue|0_0("Win32_BaseBoard", "SerialNumber", "");
			string input4 = Program.<GetHwid>g__GetStableDiskId|0_3();
			string input5 = Program.<GetHwid>g__GetPersistentTpmId|0_4();
			string input6 = Program.<GetHwid>g__GetWmiValue|0_0("Win32_SystemEnclosure", "SerialNumber", "");
			string input7 = Program.<GetHwid>g__GetWmiValue|0_0("Win32_BIOS", "SerialNumber", "");
			string s = string.Concat(new string[]
			{
				Program.<GetHwid>g__Sanitize|0_5(input),
				"|",
				Program.<GetHwid>g__Sanitize|0_5(input2),
				"|",
				Program.<GetHwid>g__Sanitize|0_5(input4),
				"|",
				Program.<GetHwid>g__Sanitize|0_5(input5),
				"|",
				Program.<GetHwid>g__Sanitize|0_5(input3),
				"|",
				Program.<GetHwid>g__Sanitize|0_5(input7),
				"|",
				Program.<GetHwid>g__Sanitize|0_5(input6)
			});
			string result;
			using (SHA512 sha = SHA512.Create())
			{
				result = Program.<GetHwid>g__ToBase62|0_6(sha.ComputeHash(Encoding.UTF8.GetBytes(s))).Substring(0, 32);
			}
			return result;
		}

		// Token: 0x060001D3 RID: 467
		[DllImport("kernel32.dll")]
		public static extern void ExitProcess(uint uExitCode);

		// Token: 0x060001D4 RID: 468
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();

		// Token: 0x060001D5 RID: 469 RVA: 0x0000C7E8 File Offset: 0x0000A9E8
		public static string GetFileHash()
		{
			string result;
			try
			{
				string executablePath = Application.ExecutablePath;
				using (SHA256 sha = SHA256.Create())
				{
					using (FileStream fileStream = File.OpenRead(executablePath))
					{
						result = BitConverter.ToString(sha.ComputeHash(fileStream)).Replace("-", "").ToLower();
					}
				}
			}
			catch
			{
				result = "UNKNOWN";
			}
			return result;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000C874 File Offset: 0x0000AA74
		public static string GetEmbeddedHash()
		{
			return BitConverter.ToString(Program.HashPlaceholder).Replace("-", "").ToLower();
		}

		// Token: 0x060001D7 RID: 471
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();

		// Token: 0x060001D8 RID: 472
		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		// Token: 0x060001D9 RID: 473 RVA: 0x0000C894 File Offset: 0x0000AA94
		[STAThread]
		private static void Main()
		{
			Verifications.InitializeAndMonitor();
			Control.CheckForIllegalCrossThreadCalls = false;
			Console.CursorVisible = false;
			Console.Title = "";
			Program.acess = false;
			Program.numkey = 10;
			Program.strkey = string.Empty;
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Program.Auth = new api("Azyre", "rc3HfSIbz3", "1.0.0", null);
			Program.Auth.init();
			string text = Program.Auth.var("version");
			if (text != null && text != "1.0.0")
			{
				Console.Write(" >> ");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Outdated version!");
				Thread.Sleep(2000);
				Program.ExitProcess(0U);
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			string text2 = Program.Auth.var("globalmsg");
			if (text2 != null && text2.ToLower() != "null")
			{
				MessageBox.Show(text2, "New Message!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				Program.ExitProcess(0U);
			}
			Program.Auth.login(Program.GetHwid().ToString(), (Program.numkey - 9).ToString() + "23".ToString());
			if (Program.Auth.response.success && Program.Auth.response.message == "098393e5c78d0f7f76f2e41b68740cc47b7132e2c305947c47e8f847472e94df0")
			{
				Program.acess = true;
				Program.numkey = 15331;
				if (Program.Auth.var("keypass51") != "euamocomerpipocapipocapipocapipocadoce")
				{
					Program.ExitProcess(0U);
				}
				string embeddedHash = Program.GetEmbeddedHash();
				Program.Auth.log(string.Concat(new string[]
				{
					"[3.0 Login]\nUser: ",
					Environment.UserName,
					"\nBuildHash: ",
					embeddedHash,
					"\nHWID: ",
					Program.GetHwid(),
					"\nName: ",
					Path.GetFileName(Application.ExecutablePath)
				}));
				Program.ShowWindow(Program.GetConsoleWindow(), 0);
				Application.Run(new MainForm());
				return;
			}
			if (!(Program.Auth.response.message == "70f703e5ce8f8472239c34472ef76fe94df0988740cc405947c41b78de27b7136"))
			{
				Console.Write(" >> ");
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(Program.Auth.response.message);
				Program.Auth.log(string.Concat(new string[]
				{
					"[Failed Login]\n",
					Environment.UserName,
					" failed to login!\nReason: ",
					Program.Auth.response.message,
					"\nHWID: ",
					Program.GetHwid(),
					"\nName: ",
					Path.GetFileName(Application.ExecutablePath),
					"\nHash: ",
					Program.GetFileHash()
				}));
				Thread.Sleep(5000);
				Program.ExitProcess(0U);
				return;
			}
			Console.Write(" >> ");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Hwid not registered");
			Program.Auth.log(string.Concat(new string[]
			{
				"[Invalid Login]\n",
				Environment.UserName,
				" tried to open the program!\nHWID: ",
				Program.GetHwid(),
				"\nName: ",
				Path.GetFileName(Application.ExecutablePath),
				"\nHash: ",
				Program.GetFileHash()
			}));
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.Write(" >> ");
			Console.ForegroundColor = ConsoleColor.White;
			Console.CursorVisible = true;
			Console.Write("Enter your license: ");
			Console.ForegroundColor = ConsoleColor.DarkGray;
			string text3 = Console.ReadLine();
			Console.CursorVisible = false;
			Program.Auth.register(Program.GetHwid(), (Program.numkey - 9).ToString() + "23", text3, "");
			if (Program.Auth.response.success && Program.Auth.response.message == "098393e5c78d0f7f76f2e41b68740cc47b7132e2c305947c47e8f847472e94df0")
			{
				Program.acess = true;
				Program.Auth.log(string.Concat(new string[]
				{
					"[Register]\n",
					Environment.UserName,
					" has just registered!\nUsed License: ",
					text3,
					"\nHWID: ",
					Program.GetHwid(),
					"\nName: ",
					Path.GetFileName(Application.ExecutablePath),
					"\nHash: ",
					Program.GetFileHash()
				}));
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Thread.Sleep(2000);
				if (Program.Auth.var("keypass51") != "euamocomerpipocapipocapipocapipocadoce")
				{
					Environment.Exit(0);
				}
				Program.numkey = 15331;
				Program.ShowWindow(Program.GetConsoleWindow(), 0);
				Application.Run(new MainForm());
				return;
			}
			Program.Auth.log(string.Concat(new string[]
			{
				"[Register Fail]\n",
				Environment.UserName,
				" just TRIED to register!\nInsert License: ",
				text3,
				"\nMessage: ",
				Program.Auth.response.message,
				"\nHWID: ",
				Program.GetHwid(),
				"\nName: ",
				Path.GetFileName(Application.ExecutablePath),
				"\nHash: ",
				Program.GetFileHash()
			}));
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.Write(" >> ");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(Program.Auth.response.message);
			Thread.Sleep(2000);
			Program.ExitProcess(0U);
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000CE98 File Offset: 0x0000B098
		[CompilerGenerated]
		internal static string <GetHwid>g__GetWmiValue|0_0(string className, string property, string where = "")
		{
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(string.IsNullOrEmpty(where) ? ("SELECT " + property + " FROM " + className) : string.Concat(new string[]
				{
					"SELECT ",
					property,
					" FROM ",
					className,
					" WHERE ",
					where
				})))
				{
					using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							object obj = ((ManagementObject)enumerator.Current)[property];
							return ((obj != null) ? obj.ToString().Trim() : null) ?? "NULL";
						}
					}
				}
			}
			catch
			{
			}
			return "NULL";
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000CF88 File Offset: 0x0000B188
		[CompilerGenerated]
		internal static bool <GetHwid>g__IsGenericValue|0_1(string value)
		{
			value = value.ToUpperInvariant();
			return new string[]
			{
				"OEM",
				"DEFAULT",
				"NONE",
				"XXXX",
				"0000",
				"1111",
				"1234",
				"SERIAL",
				"TO BE FILLED",
				"UNKNOWN"
			}.Any((string p) => value.Contains(p));
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000D01C File Offset: 0x0000B21C
		[CompilerGenerated]
		internal static string <GetHwid>g__GetStableWmiValue|0_2(string className, string property)
		{
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT " + property + " FROM " + className))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						object obj = ((ManagementObject)managementBaseObject)[property];
						string text = ((obj != null) ? obj.ToString().Trim() : null) ?? "";
						if (!string.IsNullOrEmpty(text) && !Program.<GetHwid>g__IsGenericValue|0_1(text))
						{
							return text;
						}
					}
				}
			}
			catch
			{
			}
			return "NULL";
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000D0E4 File Offset: 0x0000B2E4
		[CompilerGenerated]
		internal static string <GetHwid>g__GetStableDiskId|0_3()
		{
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive WHERE Index = 0"))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						object obj = ((ManagementObject)managementBaseObject)["SerialNumber"];
						string text = ((obj != null) ? obj.ToString().Trim() : null) ?? "";
						if (!string.IsNullOrEmpty(text) && !Program.<GetHwid>g__IsGenericValue|0_1(text))
						{
							return text;
						}
					}
				}
			}
			catch
			{
			}
			return "NODISK";
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000D1A4 File Offset: 0x0000B3A4
		[CompilerGenerated]
		internal static string <GetHwid>g__GetPersistentTpmId|0_4()
		{
			try
			{
				string text = Program.<GetHwid>g__GetStableWmiValue|0_2("ROOT\\CIMV2\\Security\\MicrosoftTpm", "InstanceGuid");
				if (text != "NULL")
				{
					return text;
				}
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\TPM"))
				{
					string text2;
					if (registryKey == null)
					{
						text2 = null;
					}
					else
					{
						object value = registryKey.GetValue("DeviceID");
						text2 = ((value != null) ? value.ToString() : null);
					}
					string text3 = text2 ?? "";
					if (!string.IsNullOrEmpty(text3))
					{
						return text3;
					}
				}
			}
			catch
			{
			}
			return "NOTPM";
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000D24C File Offset: 0x0000B44C
		[CompilerGenerated]
		internal static string <GetHwid>g__Sanitize|0_5(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return "NULL";
			}
			input = input.ToUpperInvariant().Trim();
			if (!new string[]
			{
				"OEM",
				"TO BE FILLED",
				"DEFAULT",
				"NONE",
				"XXXX",
				"00000000",
				"11111111",
				"12345678",
				"SERIAL"
			}.Any((string p) => input.Contains(p)))
			{
				return input;
			}
			return "INVALID";
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000D300 File Offset: 0x0000B500
		[CompilerGenerated]
		internal static string <GetHwid>g__ToBase62|0_6(byte[] data)
		{
			BigInteger bigInteger = new BigInteger(data.Concat(new byte[1]).ToArray<byte>());
			if (bigInteger < 0L)
			{
				bigInteger = -bigInteger;
			}
			StringBuilder stringBuilder = new StringBuilder(32);
			while (bigInteger > 0L && stringBuilder.Length < 32)
			{
				stringBuilder.Insert(0, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"[(int)(bigInteger % 62)]);
				bigInteger /= 62;
			}
			return stringBuilder.ToString().PadLeft(32, '0');
		}

		// Token: 0x04000139 RID: 313
		public static api Auth;

		// Token: 0x0400013A RID: 314
		public static int numkey = 3;

		// Token: 0x0400013B RID: 315
		public static bool acess = false;

		// Token: 0x0400013C RID: 316
		public static string strkey = "puy14gvn2uvikw";

		// Token: 0x0400013D RID: 317
		public static uint process;

		// Token: 0x0400013E RID: 318
		public static string WE1 = "1481792421581816001/1Y2T8U87Ez0cdICQWgC0kUV6AcbuCXgPvTjey8p_HAfH127HwnZfx1DW-Ec8c8MULGp3";

		// Token: 0x0400013F RID: 319
		public static string WE2 = "1482832964483289249/xj8t4kcD6XvggRpIDkWSyrb8iIv3Ncnn8EAa7Q-enu5c8RAZv7fj6bz-bcRV_3SKdJ-Y";

		// Token: 0x04000140 RID: 320
		public static string WE3 = "1482832997773213892/DyeXQL2MgwyVfwRk_BTg2PhYAtFY9QKuDU1iP4WMJLHEt0QSnASvm55-p0ECt9bwifaI";

		// Token: 0x04000141 RID: 321
		public static string WE4 = "1482833038160298107/trUovOqTADwMuaBoHF3Lp8vM-wIWdNiXIsUs4Ne5tF0FLF9a7-cI2PImsC5P7JJynB-N";

		// Token: 0x04000142 RID: 322
		public static string WE5 = "1482833068375933082/qr5X0Sfxfaz9X_laeHLZfPL-cHjASLzXxJ7YKPW-198IddwzCZru-lGwjMfwMDBKsxTs";

		// Token: 0x04000143 RID: 323
		public static string WE6 = "1482833103474135172/UA9EAfODNRVkJR89AqoheSTDyLRZaw2o_d63O2i2_82OEhelv2kiNoswPYuLzqLpFgpb";

		// Token: 0x04000144 RID: 324
		public static string WE7 = "1482833142598467888/qH6psglRAvSmVYaXVnboZoSyAkgbFnEakLzIlYXomE-u1Qul-8RK6VYBd75m528JWOUO";

		// Token: 0x04000145 RID: 325
		public static string WE8 = "1482833176156967042/xHK_caxgW0IhfIdwkrN-N4g4MDVnhhxRGIAB7FlX8auPFUQGRENIFUEg6przhDrofv7z";

		// Token: 0x04000146 RID: 326
		public static string WE9 = "1482833205806632982/GXgq_vCiDPzNeXGSCWw2OWsQHcipQV7F6SczqGI3kYlnh8rHsWpqIhkW8jbztcO8_JsV";

		// Token: 0x04000147 RID: 327
		public static string WE10 = "1482833238048243732/E6R31evWBf12VPaoncePASHeumlH822fmgijr40syZl2GwJo-mxC-YIUyn0tlveViKEM";

		// Token: 0x04000148 RID: 328
		public static byte[] HashPlaceholder = new byte[]
		{
			72,
			65,
			83,
			72,
			95,
			65,
			85,
			81,
			73,
			49,
			50,
			51,
			52,
			53,
			54,
			0
		};

		// Token: 0x04000149 RID: 329
		private const int SW_HIDE = 0;
	}
}
