using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirSounds;
using C = ClientPackets;
using S = ServerPackets;
using System.Threading;
using Mir.DiscordExtension;

namespace Client.MirScenes
{
    public class SelectScene : MirScene
    {
        private NewCharacterDialog _character;
        
        public MirImageControl Background;
        public MirAnimatedControl CharacterDisplay;
        public MirButton StartGameButton, NewCharacterButton, DeleteCharacterButton, LogoutButton, ExitGame;
        public CharacterButton[] CharacterButtons;
        public MirLabel LastAccessLabel, LastAccessLabelLabel, StartLabel, NewLabel, DelLabel, LogoutLabel, ExitLabel;
        public List<SelectInfo> Characters = new List<SelectInfo>();
        private int _selected;

        public SelectScene(List<SelectInfo> characters)
        {
            SoundManager.PlaySound(SoundList.SelectMusic, true);
            Disposing += (o, e) => SoundManager.StopSound(SoundList.SelectMusic);


            Characters = characters;
            SortList();

            KeyPress +=SelectScene_KeyPress;

            Background = new MirImageControl
            {
                Index = Settings.Resolution < 1366 ? 0 : 1,
                Location = new Point(0, 0),
                Library = Libraries.SelectScene,
                Size = new Size(1024, 758),
                Parent = this,
            };
            
            StartGameButton = new MirButton
            {
                Index = 3,
                HoverIndex = 4,
                PressedIndex = 5,
                Library = Libraries.SelectScene,
                Location = Settings.Resolution < 1366 ? new Point(301, 703) : new Point(472, 703),
                Parent = Background,
                GrayScale = true,
                Enabled = false
            };
            StartGameButton.Click += (o, e) => StartGame();

            StartLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = StartGameButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Start",
                NotControl = true,
            };

            NewCharacterButton = new MirButton
            {
                Index = 3,
                HoverIndex = 4,
                PressedIndex = 5,
                Library = Libraries.SelectScene,
                Location = Settings.Resolution < 1366 ? new Point(387, 703) : new Point(558, 703),
                Parent = Background,
             };
            NewCharacterButton.Click += (o, e) => _character = new NewCharacterDialog { Parent = this };

            NewLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = NewCharacterButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "New Char",
                NotControl = true,
            };

            DeleteCharacterButton = new MirButton
            {
                Index = 3,
                HoverIndex = 4,
                PressedIndex = 5,
                Library = Libraries.SelectScene,
                Location = Settings.Resolution < 1366 ? new Point(473, 703) : new Point(644, 703),
                Parent = Background,
            };
            DeleteCharacterButton.Click += (o, e) => DeleteCharacter();

            DelLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = DeleteCharacterButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Delete Char",
                NotControl = true,
            };

            LogoutButton = new MirButton
            {
                Index = 3,
                HoverIndex = 4,
                PressedIndex = 5,
                Library = Libraries.SelectScene,
                Location = Settings.Resolution < 1366 ? new Point(559, 703) : new Point(730, 703),
                Parent = Background,
            };
            LogoutButton.Click += (o, e) => Logout();

            LogoutLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = LogoutButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Logout",
                NotControl = true,
            };

            ExitGame = new MirButton
            {
                Index = 3,
                HoverIndex = 4,
                PressedIndex = 5,
                Library = Libraries.SelectScene,
                Location = Settings.Resolution < 1366 ? new Point(645, 703) : new Point(816, 703),
                Parent = Background,
            };
            ExitGame.Click += (o, e) => Program.Form.Close();

            ExitLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = ExitGame,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Exit",
                NotControl = true,
            };


            CharacterDisplay = new MirAnimatedControl
            {
                Animated = true,
                AnimationCount = 12,
                AnimationDelay = 150,
                FadeIn = true,
                FadeInDelay = 75,
                FadeInRate = 0.1F,
                Index = 0,
                Library = Libraries.SelectScene,
                Location = Settings.Resolution < 1366 ? new Point(162, 257) : new Point(333, 257),
                Parent = Background,
                UseOffSet = true,
                Visible = false
            };
            //CharacterDisplay.AfterDraw += (o, e) =>
            //{
            //    // if (_selected >= 0 && _selected < Characters.Count && characters[_selected].Class == MirClass.Wizard)
            //    Libraries.ChrSel.DrawBlend(CharacterDisplay.Index + 24, CharacterDisplay.DisplayLocationWithoutOffSet, Color.White, true);
            //};

            CharacterButtons = new CharacterButton[6];

            CharacterButtons[0] = new CharacterButton
            {
                Location = Settings.Resolution < 1366 ? new Point(580, 255) : new Point(750, 255),
                Parent = Background,
                Sound = SoundList.ButtonA,
            };
            CharacterButtons[0].Click += (o,e) =>
            {
                if (characters.Count <= 0) return;

                _selected = 0;
                UpdateInterface();
            };

            CharacterButtons[1] = new CharacterButton
            {
                Location = Settings.Resolution < 1366 ? new Point(580, 317) : new Point(750, 317),
                Parent = Background,
                Sound = SoundList.ButtonA,
            };
            CharacterButtons[1].Click += (o, e) =>
            {
                if (characters.Count <= 1) return;
                _selected = 1;
                UpdateInterface();
            };

            CharacterButtons[2] = new CharacterButton
            {
                Location = Settings.Resolution < 1366 ? new Point(580, 379) : new Point(750, 379),
                Parent = Background,
                Sound = SoundList.ButtonA,
            };
            CharacterButtons[2].Click += (o, e) =>
            {
                if (characters.Count <= 2) return;

                _selected = 2;
                UpdateInterface();
            };
            
            CharacterButtons[3] = new CharacterButton
            {
                Location = Settings.Resolution < 1366 ? new Point(580, 441) : new Point(750, 441),
                Parent = Background,
                Sound = SoundList.ButtonA,
            };
            CharacterButtons[3].Click += (o, e) =>
            {
                if (characters.Count <= 3) return;

                _selected = 3;
                UpdateInterface();
            };
            CharacterButtons[4] = new CharacterButton
            {
                Location = Settings.Resolution < 1366 ? new Point(580, 503) : new Point(750, 503),
                Parent = Background,
                Sound = SoundList.ButtonA,
            };
            CharacterButtons[4].Click += (o, e) =>
            {
                if (characters.Count <= 4) return;

                _selected = 4;
                UpdateInterface();
            };
            CharacterButtons[5] = new CharacterButton
            {
                Location = Settings.Resolution < 1366 ? new Point(580, 565) : new Point(750, 565),
                Parent = Background,
                Sound = SoundList.ButtonA,
            };
            CharacterButtons[5].Click += (o, e) =>
            {
                if (characters.Count <= 5) return;

                _selected = 5;
                UpdateInterface();
            };

            LastAccessLabel = new MirLabel
            {
                Location = new Point((Settings.ScreenWidth / 2) - 10, 668),
                Parent = Background,
                Size = new Size(180, 21),
                DrawFormat = TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                Border = false,
            };
            LastAccessLabelLabel = new MirLabel
                {
                    Location = new Point(-60, 0),
                    Parent = LastAccessLabel,
                    Text = "Last Online:",
                    Size = new Size(100, 21),
                    DrawFormat = TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                    Border = true,
                };
            UpdateInterface();
            Program.discord.UpdateStage(StatusType.GameState, GameState.SelectingCharacter);
            Program.discord.UpdateActivity();
        }

        private void SelectScene_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (StartGameButton.Enabled)
                StartGame();
            e.Handled = true;
        }


        public void SortList()
        {
            if (Characters != null)
                Characters.Sort((c1, c2) => c2.LastAccess.CompareTo(c1.LastAccess));
        }


        public void StartGame()
        {
            if (!Libraries.Loaded)
            {
                MirMessageBox message = new MirMessageBox(string.Format("Please wait, The game is still loading... {0:##0}%", Libraries.Progress / (double)Libraries.Count * 100), MirMessageBoxButtons.Cancel);

                message.BeforeDraw += (o, e) => message.Label.Text = string.Format("Please wait, The game is still loading... {0:##0}%", Libraries.Progress / (double)Libraries.Count * 100);

                message.AfterDraw += (o, e) =>
                {
                    if (!Libraries.Loaded) return;
                    message.Dispose();
                    StartGame();
                };

                message.Show();

                return;
            }
            StartGameButton.Enabled = false;

            Network.Enqueue(new C.StartGame
            {
                CharacterIndex = Characters[_selected].Index
            });
        }

        public override void Process()
        {
            

        }
        public override void ProcessPacket(Packet p)
        {
            switch (p.Index)
            {
                case (short)ServerPacketIds.NewCharacter:
                    NewCharacter((S.NewCharacter)p);
                    break;
                case (short)ServerPacketIds.NewCharacterSuccess:
                    NewCharacter((S.NewCharacterSuccess)p);
                    break;
                case (short)ServerPacketIds.DeleteCharacter:
                    DeleteCharacter((S.DeleteCharacter)p);
                    break;
                case (short)ServerPacketIds.DeleteCharacterSuccess:
                    DeleteCharacter((S.DeleteCharacterSuccess)p);
                    break;
                case (short)ServerPacketIds.StartGame:
                    StartGame((S.StartGame)p);
                    break;
                case (short)ServerPacketIds.StartGameBanned:
                    StartGame((S.StartGameBanned)p);
                    break;
                case (short)ServerPacketIds.StartGameDelay:
                    StartGame((S.StartGameDelay) p);
                    break;
                default:
                    base.ProcessPacket(p);
                    break;
            }
        }

        private void NewCharacter(S.NewCharacter p)
        {
            _character.AcceptButton.Enabled = true;
            
            switch (p.Result)
            {
                case 0:
                    MirMessageBox.Show(GameLanguage.CreatingCharactersDisabled);
                    _character.Dispose();
                    break;
                case 1:
                    MirMessageBox.Show(GameLanguage.InvalidCharacterName);
                    _character.NameTextBox.SetFocus();
                    break;
                case 2:
                    MirMessageBox.Show("The gender you selected does not exist.\n Contact a GM for assistance.");
                    break;
                case 3:
                    MirMessageBox.Show(GameLanguage.NoClass);
                    break;
                case 4:
                    MirMessageBox.Show(string.Format(GameLanguage.ToManyCharacters, Globals.MaxCharacterCount));
                    _character.Dispose();
                    break;
                case 5:
                    MirMessageBox.Show(GameLanguage.CharacterNameExists);
                    _character.NameTextBox.SetFocus();
                    break;
            }


        }
        private void NewCharacter(S.NewCharacterSuccess p)
        {
            _character.Dispose();
            MirMessageBox.Show(GameLanguage.CharacterCreated);
            
            Characters.Insert(0, p.CharInfo);
            _selected = 0;
            UpdateInterface();
        }

        private void Logout()
        {
            ActiveScene = new LoginScene();
            Dispose();
        }

        private void DeleteCharacter()
        {
            if (_selected < 0 || _selected >= Characters.Count) return;

            MirMessageBox message = new MirMessageBox(string.Format(GameLanguage.DeleteCharacter, Characters[_selected].Name), MirMessageBoxButtons.YesNo);
            int index = Characters[_selected].Index;

            message.YesButton.Click += (o, e) =>
            {
                DeleteCharacterButton.Enabled = false;
                Network.Enqueue(new C.DeleteCharacter { CharacterIndex = index });
            };

            message.Show();
        }

        private void DeleteCharacter(S.DeleteCharacter p)
        {
            DeleteCharacterButton.Enabled = true;
            switch (p.Result)
            {
                case 0:
                    MirMessageBox.Show("Deleting characters is currently disabled.");
                    break;
                case 1:
                    MirMessageBox.Show("The character you selected does not exist.\n Contact a GM for assistance.");
                    break;
            }
        }
        private void DeleteCharacter(S.DeleteCharacterSuccess p)
        {
            DeleteCharacterButton.Enabled = true;
            MirMessageBox.Show(GameLanguage.CharacterDeleted);

            for (int i = 0; i < Characters.Count; i++)
                if (Characters[i].Index == p.CharacterIndex)
                {
                    Characters.RemoveAt(i);
                    break;
                }

            UpdateInterface();
        }

        private void StartGame(S.StartGameDelay p)
        {
            StartGameButton.Enabled = true;

            long time = CMain.Time + p.Milliseconds;

            MirMessageBox message = new MirMessageBox(string.Format("You cannot log onto this character for another {0} seconds.", Math.Ceiling(p.Milliseconds/1000M)));

            message.BeforeDraw += (o, e) => message.Label.Text = string.Format("You cannot log onto this character for another {0} seconds.", Math.Ceiling((time - CMain.Time)/1000M));
                

            message.AfterDraw += (o, e) =>
            {
                if (CMain.Time <= time) return;
                message.Dispose();
                StartGame();
            };

            message.Show();
        }
        public void StartGame(S.StartGameBanned p)
        {
            StartGameButton.Enabled = true;

            TimeSpan d = p.ExpiryDate - CMain.Now;
            MirMessageBox.Show(string.Format("This account is banned.\n\nReason: {0}\nExpiryDate: {1}\nDuration: {2:#,##0} Hours, {3} Minutes, {4} Seconds", p.Reason,
                                             p.ExpiryDate, Math.Floor(d.TotalHours), d.Minutes, d.Seconds));
        }
        public void StartGame(S.StartGame p)
        {
            StartGameButton.Enabled = true;

            if (p.Resolution < Settings.Resolution || Settings.Resolution == 0) Settings.Resolution = p.Resolution;

            if (p.Resolution < 1366 || Settings.Resolution < 1280) Settings.Resolution = 1024;
            else if (p.Resolution < 1366 && Settings.Resolution > 1024) Settings.Resolution = 1280;//not adding an extra setting for 1280 on server cause well it just depends on the aspect ratio of your screen
            else if (p.Resolution > 1366 && Settings.Resolution < 1920) Settings.Resolution = 1366;
            else if (p.Resolution > 1366 && Settings.Resolution >= 1920) Settings.Resolution = 1920;

            switch (p.Result)
            {
                case 0:
                    MirMessageBox.Show("Starting the game is currently disabled.");
                    break;
                case 1:
                    MirMessageBox.Show("You are not logged in.");
                    break;
                case 2:
                    MirMessageBox.Show("Your character could not be found.");
                    break;
                case 3:
                    MirMessageBox.Show("No active map and/or start point found.");
                    break;
                case 4:
                    if (Settings.Resolution == 1024)
                        CMain.SetResolution(1024, 768);
                    else if (Settings.Resolution == 1280)
                        CMain.SetResolution(1280, 800);
                    else if (Settings.Resolution == 1366)
                        CMain.SetResolution(1366, 768);
                    else if (Settings.Resolution >= 1920)
                        CMain.SetResolution(1920, 1080);
                    ActiveScene = new GameScene();
                    Dispose();
                    break;
            }
        }
        private void UpdateInterface()
        {
            for (int i = 0; i < CharacterButtons.Length; i++)
            {
                CharacterButtons[i].Selected = i == _selected;
                CharacterButtons[i].Update(i >= Characters.Count ? null : Characters[i]);
            }

            if (_selected >= 0 && _selected < Characters.Count)
            {
                CharacterDisplay.Visible = true;
                //CharacterDisplay.Index = ((byte)Characters[_selected].Class + 1) * 20 + (byte)Characters[_selected].Gender * 280; 

                switch ((MirClass)Characters[_selected].Class)
                {
                    case MirClass.Warrior:
                        CharacterDisplay.Index = (byte)Characters[_selected].Gender == 0 ? 20 : 40; //220 : 500;
                        break;
                    case MirClass.Wizard:
                        CharacterDisplay.Index = (byte)Characters[_selected].Gender == 0 ? 60 : 80; //240 : 520;
                        break;
                    case MirClass.Taoist:
                        CharacterDisplay.Index = (byte)Characters[_selected].Gender == 0 ? 100 : 120; //260 : 540;
                        break;
                    case MirClass.Assassin:
                        CharacterDisplay.Index = (byte)Characters[_selected].Gender == 0 ? 120 : 140; //280 : 560;
                        break;
                    case MirClass.Archer:
                        CharacterDisplay.Index = (byte)Characters[_selected].Gender == 0 ? 160 : 180; //160 : 180;
                        break;
                }

                LastAccessLabel.Text = Characters[_selected].LastAccess == DateTime.MinValue ? GameLanguage.Never : Characters[_selected].LastAccess.ToString();
                LastAccessLabel.Visible = true;
                LastAccessLabelLabel.Visible = true;
                StartGameButton.Enabled = true;
            }
            else
            {
                CharacterDisplay.Visible = false;
                LastAccessLabel.Visible = false;
                LastAccessLabelLabel.Visible = false;
                StartGameButton.Enabled = false;
            }
        }


        #region Disposable
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Background = null;
                _character = null;
                
                CharacterDisplay = null;
                StartGameButton = null;
                StartLabel = null;
                NewLabel = null;
                DelLabel = null;
                LogoutLabel = null;
                ExitLabel = null;
                NewCharacterButton = null;
                DeleteCharacterButton = null; 
                LogoutButton = null;
                ExitGame = null;
                CharacterButtons = null;
                LastAccessLabel = null;LastAccessLabelLabel = null;
                Characters  = null;
                _selected = 0;
            }

            base.Dispose(disposing);
        }
        #endregion
        public sealed class NewCharacterDialog : MirImageControl
        {
            private static readonly Regex Reg = new Regex(@"^[\u4e00-\u9fa5_A-Za-z0-9]{" + Globals.MinCharacterNameLength + "," + Globals.MaxCharacterNameLength + "}$");

            public MirImageControl TitleLabel;
            public MirAnimatedControl CharacterDisplay;

            public MirButton AcceptButton,
                             CancelButton,
                             WarriorButton,
                             WizardButton,
                             TaoistButton,
                             MaleButton,
                             FemaleButton;

            public MirTextBox NameTextBox;

            public MirLabel Description, AcceptLabel, CancelLabel;

            private MirClass _class;
            private MirGender _gender;

            #region Descriptions
            public string WarriorDescription = GameLanguage.WarriorsDes;
            public string WizardDescription = GameLanguage.WizardDes;
            public string TaoistDescription = GameLanguage.TaoistDes;
            public string AssassinDescription = GameLanguage.AssassinDes;
            public string ArcherDescription = GameLanguage.ArcherDes;
            #endregion

            public NewCharacterDialog()
            {
                Index = 2;
                Library = Libraries.SelectScene;
                //Location = new Point((Settings.ScreenWidth - Size.Width)/2, (Settings.ScreenHeight - Size.Height)/2);
                Location = Center;
                Modal = true;
                
                AcceptButton = new MirButton
                    {
                      Enabled = false,
                      Index = 3,
                      HoverIndex = 4,
                      PressedIndex = 5,
                      Library = Libraries.SelectScene,
                      Location = new Point(404, 428),
                      Parent = this,
                    };
                AcceptButton.Click += (o, e) => CreateCharacter();

                AcceptLabel = new MirLabel
                {
                    Location = new Point(0, -2),
                    Parent = AcceptButton,
                    Size = new Size(78, 20),
                    DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                    Text = "Accept",
                    NotControl = true,
                };

                CancelButton = new MirButton
                {
                    Index = 3,
                    HoverIndex = 4,
                    PressedIndex = 5,
                    Library = Libraries.SelectScene,
                    Location = new Point(490, 428),
                    Parent = this,
                };
                CancelButton.Click += (o, e) => Dispose();

                CancelLabel = new MirLabel
                {
                    Location = new Point(0, -2),
                    Parent = CancelButton,
                    Size = new Size(78, 20),
                    DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                    Text = "Cancel",
                    NotControl = true,
                };

                NameTextBox = new MirTextBox
                    {
                        Location = new Point(463, 104),
                        Parent = this,
                        Size = new Size(158, 17),
                        MaxLength = Globals.MaxCharacterNameLength
                    };
                NameTextBox.TextBox.KeyPress += TextBox_KeyPress;
                NameTextBox.TextBox.TextChanged += CharacterNameTextBox_TextChanged;
                NameTextBox.SetFocus();

                CharacterDisplay = new MirAnimatedControl
                {
                        Animated = true,
                        AnimationCount = 12,
                        AnimationDelay = 150,
                        FadeIn = true,
                        FadeInDelay = 75,
                        FadeInRate = 0.1F,
                        Index = 20,
                        Library = Libraries.SelectScene,
                        Location = new Point(37, 74),
                        Parent = this,
                        UseOffSet = true,
                    };


                WarriorButton = new MirButton
                    {
                        HoverIndex = 2427,
                        Index = 2427,
                        Library = Libraries.Prguse,
                        Location = new Point(462, 146),
                        Parent = this,
                        PressedIndex = 2428,
                        Sound = SoundList.ButtonA,
                    };
                WarriorButton.Click += (o, e) =>
                    {
                        _class = MirClass.Warrior;
                        UpdateInterface();
                    };


                WizardButton = new MirButton
                    {
                        HoverIndex = 2430,
                        Index = 2429,
                        Library = Libraries.Prguse,
                        Location = new Point(518, 146),
                        Parent = this,
                        PressedIndex = 2431,
                        Sound = SoundList.ButtonA,
                    };
                WizardButton.Click += (o, e) =>
                    {
                        _class = MirClass.Wizard;
                        UpdateInterface();
                    };


                TaoistButton = new MirButton
                    {
                        HoverIndex = 2433,
                        Index = 2432,
                        Library = Libraries.Prguse,
                        Location = new Point(574, 146),
                        Parent = this,
                        PressedIndex = 2434,
                        Sound = SoundList.ButtonA,
                    };
                TaoistButton.Click += (o, e) =>
                    {
                        _class = MirClass.Taoist;
                        UpdateInterface();
                    };
                
                MaleButton = new MirButton
                    {
                        HoverIndex = 2421,
                        Index = 2421,
                        Library = Libraries.Prguse,
                        Location = new Point(462, 202),
                        Parent = this,
                        PressedIndex = 2422,
                        Sound = SoundList.ButtonA,
                    };
                MaleButton.Click += (o, e) =>
                    {
                        _gender = MirGender.Male;
                        UpdateInterface();
                    };

                FemaleButton = new MirButton
                    {
                        HoverIndex = 2424,
                        Index = 2423,
                        Library = Libraries.Prguse,
                        Location = new Point(518, 202),
                        Parent = this,
                        PressedIndex = 2425,
                        Sound = SoundList.ButtonA,
                    };
                FemaleButton.Click += (o, e) =>
                    {
                        _gender = MirGender.Female;
                        UpdateInterface();
                    };

                Description = new MirLabel
                    {
                        Border = true,
                        Location = new Point(364, 274),
                        Parent = this,
                        Size = new Size(258, 130),
                        Text = WarriorDescription,
                    };
            }

            private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (sender == null) return;
                if (e.KeyChar != (char)Keys.Enter) return;
                e.Handled = true;

                if (AcceptButton.Enabled)
                    AcceptButton.InvokeMouseClick(null);
            }
            private void CharacterNameTextBox_TextChanged(object sender, EventArgs e)
            {
                if (string.IsNullOrEmpty(NameTextBox.Text))
                {
                    AcceptButton.Enabled = false;
                    NameTextBox.Border = false;
                }
                else if (!Reg.IsMatch(NameTextBox.Text))
                {
                    AcceptButton.Enabled = false;
                    NameTextBox.Border = true;
                    NameTextBox.BorderColour = Color.Red;
                }
                else
                {
                    AcceptButton.Enabled = true;
                    NameTextBox.Border = true;
                    NameTextBox.BorderColour = Color.Green;
                }
            }

            private void CreateCharacter()
            {
                AcceptButton.Enabled = false;

                Network.Enqueue(new C.NewCharacter
                    {
                        Name = NameTextBox.Text,
                        Class = _class,
                        Gender = _gender
                    });
            }

            private void UpdateInterface()
            {
                MaleButton.Index = 2420;
                FemaleButton.Index = 2423;

                WarriorButton.Index = 2426;
                WizardButton.Index = 2429;
                TaoistButton.Index = 2432;

                switch (_gender)
                {
                    case MirGender.Male:
                        MaleButton.Index = 2421;
                        break;
                    case MirGender.Female:
                        FemaleButton.Index = 2424;
                        break;
                }

                switch (_class)
                {
                    case MirClass.Warrior:
                        WarriorButton.Index = 2427;
                        Description.Text = WarriorDescription;
                        CharacterDisplay.Index = (byte)_gender == 0 ? 20 : 40; //220 : 500;
                        break;
                    case MirClass.Wizard:
                        WizardButton.Index = 2430;
                        Description.Text = WizardDescription;
                        CharacterDisplay.Index = (byte)_gender == 0 ? 60 : 80; //240 : 520;
                        break;
                    case MirClass.Taoist:
                        TaoistButton.Index = 2433;
                        Description.Text = TaoistDescription;
                        CharacterDisplay.Index = (byte)_gender == 0 ? 100 : 120; //260 : 540;
                        break;
                }

                //CharacterDisplay.Index = ((byte)_class + 1) * 20 + (byte)_gender * 280;
            }
        }
        public sealed class CharacterButton : MirImageControl
        {
            public MirLabel NameLabel, LevelLabel, ClassLabel;
            public bool Selected;
            
            public CharacterButton()
            {
                Index = 44; //45 locked
                Library = Libraries.Prguse;
                Sound = SoundList.ButtonA;

                NameLabel = new MirLabel
                {
                    Location = new Point(107, 9),
                    Parent = this,
                    NotControl = true,
                    Size = new Size(170, 18)
                };

                LevelLabel = new MirLabel
                {
                    Location = new Point(107, 28),
                    Parent = this,
                    NotControl = true,
                    Size = new Size(30, 18)
                };

                ClassLabel = new MirLabel
                {
                    Location = new Point(178, 28),
                    Parent = this,
                    NotControl = true,
                    Size = new Size(100, 18)
                };
            }

            public void Update(SelectInfo info)
            {
                if (info == null)
                {
                    Index = 44;
                    Library = Libraries.Prguse;
                    NameLabel.Text = string.Empty;
                    LevelLabel.Text = string.Empty;
                    ClassLabel.Text = string.Empty;

                    NameLabel.Visible = false;
                    LevelLabel.Visible = false;
                    ClassLabel.Visible = false;

                    return;
                }

                Library = Libraries.Title;

                Index = 660 + (byte) info.Class;

                if (Selected) Index += 5;


                NameLabel.Text = info.Name;
                LevelLabel.Text = info.Level.ToString();
                ClassLabel.Text = info.Class.ToString();
                
                NameLabel.Visible = true;
                LevelLabel.Visible = true;
                ClassLabel.Visible = true;
            }
        }
    }
}
