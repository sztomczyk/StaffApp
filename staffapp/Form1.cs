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
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'staffappDataSet1.stanowiska' . Możesz go przenieść lub usunąć.
            this.stanowiskaTableAdapter.Fill(this.staffappDataSet1.stanowiska);
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'staffappDataSet1.oferty' . Możesz go przenieść lub usunąć.
            this.ofertyTableAdapter1.Fill(this.staffappDataSet1.oferty);
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'staffappDataSet1.pracownicy' . Możesz go przenieść lub usunąć.
            this.pracownicyTableAdapter.Fill(this.staffappDataSet1.pracownicy);
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'staffappDataSet1.aplikacje' . Możesz go przenieść lub usunąć.
            this.aplikacjeTableAdapter1.Fill(this.staffappDataSet1.aplikacje);
            // TODO: Ten wiersz kodu wczytuje dane do tabeli 'staffappDataSet.oferty' . Możesz go przenieść lub usunąć.
            this.ofertyTableAdapter.Fill(this.staffappDataSet.oferty);

            SqlConnection conn = new SqlConnection(connString);
            conn.ConnectionString = connString;
            conn.Open();
            string sql = "SELECT oferty.id, stanowiska.nazwa, oferty.lokalizacja, oferty.typ_kontraktu FROM oferty INNER JOIN stanowiska ON oferty.id_stanowiska = stanowiska.id";

            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader rd;
            rd = cmd.ExecuteReader();
            listBox1.Items.Clear();

            while(rd.Read())
            {
                listBox1.Items.Add("[" + rd.GetInt32(0).ToString() + "]" + " " + rd.GetString(1).ToString() + " | " + rd.GetString(2).ToString() + " | Umowa: " + rd.GetString(3).ToString());
            }

            rd.Close();
            cmd.Dispose();
            conn.Close();

            /*aplikacjeDataGridView.AutoGenerateColumns = false;

            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(String));
            dt.Columns.Add("Money", typeof(String));
            dt.Rows.Add(new object[] { "Hi", 100 });
            dt.Rows.Add(new object[] { "Ki", 30 });

            DataGridViewComboBoxColumn money = new DataGridViewComboBoxColumn();
            var list11 = new List<string>() { "10", "30", "80", "100" };
            money.DataSource = list11;
            money.HeaderText = "Money";
            money.DataPropertyName = "Money";

            DataGridViewTextBoxColumn name = new DataGridViewTextBoxColumn();
            name.HeaderText = "Name";
            name.DataPropertyName = "Name";

            aplikacjeDataGridView.DataSource = dt;
            aplikacjeDataGridView.Columns.AddRange(name, money);*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
    
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

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

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

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

        private void button7_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Pokaz główną");
            groupBox5.Visible = true;
            groupBox6.Visible = false;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Console.WriteLine("Pokaz panel");
            groupBox5.Visible = false;
            groupBox6.Visible = true;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void czy_akceptuje_regCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
