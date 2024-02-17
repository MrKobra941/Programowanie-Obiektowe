using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

//Do poprwanego działania po zmianie lokalizacji trzeba poprawić ścieżkę do pliku bazy danych

namespace LoginRegistrationForm
{
    public partial class AdminLogin : Form
    {
        //Ustanowienie obiektu łączenia do bazy danych
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Lenka\Documents\Visual Project\LoginRegistrationForm\Database\loginData.mdf;Integrated Security=True;Connect Timeout=30");
        public AdminLogin()
        {
            InitializeComponent();
        }

        //Przyciski "X" wyłączający program
        private void login_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Checkbox pokazujący/ukrywający wpisane hasło
        private void login_showPass_CheckedChanged(object sender, EventArgs e)
        {
            if (login_showPass.Checked)
            {
                login_password.PasswordChar = '\0'; //Odkrywa hasło po zaznaczeniu
            }
            else
            {
                login_password.PasswordChar = '*'; //Ukrywa hasło jeśli jest odznaczony
            }
        }

        //Przycisk logowania
        private void login_btn_Click(object sender, EventArgs e)
        {
            //Warunek sprawdzający, czy pola są puste
            if (login_username.Text == "" || login_password.Text == "")
            {
                //Wyświetlenie komunikatu proszącego o wypełnienie pól
                MessageBox.Show("Wypełnij puste pola", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //Wykonanie sprawdzenia czy dane logowania zgadzają się z danymi w bazie danych
                try
                {
                    connect.Open();//Otwarcie połączenia z bazą danych
                    String selectData = "SELECT * FROM admins WHERE username = @username AND password = @pass"; //Ustanowienie zapytania SQL, jako zmiennej
                    using (SqlCommand cmd = new SqlCommand(selectData, connect)) //wykonanie instrukcji i zwolnienie zasobów przy użyciu "using"
                    {
                        cmd.Parameters.AddWithValue("@username", login_username.Text); //Dodanie do obiektu cmd parametrów o określonych wartościach
                        cmd.Parameters.AddWithValue("@pass", login_password.Text); //Dodanie do obiektu cmd parametrów o określonych wartościach
                        //Zaciągnięcie rekordów z bazy danych
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        //Utworzenie i wypełnienie tabeli danymi z zapytania
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count >= 1) //Warunek sprawdzający czy liczba zgadzających się rekordów jest większa od zera
                        {
                            //Komunikat o poprawnym zalogowaniu
                            MessageBox.Show("Zalogowano poprawnie", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //Przejście do panelu administratora
                            AdminPanel aPanel = new AdminPanel();
                            aPanel.Show();
                            this.Hide();
                        }
                        else //Jeśli zapytanie nie zwróci żadnych rekordów, wtedy dane logowania nie są poprawne
                        {
                            //Komunikat o błędnych danych logowania
                            MessageBox.Show("Nie poprawny login lub hasło", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex) //Wyłapanie i wyświetlenie błędów połączenia
                {
                    MessageBox.Show("Error Connecting: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close(); // Zamknięcie połączenia z bazą danych
                }
            }
        }
    }
}
