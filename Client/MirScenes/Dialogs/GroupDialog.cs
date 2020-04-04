using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C = ClientPackets;
using Mir.DiscordExtension;

namespace Client.MirScenes.Dialogs
{
    public sealed class GroupDialog : MirImageControl
    {
        public static bool AllowGroup;
        public static List<string> GroupList = new List<string>();

        public MirImageControl TitleLabel;
        public MirButton SwitchButton, CloseButton, AddButton, DelButton;
        public MirLabel[] GroupMembers;
        public MirLabel AddLabel, DelLabel;

        public GroupDialog()
        {
            Index = 244;
            Library = Libraries.GameScene;
            Movable = true;
            Sort = true;
            Location = Center;

            GroupMembers = new MirLabel[Globals.MaxGroup];

            GroupMembers[0] = new MirLabel
            {
                AutoSize = true,
                Location = new Point(35, 66),
                Parent = this,
                NotControl = true,
            };

            for (int i = 1; i < GroupMembers.Length; i++)
            {
                GroupMembers[i] = new MirLabel
                {
                    AutoSize = true,
                    Location = new Point(((i + 1) % 2) * 100 + 135, 66 + ((i - 1) / 2) * 20),
                    Parent = this,
                    NotControl = true,
                };
            }

            CloseButton = new MirButton
            {
                HoverIndex = 186,
                Index = 185,
                Location = new Point(203, 25),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 187,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            SwitchButton = new MirButton
            {
                HoverIndex = 115,
                Index = 114,
                Location = new Point(26, 256),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 116,
                Sound = SoundList.ButtonA,
                Hint = "Enable/Dislable group invites.",
            };
            SwitchButton.Click += (o, e) => Network.Enqueue(new C.SwitchGroup { AllowGroup = !AllowGroup });

            AddButton = new MirButton
            {
                Index = 228,
                HoverIndex = 229,
                PressedIndex = 230,
                Location = new Point(59, 261),
                Library = Libraries.GameScene,
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            AddButton.Click += (o, e) => AddMember();

            AddLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = AddButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Add Mem",
                NotControl = true,
            };

            DelButton = new MirButton
            {
                Index = 228,
                HoverIndex = 229,
                PressedIndex = 230,
                Location = new Point(145, 261),
                Library = Libraries.GameScene,
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            DelButton.Click += (o, e) => DelMember();

            DelLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = DelButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Del Mem",
                NotControl = true,
            };

            BeforeDraw += GroupPanel_BeforeDraw;

            GroupList.Clear();
        }

        private void GroupPanel_BeforeDraw(object sender, EventArgs e)
        {
            if (GroupList.Count == 0)
            {
                AddLabel.Text = "Create";
            }
            else
            {
                AddLabel.Text = "Add Mem";
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {
                AddButton.Visible = false;
                DelButton.Visible = false;
            }
            else
            {
                AddButton.Visible = true;
                DelButton.Visible = true;
            }

            if (AllowGroup)
            {
                SwitchButton.Index = 117;
                SwitchButton.HoverIndex = 118;
                SwitchButton.PressedIndex = 119;
            }
            else
            {
                SwitchButton.Index = 114;
                SwitchButton.HoverIndex = 115;
                SwitchButton.PressedIndex = 116;
            }

            for (int i = 0; i < GroupMembers.Length; i++)
                GroupMembers[i].Text = i >= GroupList.Count ? string.Empty : GroupList[i];
        }

        public void AddMember(string name)
        {
            if (GroupList.Count >= Globals.MaxGroup)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("Your group already has the maximum number of members.", ChatType.System);
                return;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("You are not the leader of your group.", ChatType.System);
                return;
            }
            Network.Enqueue(new C.AddMember { Name = name });
        }

        private void AddMember()
        {
            if (GroupList.Count >= Globals.MaxGroup)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("Your group already has the maximum number of members.", ChatType.System);
                return;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {

                GameScene.Scene.ChatDialog.ReceiveChat("You are not the leader of your group.", ChatType.System);
                return;
            }

            MirInputBox inputBox = new MirInputBox("Please enter the name of the person you wish to group.");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.AddMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }
        private void DelMember()
        {
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {

                GameScene.Scene.ChatDialog.ReceiveChat("You are not the leader of your group.", ChatType.System);
                return;
            }

            MirInputBox inputBox = new MirInputBox("Please enter the name of the person you wish to remove.");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.DelMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }


        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }
        public void Show()
        {
            if (Visible) return;
            Visible = true;
        }
    }
}
