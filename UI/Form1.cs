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

            TextBoxTimestep.Text = "0.001";  //dt
            TextBoxCalcTime.Text = "5";      //total time
            TextBoxUVelocity.Text = "6";      //inlet U velocity

            TextBoxNu.Text = "0.15";   //nu
            TextBoxRho.Text = "1";      //rho

            TextBoxPIteration.Text = "1";      //piter

            ButtonRun.Enabled = false;
            ButtonSavePlot.Enabled = false;

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

            LabelReynolds.Text = "Re: " + reynolds.ToString("0.00");
        }
        private void DisplayCFL(float nu, float u, float dxi, float dt)
        {
            double CFL = Math.Round((u * dt * dxi), 3);
            double vN = Math.Round((2 * nu * dt * dxi * dxi), 3);

            LabelCFL.Text = "CFL/v Neumann: " + CFL.ToString("0.000") + " / " + vN.ToString("0.000");
        }

        private void DisplayTime(string txt)
        {
            LabelActualElapsed.Text = "Time Elapsed : " + txt;
        }

        /// <summary>
        /// Calls the CFD solution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRun_Click(object sender, EventArgs e)
        {
            StatusMessage(MeshConstants.MSGEMPTY);

            //top lid U velocity boundary conditions
            var u = float.Parse(TextBoxUVelocity.Text);

            //carried over from rectangular grid - for the purpose of displaying Re, we will
            //just calculate using the farfield width as the length scale
            var Lx = _farfield.Width;

            //fluid and fluid properties
            Fluid fluid = new()
            {
                InletU = u,
                Nu = float.Parse(TextBoxNu.Text),
                Rho = float.Parse(TextBoxRho.Text),
                InletP = 1,
                InletV = 0
            };

            //calc domain is used for times steps and other non-fluid calculation parameters
            CalcDomain calc = new()
            {
                Tmax = float.Parse(TextBoxCalcTime.Text),
                Dt = float.Parse(TextBoxTimestep.Text),
                Piter = int.Parse(TextBoxPIteration.Text)
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
            ButtonSavePlot.Enabled = true;

            StatusMessage(MeshConstants.MSGCFDDONE);

        }

        /// <summary>
        /// Saves the current plotted image to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSavePlot_Click(object sender, EventArgs e)
        {
            var plotModel = plot1.Model;

            SaveFileDialog f = new()
            {
                AddExtension = true,
                Filter = "Portable Network Graphics|*.png",
                FileName = plotModel.Title + ".png"
            };

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
            if (RadioButtonU.Checked == true && pmCollection != null)
            {
                plot1.Model = pmCollection.uModel;
                plot1.Refresh();
            }
            if (RadioButtonV.Checked == true && pmCollection != null)
            {
                plot1.Model = pmCollection.vModel;
                plot1.Refresh();
            }
            if (RadioButtonP.Checked == true && pmCollection != null)
            {
                plot1.Model = pmCollection.pModel;
                plot1.Refresh();
            }
            if (RadioButtonTest.Checked == true && pmCollection != null)
            {
                plot1.Model = pmCollection.tModel;
                plot1.Refresh();
            }
        }
        private void RadioButtonU_Click(object sender, EventArgs e)
        {
            ChangeDisplayType();
        }
        private void RadioButtonV_Click(object sender, EventArgs e)
        {
            ChangeDisplayType();
        }
        private void RadioButtonP_Click(object sender, EventArgs e)
        {
            ChangeDisplayType();
        }
        private void RadioButtonTest_Click(object sender, EventArgs e)
        {
            ChangeDisplayType();
        }

        /// <summary>
        /// Does mesh precalculations before CFD is attempted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPreCalc_Click(object sender, EventArgs e)
        {

            Finalize finalize = Program.container.GetInstance<Finalize>();
            finalize.Logic(_farfield);

            StatusMessage(MeshConstants.MSGPRECALC);

            //enable the CFD button and lock down the others
            ButtonRun.Enabled = true;
            ButtonPreCalc.Enabled = false;
            ButtonSavePlot.Enabled = false;
        }

        private void TextBoxTimestep_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            float fallback = 0.001F;
            TextBoxTimestep.Text = ValidateEntry<float>(ref TextBoxTimestep, fallback).ToString();
        }

        private void TextBoxCalcTime_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            float fallback = 5F;
            TextBoxCalcTime.Text = ValidateEntry<float>(ref TextBoxCalcTime, fallback).ToString();
        }

        private void TextBoxUVelocity_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            float fallback = 6F;
            TextBoxUVelocity.Text = ValidateEntry<float>(ref TextBoxUVelocity, fallback).ToString();
        }

        private void TextBoxPIteration_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            short fallback = 1;
            TextBoxPIteration.Text = ValidateEntry<short>(ref TextBoxPIteration, fallback).ToString();
        }

        private void TextBoxNu_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            float fallback = 0.15F;
            TextBoxNu.Text = ValidateEntry<float>(ref TextBoxNu, fallback).ToString();
        }

        private void TextBoxRho_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            float fallback = 1F;
            TextBoxRho.Text = ValidateEntry<float>(ref TextBoxRho, fallback).ToString();
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