using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Azyre.Utils
{
	// Token: 0x02000038 RID: 56
	public static class Binds
	{
		// Token: 0x0600024B RID: 587
		[DllImport("user32.dll")]
		public static extern short GetAsyncKeyState(int vKey);

		// Token: 0x0600024C RID: 588 RVA: 0x0000E35C File Offset: 0x0000C55C
		public static void ListenForKeyPress()
		{
			Binds.<ListenForKeyPress>d__4 <ListenForKeyPress>d__;
			<ListenForKeyPress>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ListenForKeyPress>d__.<>1__state = -1;
			<ListenForKeyPress>d__.<>t__builder.Start<Binds.<ListenForKeyPress>d__4>(ref <ListenForKeyPress>d__);
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000E38B File Offset: 0x0000C58B
		public static bool isKeyPressed(Keys key)
		{
			return ((int)Binds.GetAsyncKeyState((int)key) & 32768) != 0;
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000E39C File Offset: 0x0000C59C
		public static void setupBindListener()
		{
			List<Keys> list = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList<Keys>();
			list.Remove(Keys.LButton);
			list.Remove(Keys.RButton);
			foreach (Keys item in list)
			{
				Binds.keyList.Add(item);
			}
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000E418 File Offset: 0x0000C618
		public static Task<Keys> getBind()
		{
			Binds.<getBind>d__8 <getBind>d__;
			<getBind>d__.<>t__builder = AsyncTaskMethodBuilder<Keys>.Create();
			<getBind>d__.<>1__state = -1;
			<getBind>d__.<>t__builder.Start<Binds.<getBind>d__8>(ref <getBind>d__);
			return <getBind>d__.<>t__builder.Task;
		}

		// Token: 0x04000179 RID: 377
		public static HashSet<Keys> keysToCheck = new HashSet<Keys>();

		// Token: 0x0400017A RID: 378
		public static Dictionary<Keys, bool> keyStates = new Dictionary<Keys, bool>();

		// Token: 0x0400017B RID: 379
		public static Dictionary<Keys, List<Action>> keybinds = new Dictionary<Keys, List<Action>>();

		// Token: 0x0400017C RID: 380
		public static List<Keys> keyList = new List<Keys>();
	}
}
