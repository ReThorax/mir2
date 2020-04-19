using System;
using System.Drawing;
using System.Windows.Forms;
using Client.MirGraphics;

namespace Client.MirControls
{
    public enum MirMessageBoxButtons { OK, OKCancel, YesNo, YesNoCancel, Cancel }

    public sealed class MirMessageBox : MirImageControl
    {
        public MirLabel Label, OKLabel, YesLabel, NoLabel, CancelLabel;
        public MirButton OKButton, CancelButton, NoButton, YesButton;
        public MirMessageBoxButtons Buttons;
        public bool AllowKeyPress = true;

        public MirMessageBox(string message, MirMessageBoxButtons b = MirMessageBoxButtons.OK, bool allowKeys = true)
        {
            DrawImage = true;
            ForeColour = Color.White;
            Buttons = b;
            Modal = true;
            Movable = false;
            AllowKeyPress = allowKeys;

            Index = 88;
            Library = Libraries.GameScene;

            Location = new Point((Settings.ScreenWidth - Size.Width) / 2, (Settings.ScreenHeight - Size.Height) / 2);


            Label = new MirLabel
            {
                AutoSize = false,
                // DrawFormat = StringFormatFlags.FitBlackBox,
                Location = new Point(35, 70),
                Size = new Size(280, 50),
                Parent = this,
                Text = message
            };


            switch (Buttons)
            {
                #region Region "OK"
                case MirMessageBoxButtons.OK:
                    OKButton = new MirButton
                    {
                        HoverIndex = 229,
                        Index = 228,
                        Library = Libraries.GameScene,
                        Location = new Point(242, 134),
                        Parent = this,
                        PressedIndex = 230
                    };
                    OKButton.Click += (o, e) => Dispose();

                    OKLabel = new MirLabel
                    {
                        Location = new Point(0, -2),
                        Parent = OKButton,
                        NotControl = true,
                        Size = new Size(78, 20),
                        Text = "Ok",
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    };

                    break;

                #endregion

                #region Region "OK"/"Cancel"
                case MirMessageBoxButtons.OKCancel:
                    OKButton = new MirButton
                    {
                        HoverIndex = 229,
                        Index = 228,
                        Library = Libraries.GameScene,
                        Location = new Point(156, 134),
                        Parent = this,
                        PressedIndex = 230
                    };
                    OKButton.Click += (o, e) => Dispose();

                    OKLabel = new MirLabel
                    {
                        Location = new Point(0, -2),
                        Parent = OKButton,
                        NotControl = true,
                        Size = new Size(78, 20),
                        Text = "Ok",
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    };


                    CancelButton = new MirButton
                    {
                        HoverIndex = 229,
                        Index = 228,
                        Library = Libraries.GameScene,
                        Location = new Point(242, 134),
                        Parent = this,
                        PressedIndex = 230
                    };
                    CancelButton.Click += (o, e) => Dispose();

                    CancelLabel = new MirLabel
                    {
                        Location = new Point(0, -2),
                        Parent = CancelButton,
                        NotControl = true,
                        Size = new Size(78, 20),
                        Text = "Cancel",
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    };

                    break;

                #endregion

                #region Region "YES"/"NO"
                case MirMessageBoxButtons.YesNo:
                    YesButton = new MirButton
                    {
                        HoverIndex = 229,
                        Index = 228,
                        Library = Libraries.GameScene,
                        Location = new Point(156, 134),
                        Parent = this,
                        PressedIndex = 230
                    };
                    YesButton.Click += (o, e) => Dispose();

                    YesLabel = new MirLabel
                    {
                        Location = new Point(0, -2),
                        Parent = YesButton,
                        NotControl = true,
                        Size = new Size(78, 20),
                        Text = "Yes",
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    };


                    NoButton = new MirButton
                    {
                        HoverIndex = 229,
                        Index = 228,
                        Library = Libraries.GameScene,
                        Location = new Point(242, 134),
                        Parent = this,
                        PressedIndex = 230
                    };
                    NoButton.Click += (o, e) => Dispose();

                    NoLabel = new MirLabel
                    {
                        Location = new Point(0, -2),
                        Parent = NoButton,
                        NotControl = true,
                        Size = new Size(78, 20),
                        Text = "No",
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    };

                    break;
                #endregion

                #region Region "YES"/"NO"/"Cancel"
                case MirMessageBoxButtons.YesNoCancel:
                    YesButton = new MirButton
                    {
                        HoverIndex = 229,
                        Index = 228,
                        Library = Libraries.GameScene,
                        Location = new Point(97, 103),
                        Parent = this,
                        PressedIndex = 230
                    };
                    YesButton.Click += (o, e) => Dispose();

                    YesLabel = new MirLabel
                    {
                        Location = new Point(0, -2),
                        Parent = YesButton,
                        NotControl = true,
                        Size = new Size(78, 20),
                        Text = "Yes",
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    };


                    NoButton = new MirButton
                    {
                        HoverIndex = 229,
                        Index = 228,
                        Library = Libraries.GameScene,
                        Location = new Point(156, 134),
                        Parent = this,
                        PressedIndex = 230
                    };
                    NoButton.Click += (o, e) => Dispose();

                    NoLabel = new MirLabel
                    {
                        Location = new Point(0, -2),
                        Parent = NoButton,
                        NotControl = true,
                        Size = new Size(78, 20),
                        Text = "No",
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    };


                    CancelButton = new MirButton
                    {
                        HoverIndex = 229,
                        Index = 228,
                        Library = Libraries.GameScene,
                        Location = new Point(242, 134),
                        Parent = this,
                        PressedIndex = 230
                    };
                    CancelButton.Click += (o, e) => Dispose();

                    CancelLabel = new MirLabel
                    {
                        Location = new Point(0, -2),
                        Parent = CancelButton,
                        NotControl = true,
                        Size = new Size(78, 20),
                        Text = "Cancel",
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    };

                    break;

                #endregion

                #region Region "Cancel"
                case MirMessageBoxButtons.Cancel:
                    CancelButton = new MirButton
                    {
                        HoverIndex = 229,
                        Index = 228,
                        Library = Libraries.GameScene,
                        Location = new Point(242, 134),
                        Parent = this,
                        PressedIndex = 230
                    };
                    CancelButton.Click += (o, e) => Dispose();

                    CancelLabel = new MirLabel
                    {
                        Location = new Point(0, -2),
                        Parent = CancelButton,
                        NotControl = true,
                        Size = new Size(78, 20),
                        Text = "Cancel",
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    };

                    break;

                    #endregion
            }
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


        public override void OnKeyDown(KeyEventArgs e)
        {
            if (AllowKeyPress)
            {
                base.OnKeyDown(e);
                e.Handled = true;
            }
        }
        public override void OnKeyUp(KeyEventArgs e)
        {
            if (AllowKeyPress)
            {
                base.OnKeyUp(e);
                e.Handled = true;
            }
        }
        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (AllowKeyPress)
            {
                if (e.KeyChar == (char)Keys.Escape)
                {
                    switch (Buttons)
                    {
                        case MirMessageBoxButtons.OK:
                            if (OKButton != null && !OKButton.IsDisposed) OKButton.InvokeMouseClick(null);
                            break;
                        case MirMessageBoxButtons.OKCancel:
                        case MirMessageBoxButtons.YesNoCancel:
                            if (CancelButton != null && !CancelButton.IsDisposed) CancelButton.InvokeMouseClick(null);
                            break;
                        case MirMessageBoxButtons.YesNo:
                            if (NoButton != null && !NoButton.IsDisposed) NoButton.InvokeMouseClick(null);
                            break;
                    }
                }

                else if (e.KeyChar == (char)Keys.Enter)
                {
                    switch (Buttons)
                    {
                        case MirMessageBoxButtons.OK:
                        case MirMessageBoxButtons.OKCancel:
                            if (OKButton != null && !OKButton.IsDisposed) OKButton.InvokeMouseClick(null);
                            break;
                        case MirMessageBoxButtons.YesNoCancel:
                        case MirMessageBoxButtons.YesNo:
                            if (YesButton != null && !YesButton.IsDisposed) YesButton.InvokeMouseClick(null);
                            break;

                    }
                }
                e.Handled = true;
            }
        }


        public static void Show(string message, bool close = false)
        {
            MirMessageBox box = new MirMessageBox(message);

            if (close) box.OKButton.Click += (o, e) => Program.Form.Close();

            box.Show();
        }

        #region Disposable

        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);

            if (!disposing) return;

            Label = null;
            OKButton = null;
            CancelButton = null;
            NoButton = null;
            YesButton = null;
            Buttons = 0;

            for (int i = 0; i < Program.Form.Controls.Count; i++)
            {
                TextBox T = (TextBox)Program.Form.Controls[i];
                if (T != null && T.Tag != null)
                    ((MirTextBox)T.Tag).DialogChanged();
            }
        }

        #endregion
    }
}
