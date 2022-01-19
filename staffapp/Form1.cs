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
            Form2 myNewForm = new Form2();

            myNewForm.ShowDialog();
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
    }
}
