using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirSounds;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class MentorDialog : MirImageControl
    {
        public MirImageControl TitleLabel;
        public MirButton CloseButton, AllowButton, AddButton, RemoveButton;
        public MirLabel MentorNameLabel, MentorLevelLabel, MentorOnlineLabel, StudentNameLabel, StudentLevelLabel, StudentOnlineLabel, MentorLabel, StudentLabel, MenteeEXPLabel;
        public MirLabel AddLabel, RemoveLabel;

        public string MentorName;
        public ushort MentorLevel;
        public bool MentorOnline;
        public long MenteeEXP;

        public MentorDialog()
        {
            Index = 245;
            Library = Libraries.GameScene;
            Movable = true;
            Sort = true;
            Location = Center;

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

            AllowButton = new MirButton
            {
                HoverIndex = 115,
                Index = 114,
                Location = new Point(26, 147),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 116,
                Sound = SoundList.ButtonA,
                Hint = "Allow/Disallow Mentor Requests",
            };
            AllowButton.Click += (o, e) =>
            {
                if (AllowButton.Index == 116)
                {
                    AllowButton.Index = 117;
                    AllowButton.HoverIndex = 118;
                    AllowButton.PressedIndex = 119;
                }
                else
                {
                    AllowButton.Index = 114;
                    AllowButton.HoverIndex = 115;
                    AllowButton.PressedIndex = 116;
                }

                Network.Enqueue(new C.AllowMentor());
            };


            AddButton = new MirButton
            {
                Index = 228,
                HoverIndex = 229,
                PressedIndex = 230,
                Location = new Point(59, 152),
                Library = Libraries.GameScene,
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = "Add Mentor",
            };
            AddButton.Click += (o, e) =>
            {
                if (MentorLevel != 0)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You already have a Mentor.", ChatType.System);
                    return;
                }

                string message = "Please enter the name of the person you would like to be your Mentor.";

                MirInputBox inputBox = new MirInputBox(message);

                inputBox.OKButton.Click += (o1, e1) =>
                {
                    Network.Enqueue(new C.AddMentor { Name = inputBox.InputTextBox.Text });
                    inputBox.Dispose();
                };

                inputBox.Show();

            };

            AddLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = AddButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Mentor",
                NotControl = true,
            };

            RemoveButton = new MirButton
            {
                Index = 228,
                HoverIndex = 229,
                PressedIndex = 230,
                Location = new Point(145, 152),
                Library = Libraries.GameScene,
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = "Remove Mentor/Mentee",
            };
            RemoveButton.Click += (o, e) =>
            {
                if (MentorName == "")
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You don't currently have a Mentorship to cancel.", ChatType.System);
                    return;
                }

                MirMessageBox messageBox = new MirMessageBox(string.Format("Cancelling a Mentorship early will cause a cooldown. Are you sure?"), MirMessageBoxButtons.YesNo);

                messageBox.YesButton.Click += (oo, ee) => Network.Enqueue(new C.CancelMentor { });
                messageBox.NoButton.Click += (oo, ee) => { messageBox.Dispose(); };

                messageBox.Show();

            };

            RemoveLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = RemoveButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Secession",
                NotControl = true,
            };

            MentorNameLabel = new MirLabel
            {
                Location = new Point(35, 73),
                Size = new Size(100, 19),
                BackColour = Color.Empty,
                ForeColour = Color.LightGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 9F),
            };

            MentorLevelLabel = new MirLabel
            {
                Location = new Point(145, 73),
                Size = new Size(70, 19),
                BackColour = Color.Empty,
                ForeColour = Color.LightGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 8F),
            };

            StudentNameLabel = new MirLabel
            {
                Location = new Point(35, 107),
                Size = new Size(100, 19),
                BackColour = Color.Empty,
                ForeColour = Color.LightGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 8F),
            };

            StudentLevelLabel = new MirLabel
            {
                Location = new Point(145, 107),
                Size = new Size(70, 19),
                BackColour = Color.Empty,
                ForeColour = Color.LightGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 8F),
            };

            MentorLabel = new MirLabel
            {
                Location = new Point(35, 60),
                Size = new Size(180, 11),
                BackColour = Color.Empty,
                ForeColour = Color.DimGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 7F),
                Text = "Mentor:",
            };

            StudentLabel = new MirLabel
            {
                Location = new Point(35, 94),
                Size = new Size(180, 11),
                BackColour = Color.Empty,
                ForeColour = Color.DimGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 7F),
                Text = "Student:",
            };

            MenteeEXPLabel = new MirLabel
            {
                Location = new Point(35, 127),
                Size = new Size(180, 11),
                BackColour = Color.Empty,
                ForeColour = Color.DimGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 7F),
            };




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

        public void UpdateInterface()
        {
            if (MentorLevel == 0)
            {
                MentorNameLabel.Visible = false;
                MentorLevelLabel.Visible = false;
                StudentNameLabel.Visible = false;
                StudentLevelLabel.Visible = false;
                MenteeEXPLabel.Visible = false;
                return;
            }

            MentorNameLabel.Visible = true;
            MentorLevelLabel.Visible = true;
            StudentNameLabel.Visible = true;
            StudentLevelLabel.Visible = true;

            if (GameScene.User.Level > MentorLevel)
            {
                MentorNameLabel.Text = GameScene.User.Name;
                MentorLevelLabel.Text = "Lv " + GameScene.User.Level.ToString();
                MentorNameLabel.ForeColour = Color.LightGray;

                StudentNameLabel.Text = MentorName;
                StudentLevelLabel.Text = "Lv " + MentorLevel.ToString();
                if (MentorOnline)
                    StudentNameLabel.ForeColour = Color.Green;
                else
                    StudentNameLabel.ForeColour = Color.LightGray;

                MenteeEXPLabel.Visible = true;
                MenteeEXPLabel.Text = "MENTEE EXP: " + MenteeEXP;
            }
            else
            {
                MentorNameLabel.Text = MentorName;
                MentorLevelLabel.Text = "Lv " + MentorLevel.ToString();
                if (MentorOnline)
                    MentorNameLabel.ForeColour = Color.Green;
                else
                    MentorNameLabel.ForeColour = Color.LightGray;

                StudentNameLabel.Text = GameScene.User.Name;
                StudentLevelLabel.Text = "Lv " + GameScene.User.Level.ToString();
                StudentNameLabel.ForeColour = Color.LightGray;
            }
        }

    }
}
