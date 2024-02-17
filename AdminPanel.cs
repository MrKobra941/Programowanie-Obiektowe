using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

//Do poprwanego działania po zmianie lokalizacji trzeba poprawić ścieżkę do pliku bazy danych

namespace LoginRegistrationForm
{
    public partial class AdminPanel : Form
    {
        //Deklaracja obiektów do połączenia z bazą danych
        SqlConnection connect;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        //Zadeklarowanie zmiennych
        int id;
        bool isDoubleClick = false;
        string conPath;

        public AdminPanel()
        {
            InitializeComponent();
            //Zapisanie ścieżki do zmiennej
            conPath = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Lenka\Documents\Visual Project\LoginRegistrationForm\Database\loginData.mdf;Integrated Security=True;Connect Timeout=30";
        }

        //Metoda pobierająca i wyświetlająca dane
        private void ReadData()
        {
            //Próba wykonania polecenia
            try
            {
                connect = new SqlConnection(conPath); //Utworzenie obiektu połączenia do bazy danych
                connect.Open(); //Otwarcie połączenia z bazą
                String selectData = "SELECT * FROM users"; //Zadeklarowanie zapytania SQL do zmiennej
                cmd = new SqlCommand();
                adapter = new SqlDataAdapter(selectData, connect); //Wykonanie zapytania do bazy
                ds.Reset(); //Wyczyszczenie zbioru danych
                adapter.Fill(ds); //Zapełnienie zbioru danych wynikiem zapytania do bazy
                dt = ds.Tables[0]; //Zapisanie do DataTable informacji z DataSet
                dataGridView1.DataSource = dt; //Wskazanie źródła danych dla tabeli wyświetlającej
                connect.Close(); //Zamknięcie połączenia z bazą danych
                //Ustalenie parametrów wyświetlanej tabeli, takich jak nazwy kolumn, ich widoczność oraz ich rozmiar
                dataGridView1.Columns[1].HeaderText = "Email";
                dataGridView1.Columns[2].HeaderText = "Username";
                dataGridView1.Columns[3].HeaderText = "Password";
                dataGridView1.Columns[4].HeaderText = "Created";
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; //Określenie sposobu zaznaczania w programie
            }
            catch (Exception ex) //Wyświetlenie komunikatu o błędzie
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Funkcje wykonujące się podczas ładowania okna
        private void AdminPanel_Load(object sender, EventArgs e)
        {
            ReadData(); //Wykonanie funkcji, uzupełniającej podgląd bazy danych
        }

        //Dodawanie rekordów do bazy danych
        private void Add_Click(object sender, EventArgs e)
        {
            if (textEmail.Text == "" || textUser.Text == "" || textPass.Text == "") //Sprawdzebie czy pola są wypełnione
            {
                MessageBox.Show("Uzupełnij puste pola"); //Wyświetlenie komunikatu
            }
            else
            {
                //Próba dodania rekordu do bazy danych
                try
                {
                    DateTime date = DateTime.Today; //Zadeklarowanie aktualnej daty

                    connect = new SqlConnection(conPath); //Utworzenie obiektu połączenia do bazy danych
                    cmd = new SqlCommand(); //Utowrzenie obiektu klasy SqlCommand
                    //Zapisanie polecenia do obiektu
                    cmd.CommandText = @"INSERT INTO users (email, username, password, date_created) VALUES(@email, @username, @password, @date)";
                    cmd.Connection = connect; //Wskazanie obiektu łączącego
                    cmd.Parameters.Add(new SqlParameter("@email", textEmail.Text)); //Dodanie do obiektu cmd parametrów o określonych wartościach
                    cmd.Parameters.Add(new SqlParameter("@username", textUser.Text)); //Dodanie do obiektu cmd parametrów o określonych wartościach
                    cmd.Parameters.Add(new SqlParameter("@password", textPass.Text)); //Dodanie do obiektu cmd parametrów o określonych wartościach
                    cmd.Parameters.Add(new SqlParameter("@date", date)); //Dodanie do obiektu cmd parametrów o określonych wartościach
                    connect.Open(); //Otwarcie połączenia z bazą danych

                    int i = cmd.ExecuteNonQuery(); //Wykonanie zapytania do bazy i zapisanie informacji zwrotnej w zmiennej

                    if (i == 1) //Sprawdzenie czy informacja zwrotna wskazuje na poprawność wykonania operacji
                    {
                        MessageBox.Show("Utworzono poprawnie"); //Wyświetlenie komunikatu
                        textEmail.Text = ""; //Wyczyszczenie pola tekstowego
                        textUser.Text = ""; //Wyczyszczenie pola tekstowego
                        textPass.Text = ""; //Wyczyszczenie pola tekstowego
                        ReadData(); //Ponowne załadowanie podglądu bazy danych
                        dataGridView1.ClearSelection(); //Odznaczenie, zaznaczonych rekordów
                    }
                }
                catch (Exception ex) //Komunikat o błędzie
                {
                    MessageBox.Show(ex.Message); //Wyświetlenie komunikatu
                }
            }
        }

        //Pobranie informacji potrzebnych do wykonania funkcji Update  
        private void Edit(object sender, DataGridViewCellEventArgs e)
        {
            id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value); //Pobranie id z podglądu bazy
            textEmail.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString(); //Pobranie email z podglądu bazy i wstawienie do pola tekstowego
            textUser.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString(); //Pobranie email z podglądu bazy i wstawienie do pola tekstowego
            textPass.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString(); //Pobranie email z podglądu bazy i wstawienie do pola tekstowego
            isDoubleClick = true; //Ustawnie zmiennej bool do dalszego wykorzyystania w warunku
        }

        //Aktualizacja rekordów
        private void Update_Click(object sender, EventArgs e)
        {
            if (isDoubleClick) //Sprawdzenie czy dane zostały pobrane
            {
                //Próba zaktualizowania zaznaczonego rekordu
                try
                {
                    connect = new SqlConnection(conPath); //Utworzenie obiektu połączenia do bazy danych
                    connect.Open(); //Otwarcie połączenia
                    cmd = new SqlCommand(); //Utowrzenie obiektu klasy SqlCommand
                    //Zapisanie polecenia do obiektu
                    cmd.CommandText = @"UPDATE users set email=@email, username=@username, password=@password WHERE id = @id";
                    cmd.Connection = connect; //Wskazanie obiektu łączącego
                    cmd.Parameters.AddWithValue("@email", textEmail.Text); //Dodanie do obiektu cmd parametrów o określonych wartościach
                    cmd.Parameters.AddWithValue("@username", textUser.Text); //Dodanie do obiektu cmd parametrów o określonych wartościach
                    cmd.Parameters.AddWithValue("@password", textPass.Text); //Dodanie do obiektu cmd parametrów o określonych wartościach
                    cmd.Parameters.AddWithValue("@id", id); //Dodanie do obiektu cmd parametrów o określonych wartościach
                    int i = cmd.ExecuteNonQuery(); //Wykonanie zapytania do bazy i zapisanie informacji zwrotnej w zmiennej

                    if (i == 1) //Sprawdzenie czy informacja zwrotna wskazuje na poprawność wykonania operacji
                    {
                        MessageBox.Show("Zaktualizowano"); //Wyświetlenie komunikatu
                        textEmail.Text = ""; //Wyczyszczenie pola tekstowego
                        textUser.Text = ""; //Wyczyszczenie pola tekstowego
                        textPass.Text = ""; //Wyczyszczenie pola tekstowego
                        ReadData(); //Ponowne załadowanie podglądu bazy danych
                        id = 0; //Wyzerowanie zmiennej
                        dataGridView1.ClearSelection(); //Odznaczenie, zaznaczonych rekordów
                        dataGridView1.CurrentCell = null; //Odznaczenie komórki
                        isDoubleClick = false; //Przywrócenie do stanu pierwotnego zmiennej
                    }

                    connect.Close(); //Zamknięcie połączenia z bazą
                }
                catch (Exception ex) //Komunikat o błędzie
                {
                    MessageBox.Show(ex.Message); //Wyświetlenie komunikatu
                }
            }
        }

        //Pobranie danych potrzebnych do wykonania Delete
        private void GetIdToDelete(object sender, DataGridViewCellEventArgs e)
        {
            id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value); //Pobranie id z podglądu bazy
            isDoubleClick = false; //Upewnienie się, że zmienna pozostaje fałszywa
            textEmail.Text = ""; //Wyczyszczenie pola tekstowego
            textUser.Text = ""; //Wyczyszczenie pola tekstowego
            textPass.Text = ""; //Wyczyszczenie pola tekstowego
        }

        //Usuwanie rekordów
        private void Delete_Click(object sender, EventArgs e)
        {
            //Zapytanie użytkownika o potwierdzenie
            DialogResult dialogResult = MessageBox.Show("Czy napewno chcesz usunąć?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes) //Jeśli użytkownik potwierdził to przejście do następnego kroku
            {
                //Próba wykonania zapytania Delete
                try
                {
                    connect = new SqlConnection(conPath); //Utworzenie obiektu połączenia do bazy danych
                    connect.Open(); //Otwarcie połączenia
                    string delete = "DELETE FROM users WHERE id=@id"; //Zadeklarowanie zapytania SQL do zmiennej
                    SqlCommand cmd = new SqlCommand(delete, connect); //Utworzenie obiektu klasy SqlCommand
                    cmd.Parameters.AddWithValue("@id", id); //Dodanie do obiektu cmd parametrów o określonych wartościach
                    int i = cmd.ExecuteNonQuery(); //Wykonanie zapytania do bazy i zapisanie informacji zwrotnej w zmiennej

                    if (i == 1) //Sprawdzenie czy informacja zwrotna wskazuje na poprawność wykonania operacji
                    {
                        MessageBox.Show("Usunięto"); //Wyświetlenie komunikatu
                        id = 0; //Wyzerowanie zmiennej
                        ReadData(); //Ponowne załadowanie podglądu bazy danych
                        dataGridView1.ClearSelection(); //Odznaczenie, zaznaczonych rekordów
                        dataGridView1.CurrentCell = null; //Odznaczenie komórki
                    }
                }
                catch (Exception ex) //Komunikat o błędzie
                {
                    MessageBox.Show(ex.Message); //Wyświetlenie komunikatu
                }
            }
            else if (dialogResult == DialogResult.No) //Nie wykonanie żadnych instrukcji
            {

            }
        }

        //Wyczyszczenie pól tekstowych i odznaczenie zaznaczonych rekordów
        private void Clear_Click(object sender, EventArgs e)
        {
            id = 0; //Wyzerowanie zmiennej
            textEmail.Text = ""; //Wyczyszczenie pola tekstowego
            textUser.Text = ""; //Wyczyszczenie pola tekstowego
            textPass.Text = ""; //Wyczyszczenie pola tekstowegos
            dataGridView1.ClearSelection(); //Odznaczenie, zaznaczonych rekordów
            dataGridView1.CurrentCell = null; //Odznaczenie komórki
            isDoubleClick = false; //Przywrócenie do stanu pierwotnego zmiennej
        }
    }
}

