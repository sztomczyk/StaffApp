using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace staffapp
{
    public partial class Form1 : Form
    {

        string connString = "Data Source=DESKTOP-8DH9C8T;Initial Catalog=staffapp;Integrated Security=true";

        public Form1()
        {
            InitializeComponent();
        }

        private void aplikacjeBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();

        }

        private void aplikacjeBindingNavigatorSaveItem_Click_1(object sender, EventArgs e)
        {
            this.Validate();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'staffappDataSet1.wymagania' . Możesz go przenieść lub usunąć.
            this.wymaganiaTableAdapter.Fill(this.staffappDataSet1.wymagania);
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'staffappDataSet1.dzialy' . Możesz go przenieść lub usunąć.
            this.dzialyTableAdapter.Fill(this.staffappDataSet1.dzialy);
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'staffappDataSet1.oferty' . Możesz go przenieść lub usunąć.
            this.ofertyTableAdapter1.Fill(this.staffappDataSet1.oferty);
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'staffappDataSet1.pracownicy' . Możesz go przenieść lub usunąć.
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'staffappDataSet1.aplikacje' . Możesz go przenieść lub usunąć.
            this.aplikacjeTableAdapter1.Fill(this.staffappDataSet1.aplikacje);
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'staffappDataSet.oferty' . Możesz go przenieść lub usunąć.
            this.ofertyTableAdapter.Fill(this.staffappDataSet.oferty);

            this.loadOffers();
            this.setupApplicationDataGridView();
            this.fillApplicationFormWithSelectedRow();
            this.setupDepartmentDataGridView();
            this.fillDepartmentFormWithSelectedRow();
            this.setupRequirementDataGridView();
            this.fillRequirementFormWithSelectedRow();
            this.setupPositionDataGridView();
            this.fillPositionFormWithSelectedRow();
            this.setupOfferDataGridView();
            this.fillOfferFormWithSelectedRow();
            this.setupEmployeeDataGridView();
            this.fillEmployeeFormWithSelectedRow();
        }

        /* Ustaw oferty w ListBox */
        private void loadOffers()
        {
            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string sql = "SELECT oferty.id, stanowiska.nazwa, oferty.lokalizacja, oferty.typ_kontraktu FROM oferty INNER JOIN stanowiska ON oferty.id_stanowiska = stanowiska.id";

            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader rd;
            rd = cmd.ExecuteReader();
            listBox1.Items.Clear();

            while (rd.Read())
            {
                listBox1.Items.Add("[" + rd.GetInt32(0).ToString() + "]" + " " + rd.GetString(1).ToString() + " | " + rd.GetString(2).ToString() + " | Umowa: " + rd.GetString(3).ToString());
            }

            rd.Close();
            cmd.Dispose();
            conn.Close();
        }

        /* Aplikuj button */
        private void button2_Click(object sender, EventArgs e)
        {
            textBox3.Text = null;
            textBox4.Text = null;
            textBox5.Text = null;
            textBox6.Text = null;
            textBox8.Text = null;
            richTextBox1.Text = null;
            checkBox1.Checked = false;
            label35.Visible = false;
            label36.Visible = false;

            groupBox2.Visible = false;
            groupBox3.Visible = true;
        }

        /* Podgląd wybranej oferty */
        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string itemBoxText = listBox1.GetItemText(listBox1.SelectedItem);
            int pFrom = itemBoxText.IndexOf("[") + "[".Length;
            int pTo = itemBoxText.LastIndexOf("]");

            string offerId = itemBoxText.Substring(pFrom, pTo - pFrom);

            label17.Text = offerId;

            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string getOfferSql = "SELECT stanowiska.nazwa, oferty.lokalizacja, oferty.typ_kontraktu, oferty.plan_pracy, oferty.tryb_pracy, oferty.tryb_rekrutacji, oferty.wynagrodzenie_od, oferty.wynagrodzenie_do FROM oferty INNER JOIN stanowiska ON oferty.id_stanowiska = stanowiska.id WHERE oferty.id = " + offerId;
            string getOfferRequirementsSql = "SELECT wymagania.nazwa, wymagania.poziom FROM wymagania INNER JOIN oferty ON wymagania.id_oferty = oferty.id WHERE wymagania.id_oferty = " + offerId;

            SqlCommand cmdOffer = new SqlCommand(getOfferSql, conn);
            SqlDataReader rdOffer;
            rdOffer = cmdOffer.ExecuteReader();

            while (rdOffer.Read())
            {
                label6.Text = rdOffer.GetString(1).ToString();
                label7.Text = rdOffer.GetString(0).ToString();
                label11.Text = rdOffer.GetString(3).ToString();
                label13.Text = rdOffer.GetString(5).ToString();
                label9.Text = rdOffer.GetString(2).ToString();
                label20.Text = rdOffer.GetString(4).ToString();
                
                if(rdOffer.IsDBNull(6) && !rdOffer.IsDBNull(7))
                {
                    label15.Text = "Do " + rdOffer.GetDecimal(7).ToString() + " zł";
                } else if(!rdOffer.IsDBNull(6) && rdOffer.IsDBNull(7))
                {
                    label15.Text = "Od " + rdOffer.GetDecimal(6).ToString() + " zł";
                } else if(rdOffer.IsDBNull(6) && rdOffer.IsDBNull(7))
                {
                    label15.Text = "Brak informacji";
                } else
                {
                    label15.Text = rdOffer.GetDecimal(6).ToString() + " zł - " + rdOffer.GetDecimal(7).ToString() + " zł";
                }
            }

            rdOffer.Close();
            cmdOffer.Dispose();

            SqlCommand cmdOfferRequirements = new SqlCommand(getOfferRequirementsSql, conn);
            SqlDataReader rdOfferRequirements;
            rdOfferRequirements = cmdOfferRequirements.ExecuteReader();

            listBox2.Items.Clear();

            while (rdOfferRequirements.Read())
            {
                string emojiLevel = "";

                for(int i = 0; i <= rdOfferRequirements.GetInt32(1); i++)
                {
                    emojiLevel += "🔷";
                }

                listBox2.Items.Add(rdOfferRequirements.GetString(0).ToString() + " " + emojiLevel);
            } 

            rdOfferRequirements.Close();
            cmdOfferRequirements.Dispose();
            conn.Close();

            groupBox2.Visible = true;
        }

        /* Wyślij aplikację button */
        private void button3_Click(object sender, EventArgs e)
        {
            label35.Visible = false;
            label36.Visible = false;
            if (textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "" || textBox8.Text == ""
               || checkBox1.Checked == false)
            {
                label35.Visible = true;
            } else
            {
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string sendApplicationSql = "INSERT INTO aplikacje (imie, nazwisko, telefon, email, wiadomosc, cv_url, czy_akceptuje_reg, id_oferty)"
                    + " VALUES ('" + textBox3.Text + "', '" + textBox4.Text + "', '" + textBox6.Text + "', '" + textBox5.Text + "', '" + richTextBox1.Text + "', '" + textBox8.Text + "', " + (checkBox1.Checked ? 1 : 0) + ", " + label17.Text + ");";

                SqlCommand cmdSendApplication = new SqlCommand(sendApplicationSql, conn);
                SqlDataReader rdSendApplication;
                rdSendApplication = cmdSendApplication.ExecuteReader();

                rdSendApplication.Close();
                cmdSendApplication.Dispose();

                textBox3.Text = null;
                textBox4.Text = null;
                textBox5.Text = null;
                textBox6.Text = null;
                textBox8.Text = null;
                richTextBox1.Text = null;
                checkBox1.Checked = false;

                label36.Visible = true;
            }
        }

        /* Zaloguj się Button */
        private void button1_Click_1(object sender, EventArgs e)
        {
            if(textBox1.Text == "admin@admin.com" && textBox2.Text == "admin")
            {
                groupBox1.Visible = false;
                groupBox4.Visible = true;
                textBox1.Text = null;
                textBox2.Text = null;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
            groupBox4.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            groupBox2.Visible = true;
            groupBox3.Visible = false;
        }

        /** Pokaz glowna strone */
        private void button7_Click(object sender, EventArgs e)
        {
            this.loadOffers();
            groupBox5.Visible = true;
            groupBox6.Visible = false;
        }

        /** Pokaz panel */
        private void button4_Click_1(object sender, EventArgs e)
        {
            groupBox5.Visible = false;
            groupBox6.Visible = true;
        }

        /* Ustaw DataGridView aplikacji */
        private void setupApplicationDataGridView(Int32 lastIndex = 0)
        {
            aplikacjeDataGridView.Rows.Clear();
            aplikacjeDataGridView.ColumnCount = 9;
            aplikacjeDataGridView.Columns[0].Name = "ID";
            aplikacjeDataGridView.Columns[1].Name = "Oferta";
            aplikacjeDataGridView.Columns[2].Name = "Imię";
            aplikacjeDataGridView.Columns[3].Name = "Nazwisko";
            aplikacjeDataGridView.Columns[4].Name = "Nr telefonu";
            aplikacjeDataGridView.Columns[5].Name = "Adres e-mail";
            aplikacjeDataGridView.Columns[6].Name = "Wiadomość";
            aplikacjeDataGridView.Columns[7].Name = "CV URL";
            aplikacjeDataGridView.Columns[8].Name = "Akceptuje przetwarzanie danych";

            aplikacjeDataGridView.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            aplikacjeDataGridView.MultiSelect = false;

            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string getApplicationsSql = "SELECT aplikacje.id, oferty.id, oferty.lokalizacja, stanowiska.nazwa, aplikacje.imie, aplikacje.nazwisko, aplikacje.telefon, aplikacje.email, aplikacje.wiadomosc, aplikacje.cv_url, aplikacje.czy_akceptuje_reg FROM aplikacje INNER JOIN oferty ON oferty.id = aplikacje.id_oferty INNER JOIN stanowiska ON oferty.id_stanowiska = stanowiska.id";

            SqlCommand cmdApplication = new SqlCommand(getApplicationsSql, conn);
            SqlDataReader rdApplication;
            rdApplication = cmdApplication.ExecuteReader();

            while (rdApplication.Read())
            {
                string[] row = {
                    rdApplication.GetInt32(0).ToString(),
                    "[" + rdApplication.GetInt32(1).ToString() + "] " + rdApplication.GetString(3).ToString() + " " + rdApplication.GetString(2).ToString(),
                    rdApplication.GetString(4).ToString(),
                    rdApplication.GetString(5).ToString(),
                    rdApplication.GetString(6).ToString(), // numer telefonu
                    rdApplication.GetString(7).ToString(), // email
                    rdApplication.GetString(8).ToString(),
                    rdApplication.GetString(9).ToString(),
                    rdApplication.GetBoolean(10) ? "Tak" : "Nie"
                };
                aplikacjeDataGridView.Rows.Add(row);
            }

            if (lastIndex != 0)
            {
                aplikacjeDataGridView.Rows[lastIndex].Selected = true;
            }

            rdApplication.Close();
            cmdApplication.Dispose();
        }

        /* Ustaw DataGridView działów */
        private void setupDepartmentDataGridView(Int32 lastIndex = 0)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "ID";
            dataGridView1.Columns[1].Name = "Rodzic działu";
            dataGridView1.Columns[2].Name = "Nazwa";

            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string getDepartmentsSql = "SELECT dzial.id, dzial.id_rodzica, rodzic.nazwa, dzial.nazwa FROM dzialy as dzial LEFT JOIN dzialy as rodzic ON rodzic.id = dzial.id_rodzica";

            SqlCommand cmdDepartment = new SqlCommand(getDepartmentsSql, conn);
            SqlDataReader rdDepartment;
            rdDepartment = cmdDepartment.ExecuteReader();

            while (rdDepartment.Read())
            {
                string parentDepartment;
                if (rdDepartment.IsDBNull(1) || rdDepartment.IsDBNull(2))
                {
                    parentDepartment = "";
                } else
                {
                    parentDepartment = "[" + rdDepartment.GetInt32(1).ToString() + "] " + rdDepartment.GetString(2);
                }

                string[] row = {
                    rdDepartment.GetInt32(0).ToString(),
                    parentDepartment,
                    rdDepartment.GetString(3).ToString()
                };
                dataGridView1.Rows.Add(row);
            }

            if (lastIndex != 0)
            {
                dataGridView1.Rows[lastIndex].Selected = true;
            }

            rdDepartment.Close();
            cmdDepartment.Dispose();
        }

        /* Ustaw DataGridView wymagań */
        private void setupRequirementDataGridView(Int32 lastIndex = 0)
        {
            dataGridView2.Rows.Clear();
            dataGridView2.ColumnCount = 4;
            dataGridView2.Columns[0].Name = "ID";
            dataGridView2.Columns[1].Name = "Oferta";
            dataGridView2.Columns[2].Name = "Nazwa";
            dataGridView2.Columns[3].Name = "Poziom";

            dataGridView2.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.MultiSelect = false;

            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string getDepartmentsSql = "SELECT wymagania.id, oferty.id, oferty.lokalizacja, stanowiska.nazwa, wymagania.nazwa, wymagania.poziom FROM wymagania INNER JOIN oferty ON oferty.id = wymagania.id_oferty INNER JOIN stanowiska ON oferty.id_stanowiska = stanowiska.id";

            SqlCommand cmdRequirement = new SqlCommand(getDepartmentsSql, conn);
            SqlDataReader rdRequirement;
            rdRequirement = cmdRequirement.ExecuteReader();

            while (rdRequirement.Read())
            {
                string[] row = {
                    rdRequirement.GetInt32(0).ToString(),
                    "[" + rdRequirement.GetInt32(1).ToString() + "] " + rdRequirement.GetString(3) + " " + rdRequirement.GetString(2),
                    rdRequirement.GetString(4).ToString(),
                    rdRequirement.GetInt32(5).ToString()
                };

                dataGridView2.Rows.Add(row);
            }

            if (lastIndex != 0)
            {
                dataGridView2.Rows[lastIndex].Selected = true;
            }

            rdRequirement.Close();
            cmdRequirement.Dispose();
        }

        /* Ustaw DataGridView wymagań */
        private void setupPositionDataGridView(Int32 lastIndex = 0)
        {
            dataGridView3.Rows.Clear();
            dataGridView3.ColumnCount = 3;
            dataGridView3.Columns[0].Name = "ID";
            dataGridView3.Columns[1].Name = "Dział";
            dataGridView3.Columns[2].Name = "Nazwa";

            dataGridView3.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridView3.MultiSelect = false;

            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string getPositionsSql = "SELECT stanowiska.id, dzialy.id, dzialy.nazwa, stanowiska.nazwa FROM stanowiska INNER JOIN dzialy ON dzialy.id = stanowiska.id_dzialu";

            SqlCommand cmdPosition = new SqlCommand(getPositionsSql, conn);
            SqlDataReader rdPosition;
            rdPosition = cmdPosition.ExecuteReader();

            while (rdPosition.Read())
            {
                string[] row = {
                    rdPosition.GetInt32(0).ToString(),
                    "[" + rdPosition.GetInt32(1).ToString() + "] " + rdPosition.GetString(2),
                    rdPosition.GetString(3).ToString(),
                };

                dataGridView3.Rows.Add(row);
            }

            if (lastIndex != 0)
            {
                dataGridView3.Rows[lastIndex].Selected = true;
            }

            rdPosition.Close();
            cmdPosition.Dispose();
        }

        /* Ustaw DataGridView wymagań */
        private void setupOfferDataGridView(Int32 lastIndex = 0)
        {
            dataGridView4.Rows.Clear();
            dataGridView4.ColumnCount = 11;
            dataGridView4.Columns[0].Name = "ID";
            dataGridView4.Columns[1].Name = "Stanowisko";
            dataGridView4.Columns[2].Name = "Lokalizacja";
            dataGridView4.Columns[3].Name = "Plan pracy";
            dataGridView4.Columns[4].Name = "Tryb pracy";
            dataGridView4.Columns[5].Name = "Typ kontraktu";
            dataGridView4.Columns[6].Name = "Tryb rekrutacji";
            dataGridView4.Columns[7].Name = "Wynagrodzenie od";
            dataGridView4.Columns[8].Name = "Wynagrodzenie do";
            dataGridView4.Columns[9].Name = "Data utworzenia";
            dataGridView4.Columns[10].Name = "Data wygaśnięcia";

            dataGridView4.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridView4.MultiSelect = false;

            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string getOffersSql = "SELECT oferty.id, stanowiska.id, stanowiska.nazwa, oferty.lokalizacja, oferty.plan_pracy, oferty.tryb_pracy, oferty.typ_kontraktu, oferty.tryb_rekrutacji, oferty.wynagrodzenie_od, oferty.wynagrodzenie_do, oferty.data_utworzenia, oferty.data_wygasniecia FROM oferty INNER JOIN stanowiska ON stanowiska.id = oferty.id_stanowiska";

            SqlCommand cmdOffer = new SqlCommand(getOffersSql, conn);
            SqlDataReader rdOffer;
            rdOffer = cmdOffer.ExecuteReader();

            while (rdOffer.Read())
            {
                string wynagrodzenieOd;
                string wynagrodzenieDo;
                if (rdOffer.IsDBNull(8))
                {
                    wynagrodzenieOd = "";
                } else
                {
                    wynagrodzenieOd = rdOffer.GetDecimal(8).ToString();
                }

                if (rdOffer.IsDBNull(9))
                {
                    wynagrodzenieDo = "";
                }
                else
                {
                    wynagrodzenieDo = rdOffer.GetDecimal(9).ToString();
                }

                string[] row = {
                    rdOffer.GetInt32(0).ToString(),
                    "[" + rdOffer.GetInt32(1).ToString() + "] " + rdOffer.GetString(2),
                    rdOffer.GetString(3).ToString(),
                    rdOffer.GetString(4).ToString(),
                    rdOffer.GetString(5).ToString(),
                    rdOffer.GetString(6).ToString(),
                    rdOffer.GetString(7).ToString(),
                    wynagrodzenieOd,
                    wynagrodzenieDo,
                    rdOffer.GetDateTime(10).ToString(),
                    rdOffer.GetDateTime(11).ToString(),
                };

                dataGridView4.Rows.Add(row);
            }

            if (lastIndex != 0)
            {
                dataGridView4.Rows[lastIndex].Selected = true;
            }

            rdOffer.Close();
            cmdOffer.Dispose();
        }

        /* Ustaw DataGridView pracowników */
        private void setupEmployeeDataGridView(Int32 lastIndex = 0)
        {
            dataGridView5.Rows.Clear();
            dataGridView5.ColumnCount = 6;
            dataGridView5.Columns[0].Name = "ID";
            dataGridView5.Columns[1].Name = "Stanowisko";
            dataGridView5.Columns[2].Name = "Imię";
            dataGridView5.Columns[3].Name = "Nazwisko";
            dataGridView5.Columns[4].Name = "Nr telefonu";
            dataGridView5.Columns[5].Name = "Adres e-mail";

            dataGridView5.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridView5.MultiSelect = false;

            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string getEmployeesSql = "SELECT pracownicy.id, stanowiska.id, stanowiska.nazwa, pracownicy.imie, pracownicy.nazwisko, pracownicy.telefon, pracownicy.email FROM pracownicy INNER JOIN stanowiska ON stanowiska.id = pracownicy.id_stanowiska";

            SqlCommand cmdEmployee = new SqlCommand(getEmployeesSql, conn);
            SqlDataReader rdEmployee;
            rdEmployee = cmdEmployee.ExecuteReader();

            while (rdEmployee.Read())
            {
                string[] row = {
                    rdEmployee.GetInt32(0).ToString(),
                    "[" + rdEmployee.GetInt32(1).ToString() + "] " + rdEmployee.GetString(2),
                    rdEmployee.GetString(3).ToString(),
                    rdEmployee.GetString(4).ToString(),
                    rdEmployee.GetString(5).ToString(),
                    rdEmployee.GetString(6).ToString(),
                };

                dataGridView5.Rows.Add(row);
            }

            if (lastIndex != 0)
            {
                dataGridView5.Rows[lastIndex].Selected = true;
            }

            rdEmployee.Close();
            cmdEmployee.Dispose();
        }

        /* Uzupełnij formularz edycji aplikacji */
        private void fillApplicationFormWithSelectedRow()
        {
            if (aplikacjeDataGridView.SelectedRows.Count != 0)
            {
                button11.Visible = false;
                button10.Visible = true;
                button12.Visible = false;
                button9.Visible = true;
                button8.Visible = true;

                DataGridViewRow row = this.aplikacjeDataGridView.SelectedRows[0];
                string offerCell = row.Cells["Oferta"].Value.ToString();
                int pFrom = offerCell.IndexOf("[") + "[".Length;
                int pTo = offerCell.LastIndexOf("]");
                string offerId = offerCell.Substring(pFrom, pTo - pFrom);

                label39.Text = offerId;
                textBox7.Text = row.Cells["ID"].Value.ToString();
                textBox9.Text = row.Cells["Imię"].Value.ToString();
                textBox10.Text = row.Cells["Nazwisko"].Value.ToString();
                textBox11.Text = row.Cells["Nr telefonu"].Value.ToString();
                textBox12.Text = row.Cells["Adres e-mail"].Value.ToString();
                textBox13.Text = row.Cells["Wiadomość"].Value.ToString();
                textBox14.Text = row.Cells["CV URL"].Value.ToString();
                checkBox2.Checked = row.Cells["Akceptuje przetwarzanie danych"].Value.ToString() == "Tak" ? true : false;

                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string getOfferSql = "SELECT oferty.id, stanowiska.nazwa, oferty.lokalizacja FROM oferty INNER JOIN stanowiska ON oferty.id_stanowiska = stanowiska.id";

                SqlCommand cmdOffer = new SqlCommand(getOfferSql, conn);
                SqlDataReader rdOffer;
                rdOffer = cmdOffer.ExecuteReader();

                comboBox1.Items.Clear();

                while (rdOffer.Read())
                {
                    comboBox1.Items.Add("[" + rdOffer.GetInt32(0).ToString() + "] " + rdOffer.GetString(1).ToString() + " " + rdOffer.GetString(2).ToString());
                }

                comboBox1.SelectedIndex = comboBox1.FindStringExact(row.Cells["Oferta"].Value.ToString());

                rdOffer.Close();
                cmdOffer.Dispose();
            }
        }

        /* Uzupełnij formularz edycji działu */
        private void fillDepartmentFormWithSelectedRow()
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                DataGridViewRow row = this.dataGridView1.SelectedRows[0];
                string parentCell = row.Cells["Rodzic działu"].Value.ToString();
                if (parentCell != "")
                {
                    int pFrom = parentCell.IndexOf("[") + "[".Length;
                    int pTo = parentCell.LastIndexOf("]");
                    string parentId = parentCell.Substring(pFrom, pTo - pFrom);
                    label74.Text = parentId;
                } else
                {
                    label74.Text = "";
                }

                textBox21.Text = row.Cells["ID"].Value.ToString();
                textBox20.Text = row.Cells["Nazwa"].Value.ToString();

                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string getParentSql = "SELECT dzialy.id, dzialy.nazwa FROM dzialy";

                SqlCommand cmdParent = new SqlCommand(getParentSql, conn);
                SqlDataReader rdParent;
                rdParent = cmdParent.ExecuteReader();

                comboBox2.Items.Clear();

                comboBox2.Items.Add("");

                while (rdParent.Read())
                {
                    comboBox2.Items.Add("[" + rdParent.GetInt32(0).ToString() + "] " + rdParent.GetString(1).ToString());
                }

                if (parentCell != "")
                {
                    comboBox2.SelectedIndex = comboBox2.FindStringExact(row.Cells["Rodzic działu"].Value.ToString());
                } else
                {
                    comboBox2.ResetText();
                    comboBox2.SelectedIndex = -1;
                }

                rdParent.Close();
                cmdParent.Dispose();
            }
        }

        /* Uzupełnij formularz edycji wymagania */
        private void fillRequirementFormWithSelectedRow()
        {
            if (dataGridView2.SelectedRows.Count != 0)
            {
                DataGridViewRow row = this.dataGridView2.SelectedRows[0];
                string offerCell = row.Cells["Oferta"].Value.ToString();
                int pFrom = offerCell.IndexOf("[") + "[".Length;
                int pTo = offerCell.LastIndexOf("]");
                string offerId = offerCell.Substring(pFrom, pTo - pFrom);
                label66.Text = offerId;

                textBox16.Text = row.Cells["ID"].Value.ToString();
                textBox15.Text = row.Cells["Nazwa"].Value.ToString();

                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string getOfferSql = "SELECT oferty.id, stanowiska.nazwa, oferty.lokalizacja FROM oferty INNER JOIN stanowiska ON oferty.id_stanowiska = stanowiska.id";

                SqlCommand cmdOffer = new SqlCommand(getOfferSql, conn);
                SqlDataReader rdOffer;
                rdOffer = cmdOffer.ExecuteReader();

                comboBox3.Items.Clear();

                while (rdOffer.Read())
                {
                    comboBox3.Items.Add("[" + rdOffer.GetInt32(0).ToString() + "] " + rdOffer.GetString(1).ToString() + " " + rdOffer.GetString(2).ToString());
                }

                comboBox3.SelectedIndex = comboBox3.FindStringExact(row.Cells["Oferta"].Value.ToString());

                comboBox4.Items.Clear();

                for (int i = 1; i < 6; i++)
                {
                    comboBox4.Items.Add(i);
                }

                comboBox4.SelectedIndex = comboBox4.FindStringExact(row.Cells["Poziom"].Value.ToString());

                rdOffer.Close();
                cmdOffer.Dispose();
            }
        }

        /* Uzupełnij formularz edycji stanowiska */
        private void fillPositionFormWithSelectedRow()
        {
            if (dataGridView3.SelectedRows.Count != 0)
            {
                DataGridViewRow row = this.dataGridView3.SelectedRows[0];
                string departmentCell = row.Cells["Dział"].Value.ToString();
                int pFrom = departmentCell.IndexOf("[") + "[".Length;
                int pTo = departmentCell.LastIndexOf("]");
                string departmentId = departmentCell.Substring(pFrom, pTo - pFrom);
                label80.Text = departmentId;

                textBox18.Text = row.Cells["ID"].Value.ToString();
                textBox17.Text = row.Cells["Nazwa"].Value.ToString();

                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string getDepartmentsSql = "SELECT dzialy.id, dzialy.nazwa FROM dzialy";

                SqlCommand cmdDepartment = new SqlCommand(getDepartmentsSql, conn);
                SqlDataReader rdDepartment;
                rdDepartment = cmdDepartment.ExecuteReader();

                comboBox5.Items.Clear();

                while (rdDepartment.Read())
                {
                    comboBox5.Items.Add("[" + rdDepartment.GetInt32(0).ToString() + "] " + rdDepartment.GetString(1).ToString());
                }

                comboBox5.SelectedIndex = comboBox5.FindStringExact(row.Cells["Dział"].Value.ToString());

                rdDepartment.Close();
                cmdDepartment.Dispose();
            }
        }

        /* Uzupełnij formularz edycji oferty */
        private void fillOfferFormWithSelectedRow()
        {
            if (dataGridView4.SelectedRows.Count != 0)
            {
                DataGridViewRow row = this.dataGridView4.SelectedRows[0];
                string positionCell = row.Cells["Stanowisko"].Value.ToString();
                int pFrom = positionCell.IndexOf("[") + "[".Length;
                int pTo = positionCell.LastIndexOf("]");
                string positionId = positionCell.Substring(pFrom, pTo - pFrom);

                label98.Text = positionId;
                textBox27.Text = row.Cells["ID"].Value.ToString();
                textBox19.Text = row.Cells["Lokalizacja"].Value.ToString();
                textBox22.Text = row.Cells["Plan pracy"].Value.ToString();
                textBox24.Text = row.Cells["Tryb pracy"].Value.ToString();
                textBox23.Text = row.Cells["Typ kontraktu"].Value.ToString();
                textBox26.Text = row.Cells["Tryb rekrutacji"].Value.ToString();
                textBox25.Text = row.Cells["Wynagrodzenie od"].Value.ToString();
                textBox29.Text = row.Cells["Wynagrodzenie do"].Value.ToString();
                dateTimePicker1.Text = row.Cells["Data wygaśnięcia"].Value.ToString();

                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string getPositionsSql = "SELECT stanowiska.id, stanowiska.nazwa FROM stanowiska;";

                SqlCommand cmdPosition = new SqlCommand(getPositionsSql, conn);
                SqlDataReader rdPosition;
                rdPosition = cmdPosition.ExecuteReader();

                comboBox6.Items.Clear();

                while (rdPosition.Read())
                {
                    comboBox6.Items.Add("[" + rdPosition.GetInt32(0).ToString() + "] " + rdPosition.GetString(1).ToString());
                }

                comboBox6.SelectedIndex = comboBox6.FindStringExact(row.Cells["Stanowisko"].Value.ToString());

                rdPosition.Close();
                cmdPosition.Dispose();
            }
        }

        /* Uzupełnij formularz edycji oferty */
        private void fillEmployeeFormWithSelectedRow()
        {
            if (dataGridView5.SelectedRows.Count != 0)
            {
                DataGridViewRow row = this.dataGridView5.SelectedRows[0];
                string positionCell = row.Cells["Stanowisko"].Value.ToString();
                int pFrom = positionCell.IndexOf("[") + "[".Length;
                int pTo = positionCell.LastIndexOf("]");
                string positionId = positionCell.Substring(pFrom, pTo - pFrom);

                label111.Text = positionId;
                textBox35.Text = row.Cells["ID"].Value.ToString();
                textBox34.Text = row.Cells["Imię"].Value.ToString();
                textBox33.Text = row.Cells["Nazwisko"].Value.ToString();
                textBox30.Text = row.Cells["Nr telefonu"].Value.ToString();
                textBox28.Text = row.Cells["Adres e-mail"].Value.ToString();

                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string getPositionsSql = "SELECT stanowiska.id, stanowiska.nazwa FROM stanowiska;";

                SqlCommand cmdPosition = new SqlCommand(getPositionsSql, conn);
                SqlDataReader rdPosition;
                rdPosition = cmdPosition.ExecuteReader();

                comboBox7.Items.Clear();

                while (rdPosition.Read())
                {
                    comboBox7.Items.Add("[" + rdPosition.GetInt32(0).ToString() + "] " + rdPosition.GetString(1).ToString());
                }

                comboBox7.SelectedIndex = comboBox7.FindStringExact(row.Cells["Stanowisko"].Value.ToString());

                rdPosition.Close();
                cmdPosition.Dispose();
            }
        }

        /* Zmiana oferty aplikacji ComboBox */
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pFrom = comboBox1.SelectedItem.ToString().IndexOf("[") + "[".Length;
            int pTo = comboBox1.SelectedItem.ToString().LastIndexOf("]");
            label39.Text = comboBox1.SelectedItem.ToString().Substring(pFrom, pTo - pFrom);
        }

        /* Zapisz aplikacje button */
        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox9.Text == "" || textBox10.Text == "" || textBox11.Text == "" || textBox12.Text == "" || textBox14.Text == "")
            {
                label40.Visible = true;
            } else
            {
                label40.Visible = false;
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string updateApplicationSql = "UPDATE aplikacje SET id_oferty = " + label39.Text + ", imie = '" + textBox9.Text + "', nazwisko = '" + textBox10.Text + "', telefon = '" + textBox11.Text + "', email = '" + textBox12.Text + "', wiadomosc = '" + textBox13.Text + "', cv_url = '" + textBox14.Text + "', czy_akceptuje_reg = " + (checkBox2.Checked ? 1 : 0) + " WHERE id = " + textBox7.Text;

                SqlCommand cmdUpdateApplication = new SqlCommand(updateApplicationSql, conn);
                SqlDataReader rdUpdateApplication;
                rdUpdateApplication = cmdUpdateApplication.ExecuteReader();

                rdUpdateApplication.Close();
                cmdUpdateApplication.Dispose();

                Int32 lastIndex = this.aplikacjeDataGridView.SelectedRows[0].Index;
                this.setupApplicationDataGridView(lastIndex);
                this.fillApplicationFormWithSelectedRow();
            }
        }

        /* Usuń aplikację button */
        private void button9_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string deleteApplicationSql = "DELETE FROM aplikacje WHERE id = " + textBox7.Text;

            SqlCommand cmdDeleteApplication = new SqlCommand(deleteApplicationSql, conn);
            SqlDataReader rdDeleteApplication;
            rdDeleteApplication = cmdDeleteApplication.ExecuteReader();

            rdDeleteApplication.Close();
            cmdDeleteApplication.Dispose();

            this.setupApplicationDataGridView();
            this.fillApplicationFormWithSelectedRow();
        }

        /* Dodaj nową aplikacje button */
        private void button10_Click(object sender, EventArgs e)
        {
            button11.Visible = true;
            button10.Visible = false;
            button12.Visible = true;
            button9.Visible = false;
            button8.Visible = false;

            label39.Text = "";
            textBox7.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            textBox11.Text = "";
            textBox12.Text = "";
            textBox13.Text = "";
            textBox14.Text = "";
            checkBox2.Checked = false;

            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string getOfferSql = "SELECT oferty.id, stanowiska.nazwa, oferty.lokalizacja FROM oferty INNER JOIN stanowiska ON oferty.id_stanowiska = stanowiska.id";

            SqlCommand cmdOffer = new SqlCommand(getOfferSql, conn);
            SqlDataReader rdOffer;
            rdOffer = cmdOffer.ExecuteReader();

            comboBox1.Items.Clear();

            while (rdOffer.Read())
            {
                comboBox1.Items.Add("[" + rdOffer.GetInt32(0).ToString() + "] " + rdOffer.GetString(1).ToString() + " " + rdOffer.GetString(2).ToString());
            }

            comboBox1.SelectedIndex = 0;

            string offerCell = comboBox1.Items[0].ToString();
            int pFrom = offerCell.IndexOf("[") + "[".Length;
            int pTo = offerCell.LastIndexOf("]");
            label39.Text = offerCell.Substring(pFrom, pTo - pFrom);

            rdOffer.Close();
            cmdOffer.Dispose();
        }

        /* Wróc do edycji aplikacji button */
        private void button11_Click(object sender, EventArgs e)
        {
            button11.Visible = false;
            button10.Visible = true;
            button12.Visible = false;
            button9.Visible = true;
            button8.Visible = true;

            this.fillApplicationFormWithSelectedRow();
        }

        /* Dodaj nową aplikację Button */
        private void button12_Click(object sender, EventArgs e)
        {
            if (textBox9.Text == "" || textBox10.Text == "" || textBox11.Text == "" || textBox12.Text == "" || textBox14.Text == "")
            {
                label40.Visible = true;
            }
            else
            {
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string sendApplicationSql = "INSERT INTO aplikacje (imie, nazwisko, telefon, email, wiadomosc, cv_url, czy_akceptuje_reg, id_oferty)"
                    + " VALUES ('" + textBox9.Text + "', '" + textBox10.Text + "', '" + textBox11.Text + "', '" + textBox12.Text + "', '" + textBox13.Text + "', '" + textBox14.Text + "', " + (checkBox2.Checked ? 1 : 0) + ", " + label39.Text + ");";

                SqlCommand cmdSendApplication = new SqlCommand(sendApplicationSql, conn);
                SqlDataReader rdSendApplication;
                rdSendApplication = cmdSendApplication.ExecuteReader();

                rdSendApplication.Close();
                cmdSendApplication.Dispose();

                textBox9.Text = null;
                textBox10.Text = null;
                textBox11.Text = null;
                textBox12.Text = null;
                textBox13.Text = null;
                textBox14.Text = null;
                checkBox2.Checked = false;

                label40.Visible = false;

                this.setupApplicationDataGridView();
            }
        }

        /* Kliknięcie w DataGridView aplikacji Handler */
        private void aplikacjeDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.fillApplicationFormWithSelectedRow();
        }

        /* Kliknięcie w DataGridView działów Handler */
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            button15.Visible = true;
            button14.Visible = false;
            button13.Visible = false;
            button17.Visible = true;
            this.fillDepartmentFormWithSelectedRow();
        }

        /* Zmiana zakładki TabControl Handler */
        private void tabControl1_SelectedIndexChanged(Object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    this.setupApplicationDataGridView();
                    this.fillApplicationFormWithSelectedRow();
                    break;
                case 2:
                    this.setupOfferDataGridView();
                    this.fillOfferFormWithSelectedRow();
                    break;
                case 4:
                    this.setupDepartmentDataGridView();
                    this.fillDepartmentFormWithSelectedRow();
                    break;
                case 5:
                    this.setupRequirementDataGridView();
                    this.fillRequirementFormWithSelectedRow();
                    break;
                case 3:
                    this.setupPositionDataGridView();
                    this.fillPositionFormWithSelectedRow();
                    break;
                case 1:
                    this.setupEmployeeDataGridView();
                    this.fillEmployeeFormWithSelectedRow();
                    break;
                default:
                    this.setupApplicationDataGridView();
                    this.fillApplicationFormWithSelectedRow();
                    this.setupDepartmentDataGridView();
                    this.fillDepartmentFormWithSelectedRow();
                    break;
            }
        }

        /* Zapisz dział Button */
        private void button17_Click(object sender, EventArgs e)
        {
            if (textBox20.Text == "")
            {
                label65.Visible = true;
            }
            else
            {
                label65.Visible = false;
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();

                string updateDepartmentSql = "UPDATE dzialy SET id_rodzica = " + (label74.Text != "" ? label74.Text : "NULL") + ", nazwa = '" + textBox20.Text + "' WHERE id = " + textBox21.Text;

                SqlCommand cmdUpdateDepartment = new SqlCommand(updateDepartmentSql, conn);
                SqlDataReader rdUpdateDepartment;
                rdUpdateDepartment = cmdUpdateDepartment.ExecuteReader();

                rdUpdateDepartment.Close();
                cmdUpdateDepartment.Dispose();

                Int32 lastIndex = this.dataGridView1.SelectedRows[0].Index;
                this.setupDepartmentDataGridView(lastIndex);
                this.fillDepartmentFormWithSelectedRow();
            }
        }

        /* Zmiana rodzica działu ComboBox */
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox2.SelectedItem != null && comboBox2.SelectedItem != "")
            {
                int pFrom = comboBox2.SelectedItem.ToString().IndexOf("[") + "[".Length;
                int pTo = comboBox2.SelectedItem.ToString().LastIndexOf("]");
                label74.Text = comboBox2.SelectedItem.ToString().Substring(pFrom, pTo - pFrom);
            }
            else
            {
                label74.Text = "";
            }
        }

        /* Dodaj dział Button */
        private void button15_Click(object sender, EventArgs e)
        {
            textBox21.Text = "";
            comboBox2.ResetText();
            comboBox2.SelectedIndex = -1;
            textBox20.Text = "";
            label74.Text = "";

            button15.Visible = false;
            button14.Visible = true;
            button13.Visible = true;
            button17.Visible = false;
        }

        /* Wróc do edycji działu Button */
        private void button14_Click(object sender, EventArgs e)
        {
            button15.Visible = true;
            button14.Visible = false;
            button13.Visible = false;
            button17.Visible = true;

            this.fillDepartmentFormWithSelectedRow();
        }

        /* Dodaj dział Button */
        private void button13_Click(object sender, EventArgs e)
        {
            if (textBox9.Text == "")
            {
                label65.Visible = true;
            }
            else
            {
                label65.Visible = false;
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string sendApplicationSql = "INSERT INTO dzialy (id_rodzica, nazwa)"
                    + " VALUES (" + (label74.Text != "" ? label74.Text : "NULL") + ", '" + textBox20.Text + "')";

                SqlCommand cmdSendDepartment = new SqlCommand(sendApplicationSql, conn);
                SqlDataReader rdSendDepartment;
                rdSendDepartment = cmdSendDepartment.ExecuteReader();

                rdSendDepartment.Close();
                cmdSendDepartment.Dispose();

                textBox21.Text = "";
                textBox20.Text = "";
                comboBox2.SelectedIndex = -1;

                this.setupDepartmentDataGridView();
            }
        }

        /* Kliknięcie w DataGridView wymagań */
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            button16.Visible = false;
            button18.Visible = false;
            button19.Visible = true;
            button20.Visible = true;
            button21.Visible = true;

            this.fillRequirementFormWithSelectedRow();
        }

        /* Usuwanie wymagania Button */
        private void button21_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string deleteRequirementSql = "DELETE FROM wymagania WHERE id = " + textBox16.Text;

            SqlCommand cmdDeleteRequirement = new SqlCommand(deleteRequirementSql, conn);
            SqlDataReader rdDeleteRequirement;
            rdDeleteRequirement = cmdDeleteRequirement.ExecuteReader();

            rdDeleteRequirement.Close();
            cmdDeleteRequirement.Dispose();

            this.setupRequirementDataGridView();
            this.fillRequirementFormWithSelectedRow();
        }
        
        /* Zapisz wymagania Button */
        private void button19_Click(object sender, EventArgs e)
        {
            if (textBox15.Text == "")
            {
                label59.Visible = true;
            }
            else
            {
                label59.Visible = false;
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();

                string updateRequirementSql = "UPDATE wymagania SET id_oferty = " + label66.Text + ", nazwa = '" + textBox15.Text + "', poziom = " + comboBox4.SelectedItem + " WHERE id = " + textBox16.Text;

                SqlCommand cmdUpdateRequirement = new SqlCommand(updateRequirementSql, conn);
                SqlDataReader rdUpdateRequirement;
                rdUpdateRequirement = cmdUpdateRequirement.ExecuteReader();

                rdUpdateRequirement.Close();
                cmdUpdateRequirement.Dispose();

                Int32 lastIndex = this.dataGridView2.SelectedRows[0].Index;
                this.setupRequirementDataGridView(lastIndex);
                this.fillRequirementFormWithSelectedRow();
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pFrom = comboBox3.SelectedItem.ToString().IndexOf("[") + "[".Length;
            int pTo = comboBox3.SelectedItem.ToString().LastIndexOf("]");
            label66.Text = comboBox3.SelectedItem.ToString().Substring(pFrom, pTo - pFrom);
        }

        /* Dodaj wymaganie Button */
        private void button20_Click(object sender, EventArgs e)
        {
            button16.Visible = true;
            button18.Visible = true;
            button19.Visible = false;
            button20.Visible = false;
            button21.Visible = false;

            textBox16.Text = "";
            comboBox3.SelectedIndex = 0;
            textBox15.Text = "";
            comboBox4.SelectedIndex = 0;
        }

        /* Wróć do edycji wymagania Button */
        private void button16_Click(object sender, EventArgs e)
        {
            button16.Visible = false;
            button18.Visible = false;
            button19.Visible = true;
            button20.Visible = true;
            button21.Visible = true;

            this.fillRequirementFormWithSelectedRow();
        }

        /* Dodaj wymaganie Button */
        private void button18_Click(object sender, EventArgs e)
        {
            if (textBox15.Text == "")
            {
                label59.Visible = true;
            }
            else
            {
                label59.Visible = false;
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string sendApplicationSql = "INSERT INTO wymagania (id_oferty, nazwa, poziom)"
                    + " VALUES (" + (label66.Text != "" ? label66.Text : "NULL") + ", '" + textBox15.Text + "', '" + comboBox4.SelectedItem.ToString() + "')";

                SqlCommand cmdSendRequirement = new SqlCommand(sendApplicationSql, conn);
                SqlDataReader rdSendRequirement;
                rdSendRequirement = cmdSendRequirement.ExecuteReader();

                rdSendRequirement.Close();
                cmdSendRequirement.Dispose();

                textBox16.Text = "";
                comboBox3.SelectedIndex = 0;
                textBox15.Text = "";
                comboBox4.SelectedIndex = 0;

                this.setupRequirementDataGridView();
            }
        }

        /* Zmiana Działu ComboBox */
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pFrom = comboBox5.SelectedItem.ToString().IndexOf("[") + "[".Length;
            int pTo = comboBox5.SelectedItem.ToString().LastIndexOf("]");
            label80.Text = comboBox5.SelectedItem.ToString().Substring(pFrom, pTo - pFrom);
        }

        /* Kliknięcie na DataGridView Stanowisk */
        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            button25.Visible = true;
            button22.Visible = false;
            button24.Visible = true;
            button23.Visible = false;

            this.fillPositionFormWithSelectedRow();
        }

        /* Zapisz stanowisko Button */
        private void button24_Click(object sender, EventArgs e)
        {
            if (textBox17.Text == "")
            {
                label76.Visible = true;
            }
            else
            {
                label76.Visible = false;
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();

                string updatePositionSql = "UPDATE stanowiska SET id_dzialu = " + label80.Text + ", nazwa = '" + textBox17.Text + "' WHERE id = " + textBox18.Text;

                SqlCommand cmdUpdatePosition = new SqlCommand(updatePositionSql, conn);
                SqlDataReader rdUpdatePosition;
                rdUpdatePosition = cmdUpdatePosition.ExecuteReader();

                rdUpdatePosition.Close();
                cmdUpdatePosition.Dispose();

                Int32 lastIndex = this.dataGridView3.SelectedRows[0].Index;
                this.setupPositionDataGridView(lastIndex);
                this.fillPositionFormWithSelectedRow();
            }
        }

        /* Dodaj dział button */
        private void button25_Click(object sender, EventArgs e)
        {
            button25.Visible = false;
            button22.Visible = true;
            button24.Visible = false;
            button23.Visible = true;

            textBox18.Text = "";
            comboBox5.SelectedIndex = 0;
            textBox17.Text = "";
        }

        /* Dodaj dział button */
        private void button23_Click(object sender, EventArgs e)
        {
            if (textBox17.Text == "")
            {
                label76.Visible = true;
            }
            else
            {
                label76.Visible = false;
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string sendApplicationSql = "INSERT INTO stanowiska (id_dzialu, nazwa)"
                    + " VALUES (" + label80.Text + ", '" + textBox17.Text + "')";

                SqlCommand cmdSendPosition = new SqlCommand(sendApplicationSql, conn);
                SqlDataReader rdSendPosition;
                rdSendPosition = cmdSendPosition.ExecuteReader();

                rdSendPosition.Close();
                cmdSendPosition.Dispose();

                textBox17.Text = "";
                comboBox5.SelectedIndex = 0;

                this.setupPositionDataGridView();
            }
        }

        /* Kliknięcie na DataGridView ofert */
        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            button28.Visible = true;
            button27.Visible = false;
            button30.Visible = true;
            button26.Visible = false;
            button29.Visible = true;

            this.fillOfferFormWithSelectedRow();
        }

        /* Stanowiska ComboBox */
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pFrom = comboBox6.SelectedItem.ToString().IndexOf("[") + "[".Length;
            int pTo = comboBox6.SelectedItem.ToString().LastIndexOf("]");
            label98.Text = comboBox6.SelectedItem.ToString().Substring(pFrom, pTo - pFrom);
        }

        /* Zapisz oferte Button */
        private void button30_Click(object sender, EventArgs e)
        {
            if (textBox19.Text == "" || textBox22.Text == "" || textBox24.Text == "" || textBox23.Text == "" || textBox26.Text == "")
            {
                label89.Visible = true;
            }
            else
            {
                label89.Visible = false;
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();

                string updateOfferSql = "UPDATE oferty SET id_stanowiska = " + label98.Text + ", lokalizacja = '" + textBox19.Text + "', plan_pracy = '" + textBox22.Text + "', tryb_pracy = '" + textBox24.Text + "', typ_kontraktu = '" + textBox23.Text + "', tryb_rekrutacji = '" + textBox26.Text + "', wynagrodzenie_od = " + (textBox25.Text != "" ? textBox25.Text.Replace(',', '.') : "NULL") + ", wynagrodzenie_do = " + (textBox29.Text != "" ? textBox29.Text.Replace(',', '.') : "NULL") + ", data_wygasniecia = '" + dateTimePicker1.Text + "' WHERE id = " + textBox27.Text;

                SqlCommand cmdUpdateOffer = new SqlCommand(updateOfferSql, conn);
                SqlDataReader rdUpdateOffer;
                rdUpdateOffer = cmdUpdateOffer.ExecuteReader();

                rdUpdateOffer.Close();
                cmdUpdateOffer.Dispose();

                Int32 lastIndex = this.dataGridView4.SelectedRows[0].Index;
                this.setupOfferDataGridView(lastIndex);
                this.fillOfferFormWithSelectedRow();
            }
        }

        /* Usun oferte Button */
        private void button29_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string deleteRequirementsSql = "DELETE FROM wymagania WHERE id_oferty = " + textBox27.Text;
            string deleteApplicationSql = "DELETE FROM aplikacje WHERE id_oferty = " + textBox27.Text;
            string deleteOfferSql = "DELETE FROM oferty WHERE id = " + textBox27.Text;

            SqlCommand cmdDeleteRequirement = new SqlCommand(deleteRequirementsSql, conn);
            SqlDataReader rdDeleteRequirement;
            rdDeleteRequirement = cmdDeleteRequirement.ExecuteReader();

            rdDeleteRequirement.Close();
            cmdDeleteRequirement.Dispose();

            SqlCommand cmdDeleteApplication = new SqlCommand(deleteApplicationSql, conn);
            SqlDataReader rdDeleteApplication;
            rdDeleteApplication = cmdDeleteApplication.ExecuteReader();

            rdDeleteApplication.Close();
            cmdDeleteApplication.Dispose();

            SqlCommand cmdDeleteOffer = new SqlCommand(deleteOfferSql, conn);
            SqlDataReader rdDeleteOffer;
            rdDeleteOffer = cmdDeleteOffer.ExecuteReader();
            
            rdDeleteOffer.Close();
            cmdDeleteOffer.Dispose();

            this.setupOfferDataGridView();
            this.fillOfferFormWithSelectedRow();
        }

        /* Dodaj oferte button */
        private void button28_Click(object sender, EventArgs e)
        {
            button28.Visible = false;
            button27.Visible = true;
            button30.Visible = false;
            button26.Visible = true;
            button29.Visible = false;

            textBox27.Text = "";
            comboBox6.SelectedIndex = 0;
            textBox19.Text = "";
            textBox22.Text = "";
            textBox24.Text = "";
            textBox23.Text = "";
            textBox26.Text = "";
            textBox25.Text = "";
            textBox29.Text = "";
            dateTimePicker1.Value = DateTime.Now;
        }

        /* Dodaj oferte button */
        private void button26_Click(object sender, EventArgs e)
        {
            if (textBox19.Text == "" || textBox22.Text == "" || textBox24.Text == "" || textBox23.Text == "" || textBox26.Text == "")
            {
                label89.Visible = true;
            }
            else
            {
                label89.Visible = false;
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string sendOfferSql = "INSERT INTO oferty (id_stanowiska, lokalizacja, plan_pracy, tryb_pracy, typ_kontraktu, tryb_rekrutacji, wynagrodzenie_od, wynagrodzenie_do, data_utworzenia, data_wygasniecia)"
                    + " VALUES (" + label98.Text + ", '" + textBox19.Text + "', '" + textBox22.Text + "', '" + textBox24.Text + "', '" + textBox23.Text + "', '" + textBox26.Text + "', '" + (textBox25.Text != "" ? textBox25.Text.Replace(',', '.') : "NULL") + "', '" + (textBox29.Text != "" ? textBox29.Text.Replace(',', '.') : "NULL") + "', '" + DateTime.Now.ToString("yyyy-MM-dd") + "', '" + dateTimePicker1.Text + "' )";

                Console.WriteLine(sendOfferSql);

                SqlCommand cmdSendOffer = new SqlCommand(sendOfferSql, conn);
                SqlDataReader rdSendOffer;
                rdSendOffer = cmdSendOffer.ExecuteReader();

                rdSendOffer.Close();
                cmdSendOffer.Dispose();

                textBox27.Text = "";
                comboBox6.SelectedIndex = 0;
                textBox19.Text = "";
                textBox22.Text = "";
                textBox24.Text = "";
                textBox23.Text = "";
                textBox26.Text = "";
                textBox25.Text = "";
                textBox29.Text = "";
                dateTimePicker1.Value = DateTime.Now;

                this.setupOfferDataGridView();
            }
        }

        /* Wróc do edycji oferty Button */
        private void button27_Click(object sender, EventArgs e)
        {
            button28.Visible = true;
            button27.Visible = false;
            button30.Visible = true;
            button26.Visible = false;
            button29.Visible = true;

            this.fillOfferFormWithSelectedRow();
        }

        private void dataGridView5_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            button33.Visible = true;
            button32.Visible = false;
            button35.Visible = true;
            button34.Visible = true;
            button31.Visible = false;


            this.fillEmployeeFormWithSelectedRow();
        }

        private void button35_Click(object sender, EventArgs e)
        {
            if (textBox34.Text == "" || textBox33.Text == "" || textBox30.Text == "" || textBox28.Text == "")
            {
                label102.Visible = true;
            }
            else
            {
                label102.Visible = false;
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();

                string updateEmployeeSql = "UPDATE pracownicy SET id_stanowiska = " + label111.Text + ", imie = '" + textBox34.Text + "', nazwisko = '" + textBox33.Text + "', telefon = '" + textBox30.Text + "', email = '" + textBox28.Text + "' WHERE id = " + textBox35.Text;

                SqlCommand cmdUpdateEmployee = new SqlCommand(updateEmployeeSql, conn);
                SqlDataReader rdUpdateEmployee;
                rdUpdateEmployee = cmdUpdateEmployee.ExecuteReader();

                rdUpdateEmployee.Close();
                cmdUpdateEmployee.Dispose();

                Int32 lastIndex = this.dataGridView5.SelectedRows[0].Index;
                this.setupEmployeeDataGridView(lastIndex);
                this.fillEmployeeFormWithSelectedRow();
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pFrom = comboBox7.SelectedItem.ToString().IndexOf("[") + "[".Length;
            int pTo = comboBox7.SelectedItem.ToString().LastIndexOf("]");
            label111.Text = comboBox7.SelectedItem.ToString().Substring(pFrom, pTo - pFrom);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string deleteEmployeeSql = "DELETE FROM pracownicy WHERE id = " + textBox35.Text;

            SqlCommand cmdDeleteEmployee = new SqlCommand(deleteEmployeeSql, conn);
            SqlDataReader rdDeleteEmployee;
            rdDeleteEmployee = cmdDeleteEmployee.ExecuteReader();

            rdDeleteEmployee.Close();
            cmdDeleteEmployee.Dispose();

            this.setupEmployeeDataGridView();
            this.fillEmployeeFormWithSelectedRow();
        }

        private void button33_Click(object sender, EventArgs e)
        {
            button33.Visible = false;
            button32.Visible = true;
            button35.Visible = false;
            button34.Visible = false;
            button31.Visible = true;

            textBox35.Text = "";
            comboBox7.SelectedIndex = 0;
            textBox34.Text = "";
            textBox33.Text = "";
            textBox30.Text = "";
            textBox28.Text = "";
        }

        private void button31_Click(object sender, EventArgs e)
        {
            if (textBox34.Text == "" || textBox33.Text == "" || textBox30.Text == "" || textBox28.Text == "")
            {
                label102.Visible = true;
            }
            else
            {
                label102.Visible = false;
                SqlConnection conn = new SqlConnection(connString);
                conn.ConnectionString = connString;
                conn.Open();
                string sendEmployeeSql = "INSERT INTO pracownicy (id_stanowiska, imie, nazwisko, telefon, email)"
                    + " VALUES (" + label111.Text + ", '" + textBox34.Text + "', '" + textBox33.Text + "', '" + textBox30.Text + "', '" + textBox28.Text + "')";

                SqlCommand cmdSendEmployee = new SqlCommand(sendEmployeeSql, conn);
                SqlDataReader rdSendEmployee;
                rdSendEmployee = cmdSendEmployee.ExecuteReader();

                rdSendEmployee.Close();
                cmdSendEmployee.Dispose();

                textBox35.Text = "";
                comboBox7.SelectedIndex = 0;
                textBox34.Text = "";
                textBox33.Text = "";
                textBox30.Text = "";
                textBox28.Text = "";

                this.setupEmployeeDataGridView();
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            button33.Visible = true;
            button32.Visible = false;
            button35.Visible = true;
            button34.Visible = true;
            button31.Visible = false;
        }
    }
}
