using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        private List<User> users = new List<User>();
        private int progressValue = 0;
        private System.Timers.Timer timer1 = new();
        public Form2()
        {
            InitializeComponent();
        }
        private void fillthefields(User user)
        {
            textBox2.Text = user.Name;
            textBox3.Text = user.Surname;
            textBox4.Text = user.Fname;
            textBox5.Text = user.Country;
            textBox6.Text = user.City;
            textBox7.Text = user.Phone;
            if (user.gender == 0)
            {
                radioButton1.Checked = true; radioButton2.Checked = false;
            }
            else { radioButton1.Checked = false; radioButton2.Checked = true; }
            dateTimePicker1.Value = user.Birthday;
            textBox8.Text = user.ID;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Value = 0;

            progressValue = 0;
            timer1.Interval = 10;
            timer1.Elapsed += timer1_Tick;
            timer1.AutoReset = true;
            timer1.Start();

            if (string.IsNullOrWhiteSpace(textBox1.Text) == true)
                return;
            string namesearch = textBox1.Text.ToLower();
            foreach (var user in users)
            {
                if (user.Name.ToLower().Contains(namesearch))
                {
                    fillthefields(user);
                    MessageBox.Show("User tapildi");
                    return;
                }
            }
            MessageBox.Show("User tapilmadi :(");
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            progressValue += 1;
            if (progressValue <= 100)
                progressBar1.Value = progressValue;
            else
            {
                timer1.Stop();
                progressBar1.Visible = false;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string name = textBox2.Text;
            string surname = textBox3.Text;
            string fname = textBox4.Text;
            string country = textBox5.Text;
            string city = textBox6.Text;
            string phone = textBox7.Text;
            DateTime birthday = dateTimePicker1.Value;
            Gender_enum gender = default;
            string getid = textBox8.Text;
            if (radioButton1.Checked == true)
                gender = Gender_enum.Male;
            else gender = Gender_enum.Female;

            if (string.IsNullOrEmpty(textBox8.Text) == false)
            {
                foreach (var user in users)
                {
                    if (user.ID == getid)
                    {
                        try
                        {
                            user.Name = name;
                            user.Surname = surname;
                            user.Fname = fname;
                            user.Country = country;
                            user.City = city;
                            user.Phone = phone;
                            user.Birthday = birthday;
                            user.gender = gender;
                            savetofile();
                            textBox2.Text = string.Empty;
                            textBox3.Text = string.Empty;
                            textBox4.Text = string.Empty;
                            textBox5.Text = string.Empty;
                            textBox6.Text = string.Empty;
                            textBox7.Text = string.Empty;
                            dateTimePicker1.Text = string.Empty;
                            textBox8.Text = string.Empty;
                            MessageBox.Show("Melumatlar yenilendi :)");
                            return;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                    }
                }
                return;
            }
            string id = Random.Shared.Next(0, 9999999).ToString();
            try
            {
                User user = new(name, surname, fname, country, city, phone, birthday, gender, id);
                users.Add(user);
                savetofile();
                textBox2.Text = string.Empty;
                textBox3.Text = string.Empty;
                textBox4.Text = string.Empty;
                textBox5.Text = string.Empty;
                textBox6.Text = string.Empty;
                textBox7.Text = string.Empty;
                dateTimePicker1.Text = string.Empty;
                MessageBox.Show("Elave edildi :)");
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
        private void savetofile()
        {
            string fileName = "users.json";
            string jsonString = JsonSerializer.Serialize(users);
            File.WriteAllText(fileName, jsonString);
        }
        private void loadfromfile()
        {
            string fileName = "users.json";
            if (File.Exists(fileName) == false)
                return;
            string jsonString = File.ReadAllText(fileName);
            users = JsonSerializer.Deserialize<List<User>>(jsonString);
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            loadfromfile();
        }
    }
    public enum Gender_enum { Male, Female }
    public class User
    {
        public static bool RegexPhone(string phone)
        {
            string pattern = @"^\+994(50|51|70|77|55|10|90)\d{7}$";
            return Regex.IsMatch(phone, pattern);
        }
        public string ID { get; set; }
        private string name;
        public string Name { get => name; set { if (!string.IsNullOrWhiteSpace(value)) name = value; else throw new Exception("Ad bos qala bilmez"); } }
        private string surname;
        public string Surname { get => surname; set { if (!string.IsNullOrWhiteSpace(value)) surname = value; else throw new Exception("Soyad bos qala bilmez"); } }
        private string fname;
        public string Fname { get => fname; set { if (!string.IsNullOrWhiteSpace(value)) fname = value; else throw new Exception("Ata adi bos qala bilmez"); } }
        private string country;
        public string Country { get => country; set { if (!string.IsNullOrWhiteSpace(value)) country = value; else throw new Exception("Olke adi bos qala bilmez"); } }
        private string city;
        public string City { get => city; set { if (!string.IsNullOrWhiteSpace(value)) city = value; else throw new Exception("Seher adi bos qala bilmez"); } }
        private string phone;
        public string Phone { get => phone; set { if (RegexPhone(value)) phone = value; else throw new Exception("Telefon nomresi duzgun formatda verilmeyib"); } }
        private DateTime birthday;
        public DateTime Birthday
        {
            get => birthday; set
            {
                TimeSpan age = DateTime.Today - value;
                if (age.TotalDays >= 18 * 365)
                    birthday = value;
                else
                    throw new Exception("18 yasdan kicik vetendaslar bu anketi doldura bilmezler");
            }
        }
        public Gender_enum gender { get; set; }
        public User() { }
        public User(string name, string surname, string fname, string country, string city, string phone, DateTime birthday, Gender_enum gender, string id)
        {
            Name = name;
            Surname = surname;
            Fname = fname;
            Country = country;
            City = city;
            Phone = phone;
            Birthday = birthday;
            this.gender = gender;
            ID = id;
        }
    }
}
