using System;
using System.Collections.Concurrent;
using System.Windows.Forms;
using Server.MirEnvir;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using Server.MirDatabase;
using Server.MirForms.Systems;
using S = ServerPackets;
using Server.MirForms;
using System.Collections.Generic;

namespace Server
{
    public partial class SMain : Form
    {
        public static Envir Envir => Envir.Main;

        public static Envir EditEnvir => Envir.Edit;

        protected static MessageQueue MessageQueue => MessageQueue.Instance;

        public SMain()
        {
            InitializeComponent();

            AutoResize();
        }

        private void AutoResize()
        {
            int columnCount = PlayersOnlineListView.Columns.Count;

            foreach (ColumnHeader column in PlayersOnlineListView.Columns)
            {
                column.Width = PlayersOnlineListView.Width / (columnCount - 1) - 1;
            }

            indexHeader.Width = 2;
        }

        public static void Enqueue(Exception ex)
        {
            MessageQueue.Enqueue(ex);
        }

        public static void EnqueueDebugging(string msg)
        {
            MessageQueue.EnqueueDebugging(msg);
        }

        public static void EnqueueChat(string msg)
        {
            MessageQueue.EnqueueChat(msg);
        }

        public static void Enqueue(string msg)
        {
            MessageQueue.Enqueue(msg);
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void InterfaceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Text = $"Total: {Envir.LastCount}, Real: {Envir.LastRealCount}";
                PlayersLabel.Text = $"Players: {Envir.Players.Count}";
                MonsterLabel.Text = $"Monsters: {Envir.MonsterCount}";
                ConnectionsLabel.Text = $"Connections: {Envir.Connections.Count}";

                if (Settings.Multithreaded && (Envir.MobThreads != null))
                {
                    CycleDelayLabel.Text = $"CycleDelays: {Envir.LastRunTime:0000}";
                    for (int i = 0; i < Envir.MobThreads.Length; i++)
                    {
                        if (Envir.MobThreads[i] == null) break;
                        CycleDelayLabel.Text = CycleDelayLabel.Text + $"|{Envir.MobThreads[i].LastRunTime:0000}";

                    }
                }
                else
                    CycleDelayLabel.Text = $"CycleDelay: {Envir.LastRunTime}";

                while (!MessageQueue.MessageLog.IsEmpty)
                {
                    string message;

                    if (!MessageQueue.MessageLog.TryDequeue(out message)) continue;

                    LogTextBox.AppendText(message);
                }

                while (!MessageQueue.DebugLog.IsEmpty)
                {
                    string message;

                    if (!MessageQueue.DebugLog.TryDequeue(out message)) continue;

                    DebugLogTextBox.AppendText(message);
                }

                while (!MessageQueue.ChatLog.IsEmpty)
                {
                    string message;

                    if (!MessageQueue.ChatLog.TryDequeue(out message)) continue;

                    ChatLogTextBox.AppendText(message);
                }

                ProcessPlayersOnlineTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private ListViewItem CreateListView(CharacterInfo character)
        {
            ListViewItem ListItem = new ListViewItem(character.Index.ToString()) { Tag = character };

            ListItem.SubItems.Add(character.Name);
            ListItem.SubItems.Add(character.Level.ToString());
            ListItem.SubItems.Add(character.Class.ToString());
            ListItem.SubItems.Add(character.Gender.ToString());

            return ListItem;
        }

        private void ProcessPlayersOnlineTab()
        {
            if (PlayersOnlineListView.Items.Count != Envir.Players.Count)
            {
                PlayersOnlineListView.Items.Clear();

                for (int i = PlayersOnlineListView.Items.Count; i < Envir.Players.Count; i++)
                {
                    CharacterInfo character = Envir.Players[i].Info;

                    ListViewItem tempItem = CreateListView(character);

                    PlayersOnlineListView.Items.Add(tempItem);
                }
            }
        }

        private void startServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Envir.Start();
        }

        private void stopServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Envir.Stop();
            Envir.MonsterCount = 0;
        }

        private void SMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Envir.Stop();
        }

        private void closeServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void itemInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItemInfoForm form = new ItemInfoForm();

            form.ShowDialog();
        }

        private void monsterInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MonsterInfoForm form = new MonsterInfoForm();

            form.ShowDialog();
        }

        private void nPCInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NPCInfoForm form = new NPCInfoForm();

            form.ShowDialog();
        }

        private void balanceConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BalanceConfigForm form = new BalanceConfigForm();

            form.ShowDialog();
        }

        private void questInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuestInfoForm form = new QuestInfoForm();

            form.ShowDialog();
        }

        private void serverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigForm form = new ConfigForm();

            form.ShowDialog();
        }

        private void balanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BalanceConfigForm form = new BalanceConfigForm();

            form.ShowDialog();
        }

        private void accountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountInfoForm form = new AccountInfoForm();

            form.ShowDialog();
        }

        private void mapInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapInfoForm form = new MapInfoForm();

            form.ShowDialog();
        }

        private void itemInfoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ItemInfoForm form = new ItemInfoForm();

            form.ShowDialog();
        }

        private void monsterInfoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MonsterInfoForm form = new MonsterInfoForm();

            form.ShowDialog();
        }

        private void nPCInfoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            NPCInfoForm form = new NPCInfoForm();

            form.ShowDialog();
        }

        private void questInfoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            QuestInfoForm form = new QuestInfoForm();

            form.ShowDialog();
        }

        private void dragonSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DragonInfoForm form = new DragonInfoForm();

            form.ShowDialog();
        }

        private void miningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiningInfoForm form = new MiningInfoForm();

            form.ShowDialog();
        }

        private void guildsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuildInfoForm form = new GuildInfoForm();

            form.ShowDialog();
        }

        private void fishingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemInfoForm form = new SystemInfoForm(0);

            form.ShowDialog();
        }

        private void GlobalMessageButton_Click(object sender, EventArgs e)
        {
            if (GlobalMessageTextBox.Text.Length < 1) return;

            foreach (var player in Envir.Players)
            {
                player.ReceiveChat(GlobalMessageTextBox.Text, ChatType.Announcement);
            }

            EnqueueChat(GlobalMessageTextBox.Text);
            GlobalMessageTextBox.Text = string.Empty;
        }

        private void PlayersOnlineListView_DoubleClick(object sender, EventArgs e)
        {
            ListViewNF list = (ListViewNF)sender;

            if (list.SelectedItems.Count > 0)
            {
                ListViewItem item = list.SelectedItems[0];
                string index = item.SubItems[0].Text;

                PlayerInfoForm form = new PlayerInfoForm(Convert.ToUInt32(index));

                form.ShowDialog();
            }
        }

        private void PlayersOnlineListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = PlayersOnlineListView.Columns[e.ColumnIndex].Width;
        }

        private void mailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemInfoForm form = new SystemInfoForm(1);

            form.ShowDialog();
        }

        private void goodsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemInfoForm form = new SystemInfoForm(2);

            form.ShowDialog();
        }

        private void relationshipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemInfoForm form = new SystemInfoForm(4);

            form.ShowDialog();
        }

        private void refiningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemInfoForm form = new SystemInfoForm(3);

            form.ShowDialog();
        }

        private void mentorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemInfoForm form = new SystemInfoForm(5);

            form.ShowDialog();
        }

        private void magicInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MagicInfoForm form = new MagicInfoForm();
            form.ShowDialog();
        }

        private void SMain_Load(object sender, EventArgs e)
        {
            EditEnvir.LoadDB();
            Envir.Start();
            AutoResize();
        }

        private void gemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemInfoForm form = new SystemInfoForm(6);

            form.ShowDialog();
        }

        private void conquestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConquestInfoForm form = new ConquestInfoForm();

            form.ShowDialog();
        }

        private void rebootServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Envir.Reboot();
        }

        private void respawnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemInfoForm form = new SystemInfoForm(7);

            form.ShowDialog();
        }

        private void monsterTunerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SMain.Envir.Running)
            {
                MessageBox.Show("Server must be running to tune monsters", "Notice",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            MonsterTunerForm form = new MonsterTunerForm();

            form.ShowDialog();
        }

        private void reloadNPCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Envir.ReloadNPCs();
        }

        private void reloadDropsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Envir.ReloadDrops();
        }

        private void gameshopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameShop form = new GameShop();
            form.ShowDialog();
        }

        private void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int u = 0;


            foreach (var NewItem in EditEnvir.ItemInfoList)
            {
                ItemInfo OldItem = Envir.ItemInfoList.Find(x => x.Index == NewItem.Index);
                if (OldItem != null)
                {
                    OldItem.UpdateItem(NewItem);
                }
                else
                {
                    ItemInfo CloneItem = ItemInfo.CloneItem(NewItem);
                    Envir.ItemInfoList.Add(CloneItem);
                    u++;
                }
            }

            SMain.Enqueue("[Item DataBase] total items: " + Envir.ItemInfoList.Count.ToString());
            SMain.Enqueue("[Item DataBase] " + (Envir.ItemInfoList.Count - u).ToString() + " have been updated");
            SMain.Enqueue("[Item DataBase] " + u.ToString() + " have been added");

            foreach (var c in Envir.Connections)// update all info on players items
            {
                if (!c.Connected) continue;

                foreach (var i in c.SentItemInfo)
                {
                    c.Enqueue(new S.UpdateItemInfo { Info = i });
                }
            }

            foreach (var p in Envir.Players) // refresh all existing players stats
            {
                if (p.Info == null) continue;

                p.RefreshStats();
                p.Enqueue(new S.RefreshStats());

            }
        }

        private void monsterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int u = 0;


            foreach (var NewMob in EditEnvir.MonsterInfoList)
            {
                MonsterInfo OldMob = Envir.MonsterInfoList.Find(x => x.Index == NewMob.Index);
                if (OldMob != null)
                {
                    OldMob.UpdateMonster(NewMob);
                }
                else
                {
                    MonsterInfo CloneMonster = MonsterInfo.CloneMonster(NewMob);
                    Envir.MonsterInfoList.Add(CloneMonster);
                    u++;
                }
            }

            SMain.Enqueue("[Monster DataBase] total monsters: " + Envir.MonsterInfoList.Count.ToString());
            SMain.Enqueue("[Monster DataBase] " + (Envir.MonsterInfoList.Count - u).ToString() + " have been updated");
            SMain.Enqueue("[Monster DataBase] " + u.ToString() + " have been added");


        }

        private void dropsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var t in Envir.MonsterInfoList)
                t.LoadDrops();

            Envir.LoadFishingDrops();
            Envir.LoadAwakeningMaterials();
            Envir.LoadStrongBoxDrops();
            Envir.LoadBlackStoneDrops();
            SMain.Enqueue("Monster Drops Reloaded");
        }

        private void questsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int updated = 0;
            int added = 0;
            List<QuestInfo> newList = new List<QuestInfo>();
            foreach (var NewQuest in EditEnvir.QuestInfoList)
            {
                QuestInfo OldQuest = Envir.QuestInfoList.Find(x => x.Index == NewQuest.Index);
                if (OldQuest != null)
                {
                    newList.Add(OldQuest.UpdateQuestInfo(NewQuest));
                    updated++;
                }
                else if (OldQuest == null)
                {
                    QuestInfo CloneQuest = QuestInfo.CloneMonster(NewQuest);
                    CloneQuest.LoadInfo();
                    Envir.QuestInfoList.Add(CloneQuest);
                    added++;
                }
            }
            Enqueue("[Quest DataBase] total quests: " + Envir.QuestInfoList.Count.ToString());
            Enqueue(string.Format("[Quest Database] {0} quests have been updated", updated));
            Enqueue("[Quest DataBase] " + added.ToString() + " have been added");
        }

        private void nPCScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Envir.MapList.Count; i++)
            {
                for (int j = 0; j < Envir.MapList[i].NPCs.Count; j++)
                    Envir.MapList[i].NPCs[j].LoadInfo(true);
            }

            Envir.DefaultNPC.LoadInfo(true);

            Enqueue(String.Format("[{0}]: NPC Scripts reloaded" + Environment.NewLine, DateTime.Now));
        }
    }
}
