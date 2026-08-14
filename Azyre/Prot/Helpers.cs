using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Azyre.Prot
{
	// Token: 0x02000047 RID: 71
	public static class Helpers
	{
		// Token: 0x06000279 RID: 633
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

		// Token: 0x0600027A RID: 634
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetCurrentProcess();

		// Token: 0x0600027B RID: 635 RVA: 0x00010B9C File Offset: 0x0000ED9C
		public static string GetHwid()
		{
			string input = Helpers.<GetHwid>g__GetStableWmiValue|2_2("Win32_ComputerSystemProduct", "UUID");
			string input2 = Helpers.<GetHwid>g__GetStableWmiValue|2_2("Win32_Processor", "ProcessorId");
			string input3 = Helpers.<GetHwid>g__GetWmiValue|2_0("Win32_BaseBoard", "SerialNumber", "");
			string input4 = Helpers.<GetHwid>g__GetStableDiskId|2_3();
			string input5 = Helpers.<GetHwid>g__GetPersistentTpmId|2_4();
			string input6 = Helpers.<GetHwid>g__GetWmiValue|2_0("Win32_SystemEnclosure", "SerialNumber", "");
			string input7 = Helpers.<GetHwid>g__GetWmiValue|2_0("Win32_BIOS", "SerialNumber", "");
			string s = string.Concat(new string[]
			{
				Helpers.<GetHwid>g__Sanitize|2_5(input),
				"|",
				Helpers.<GetHwid>g__Sanitize|2_5(input2),
				"|",
				Helpers.<GetHwid>g__Sanitize|2_5(input4),
				"|",
				Helpers.<GetHwid>g__Sanitize|2_5(input5),
				"|",
				Helpers.<GetHwid>g__Sanitize|2_5(input3),
				"|",
				Helpers.<GetHwid>g__Sanitize|2_5(input7),
				"|",
				Helpers.<GetHwid>g__Sanitize|2_5(input6)
			});
			string result;
			using (SHA512 sha = SHA512.Create())
			{
				result = Helpers.<GetHwid>g__ToBase62|2_6(sha.ComputeHash(Encoding.UTF8.GetBytes(s))).Substring(0, 32);
			}
			return result;
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00010CE4 File Offset: 0x0000EEE4
		public static void ShowMessage(string message, bool CMD, bool SelfDelete)
		{
			if (CMD)
			{
				Process.Start(new ProcessStartInfo("cmd.exe", "/c start cmd /C \"color 03 && echo " + message + " && echo. && pause && pause && pause && pause")
				{
					CreateNoWindow = true,
					UseShellExecute = false
				});
				if (SelfDelete)
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = "cmd.exe",
						Arguments = "/C timeout 1 & del \"" + Process.GetCurrentProcess().MainModule.FileName + "\"",
						CreateNoWindow = true,
						UseShellExecute = false
					});
				}
				Helpers.TerminateProcess(Helpers.GetCurrentProcess(), 0U);
				Application.Exit();
				Environment.Exit(0);
				return;
			}
			MessageBox.Show(message, "Elite Private Softwares", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00010D98 File Offset: 0x0000EF98
		[CompilerGenerated]
		internal static string <GetHwid>g__GetWmiValue|2_0(string className, string property, string where = "")
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

		// Token: 0x0600027E RID: 638 RVA: 0x00010E88 File Offset: 0x0000F088
		[CompilerGenerated]
		internal static bool <GetHwid>g__IsGenericValue|2_1(string value)
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

		// Token: 0x0600027F RID: 639 RVA: 0x00010F1C File Offset: 0x0000F11C
		[CompilerGenerated]
		internal static string <GetHwid>g__GetStableWmiValue|2_2(string className, string property)
		{
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT " + property + " FROM " + className))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						object obj = ((ManagementObject)managementBaseObject)[property];
						string text = ((obj != null) ? obj.ToString().Trim() : null) ?? "";
						if (!string.IsNullOrEmpty(text) && !Helpers.<GetHwid>g__IsGenericValue|2_1(text))
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

		// Token: 0x06000280 RID: 640 RVA: 0x00010FE4 File Offset: 0x0000F1E4
		[CompilerGenerated]
		internal static string <GetHwid>g__GetStableDiskId|2_3()
		{
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive WHERE Index = 0"))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						object obj = ((ManagementObject)managementBaseObject)["SerialNumber"];
						string text = ((obj != null) ? obj.ToString().Trim() : null) ?? "";
						if (!string.IsNullOrEmpty(text) && !Helpers.<GetHwid>g__IsGenericValue|2_1(text))
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

		// Token: 0x06000281 RID: 641 RVA: 0x000110A4 File Offset: 0x0000F2A4
		[CompilerGenerated]
		internal static string <GetHwid>g__GetPersistentTpmId|2_4()
		{
			try
			{
				string text = Helpers.<GetHwid>g__GetStableWmiValue|2_2("ROOT\\CIMV2\\Security\\MicrosoftTpm", "InstanceGuid");
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

		// Token: 0x06000282 RID: 642 RVA: 0x0001114C File Offset: 0x0000F34C
		[CompilerGenerated]
		internal static string <GetHwid>g__Sanitize|2_5(string input)
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

		// Token: 0x06000283 RID: 643 RVA: 0x00011200 File Offset: 0x0000F400
		[CompilerGenerated]
		internal static string <GetHwid>g__ToBase62|2_6(byte[] data)
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
	}
}
