using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using BeautyUI;
using BeautyUI.Components;
using BeautyUI2.Controls;
using Bleak;

namespace Azyre
{
	// Token: 0x02000021 RID: 33
	public partial class MainForm : Form
	{
		// Token: 0x060001B3 RID: 435
		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		// Token: 0x060001B4 RID: 436
		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		// Token: 0x060001B5 RID: 437 RVA: 0x0000B06E File Offset: 0x0000926E
		public MainForm()
		{
			this.InitializeComponent();
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000B07C File Offset: 0x0000927C
		private void MainForm_Load(object sender, EventArgs e)
		{
			MainForm.<MainForm_Load>d__4 <MainForm_Load>d__;
			<MainForm_Load>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<MainForm_Load>d__.<>4__this = this;
			<MainForm_Load>d__.<>1__state = -1;
			<MainForm_Load>d__.<>t__builder.Start<MainForm.<MainForm_Load>d__4>(ref <MainForm_Load>d__);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000B0B4 File Offset: 0x000092B4
		private Task RunLoader()
		{
			MainForm.<RunLoader>d__5 <RunLoader>d__;
			<RunLoader>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RunLoader>d__.<>4__this = this;
			<RunLoader>d__.<>1__state = -1;
			<RunLoader>d__.<>t__builder.Start<MainForm.<RunLoader>d__5>(ref <RunLoader>d__);
			return <RunLoader>d__.<>t__builder.Task;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000B0F8 File Offset: 0x000092F8
		private Task SwitchToMainUI()
		{
			MainForm.<SwitchToMainUI>d__6 <SwitchToMainUI>d__;
			<SwitchToMainUI>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SwitchToMainUI>d__.<>4__this = this;
			<SwitchToMainUI>d__.<>1__state = -1;
			<SwitchToMainUI>d__.<>t__builder.Start<MainForm.<SwitchToMainUI>d__6>(ref <SwitchToMainUI>d__);
			return <SwitchToMainUI>d__.<>t__builder.Task;
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000B13C File Offset: 0x0000933C
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			MainForm.<MainForm_FormClosing>d__7 <MainForm_FormClosing>d__;
			<MainForm_FormClosing>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<MainForm_FormClosing>d__.<>1__state = -1;
			<MainForm_FormClosing>d__.<>t__builder.Start<MainForm.<MainForm_FormClosing>d__7>(ref <MainForm_FormClosing>d__);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000B16C File Offset: 0x0000936C
		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			MainForm.<MainForm_FormClosed>d__8 <MainForm_FormClosed>d__;
			<MainForm_FormClosed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<MainForm_FormClosed>d__.<>1__state = -1;
			<MainForm_FormClosed>d__.<>t__builder.Start<MainForm.<MainForm_FormClosed>d__8>(ref <MainForm_FormClosed>d__);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000B19B File Offset: 0x0000939B
		private void CombatButton_CheckedChanged(object sender, EventArgs e)
		{
		}

		// Token: 0x0400010E RID: 270
		public static Injector injector;
	}
}
