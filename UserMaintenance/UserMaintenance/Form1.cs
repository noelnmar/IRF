using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserMaintenance.Entities;

namespace UserMaintenance
{
    public partial class Form1 : Form
    {
        BindingList<User> users = new BindingList<User>();
        public Form1()
        {
            InitializeComponent();
            label1.Text = Resource1.FullName;

            button1.Text = Resource1.Add;
            button2.Text = Resource2.Add2;
            button3.Text = Resource2.Del;

            listBox1.DataSource = users;
            listBox1.ValueMember = "ID";
            listBox1.DisplayMember = "FullName";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var u = new User();
            u.FullName = textBox1.Text;
            users.Add(u);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Application.StartupPath; 
            sfd.Filter = "Comma Seperated Values (*.csv)|*.csv"; 
            sfd.DefaultExt = "csv"; 
            sfd.AddExtension = true; 

            if (sfd.ShowDialog() != DialogResult.OK) return;

            using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
            {
                foreach (var u in users)
                {
                  
                    sw.Write(s.FullName);
                    sw.Write(";");
                    sw.Write(s.ID);
                    //sw.Write(";");
                    //sw.Write(s.BirthDate.ToString());
                    //sw.Write(";");
                    //sw.Write(s.AverageGrade.ToString());
                    //sw.Write(";");
                    //sw.Write(s.IsActive.ToString());
                    sw.WriteLine(); 
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //var törlendő = (from x in User.user
            //                where x.ReceptID == rid
            //                select x).FirstOrDefault();
            //users.Remove(törlendő);
            //context.SaveChanges();
        }
    }
}
