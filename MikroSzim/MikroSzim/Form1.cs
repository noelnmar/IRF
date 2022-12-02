using MikroSzim.Entities;
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

namespace MikroSzim
{

    public partial class Form1 : Form
    {
        List<Person> Population = new List<Person>();
        List<BirthProbability> BirthProbabilities = new List<BirthProbability>();
        List<DeathProbability> DeathProbabilities = new List<DeathProbability>();
        List<int> Males = new List<int>();
        List<int> Females = new List<int>();

        Random rng = new Random(1234);

        public Form1()
        {
            InitializeComponent();
            //Population = GetPopulation(@"C:\Temp\nép.csv");
            //BirthProbabilities = GetBirthProbabilities(@"C:\Temp\születés.csv");
            //DeathProbabilities = GetDeathProbabilities(@"C:\Temp\halál.csv");

            // Végigmegyünk a vizsgált éveken
            //for (int year = 2005; year <= 2024; year++)
            //{
            //    // Végigmegyünk az összes személyen
            //    for (int i = 0; i < Population.Count; i++)
            //    {
            //        SimStep(2024, Population[1]);


            //    }

            //    int nbrOfMales = (from x in Population
            //                      where x.Gender == Gender.Male && x.IsAlive
            //                      select x).Count();
            //    int nbrOfFemales = (from x in Population
            //                        where x.Gender == Gender.Female && x.IsAlive
            //                        select x).Count();
            //    Console.WriteLine(
            //        string.Format("Év:{0} Fiúk:{1} Lányok:{2}", year, nbrOfMales, nbrOfFemales));
            //}
        }

        public List<Person> GetPopulation(string csvpath)
        {
            List<Person> population = new List<Person>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    Person p = new Person();
                    p.BirthYear = int.Parse(line[0]);
                    p.Gender = (Gender)int.Parse(line[1]);
                    p.NumberOfChildren = int.Parse(line[2]);
                    population.Add(p);
                }
            }

            return population;
        }
        public List<BirthProbability> GetBirthProbabilities(string csvpath)
        {
            List<BirthProbability> birthprobaility = new List<BirthProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    BirthProbability b = new BirthProbability();
                    b.Age = int.Parse(line[0]);
                    b.NumberOfChildren = int.Parse(line[1]);
                    b.P = double.Parse(line[2]);
                    birthprobaility.Add(b);
                }
            }

            return birthprobaility;
        }
        public List<DeathProbability> GetDeathProbabilities(string csvpath)
        {
            List<DeathProbability> deathprobability = new List<DeathProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    DeathProbability d = new DeathProbability();
                    d.Gender = (Gender)int.Parse(line[0]);
                    d.Age = int.Parse(line[1]);
                    d.P = double.Parse(line[2]);
                    deathprobability.Add(d);
                }
            }

            return deathprobability;
        }

        private void SimStep(int year, Person person)
        {
            //Ha halott akkor kihagyjuk, ugrunk a ciklus következő lépésére
            if (!person.IsAlive) return;

            // Letároljuk az életkort, hogy ne kelljen mindenhol újraszámolni
            byte age = (byte)(year - person.BirthYear);

            // Halál kezelése
            // Halálozási valószínűség kikeresése
            double pDeath = (from x in DeathProbabilities
                             where x.Gender == person.Gender && x.Age == age
                             select x.P).FirstOrDefault();
            // Meghal a személy?
            if (rng.NextDouble() <= pDeath)
                person.IsAlive = false;

            //Születés kezelése - csak az élő nők szülnek
            if (person.IsAlive && person.Gender == Gender.Female)
            {
                //Szülési valószínűség kikeresése
                double pBirth = (from x in BirthProbabilities
                                 where x.Age == age
                                 select x.P).FirstOrDefault();
                //Születik gyermek?
                if (rng.NextDouble() <= pBirth)
                {
                    Person újszülött = new Person();
                    újszülött.BirthYear = year;
                    újszülött.NumberOfChildren = 0;
                    újszülött.Gender = (Gender)(rng.Next(1, 3));
                    Population.Add(újszülött);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Population = GetPopulation(textBox1.Text);
            BirthProbabilities = GetBirthProbabilities(@"C:\Windows\Temp\születés.csv");
            DeathProbabilities = GetDeathProbabilities(@"C:\Windows\Temp\halál.csv");

            Males.DefaultIfEmpty();
            Females.DefaultIfEmpty();
            richTextBox1.Text = "";

            Simulation();
        }

        void Simulation()
        {
            for (int i = 2005; i <= numericUpDown1.Value; i++)
            {
                for (int j = 0; j < Population.Count; j++)
                {
                    SimStep(i, Population[j]);

                    if (Population[j].Gender == Gender.Male)
                    {
                        Males.Add(i);
                    }
                    else
                    {
                        Females.Add(i);
                    }

                }
                int NbrOfMales = (from x in Population
                                  where x.Gender == Gender.Male && x.IsAlive == true
                                  select x).Count();

                int NbrOfFemales = (from x in Population
                                    where x.Gender == Gender.Female && x.IsAlive == true
                                    select x).Count();
            }
            DisplayResult(Males, Females);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath;
            ofd.Filter = "Comma Seperated Values (*.csv)|*.csv";
            ofd.DefaultExt = "csv";
            ofd.AddExtension = true;
            if (ofd.ShowDialog() != DialogResult.OK) return;
            textBox1.Text = ofd.FileName;
        }

        void DisplayResult(List<int> males, List<int> females)
        {
            string text = "";

            for (int i = 2005; i <= numericUpDown1.Value; i++)
            {
                int NbrOfMales = (from x in males
                                  where x == i
                                  select x).Count();

                int NbrOfFemales = (from x in females
                                    where x == i
                                    select x).Count();

                text = text + "Szimulációs év: " + i + "\n\tFiúk: " + NbrOfMales + "\n\tLányok:" + NbrOfFemales + "\n\n";
            }

            richTextBox1.Text = text;
        }

    }
}