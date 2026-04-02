using System.Windows;

namespace MoodTrack
{
    public partial class SetupWindow : Window
    {
        private readonly Database db = new Database();

        public SetupWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string pin = txtPin.Password.Trim();

            if (string.IsNullOrWhiteSpace(fullName))
            {
                txtStatus.Text = "Ad soyad gir.";
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                txtStatus.Text = "Email gir.";
                return;
            }

            if (string.IsNullOrWhiteSpace(pin))
            {
                txtStatus.Text = "PIN gir.";
                return;
            }

            if (pin.Length != 4)
            {
                txtStatus.Text = "PIN 4 haneli olmalı.";
                return;
            }

            User user = new User
            {
                FullName = fullName,
                Email = email,
                Pin = pin,
                IsActive = true
            };

            db.CreateUser(user);

            MessageBox.Show("İlk kurulum tamamlandı.");
            DialogResult = true;
            Close();
        }
    }
}