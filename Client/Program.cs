using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using Launcher;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Mir.DiscordExtension;
using Mir.DiscordExtension.SDK;

namespace Client
{
    internal static class Program
    {
        public static CMain Form;
        public static AMain PForm;
        public static bool Restart;

        public static DiscordsApp discord => DiscordsApp.GetApp();

        [STAThread]
        private static void Main(string[] args)
        {
            //var discord = DiscordsApp.GetApp();
            discord.ClientId = 634555224123506698;
            discord.StartFailure += DiscordOnStartFailure;
            discord.Started += DiscordOnStarted;
            discord.HasException += DiscordOnHasException;
            discord.Stopped += DiscordOnStopped;
            discord.StartApp();
            //discord.UpdateActivity();
            discord.StartLoop();

            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg.ToLower() == "-tc") Settings.UseTestConfig = true;
                }
            }

#if DEBUG
                Settings.UseTestConfig = true;
#endif

            try
            {
                if (UpdatePatcher()) return;

                if (RuntimePolicyHelper.LegacyV2RuntimeEnabledSuccessfully == true) { }

                Packet.IsServer = false;
                Settings.Load();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (Settings.P_Patcher)
                {
                    discord.UpdateStage(StatusType.GameState, GameState.Patching);
                    discord.UpdateActivity();
                    Application.Run(PForm = new Launcher.AMain());
                }
                else
                {
                    discord.UpdateStage(StatusType.GameState, GameState.Launching);
                    discord.UpdateActivity();
                    Application.Run(Form = new CMain());
                }

                Settings.Save();
                CMain.InputKeys.Save();

                if (Restart)
                {
                    Application.Restart();
                }
            }
            catch (Exception ex)
            {
                CMain.SaveError(ex.ToString());
            }
        }

        private static void DiscordOnActivityCallBack(object sender, EventArgs e)
        {
            Console.WriteLine($"Call back Received ({(Result)sender})");
        }

        private static void DiscordOnStopped(object sender, EventArgs e)
        {
            Console.WriteLine("Discord Stopped");
        }

        private static void DiscordOnHasException(object sender, EventArgs e)
        {
            Console.WriteLine(((Exception)sender));
        }

        private static void DiscordOnStarted(object sender, EventArgs e)
        {
            Console.WriteLine("Discord Started");
        }

        private static void DiscordOnStartFailure(object sender, EventArgs e)
        {
            Console.WriteLine($"Discord Start failed with {(byte)sender}");
        }

        private static bool UpdatePatcher()
        {
            try
            {
                const string fromName = @".\AutoPatcher.gz", toName = @".\AutoPatcher.exe";
                if (!File.Exists(fromName)) return false;

                Process[] processes = Process.GetProcessesByName("AutoPatcher");

                if (processes.Length > 0)
                {
                    string patcherPath = Application.StartupPath + @"\AutoPatcher.exe";

                    for (int i = 0; i < processes.Length; i++)
                        if (processes[i].MainModule.FileName == patcherPath)
                            processes[i].Kill();

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    bool wait = true;
                    processes = Process.GetProcessesByName("AutoPatcher");

                    while (wait)
                    {
                        wait = false;
                        for (int i = 0; i < processes.Length; i++)
                            if (processes[i].MainModule.FileName == patcherPath)
                            {
                                wait = true;
                            }

                        if (stopwatch.ElapsedMilliseconds <= 3000) continue;
                        MessageBox.Show("Failed to close AutoPatcher during update.");
                        return true;
                    }
                }

                if (File.Exists(toName)) File.Delete(toName);
                File.Move(fromName, toName);
                Process.Start(toName, "Auto");

                return true;
            }
            catch (Exception ex)
            {
                CMain.SaveError(ex.ToString());

                throw;
            }





        }

        public static class RuntimePolicyHelper
        {
            public static bool LegacyV2RuntimeEnabledSuccessfully { get; private set; }

            static RuntimePolicyHelper()
            {
                ICLRRuntimeInfo clrRuntimeInfo =
                    (ICLRRuntimeInfo)RuntimeEnvironment.GetRuntimeInterfaceAsObject(
                        Guid.Empty,
                        typeof(ICLRRuntimeInfo).GUID);
                try
                {
                    clrRuntimeInfo.BindAsLegacyV2Runtime();
                    LegacyV2RuntimeEnabledSuccessfully = true;
                }
                catch (COMException)
                {
                    // This occurs with an HRESULT meaning 
                    // "A different runtime was already bound to the legacy CLR version 2 activation policy."
                    LegacyV2RuntimeEnabledSuccessfully = false;
                }
            }

            [ComImport]
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            [Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891")]
            private interface ICLRRuntimeInfo
            {
                void xGetVersionString();
                void xGetRuntimeDirectory();
                void xIsLoaded();
                void xIsLoadable();
                void xLoadErrorString();
                void xLoadLibrary();
                void xGetProcAddress();
                void xGetInterface();
                void xSetDefaultStartupFlags();
                void xGetDefaultStartupFlags();

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void BindAsLegacyV2Runtime();
            }
        }

    }
}
