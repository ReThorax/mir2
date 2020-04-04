using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using Microsoft.DirectX.Direct3D;
using Font = System.Drawing.Font;
using S = ServerPackets;
using C = ClientPackets;
using Effect = Client.MirObjects.Effect;

using Client.MirScenes.Dialogs;
using System.Drawing.Imaging;

namespace Client.MirScenes.Dialogs
{
    public sealed class MainDialog : MirImageControl
    {
        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public MirImageControl ExperienceBar, WeightBar, LeftCap, RightCap;
        public MirAnimatedControl HealthOrbGlow;
        public MirButton GameShopButton, InventoryButton, CharacterButton, SkillButton, QuestButton, OptionButton, HelpButton, DuraButton, MailButton, LogOutButton,
                         ExitButton, ChatOptionsButton, BigMapButton, TradeButton, GuildButton, GroupButton, FriendButton, MentorButton, LoverButton, RankingButton,
                         CraftingButton, IntelligentCreatureButton, MountButton, FishingButton;
        public MirControl HealthOrb;
        public MirLabel HealthLabel, ManaLabel, TopLabel, BottomLabel, LevelLabel, CharacterName, ExperienceLabel, GoldLabel, WeightLabel, SpaceLabel, AModeLabel,
                        PModeLabel, SModeLabel, PingLabel, TimeLabel, ObserveLabel;

        public bool HPOnly
        {
            get { return User != null && User.Class == MirClass.Warrior && User.Level < 0; }
        }

        public MainDialog()
        {
            Index = 0;
            Library = Libraries.GameScene;
            Location = new Point(((Settings.ScreenWidth / 2) - (Size.Width / 2)), Settings.ScreenHeight - Size.Height);
            PixelDetect = true;

            PingLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.Yellow,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(this.Size.Width - 424, 86),
                Visible = true
            };

            TimeLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.Cyan,
                OutLineColour = Color.Black,
                Parent = this,
                Border = true,
                BorderColour = Color.Green,
                Location = new Point(this.Size.Width - this.Size.Width + 200, 65),
                Visible = true
            };

            #region Caps
            LeftCap = new MirImageControl
            {
                Index = 3,
                Library = Libraries.GameScene,
                Location = new Point(-108, this.Size.Height - 243),
                Parent = this,
                Visible = false
            };
            RightCap = new MirImageControl
            {
                Index = 4,
                Library = Libraries.GameScene,
                Location = new Point(1024, this.Size.Height - 238),
                Parent = this,
                Visible = false
            };

            if (Settings.Resolution > 1024)
            {
                LeftCap.Visible = true;
                RightCap.Visible = true;
            }
            #endregion

            #region MainDialog Buttons

            #region InventoryButton
            InventoryButton = new MirButton
            {
                HoverIndex = 9,
                Index = 9,
                Library = Libraries.GameScene,
                Location = new Point((this.Size.Width - 28) - 125, 79),
                Parent = this,
                PressedIndex = 10,
                Sound = SoundList.ButtonA,
                Hint = "Inventory (" + CMain.InputKeys.GetKey(KeybindOptions.Inventory) + ")"
            };
            InventoryButton.Click += (o, e) =>
            {
                if (GameScene.Scene.InventoryDialog.Visible)
                    GameScene.Scene.InventoryDialog.Hide();
                else
                    GameScene.Scene.InventoryDialog.Show();
            };
            #endregion

            #region CharacterButton
            CharacterButton = new MirButton
            {
                HoverIndex = 7,
                Index = 7,
                Library = Libraries.GameScene,
                Location = new Point((this.Size.Width - 28) - 89, 58),
                Parent = this,
                PressedIndex = 8,
                Sound = SoundList.ButtonA,
                Hint = "Character (" + CMain.InputKeys.GetKey(KeybindOptions.Equipment) + ")"
            };
            CharacterButton.Click += (o, e) =>
            {
                if (GameScene.Scene.CharacterDialog.Visible && GameScene.Scene.CharacterDialog.CharacterPage.Visible)
                    GameScene.Scene.CharacterDialog.Hide();
                else
                {
                    GameScene.Scene.CharacterDialog.Show();
                    GameScene.Scene.CharacterDialog.ShowCharacterPage();
                }
            };
            #endregion

            #region SkillButton
            SkillButton = new MirButton
            {
                HoverIndex = 11,
                Index = 11,
                Library = Libraries.GameScene,
                Location = new Point((this.Size.Width - 28) - 49, 45),
                Parent = this,
                PressedIndex = 12,
                Sound = SoundList.ButtonA,
                Hint = "Skills (" + CMain.InputKeys.GetKey(KeybindOptions.Skills) + ")"
            };
            SkillButton.Click += (o, e) =>
            {
                if (GameScene.Scene.CharacterDialog.Visible && GameScene.Scene.CharacterDialog.SkillPage.Visible)
                    GameScene.Scene.CharacterDialog.Hide();
                else
                {
                    GameScene.Scene.CharacterDialog.Show();
                    GameScene.Scene.CharacterDialog.ShowSkillPage();
                }
            };
            #endregion

            #region OptionButton
            OptionButton = new MirButton
            {
                HoverIndex = 13,
                Index = 13,
                Library = Libraries.GameScene,
                Location = new Point((this.Size.Width - 28) - 8, 39),
                Parent = this,
                PressedIndex = 14,
                Sound = SoundList.ButtonA,
                Hint = "Options (" + CMain.InputKeys.GetKey(KeybindOptions.Options) + ")"
            };
            OptionButton.Click += (o, e) =>
            {
                if (!GameScene.Scene.OptionDialog.Visible)
                    GameScene.Scene.OptionDialog.Show();
                else GameScene.Scene.OptionDialog.Hide();
            };
            #endregion

            #region DuraButton
            DuraButton = new MirButton()
            {
                HoverIndex = 15,
                Index = 15,
                Library = Libraries.GameScene,
                Location = new Point((this.Size.Width - 20) - 97, 98),
                Parent = this,
                PressedIndex = 16,
                Sound = SoundList.ButtonA,
                Hint = "Dura Panel"
            };
            DuraButton.Click += (o, e) =>
            {
                if (GameScene.Scene.CharacterDuraPanel.Visible == true)
                {
                    GameScene.Scene.CharacterDuraPanel.Hide();
                    Settings.DuraView = false;
                }
                else
                {
                    GameScene.Scene.CharacterDuraPanel.Show();
                    Settings.DuraView = true;
                }
            };
            #endregion

            #region QuestButton
            QuestButton = new MirButton
            {
                HoverIndex = 17,
                Index = 17,
                Library = Libraries.GameScene,
                Location = new Point((this.Size.Width - 20) - 69, 87),
                Parent = this,
                PressedIndex = 18,
                Sound = SoundList.ButtonA,
                Hint = "Quests (" + CMain.InputKeys.GetKey(KeybindOptions.Quests) + ")"
            };
            QuestButton.Click += (o, e) =>
            {
                if (!GameScene.Scene.QuestLogDialog.Visible)
                    GameScene.Scene.QuestLogDialog.Show();
                else GameScene.Scene.QuestLogDialog.Hide();
            };
            #endregion

            #region HelpButton
            HelpButton = new MirButton
            {
                HoverIndex = 21,
                Index = 21,
                Library = Libraries.GameScene,
                Location = new Point((this.Size.Width - 20) - 10, 76),
                Parent = this,
                PressedIndex = 22,
                Sound = SoundList.ButtonA,
                Hint = "Help (" + CMain.InputKeys.GetKey(KeybindOptions.Help) + ")"
            };
            HelpButton.Click += (o, e) =>
            {
                if (GameScene.Scene.HelpDialog.Visible)
                    GameScene.Scene.HelpDialog.Hide();
                else GameScene.Scene.HelpDialog.Show();
            };
            #endregion

            #region MailButton

            MailButton = new MirButton
            {
                Index = 19,
                HoverIndex = 19,
                PressedIndex = 20,
                Parent = this,
                Location = new Point(this.Size.Width - 60, 79),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Mail"
            };
            MailButton.Click += (o, e) => GameScene.Scene.MailListDialog.Toggle();

            #endregion

            #region GameShopButton
            GameShopButton = new MirButton
            {
                HoverIndex = 24,
                Index = 23,
                Library = Libraries.GameScene,
                Location = new Point(this.Size.Width - 45, 194),
                Parent = this,
                PressedIndex = 25,
                Sound = SoundList.ButtonC,
                Hint = "Game Shop (" + CMain.InputKeys.GetKey(KeybindOptions.GameShop) + ")"
            };
            GameShopButton.Click += (o, e) =>
            {
                if (!GameScene.Scene.GameShopDialog.Visible) GameScene.Scene.GameShopDialog.Show();
                else GameScene.Scene.GameShopDialog.Hide();
            };
            #endregion

            #endregion


            #region Chat Buttons



            #region ExitButton

            ExitButton = new MirButton
            {
                Index = 37,
                HoverIndex = 38,
                PressedIndex = 39,
                Parent = this,
                Location = new Point(this.Size.Width - 226, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Exit (" + CMain.InputKeys.GetKey(KeybindOptions.Exit) + ")"
            };
            ExitButton.Click += (o, e) => GameScene.Scene.QuitGame();


            #endregion

            #region LogOutButton

            LogOutButton = new MirButton
            {
                Index = 34,
                HoverIndex = 35,
                PressedIndex = 36,
                Parent = this,
                Location = new Point(this.Size.Width - 259, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Log Out (" + CMain.InputKeys.GetKey(KeybindOptions.Logout) + ")"
            };
            LogOutButton.Click += (o, e) => GameScene.Scene.LogOut();

            #endregion

            #region ChatOptionsButton

            ChatOptionsButton = new MirButton
            {
                Index = 59,
                HoverIndex = 60,
                PressedIndex = 61,
                Parent = this,
                Location = new Point(this.Size.Width - 292, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Chat Options"
            };
            ChatOptionsButton.Click += (o, e) =>
            {
                if (GameScene.Scene.ChatOptionDialog.Visible)
                    GameScene.Scene.ChatOptionDialog.Hide();
                else
                    GameScene.Scene.ChatOptionDialog.Show();
            };

            #endregion

            #region BigMapButton

            BigMapButton = new MirButton
            {
                Index = 41,
                HoverIndex = 42,
                PressedIndex = 43,
                Parent = this,
                Location = new Point(this.Size.Width - 820, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "BigMap (" + CMain.InputKeys.GetKey(KeybindOptions.Bigmap) + ")"
            };
            BigMapButton.Click += (o, e) => GameScene.Scene.BigMapDialog.Toggle();

            #endregion

            #region TradeButton

            TradeButton = new MirButton
            {
                Index = 44,
                HoverIndex = 45,
                PressedIndex = 46,
                Parent = this,
                Location = new Point(this.Size.Width - 787, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Trade (" + CMain.InputKeys.GetKey(KeybindOptions.Trade) + ")"
            };
            TradeButton.Click += (o, e) => Network.Enqueue(new C.TradeRequest());

            #endregion

            #region GuildButton

            GuildButton = new MirButton
            {
                Index = 47,
                HoverIndex = 48,
                PressedIndex = 49,
                Parent = this,
                Location = new Point(this.Size.Width - 754, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Guild (" + CMain.InputKeys.GetKey(KeybindOptions.Guilds) + ")"
            };
            GuildButton.Click += (o, e) =>
            {
                if (GameScene.Scene.GuildDialog.Visible)
                    GameScene.Scene.GuildDialog.Hide();
                else GameScene.Scene.GuildDialog.Show();
            };

            #endregion

            #region GroupButton

            GroupButton = new MirButton
            {
                Index = 50,
                HoverIndex = 51,
                PressedIndex = 52,
                Parent = this,
                Location = new Point(this.Size.Width - 721, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Group (" + CMain.InputKeys.GetKey(KeybindOptions.Group) + ")"
            };
            GroupButton.Click += (o, e) =>
            {
                if (GameScene.Scene.GroupDialog.Visible)
                    GameScene.Scene.GroupDialog.Hide();
                else GameScene.Scene.GroupDialog.Show();
            };

            #endregion

            #region FriendButton

            FriendButton = new MirButton
            {
                Index = 62,
                HoverIndex = 63,
                PressedIndex = 64,
                Parent = this,
                Location = new Point(this.Size.Width - 688, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Friends (" + CMain.InputKeys.GetKey(KeybindOptions.Friends) + ")"
            };
            FriendButton.Click += (o, e) =>
            {
                if (GameScene.Scene.FriendDialog.Visible)
                    GameScene.Scene.FriendDialog.Hide();
                else GameScene.Scene.FriendDialog.Show();
            };

            #endregion

            #region MentorButton

            MentorButton = new MirButton
            {
                Index = 53,
                HoverIndex = 54,
                PressedIndex = 55,
                Parent = this,
                Location = new Point(this.Size.Width - 655, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Mentor (" + CMain.InputKeys.GetKey(KeybindOptions.Mentor) + ")"
            };
            MentorButton.Click += (o, e) =>
            {
                if (GameScene.Scene.MentorDialog.Visible)
                    GameScene.Scene.MentorDialog.Hide();
                else GameScene.Scene.MentorDialog.Show();
            };

            #endregion

            #region LoverButton

            LoverButton = new MirButton
            {
                Index = 65,
                HoverIndex = 66,
                PressedIndex = 67,
                Parent = this,
                Location = new Point(this.Size.Width - 622, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Relationship (" + CMain.InputKeys.GetKey(KeybindOptions.Relationship) + ")"
            };
            LoverButton.Click += (o, e) =>
            {
                if (GameScene.Scene.RelationshipDialog.Visible)
                    GameScene.Scene.RelationshipDialog.Hide();
                else GameScene.Scene.RelationshipDialog.Show();
            };

            #endregion

            #region RankingButton

            RankingButton = new MirButton
            {
                Index = 68,
                HoverIndex = 69,
                PressedIndex = 70,
                Parent = this,
                Location = new Point(this.Size.Width - 589, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Ranking (" + CMain.InputKeys.GetKey(KeybindOptions.Ranking) + ")"
            };
            RankingButton.Click += (o, e) =>
            {
                if (GameScene.Scene.RankingDialog.Visible)
                    GameScene.Scene.RankingDialog.Hide();
                else GameScene.Scene.RankingDialog.Show();
            };

            #endregion

            #region CraftingButton

            CraftingButton = new MirButton
            {
                Index = 71,
                HoverIndex = 72,
                PressedIndex = 73,
                Parent = this,
                Location = new Point(this.Size.Width - 556, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Crafting (Coming Soon)"
            };
            CraftingButton.Click += (o, e) =>
            {

                //Crafting Not Implemented Yet

            };

            #endregion

            #region IntelligentCreatureButton

            IntelligentCreatureButton = new MirButton
            {
                Index = 74,
                HoverIndex = 75,
                PressedIndex = 76,
                Parent = this,
                Location = new Point(this.Size.Width - 523, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Creatures (" + CMain.InputKeys.GetKey(KeybindOptions.Creature) + ")"
            };
            IntelligentCreatureButton.Click += (o, e) =>
            {
                if (GameScene.Scene.IntelligentCreatureDialog.Visible)
                    GameScene.Scene.IntelligentCreatureDialog.Hide();
                else GameScene.Scene.IntelligentCreatureDialog.Show();
            };

            #endregion

            #region MountButton

            MountButton = new MirButton
            {
                Index = 77,
                HoverIndex = 78,
                PressedIndex = 79,
                Parent = this,
                Location = new Point(this.Size.Width - 490, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Mount (" + CMain.InputKeys.GetKey(KeybindOptions.Mount) + ")"
            };
            MountButton.Click += (o, e) =>
            {
                if (GameScene.Scene.MountDialog.Visible)
                    GameScene.Scene.MountDialog.Hide();
                else GameScene.Scene.MountDialog.Show();
            };

            #endregion

            #region FishingButton

            FishingButton = new MirButton
            {
                Index = 80,
                HoverIndex = 81,
                PressedIndex = 82,
                Parent = this,
                Location = new Point(this.Size.Width - 457, 86),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Fishing (" + CMain.InputKeys.GetKey(KeybindOptions.Fishing) + ")"
            };
            FishingButton.Click += (o, e) =>
            {
                if (GameScene.Scene.FishingDialog.Visible)
                    GameScene.Scene.FishingDialog.Hide();
                else GameScene.Scene.FishingDialog.Show();
            };

            #endregion

            #endregion Chat Buttons


            #region Health Orb
            HealthOrb = new MirControl
            {
                Parent = this,
                Location = new Point(0, 76),
                NotControl = true,
            };

            HealthOrb.BeforeDraw += HealthOrb_BeforeDraw;

            //HealthOrbGlow = new MirAnimatedControl
            //{
            //    Parent = this,
            //    Animated = true,
            //    AnimationCount = 14,
            //    Location = new Point(0, 76),
            //    NotControl = true,
            //};
            //HealthOrbGlow.BeforeDraw += HealthOrbGlow_BeforeDraw;

            HealthLabel = new MirLabel
            {
                AutoSize = true,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Location = new Point(40, 130),
                Parent = HealthOrb
            };


            ManaLabel = new MirLabel
            {
                AutoSize = true,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Location = new Point(40, 146),
                Parent = HealthOrb
            };


            TopLabel = new MirLabel
            {
                Size = new Size(85, 30),
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Location = new Point(14, 20),
                Parent = HealthOrb,
            };

            BottomLabel = new MirLabel
            {
                Size = new Size(85, 30),
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Location = new Point(14, 50),
                Parent = HealthOrb,
            };
            #endregion


            #region Level Label
            LevelLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(this.Size.Width - 131, 157)
            };
            #endregion

            #region Name Label
            CharacterName = new MirLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(this.Size.Width - 131, 127),
                Size = new Size(90, 16)
            };
            #endregion

            ExperienceBar = new MirImageControl
            {
                Index = 40,
                Library = Libraries.GameScene,
                Location = new Point(this.Size.Width - 130, 182),
                Parent = this,
                DrawImage = false,
                NotControl = true,
            };
            ExperienceBar.BeforeDraw += ExperienceBar_BeforeDraw;

            ExperienceLabel = new MirLabel
            {
                AutoSize = true,
                Parent = ExperienceBar,
                NotControl = true,
            };

            GoldLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter,
                Font = new Font(Settings.FontName, 8F),
                Location = new Point(this.Size.Width - 130, 219),
                Parent = this,
                Size = new Size(85, 13),
                Sound = SoundList.Gold,
            };
            GoldLabel.Click += (o, e) =>
            {
                if (GameScene.SelectedCell == null)
                    GameScene.PickedUpGold = !GameScene.PickedUpGold && GameScene.Gold > 0;
            };



            WeightBar = new MirImageControl
            {
                Index = 40,
                Library = Libraries.GameScene,
                Location = new Point(this.Size.Width - 130, 203),
                Parent = this,
                DrawImage = false,
                NotControl = true,
            };
            WeightBar.BeforeDraw += WeightBar_BeforeDraw;

            WeightLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(this.Size.Width - 131, 199),
                Size = new Size(35, 14),
            };

            SpaceLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(this.Size.Width - 75, 199),
                Size = new Size(26, 14),
            };


            AModeLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.Yellow,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(Settings.ScreenWidth - 145, 195),
            };

            ObserveLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.SlateBlue,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(Settings.Resolution != 800 ? 899 : 675, Settings.Resolution != 800 ? -443 : -235),
                Text = "[Observers: 0]",
                Visible = false,
            };

            PModeLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.Orange,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(230, 155),
                Visible = true
            };

            SModeLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.LimeGreen,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(Settings.Resolution != 800 ? 899 : 675, Settings.Resolution != 800 ? -463 : -295),
            };

        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Process()
        {
            if (GameScene.User == null) return;
            TimeLabel.Text = string.Format("Time: {0}:{1}:{2}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            switch (GameScene.Scene.AMode)
            {
                case AttackMode.Peace:
                    AModeLabel.Text = GameLanguage.AttackMode_Peace;
                    break;
                case AttackMode.Group:
                    AModeLabel.Text = GameLanguage.AttackMode_Group;
                    break;
                case AttackMode.Guild:
                    AModeLabel.Text = GameLanguage.AttackMode_Guild;
                    break;
                case AttackMode.EnemyGuild:
                    AModeLabel.Text = GameLanguage.AttackMode_EnemyGuild;
                    break;
                case AttackMode.RedBrown:
                    AModeLabel.Text = GameLanguage.AttackMode_RedBrown;
                    break;
                case AttackMode.All:
                    AModeLabel.Text = GameLanguage.AttackMode_All;
                    break;
            }

            switch (GameScene.Scene.PMode)
            {
                case PetMode.Both:
                    PModeLabel.Text = GameLanguage.PetMode_Both;
                    break;
                case PetMode.MoveOnly:
                    PModeLabel.Text = GameLanguage.PetMode_MoveOnly;
                    break;
                case PetMode.AttackOnly:
                    PModeLabel.Text = GameLanguage.PetMode_AttackOnly;
                    break;
                case PetMode.None:
                    PModeLabel.Text = GameLanguage.PetMode_None;
                    break;
            }

            if ((GameScene.Scene.PingTime) > 0)
            {
                PingLabel.Text = string.Format("Ping: {0}", GameScene.Scene.PingTime);
                PingLabel.Visible = true;
            }
            else
            {
                PingLabel.Visible = false;
            }



            switch (Settings.SkillMode)
            {
                case true:
                    SModeLabel.Text = "[Skill Mode: ~]";
                    break;
                case false:
                    SModeLabel.Text = "[Skill Mode: Ctrl]";
                    break;
            }

            if (Settings.HPView)
            {
                HealthLabel.Text = string.Format("{0}/{1}", User.HP, User.MaxHP);
                ManaLabel.Text = HPOnly ? "" : string.Format("{0}/{1} ", User.MP, User.MaxMP);
                TopLabel.Text = string.Empty;
                BottomLabel.Text = string.Empty;
            }
            else
            {
                if (HPOnly)
                {
                    TopLabel.Text = string.Format("{0}\n" + "--", User.HP);
                    BottomLabel.Text = string.Format("{0}", User.MaxHP);
                }
                else
                {
                    TopLabel.Text = string.Format(" {0}    {1} \n" + "---------------", User.HP, User.MP);
                    BottomLabel.Text = string.Format(" {0}    {1} ", User.MaxHP, User.MaxMP);
                }
                HealthLabel.Text = string.Empty;
                ManaLabel.Text = string.Empty;
            }

            LevelLabel.Text = User.Level.ToString();
            ExperienceLabel.Text = string.Format("{0:#0.##%}", User.Experience / (double)User.MaxExperience);
            ExperienceLabel.Location = new Point((ExperienceBar.Size.Width / 2) - 20, -4);
            GoldLabel.Text = GameScene.Gold.ToString("###,###,##0");
            CharacterName.Text = User.Name;
            SpaceLabel.Text = User.Inventory.Count(t => t == null).ToString();
            WeightLabel.Text = (MapObject.User.MaxBagWeight - MapObject.User.CurrentBagWeight).ToString();
        }

        private void Label_SizeChanged(object sender, EventArgs e)
        {
            MirLabel l = sender as MirLabel;

            if (l == null) return;

            l.Location = new Point(50 - (l.Size.Width / 2), l.Location.Y);
        }

        private void HealthOrb_BeforeDraw(object sender, EventArgs e)
        {
            if (Libraries.GameScene == null) return;

            int height;
            if (User != null && User.HP != User.MaxHP)
                height = (int)(114 * User.HP / (float)User.MaxHP);
            else
                height = 114;

            if (height < 0) height = 0;
            if (height > 114) height = 114;

            int orbImage = 1;

            bool hpOnly = false;

            if (HPOnly)
            {
                hpOnly = true;
                orbImage = 1;
            }

            Rectangle r = new Rectangle(0, 114 - height, hpOnly ? 114 : 57, height);
            Libraries.GameScene.Draw(orbImage, r, new Point((Settings.ScreenWidth / 2) - 474, HealthOrb.DisplayLocation.Y + 114 - height), Color.White, false);

            if (hpOnly) return;

            if (User.MP != User.MaxMP)
                height = (int)(114 * User.MP / (float)User.MaxMP);
            else
                height = 114;

            if (height < 0) height = 0;
            if (height > 114) height = 114;
            r = new Rectangle(0, 114 - height, 57, height);

            Libraries.GameScene.Draw(2, r, new Point(((Settings.ScreenWidth / 2) - 474) + 57, HealthOrb.DisplayLocation.Y + 114 - height), Color.White, false);
        }

        // private void HealthOrbGlow_BeforeDraw(object sender, EventArgs e)
        // {
        //     if (Libraries.GameScene == null) return;

        //     int height;
        //     if (User != null && User.HP != User.MaxHP)
        //         height = (int)(114 * User.HP / (float)User.MaxHP);
        //     else
        //         height = 120;

        //     if (height < 0) height = 0;
        //     if (height > 120) height = 120;

        //     int orbImage = 1;

        //     bool hpOnly = false;

        //     if (HPOnly)
        //     {
        //         hpOnly = true;
        //         orbImage = 1;
        //     }

        //     Rectangle r = new Rectangle(0, 120 - height, hpOnly ? 120 : 57, height);
        //     Libraries.GameScene.DrawBlend(59, new Point((Settings.ScreenWidth / 2) - 474, HealthOrbGlow.DisplayLocation.Y + 120 - height), Color.White, false);

        //     if (hpOnly) return;

        //     if (User.MP != User.MaxMP)
        //         height = (int)(114 * User.MP / (float)User.MaxMP);
        //     else
        //         height = 114;

        //     if (height < 0) height = 0;
        //     if (height > 114) height = 114;
        //     r = new Rectangle(0, 120 - height, 60, height);

        //     Libraries.GameScene.DrawBlend(73, new Point(((Settings.ScreenWidth / 2) - 474) + 60, HealthOrbGlow.DisplayLocation.Y + 120 - height), Color.White, false);
        //}

        private void ExperienceBar_BeforeDraw(object sender, EventArgs e)
        {
            if (ExperienceBar.Library == null) return;
            if (MapObject.User.Experience <= 0) return;

            double percent = MapObject.User.Experience / (double)MapObject.User.MaxExperience;
            if (percent > 1) percent = 1;
            if (percent <= 0) return;

            Rectangle section = new Rectangle
            {
                Size = new Size((int)((ExperienceBar.Size.Width - 0) * percent), ExperienceBar.Size.Height)
            };

            ExperienceBar.Library.Draw(ExperienceBar.Index, section, ExperienceBar.DisplayLocation, Color.White, false);
        }

        private void WeightBar_BeforeDraw(object sender, EventArgs e)
        {
            if (WeightBar.Library == null) return;
            double percent = MapObject.User.CurrentBagWeight / (double)MapObject.User.MaxBagWeight;
            if (percent > 1) percent = 1;
            if (percent <= 0) return;

            Rectangle section = new Rectangle
            {
                Size = new Size((int)((WeightBar.Size.Width - 0) * percent), WeightBar.Size.Height)
            };

            WeightBar.Library.Draw(WeightBar.Index, section, WeightBar.DisplayLocation, Color.White, false);
        }
    }
    public sealed class ChatDialog : MirImageControl
    {
        public List<ChatHistory> FullHistory = new List<ChatHistory>();
        public List<ChatHistory> History = new List<ChatHistory>();
        public List<MirLabel> ChatLines = new List<MirLabel>();

        public MirButton HomeButton, UpButton, EndButton, DownButton, PositionBar;
        public MirImageControl CountBar;
        public MirTextBox ChatTextBox;
        public Font ChatFont = new Font(Settings.FontName, 8F);
        public string LastPM = string.Empty;

        public int StartIndex, LineCount = 8, WindowSize;
        public string ChatPrefix = "";

        public bool Transparent;

        public ChatDialog()
        {
            Index = 33;
            Library = Libraries.GameScene;
            Location = new Point(GameScene.Scene.MainDialog.Location.X + 204, Settings.ScreenHeight - 133);
            PixelDetect = true;

            KeyPress += ChatPanel_KeyPress;
            KeyDown += ChatPanel_KeyDown;
            MouseWheel += ChatPanel_MouseWheel;

            ChatTextBox = new MirTextBox
            {
                BackColour = Color.DarkGray,
                ForeColour = Color.Black,
                Parent = this,
                Size = new Size(626, 13),
                Location = new Point(0, 117),
                MaxLength = Globals.MaxChatLength,
                Visible = false,
                Font = ChatFont,
            };
            ChatTextBox.TextBox.KeyPress += ChatTextBox_KeyPress;
            ChatTextBox.TextBox.KeyDown += ChatTextBox_KeyDown;
            ChatTextBox.TextBox.KeyUp += ChatTextBox_KeyUp;


            UpButton = new MirButton
            {
                Index = 2021,
                HoverIndex = 2022,
                Library = Libraries.Prguse,
                Location = new Point(this.Size.Width - 16, 5),
                Parent = this,
                PressedIndex = 2023,
                Sound = SoundList.ButtonA,
            };
            UpButton.Click += (o, e) =>
            {
                if (StartIndex == 0) return;
                StartIndex--;
                Update();
            };

            DownButton = new MirButton
            {
                Index = 2024,
                HoverIndex = 2025,
                Library = Libraries.Prguse,
                Location = new Point(this.Size.Width - 16, 100),
                Parent = this,
                PressedIndex = 2026,
                Sound = SoundList.ButtonA,
            };
            DownButton.Click += (o, e) =>
            {
                if (StartIndex == History.Count - 1) return;
                StartIndex++;
                Update();
            };



            CountBar = new MirImageControl
            {
                Index = 2012,
                Library = Libraries.Prguse,
                Location = new Point(this.Size.Width - 16, 21),
                Parent = this,
            };

            PositionBar = new MirButton
            {
                Index = 2015,
                HoverIndex = 2016,
                Library = Libraries.Prguse,
                Location = new Point(610, 21),
                Parent = this,
                PressedIndex = 2017,
                Movable = true,
                Sound = SoundList.None,
            };
            PositionBar.OnMoving += PositionBar_OnMoving;

        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

        private void ChatTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:
                    e.Handled = true;
                    if (!string.IsNullOrEmpty(ChatTextBox.Text))
                    {
                        string msg = ChatTextBox.Text;

                        if (msg.ToUpper() == "@LEVELEFFECT")
                        {
                            Settings.LevelEffect = !Settings.LevelEffect;
                        }

                        if (msg.ToUpper() == "@TARGETDEAD")
                        {
                            Settings.TargetDead = !Settings.TargetDead;
                        }

                        Network.Enqueue(new C.Chat
                        {
                            Message = msg
                        });

                        if (ChatTextBox.Text[0] == '/')
                        {
                            string[] parts = ChatTextBox.Text.Split(' ');
                            if (parts.Length > 0)
                                LastPM = parts[0];
                        }
                    }
                    ChatTextBox.Visible = false;
                    ChatTextBox.Text = string.Empty;
                    break;
                case (char)Keys.Escape:
                    e.Handled = true;
                    ChatTextBox.Visible = false;
                    ChatTextBox.Text = string.Empty;
                    break;
            }
        }

        void PositionBar_OnMoving(object sender, MouseEventArgs e)
        {
            int x = 610;
            int y = PositionBar.Location.Y;
            if (y >= 21 + CountBar.Size.Height - PositionBar.Size.Height) y = 21 + CountBar.Size.Height - PositionBar.Size.Height;
            if (y < 21) y = 21;

            int h = CountBar.Size.Height - PositionBar.Size.Height;
            h = (int)((y - 21) / (h / (float)(History.Count - 1)));

            if (h != StartIndex)
            {
                StartIndex = h;
                Update();
            }

            PositionBar.Location = new Point(x, y);
        }

        public void ReceiveChat(string text, ChatType type)
        {
            Color foreColour, backColour;

            switch (type)
            {
                case ChatType.Hint:
                    backColour = Color.White;
                    foreColour = Color.DarkGreen;
                    break;
                case ChatType.Announcement:
                    backColour = Color.Blue;
                    foreColour = Color.White;
                    GameScene.Scene.ChatNoticeDialog.ShowNotice(text);
                    break;
                case ChatType.Shout:
                    backColour = Color.Yellow;
                    foreColour = Color.Black;
                    break;
                case ChatType.Shout2:
                    backColour = Color.Green;
                    foreColour = Color.White;
                    break;
                case ChatType.Shout3:
                    backColour = Color.Purple;
                    foreColour = Color.White;
                    break;
                case ChatType.System:
                    backColour = Color.Red;
                    foreColour = Color.White;
                    break;
                case ChatType.System2:
                    backColour = Color.DarkRed;
                    foreColour = Color.White;
                    break;
                case ChatType.Group:
                    backColour = Color.White;
                    foreColour = Color.Brown;
                    break;
                case ChatType.WhisperOut:
                    foreColour = Color.CornflowerBlue;
                    backColour = Color.White;
                    break;
                case ChatType.WhisperIn:
                    foreColour = Color.DarkBlue;
                    backColour = Color.White;
                    break;
                case ChatType.Guild:
                    backColour = Color.White;
                    foreColour = Color.Green;
                    break;
                case ChatType.LevelUp:
                    backColour = Color.FromArgb(255, 225, 185, 250);
                    foreColour = Color.Blue;
                    break;
                case ChatType.Relationship:
                    backColour = Color.Transparent;
                    foreColour = Color.HotPink;
                    break;
                case ChatType.Mentor:
                    backColour = Color.White;
                    foreColour = Color.Purple;
                    break;
                case ChatType.Observer:
                    backColour = Color.White;
                    foreColour = Color.SlateBlue;
                    break;
                default:
                    backColour = Color.White;
                    foreColour = Color.Black;
                    break;
            }

            int chatWidth = Settings.Resolution != 800 ? 614 : 390;
            List<string> chat = new List<string>();

            int index = 0;
            for (int i = 1; i < text.Length; i++)
                if (TextRenderer.MeasureText(CMain.Graphics, text.Substring(index, i - index), ChatFont).Width > chatWidth)
                {
                    chat.Add(text.Substring(index, i - index - 1));
                    index = i - 1;
                }
            chat.Add(text.Substring(index, text.Length - index));

            if (StartIndex == History.Count - LineCount)
                StartIndex += chat.Count;

            for (int i = 0; i < chat.Count; i++)
                FullHistory.Add(new ChatHistory { Text = chat[i], BackColour = backColour, ForeColour = foreColour, Type = type });

            Update();
        }

        public void Update()
        {
            History = new List<ChatHistory>();

            for (int i = 0; i < FullHistory.Count; i++)
            {
                switch (FullHistory[i].Type)
                {
                    case ChatType.Normal:
                        if (Settings.FilterNormalChat) continue;
                        break;
                    case ChatType.WhisperIn:
                    case ChatType.WhisperOut:
                        if (Settings.FilterWhisperChat) continue;
                        break;
                    case ChatType.Shout:
                        if (Settings.FilterShoutChat) continue;
                        break;
                    case ChatType.System:
                    case ChatType.System2:
                        if (Settings.FilterSystemChat) continue;
                        break;
                    case ChatType.Group:
                        if (Settings.FilterGroupChat) continue;
                        break;
                    case ChatType.Guild:
                        if (Settings.FilterGuildChat) continue;
                        break;
                }

                History.Add(FullHistory[i]);
            }

            for (int i = 0; i < ChatLines.Count; i++)
                ChatLines[i].Dispose();

            ChatLines.Clear();

            if (StartIndex >= History.Count) StartIndex = History.Count - 1;
            if (StartIndex < 0) StartIndex = 0;

            if (History.Count > 1)
            {
                int h = CountBar.Size.Height - PositionBar.Size.Height;
                h = (int)((h / (float)(History.Count - 1)) * StartIndex);
                PositionBar.Location = new Point(Settings.Resolution != 800 ? 610 : 395, 21 + h);
            }

            int y = 1;

            for (int i = StartIndex; i < History.Count; i++)
            {
                MirLabel temp = new MirLabel
                {
                    AutoSize = true,
                    BackColour = History[i].BackColour,
                    ForeColour = History[i].ForeColour,
                    Location = new Point(1, y),
                    OutLine = false,
                    Parent = this,
                    Text = History[i].Text,
                    Font = ChatFont,
                };
                temp.MouseWheel += ChatPanel_MouseWheel;
                ChatLines.Add(temp);

                temp.Click += (o, e) =>
                {
                    MirLabel l = o as MirLabel;
                    if (l == null) return;

                    string[] parts = l.Text.Split(':', ' ');
                    if (parts.Length == 0) return;

                    string name = Regex.Replace(parts[0], "[^A-Za-z0-9]", "");

                    ChatTextBox.SetFocus();
                    ChatTextBox.Text = string.Format("/{0} ", name);
                    ChatTextBox.Visible = true;
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                };


                y += 13;
                if (i - StartIndex == LineCount - 1) break;
            }

        }

        private void ChatPanel_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (StartIndex == 0) return;
                    StartIndex--;
                    break;
                case Keys.Home:
                    if (StartIndex == 0) return;
                    StartIndex = 0;
                    break;
                case Keys.Down:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex++;
                    break;
                case Keys.End:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex = History.Count - 1;
                    break;
                case Keys.PageUp:
                    if (StartIndex == 0) return;
                    StartIndex -= LineCount;
                    break;
                case Keys.PageDown:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex += LineCount;
                    break;
                default:
                    return;
            }
            Update();
            e.Handled = true;
        }
        private void ChatPanel_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '@':
                case '!':
                case ' ':
                case (char)Keys.Enter:
                    ChatTextBox.SetFocus();
                    if (e.KeyChar == '!') ChatTextBox.Text = "!";
                    if (e.KeyChar == '@') ChatTextBox.Text = "@";
                    if (ChatPrefix != "") ChatTextBox.Text = ChatPrefix;

                    ChatTextBox.Visible = true;
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                    e.Handled = true;
                    break;
                case '/':
                    ChatTextBox.SetFocus();
                    ChatTextBox.Text = LastPM + " ";
                    ChatTextBox.Visible = true;
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                    e.Handled = true;
                    break;
            }
        }
        private void ChatPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            int count = e.Delta / SystemInformation.MouseWheelScrollDelta;

            if (StartIndex == 0 && count >= 0) return;
            if (StartIndex == History.Count - 1 && count <= 0) return;

            StartIndex -= count;
            Update();
        }
        private void ChatTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            CMain.Shift = e.Shift;
            CMain.Alt = e.Alt;
            CMain.Ctrl = e.Control;

            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.Tab:
                    CMain.CMain_KeyUp(sender, e);
                    break;

            }
        }
        private void ChatTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            CMain.Shift = e.Shift;
            CMain.Alt = e.Alt;
            CMain.Ctrl = e.Control;

            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.Tab:
                    CMain.CMain_KeyDown(sender, e);
                    break;

            }
        }


        public void ChangeSize()
        {
            if (++WindowSize >= 3) WindowSize = 0;

            int y = DisplayRectangle.Bottom;
            switch (WindowSize)
            {
                case 0:
                    LineCount = 4;
                    Index = Settings.Resolution != 800 ? 2221 : 2201;
                    CountBar.Index = 2012;
                    DownButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 39);
                    EndButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 45);
                    ChatTextBox.Location = new Point(1, 54);
                    break;
                case 1:
                    LineCount = 7;
                    Index = Settings.Resolution != 800 ? 2224 : 2204;
                    CountBar.Index = 2013;
                    DownButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 39 + 48);
                    EndButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 45 + 48);
                    ChatTextBox.Location = new Point(1, 54 + 48);
                    break;
                case 2:
                    LineCount = 11;
                    Index = Settings.Resolution != 800 ? 2227 : 2207;
                    CountBar.Index = 2014;
                    DownButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 39 + 96);
                    EndButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 45 + 96);
                    ChatTextBox.Location = new Point(1, 54 + 96);
                    break;
            }

            Location = new Point(Location.X, y - Size.Height);

            UpdateBackground();

            Update();
        }

        public void UpdateBackground()
        {
            int offset = Transparent ? 1 : 0;

            switch (WindowSize)
            {
                case 0:
                    Index = Settings.Resolution != 800 ? 2221 : 2201;
                    break;
                case 1:
                    Index = Settings.Resolution != 800 ? 2224 : 2204;
                    break;
                case 2:
                    Index = Settings.Resolution != 800 ? 2227 : 2207;
                    break;
            }

            Index -= offset;
        }

        public class ChatHistory
        {
            public string Text;
            public Color ForeColour, BackColour;
            public ChatType Type;
        }
    }
    //public sealed class ChatControlBar : MirImageControl
    //{
    //    public MirButton SizeButton, SettingsButton, NormalButton, ShoutButton, WhisperButton, LoverButton, MentorButton, GroupButton, GuildButton, ReportButton;

    //    public ChatControlBar()
    //    {
    //        Index = Settings.Resolution != 800 ? 2034 : 2035;
    //        Library = Libraries.Prguse;
    //        Location = new Point(GameScene.Scene.MainDialog.Location.X + 230, Settings.ScreenHeight - 112);

    //        SizeButton = new MirButton
    //        {
    //            Index = 2057,
    //            HoverIndex = 2058,
    //            PressedIndex = 2059,
    //            Library = Libraries.Prguse,
    //            Parent = this,
    //            Location = new Point(Settings.Resolution != 800 ? 574 : 350, 1),
    //            Visible = true,
    //            Sound = SoundList.ButtonA,
    //            Hint = "Size"
    //        };
    //        SizeButton.Click += (o, e) =>
    //        {
    //            GameScene.Scene.ChatDialog.ChangeSize();
    //            Location = new Point(Location.X, GameScene.Scene.ChatDialog.DisplayRectangle.Top - Size.Height);
    //            if (GameScene.Scene.BeltDialog.Index == 1932)
    //                GameScene.Scene.BeltDialog.Location = new Point(GameScene.Scene.MainDialog.Location.X + 230, Location.Y - GameScene.Scene.BeltDialog.Size.Height);
    //        };

    //        SettingsButton = new MirButton
    //        {
    //            Index = 2060,
    //            HoverIndex = 2061,
    //            PressedIndex = 2062,
    //            Library = Libraries.Prguse,
    //            Parent = this,
    //            Location = new Point(Settings.Resolution != 800 ? 596 : 372, 1),
    //            Sound = SoundList.ButtonA,
    //            Hint = "Chat Settings"
    //        };
    //        SettingsButton.Click += (o, e) =>
    //        {
    //            if (GameScene.Scene.ChatOptionDialog.Visible)
    //                GameScene.Scene.ChatOptionDialog.Hide();
    //            else
    //                GameScene.Scene.ChatOptionDialog.Show();

    //            //GameScene.Scene.ChatDialog.Transparent = !GameScene.Scene.ChatDialog.Transparent;
    //            //GameScene.Scene.ChatDialog.UpdateBackground();
    //        };

    //        NormalButton = new MirButton
    //        {
    //            Index = 2036,
    //            HoverIndex = 2037,
    //            PressedIndex = 2038,
    //            Library = Libraries.Prguse,
    //            Parent = this,
    //            Location = new Point(12, 1),
    //            Sound = SoundList.ButtonA,
    //            Hint = "All"
    //        };
    //        NormalButton.Click += (o, e) =>
    //        {
    //            ToggleChatFilter("All");
    //        };

    //        ShoutButton = new MirButton
    //        {
    //            Index = 2039,
    //            HoverIndex = 2040,
    //            PressedIndex = 2041,
    //            Library = Libraries.Prguse,
    //            Parent = this,
    //            Location = new Point(34, 1),
    //            Sound = SoundList.ButtonA,
    //            Hint = "Shout"
    //        };
    //        ShoutButton.Click += (o, e) =>
    //        {
    //            ToggleChatFilter("Shout");
    //        };

    //        WhisperButton = new MirButton
    //        {
    //            Index = 2042,
    //            HoverIndex = 2043,
    //            PressedIndex = 2044,
    //            Library = Libraries.Prguse,
    //            Parent = this,
    //            Location = new Point(56, 1),
    //            Sound = SoundList.ButtonA,
    //            Hint = "Whisper"
    //        };
    //        WhisperButton.Click += (o, e) =>
    //        {
    //            ToggleChatFilter("Whisper");
    //        };

    //        LoverButton = new MirButton
    //        {
    //            Index = 2045,
    //            HoverIndex = 2046,
    //            PressedIndex = 2047,
    //            Library = Libraries.Prguse,
    //            Parent = this,
    //            Location = new Point(78, 1),
    //            Sound = SoundList.ButtonA,
    //            Hint = "Lover"
    //        };
    //        LoverButton.Click += (o, e) =>
    //        {
    //            ToggleChatFilter("Lover");
    //        };

    //        MentorButton = new MirButton
    //        {
    //            Index = 2048,
    //            HoverIndex = 2049,
    //            PressedIndex = 2050,
    //            Library = Libraries.Prguse,
    //            Parent = this,
    //            Location = new Point(100, 1),
    //            Sound = SoundList.ButtonA,
    //            Hint = "Mentor"
    //        };
    //        MentorButton.Click += (o, e) =>
    //        {
    //            ToggleChatFilter("Mentor");
    //        };

    //        GroupButton = new MirButton
    //        {
    //            Index = 2051,
    //            HoverIndex = 2052,
    //            PressedIndex = 2053,
    //            Library = Libraries.Prguse,
    //            Parent = this,
    //            Location = new Point(122, 1),
    //            Sound = SoundList.ButtonA,
    //            Hint = "Group"
    //        };
    //        GroupButton.Click += (o, e) =>
    //        {
    //            ToggleChatFilter("Group");
    //        };


    //        ReportButton = new MirButton
    //        {
    //            Index = 2063,
    //            HoverIndex = 2064,
    //            PressedIndex = 2065,
    //            Library = Libraries.Prguse,
    //            Parent = this,
    //            Location = new Point(Settings.Resolution != 800 ? 552 : 328, 1),
    //            Sound = SoundList.ButtonA,
    //            Hint = "Report",
    //            Visible = false
    //        };
    //        ReportButton.Click += (o, e) =>
    //        {
    //            GameScene.Scene.ReportDialog.Visible = !GameScene.Scene.ReportDialog.Visible;
    //        };

    //        ToggleChatFilter("All");
    //    }

    //    public void ToggleChatFilter(string chatFilter)
    //    {
    //        NormalButton.Index = 2036;
    //        NormalButton.HoverIndex = 2037;
    //        ShoutButton.Index = 2039;
    //        ShoutButton.HoverIndex = 2040;
    //        WhisperButton.Index = 2042;
    //        WhisperButton.HoverIndex = 2043;
    //        LoverButton.Index = 2045;
    //        LoverButton.HoverIndex = 2046;
    //        MentorButton.Index = 2048;
    //        MentorButton.HoverIndex = 2049;
    //        GroupButton.Index = 2051;
    //        GroupButton.HoverIndex = 2052;
    //        GuildButton.Index = 2054;
    //        GuildButton.HoverIndex = 2055;

    //        GameScene.Scene.ChatDialog.ChatPrefix = "";

    //        switch (chatFilter)
    //        {
    //            case "All":
    //                NormalButton.Index = 2038;
    //                NormalButton.HoverIndex = 2038;
    //                GameScene.Scene.ChatDialog.ChatPrefix = "";
    //                break;
    //            case "Shout":
    //                ShoutButton.Index = 2041;
    //                ShoutButton.HoverIndex = 2041;
    //                GameScene.Scene.ChatDialog.ChatPrefix = "!";
    //                break;
    //            case "Whisper":
    //                WhisperButton.Index = 2044;
    //                WhisperButton.HoverIndex = 2044;
    //                GameScene.Scene.ChatDialog.ChatPrefix = "/";
    //                break;
    //            case "Group":
    //                GroupButton.Index = 2053;
    //                GroupButton.HoverIndex = 2053;
    //                GameScene.Scene.ChatDialog.ChatPrefix = "!!";
    //                break;
    //            case "Guild":
    //                GuildButton.Index = 2056;
    //                GuildButton.HoverIndex = 2056;
    //                GameScene.Scene.ChatDialog.ChatPrefix = "!~";
    //                break;
    //            case "Lover":
    //                LoverButton.Index = 2047;
    //                LoverButton.HoverIndex = 2047;
    //                GameScene.Scene.ChatDialog.ChatPrefix = ":)";
    //                break;
    //            case "Mentor":
    //                MentorButton.Index = 2050;
    //                MentorButton.HoverIndex = 2050;
    //                GameScene.Scene.ChatDialog.ChatPrefix = "!#";
    //                break;
    //        }
    //    }

    //    public void Show()
    //    {
    //        Visible = true;
    //    }

    //    public void Hide()
    //    {
    //        Visible = false;
    //    }
    //}
    public sealed class InventoryDialog : MirImageControl
    {
        public MirImageControl WeightBar;
        public MirImageControl[] LockBar = new MirImageControl[10];
        public MirItemCell[] Grid;
        public MirItemCell[] QuestGrid;

        public MirButton CloseButton, ItemButton, ItemButton2, QuestButton, AddButton, GoldButton;
        public MirLabel GoldLabel, WeightLabel;

        public InventoryDialog()
        {
            Index = 180;
            Library = Libraries.GameScene;
            Movable = true;
            Sort = true;
            Visible = false;

            ItemButton = new MirButton
            {
                Index = 197,
                Library = Libraries.Title,
                Location = new Point(5000, 5000),
                Parent = this,
                Size = new Size(72, 23),
                Sound = SoundList.ButtonA,
            };
            ItemButton.Click += Button_Click;

            ItemButton2 = new MirButton
            {
                Index = 738,
                Library = Libraries.Title,
                Location = new Point(5000, 5000),
                Parent = this,
                Size = new Size(72, 23),
                Sound = SoundList.ButtonA,
            };
            ItemButton2.Click += Button_Click;

            QuestButton = new MirButton
            {
                Index = 739,
                Library = Libraries.Title,
                Location = new Point(5000, 5000),
                Parent = this,
                Size = new Size(72, 23),
                Sound = SoundList.ButtonA,
            };
            QuestButton.Click += Button_Click;

            AddButton = new MirButton
            {
                Index = 483,
                HoverIndex = 484,
                PressedIndex = 485,
                Library = Libraries.Title,
                Location = new Point(235, 5),
                Parent = this,
                Size = new Size(72, 23),
                Sound = SoundList.ButtonA,
                Visible = false,
            };
            AddButton.Click += (o1, e) =>
            {
                int openLevel = (GameScene.User.Inventory.Length - 48) / 4;
                int openGold = (1000000 + openLevel * 1000000);
                MirMessageBox messageBox = new MirMessageBox(string.Format("Are you sure you would like to unlock 4 extra slots for {0:###,###} gold ?\n" +
                                                    "This will take your inventory space up to {1} slots in total.", openGold, GameScene.User.Inventory.Length + 4), MirMessageBoxButtons.OKCancel);

                messageBox.OKButton.Click += (o, a) =>
                {
                    Network.Enqueue(new C.Chat { Message = "Could not increase Inventory size. Feature is in development and will be implemented soon." });
                };
                messageBox.Show();
            };

            CloseButton = new MirButton
            {
                HoverIndex = 186,
                Index = 185,
                Location = new Point(288, 25),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 187,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            GoldLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(63, 254),
                Size = new Size(114, 14),
                Sound = SoundList.Gold,
            };
            GoldLabel.Click += (o, e) =>
            {
                if (GameScene.SelectedCell == null)
                    GameScene.PickedUpGold = !GameScene.PickedUpGold && GameScene.Gold > 0;
            };

            GoldButton = new MirButton
            {
                HoverIndex = 184,
                Index = 184,
                Location = new Point(20, 246),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 184,
                Sound = SoundList.Gold,
            };
            GoldButton.Click += (o, e) =>
            {
                if (GameScene.SelectedCell == null)
                    GameScene.PickedUpGold = !GameScene.PickedUpGold && GameScene.Gold > 0;
            };


            Grid = new MirItemCell[8 * 10];

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    int idx = 8 * y + x;
                    Grid[idx] = new MirItemCell
                    {
                        ItemSlot = 8 + idx,
                        GridType = MirGridType.Inventory,
                        Library = Libraries.Items,
                        Parent = this,
                        Location = new Point(x * 36 + 22 + x, y % 5 * 36 + 56 + y % 5),
                    };

                    if (idx >= 40)
                        Grid[idx].Visible = false;
                }
            }

            QuestGrid = new MirItemCell[8 * 5];

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    QuestGrid[8 * y + x] = new MirItemCell
                    {
                        ItemSlot = 8 * y + x,
                        GridType = MirGridType.QuestInventory,
                        Library = Libraries.Items,
                        Parent = this,
                        Location = new Point(x * 36 + 9 + x, y * 32 + 37 + y),
                        Visible = false
                    };
                }
            }

            for (int i = 0; i < LockBar.Length; i++)
            {
                LockBar[i] = new MirImageControl
                {
                    Index = 307,
                    Library = Libraries.Prguse2,
                    Location = new Point(9 + i % 2 * 148, 37 + i / 2 * 33),
                    Parent = this,
                    DrawImage = true,
                    NotControl = true,
                    Visible = false,
                };
            }

        }

        void Button_Click(object sender, EventArgs e)
        {
            if (GameScene.User.Inventory.Length == 48 && sender == ItemButton2)
            {
                MirMessageBox messageBox = new MirMessageBox("Are you sure you would like to buy 8 extra slots for 1,000,000 gold?\n" +
                    "Next purchase you can unlock 4 extra slots up to a maximum of 40 slots.", MirMessageBoxButtons.OKCancel);

                messageBox.OKButton.Click += (o, a) =>
                {
                    Network.Enqueue(new C.Chat { Message = "@ADDINVENTORY" });
                };
                messageBox.Show();
            }
            else
            {
                if (sender == ItemButton)
                {
                    RefreshInventory();
                }
                else if (sender == ItemButton2)
                {
                    RefreshInventory2();
                }
                else if (sender == QuestButton)
                {
                    Reset();

                    ItemButton.Index = 737;
                    ItemButton2.Index = 738;
                    QuestButton.Index = 198;

                    if (GameScene.User.Inventory.Length == 48)
                    {
                        ItemButton2.Index = 169;
                    }

                    foreach (var grid in QuestGrid)
                    {
                        grid.Visible = true;
                    }
                }
            }
        }

        void Reset()
        {
            foreach (MirItemCell grid in QuestGrid)
            {
                grid.Visible = false;
            }

            foreach (MirItemCell grid in Grid)
            {
                grid.Visible = false;
            }

            for (int i = 0; i < LockBar.Length; i++)
            {
                LockBar[i].Visible = false;
            }

            AddButton.Visible = false;
        }



        public void RefreshInventory()
        {
            Reset();

            ItemButton.Index = 197;
            ItemButton2.Index = 738;
            QuestButton.Index = 739;

            if (GameScene.User.Inventory.Length == 48)
            {
                ItemButton2.Index = 169;
            }

            foreach (var grid in Grid)
            {
                if (grid.ItemSlot < 48)
                    grid.Visible = true;
                else
                    grid.Visible = false;
            }
        }

        public void RefreshInventory2()
        {
            Reset();

            ItemButton.Index = 737;
            ItemButton2.Index = 168;
            QuestButton.Index = 739;

            foreach (var grid in Grid)
            {
                if (grid.ItemSlot < 48 || grid.ItemSlot >= GameScene.User.Inventory.Length)
                    grid.Visible = false;
                else
                    grid.Visible = true;
            }

            int openLevel = (GameScene.User.Inventory.Length - 48) / 4;
            for (int i = 0; i < LockBar.Length; i++)
            {
                LockBar[i].Visible = (i < openLevel) ? false : true;
            }

            AddButton.Visible = openLevel >= 10 ? false : true;
        }

        public void Process()
        {
            if (GameScene.User == null) return;
            //WeightLabel.Text = (MapObject.User.MaxBagWeight - MapObject.User.CurrentBagWeight).ToString();
            GoldLabel.Text = GameScene.Gold.ToString("###,###,##0");
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;

            RefreshInventory();
        }




        public MirItemCell GetCell(ulong id)
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item == null || Grid[i].Item.UniqueID != id) continue;
                return Grid[i];
            }
            return null;
        }

        public MirItemCell GetQuestCell(ulong id)
        {
            return QuestGrid.FirstOrDefault(t => t.Item != null && t.Item.UniqueID == id);
        }

        public void DisplayItemGridEffect(ulong id, int type = 0)
        {
            MirItemCell cell = GetCell(id);

            if (cell.Item == null) return;

            MirAnimatedControl animEffect = null;

            switch (type)
            {
                case 0:
                    animEffect = new MirAnimatedControl
                    {
                        Animated = true,
                        AnimationCount = 9,
                        AnimationDelay = 150,
                        Index = 410,
                        Library = Libraries.Prguse,
                        Location = cell.Location,
                        Parent = this,
                        Loop = false,
                        NotControl = true,
                        UseOffSet = true,
                        Blending = true,
                        BlendingRate = 1F
                    };
                    animEffect.AfterAnimation += (o, e) => animEffect.Dispose();
                    SoundManager.PlaySound(20000 + (ushort)Spell.MagicShield * 10);
                    break;
            }

        }
    }
    public sealed class BeltDialog : MirImageControl
    {
        public MirLabel[] Key = new MirLabel[8];
        public MirItemCell[] Grid;

        public BeltDialog()
        {
            Index = 32;
            Library = Libraries.GameScene;
            Movable = false;
            Sort = false;
            Visible = true;
            Location = new Point(((Settings.ScreenWidth / 2) - (Size.Width / 2)) + 5, Settings.ScreenHeight - 202);

            BeforeDraw += BeltPanel_BeforeDraw;

            for (int i = 0; i < Key.Length; i++)
            {
                Key[i] = new MirLabel
                {
                    Parent = this,
                    Size = new Size(26, 14),
                    Location = new Point(18 + i * 41, 8),
                    Text = (i + 1).ToString()
                };
            }

            Grid = new MirItemCell[8];

            for (int x = 0; x < 8; x++)
            {
                Grid[x] = new MirItemCell
                {
                    ItemSlot = x,
                    Size = new Size(32, 32),
                    GridType = MirGridType.Inventory,
                    Library = Libraries.Items,
                    Parent = this,
                    Location = new Point(x * 41 + 19, 9),
                };
            }

        }

        private void BeltPanel_BeforeDraw(object sender, EventArgs e)
        {
            //if Transparent return

            //if (Libraries.Prguse != null)
            //    Libraries.Prguse.Draw(Index + 1, DisplayLocation, Color.White, false, 0.5F);
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
        }

        public void Flip()
        {
            //0,70 LOCATION
            if (Index == 1932)
            {
                Index = 1944;
                Location = new Point(0, 200);

                for (int x = 0; x < 6; x++)
                    Grid[x].Location = new Point(3, x * 35 + 12);



            }
            else
            {
                Index = 1932;
                Location = new Point(GameScene.Scene.MainDialog.Location.X + 230, Settings.ScreenHeight - 150);

                for (int x = 0; x < 6; x++)
                    Grid[x].Location = new Point(x * 35 + 12, 3);


            }

            for (int i = 0; i < Key.Length; i++)
            {
                Key[i].Location = (Index != 1932) ? new Point(-1, 11 + i * 35) : new Point(8 + i * 35, 2);
            }
        }


        public MirItemCell GetCell(ulong id)
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item == null || Grid[i].Item.UniqueID != id) continue;
                return Grid[i];
            }
            return null;
        }
    }
    public sealed class SkillBarDialog : MirImageControl
    {
        private readonly MirButton _switchBindsButton;

        public bool AltBind;
        public bool HasSkill = false;
        public byte BarIndex;

        //public bool TopBind = !Settings.SkillMode;
        public MirImageControl[] Cells = new MirImageControl[8];
        public MirLabel[] KeyNameLabels = new MirLabel[8];
        public MirLabel BindNumberLabel = new MirLabel();

        public MirAnimatedControl[] CoolDowns = new MirAnimatedControl[8];

        public SkillBarDialog()
        {
            Index = 2190;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = new Point(0, BarIndex * 20);
            Visible = true;

            BeforeDraw += MagicKeyDialog_BeforeDraw;

            _switchBindsButton = new MirButton
            {
                Index = 2247,
                Library = Libraries.Prguse,
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(16, 28),
                Location = new Point(0, 0)
            };
            _switchBindsButton.Click += (o, e) =>
            {
                //Settings.SkillSet = !Settings.SkillSet;

                Update();
            };

            for (var i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new MirImageControl
                {
                    Index = -1,
                    Library = Libraries.MagIcon,
                    Parent = this,
                    Location = new Point(i * 25 + 15, 3),
                };
                int j = i + 1;
                Cells[i].Click += (o, e) =>
                {
                    GameScene.Scene.UseSpell(j + (8 * BarIndex));
                };

                CoolDowns[i] = new MirAnimatedControl
                {
                    Library = Libraries.Prguse2,
                    Parent = this,
                    Location = new Point(i * 25 + 15, 3),
                    NotControl = true,
                    UseOffSet = true,
                    Loop = false,
                    Animated = false,
                    Opacity = 0.6F
                };
            }

            BindNumberLabel = new MirLabel
            {
                Text = "1",
                Font = new Font(Settings.FontName, 8F),
                ForeColour = Color.White,
                Parent = this,
                Location = new Point(0, 1),
                Size = new Size(10, 25),
                NotControl = true
            };

            for (var i = 0; i < KeyNameLabels.Length; i++)
            {
                KeyNameLabels[i] = new MirLabel
                {
                    Text = "F" + (i + 1),
                    Font = new Font(Settings.FontName, 8F),
                    ForeColour = Color.White,
                    Parent = this,
                    Location = new Point(i * 25 + 13, 0),
                    Size = new Size(25, 25),
                    NotControl = true
                };
            }
            OnMoving += SkillBar_OnMoving;
        }

        private void SkillBar_OnMoving(object sender, MouseEventArgs e)
        {
            if (BarIndex * 2 >= Settings.SkillbarLocation.Length) return;
            Settings.SkillbarLocation[BarIndex, 0] = this.Location.X;
            Settings.SkillbarLocation[BarIndex, 1] = this.Location.Y;
        }

        private string GetKey(int barindex, int i)
        {
            //KeybindOptions Type = KeybindOptions.Bar1Skill1;
            if ((barindex == 0) && (i == 1))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill1);
            if ((barindex == 0) && (i == 2))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill2);
            if ((barindex == 0) && (i == 3))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill3);
            if ((barindex == 0) && (i == 4))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill4);
            if ((barindex == 0) && (i == 5))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill5);
            if ((barindex == 0) && (i == 6))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill6);
            if ((barindex == 0) && (i == 7))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill7);
            if ((barindex == 0) && (i == 8))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill8);
            if ((barindex == 1) && (i == 1))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill1);
            if ((barindex == 1) && (i == 2))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill2);
            if ((barindex == 1) && (i == 3))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill3);
            if ((barindex == 1) && (i == 4))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill4);
            if ((barindex == 1) && (i == 5))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill5);
            if ((barindex == 1) && (i == 6))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill6);
            if ((barindex == 1) && (i == 7))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill7);
            if ((barindex == 1) && (i == 8))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill8);
            return "";
        }


        void MagicKeyDialog_BeforeDraw(object sender, EventArgs e)
        {
            Libraries.Prguse.Draw(2193, new Point(DisplayLocation.X + 12, DisplayLocation.Y), Color.White, true, 0.5F);
        }

        public void Update()
        {
            HasSkill = false;
            foreach (var m in GameScene.User.Magics)
            {
                if ((m.Key < (BarIndex * 8) + 1) || (m.Key > ((BarIndex + 1) * 8) + 1)) continue;
                HasSkill = true;
            }
            if (!Visible) return;
            Index = 2190;
            _switchBindsButton.Index = 2247;
            BindNumberLabel.Text = (BarIndex + 1).ToString();
            BindNumberLabel.Location = new Point(0, 1);

            for (var i = 1; i <= 8; i++)
            {
                Cells[i - 1].Index = -1;

                int offset = BarIndex * 8;
                string key = GetKey(BarIndex, i);
                KeyNameLabels[i - 1].Text = key;

                foreach (var m in GameScene.User.Magics)
                {
                    if (m.Key != i + offset) continue;
                    HasSkill = true;
                    ClientMagic magic = MapObject.User.GetMagic(m.Spell);
                    if (magic == null) continue;

                    //string key = m.Key > 8 ? string.Format("CTRL F{0}", i) : string.Format("F{0}", m.Key);

                    Cells[i - 1].Index = magic.Icon*2;
                    Cells[i - 1].Hint = string.Format("{0}\nMP: {1}\nCooldown: {2}\nKey: {3}", magic.Name,
                        (magic.BaseCost + (magic.LevelCost * magic.Level)), Functions.PrintTimeSpanFromMilliSeconds(magic.Delay), key);

                    KeyNameLabels[i - 1].Text = "";
                }

                CoolDowns[i - 1].Dispose();
            }
        }


        public void Process()
        {
            ProcessSkillDelay();
        }

        private void ProcessSkillDelay()
        {
            if (GameScene.User == null) return;

            if (!Visible) return;

            int offset = BarIndex * 8;

            for (int i = 0; i < Cells.Length; i++)
            {
                foreach (var magic in GameScene.User.Magics)
                {
                    if (magic.Key != i + offset + 1) continue;

                    int totalFrames = 22;
                    long timeLeft = magic.CastTime + magic.Delay - CMain.Time;

                    if (timeLeft < 100 || (CoolDowns[i] != null && CoolDowns[i].Animated))
                    {
                        if (timeLeft > 0)
                            CoolDowns[i].Dispose();
                        else
                            continue;
                    }

                    int delayPerFrame = (int)(magic.Delay / totalFrames);
                    int startFrame = totalFrames - (int)(timeLeft / delayPerFrame);

                    if ((CMain.Time <= magic.CastTime + magic.Delay) && magic.CastTime > 0)
                    {
                        CoolDowns[i].Dispose();

                        CoolDowns[i] = new MirAnimatedControl
                        {
                            Index = 1260 + startFrame,
                            AnimationCount = (totalFrames - startFrame),
                            AnimationDelay = delayPerFrame,
                            Library = Libraries.Prguse2,
                            Parent = this,
                            Location = new Point(i * 25 + 15, 3),
                            NotControl = true,
                            UseOffSet = true,
                            Loop = false,
                            Animated = true,
                            Opacity = 0.6F
                        };
                    }
                }
            }
        }

        public void Show()
        {
            if (Visible) return;
            if (!HasSkill) return;
            Settings.SkillBar = true;
            Visible = true;
            Update();
        }

        public void Hide()
        {
            if (!Visible) return;
            Settings.SkillBar = false;
            Visible = false;
        }
    }
    public sealed class CharacterDialog : MirImageControl
    {
        public MirButton CloseButton, CharacterButton, StatusButton, StateButton, SkillButton;
        public MirImageControl CharacterPage, StatusPage, StatePage, SkillPage, ClassImage;
        public MirAnimatedControl ArmourEffect, WeaponEffect;

        public MirLabel NameLabel, LoverLabel;

        public MirLabel ACLabel, MACLabel, DCLabel, MCLabel, SCLabel, HealthLabel, ManaLabel;
        public MirLabel ACLabelLabel, MACLabelLabel, DCLabelLabel, MCLabelLabel, SCLabelLabel, HealthLabelLabel, ManaLabelLabel;

        public MirLabel CritRLabel, CritDLabel, LuckLabel, AttkSpdLabel, AccLabel, AgilLabel;
        public MirLabel CritRLabelLabel, CritDLabelLabel, LuckLabelLabel, AttkSpdLabelLabel, AccLabelLabel, AgilLabelLabel;

        public MirLabel ExpPLabel, BagWLabel, WearWLabel, HandWLabel, MagicRLabel, PoisonRecLabel, HealthRLabel, ManaRLabel, PoisonResLabel, HolyTLabel, FreezeLabel, PoisonAtkLabel;
        public MirLabel ExpPLabelLabel, BagWLabelLabel, WearWLabelLabel, HandWLabelLabel, MagicRLabelLabel, PoisonRecLabelLabel, HealthRLabelLabel, ManaRLabelLabel, PoisonResLabelLabel, HolyTLabelLabel, FreezeLabelLabel, PoisonAtkLabelLabel;

        public MirLabel HeadingLabel, StatusLabel, StateLabel, MagicLabel, NextLabel, BackLabel;
        public MirButton NextButton, BackButton;

        public MirItemCell[] Grid;
        public MagicButton[] Magics;

        public int StartIndex;

        public CharacterDialog()
        {
            Index = 220;
            Library = Libraries.GameScene;
            Location = new Point(Settings.ScreenWidth - 352, 0);
            Movable = true;
            Sort = true;

            BeforeDraw += (o, e) => RefreshInterface();


            CharacterPage = new MirImageControl
            {
                Index = 221,
                Parent = this,
                Library = Libraries.GameScene,
                Location = new Point(47, 104),
            };
            CharacterPage.AfterDraw += (o, e) =>
            {
                if (Libraries.StateItems == null) return;
                if (Libraries.StateEffect == null) return;
                ItemInfo RealItem = null;

                #region Armour
                if (Grid[(int)EquipmentSlot.Armour].Item != null)
                {
                    RealItem = Functions.GetRealItem(Grid[(int)EquipmentSlot.Armour].Item.Info, MapObject.User.Level, MapObject.User.Class, GameScene.ItemInfoList);
                    Libraries.StateItems.Draw(RealItem.Image, new Point(DisplayLocation.X + 58, DisplayLocation.Y + 23), Color.White, true, 1F);

                    
                    int genderOffset = MapObject.User.Gender == MirGender.Male ? 0 : 27;

                    if (Settings.Effect == true)
                    {
                        switch (GameScene.User.WingEffect)
                        {
                            case 36:
                                ArmourEffect.Animated = true;
                                ArmourEffect.Index = 2405 + genderOffset;
                                ArmourEffect.AnimationCount = 14;
                                ArmourEffect.Loop = true;
                                ArmourEffect.Visible = true;
                                break;
                            case 0:
                                ArmourEffect.Animated = false;
                                ArmourEffect.Visible = false;
                                break;
                            default:
                                ArmourEffect.Animated = false;
                                ArmourEffect.Visible = false;
                                break;
                        }
                    }
                }
                #endregion

                #region Weapon
                if (Grid[(int)EquipmentSlot.Weapon].Item != null)
                {
                    RealItem = Functions.GetRealItem(Grid[(int)EquipmentSlot.Weapon].Item.Info, MapObject.User.Level, MapObject.User.Class, GameScene.ItemInfoList);
                    Libraries.StateItems.Draw(RealItem.Image, new Point(DisplayLocation.X + 58, DisplayLocation.Y + 23), Color.White, true, 1F);

                    if (Settings.Effect == true)
                    {
                        switch (RealItem.Shape)
                        {
                            case 136: //Test
                                WeaponEffect.Animated = true;
                                WeaponEffect.Index = 2268;
                                WeaponEffect.AnimationCount = 14;
                                WeaponEffect.Loop = true;
                                WeaponEffect.Visible = true;
                                break;
                            case 0:
                                WeaponEffect.Animated = false;
                                WeaponEffect.Visible = false;
                                break;
                            default:
                                WeaponEffect.Animated = false;
                                WeaponEffect.Visible = false;
                                break;
                        }
                    }
                }
                #endregion

                #region Shield
                if (Grid[(int)EquipmentSlot.Shield].Item != null)
                {
                    RealItem = Functions.GetRealItem(Grid[(int)EquipmentSlot.Shield].Item.Info, MapObject.User.Level, MapObject.User.Class, GameScene.ItemInfoList);
                    Libraries.StateItems.Draw(RealItem.Image, new Point(DisplayLocation.X + 179, DisplayLocation.Y + 229), Color.White, true, 1F);
                }
                #endregion

                #region Hair + Helmet
                if (Grid[(int)EquipmentSlot.Helmet].Item != null)
                    Libraries.StateItems.Draw(Grid[(int)EquipmentSlot.Helmet].Item.Info.Image, new Point(DisplayLocation.X + 58, DisplayLocation.Y + 23), Color.White, true, 1F);
                else
                {
                    int hair = 441 + MapObject.User.Hair + (MapObject.User.Class == MirClass.Assassin ? 20 : 0) + (MapObject.User.Gender == MirGender.Male ? 0 : 40);

                    int offSetX = MapObject.User.Class == MirClass.Assassin ? (MapObject.User.Gender == MirGender.Male ? 6 : 4) : 0;
                    int offSetY = MapObject.User.Class == MirClass.Assassin ? (MapObject.User.Gender == MirGender.Male ? 25 : 18) : 0;

                    Libraries.Prguse.Draw(hair, new Point(DisplayLocation.X + 58 + offSetX, DisplayLocation.Y + 26 + offSetY), Color.White, true, 1F);
                }
                #endregion
            };

            #region Effects

            ArmourEffect = new MirAnimatedControl
            {
                Animated = false,
                AnimationCount = 11,
                AnimationDelay = 100,
                Index = 15,
                Library = Libraries.StateEffect,
                Location = new Point(190, 256),
                Loop = false,
                Parent = this,
                Blending = true,
                BlendingRate = 1,
                UseOffSet = true,
                NotControl = true,
                Visible = false,
                //Sort = true,
            };

            WeaponEffect = new MirAnimatedControl
            {
                Animated = false,
                AnimationCount = 11,
                AnimationDelay = 100,
                Index = 15,
                Library = Libraries.StateEffect,
                Location = new Point(187, 221),
                Loop = false,
                Parent = this,
                Blending = true,
                BlendingRate = 1,
                UseOffSet = true,
                NotControl = true,
                Visible = false,
            };

            #endregion

            StatusPage = new MirImageControl
            {
                Index = 224,
                Parent = this,
                Library = Libraries.GameScene,
                Location = new Point(47, 104),
                Visible = false,
            };
            StatusPage.BeforeDraw += (o, e) =>
            {
                ACLabel.Text = string.Format("{0}-{1}", MapObject.User.MinAC, MapObject.User.MaxAC);
                MACLabel.Text = string.Format("{0}-{1}", MapObject.User.MinMAC, MapObject.User.MaxMAC);
                DCLabel.Text = string.Format("{0}-{1}", MapObject.User.MinDC, MapObject.User.MaxDC);
                MCLabel.Text = string.Format("{0}-{1}", MapObject.User.MinMC, MapObject.User.MaxMC);
                SCLabel.Text = string.Format("{0}-{1}", MapObject.User.MinSC, MapObject.User.MaxSC);
                HealthLabel.Text = string.Format("{0}/{1}", MapObject.User.HP, MapObject.User.MaxHP);
                ManaLabel.Text = string.Format("{0}/{1}", MapObject.User.MP, MapObject.User.MaxMP);
                CritRLabel.Text = string.Format("{0}%", MapObject.User.CriticalRate);
                CritDLabel.Text = string.Format("{0}", MapObject.User.CriticalDamage);
                AttkSpdLabel.Text = string.Format("{0}", MapObject.User.ASpeed);
                AccLabel.Text = string.Format("+{0}", MapObject.User.Accuracy);
                AgilLabel.Text = string.Format("+{0}", MapObject.User.Agility);
                LuckLabel.Text = string.Format("+{0}", MapObject.User.Luck);
            };

            StatePage = new MirImageControl
            {
                Index = 225,
                Parent = this,
                Library = Libraries.GameScene,
                Location = new Point(47, 104),
                Visible = false
            };
            StatePage.BeforeDraw += (o, e) =>
            {
                ExpPLabel.Text = string.Format("{0:0.##%}", MapObject.User.Experience / (double)MapObject.User.MaxExperience);
                BagWLabel.Text = string.Format("{0}/{1}", MapObject.User.CurrentBagWeight, MapObject.User.MaxBagWeight);
                WearWLabel.Text = string.Format("{0}/{1}", MapObject.User.CurrentWearWeight, MapObject.User.MaxWearWeight);
                HandWLabel.Text = string.Format("{0}/{1}", MapObject.User.CurrentHandWeight, MapObject.User.MaxHandWeight);
                MagicRLabel.Text = string.Format("+{0}", MapObject.User.MagicResist);
                PoisonResLabel.Text = string.Format("+{0}", MapObject.User.PoisonResist);
                HealthRLabel.Text = string.Format("+{0}", MapObject.User.HealthRecovery);
                ManaRLabel.Text = string.Format("+{0}", MapObject.User.SpellRecovery);
                PoisonRecLabel.Text = string.Format("+{0}", MapObject.User.PoisonRecovery);
                HolyTLabel.Text = string.Format("+{0}", MapObject.User.Holy);
                FreezeLabel.Text = string.Format("+{0}", MapObject.User.Freezing);
                PoisonAtkLabel.Text = string.Format("+{0}", MapObject.User.PoisonAttack);
            };


            SkillPage = new MirImageControl
            {
                Index = 226,
                Parent = this,
                Library = Libraries.GameScene,
                Location = new Point(47, 104),
                Visible = false
            };


            CharacterButton = new MirButton
            {
                Index = 227,
                Library = Libraries.GameScene,
                Location = new Point(32, 72),
                Parent = this,
                PressedIndex = 227,
                Size = new Size(82, 21),
                Sound = SoundList.ButtonA,
            };
            CharacterButton.Click += (o, e) => ShowCharacterPage();
            StatusButton = new MirButton
            {
                Library = Libraries.GameScene,
                Location = new Point(105, 72),
                Parent = this,
                PressedIndex = 227,
                Size = new Size(82, 21),
                Sound = SoundList.ButtonA
            };
            StatusButton.Click += (o, e) => ShowStatusPage();

            StateButton = new MirButton
            {
                Library = Libraries.GameScene,
                Location = new Point(178, 72),
                Parent = this,
                PressedIndex = 227,
                Size = new Size(82, 21),
                Sound = SoundList.ButtonA
            };
            StateButton.Click += (o, e) => ShowStatePage();

            SkillButton = new MirButton
            {
                Library = Libraries.GameScene,
                Location = new Point(251, 72),
                Parent = this,
                PressedIndex = 227,
                Size = new Size(82, 21),
                Sound = SoundList.ButtonA
            };
            SkillButton.Click += (o, e) => ShowSkillPage();

            CloseButton = new MirButton
            {
                HoverIndex = 186,
                Index = 185,
                Location = new Point(306, 43),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 187,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            NameLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(17, 41),
                Size = new Size(318, 26),
                NotControl = true,
            };
            ClassImage = new MirImageControl
            {
                Index = 100,
                Library = Libraries.Prguse,
                Location = new Point(17, 38),
                Parent = this,
                NotControl = true,
            };

            Grid = new MirItemCell[Enum.GetNames(typeof(EquipmentSlot)).Length];

            Grid[(int)EquipmentSlot.Weapon] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Weapon,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(8, 45),
            };


            Grid[(int)EquipmentSlot.Armour] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Armour,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(8, 90),
            };


            Grid[(int)EquipmentSlot.Helmet] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Helmet,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(248, 45),
            };



            Grid[(int)EquipmentSlot.Necklace] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Necklace,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(248, 90),
            };



            Grid[(int)EquipmentSlot.Torch] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Torch,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(248, 135),
            };



            Grid[(int)EquipmentSlot.BraceletL] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.BraceletL,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(8, 180),
            };

            Grid[(int)EquipmentSlot.BraceletR] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.BraceletR,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(248, 180),
            };

            Grid[(int)EquipmentSlot.RingL] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RingL,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(8, 225),
            };

            Grid[(int)EquipmentSlot.RingR] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RingR,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(248, 225),
            };


            Grid[(int)EquipmentSlot.AmuletL] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.AmuletL,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(8, 270),
            };

            Grid[(int)EquipmentSlot.AmuletR] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.AmuletR,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(56, 270),
            };

            Grid[(int)EquipmentSlot.Boots] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Boots,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(104, 270),
            };

            Grid[(int)EquipmentSlot.Belt] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Belt,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(152, 270),
            };


            Grid[(int)EquipmentSlot.Stone] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Stone,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(200, 270),
            };

            Grid[(int)EquipmentSlot.Shield] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Shield,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(248, 270),
            };

            Grid[(int)EquipmentSlot.Mount] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Mount,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(56, 315),
            };

            Grid[(int)EquipmentSlot.Charm] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Charm,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(104, 315),
            };

            Grid[(int)EquipmentSlot.Medal] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Medal,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(152, 315),
            };

            Grid[(int)EquipmentSlot.RewardStone] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RewardStone,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(200, 315),
            };

            Grid[(int)EquipmentSlot.ShoulderPad] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.ShoulderPad,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(8, 135),
            };

            // STATS I
            HealthLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 39),
                NotControl = true,
                Text = "0-0",
            };

            HealthLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = HealthLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Health Points :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            ManaLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 60),
                NotControl = true,
                Text = "0-0",
            };

            ManaLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = ManaLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Mana Points :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            ACLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 81),
                NotControl = true,
                Text = "0-0",
            };

            ACLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = ACLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "AC :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };


            MACLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 102),
                NotControl = true,
                Text = "0-0",
            };

            MACLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = MACLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "MAC :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            DCLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 123),
                NotControl = true,
                Text = "0-0"
            };

            DCLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = DCLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "DC :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            MCLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 144),
                NotControl = true,
                Text = "0-0"
            };

            MCLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = MCLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "MC :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            SCLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 165),
                NotControl = true,
                Text = "0-0"
            };

            SCLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = SCLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "SC :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            //Breezer - New Labels
            CritRLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 186),
                NotControl = true
            };

            CritRLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = CritRLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Critical Rate :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            CritDLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 207),
                NotControl = true
            };

            CritDLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = CritDLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Critical Damage :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            AttkSpdLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 228),
                NotControl = true
            };

            AttkSpdLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = AttkSpdLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Attack Speed :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            AccLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 249),
                NotControl = true
            };

            AccLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = AccLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Accuracy :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            AgilLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 270),
                NotControl = true
            };

            AgilLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = AgilLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Agility :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            LuckLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(144, 291),
                NotControl = true
            };

            LuckLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = LuckLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Luck :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            // STATS II 
            ExpPLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(144, 39),
                NotControl = true,
                Text = "0-0",
            };

            ExpPLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = ExpPLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Experience :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            BagWLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(144, 60),
                NotControl = true,
                Text = "0-0",
            };

            BagWLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = BagWLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Bag Weight :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            WearWLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(144, 81),
                NotControl = true,
                Text = "0-0",
            };

            WearWLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = WearWLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Wear Weight :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            HandWLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(144, 102),
                NotControl = true,
                Text = "0-0",
            };

            HandWLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = HandWLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Hand Weight :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            MagicRLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(144, 123),
                NotControl = true,
                Text = "0-0"
            };

            MagicRLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = MagicRLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Magic Resistance :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            PoisonResLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(144, 144),
                NotControl = true,
                Text = "0/0"
            };

            PoisonResLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = PoisonResLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Poison Resistance :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            HealthRLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(144, 165),
                NotControl = true,
                Text = "0/0"
            };

            HealthRLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = HealthRLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Health Recovery :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            //Breezer
            ManaRLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(144, 186),
                NotControl = true
            };

            ManaRLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = ManaRLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Mana Recovery :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            PoisonRecLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(144, 207),
                NotControl = true
            };

            PoisonRecLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = PoisonRecLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Poison Recovery :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            HolyTLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(144, 228),
                NotControl = true
            };

            HolyTLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = HolyTLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Holy Power :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            FreezeLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(144, 249),
                NotControl = true
            };

            FreezeLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = FreezeLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Freeze Attack :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            PoisonAtkLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(144, 270),
                NotControl = true
            };

            PoisonAtkLabelLabel = new MirLabel
            {
                Size = new Size(100, 14),
                Parent = PoisonAtkLabel,
                Location = new Point(-100, 0),
                NotControl = true,
                Text = "Poison Attack :",
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter
            };

            Magics = new MagicButton[6];

            for (int i = 0; i < Magics.Length; i++)
                Magics[i] = new MagicButton { Parent = SkillPage, Visible = false, Location = new Point(20, 39 + i * 44) };

            NextButton = new MirButton
            {
                Index = 228,
                Location = new Point(148, 318),
                Library = Libraries.GameScene,
                Parent = SkillPage,
                PressedIndex = 230,
                HoverIndex = 229,
                Sound = SoundList.ButtonA,
            };
            NextButton.Click += (o, e) =>
            {
                if (StartIndex + 6 >= MapObject.User.Magics.Count) return;

                StartIndex += 6;
                RefreshInterface();

                ClearCoolDowns();
            };

            NextLabel = new MirLabel
            {
                Size = new Size(78, 20),
                Parent = NextButton,
                Location = new Point(0, -2),
                NotControl = true,
                Text = "Next",
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };

            BackButton = new MirButton
            {
                Index = 228,
                Location = new Point(62, 318),
                Library = Libraries.GameScene,
                Parent = SkillPage,
                PressedIndex = 230,
                HoverIndex = 229,
                Sound = SoundList.ButtonA,
            };
            BackButton.Click += (o, e) =>
            {
                if (StartIndex - 6 < 0) return;

                StartIndex -= 6;
                RefreshInterface();

                ClearCoolDowns();
            };

            BackLabel = new MirLabel
            {
                Size = new Size(78, 20),
                Parent = BackButton,
                Location = new Point(0, -2),
                NotControl = true,
                Text = "Back",
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
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

            ClearCoolDowns();
        }

        public void ShowCharacterPage()
        {
            CharacterPage.Visible = true;
            StatusPage.Visible = false;
            StatePage.Visible = false;
            SkillPage.Visible = false;
            CharacterButton.Index = 227;
            StatusButton.Index = -1;
            StateButton.Index = -1;
            SkillButton.Index = -1;
        }

        private void ShowStatusPage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = true;
            StatePage.Visible = false;
            SkillPage.Visible = false;
            CharacterButton.Index = -1;
            StatusButton.Index = 227;
            StateButton.Index = -1;
            SkillButton.Index = -1;
            ArmourEffect.Visible = false;
            WeaponEffect.Visible = false;
        }

        private void ShowStatePage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = false;
            StatePage.Visible = true;
            SkillPage.Visible = false;
            CharacterButton.Index = -1;
            StatusButton.Index = -1;
            StateButton.Index = 227;
            SkillButton.Index = -1;
            ArmourEffect.Visible = false;
            WeaponEffect.Visible = false;
        }

        public void ShowSkillPage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = false;
            StatePage.Visible = false;
            SkillPage.Visible = true;
            CharacterButton.Index = -1;
            StatusButton.Index = -1;
            StateButton.Index = -1;
            SkillButton.Index = 227;
            StartIndex = 0;
            ArmourEffect.Visible = false;
            WeaponEffect.Visible = false;

            ClearCoolDowns();
        }

        private void ClearCoolDowns()
        {
            for (int i = 0; i < Magics.Length; i++)
            {
                Magics[i].CoolDown.Dispose();
            }
        }

        private void RefreshInterface()
        {
            int offSet = MapObject.User.Gender == MirGender.Male ? 0 : 1;

            Index = 220;// +offSet;
            CharacterPage.Index = 222 + offSet;

            switch (MapObject.User.Class)
            {
                case MirClass.Warrior:
                    ClassImage.Index = 100;// + offSet * 5;
                    break;
                case MirClass.Wizard:
                    ClassImage.Index = 101;// + offSet * 5;
                    break;
                case MirClass.Taoist:
                    ClassImage.Index = 102;// + offSet * 5;
                    break;
                case MirClass.Assassin:
                    ClassImage.Index = 103;// + offSet * 5;
                    break;
                case MirClass.Archer:
                    ClassImage.Index = 104;// + offSet * 5;
                    break;
            }

            NameLabel.Text = MapObject.User.Name;

            for (int i = 0; i < Magics.Length; i++)
            {
                if (i + StartIndex >= MapObject.User.Magics.Count)
                {
                    Magics[i].Visible = false;
                    continue;
                }

                Magics[i].Visible = true;
                Magics[i].Update(MapObject.User.Magics[i + StartIndex]);
            }
        }

        public MirItemCell GetCell(ulong id)
        {

            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item == null || Grid[i].Item.UniqueID != id) continue;
                return Grid[i];
            }
            return null;
        }

    }
    public sealed class MiniMapDialog : MirImageControl
    {
        public MirButton ToggleButton;
        public MirLabel LocationLabel, MapNameLabel;
        private float _fade = 1F;
        private bool _bigMode = true, _realBigMode = true;

        public MirLabel AModeLabel, PModeLabel;

        public List<MirLabel> QuestIcons = new List<MirLabel>();

        public MiniMapDialog()
        {
            Index = 5;
            Library = Libraries.GameScene;
            Location = new Point(Settings.ScreenWidth - 174, 0);
            PixelDetect = true;

            BeforeDraw += MiniMap_BeforeDraw;
            AfterDraw += MiniMapDialog_AfterDraw;

            MapNameLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                Size = new Size(137, 18),
                Location = new Point(8, 5),
                NotControl = true,
            };

            LocationLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                Size = new Size(57, 14),
                Location = new Point(78, 156),
                NotControl = true,
            };

            ToggleButton = new MirButton
            {
                Index = 29,
                HoverIndex = 30,
                PressedIndex = 31,
                Parent = this,
                Location = new Point(147, 3),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "MiniMap (" + CMain.InputKeys.GetKey(KeybindOptions.Minimap) + ")"
            };
            ToggleButton.Click += (o, e) => Toggle();


            AModeLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.Yellow,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(115, 155)
            };

            PModeLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.Yellow,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(230, 155),
                Visible = false
            };
        }

        private void MiniMapDialog_AfterDraw(object sender, EventArgs e)
        {

        }

        private void MiniMap_BeforeDraw(object sender, EventArgs e)
        {

            foreach (var icon in QuestIcons)
                icon.Dispose();

            QuestIcons.Clear();

            MapControl map = GameScene.Scene.MapControl;
            if (map == null) return;

            if (map.MiniMap == 0 && Index != 6)
            {
                SetSmallMode();
            }
            else if (map.MiniMap > 0 && _bigMode && Index == 6)
            {
                SetBigMode();
            }

            if (map.MiniMap <= 0 || Index != 5 || Libraries.MiniMap == null)
            {
                return;
            }

            Rectangle viewRect = new Rectangle(0, 0, 122, 122);
            Point drawLocation = Location;
            drawLocation.Offset(46, 31);

            Size miniMapSize = Libraries.MiniMap.GetSize(map.MiniMap);
            float scaleX = miniMapSize.Width / (float)map.Width;
            float scaleY = miniMapSize.Height / (float)map.Height;

            viewRect.Location = new Point(
                (int)(scaleX * MapObject.Camera.CurrentLocation.X) - viewRect.Width / 2,
                (int)(scaleY * MapObject.Camera.CurrentLocation.Y) - viewRect.Height / 2);

            //   viewRect.Location = viewRect.Location.Subtract(1, 1);
            if (viewRect.Right >= miniMapSize.Width)
                viewRect.X = miniMapSize.Width - viewRect.Width;
            if (viewRect.Bottom >= miniMapSize.Height)
                viewRect.Y = miniMapSize.Height - viewRect.Height;

            if (viewRect.X < 0) viewRect.X = 0;
            if (viewRect.Y < 0) viewRect.Y = 0;

            Libraries.MiniMap.Draw(map.MiniMap, viewRect, drawLocation, Color.FromArgb(255, 255, 255), _fade);


            int startPointX = (int)(viewRect.X / scaleX);
            int startPointY = (int)(viewRect.Y / scaleY);

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];

                if (ob.Race == ObjectType.Observer) continue;

                if (ob.Race == ObjectType.Item || ob.Dead || ob.Race == ObjectType.Spell || ob.Sneaking) continue;
                float x = ((ob.CurrentLocation.X - startPointX) * scaleX) + drawLocation.X;
                float y = ((ob.CurrentLocation.Y - startPointY) * scaleY) + drawLocation.Y;

                Color colour;

                if ((GroupDialog.GroupList.Contains(ob.Name) && MapObject.Camera != ob) || ob.Name.EndsWith(string.Format("({0})", MapObject.Camera.Name)))
                    colour = Color.FromArgb(0, 0, 255);
                else
                    if (ob is PlayerObject)
                {
                    colour = Color.FromArgb(255, 255, 255);
                }
                else if (ob is NPCObject || ob.AI == 6)
                {
                    colour = Color.FromArgb(0, 255, 50);
                }
                else
                    colour = Color.FromArgb(255, 0, 0);

                DXManager.Sprite.Draw2D(DXManager.RadarTexture, Point.Empty, 0, new PointF((int)(x - 0.5F), (int)(y - 0.5F)), colour);

                #region NPC Quest Icons

                NPCObject npc = ob as NPCObject;
                if (npc != null && npc.GetAvailableQuests(true).Any())
                {
                    string text = "";
                    Color color = Color.Empty;

                    switch (npc.QuestIcon)
                    {
                        case QuestIcon.ExclamationBlue:
                            color = Color.DodgerBlue;
                            text = "!";
                            break;
                        case QuestIcon.ExclamationYellow:
                            color = Color.Yellow;
                            text = "!";
                            break;
                        case QuestIcon.ExclamationGreen:
                            color = Color.Green;
                            text = "!";
                            break;
                        case QuestIcon.QuestionBlue:
                            color = Color.DodgerBlue;
                            text = "?";
                            break;
                        case QuestIcon.QuestionWhite:
                            color = Color.White;
                            text = "?";
                            break;
                        case QuestIcon.QuestionYellow:
                            color = Color.Yellow;
                            text = "?";
                            break;
                        case QuestIcon.QuestionGreen:
                            color = Color.Green;
                            text = "?";
                            break;
                    }

                    QuestIcons.Add(new MirLabel
                    {
                        AutoSize = true,
                        Parent = GameScene.Scene.MiniMapDialog,
                        Font = new Font(Settings.FontName, 9f, FontStyle.Bold),
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                        Text = text,
                        ForeColour = color,
                        Location = new Point((int)(x - Settings.ScreenWidth + GameScene.Scene.MiniMapDialog.Size.Width) - 6, (int)(y) - 10),
                        NotControl = true,
                        Visible = true,
                        Modal = true
                    });
                }

                #endregion

            }
        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Toggle()
        {
            if (_fade == 0F)
            {
                _bigMode = true;
                SetBigMode();
                _fade = 1F;
            }
            //else if(_fade == 1F)
            //{
            //    _bigMode = true;
            //    SetBigMode();
            //    _fade = 0.8F;
            //}
            else
            {
                _bigMode = false;
                SetSmallMode();
                _fade = 0;
            }

            Redraw();
        }

        private void SetSmallMode()
        {
            Index = 6;
            LocationLabel.Location = new Point(60, 27);

            _realBigMode = false;

            ToggleButton.Index = 26;
            ToggleButton.HoverIndex = 27;
            ToggleButton.PressedIndex = 28;
        }

        private void SetBigMode()
        {
            Index = 5;
            LocationLabel.Location = new Point(78, 156);

            _realBigMode = true;

            ToggleButton.Index = 29;
            ToggleButton.HoverIndex = 30;
            ToggleButton.PressedIndex = 31;


        }

        public void Process()
        {
            if (GameScene.Camera == null) return;

            if (GameScene.Observing == false)
            {

                if (GameScene.Scene.CharacterDialog.Grid[(int)EquipmentSlot.Armour].Item == null)
                {
                    GameScene.Scene.CharacterDialog.ArmourEffect.Visible = false;
                }
                if (GameScene.Scene.CharacterDialog.Grid[(int)EquipmentSlot.Weapon].Item == null)
                {
                    GameScene.Scene.CharacterDialog.WeaponEffect.Visible = false;
                }
                if (GameScene.Scene.InspectDialog.ArmorCell.Item == null)
                {
                    GameScene.Scene.InspectDialog.ArmourEffect.Visible = false;
                }
                if (GameScene.Scene.InspectDialog.WeaponCell.Item == null)
                {
                    GameScene.Scene.InspectDialog.WeaponEffect.Visible = false;
                }

                if (Settings.Effect == false)
                {
                    GameScene.Scene.CharacterDialog.ArmourEffect.Visible = false;
                    GameScene.Scene.CharacterDialog.WeaponEffect.Visible = false;
                    GameScene.Scene.InspectDialog.ArmourEffect.Visible = false;
                    GameScene.Scene.InspectDialog.WeaponEffect.Visible = false;
                }
            }

            MapControl map = GameScene.Scene.MapControl;
            if (map == null) return;
            MapNameLabel.Text = map.Title;
            LocationLabel.Text = Functions.PointToString(MapObject.Camera.CurrentLocation);

            int offset = _realBigMode ? 0 : 108;

            GameScene.Scene.MainDialog.SModeLabel.Location = new Point((GameScene.Scene.MiniMapDialog.Location.X + 7) - GameScene.Scene.MainDialog.Location.X,
            (GameScene.Scene.MiniMapDialog.Size.Height + 245) - Settings.ScreenHeight);
            GameScene.Scene.MainDialog.AModeLabel.Location = new Point((GameScene.Scene.MiniMapDialog.Location.X + 7) - GameScene.Scene.MainDialog.Location.X,
            (GameScene.Scene.MiniMapDialog.Size.Height + 260) - Settings.ScreenHeight);
            GameScene.Scene.MainDialog.PModeLabel.Location = new Point((GameScene.Scene.MiniMapDialog.Location.X + 7) - GameScene.Scene.MainDialog.Location.X,
            (GameScene.Scene.MiniMapDialog.Size.Height + 275) - Settings.ScreenHeight);


            if (GameScene.Scene.MainDialog.ObserveLabel.Visible)
            {
                GameScene.Scene.MainDialog.ObserveLabel.Location = new Point((GameScene.Scene.MiniMapDialog.Location.X + 7) - GameScene.Scene.MainDialog.Location.X,
                (GameScene.Scene.MiniMapDialog.Size.Height + 290) - Settings.ScreenHeight);
                GameScene.Scene.MainDialog.PingLabel.Location = new Point((GameScene.Scene.MiniMapDialog.Location.X + 7) - GameScene.Scene.MainDialog.Location.X,
                (GameScene.Scene.MiniMapDialog.Size.Height + 305) - Settings.ScreenHeight);
            }
            else
            {
                GameScene.Scene.MainDialog.PingLabel.Location = new Point((GameScene.Scene.MiniMapDialog.Location.X + 7) - GameScene.Scene.MainDialog.Location.X,
                (GameScene.Scene.MiniMapDialog.Size.Height + 290) - Settings.ScreenHeight);
            }





            if (GameScene.Scene.NewMail)
            {
                //    double time = (CMain.Time) / 100D;

                //    if (Math.Round(time) % 10 < 5 || GameScene.Scene.NewMailCounter >= 10)
                //    {
                //        NewMail.Visible = true;
                //    }
                //    else
                //    {
                //        if (NewMail.Visible)
                //        {
                //            GameScene.Scene.NewMailCounter++;
                //        }

                //        NewMail.Visible = false;
                //    }
                //}
                //else
                //{
                //    NewMail.Visible = false;
                //}
            }
        }
    }
    public sealed class InspectDialog : MirImageControl
    {
        public static UserItem[] Items = new UserItem[20];
        public static uint InspectID;

        public string Name;
        public string GuildName;
        public string GuildRank;
        public MirClass Class;
        public MirGender Gender;
        public byte Hair;
        public ushort Level;
        public string LoverName;

        public MirButton CloseButton, GroupButton, FriendButton, MailButton, TradeButton, LoverButton;
        public MirImageControl CharacterPage, ClassImage;
        public MirAnimatedControl ArmourEffect, WeaponEffect;
        public MirLabel NameLabel;
        public MirLabel GuildLabel, GuildRankLabel, LoverLabel;



        public MirItemCell
            WeaponCell,
            ArmorCell,
            HelmetCell,
            TorchCell,
            NecklaceCell,
            BraceletLCell,
            BraceletRCell,
            RingLCell,
            RingRCell,
            AmuletLCell,
            BeltCell,
            BootsCell,
            StoneCell,
            MountCell,
            AmuletRCell,
            ShieldCell,
            MedalCell,
            RewardStoneCell,
            CharmCell,
            ShoulderPadCell;

        public InspectDialog()
        {
            Index = 221;
            Library = Libraries.GameScene;
            Location = new Point(Settings.ScreenWidth - 352, 0);
            Movable = true;
            Sort = true;

            BeforeDraw += InspectDialog_BeforeDraw;

            CharacterPage = new MirImageControl
            {
                Index = 223,
                Parent = this,
                Library = Libraries.GameScene,
                Location = new Point(47, 104),
            };
            CharacterPage.AfterDraw += (o, e) =>
            {
                if (Libraries.StateItems == null) return;

                ItemInfo RealItem = null;

                if (ArmorCell.Item != null)
                {
                    RealItem = Functions.GetRealItem(ArmorCell.Item.Info, Level, Class, GameScene.ItemInfoList);
                    Libraries.StateItems.Draw(RealItem.Image, new Point(DisplayLocation.X + 58, DisplayLocation.Y + 23), Color.White, true, 1F);

                    int genderOffset = MapObject.User.Gender == MirGender.Male ? 0 : 27;
                    if (Settings.Effect == false) return;

                    switch (RealItem.Effect)
                    {
                        case 36:
                            ArmourEffect.Animated = true;
                            ArmourEffect.Index = 2405 + genderOffset;
                            ArmourEffect.AnimationCount = 14;
                            ArmourEffect.Loop = true;
                            ArmourEffect.Visible = true;
                            break;
                        case 0:
                            ArmourEffect.Animated = false;
                            ArmourEffect.Visible = false;
                            break;
                        default:
                            ArmourEffect.Animated = false;
                            ArmourEffect.Visible = false;
                            break;
                    }
                }

                if (WeaponCell.Item != null)
                {
                    RealItem = Functions.GetRealItem(WeaponCell.Item.Info, Level, Class, GameScene.ItemInfoList);
                    Libraries.StateItems.Draw(RealItem.Image, new Point(DisplayLocation.X + 58, DisplayLocation.Y + 23), Color.White, true, 1F);

                    if (Settings.Effect == false) return;

                    switch (RealItem.Shape)
                    {
                        case 136: //Test
                            WeaponEffect.Animated = true;
                            WeaponEffect.Index = 2268;
                            WeaponEffect.AnimationCount = 14;
                            WeaponEffect.Loop = true;
                            WeaponEffect.Visible = true;
                            break;
                        case 0:
                            WeaponEffect.Animated = false;
                            WeaponEffect.Visible = false;
                            break;
                        default:
                            WeaponEffect.Animated = false;
                            WeaponEffect.Visible = false;
                            break;
                    }
                }

                if (HelmetCell.Item != null)
                    Libraries.StateItems.Draw(HelmetCell.Item.Info.Image, new Point(DisplayLocation.X + 58, DisplayLocation.Y + 23), Color.White, true, 1F);
                else
                {
                    int hair = 441 + Hair + (Class == MirClass.Assassin ? 20 : 0) + (Gender == MirGender.Male ? 0 : 40);

                    int offSetX = Class == MirClass.Assassin ? (Gender == MirGender.Male ? 6 : 4) : 0;
                    int offSetY = Class == MirClass.Assassin ? (Gender == MirGender.Male ? 25 : 18) : 0;

                    Libraries.Prguse.Draw(hair, new Point(DisplayLocation.X + 58 + offSetX, DisplayLocation.Y + 26 + offSetY), Color.White, true, 1F);
                }
            };

            #region Effects

            ArmourEffect = new MirAnimatedControl
            {
                Animated = false,
                AnimationCount = 11,
                AnimationDelay = 100,
                Index = 15,
                Library = Libraries.StateEffect,
                Location = new Point(190, 256),
                Loop = false,
                Parent = this,
                Blending = true,
                BlendingRate = 1,
                UseOffSet = true,
                NotControl = true,
                Visible = false,
                //Sort = true,
            };

            WeaponEffect = new MirAnimatedControl
            {
                Animated = false,
                AnimationCount = 11,
                AnimationDelay = 100,
                Index = 15,
                Library = Libraries.StateEffect,
                Location = new Point(187, 221),
                Loop = false,
                Parent = this,
                Blending = true,
                BlendingRate = 1,
                UseOffSet = true,
                NotControl = true,
                Visible = false,
            };

            #endregion

            CloseButton = new MirButton
            {
                HoverIndex = 186,
                Index = 185,
                Location = new Point(306, 28),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 187,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            GroupButton = new MirButton
            {
                Index = 50,
                HoverIndex = 51,
                PressedIndex = 52,
                Parent = this,
                Location = new Point(55, 436),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Invite to Group"
            };
            GroupButton.Click += (o, e) =>
            {

                if (GroupDialog.GroupList.Count >= Globals.MaxGroup)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("Your group already has the maximum number of members.", ChatType.System);
                    return;
                }
                if (GroupDialog.GroupList.Count > 0 && GroupDialog.GroupList[0] != MapObject.Camera.Name)
                {

                    GameScene.Scene.ChatDialog.ReceiveChat("You are not the leader of your group.", ChatType.System);
                }

                Network.Enqueue(new C.AddMember { Name = Name });
                return;
            };

            FriendButton = new MirButton
            {
                Index = 62,
                HoverIndex = 63,
                PressedIndex = 64,
                Parent = this,
                Location = new Point(55, 416),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Add Friend"
            };
            FriendButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.AddFriend { Name = Name, Blocked = false });
            };

            MailButton = new MirButton
            {
                Index = 77,
                HoverIndex = 78,
                PressedIndex = 79,
                Parent = this,
                Location = new Point(296, 436),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Send Mail"
            };
            MailButton.Click += (o, e) => GameScene.Scene.MailComposeLetterDialog.ComposeMail(Name);

            TradeButton = new MirButton
            {
                Index = 44,
                HoverIndex = 45,
                PressedIndex = 46,
                Parent = this,
                Location = new Point(296, 416),
                Library = Libraries.GameScene,
                Sound = SoundList.ButtonA,
                Hint = "Begin Trade (" + CMain.InputKeys.GetKey(KeybindOptions.Trade) + ")"
            };
            TradeButton.Click += (o, e) => Network.Enqueue(new C.TradeRequest());

            NameLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(17, 25),
                Size = new Size(318, 26),
                NotControl = true,
            };
            NameLabel.Click += (o, e) =>
            {
                GameScene.Scene.ChatDialog.ChatTextBox.SetFocus();
                GameScene.Scene.ChatDialog.ChatTextBox.Text = string.Format("/{0} ", Name);
                GameScene.Scene.ChatDialog.ChatTextBox.Visible = true;
                GameScene.Scene.ChatDialog.ChatTextBox.TextBox.SelectionLength = 0;
                GameScene.Scene.ChatDialog.ChatTextBox.TextBox.SelectionStart = Name.Length + 2;

            };
            LoverButton = new MirButton
            {
                Index = 605,
                Location = new Point(50, 34),
                Library = Libraries.Prguse,
                Parent = this,
                Sound = SoundList.None
            };

            GuildLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(17, 57),
                Size = new Size(318, 17),
                NotControl = true,
            };

            GuildRankLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(17, 72),
                Size = new Size(318, 17),
                NotControl = true,
            };

            ClassImage = new MirImageControl
            {
                Index = 100,
                Library = Libraries.Prguse,
                Location = new Point(17, 23),
                Parent = this,
                NotControl = true,
            };


            WeaponCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Weapon,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(8, 45),
            };

            ArmorCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Armour,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(8, 90),
            };

            HelmetCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Helmet,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(248, 45),
            };

            NecklaceCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Necklace,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(248, 90),
            };

            TorchCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Torch,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(248, 135),
            };

            BraceletLCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.BraceletL,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(8, 180),
            };
            BraceletRCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.BraceletR,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(248, 180),
            };
            RingLCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RingL,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(8, 225),
            };
            RingRCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RingR,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(248, 225),
            };

            AmuletLCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.AmuletL,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(8, 270),
            };

            AmuletRCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.AmuletR,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(56, 270),
            };

            BootsCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Boots,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(104, 270),
            };
            BeltCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Belt,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(152, 270),
            };

            StoneCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Stone,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(200, 270),
            };

            ShieldCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Shield,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(248, 270),
            };

            MountCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Mount,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(56, 315),
            };

            CharmCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Charm,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(104, 315),
            };

            MedalCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Medal,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(152, 315),
            };

            RewardStoneCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RewardStone,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(200, 315),
            };

            ShoulderPadCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.ShoulderPad,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(8, 135),
            };
        }

        private void InspectDialog_BeforeDraw(object sender, EventArgs e)
        {
            GroupButton.Visible = !GameScene.Observing;
            FriendButton.Visible = !GameScene.Observing;
            MailButton.Visible = !GameScene.Observing;
            TradeButton.Visible = !GameScene.Observing;

        }

        public void RefreshInferface()
        {
            int offSet = Gender == MirGender.Male ? 0 : 1;

            CharacterPage.Index = 222 + offSet;

            switch (Class)
            {
                case MirClass.Warrior:
                    ClassImage.Index = 100;// + offSet * 5;
                    break;
                case MirClass.Wizard:
                    ClassImage.Index = 101;// + offSet * 5;
                    break;
                case MirClass.Taoist:
                    ClassImage.Index = 102;// + offSet * 5;
                    break;
                case MirClass.Assassin:
                    ClassImage.Index = 103;// + offSet * 5;
                    break;
                case MirClass.Archer:
                    ClassImage.Index = 104;// + offSet * 5;
                    break;
            }

            NameLabel.Text = Name;
            GuildLabel.Text = GuildName;
            GuildRankLabel.Text = GuildRank;
            if (LoverName != "")
            {
                LoverButton.Visible = true;
                LoverButton.Hint = LoverName;
            }
            else
                LoverButton.Visible = false;


            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == null) continue;
                GameScene.Bind(Items[i]);
            }
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
    public sealed class OptionDialog : MirImageControl
    {
        public MirButton SkillModeOn, SkillModeOff;
        public MirButton SkillBarOn, SkillBarOff;
        public MirButton EffectOn, EffectOff;
        public MirButton DropViewOn, DropViewOff;
        public MirButton NameViewOn, NameViewOff;
        public MirButton HPViewOn, HPViewOff;
        public MirImageControl SoundBar, MusicSoundBar;
        public MirImageControl VolumeBar, MusicVolumeBar;
        public MirButton ObserveOn, ObserveOff;


        public MirButton CloseButton;


        public OptionDialog()
        {
            Index = 96;
            Library = Libraries.GameScene;
            Movable = true;
            Sort = true;

            Location = new Point((Settings.ScreenWidth - Size.Width) / 2, (Settings.ScreenHeight - Size.Height) / 2);

            BeforeDraw += OptionPanel_BeforeDraw;

            CloseButton = new MirButton
            {
                HoverIndex = 186,
                Index = 185,
                Location = new Point(228, 25),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 187,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            //tilde option
            SkillModeOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 68),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 451,
            };
            SkillModeOn.Click += (o, e) =>
            {
                Settings.SkillMode = true;
                GameScene.Scene.ChatDialog.ReceiveChat("<SkillMode 2>", ChatType.Hint);
                ToggleSkillButtons(false);
            };

            //ctrl option
            SkillModeOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 68),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 454
            };
            SkillModeOff.Click += (o, e) =>
            {
                Settings.SkillMode = false;
                GameScene.Scene.ChatDialog.ReceiveChat("<SkillMode 1>", ChatType.Hint);
                ToggleSkillButtons(true);
            };

            SkillBarOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 93),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            SkillBarOn.Click += (o, e) => Settings.SkillBar = true;

            SkillBarOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 93),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            SkillBarOff.Click += (o, e) => Settings.SkillBar = false;

            EffectOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 118),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            EffectOn.Click += (o, e) => Settings.Effect = true;

            EffectOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 118),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            EffectOff.Click += (o, e) => Settings.Effect = false;

            DropViewOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 143),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            DropViewOn.Click += (o, e) => Settings.DropView = true;

            DropViewOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 143),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            DropViewOff.Click += (o, e) => Settings.DropView = false;

            NameViewOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 168),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            NameViewOn.Click += (o, e) => Settings.NameView = true;

            NameViewOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 168),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            NameViewOff.Click += (o, e) => Settings.NameView = false;

            HPViewOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 193),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 463,
            };
            HPViewOn.Click += (o, e) =>
            {
                Settings.HPView = true;
                GameScene.Scene.ChatDialog.ReceiveChat("<HP/MP Mode 1>", ChatType.Hint);
            };

            HPViewOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 193),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 466
            };
            HPViewOff.Click += (o, e) =>
            {
                Settings.HPView = false;
                GameScene.Scene.ChatDialog.ReceiveChat("<HP/MP Mode 2>", ChatType.Hint);
            };

            SoundBar = new MirImageControl
            {
                Index = 468,
                Library = Libraries.Prguse2,
                Location = new Point(159, 225),
                Parent = this,
                DrawImage = false,
            };
            SoundBar.MouseDown += SoundBar_MouseMove;
            SoundBar.MouseMove += SoundBar_MouseMove;
            SoundBar.BeforeDraw += SoundBar_BeforeDraw;

            VolumeBar = new MirImageControl
            {
                Index = 20,
                Library = Libraries.Prguse,
                Location = new Point(155, 218),
                Parent = this,
                NotControl = true,
            };

            MusicSoundBar = new MirImageControl
            {
                Index = 468,
                Library = Libraries.Prguse2,
                Location = new Point(159, 251),
                Parent = this,
                DrawImage = false
            };
            MusicSoundBar.MouseDown += MusicSoundBar_MouseMove;
            MusicSoundBar.MouseMove += MusicSoundBar_MouseMove;
            MusicSoundBar.MouseUp += MusicSoundBar_MouseUp;
            MusicSoundBar.BeforeDraw += MusicSoundBar_BeforeDraw;

            MusicVolumeBar = new MirImageControl
            {
                Index = 20,
                Library = Libraries.Prguse,
                Location = new Point(155, 244),
                Parent = this,
                NotControl = true,
            };

            ObserveOn = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 271),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            ObserveOn.Click += (o, e) =>
            {
                if (GameScene.Scene.AllowObserve) return;
                GameScene.Scene.AllowObserve = true;
                Network.Enqueue(new C.ChangeObserve { Allow = true });
            };

            ObserveOff = new MirButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 271),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            ObserveOff.Click += (o, e) =>
            {
                if (!GameScene.Scene.AllowObserve) return;
                GameScene.Scene.AllowObserve = false;
                Network.Enqueue(new C.ChangeObserve { Allow = false });
            };

        }


        private void ToggleSkillButtons(bool Ctrl)
        {
            foreach (KeyBind KeyCheck in CMain.InputKeys.Keylist)
            {
                if (KeyCheck.Key == Keys.None)
                    continue;
                if ((KeyCheck.function < KeybindOptions.Bar1Skill1) || (KeyCheck.function > KeybindOptions.Bar2Skill8)) continue;
                //need to test this 
                if ((KeyCheck.RequireCtrl != 1) && (KeyCheck.RequireTilde != 1)) continue;
                KeyCheck.RequireCtrl = (byte)(Ctrl ? 1 : 0);
                KeyCheck.RequireTilde = (byte)(Ctrl ? 0 : 1);
            }
        }

        private void SoundBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || SoundBar != ActiveControl) return;

            Point p = e.Location.Subtract(SoundBar.DisplayLocation);

            byte volume = (byte)(p.X / (double)SoundBar.Size.Width * 100);
            Settings.Volume = volume;


            double percent = Settings.Volume / 100D;
            if (percent > 1) percent = 1;

            VolumeBar.Location = percent > 0 ? new Point(159 + (int)((SoundBar.Size.Width - 2) * percent), 218) : new Point(159, 218);
        }

        private void SoundBar_BeforeDraw(object sender, EventArgs e)
        {
            if (SoundBar.Library == null) return;

            double percent = Settings.Volume / 100D;
            if (percent > 1) percent = 1;
            if (percent > 0)
            {
                Rectangle section = new Rectangle
                {
                    Size = new Size((int)((SoundBar.Size.Width - 2) * percent), SoundBar.Size.Height)
                };

                SoundBar.Library.Draw(SoundBar.Index, section, SoundBar.DisplayLocation, Color.White, false);
                VolumeBar.Location = new Point(159 + section.Size.Width, 218);
            }
            else
                VolumeBar.Location = new Point(159, 218);
        }

        private void MusicSoundBar_BeforeDraw(object sender, EventArgs e)
        {
            if (MusicSoundBar.Library == null) return;

            double percent = Settings.MusicVolume / 100D;
            if (percent > 1) percent = 1;
            if (percent > 0)
            {
                Rectangle section = new Rectangle
                {
                    Size = new Size((int)((MusicSoundBar.Size.Width - 2) * percent), MusicSoundBar.Size.Height)
                };

                MusicSoundBar.Library.Draw(MusicSoundBar.Index, section, MusicSoundBar.DisplayLocation, Color.White, false);
                MusicVolumeBar.Location = new Point(159 + section.Size.Width, 244);
            }
            else
                MusicVolumeBar.Location = new Point(159, 244);
        }

        public void MusicSoundBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (SoundManager.MusicVol <= -2900)
                SoundManager.MusicVol = -3000;
            if (SoundManager.MusicVol >= -100)
                SoundManager.MusicVol = 0;


            //SoundManager.Device.Dispose();
            //SoundManager.Create();
            //SoundManager.PlayMusic(SoundList.Music, true);

            if (SoundManager.Music == null) return;

            SoundManager.Music.SetVolume(SoundManager.MusicVol);

        }

        private void MusicSoundBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || MusicSoundBar != ActiveControl) return;

            Point p = e.Location.Subtract(MusicSoundBar.DisplayLocation);

            byte volume = (byte)(p.X / (double)MusicSoundBar.Size.Width * 100);
            Settings.MusicVolume = volume;


            double percent = Settings.MusicVolume / 100D;
            if (percent > 1) percent = 1;

            MusicVolumeBar.Location = percent > 0 ? new Point(159 + (int)((MusicSoundBar.Size.Width - 2) * percent), 244) : new Point(159, 244);
        }

        private void OptionPanel_BeforeDraw(object sender, EventArgs e)
        {
            if (Settings.SkillMode)
            {
                SkillModeOn.Index = 452;
                SkillModeOff.Index = 453;
            }
            else
            {
                SkillModeOn.Index = 450;
                SkillModeOff.Index = 455;
            }

            if (Settings.SkillBar)
            {
                SkillBarOn.Index = 458;
                SkillBarOff.Index = 459;
            }
            else
            {
                SkillBarOn.Index = 456;
                SkillBarOff.Index = 461;
            }

            if (Settings.Effect)
            {
                EffectOn.Index = 458;
                EffectOff.Index = 459;
            }
            else
            {
                EffectOn.Index = 456;
                EffectOff.Index = 461;
            }

            if (Settings.DropView)
            {
                DropViewOn.Index = 458;
                DropViewOff.Index = 459;
            }
            else
            {
                DropViewOn.Index = 456;
                DropViewOff.Index = 461;
            }

            if (Settings.NameView)
            {
                NameViewOn.Index = 458;
                NameViewOff.Index = 459;
            }
            else
            {
                NameViewOn.Index = 456;
                NameViewOff.Index = 461;
            }

            if (Settings.HPView)
            {
                HPViewOn.Index = 464;
                HPViewOff.Index = 465;
            }
            else
            {
                HPViewOn.Index = 462;
                HPViewOff.Index = 467;
            }

            if (GameScene.Scene.AllowObserve)
            {
                ObserveOn.Index = 458;
                ObserveOff.Index = 459;
            }
            else
            {
                ObserveOn.Index = 456;
                ObserveOff.Index = 461;
            }
        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

    }

    public sealed class MagicButton : MirControl
    {
        public MirImageControl LevelImage, ExpImage;
        public MirButton SkillButton;
        public MirLabel LevelLabel, NameLabel, ExpLabel, KeyLabel;
        public ClientMagic Magic;
        public MirAnimatedControl CoolDown;

        public MagicButton()
        {
            Size = new Size(300, 200);

            SkillButton = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.MagIcon2,
                Parent = this,
                Location = new Point(0, 0),
                Sound = SoundList.ButtonA,
            };
            SkillButton.Click += (o, e) => new AssignKeyPanel(Magic);

            LevelImage = new MirImageControl
            {
                Index = 516,
                Library = Libraries.Title,
                Location = new Point(73, 7),
                Parent = this,
                NotControl = true,
            };

            ExpImage = new MirImageControl
            {
                Index = 517,
                Library = Libraries.Title,
                Location = new Point(73, 19),
                Parent = this,
                NotControl = true,
            };

            LevelLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(88, 2),
                NotControl = true,
            };

            NameLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(109, 2),
                NotControl = true,
            };

            ExpLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(109, 15),
                NotControl = true,
            };

            KeyLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(38, 2),
                NotControl = true,
            };

            CoolDown = new MirAnimatedControl
            {
                Library = Libraries.Prguse2,
                Parent = this,
                Location = new Point(36, 0),
                NotControl = true,
                UseOffSet = true,
                Loop = false,
                Animated = false,
                Opacity = 0.6F
            };
        }

        public void Update(ClientMagic magic)
        {
            Magic = magic;

            NameLabel.Text = Magic.Name;

            LevelLabel.Text = Magic.Level.ToString();
            switch (Magic.Level)
            {
                case 0:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need1);
                    break;
                case 1:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need2);
                    break;
                case 2:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need3);
                    break;
                case 3:
                    ExpLabel.Text = "-";
                    break;
            }

            if (Magic.Key > 8)
            {
                int key = Magic.Key % 8;

                KeyLabel.Text = string.Format("CTRL" + Environment.NewLine + "F{0}", key != 0 ? key : 8);
            }
            else if (Magic.Key > 0)
                KeyLabel.Text = string.Format("F{0}", Magic.Key);
            else
                KeyLabel.Text = string.Empty;


            SkillButton.Index = Magic.Icon * 2;
            SkillButton.PressedIndex = Magic.Icon * 2 + 1;

            SetDelay();
        }

        public void SetDelay()
        {
            if (Magic == null) return;

            int totalFrames = 34;

            long timeLeft = Magic.CastTime + Magic.Delay - CMain.Time;

            if (timeLeft < 100 || (CoolDown != null && CoolDown.Animated)) return;

            int delayPerFrame = (int)(Magic.Delay / totalFrames);
            int startFrame = totalFrames - (int)(timeLeft / delayPerFrame);

            if ((CMain.Time <= Magic.CastTime + Magic.Delay) && Magic.CastTime > 0)
            {
                CoolDown.Dispose();

                CoolDown = new MirAnimatedControl
                {
                    Index = 1290 + startFrame,
                    AnimationCount = (totalFrames - startFrame),
                    AnimationDelay = delayPerFrame,
                    Library = Libraries.Prguse2,
                    Parent = this,
                    Location = new Point(36, 0),
                    NotControl = true,
                    UseOffSet = true,
                    Loop = false,
                    Animated = true,
                    Opacity = 0.6F
                };
            }
        }
    }
    public sealed class AssignKeyPanel : MirImageControl
    {
        public MirButton SaveButton, NoneButton;

        public MirLabel TitleLabel, SaveLabel, NoneLabel;
        public MirImageControl MagicImage;
        public MirButton[] FKeys;

        public ClientMagic Magic;
        public byte Key;

        public AssignKeyPanel(ClientMagic magic)
        {
            Magic = magic;
            Key = magic.Key;

            Modal = true;
            Index = 87;
            Library = Libraries.GameScene;
            Location = Center;
            Parent = GameScene.Scene;
            Visible = true;

            MagicImage = new MirImageControl
            {
                Location = new Point(20, 53),
                Index = magic.Icon * 2,
                Library = Libraries.MagIcon2,
                Parent = this,
            };

            TitleLabel = new MirLabel
            {
                Location = new Point(54, 57),
                Parent = this,
                Size = new Size(230, 32),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak,
                Text = string.Format(GameLanguage.SelectKey, magic.Name)
            };

            NoneButton = new MirButton
            {
                Index = 228, //154
                HoverIndex = 229,
                PressedIndex = 230,
                Library = Libraries.GameScene,
                Parent = this,
                Location = new Point(285, 98),
            };
            NoneButton.Click += (o, e) => Key = 0;

            NoneLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = NoneButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "None",
                NotControl = true,
            };

            SaveButton = new MirButton
            {
                Library = Libraries.GameScene,
                Parent = this,
                Location = new Point(285, 135),
                Index = 228,
                HoverIndex = 229,
                PressedIndex = 230,
            };
            SaveButton.Click += (o, e) =>
            {
                for (int i = 0; i < MapObject.User.Magics.Count; i++)
                {
                    if (MapObject.User.Magics[i].Key == Key)
                        MapObject.User.Magics[i].Key = 0;
                }

                Network.Enqueue(new C.MagicKey { Spell = Magic.Spell, Key = Key });
                Magic.Key = Key;
                foreach (SkillBarDialog Bar in GameScene.Scene.SkillBarDialogs)
                    Bar.Update();

                Dispose();
            };

            SaveLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = SaveButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Save",
                NotControl = true,
            };


            FKeys = new MirButton[16];

            FKeys[0] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(21, 92),
                Sound = SoundList.ButtonA,
                Text = "F1"
            };
            FKeys[0].Click += (o, e) => Key = 1;

            FKeys[1] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(53, 92),
                Sound = SoundList.ButtonA,
                Text = "F2"
            };
            FKeys[1].Click += (o, e) => Key = 2;

            FKeys[2] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(85, 92),
                Sound = SoundList.ButtonA,
                Text = "F3"
            };
            FKeys[2].Click += (o, e) => Key = 3;

            FKeys[3] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(117, 92),
                Sound = SoundList.ButtonA,
                Text = "F4"
            };
            FKeys[3].Click += (o, e) => Key = 4;

            FKeys[4] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(154, 92),
                Sound = SoundList.ButtonA,
                Text = "F5"
            };
            FKeys[4].Click += (o, e) => Key = 5;

            FKeys[5] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(186, 92),
                Sound = SoundList.ButtonA,
                Text = "F6",
            };
            FKeys[5].Click += (o, e) => Key = 6;

            FKeys[6] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(218, 92),
                Sound = SoundList.ButtonA,
                Text = "F7"
            };
            FKeys[6].Click += (o, e) => Key = 7;

            FKeys[7] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(250, 92),
                Sound = SoundList.ButtonA,
                Text = "F8"
            };
            FKeys[7].Click += (o, e) => Key = 8;


            FKeys[8] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(21, 129),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F1"
            };
            FKeys[8].Click += (o, e) => Key = 9;

            FKeys[9] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(53, 129),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F2"
            };
            FKeys[9].Click += (o, e) => Key = 10;

            FKeys[10] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(85, 129),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F3"
            };
            FKeys[10].Click += (o, e) => Key = 11;

            FKeys[11] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(117, 129),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F4"
            };
            FKeys[11].Click += (o, e) => Key = 12;

            FKeys[12] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(154, 129),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F5"
            };
            FKeys[12].Click += (o, e) => Key = 13;

            FKeys[13] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(186, 129),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F6"
            };
            FKeys[13].Click += (o, e) => Key = 14;

            FKeys[14] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(218, 129),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F7"
            };
            FKeys[14].Click += (o, e) => Key = 15;

            FKeys[15] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(250, 129),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F8"
            };
            FKeys[15].Click += (o, e) => Key = 16;

            BeforeDraw += AssignKeyPanel_BeforeDraw;
        }

        private void AssignKeyPanel_BeforeDraw(object sender, EventArgs e)
        {
            for (int i = 0; i < FKeys.Length; i++)
            {
                FKeys[i].Index = 1656;
                FKeys[i].HoverIndex = 1657;
                FKeys[i].PressedIndex = 1658;
                FKeys[i].Visible = true;
            }

            if (Key == 0 || Key > FKeys.Length) return;

            FKeys[Key - 1].Index = 1658;
            FKeys[Key - 1].HoverIndex = 1658;
            FKeys[Key - 1].PressedIndex = 1658;
        }
    }
    public sealed class BigMapDialog : MirControl
    {
        public BigMapDialog()
        {
            NotControl = true;
            Location = new Point(130, 100);
            //Border = true;
            //BorderColour = Color.Lime;
            BeforeDraw += (o, e) => OnBeforeDraw();
            Sort = true;
        }

        private void OnBeforeDraw()
        {
            MapControl map = GameScene.Scene.MapControl;
            if (map == null || !Visible) return;

            //int index = map.BigMap <= 0 ? map.MiniMap : map.BigMap;
            int index = map.BigMap;

            if (index <= 0)
            {
                if (Visible)
                {
                    Visible = false;
                }
                return;
            }

            TrySort();

            Rectangle viewRect = new Rectangle(0, 0, 600, 400);

            Size = Libraries.MiniMap.GetSize(index);

            if (Size.Width < 600)
                viewRect.Width = Size.Width;

            if (Size.Height < 400)
                viewRect.Height = Size.Height;

            viewRect.X = (Settings.ScreenWidth - viewRect.Width) / 2;
            viewRect.Y = (Settings.ScreenHeight - 120 - viewRect.Height) / 2;

            Location = viewRect.Location;
            Size = viewRect.Size;

            float scaleX = Size.Width / (float)map.Width;
            float scaleY = Size.Height / (float)map.Height;

            viewRect.Location = new Point(
                (int)(scaleX * MapObject.Camera.CurrentLocation.X) - viewRect.Width / 2,
                (int)(scaleY * MapObject.Camera.CurrentLocation.Y) - viewRect.Height / 2);

            if (viewRect.Right >= Size.Width)
                viewRect.X = Size.Width - viewRect.Width;
            if (viewRect.Bottom >= Size.Height)
                viewRect.Y = Size.Height - viewRect.Height;

            if (viewRect.X < 0) viewRect.X = 0;
            if (viewRect.Y < 0) viewRect.Y = 0;

            Libraries.MiniMap.Draw(index, Location, Size, Color.FromArgb(255, 255, 255));

            int startPointX = (int)(viewRect.X / scaleX);
            int startPointY = (int)(viewRect.Y / scaleY);

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];


                if (ob.Race == ObjectType.Item || ob.Dead || ob.Race == ObjectType.Spell) continue; // || (ob.ObjectID != MapObject.User.ObjectID)
                float x = ((ob.CurrentLocation.X - startPointX) * scaleX) + Location.X;
                float y = ((ob.CurrentLocation.Y - startPointY) * scaleY) + Location.Y;

                Color colour;

                if ((GroupDialog.GroupList.Contains(ob.Name) && MapObject.Camera != ob) || ob.Name.EndsWith(string.Format("({0})", MapObject.Camera.Name)))
                    colour = Color.FromArgb(0, 0, 255);
                else
                    if (ob is PlayerObject)
                    colour = Color.FromArgb(255, 255, 255);
                else if (ob is NPCObject || ob.AI == 6)
                    colour = Color.FromArgb(0, 255, 50);
                else
                    colour = Color.FromArgb(255, 0, 0);

                DXManager.Sprite.Draw2D(DXManager.RadarTexture, Point.Empty, 0, new PointF((int)(x - 0.5F), (int)(y - 0.5F)), colour);
            }
        }


        public void Toggle()
        {
            Visible = !Visible;

            Redraw();
        }
    }
    public sealed class DuraStatusDialog : MirImageControl
    {
        public MirButton Character;

        public DuraStatusDialog()
        {
            Size = new Size(40, 19);
            Location = new Point((GameScene.Scene.MiniMapDialog.Location.X + 86), GameScene.Scene.MiniMapDialog.Size.Height);

            //Character = new MirButton()
            //{
            //    Index = 2113,
            //    Library = Libraries.Prguse,
            //    Parent = this,
            //    Size = new Size(20, 19),
            //    Location = new Point(20, 0),
            //    HoverIndex = 2111,
            //    PressedIndex = 2112,
            //    Sound = SoundList.ButtonA,
            //    Hint = "Dura Panel"
            //};
            //Character.Click += (o, e) =>
            //{
            //    if (GameScene.Scene.CharacterDuraPanel.Visible == true)
            //    {
            //        GameScene.Scene.CharacterDuraPanel.Hide();
            //        Settings.DuraView = false;
            //    }
            //    else
            //    {
            //        GameScene.Scene.CharacterDuraPanel.Show();
            //        Settings.DuraView = true;
            //    }
            //};
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
    public sealed class CharacterDuraPanel : MirImageControl
    {
        public MirImageControl GrayBackground, Background, Helmet, Armour, Belt, Boots, Weapon, Necklace, RightBracelet, LeftBracelet, RightRing, LeftRing, Torch, Stone, Amulet, Mount, Item1, Item2;

        public CharacterDuraPanel()
        {
            Index = 2105;
            Library = Libraries.Prguse;
            Movable = true;
            Location = new Point(Settings.ScreenWidth - 61, 250);

            GrayBackground = new MirImageControl()
            {
                Index = 2161,
                Library = Libraries.Prguse,
                Parent = this,
                Size = new Size(56, 80),
                Location = new Point(3, 3),
                Opacity = 0.4F
            };
            Background = new MirImageControl()
            {
                Index = 2162,
                Library = Libraries.Prguse,
                Parent = this,
                Size = new Size(56, 80),
                Location = new Point(3, 3),
            };

            #region Pieces

            Helmet = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(24, 3) };
            Belt = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 7), Location = new Point(23, 23) };
            Armour = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(28, 32), Location = new Point(16, 11) };
            Boots = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(24, 9), Location = new Point(17, 43) };
            Weapon = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 33), Location = new Point(4, 5) };
            Necklace = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(3, 67) };
            LeftBracelet = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 8), Location = new Point(3, 43) };
            RightBracelet = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 8), Location = new Point(43, 43) };
            LeftRing = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(3, 54) };
            RightRing = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(43, 54) };
            Torch = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(8, 32), Location = new Point(44, 5) };
            Stone = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(30, 54) };
            Amulet = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(16, 54) };
            Mount = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(43, 68) };
            Item1 = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(8, 12), Location = new Point(19, 67) };
            Item2 = new MirImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(8, 12), Location = new Point(31, 67) };

            #endregion
        }

        public void GetCharacterDura()
        {
            if (GameScene.Scene.CharacterDialog.Grid[0].Item == null) { Weapon.Index = -1; }
            if (GameScene.Scene.CharacterDialog.Grid[1].Item == null) { Armour.Index = -1; }
            if (GameScene.Scene.CharacterDialog.Grid[2].Item == null) { Helmet.Index = -1; }
            if (GameScene.Scene.CharacterDialog.Grid[3].Item == null) { Torch.Index = -1; }
            if (GameScene.Scene.CharacterDialog.Grid[4].Item == null) { Necklace.Index = -1; }
            if (GameScene.Scene.CharacterDialog.Grid[5].Item == null) { LeftBracelet.Index = -1; }
            if (GameScene.Scene.CharacterDialog.Grid[6].Item == null) { RightBracelet.Index = -1; }
            if (GameScene.Scene.CharacterDialog.Grid[7].Item == null) { LeftRing.Index = -1; }
            if (GameScene.Scene.CharacterDialog.Grid[8].Item == null) { RightRing.Index = -1; }
            if (GameScene.Scene.CharacterDialog.Grid[9].Item == null) { Amulet.Index = -1; }
            if (GameScene.Scene.CharacterDialog.Grid[10].Item == null) { Belt.Index = -1; }
            if (GameScene.Scene.CharacterDialog.Grid[11].Item == null) { Boots.Index = -1; }
            if (GameScene.Scene.CharacterDialog.Grid[12].Item == null) { Stone.Index = -1; }

            for (int i = 0; i < MapObject.User.Equipment.Length; i++)
            {
                if (MapObject.User.Equipment[i] == null) continue;
                UpdateCharacterDura(MapObject.User.Equipment[i]);
            }
        }
        public void UpdateCharacterDura(UserItem item)
        {
            int Warning = item.MaxDura / 2;
            int Danger = item.MaxDura / 5;
            uint AmuletWarning = item.Info.StackSize / 2;
            uint AmuletDanger = item.Info.StackSize / 5;

            switch (item.Info.Type)
            {
                case ItemType.Amulet: //Based on stacks of 5000
                    if (item.Count > AmuletWarning)
                        Amulet.Index = 2134;
                    if (item.Count <= AmuletWarning)
                        Amulet.Index = 2135;
                    if (item.Count <= AmuletDanger)
                        Amulet.Index = 2136;
                    if (item.Count == 0)
                        Amulet.Index = -1;
                    break;
                case ItemType.Armour:
                    if (item.CurrentDura > Warning)
                        Armour.Index = 2149;
                    if (item.CurrentDura <= Warning)
                        Armour.Index = 2150;
                    if (item.CurrentDura <= Danger)
                        Armour.Index = 2151;
                    if (item.CurrentDura == 0)
                        Armour.Index = -1;
                    break;
                case ItemType.Belt:
                    if (item.CurrentDura > Warning)
                        Belt.Index = 2158;
                    if (item.CurrentDura <= Warning)
                        Belt.Index = 2159;
                    if (item.CurrentDura <= Danger)
                        Belt.Index = 2160;
                    if (item.CurrentDura == 0)
                        Belt.Index = -1;
                    break;
                case ItemType.Boots:
                    if (item.CurrentDura > Warning)
                        Boots.Index = 2152;
                    if (item.CurrentDura <= Warning)
                        Boots.Index = 2153;
                    if (item.CurrentDura <= Danger)
                        Boots.Index = 2154;
                    if (item.CurrentDura == 0)
                        Boots.Index = -1;
                    break;
                case ItemType.Bracelet:
                    if (GameScene.Scene.CharacterDialog.Grid[(byte)EquipmentSlot.BraceletR].Item != null && item.UniqueID == GameScene.Scene.CharacterDialog.Grid[(byte)EquipmentSlot.BraceletR].Item.UniqueID)
                    {
                        if (item.CurrentDura > Warning)
                            RightBracelet.Index = 2143;
                        if (item.CurrentDura <= Warning)
                            RightBracelet.Index = 2144;
                        if (item.CurrentDura <= Danger)
                            RightBracelet.Index = 2145;
                        if (item.CurrentDura == 0)
                            RightBracelet.Index = -1;
                    }
                    else if (GameScene.Scene.CharacterDialog.Grid[(byte)EquipmentSlot.BraceletL].Item != null && item.UniqueID == GameScene.Scene.CharacterDialog.Grid[(byte)EquipmentSlot.BraceletL].Item.UniqueID)
                    {
                        if (item.CurrentDura > Warning)
                            LeftBracelet.Index = 2143;
                        if (item.CurrentDura <= Warning)
                            LeftBracelet.Index = 2144;
                        if (item.CurrentDura <= Danger)
                            LeftBracelet.Index = 2145;
                        if (item.CurrentDura == 0)
                            LeftBracelet.Index = -1;
                    }
                    break;
                case ItemType.Helmet:
                    if (item.CurrentDura > Warning)
                        Helmet.Index = 2155;
                    if (item.CurrentDura <= Warning)
                        Helmet.Index = 2156;
                    if (item.CurrentDura <= Danger)
                        Helmet.Index = 2157;
                    if (item.CurrentDura == 0)
                        Helmet.Index = -1;
                    break;
                case ItemType.Necklace:
                    if (item.CurrentDura > Warning)
                        Necklace.Index = 2122;
                    if (item.CurrentDura <= Warning)
                        Necklace.Index = 2123;
                    if (item.CurrentDura <= Danger)
                        Necklace.Index = 2124;
                    if (item.CurrentDura == 0)
                        Necklace.Index = -1;
                    break;
                case ItemType.Ring:
                    if (GameScene.Scene.CharacterDialog.Grid[(byte)EquipmentSlot.RingR].Item != null && item.UniqueID == GameScene.Scene.CharacterDialog.Grid[(byte)EquipmentSlot.RingR].Item.UniqueID)
                    {
                        if (item.CurrentDura > Warning)
                            RightRing.Index = 2131;
                        if (item.CurrentDura <= Warning)
                            RightRing.Index = 2132;
                        if (item.CurrentDura <= Danger)
                            RightRing.Index = 2133;
                        if (item.CurrentDura == 0)
                            RightRing.Index = -1;
                    }
                    else if (GameScene.Scene.CharacterDialog.Grid[(byte)EquipmentSlot.RingL].Item != null && item.UniqueID == GameScene.Scene.CharacterDialog.Grid[(byte)EquipmentSlot.RingL].Item.UniqueID)
                    {
                        if (item.CurrentDura > Warning)
                            LeftRing.Index = 2131;
                        if (item.CurrentDura <= Warning)
                            LeftRing.Index = 2132;
                        if (item.CurrentDura <= Danger)
                            LeftRing.Index = 2133;
                        if (item.CurrentDura == 0)
                            LeftRing.Index = -1;
                    }
                    break;
                case ItemType.Stone:
                    if (item.CurrentDura == 0)
                        Stone.Index = 2137;
                    break;
                case ItemType.Mount:
                    if (item.CurrentDura > Warning)
                        Mount.Index = 2140;
                    if (item.CurrentDura <= Warning)
                        Mount.Index = 2141;
                    if (item.CurrentDura <= Danger)
                        Mount.Index = 2142;
                    if (item.CurrentDura == 0)
                        Mount.Index = -1;
                    break;
                case ItemType.Torch:
                    if (item.CurrentDura > Warning)
                        Torch.Index = 2146;
                    if (item.CurrentDura <= Warning)
                        Torch.Index = 2147;
                    if (item.CurrentDura <= Danger)
                        Torch.Index = 2148;
                    if (item.CurrentDura == 0)
                        Torch.Index = -1;
                    break;
                case ItemType.Weapon:
                    if (item.CurrentDura > Warning)
                        Weapon.Index = 2125;
                    if (item.CurrentDura <= Warning)
                        Weapon.Index = 2126;
                    if (item.CurrentDura <= Danger)
                        Weapon.Index = 2127;
                    if (item.CurrentDura == 0)
                        Weapon.Index = -1;
                    break;
            }
        }

        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
            GameScene.Scene.MainDialog.DuraButton.Index = 15;
        }
        public void Show()
        {
            if (Visible) return;
            Visible = true;
            GameScene.Scene.MainDialog.DuraButton.Index = 15;

            GetCharacterDura();
        }
    }
}