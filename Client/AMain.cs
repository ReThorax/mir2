using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Client;
using Mir.DiscordExtension;


namespace Launcher
{
    public partial class AMain : Form
    {

        long _totalBytes, _completedBytes, _currentBytes;
        private int _fileCount, _currentCount;

        private FileInformation _currentFile;
        public bool Completed, Checked, CleanFiles, LabelSwitch, ErrorFound;
        
        public List<FileInformation> OldList;
        public Queue<FileInformation> DownloadList;

        private Stopwatch _stopwatch = Stopwatch.StartNew();

        public Thread _workThread;

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        private string oldClientName = "OldClient.exe";

        private Config ConfigForm = new Config();

        private bool Restart = false;

        public AMain()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(1, 0, 0);
            TransparencyKey = Color.FromArgb(1, 0, 0);
        }

        public static void SaveError(string ex)
        {
            try
            {
                if (Settings.RemainingErrorLogs-- > 0)
                {
                    File.AppendAllText(@".\Error.txt",
                                       string.Format("[{0}] {1}{2}", DateTime.Now, ex, Environment.NewLine));
                }
            }
            catch
            {
            }
        }

        public void Start()
        {
            try
            {
                OldList = new List<FileInformation>();
                DownloadList = new Queue<FileInformation>();

                byte[] data = Download(Settings.P_PatchFileName);

                if (data != null)
                {
                    using (MemoryStream stream = new MemoryStream(data))
                    using (BinaryReader reader = new BinaryReader(stream))
                        ParseOld(reader);
                }
                else
                {
                    MessageBox.Show("Could not get Patch Information.");
                    Completed = true;
                    return;
                }

                _fileCount = OldList.Count;
                for (int i = 0; i < OldList.Count; i++)
                    CheckFile(OldList[i]);

                Checked = true;
                _fileCount = 0;
                _currentCount = 0;


                _fileCount = DownloadList.Count;
                BeginDownload();
            }
            catch (EndOfStreamException ex)
            {
                MessageBox.Show("End of stream found. Host is likely using a pre version 1.1.0.0 patch system");
                Completed = true;
                SaveError(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
                Completed = true;
                SaveError(ex.ToString());
            }
        }

        

        private void BeginDownload()
        {
            if (DownloadList == null) return;

            if (DownloadList.Count == 0)
            {
                DownloadList = null;
                _currentFile = null;
                Completed = true;

                CleanUp();
                return;
            }

            _currentFile = DownloadList.Dequeue();

            Download(_currentFile);
        }
        private void CleanUp()
        {
            if (!CleanFiles) return;

            string[] fileNames = Directory.GetFiles(@".\", "*.*", SearchOption.AllDirectories);
            string fileName;
            for (int i = 0; i < fileNames.Length; i++)
            {
                if (fileNames[i].StartsWith(".\\Screenshots\\")) continue;

                fileName = Path.GetFileName(fileNames[i]);

                if (fileName == "Configuration.ini" || fileName == System.AppDomain.CurrentDomain.FriendlyName) continue;

                try
                {
                    if (!NeedFile(fileNames[i]))
                        File.Delete(fileNames[i]);
                }
                catch{}
            }
        }
        public bool NeedFile(string fileName)
        {
            for (int i = 0; i < OldList.Count; i++)
            {
                if (fileName.EndsWith(OldList[i].FileName))
                    return true;
            }

            return false;
        }


        public void ParseOld(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                OldList.Add(new FileInformation(reader));
        }

        public void CheckFile(FileInformation old)
        {
            FileInformation info = GetFileInformation(Settings.P_Client + old.FileName);
            _currentCount++;

            if (info == null || old.Length != info.Length || old.Creation != info.Creation)
            {
                if ((old.FileName.EndsWith(System.AppDomain.CurrentDomain.FriendlyName)))
                {
                    File.Move(Settings.P_Client + System.AppDomain.CurrentDomain.FriendlyName, Settings.P_Client + oldClientName);
                    Restart = true;
                }

                DownloadList.Enqueue(old);
                _totalBytes += old.Length;
            }
        }

        public void Download(FileInformation info)
        {
            string fileName = info.FileName.Replace(@"\", "/");

            if (fileName != "PList.gz")
                fileName += ".gz";

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += (o, e) =>
                        {
                            _currentBytes = e.BytesReceived;
                        };
                    client.DownloadDataCompleted += (o, e) =>
                        {
                            if (e.Error != null)
                            {
                                File.AppendAllText(@".\Error.txt",
                                       string.Format("[{0}] {1}{2}", DateTime.Now, info.FileName + " could not be downloaded. (" + e.Error.Message + ")", Environment.NewLine));
                                ErrorFound = true;
                            }
                            else
                            {
                                _currentCount++;
                                _completedBytes += _currentBytes;
                                _currentBytes = 0;
                                _stopwatch.Stop();

                            if (!Directory.Exists(Settings.P_Client + Path.GetDirectoryName(info.FileName)))
                                Directory.CreateDirectory(Settings.P_Client + Path.GetDirectoryName(info.FileName));

                            File.WriteAllBytes(Settings.P_Client + info.FileName, e.Result);
                            File.SetLastWriteTime(Settings.P_Client + info.FileName, info.Creation);
                            }
                            BeginDownload();
                        };

                    if (Settings.P_NeedLogin) client.Credentials = new NetworkCredential(Settings.P_Login, Settings.P_Password);


                    _stopwatch = Stopwatch.StartNew();
                    client.DownloadDataAsync(new Uri(Settings.P_Host + fileName));
                }
            }
            catch
            {
                MessageBox.Show(string.Format("Failed to download file: {0}", fileName));
            }
        }

        public byte[] Download(string fileName)
        {
            fileName = fileName.Replace(@"\", "/");

            if (fileName != "PList.gz")
                fileName += Path.GetExtension(fileName);

            try
            {
                using (WebClient client = new WebClient())
                {
                    if (Settings.P_NeedLogin)
                        client.Credentials = new NetworkCredential(Settings.P_Login, Settings.P_Password);
                    else
                        client.Credentials = new NetworkCredential("", "");

                    return client.DownloadData(Settings.P_Host + Path.ChangeExtension(fileName, ".gz"));
                }
            }
            catch
            {
                return null;
            }
        }
        public static byte[] Decompress(byte[] raw)
        {
            using (GZipStream gStream = new GZipStream(new MemoryStream(raw), CompressionMode.Decompress))
            {
                const int size = 4096; //4kb
                byte[] buffer = new byte[size];
                using (MemoryStream mStream = new MemoryStream())
                {
                    int count;
                    do
                    {
                        count = gStream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            mStream.Write(buffer, 0, count);
                        }
                    } while (count > 0);
                    return mStream.ToArray();
                }
            }
        }
        public static byte[] Compress(byte[] raw)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (GZipStream gStream = new GZipStream(mStream, CompressionMode.Compress, true))
                    gStream.Write(raw, 0, raw.Length);
                return mStream.ToArray();
            }
        }

        public FileInformation GetFileInformation(string fileName)
        {
            if (!File.Exists(fileName)) return null;

            FileInfo info = new FileInfo(fileName);
            return new FileInformation
            {
                FileName = fileName.Remove(0, Settings.P_Client.Length),
                Length = (int)info.Length,
                Creation = info.LastWriteTime
            };
        }

        private void AMain_Load(object sender, EventArgs e)
        {
            if (Settings.P_BrowserAddress != "") Main_browser.Navigate(new Uri(Settings.P_BrowserAddress));

            if (File.Exists(Settings.P_Client + oldClientName)) File.Delete(Settings.P_Client + oldClientName);

            Launch_pb.Enabled = false;
            ProgressCurrent_pb.Width = 2;
            TotalProg_pb.Width = 2;
            Version_label.Text = "Version " + Application.ProductVersion;
            
            //DiscordsApp.GetApp().UpdateStage(StatusType.GameState, GameState.Patching);
            //DiscordsApp.GetApp().UpdateActivity();

            _workThread = new Thread(Start) { IsBackground = true };
            _workThread.Start();
        }

        private void Launch_pb_Click(object sender, EventArgs e)
        {
            Launch();
        }

        private void Launch()
        {
            if (ConfigForm.Visible) ConfigForm.Visible = false;
            Program.Form = new CMain();
            Program.Form.Closed += (s, args) => this.Close();
            Program.Form.Show();
            Program.PForm.Hide();
        }

        private void Close_pb_Click(object sender, EventArgs e)
        {
            if (ConfigForm.Visible) ConfigForm.Visible = false;
            Close();
        }

        private void Movement_panel_MouseClick(object sender, MouseEventArgs e)
        {
            if (ConfigForm.Visible) ConfigForm.Visible = false;
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void Movement_panel_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void Movement_panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void Launch_pb_MouseEnter(object sender, EventArgs e)
        {
            Launch_pb.Image = Client.Properties.Resources.Start_Hover;
        }

        private void Launch_pb_MouseLeave(object sender, EventArgs e)
        {
            Launch_pb.Image = Client.Properties.Resources.Start_Normal;
        }

        private void Close_pb_MouseEnter(object sender, EventArgs e)
        {
            Close_pb.Image = Client.Properties.Resources.Close_Hover;
        }

        private void Close_pb_MouseLeave(object sender, EventArgs e)
        {
            Close_pb.Image = Client.Properties.Resources.Close_Normal;
        }

        private void Launch_pb_MouseDown(object sender, MouseEventArgs e)
        {
            Launch_pb.Image = Client.Properties.Resources.Start_Clicked;
        }

        private void Launch_pb_MouseUp(object sender, MouseEventArgs e)
        {
            Launch_pb.Image = Client.Properties.Resources.Start_Normal;
        }

        private void Close_pb_MouseDown(object sender, MouseEventArgs e)
        {
            Close_pb.Image = Client.Properties.Resources.Close_Clicked;
        }

        private void Close_pb_MouseUp(object sender, MouseEventArgs e)
        {
            Close_pb.Image = Client.Properties.Resources.Close_Normal;
        }

        private void Config_pb_MouseDown(object sender, MouseEventArgs e)
        {
            Config_pb.Image = Client.Properties.Resources.Config_Clicked;
        }

        private void Config_pb_MouseEnter(object sender, EventArgs e)
        {
            Config_pb.Image = Client.Properties.Resources.Config_Hover;
        }

        private void Config_pb_MouseLeave(object sender, EventArgs e)
        {
            Config_pb.Image = Client.Properties.Resources.Config_Normal;
        }

        private void Config_pb_MouseUp(object sender, MouseEventArgs e)
        {
            Config_pb.Image = Client.Properties.Resources.Config_Normal;
        }

        private void Config_pb_Click(object sender, EventArgs e)
        {
            if (ConfigForm.Visible) ConfigForm.Hide();
            else ConfigForm.Show(Program.PForm);
            ConfigForm.Location = new Point(Location.X - 13, Location.Y + 85);
        }

        private void Version_label_Click(object sender, EventArgs e)
        {
            if (Version_label.Text == "Version " + Application.ProductVersion) Version_label.Text = "Ooooh a Secret!";
            else Version_label.Text = "Version " + Application.ProductVersion;
        }

        private void Main_browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (Main_browser.Url.AbsolutePath != "blank") Main_browser.Visible = true; IncorrectWebsite_Image.Visible = false;
        }

        private void InterfaceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Completed)
                {
                    
                    ActionLabel.Text = "";
                    CurrentFile_label.Text = "Up to date.";
                    SpeedLabel.Text = " 0KB/s";
                    ProgressCurrent_pb.Width = 250;
                    TotalProg_pb.Width = 250;
                    InterfaceTimer.Enabled = false;
                    Launch_pb.Enabled = true;
                    if (ErrorFound) MessageBox.Show("One or more files failed to download, check Error.txt for details.", "Failed to Download.");
                    ErrorFound = false;

                    if (CleanFiles)
                    {
                        CleanFiles = false;
                        MessageBox.Show("Your files have been cleaned up.", "Clean Files");
                    }

                    if (Restart)

                    {
                        Program.Restart = true;
                        MoveOldClientToCurrent();

                        Close();
                    }

                    if (Settings.P_AutoStart)
                    {
                        Launch();
                    }
                    return;
                }

                ActionLabel.Visible = true;
                SpeedLabel.Visible = true;
                CurrentFile_label.Visible = true;

                if (LabelSwitch) ActionLabel.Text = string.Format("{0} Files Remaining", _fileCount - _currentCount);
                else ActionLabel.Text = string.Format("{0:#,##0}MB Remaining",  ((_totalBytes) - (_completedBytes + _currentBytes)) / 1024 / 1024);

                //ActionLabel.Text = string.Format("{0:#,##0}MB / {1:#,##0}MB", (_completedBytes + _currentBytes) / 1024 / 1024, _totalBytes / 1024 / 1024);

                if (_currentFile != null)
                {
                    //FileLabel.Text = string.Format("{0}, ({1:#,##0} MB) / ({2:#,##0} MB)", _currentFile.FileName, _currentBytes / 1024 / 1024, _currentFile.Compressed / 1024 / 1024);
                    CurrentFile_label.Text = string.Format("{0}", _currentFile.FileName);
                    SpeedLabel.Text = (_currentBytes / 1024F / _stopwatch.Elapsed.TotalSeconds).ToString("#,##0.##") + "KB/s";
                    ProgressCurrent_pb.Width = (int)( 2.5 * (100 * _currentBytes / _currentFile.Compressed));
                }
                TotalProg_pb.Width = (int)(2.5 * (100 * (_completedBytes + _currentBytes) / _totalBytes));
            }
            catch

            {
                
            }

        }

        private void AMain_Click(object sender, EventArgs e)
        {
            if (ConfigForm.Visible) ConfigForm.Visible = false;
        }

        private void ActionLabel_Click(object sender, EventArgs e)
        {
            LabelSwitch = !LabelSwitch;
        }

        private void Credit_label_Click(object sender, EventArgs e)
        {
            if (Credit_label.Text == "Nexus Mir - Powered by Crystal M2") Credit_label.Text = "Designed by Valhalla";
            else Credit_label.Text = "Nexus Mir - Powered by Crystal M2";
        }

        private void AMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            MoveOldClientToCurrent();
        }

        private void MoveOldClientToCurrent()
        {
            string oldClient = Settings.P_Client + oldClientName;
            string currentClient = Settings.P_Client + System.AppDomain.CurrentDomain.FriendlyName;

            if (!File.Exists(currentClient) && File.Exists(oldClient))
                File.Move(oldClient, currentClient);
        }

    }

    public class FileInformation
    {
        public string FileName; //Relative.
        public int Length, Compressed;
        public DateTime Creation;

        public FileInformation()
        {

        }
        public FileInformation(BinaryReader reader)
        {
            FileName = reader.ReadString();
            Length = reader.ReadInt32();
            Compressed = reader.ReadInt32();

            Creation = DateTime.FromBinary(reader.ReadInt64());
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write(FileName);
            writer.Write(Length);
            writer.Write(Compressed);
            writer.Write(Creation.ToBinary());
        }
    }
}
