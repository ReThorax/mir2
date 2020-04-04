using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirSounds;
using System.Drawing;
using System.Windows.Forms;

using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class ItemRentingDialog : MirImageControl
    {
        public static UserItem RentalItem;

        public readonly MirItemCell ItemCell;
        public uint RentalPeriod;

        private readonly MirLabel _nameLabel, _rentalPeriodLabel;
        private readonly MirButton _lockButton, _setRentalPeriodButton, _confirmButton;
        public MirLabel ConfirmLabel, PeriodLabel;
        
        public ItemRentingDialog()
        {
            Index = 268;
            Library = Libraries.GameScene;
            Movable = true;
            Size = new Size(260, 141);
            Location = new Point(Settings.ScreenWidth - Size.Width - Size.Width / 2, Size.Height * 2 + Size.Height / 2 + 15);
            Sort = true;

            // Confirm Button

            _confirmButton = new MirButton
            {
                Index = 228,
                HoverIndex = 229,
                Location = new Point(160, 96),
                Size = new Size(78, 20),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 230,
                Sound = SoundList.ButtonA,
                Enabled = false
            };
            _confirmButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.ConfirmItemRental());
            };

            ConfirmLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = _confirmButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Confirm",
                NotControl = true,
            };

            // Close Button

            var closeButton = new MirButton
            {
                HoverIndex = 186,
                Index = 185,
                Location = new Point(213, 24),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 187,
                Sound = SoundList.ButtonA,
            };
            closeButton.Click += (sender, args) =>
            {
                CancelItemRental();
            };

            // Lock Button

            _lockButton = new MirButton
            {
                Index = 250,
                HoverIndex = 252,
                Location = new Point(210, 65),
                Size = new Size(28, 25),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 251,
                Sound = SoundList.ButtonA,
            };
            _lockButton.Click += (o, e) =>
            {
                if (RentalPeriod < 1 || RentalPeriod > 30)
                    return;

                Network.Enqueue(new C.ItemRentalLockItem());
            };

            // Set Rental Period Button

            _setRentalPeriodButton = new MirButton
            {
                Index = 228,
                HoverIndex = 229,
                Location = new Point(69, 96),
                Size = new Size(78, 20),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 230,
                Sound = SoundList.ButtonA,
            };
            _setRentalPeriodButton.Click += (o, e) =>
            {
                InputRentalPeroid();
            };

            PeriodLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = _setRentalPeriodButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Set Period",
                NotControl = true,
            };

            // Name Label

            _nameLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(30, 22),
                Size = new Size(170, 14),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                NotControl = true,
            };

            // Rental Period Label

            _rentalPeriodLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(70, 70),
                Size = new Size(137, 18),
                DrawFormat = TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                NotControl = true,
            };

            // Item Cell

            ItemCell = new MirItemCell
            {
                BorderColour = Color.Lime,
                GridType = MirGridType.Renting,
                Library = Libraries.Items,
                Parent = this,
                Location = new Point(27, 61),
                ItemSlot = 0,
            };
        }

        private static void CancelItemRental()
        {
            Network.Enqueue(new C.CancelItemRental());
        }

        public void EnableConfirmButton()
        {
            _confirmButton.Enabled = true;
        }

        public void InputRentalPeroid()
        {
            var inputBox = new MirInputBox($"How long would you like to rent {RentalItem.Name} to {GameScene.Scene.GuestItemRentDialog.GuestName} for? (1 to 30 days).");

            inputBox.OKButton.Click += (o1, e1) =>
            {
                if (!uint.TryParse(inputBox.InputTextBox.Text, out RentalPeriod))
                    return;

                if (RentalPeriod < 1 || RentalPeriod > 30)
                    return;

                RefreshInterface();
                inputBox.Dispose();

                Network.Enqueue(new C.ItemRentalPeriod { Days = RentalPeriod });
            };

            inputBox.Show();
        }

        public void RefreshInterface()
        {
            _nameLabel.Text = GameScene.User.Name;
            _rentalPeriodLabel.Text = $"Rental Period: {RentalPeriod} Days";

            GameScene.Scene.GuestItemRentDialog.RefreshInterface();
            GameScene.Scene.GuestItemRentingDialog.RefreshInterface();

            Redraw();
        }

        public void OpenItemRentalDialog()
        {
            GameScene.Scene.InventoryDialog.Show();

            Show();
            RefreshInterface();
            GameScene.Scene.GuestItemRentDialog.Show();
        }

        public void Reset()
        {
            RentalItem = null;
            RentalPeriod = 0;
            _confirmButton.Enabled = false;
            GameScene.User.RentalGoldAmount = 0;
            
            GameScene.Scene.GuestItemRentingDialog.Reset();

            Unlock();
            RefreshInterface();
            Hide();
        }

        public void Lock()
        {
            _lockButton.Index = 253;
            _lockButton.Enabled = false;
            _setRentalPeriodButton.Enabled = false;

            RefreshInterface();
        }

        private void Unlock()
        {
            _lockButton.Index = 250;
            _lockButton.Enabled = true;
            _setRentalPeriodButton.Enabled = true;
        }

        private void Hide()
        {
            Visible = false;
        }

        private void Show()
        {
            Visible = true;
        }
    }

    public sealed class GuestItemRentingDialog : MirImageControl
    {
        public static UserItem GuestLoanItem;

        public uint GuestRentalPeriod;

        private readonly MirLabel _nameLabel, _rentalPeriodLabel;
        private readonly MirButton _lockButton, _setRentalPeriodButton, _confirmButton;
        private string _guestName;
        public MirLabel ConfirmLabel, PeriodLabel;

        private MirItemCell _guestItemCell;
    
        public GuestItemRentingDialog()
        {
            Index = 268;
            Library = Libraries.GameScene;
            Movable = true;
            Size = new Size(260, 141);
            Location = new Point(Settings.ScreenWidth - Size.Width - Size.Width / 2, Size.Height * 2 + Size.Height / 2 + 15);
            Sort = true;

            _lockButton = new MirButton
            {
                Index = 250,
                Location = new Point(210, 65),
                Size = new Size(28, 25),
                Library = Libraries.Prguse,
                Parent = this,
                Enabled = false
            };

            _setRentalPeriodButton = new MirButton
            {
                Index = 228,
                Location = new Point(69, 96),
                Size = new Size(78, 20),
                Library = Libraries.GameScene,
                Parent = this,
                Enabled = false
            };

            PeriodLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = _setRentalPeriodButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Set Period",
                NotControl = true,
            };

            _confirmButton = new MirButton
            {
                Index = 228,
                HoverIndex = 229,
                Location = new Point(160, 96),
                Size = new Size(78, 20),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 230,
                Sound = SoundList.ButtonA,
                Enabled = false
            };

            ConfirmLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = _confirmButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Confirm",
                NotControl = true,
            };

            _nameLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(30, 22),
                Size = new Size(170, 14),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                NotControl = true,
            };

            _rentalPeriodLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(70, 70),
                Size = new Size(137, 18),
                DrawFormat = TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                NotControl = true,
                Text = "Rental Period: 0 Days"
            };

            _guestItemCell = new MirItemCell
            {
                BorderColour = Color.Lime,
                GridType = MirGridType.GuestRenting,
                Library = Libraries.Items,
                Parent = this,
                Location = new Point(27, 61),
                ItemSlot = 0,
            };
        }

        public void RefreshInterface()
        {
            _nameLabel.Text = _guestName;
            _rentalPeriodLabel.Text = $"Rental Period: {GuestRentalPeriod} Days";

            if (GuestLoanItem != null)
                GameScene.Bind(GuestLoanItem);
            
            Redraw();
        }

        public void Reset()
        {
            Unlock();
            GuestLoanItem = null;
            _guestName = string.Empty;

            Hide();
        }

        public void SetGuestName(string name)
        {
            _guestName = name;
        }

        public void Lock()
        {
            _lockButton.Index = 253;

            RefreshInterface();
        }

        private void Unlock()
        {
            _lockButton.Index = 250;
        }

        private void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
        }
    }
}
