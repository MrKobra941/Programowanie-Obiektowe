using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

//Do poprwanego działania po zmianie lokalizacji trzeba poprawić ścieżkę do pliku bazy danych

namespace LoginRegistrationForm
{
    public partial class Signup : Form
    {
        //Ustanowienie obiektu łączenia do bazy danych
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Lenka\Documents\Visual Project\LoginRegistrationForm\Database\loginData.mdf;Integrated Security=True;Connect Timeout=30");
        public Signup()
        {
            InitializeComponent();
        }

        //Przycisk przenoszący do ekranu logowania
        private void Signup_loginHere_Click(object sender, EventArgs e)
        {
            //Instrukcja przenosząca do ekranu logowania
            Form1 lForm = new Form1();
            lForm.Show();
            this.Hide();
        }

        //Przycisk "X" zamykający program
        private void Signup_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Przycisko dokonujący rejestracji nowego użytkownika
        private void Signup_btn_Click(object sender, EventArgs e)
        {
            //Warunek sprawdzający, czy pola są puste
            if (signup_email.Text == "" || signup_username.Text == "" 
                || signup_password.Text == "")
            {
                //Wyświetlenie komunikatu proszącego o wypełnienie pól
                MessageBox.Show("Proszę wypełnić puste pola", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //Wykonanie sprawdzenia czy użytkownik isnieje oraz dodanie nowego w przypadku braku
                try
                {
                    connect.Open(); //Otwarcie połączenia z bazą
                    String checkUsername = "SELECT * FROM users WHERE username = '"
                        + signup_username.Text.Trim() + "'"; //Zaciągnięcie danych z bazy

                    using (SqlCommand checkUser = new SqlCommand(checkUsername, connect)) //wykonanie instrukcji i zwolnienie zasobów przy użyciu "using"
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(checkUser);
                        //Utworzenie i wypełnienie tabeli danymi z zapytania
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count >= 1) //Sprawdzenie czy użytkownik już istnieje jeśli zwróconych rekordów jest więcej niż zero
                        {
                            //Wyświetlenie komunikatu o isniejącym użytkowniku
                            MessageBox.Show(signup_username.Text + " już isnieje", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else //Wykonanie dodania nowego użytkownika
                        {
                            string insertData = "INSERT INTO users (email, username, password, date_created) " +
                                "VALUES(@email, @username, @pass, @date)"; //Ustanowienie zapytania SQL, jako zmiennej

                            DateTime date = DateTime.Today; //Pobranie aktualnej daty w celu wprowadzenia dnia utworzenia użytkownika

                            using (SqlCommand cmd = new SqlCommand(insertData, connect)) //wykonanie instrukcji i zwolnienie zasobów przy użyciu "using"
                            {
                                {
                                    cmd.Parameters.AddWithValue("@email", signup_email.Text.Trim()); //Dodanie do obiektu cmd parametrów o określonych wartościach
                                    cmd.Parameters.AddWithValue("@username", signup_username.Text.Trim()); //Dodanie do obiektu cmd parametrów o określonych wartościach
                                    cmd.Parameters.AddWithValue("@pass", signup_password.Text.Trim()); //Dodanie do obiektu cmd parametrów o określonych wartościach
                                    cmd.Parameters.AddWithValue("@date", date);

                                    cmd.ExecuteNonQuery(); //Wykonanie polecenia na bazie dabych

                                    //Wyświetlenie komunikatu o powodzeniu
                                    MessageBox.Show("Pomyślnie zarejestrowano", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    //Przełączenie do ekranu logowania
                                    Form1 lForm = new Form1();
                                    lForm.Show();
                                    this.Hide();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) //Wyłapanie błędów z połączniem do bazy danych
                {
                    MessageBox.Show("Error connecting Database: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close(); //Zamknięcie połączenia z bazą danych
                }
            }
        }

        //Checkbox pokazujący/ukrywający hasło
        private void Signup_showPass_CheckedChanged(object sender, EventArgs e)
        {
            if (signup_showPass.Checked)
            {
                signup_password.PasswordChar = '\0'; //Odkrywa hasło po zaznaczeniu
            }
            else
            {
                signup_password.PasswordChar = '*'; //Ukrywa hasło jeśli jest odznaczony
            }
        }
    }
}
