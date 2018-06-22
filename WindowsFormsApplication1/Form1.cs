using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        const int n = 8, m = 3;
        int layersNumber, nN = 0;
        bool stop = false;
        public double error = 1;
        static Random rnd = new Random();
        Layer[] Net;// = new Layer[3];

        double [,]ObuchMas = new double [n,m] {{0.1,0.1,0.2},
                                               {0.2,0.2,0.4},
                                               {0.1,0.2,0.3},
                                               {0.2,0.3,0.5},
                                               {0.3,0.4,0.7},
                                               {0.11,0.11,0.22},
                                               {0.12,0.12,0.24},
                                               {0.21,0.21,0.42}};


        public class Neuron
        {
            public double [] w;
            public double s;
            public double x;
            public double b;
            public int relNumber;
            public Neuron()
            {
            }
            public void weight()
            {
                w = new double[relNumber];
                for (int i = 0; i < relNumber; i++)
                    w[i] = rnd.Next(200) / 1000.0 - 0.1;
                s = 0;
                x = 0;
                b = 0;
                return;
            }
        }

        public class Layer
        {
            public int neuronsNumber;
            public Neuron []Neuron;
            public Layer()
            {
                neuronsNumber = 0;
            }
            public void createNeurons()
            { 
                Neuron = new Neuron[neuronsNumber];
                for (int i = 0; i < neuronsNumber; i++)
                {
                    Neuron[i] = new Neuron();
                }
                return;
            }
        }
                
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button2.Visible = true;
            layersNumber = Convert.ToInt32(textBox1.Text)+2;
            int neuronsNumber = Convert.ToInt32(textBox2.Text);
            Net = new Layer[layersNumber];

            for (int i = 0; i < layersNumber; i++)
            {
                Net[i] = new Layer();
            }

            Net[0].neuronsNumber = (m - 1); // input numbers
            Net[0].createNeurons();

            for (int j = 0; j < Net[0].neuronsNumber; j++)
                Net[0].Neuron[j].x = ObuchMas[nN,j];

            for (int i = 1; i < layersNumber; i++ )
            {
                if (i == (layersNumber - 1)) Net[i].neuronsNumber = 1;
                else Net[i].neuronsNumber = neuronsNumber;
                Net[i].createNeurons();
                for (int j = 0; j < Net[i].neuronsNumber; j++)
                {
                    Net[i].Neuron[j].relNumber = Net[i - 1].neuronsNumber; //кол-во связей нейрона=кол-ву нейронов предыдущего слоя
                    Net[i].Neuron[j].weight();
                }
            }
        }



        void Result()
        {
            double Sum;
            for (int i = 1; i < layersNumber; i++ )
            {
                for (int j = 0; j < Net[i].neuronsNumber; j++)
                {
                    Sum = 0;
                    for (int k = 0; k < Net[i].Neuron[j].relNumber; k++)
                        Sum += (Net[i - 1].Neuron[k].x * Net[i].Neuron[j].w[k]);

                    Sum += Net[i].Neuron[j].s;
                    Net[i].Neuron[j].x = 1.0 / (1.0 + Math.Pow(Math.E, -Sum)); ;       //функция активации
                }
            }
        }


        void CalcError()
        {
            double temp = 0;
            button3.Visible = true;
            Net[layersNumber - 1].Neuron[0].b = Net[layersNumber - 1].Neuron[0].x * (1 - Net[layersNumber - 1].Neuron[0].x) * (ObuchMas[nN,m - 1] - Net[layersNumber - 1].Neuron[0].x);
            //по методу обратного распространения ошибки
            for (int i = layersNumber - 2; i > 0; i--)
                for (int j = 0; j < Net[i].neuronsNumber; j++)   //коррекция веса следующего слоя
                {
                    temp = 0;
                    for (int k = 0; k < Net[i + 1].neuronsNumber; k++)
                        temp += (Net[i + 1].Neuron[k].b * Net[i + 1].Neuron[k].w[j]);
                    temp *= (Net[i].Neuron[j].x * (1 - Net[i].Neuron[j].x));
                    Net[i].Neuron[j].b = temp;
                }
            for (int i = layersNumber - 2; i >= 0; i--)
                for (int j = 0; j < Net[i].neuronsNumber; j++)
                    for (int k = 0; k < Net[i + 1].neuronsNumber; k++)
                    {
                        Net[i + 1].Neuron[k].w[j] += (0.8 * Net[i + 1].Neuron[k].b * Net[i].Neuron[j].x);
                    }
            for (int i = layersNumber - 1; i > 0; i--)
                for (int k = 0; k < Net[i].neuronsNumber; k++)
                    Net[i].Neuron[k].s += (0.8 * Net[i].Neuron[k].b);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            double temp;
            stop = false;
            while (!stop)
            {
                error = 0;
                for (nN = 0; nN < n; nN++)
                {
                    for (int j = 0; j < Net[0].neuronsNumber; j++)
                        Net[0].Neuron[j].x = ObuchMas[nN, j]; //подаем на вход
                    Result();
                    CalcError();
                    temp = ObuchMas[nN, m - 1] - Net[layersNumber - 1].Neuron[0].x;// разн между
                    if (temp < 0) temp *= -1;
                    error += temp;
                }
                error /= (double)n;
                label4.Text = Convert.ToString(error);
                Application.DoEvents();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            stop = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double x1, x2;
            x1 = Convert.ToDouble(textBox3.Text);
            x2 = Convert.ToDouble(textBox4.Text);

            textBox5.Text = Convert.ToString(x1+x2);
            Net[0].Neuron[0].x = x1;
            Net[0].Neuron[1].x = x2;

            Result();
            textBox6.Text = Convert.ToString(Net[layersNumber - 1].Neuron[0].x);
        }


    }
}
