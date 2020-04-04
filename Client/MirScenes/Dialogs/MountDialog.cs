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

namespace Client.MirScenes.Dialogs
{
    public sealed class MountDialog : MirImageControl
    {
        public MirLabel MountName, MountLoyalty;
        public MirButton CloseButton, MountButton;
        private MirAnimatedControl MountImage;
        public MirItemCell[] Grid;

        public int StartIndex = 0;

        public MountDialog()
        {
            Index = 260;
            Library = Libraries.GameScene;
            Movable = true;
            Sort = true;
            Location = new Point(0, 0);
            BeforeDraw += MountDialog_BeforeDraw;

            MountName = new MirLabel
            {
                Location = new Point(104, 275),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Parent = this,
                NotControl = true,
            };
            MountLoyalty = new MirLabel
            {
                Location = new Point(227, 275),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
                Parent = this,
                NotControl = true,
            };

            MountButton = new MirButton
            {
                Library = Libraries.Prguse,
                Parent = this,
                Sound = SoundList.ButtonA,
                Location = new Point(262, 70)
            };
            MountButton.Click += (o, e) =>
            {
                if (CanRide())
                {
                    Ride();
                }
            };

            CloseButton = new MirButton
            {
                HoverIndex = 186,
                Index = 185,
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 187,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            MountImage = new MirAnimatedControl
            {
                Animated = false,
                AnimationCount = 16,
                AnimationDelay = 100,
                Index = 0,
                Library = Libraries.Prguse,
                Loop = true,
                Parent = this,
                NotControl = true,
                UseOffSet = true
            };

            Grid = new MirItemCell[Enum.GetNames(typeof(MountSlot)).Length];

            Grid[(int)MountSlot.Reins] = new MirItemCell
            {
                ItemSlot = (int)MountSlot.Reins,
                GridType = MirGridType.Mount,
                Parent = this,

            };
            Grid[(int)MountSlot.Bells] = new MirItemCell
            {
                ItemSlot = (int)MountSlot.Bells,
                GridType = MirGridType.Mount,
                Parent = this,
            };

            Grid[(int)MountSlot.Saddle] = new MirItemCell
            {
                ItemSlot = (int)MountSlot.Saddle,
                GridType = MirGridType.Mount,
                Parent = this,
            };

            Grid[(int)MountSlot.Ribbon] = new MirItemCell
            {
                ItemSlot = (int)MountSlot.Ribbon,
                GridType = MirGridType.Mount,
                Parent = this,
            };


            Grid[(int)MountSlot.Mask] = new MirItemCell
            {
                ItemSlot = (int)MountSlot.Mask,
                GridType = MirGridType.Mount,
                Parent = this,
            };

        }

        void MountDialog_BeforeDraw(object sender, EventArgs e)
        {
            RefreshDialog();
        }

        public void RefreshDialog()
        {
            SwitchType();
            DrawMountAnimation();
        }

        private void SwitchType()
        {
            UserItem MountItem = GameScene.User.Equipment[(int)EquipmentSlot.Mount];
            UserItem[] MountSlots = null;

            if (MountItem != null)
            {
                MountSlots = MountItem.Slots;
            }

            if (MountSlots == null) return;

            switch (MountSlots.Length)
            {
                case 4:
                    Index = 264;
                    StartIndex = 1170;
                    MountName.Size = new Size(122, 16);
                    MountLoyalty.Size = new Size(121, 16);
                    MountImage.Location = new Point(201, 206);
                    MountButton.Index = 164;
                    MountButton.HoverIndex = 165;
                    MountButton.PressedIndex = 166;
                    MountButton.Location = new Point(29, 63);
                    CloseButton.Location = new Point(404, 24);
                    Grid[(int)MountSlot.Mask].Visible = false;

                    Grid[(int)MountSlot.Reins].Location = new Point(132, 307);
                    Grid[(int)MountSlot.Ribbon].Location = new Point(184, 307);
                    Grid[(int)MountSlot.Bells].Location = new Point(236, 307);
                    Grid[(int)MountSlot.Saddle].Location = new Point(288, 307);


                    Grid[(int)MountSlot.Mask].Location = new Point(158, 307);
                    break;
                case 5:
                    Index = 260;
                    StartIndex = 1330;
                    MountName.Size = new Size(122, 16);
                    MountLoyalty.Size = new Size(121, 16);
                    MountImage.Location = new Point(78, 32);
                    MountButton.Index = 155;
                    MountButton.HoverIndex = 156;
                    MountButton.PressedIndex = 157;
                    MountButton.Location = new Point(29, 63);
                    CloseButton.Location = new Point(404, 24);
                    Grid[(int)MountSlot.Mask].Visible = true;

                    Grid[(int)MountSlot.Reins].Location = new Point(106, 307);
                    Grid[(int)MountSlot.Ribbon].Location = new Point(210, 307);
                    Grid[(int)MountSlot.Bells].Location = new Point(262, 307);
                    Grid[(int)MountSlot.Saddle].Location = new Point(314, 307);
                    Grid[(int)MountSlot.Mask].Location = new Point(158, 307);
                    break;
            }
        }

        private void DrawMountAnimation()
        {
            if (GameScene.User.MountType < 0)
            {
                MountImage.Index = 0;
                MountImage.Animated = false;
            }
            else
            {
                MountImage.Index = StartIndex + (GameScene.User.MountType * 20);
                MountImage.Animated = true;

                UserItem item = MapObject.User.Equipment[(int)EquipmentSlot.Mount];

                if (item != null)
                {
                    MountName.Text = item.FriendlyName;
                    MountLoyalty.Text = string.Format("{0} / {1} Loyalty", item.CurrentDura, item.MaxDura);
                }
            }

        }

        public bool CanRide()
        {
            if (GameScene.User.MountType < 0 || GameScene.User.MountTime + 500 > CMain.Time) return false;
            if (GameScene.User.CurrentAction != MirAction.Standing && GameScene.User.CurrentAction != MirAction.MountStanding) return false;

            return true;
        }

        public void Ride()
        {
            Network.Enqueue(new C.Chat { Message = "@ride" });
        }


        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }
        public void Show()
        {
            if (Visible) return;
            if (GameScene.User.MountType < 0)
            {
                MirMessageBox messageBox = new MirMessageBox("You do not own a mount.", MirMessageBoxButtons.OK);
                messageBox.Show();
                return;
            }

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
}
