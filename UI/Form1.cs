using CFDCore;
using Core.Common;
using Core.Domain;
using Core.Interfaces;
using Mesh.Logic;
using OxyPlot.WindowsForms;
using UI;

namespace CFDSolv
{
    public partial class Form1 : Form
    {
        private readonly IDataAccessService _data;

        private readonly Farfield _farfield;
        private PMCollection? pmCollection;
        private double reynolds;

        public Form1(Farfield farfield, IDataAccessService data)
        {
            _data = data;

            InitializeComponent();

            _farfield = farfield;

            textBox1.Text = "0.001";  //dt
            textBox2.Text = "5";      //total time
            textBox3.Text = "6";      //inlet U velocity

            textBox7.Text = "0.15";   //nu
            textBox8.Text = "1";      //rho

            textBox4.Text = "1";      //piter

            button1.Enabled = false;
            button2.Enabled = false;

        }

        /// <summary>
        /// Displays a status message
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private int StatusMessage(string s)
        {
            TextBoxStatus.Text = s;
            TextBoxStatus.Refresh();
            return 0;
        }

        private void DisplayRe(float nu, float uTop, float Lx)
        {
            //calculate and display Reynolds number
            if (nu > 0)
                reynolds = Math.Round(uTop * Lx / nu, 2);
            else
                reynolds = 0;

            label14.Text = "Re: " + reynolds.ToString("0.00");
        }
        private void DisplayCFL(float nu, float u, float dxi, float dt)
        {
            double CFL = Math.Round((u * dt * dxi), 3);
            double vN = Math.Round((2 * nu * dt * dxi * dxi), 3);

            label5.Text = "CFL/v Neumann: " + CFL.ToString("0.000") + " / " + vN.ToString("0.000");
        }

        private void DisplayTime(string txt)
        {
            label6.Text = "Time Elapsed : " + txt;
        }

        /// <summary>
        /// Calls the CFD solution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            StatusMessage(MeshConstants.MSGEMPTY);

            //top lid U velocity boundary conditions
            var u = float.Parse(textBox3.Text);

            //carried over from rectangular grid - for the purpose of displaying Re, we will
            //just calculate using the farfield width as the length scale
            var Lx = _farfield.Width;

            //fluid and fluid properties
            Fluid fluid = new()
            {
                InletU = u,
                Nu = float.Parse(textBox7.Text),
                Rho = float.Parse(textBox8.Text),
                InletP = 1,
                InletV = 0
            };

            //calc domain is used for times steps and other non-fluid calculation parameters
            CalcDomain calc = new()
            {
                Tmax = float.Parse(textBox2.Text),
                Dt = float.Parse(textBox1.Text),
                Piter = int.Parse(textBox4.Text)
            };

            //calculate and display Reynolds number
            DisplayRe(fluid.Nu, u, Lx);

            //display CFL. Use count of cells on top edge.
            DisplayCFL(fluid.Nu, u, _data.GetElementsByBoundary("top", _farfield).Count / Lx, calc.Dt);

            //call CFD Logic
            var simulation = Program.container.GetInstance<CFDLogic>();
            simulation.FlowSimulation(_farfield, calc, fluid);

            //generate plots
            pmCollection = new();
            plot1.Model = pmCollection.uModel;
            plot1.Refresh();

            //display initial plot depending on selection
            ChangeDisplayType();

            //show elapsed time
            if (Utilities.ElapsedTime != null)
            {
                DisplayTime(Utilities.ElapsedTime);
            }

            //enable the save plot button
            button2.Enabled = true;

            StatusMessage(MeshConstants.MSGCFDDONE);

        }

        /// <summary>
        /// Saves the current plotted image to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_Click(object sender, EventArgs e)
        {
            var plotModel = plot1.Model;

            SaveFileDialog f = new();
            f.AddExtension = true;
            f.Filter = "Portable Network Graphics|*.png";
            f.FileName = plotModel.Title + ".png";
            if (f.ShowDialog() == DialogResult.OK)
            {
                var pngExporter = new PngExporter { Width = 800, Height = 800, Resolution = 300 };
                pngExporter.ExportToFile(plotModel, f.FileName);
            }
        }

        /// <summary>
        /// Change display to show u velocity, v velocity or pressure p depending on which
        /// radio button is checked
        /// </summary>
        private void ChangeDisplayType()
        {
            if (radioButton1.Checked == true && pmCollection != null)
            {
                plot1.Model = pmCollection.uModel;
                plot1.Refresh();
            }
            if (radioButton2.Checked == true && pmCollection != null)
            {
                plot1.Model = pmCollection.vModel;
                plot1.Refresh();
            }
            if (radioButton3.Checked == true && pmCollection != null)
            {
                plot1.Model = pmCollection.pModel;
                plot1.Refresh();
            }
            if (radioButton4.Checked == true && pmCollection != null)
            {
                plot1.Model = pmCollection.tModel;
                plot1.Refresh();
            }
        }
        private void RadioButton1_Click(object sender, EventArgs e)
        {
            ChangeDisplayType();
        }
        private void RadioButton2_Click(object sender, EventArgs e)
        {
            ChangeDisplayType();
        }
        private void RadioButton3_Click(object sender, EventArgs e)
        {
            ChangeDisplayType();
        }
        private void RadioButton4_Click(object sender, EventArgs e)
        {
            ChangeDisplayType();
        }

        /// <summary>
        /// Does mesh precalculations before CFD is attempted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button3_Click(object sender, EventArgs e)
        {

            Finalize finalize = Program.container.GetInstance<Finalize>();
            finalize.Logic(_farfield);

            StatusMessage(MeshConstants.MSGPRECALC);

            //enable the CFD button and lock down the others
            button1.Enabled = true;
            button3.Enabled = false;
            button2.Enabled = false;
        }

        private void TextBox1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            float fallback = 0.001F;
            textBox1.Text = ValidateEntry<float>(ref textBox1, fallback).ToString();
        }

        private void TextBox2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            float fallback = 5F;
            textBox2.Text = ValidateEntry<float>(ref textBox2, fallback).ToString();
        }

        private void TextBox3_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            float fallback = 6F;
            textBox3.Text = ValidateEntry<float>(ref textBox3, fallback).ToString();
        }

        private void TextBox4_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            short fallback = 1;
            textBox4.Text = ValidateEntry<short>(ref textBox4, fallback).ToString();
        }

        private void TextBox7_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            float fallback = 0.15F;
            textBox7.Text = ValidateEntry<float>(ref textBox7, fallback).ToString();
        }

        private void TextBox8_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            float fallback = 1F;
            textBox8.Text = ValidateEntry<float>(ref textBox8, fallback).ToString();
        }

        /// <summary>
        /// Provides a type validation method for input fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="textBox"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        private static T ValidateEntry<T>(ref TextBox textBox, object fallback)
        {
            //success return the value converted to specified type. On failure return the fallback value and highlight
            //the box in yellow.
            try
            {
                textBox.BackColor = Color.White;
                return (T)Convert.ChangeType(textBox.Text, typeof(T));
            }
            catch (Exception)
            {
                textBox.BackColor = Color.Yellow;
                textBox.Text = fallback.ToString();
                return (T)fallback;
            }
        }
    }
}