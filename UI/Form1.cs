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

            //Get values for combo box and make a default selection
            comboBox1.DataSource = Enum.GetValues(typeof(CalcType));
            comboBox1.SelectedItem = CalcType.LidCavity;

            _farfield = farfield;

            textBox1.Text = "0.001";    //dt
            textBox2.Text = "5";    //total time
            textBox3.Text = "2";      //inlet U velocity

            textBox7.Text = "0.15";   //nu
            textBox8.Text = "1";      //rho

            textBox4.Text = "1";      //Piter

            button1.Enabled = false;
            button2.Enabled = false;

        }

        private int StatusMessage(string s)
        {
            // displays a status message in the form
            TextBoxStatus.Text = s;
            TextBoxStatus.Refresh();
            return 0;
        }

        private void DisplayRe(float nu, float uTop, float Lx)
        {
            //Calculate and display Reynolds number
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

            //Top lid U velocity boundary conditions
            var u = float.Parse(textBox3.Text);

            //Carried over from rectangular grid - for the purpose of displaying Re, we will
            //just calculate using the farfield width as the length scale
            var Lx = _farfield.Width;

            //Fluid and fluid properties
            Fluid fluid = new()
            {
                InletU = u,
                Nu = float.Parse(textBox7.Text),
                Rho = float.Parse(textBox8.Text),
                InletP = 1,
                InletV = 0
            };

            //Calc domain is used for times steps and other non-fluid calculation parameters
            CalcDomain calc = new()
            {
                Tmax = float.Parse(textBox2.Text),
                Dt = float.Parse(textBox1.Text),
                CalcType = (CalcType)comboBox1.SelectedItem,
                Piter = int.Parse(textBox4.Text)
            };

            //Calculate and display Reynolds number
            DisplayRe(fluid.Nu, u, Lx);

            //Display CFL. Use count of cells on top edge.
            DisplayCFL(fluid.Nu, u, _data.GetElementsByBoundary("top", _farfield).Count / Lx, calc.Dt);

            //Call CFD Logic
            var simulation = Program.container.GetInstance<CFDLogic>();
            simulation.FlowSimulation(_farfield, calc, fluid);

            pmCollection = new();

            plot1.Model = pmCollection.uModel;

            plot1.Refresh();

            //Display initial plot depending on selection
            ChangeDisplayType();

            //Show elapsed time
            if (Utilities.ElapsedTime != null)
            {
                DisplayTime(Utilities.ElapsedTime);
            }

            StatusMessage(MeshConstants.MSGCFDDONE);

        }

        /// <summary>
        /// Saves the current plotted image to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
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
        private void radioButton1_Click(object sender, EventArgs e)
        {
            ChangeDisplayType();
        }
        private void radioButton2_Click(object sender, EventArgs e)
        {
            ChangeDisplayType();
        }
        private void radioButton3_Click(object sender, EventArgs e)
        {
            ChangeDisplayType();
        }
        private void radioButton4_Click(object sender, EventArgs e)
        {
            ChangeDisplayType();
        }

        /// <summary>
        /// Does mesh precalculations before CFD is attempted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {

            // call the logic layer - here we finalize the grid before we can proceed to a CFD solution
            // the first line gets the object by type from the container

            //if (textBox9.Text != null)  //we must have a length provided
            //{
            //    double scale = _farfield.Width / double.Parse(textBox9.Text);  //scales airfoil to given size

            //    Finalize finalize = Program.container.GetInstance<Finalize>();
            //    finalize.Logic(_farfield, scale);
            //}

            double scale = 1.0;

            Finalize finalize = Program.container.GetInstance<Finalize>();
            finalize.Logic(_farfield, scale);

            StatusMessage(MeshConstants.MSGPRECALC);

            button3.Enabled = false;
            button1.Enabled = true;
            button2.Enabled = true;
        }
    }
}