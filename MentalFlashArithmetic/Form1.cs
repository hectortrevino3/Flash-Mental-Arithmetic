using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlashMentalArithmetic
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer fontSizeTimer;
        private Button easyButton;
        private Button mediumButton;
        private Button hardButton;
        private Label titleLabel;
        private TableLayoutPanel buttonPanel;

        public Form1()
        {
            InitializeComponent();
            InitializeMenuComponents();
            InitializeFontSizeTimer();
        }

        private void InitializeMenuComponents()
        {
            titleLabel = new Label
            {
                Text = "Flash Mental Arithmetic",
                ForeColor = Color.Green,
                BackColor = Color.Black,
                Font = new Font("Arial", 24, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 150
            };

            buttonPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Black,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
            buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
            buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));

            easyButton = new Button
            {
                Text = "Easy",
                BackColor = Color.Black,
                ForeColor = Color.Green,
                Font = new Font("Arial", 14, FontStyle.Bold),
                Padding = new Padding(3),
                Dock = DockStyle.Fill
            };
            easyButton.Click += EasyButton_Click;

            mediumButton = new Button
            {
                Text = "Medium",
                BackColor = Color.Black,
                ForeColor = Color.Green,
                Font = new Font("Arial", 14, FontStyle.Bold),
                Padding = new Padding(3),
                Dock = DockStyle.Fill
            };
            mediumButton.Click += MediumButton_Click;

            hardButton = new Button
            {
                Text = "Hard",
                BackColor = Color.Black,
                ForeColor = Color.Green,
                Font = new Font("Arial", 14, FontStyle.Bold),
                Padding = new Padding(3),
                Dock = DockStyle.Fill
            };
            hardButton.Click += HardButton_Click;

            buttonPanel.Controls.Add(easyButton, 0, 1);
            buttonPanel.Controls.Add(mediumButton, 0, 2);
            buttonPanel.Controls.Add(hardButton, 0, 3);

            Controls.Add(titleLabel);
            Controls.Add(buttonPanel);

            this.Layout += (sender, e) =>
            {
                buttonPanel.Left = (this.ClientSize.Width - buttonPanel.Width) / 2;
                buttonPanel.Top = (this.ClientSize.Height - buttonPanel.Height) / 2;
            };
        }

        private void InitializeFontSizeTimer()
        {
            fontSizeTimer = new System.Windows.Forms.Timer
            {
                Interval = 50
            };
            fontSizeTimer.Tick += FontSizeTimer_Tick;
            fontSizeTimer.Start();
        }

        private void FontSizeTimer_Tick(object sender, EventArgs e)
        {
            int buttonWidth = Math.Max(100, this.ClientSize.Height / 12);
            int buttonHeight = Math.Max(60, this.ClientSize.Height / 16);
            int newFontSize = Math.Max(24, this.ClientSize.Height / 10);
            titleLabel.Font = new Font("Arial", newFontSize, FontStyle.Bold);

            foreach (var button in new[] { easyButton, mediumButton, hardButton })
            {
                button.Font = new Font("Arial", Math.Max(12, newFontSize / 2), FontStyle.Bold);
            }
        }

        private void EasyButton_Click(object sender, EventArgs e)
        {
            OpenGameForm("Easy");
        }

        private void MediumButton_Click(object sender, EventArgs e)
        {
            OpenGameForm("Medium");
        }

        private void HardButton_Click(object sender, EventArgs e)
        {
            OpenGameForm("Hard");
        }

        private void OpenGameForm(string difficulty)
        {
            var gameForm = new GameForm(difficulty, this);
            gameForm.Show();
            this.Hide();
        }
    }
}
