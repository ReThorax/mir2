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
    public sealed class FriendDialog : MirImageControl
    {
        public MirLabel PageNumberLabel;
        public MirButton CloseButton, PreviousButton, NextButton;
        public MirButton FriendTabButton, BlackListTabButton;
        public MirButton AddButton, RemoveButton, MemoButton, EmailButton, WhisperButton;
        public FriendRow[] Rows = new FriendRow[12];

        public List<ClientFriend> Friends = new List<ClientFriend>();
        private ClientFriend SelectedFriend = null;
        private bool _tempBlockedTab = false;
        private bool _blockedTab = false;

        public int SelectedIndex = 0;
        public int StartIndex = 0;
        public int Page = 0;

        public FriendDialog()
        {
            Index = 242;
            Library = Libraries.GameScene;
            Movable = true;
            Sort = true;
            Location = Center;

            FriendTabButton = new MirButton
            {
                Index = 227,
                Library = Libraries.GameScene,
                Location = new Point(77, 54),
                Parent = this,
                PressedIndex = 227,
                Size = new Size(82, 21),
                Sound = SoundList.ButtonA,
            };
            FriendTabButton.Click += (o, e) =>
            {
                _tempBlockedTab = false;
                UpdateDisplay();
            };

            BlackListTabButton = new MirButton
            {
                Library = Libraries.GameScene,
                Location = new Point(150, 54),
                Parent = this,
                PressedIndex = 227,
                Size = new Size(82, 21),
                Sound = SoundList.ButtonA,
            };
            BlackListTabButton.Click += (o, e) =>
            {
                _tempBlockedTab = true;
                UpdateDisplay();
            };

            PageNumberLabel = new MirLabel
            {
                Text = "",
                Parent = this,
                Size = new Size(198, 18),
                Location = new Point(26, 264),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };

            #region Buttons

            PreviousButton = new MirButton
            {
                Index = 398,
                HoverIndex = 398,
                PressedIndex = 399,
                Library = Libraries.Prguse,
                Parent = this,
                Size = new Size(16, 14),
                Location = new Point(35, 268),
                Sound = SoundList.ButtonA,
            };
            PreviousButton.Click += (o, e) =>
            {
                Page--;
                if (Page < 0) Page = 0;
                StartIndex = Rows.Length * Page;
                Update();
            };

            NextButton = new MirButton
            {
                Index = 396,
                HoverIndex = 396,
                PressedIndex = 397,
                Library = Libraries.Prguse,
                Parent = this,
                Size = new Size(16, 14),
                Location = new Point(201, 268),
                Sound = SoundList.ButtonA,
            };
            NextButton.Click += (o, e) =>
            {
                Page++;
                if (Page > Friends.Count() / Rows.Length) Page = Friends.Count() / Rows.Length;
                StartIndex = Rows.Length * Page;

                Update();
            };

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

            AddButton = new MirButton
            {
                Index = 554,
                HoverIndex = 555,
                PressedIndex = 556,
                Library = Libraries.Prguse,
                Location = new Point(54, 291),
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = "Add Friend"
            };
            AddButton.Click += (o, e) =>
            {
                ;
                string message = string.Format("Please enter the name of the person you would like to {0}.", _blockedTab ? "block" : "add");

                MirInputBox inputBox = new MirInputBox(message);

                inputBox.OKButton.Click += (o1, e1) =>
                {
                    Network.Enqueue(new C.AddFriend { Name = inputBox.InputTextBox.Text, Blocked = _blockedTab });
                    inputBox.Dispose();
                };

                inputBox.Show();
            };

            RemoveButton = new MirButton
            {
                Index = 557,
                HoverIndex = 558,
                PressedIndex = 559,
                Library = Libraries.Prguse,
                Location = new Point(83, 291),
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = "Remove Friend"
            };
            RemoveButton.Click += (o, e) =>
            {
                if (SelectedFriend == null) return;

                MirMessageBox messageBox = new MirMessageBox(string.Format("Are you sure you wish to remove '{0}'?", SelectedFriend.Name), MirMessageBoxButtons.YesNo);

                messageBox.YesButton.Click += (o1, e1) =>
                {
                    Network.Enqueue(new C.RemoveFriend { CharacterIndex = SelectedFriend.Index });
                    messageBox.Dispose();
                };

                messageBox.Show();
            };

            MemoButton = new MirButton
            {
                Index = 560,
                HoverIndex = 561,
                PressedIndex = 562,
                Library = Libraries.Prguse,
                Location = new Point(112, 291),
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = "Memo"
            };
            MemoButton.Click += (o, e) =>
            {
                if (SelectedFriend == null) return;

                GameScene.Scene.MemoDialog.Friend = SelectedFriend;
                GameScene.Scene.MemoDialog.Show();
            };

            EmailButton = new MirButton
            {
                Index = 563,
                HoverIndex = 564,
                PressedIndex = 565,
                Library = Libraries.Prguse,
                Location = new Point(141, 291),
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = "Email"
            };
            EmailButton.Click += (o, e) =>
            {
                if (SelectedFriend == null) return;

                GameScene.Scene.MailComposeLetterDialog.ComposeMail(SelectedFriend.Name);
            };

            WhisperButton = new MirButton
            {
                Index = 566,
                HoverIndex = 567,
                PressedIndex = 568,
                Library = Libraries.Prguse,
                Location = new Point(170, 291),
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = "Whisper"
            };
            WhisperButton.Click += (o, e) =>
            {
                if (SelectedFriend == null) return;

                if (!SelectedFriend.Online)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("Player is not online", ChatType.System);
                    return;
                }

                GameScene.Scene.ChatDialog.ChatTextBox.SetFocus();
                GameScene.Scene.ChatDialog.ChatTextBox.Text = "/" + SelectedFriend.Name + " ";
                GameScene.Scene.ChatDialog.ChatTextBox.Visible = true;
                GameScene.Scene.ChatDialog.ChatTextBox.TextBox.SelectionLength = 0;
                GameScene.Scene.ChatDialog.ChatTextBox.TextBox.SelectionStart = GameScene.Scene.ChatDialog.ChatTextBox.Text.Length;
            };
            #endregion
        }

        private void UpdateDisplay()
        {
            if (!Visible) return;

            if (_blockedTab != _tempBlockedTab)
            {
                _blockedTab = _tempBlockedTab;

                if (_blockedTab)
                {
                    FriendTabButton.Index = -1;
                    BlackListTabButton.Index = 227;
                }
                else
                {
                    FriendTabButton.Index = 227;
                    BlackListTabButton.Index = -1;
                }
                Update();
                GameScene.Scene.MemoDialog.Hide();
                GameScene.Scene.DisposeMemoLabel();
            }
        }

        public void Update(bool clearSelection = true)
        {
            if (clearSelection)
                SelectedFriend = null;

            for (int i = 0; i < Rows.Length; i++)
            {
                if (Rows[i] != null) Rows[i].Dispose();

                Rows[i] = null;
            }

            List<ClientFriend> filteredFriends = new List<ClientFriend>();

            if (_blockedTab)
                filteredFriends = Friends.Where(e => e.Blocked).ToList();
            else
                filteredFriends = Friends.Where(e => !e.Blocked).ToList();

            int maxPage = filteredFriends.Count / Rows.Length + 1;
            if (maxPage < 1) maxPage = 1;

            PageNumberLabel.Text = (Page + 1) + " / " + maxPage;

            int maxIndex = filteredFriends.Count - 1;

            if (StartIndex > maxIndex) StartIndex = maxIndex;
            if (StartIndex < 0) StartIndex = 0;

            for (int i = 0; i < Rows.Length; i++)
            {
                if (i + StartIndex >= filteredFriends.Count) break;

                if (Rows[i] != null)
                    Rows[i].Dispose();

                Rows[i] = new FriendRow
                {
                    Friend = filteredFriends[i + StartIndex],
                    Location = new Point((i % 2) * 90 + 35, 105 + ((i) / 2) * 22),
                    Parent = this,
                };
                Rows[i].Click += (o, e) =>
                {
                    FriendRow row = (FriendRow)o;

                    if (row.Friend != SelectedFriend)
                    {
                        SelectedFriend = row.Friend;
                        SelectedIndex = FindSelectedIndex();
                        UpdateRows();
                    }
                };

                if (SelectedFriend != null)
                {
                    if (SelectedIndex == i)
                    {
                        SelectedFriend = Rows[i].Friend;
                    }
                }
            }
        }

        public void UpdateRows()
        {
            if (SelectedFriend == null)
            {
                if (Rows[0] == null) return;

                SelectedFriend = Rows[0].Friend;
            }

            for (int i = 0; i < Rows.Length; i++)
            {
                if (Rows[i] == null) continue;

                Rows[i].Selected = false;

                if (Rows[i].Friend == SelectedFriend)
                {
                    Rows[i].Selected = true;
                }

                Rows[i].UpdateInterface();
            }
        }

        public int FindSelectedIndex()
        {
            int selectedIndex = 0;
            if (SelectedFriend != null)
            {
                for (int i = 0; i < Rows.Length; i++)
                {
                    if (Rows[i] == null || SelectedFriend != Rows[i].Friend) continue;

                    selectedIndex = i;
                }
            }

            return selectedIndex;
        }

        public void Hide()
        {
            if (!Visible) return;
            Visible = false;

            GameScene.Scene.MemoDialog.Hide();
        }
        public void Show()
        {
            if (Visible) return;
            Visible = true;
            UpdateDisplay();
            Network.Enqueue(new C.RefreshFriends());
        }
    }
    public sealed class FriendRow : MirControl
    {
        public ClientFriend Friend;
        public MirLabel NameLabel, OnlineLabel;

        public bool Selected = false;

        public FriendRow()
        {
            Sound = SoundList.ButtonA;
            Size = new Size(90, 17);

            BeforeDraw += FriendRow_BeforeDraw;

            NameLabel = new MirLabel
            {
                Location = new Point(0, 0),
                Size = new Size(90, 17),
                BackColour = Color.Empty,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
            };

            UpdateInterface();
        }

        void FriendRow_BeforeDraw(object sender, EventArgs e)
        {
            UpdateInterface();
        }

        public void UpdateInterface()
        {
            if (Friend == null) return;

            NameLabel.Text = Friend.Name;

            if (Friend.Online)
            {
                NameLabel.ForeColour = Color.Green;
            }
            else
            {
                NameLabel.ForeColour = Color.White;
            }

            if (Selected)
            {
                NameLabel.BackColour = Color.Gray;
            }
            else
            {
                NameLabel.BackColour = Color.Empty;
            }
        }


        protected override void OnMouseEnter()
        {
            if (Friend == null || Friend.Memo.Length < 1) return;

            base.OnMouseEnter();
            GameScene.Scene.CreateMemoLabel(Friend);
        }
        protected override void OnMouseLeave()
        {
            base.OnMouseLeave();
            GameScene.Scene.DisposeMemoLabel();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Friend = null;
            NameLabel = null;

            Selected = false;
        }
    }
    public sealed class MemoDialog : MirImageControl
    {
        //public MirImageControl TitleLabel;
        public MirTextBox MemoTextBox;
        public MirButton CloseButton, OKButton, CancelButton;
        public MirLabel OKLabel, CancelLabel;

        public ClientFriend Friend;

        public MemoDialog()
        {
            Index = 243;
            Library = Libraries.GameScene;
            Movable = true;
            Sort = true;
            Location = Center;

            MemoTextBox = new MirTextBox
            {
                ForeColour = Color.White,
                Parent = this,
                Font = new Font(Settings.FontName, 8F),
                Location = new Point(26, 59),
                Size = new Size(296, 91),
            };
            MemoTextBox.MultiLine();

            OKButton = new MirButton
            {
                Index = 228,
                Location = new Point(157, 160),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 230,
                HoverIndex = 229,
                Sound = SoundList.ButtonA,
            };
            OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.AddMemo { CharacterIndex = Friend.Index, Memo = MemoTextBox.Text });
                Hide();
            };

            OKLabel = new MirLabel
            {
                Size = new Size(78, 20),
                Parent = OKButton,
                Location = new Point(0, -2),
                NotControl = true,
                Text = "Okay",
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };

            CancelButton = new MirButton
            {
                Index = 228,
                Location = new Point(243, 160),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 230,
                HoverIndex = 229,
                Sound = SoundList.ButtonA,
            };
            CancelButton.Click += (o, e) => Hide();

            CancelLabel = new MirLabel
            {
                Size = new Size(78, 20),
                Parent = CancelButton,
                Location = new Point(0, -2),
                NotControl = true,
                Text = "Cancel",
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };

            #region Buttons

            CloseButton = new MirButton
            {
                HoverIndex = 186,
                Index = 185,
                Location = new Point(301, 25),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 187,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            #endregion
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


            if (Friend == null)
            {
                Hide();
                return;
            }

            MemoTextBox.Text = Friend.Memo;
            MemoTextBox.SetFocus();
            MemoTextBox.TextBox.SelectionLength = 0;
            MemoTextBox.TextBox.SelectionStart = MemoTextBox.Text.Length;
        }
    }
}
