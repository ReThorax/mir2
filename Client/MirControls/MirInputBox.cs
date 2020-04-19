using System;
using System.Drawing;
using System.Windows.Forms;
using Client.MirGraphics;
using Client.MirSounds;

namespace Client.MirControls
{
    public sealed class MirInputBox : MirImageControl
    {
        public readonly MirLabel CaptionLabel, OKLabel, CancelLabel;
        public readonly MirButton OKButton, CancelButton;
        public readonly MirTextBox InputTextBox;


        public MirInputBox(string message)
        {
            Modal = true;
            Movable = false;

            Index = 89;
            Library = Libraries.GameScene;

            Location = new Point((Settings.ScreenWidth - Size.Width) / 2, (Settings.ScreenHeight - Size.Height) / 2);

            CaptionLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.WordBreak,
                Location = new Point(26, 63),
                Size = new Size(292, 61),
                Parent = this,
                Text = message,
            };

            InputTextBox = new MirTextBox
            {
                Parent = this,
                Border = true,
                BorderColour = Color.Lime,
                Location = new Point(26, 133),
                Size = new Size(296, 17),
                MaxLength = 50,
            };
            InputTextBox.SetFocus();
            InputTextBox.TextBox.KeyPress += MirInputBox_KeyPress;

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
            CancelButton.Click += DisposeDialog;

            CancelLabel = new MirLabel
            {
                Size = new Size(78, 20),
                Parent = CancelButton,
                Location = new Point(0, -2),
                NotControl = true,
                Text = "Cancel",
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };
        }

        void MirInputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
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
        void DisposeDialog(object sender, EventArgs e)
        {
            Dispose();
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
            {
                if (CancelButton != null && !CancelButton.IsDisposed)
                    CancelButton.InvokeMouseClick(EventArgs.Empty);
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                if (OKButton != null && !OKButton.IsDisposed)
                    OKButton.InvokeMouseClick(EventArgs.Empty);

            }
            e.Handled = true;
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
