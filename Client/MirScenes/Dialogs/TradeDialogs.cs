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
    public sealed class TradeDialog : MirImageControl
    {
        public MirItemCell[] Grid;
        public MirLabel NameLabel, GoldLabel, ConfirmLabel;
        public MirButton ConfirmButton, CloseButton;

        public TradeDialog()
        {
            Index = 98;
            Library = Libraries.GameScene;
            Movable = true;
            Size = new Size(244, 200);
            Location = new Point((Settings.ScreenWidth / 2) - Size.Width - 10, Settings.ScreenHeight - 350);
            Sort = true;

            #region Buttons
            ConfirmButton = new MirButton
            {
                Index = 228,
                HoverIndex = 229,
                PressedIndex = 230,
                Location = new Point(141, 140),
                Library = Libraries.GameScene,
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            ConfirmButton.Click += (o, e) => 
            {
                ChangeLockState(!GameScene.User.TradeLocked);
                Network.Enqueue(new C.TradeConfirm { Locked = GameScene.User.TradeLocked });
            };

            ConfirmLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = ConfirmButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Confirm",
                NotControl = true,
            };

            CloseButton = new MirButton
            {
                HoverIndex = 186,
                Index = 185,
                Location = new Point(197, 24),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 187,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) =>
            {
                Hide();
                GameScene.Scene.GuestTradeDialog.Hide();
                TradeCancel();
            };

            #endregion

            #region Host labels
            NameLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(21, 23),
                Size = new Size(170, 23),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                NotControl = true,
            };

            GoldLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Font = new Font(Settings.FontName, 8F),
                Location = new Point(28, 143),
                Parent = this,
                Size = new Size(90, 14),
                Sound = SoundList.Gold,
            };
            GoldLabel.Click += (o, e) =>
            {
                if (GameScene.SelectedCell == null && GameScene.Gold > 0)
                {
                    MirAmountBox amountBox = new MirAmountBox("Trade Amount:", 116, GameScene.Gold);

                    amountBox.OKButton.Click += (c, a) =>
                    {
                        if (amountBox.Amount > 0)
                        {
                            GameScene.User.TradeGoldAmount += amountBox.Amount;
                            Network.Enqueue(new C.TradeGold { Amount = amountBox.Amount });

                            RefreshInterface();
                        }
                    };

                    amountBox.Show();
                    GameScene.PickedUpGold = false;
                }
            };
            #endregion

            #region Grids
            Grid = new MirItemCell[5 * 2];

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Grid[2 * x + y] = new MirItemCell
                    {
                        ItemSlot = 2 * x + y,
                        GridType = MirGridType.Trade,
                        Parent = this,
                        Location = new Point(x * 36 + 32 + x, y * 36 + 63 + y),
                    };
                }
            }
            #endregion
        }

        public void ChangeLockState(bool lockState, bool cancelled = false)
        {
            GameScene.User.TradeLocked = lockState;

            if (GameScene.User.TradeLocked)
            {
                ConfirmButton.Index = 229;
            }
            else
            {
                ConfirmButton.Index = 228;
            }

            //if (!cancelled)
            //{
            //    //Send lock info to server
            //    Network.Enqueue(new C.TradeConfirm { Locked = lockState });
            //}
        }

        public void RefreshInterface()
        {
            NameLabel.Text = GameScene.User.Name;
            GoldLabel.Text = GameScene.User.TradeGoldAmount.ToString("###,###,##0");

            GameScene.Scene.GuestTradeDialog.RefreshInterface();

            Redraw();
        }

        public void TradeAccept()
        {
            GameScene.Scene.InventoryDialog.Location = new Point(Settings.ScreenWidth - GameScene.Scene.InventoryDialog.Size.Width, 0);
            GameScene.Scene.InventoryDialog.Show();

            RefreshInterface();

            Show();
            GameScene.Scene.GuestTradeDialog.Show();
        }

        public void TradeReset()
        {
            GameScene.Scene.GuestTradeDialog.TradeReset();

            for (int i = 0; i < GameScene.User.Trade.Length; i++)
                GameScene.User.Trade[i] = null;

            GameScene.User.TradeGoldAmount = 0;
            ChangeLockState(false, true);

            RefreshInterface();

            Hide();
            GameScene.Scene.GuestTradeDialog.Hide();
        }

        public void TradeCancel()
        {
            Network.Enqueue(new C.TradeCancel());
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
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
    public sealed class GuestTradeDialog : MirImageControl
    {
        public MirItemCell[] GuestGrid;
        public static UserItem[] GuestItems = new UserItem[10];
        public string GuestName;
        public uint GuestGold;

        public MirLabel GuestNameLabel, GuestGoldLabel;

        public MirButton ConfirmButton;

        public GuestTradeDialog()
        {
            Index = 98;
            Library = Libraries.GameScene;
            Movable = true;
            Size = new Size(244, 200);
            Location = new Point((Settings.ScreenWidth / 2) + 10, Settings.ScreenHeight - 350);
            Sort = true;

            #region Host labels
            GuestNameLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(21, 23),
                Size = new Size(170, 23),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                NotControl = true,
            };

            GuestGoldLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Font = new Font(Settings.FontName, 8F),
                Location = new Point(28, 143),
                Parent = this,
                Size = new Size(90, 14),
                Sound = SoundList.Gold,
            };
            #endregion

            #region Grids
            GuestGrid = new MirItemCell[5 * 2];

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    GuestGrid[2 * x + y] = new MirItemCell
                    {
                        ItemSlot = 2 * x + y,
                        GridType = MirGridType.GuestTrade,
                        Parent = this,
                        Location = new Point(x * 36 + 32 + x, y * 36 + 63 + y),
                    };
                }
            }
            #endregion
        }

        public void RefreshInterface()
        {
            GuestNameLabel.Text = GuestName;
            GuestGoldLabel.Text = string.Format("{0:###,###,##0}", GuestGold);

            for (int i = 0; i < GuestItems.Length; i++)
            {
                if (GuestItems[i] == null) continue;
                GameScene.Bind(GuestItems[i]);
            }

            Redraw();
        }

        public void TradeReset()
        {
            for (int i = 0; i < GuestItems.Length; i++)
                GuestItems[i] = null;

            GuestName = string.Empty;
            GuestGold = 0;

            Hide();
        }


        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
        }
    }
}
