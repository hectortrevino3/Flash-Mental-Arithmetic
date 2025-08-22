using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashMentalArithmetic
{
    public partial class GameForm : Form
    {
        private string difficulty;
        private Label countdownLabel;
        private TextBox answerBox;
        private Button submitButton;
        private int currentQuestion;
        private int totalPoints;
        private int correctAnswersCount;
        private int correctAnswers = 0;
        private int[] questionPoints = { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19 };
        private int[] numbersToDisplay;
        private int correctAnswer;
        private HashSet<int> displayedNumbers;
        private Form1 parentForm;

        public GameForm(string difficulty,Form1 parentForm)
        {
            InitializeComponent();
            this.difficulty = difficulty;
            this.parentForm = parentForm;
            this.FormClosing += GameFormClosing;
            InitializeGameComponents();
            StartGame();
        }
        private void GameFormClosing(object sender, FormClosingEventArgs e)
        {
            // Close Form1 when GameForm is closed
            parentForm.Close();
        }
        private void InitializeGameComponents()
        {
            countdownLabel = new Label
            {
                Text = "",
                ForeColor = Color.Green,
                BackColor = Color.Black,
                Font = new Font("Arial", 72, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            answerBox = new TextBox
            {
                Font = new Font("Arial", 24, FontStyle.Bold),
                Visible = false,
                Dock = DockStyle.Bottom,
                Height = 60
            };

            submitButton = new Button
            {
                Text = "Submit",
                BackColor = Color.Black,
                ForeColor = Color.Green,
                Font = new Font("Arial", 24, FontStyle.Bold),
                Dock = DockStyle.Bottom,
                Height = 60,
                Visible = false
            };
            submitButton.Click += SubmitButton_Click;

            Controls.Add(countdownLabel);
            Controls.Add(answerBox);
            Controls.Add(submitButton);

            this.BackColor = Color.Black;
            this.WindowState = FormWindowState.Maximized;
        }

        private async void StartGame()
        {
            displayedNumbers = new HashSet<int>();

            await ShowCountdown();

            for (int i = 0; i < 10; i++)
            {
                currentQuestion = i;
                totalPoints += questionPoints[i];
                int numDigits = GetNumberOfDigits(i);
                double displayTime = GetDisplayTime(i);

                await DisplayNumbers(numDigits, displayTime);
                await GetUserAnswer();
            }

            ShowFinalResults();

            ReturnToMainMenu();
        }

        private int GetNumberOfDigits(int questionIndex)
        {
            int[] easyDigits = { 3, 3, 6, 6, 6, 8, 12, 15, 15, 15 };
            int[] mediumDigits = { 3, 3, 6, 6, 6, 8, 12, 15, 15, 15 };
            int[] hardDigits = { 3, 3, 6, 6, 6, 8, 12, 15, 15, 15 };

            if (difficulty == "Easy") return easyDigits[questionIndex];
            if (difficulty == "Medium") return mediumDigits[questionIndex];
            return hardDigits[questionIndex];
        }

        private double GetDisplayTime(int questionIndex)
        {
            double[] easyTimes = { 5, 5, 4.5, 3.5, 2.5, 5, 4.5, 3.5, 2.5, 1.6 };
            double[] mediumTimes = { 5, 5, 4.5, 3.5, 2.5, 5, 4.5, 3.5, 2.5, 1.6 };
            double[] hardTimes = { 5, 5, 4.5, 3.5, 2.5, 5, 4.5, 3.5, 2.5, 1.6 };

            if (difficulty == "Easy") return easyTimes[questionIndex];
            if (difficulty == "Medium") return mediumTimes[questionIndex];
            return hardTimes[questionIndex];
        }

        private async Task ShowCountdown()
        {
            for (int i = 3; i > 0; i--)
            {
                countdownLabel.Text = $"Countdown\n{i}";
                Controls.Add(countdownLabel);
                await Task.Delay(1000);
                Controls.Remove(countdownLabel);
            }
        }

        private async Task DisplayNumbers(int numDigits, double displayTime)
        {
            double timePerDigit = displayTime / numDigits * 1000; // Convert to milliseconds

            Random random = new Random();
            numbersToDisplay = new int[numDigits];
            correctAnswer = 0;

            // Generate unique numbers based on difficulty
            if (difficulty == "Easy")
            {
                // Handle the case where numDigits is larger than the range of unique single-digit numbers
                List<int> availableNumbers = Enumerable.Range(1, 9).ToList();
                availableNumbers = availableNumbers.OrderBy(x => random.Next()).ToList(); // Shuffle

                for (int i = 0; i < numDigits; i++)
                {
                    numbersToDisplay[i] = availableNumbers[i % availableNumbers.Count];
                    correctAnswer += numbersToDisplay[i];
                }
            }
            else
            {
                displayedNumbers.Clear();
                for (int i = 0; i < numDigits; i++)
                {
                    int newNumber;
                    do
                    {
                        newNumber = GenerateUniqueNumber();
                    } while (displayedNumbers.Contains(newNumber));

                    displayedNumbers.Add(newNumber);
                    numbersToDisplay[i] = newNumber;
                    correctAnswer += newNumber;
                }
            }

            // Display the numbers
            foreach (int number in numbersToDisplay)
            {
                Label numberLabel = new Label
                {
                    Text = number.ToString(),
                    ForeColor = Color.Green,
                    BackColor = Color.Black,
                    Font = new Font("Arial", 72, FontStyle.Bold),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                Controls.Add(numberLabel);
                Console.Beep(2500, 100);
                await Task.Delay((int)timePerDigit);
                Controls.Remove(numberLabel); // Clear the screen before displaying the next number
            }
        }

        private int GenerateUniqueNumber()
        {
            int min, max;
            if (difficulty == "Easy")
            {
                min = 1; // Single-digit minimum
                max = 9; // Single-digit maximum
            }
            else if (difficulty == "Medium")
            {
                min = 10; // Double-digit minimum
                max = 99; // Double-digit maximum
            }
            else
            {
                min = 100; // Triple-digit minimum
                max = 999; // Triple-digit maximum
            }

            return new Random().Next(min, max + 1);
        }

        private async Task GetUserAnswer()
        {
            answerBox.Visible = true;
            submitButton.Visible = true;
            submitButton.Enabled = true;

            await Task.Run(() =>
            {
                while (submitButton.Enabled)
                {
                    Application.DoEvents();
                }
            });
            string windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string correctPath = Path.Combine(windowsPath, "Media", "Speech On.wav");
            string incorrectPath = Path.Combine(windowsPath, "Media", "Windows Critical Stop.wav");
            if (int.TryParse(answerBox.Text, out int userAnswer))
            {
                if (userAnswer == correctAnswer)
                {
                    PlaySound(correctPath);
                    MessageBox.Show("Correct!");
                    correctAnswersCount += questionPoints[currentQuestion];
                    correctAnswers += 1;
                }
                else
                {
                    PlaySound(incorrectPath);
                    MessageBox.Show($"Incorrect! The correct answer was {correctAnswer}.");
                }
            }

            answerBox.Visible = false;
            submitButton.Visible = false;
        }
        private void PlaySound(string soundFilePath)
        {
            if (File.Exists(soundFilePath))
            {
                SoundPlayer player = new SoundPlayer(soundFilePath);
                player.Play();
            }
        }
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            submitButton.Enabled = false;
        }

        private void ShowFinalResults()
        {
            MessageBox.Show($"You got {correctAnswers} questions correct and scored {correctAnswersCount} / 100 points.");
        }

        private void ReturnToMainMenu()
        {
            this.Hide();
            parentForm.Show();
        }
    }
}
