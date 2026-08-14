using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Azyre.Prot
{
	// Token: 0x02000041 RID: 65
	public static class Verifications
	{
		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000266 RID: 614 RVA: 0x0000F2B8 File Offset: 0x0000D4B8
		public static bool EstáEnviandoAlerta
		{
			get
			{
				return Verifications._enviandoAlerta;
			}
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000F2C0 File Offset: 0x0000D4C0
		public static void InitializeAndMonitor()
		{
			Verifications.<>c__DisplayClass5_0 CS$<>8__locals1 = new Verifications.<>c__DisplayClass5_0();
			if (Interlocked.Exchange(ref Verifications._ran, 1) == 1)
			{
				return;
			}
			CS$<>8__locals1.cachedIP = "unk";
			Task.Run(delegate()
			{
				try
				{
					using (WebClient webClient = new WebClient
					{
						Proxy = null
					})
					{
						CS$<>8__locals1.cachedIP = webClient.DownloadString("https://api.ipify.org/?format=text");
					}
				}
				catch
				{
				}
			});
			new Thread(delegate()
			{
				Verifications.<>c__DisplayClass5_0.<<InitializeAndMonitor>b__1>d <<InitializeAndMonitor>b__1>d;
				<<InitializeAndMonitor>b__1>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<InitializeAndMonitor>b__1>d.<>4__this = CS$<>8__locals1;
				<<InitializeAndMonitor>b__1>d.<>1__state = -1;
				<<InitializeAndMonitor>b__1>d.<>t__builder.Start<Verifications.<>c__DisplayClass5_0.<<InitializeAndMonitor>b__1>d>(ref <<InitializeAndMonitor>b__1>d);
			})
			{
				IsBackground = true,
				Priority = ThreadPriority.Lowest
			}.Start();
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000F324 File Offset: 0x0000D524
		private static Task<bool> SendAllWebhooksRaceAsync(string[] webhooks, string json, byte[] image, CancellationTokenSource cts)
		{
			Verifications.<SendAllWebhooksRaceAsync>d__6 <SendAllWebhooksRaceAsync>d__;
			<SendAllWebhooksRaceAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SendAllWebhooksRaceAsync>d__.webhooks = webhooks;
			<SendAllWebhooksRaceAsync>d__.json = json;
			<SendAllWebhooksRaceAsync>d__.image = image;
			<SendAllWebhooksRaceAsync>d__.cts = cts;
			<SendAllWebhooksRaceAsync>d__.<>1__state = -1;
			<SendAllWebhooksRaceAsync>d__.<>t__builder.Start<Verifications.<SendAllWebhooksRaceAsync>d__6>(ref <SendAllWebhooksRaceAsync>d__);
			return <SendAllWebhooksRaceAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000F380 File Offset: 0x0000D580
		private static Task<bool> SendWebhookWithRetryAsync(string webhookUrl, string json, byte[] image, CancellationToken ct = default(CancellationToken))
		{
			Verifications.<SendWebhookWithRetryAsync>d__7 <SendWebhookWithRetryAsync>d__;
			<SendWebhookWithRetryAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SendWebhookWithRetryAsync>d__.webhookUrl = webhookUrl;
			<SendWebhookWithRetryAsync>d__.json = json;
			<SendWebhookWithRetryAsync>d__.image = image;
			<SendWebhookWithRetryAsync>d__.ct = ct;
			<SendWebhookWithRetryAsync>d__.<>1__state = -1;
			<SendWebhookWithRetryAsync>d__.<>t__builder.Start<Verifications.<SendWebhookWithRetryAsync>d__7>(ref <SendWebhookWithRetryAsync>d__);
			return <SendWebhookWithRetryAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000F3DC File Offset: 0x0000D5DC
		private static bool TryKeyAuthWebhook(string json, byte[] image)
		{
			bool result;
			try
			{
				string text = json;
				if (image != null)
				{
					string str = Convert.ToBase64String(image);
					text = json.Substring(0, json.Length - 4);
					text = text + ",\"image_base64\":\"" + str + "\"}}]}";
				}
				if (Program.Auth != null && Program.Auth.IsInitialized())
				{
					Program.Auth.webhook("LogAzyre", "", text, "application/json");
					result = (Program.Auth.response != null && Program.Auth.response.success);
				}
				else
				{
					result = false;
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600026B RID: 619
		[DllImport("kernel32.dll")]
		private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

		// Token: 0x0400018F RID: 399
		private static int _ran;

		// Token: 0x04000190 RID: 400
		private static HttpClient _httpClient;

		// Token: 0x04000191 RID: 401
		private static bool _enviandoAlerta;
	}
}
