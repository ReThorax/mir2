using System;
using System.Drawing;
using System.Windows.Forms;
using Client.MirGraphics;
using Client.MirSounds;

namespace Client.MirControls
{
    public sealed class MirAmountBox : MirImageControl
    {
        public MirLabel TitleLabel, TextLabel, AcceptLabel, CancelLabel;
        public MirButton OKButton, CancelButton, CloseButton;
        public MirTextBox InputTextBox;
        public MirControl ItemImage;
        public int ImageIndex;
        public uint Amount, MinAmount, MaxAmount;

        public MirAmountBox(string title, int image, uint max, uint min = 0, uint defaultAmount = 0)
        {
            ImageIndex = image;
            MaxAmount = max;
            MinAmount = min;
            Amount = max;
            Modal = true;
            Movable = false;

            Index = 246;
            Library = Libraries.GameScene;

            Location = new Point((Settings.ScreenWidth - Size.Width) / 2, (Settings.ScreenHeight - Size.Height) / 2);

            TitleLabel = new MirLabel
            {
                AutoSize = true,
                Location = new Point(73, 67),
                Parent = this,
                NotControl = true,
                Text = title
            };

            CloseButton = new MirButton
            {
                HoverIndex = 186,
                Index = 185,
                Location = new Point(171, 25),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 187,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Dispose();

            ItemImage = new MirControl
            {
                Location = new Point(30, 65),
                Size = new Size(32, 32),
                Parent = this,
            };
            ItemImage.AfterDraw += (o, e) => DrawItem();

            OKButton = new MirButton
            {
                Index = 228,
                HoverIndex = 229,
                PressedIndex = 230,
                Location = new Point(27, 109),
                Library = Libraries.GameScene,
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            OKButton.Click += (o, e) => Dispose();

            AcceptLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = OKButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Accept",
                NotControl = true,
            };

            CancelButton = new MirButton
            {
                Index = 228,
                HoverIndex = 229,
                PressedIndex = 230,
                Location = new Point(113, 109),
                Library = Libraries.GameScene,
                Parent = this,
                Sound = SoundList.ButtonA,
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

            InputTextBox = new MirTextBox
            {
                Parent = this,
                Border = true,
                BorderColour = Color.Lime,
                Location = new Point(73, 83),
                Size = new Size(117, 19),
            };
            InputTextBox.SetFocus();
            InputTextBox.TextBox.KeyPress += MirInputBox_KeyPress;
            InputTextBox.TextBox.TextChanged += TextBox_TextChanged;
            InputTextBox.Text = (defaultAmount > 0 && defaultAmount <= MaxAmount) ? defaultAmount.ToString() : MaxAmount.ToString();
            InputTextBox.TextBox.SelectionStart = 0;
            InputTextBox.TextBox.SelectionLength = InputTextBox.Text.Length;

        }
        public MirAmountBox(string title, int image, string message)
        {
            ImageIndex = image;

            Modal = true;
            Movable = false;

            Index = 246;
            Library = Libraries.GameScene;

            Location = new Point((800 - Size.Width) / 2, (600 - Size.Height) / 2);



            TitleLabel = new MirLabel
            {
                AutoSize = true,
                Location = new Point(73, 67),
                Parent = this,
                NotControl = true,
                Text = title
            };

            TextLabel = new MirLabel
            {
                AutoSize = true,
                Location = new Point(73, 83),
                ForeColour = Color.Yellow,
                Parent = this,
                NotControl = true,
                Text = message
            };

            CloseButton = new MirButton
            {
                HoverIndex = 186,
                Index = 185,
                Location = new Point(171, 25),
                Library = Libraries.GameScene,
                Parent = this,
                PressedIndex = 187,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Dispose();

            ItemImage = new MirControl
            {
                Location = new Point(30, 65),
                Size = new Size(32, 32),
                Parent = this,
            };
            ItemImage.AfterDraw += (o, e) => DrawItem();

            OKButton = new MirButton
            {
                Index = 228,
                HoverIndex = 229,
                PressedIndex = 230,
                Location = new Point(27, 109),
                Library = Libraries.GameScene,
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            OKButton.Click += (o, e) => Dispose();

            AcceptLabel = new MirLabel
            {
                Location = new Point(0, -2),
                Parent = OKButton,
                Size = new Size(78, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "Accept",
                NotControl = true,
            };

            CancelButton = new MirButton
            {
                Index = 228,
                HoverIndex = 229,
                PressedIndex = 230,
                Location = new Point(113, 109),
                Library = Libraries.GameScene,
                Parent = this,
                Sound = SoundList.ButtonA,
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
        }

        void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (uint.TryParse(InputTextBox.Text, out Amount) && Amount >= MinAmount)
            {
                InputTextBox.BorderColour = Color.Lime;

                OKButton.Visible = true;
                if (Amount > MaxAmount)
                {
                    Amount = MaxAmount;
                    InputTextBox.Text = MaxAmount.ToString();
                    InputTextBox.TextBox.SelectionStart = InputTextBox.Text.Length;
                }

                if (Amount == MaxAmount)
                    InputTextBox.BorderColour = Color.Orange;
            }
            else
            {
                InputTextBox.BorderColour = Color.Red;
                OKButton.Visible = false;
            }
        }

        void MirInputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                if (OKButton != null && !OKButton.IsDisposed)
                    OKButton.InvokeMouseClick(EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                if (CancelButton != null && !CancelButton.IsDisposed)
                    CancelButton.InvokeMouseClick(EventArgs.Empty);
                e.Handled = true;
            }
        }

        void DrawItem()
        {
            int x = ItemImage.DisplayLocation.X, y = ItemImage.DisplayLocation.Y;

            Size s = Libraries.Items.GetTrueSize(ImageIndex);

            x += (ItemImage.Size.Width - s.Width) / 2;
            y += (ItemImage.Size.Height - s.Height) / 2;

            Libraries.Items.Draw(ImageIndex, x, y);
        }

        public void Show()
        {
            if (Parent != null) return;

            Parent = MirScene.ActiveScene;

            Highlight();

            for (int i = 0; i < Program.Form.Controls.Count; i++)
            {
                TextBox T = Program.Form.Controls[i] as TextBox;
                if (T != null && T.Tag != null && T.Tag != null)
                    ((MirTextBox)T.Tag).DialogChanged();
            }

            /*
            CMain.Shift = false;
            CMain.Ctrl = false;
            CMain.Alt = false;

            Parent = MirScene.ActiveScene;
            Activate();
            Highlight();

            for (int i = 0; i < Main.This.Controls.Count; i++)
            {
                TextBox T = (TextBox)Main.This.Controls[i];
                if (T != null && T.Tag != null && T.Tag != null)
                    ((MirTextBox)T.Tag).DialogChanged();
            }*/
        }
        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            e.Handled = true;
        }
        public override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            e.Handled = true;
        }
        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyChar == (char)Keys.Escape)
                CancelButton.InvokeMouseClick(EventArgs.Empty);
            else if (e.KeyChar == (char)Keys.Enter)
                OKButton.InvokeMouseClick(EventArgs.Empty);
            e.Handled = true;
        }

        #region Disposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            for (int i = 0; i < Program.Form.Controls.Count; i++)
            {
                TextBox T = (TextBox)Program.Form.Controls[i];
                if (T != null && T.Tag != null && T.Tag != null)
                    ((MirTextBox)T.Tag).DialogChanged();
            }
        }

        #endregion
    }
}
