using Core.Common;
using Core.Interfaces;
using Core.DataCollections;
using Mesh.Logic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.ComponentModel;

namespace CFDSolv
{
    public partial class Form2 : Form
    {
        private readonly IDataAccessService data;

        private readonly Settings settings = new();
        private readonly Farfield farfield = new();

        #region "Constructor"
        public Form2(IDataAccessService data)
        {

            this.data = data;
            InitializeComponent();

        }
        #endregion

        /// <summary>
        /// Runs on form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form2_Load(object sender, EventArgs e)
        {

            //get values for combo box
            ComboBox1.DataSource = Enum.GetValues(typeof(GridType));
            comboBox2.DataSource = Enum.GetValues(typeof(Tiling));

            //get settings
            settings.CreateSettings();

            //initial farfield values are copied from saved settings
            farfield.Height = settings.Height;
            farfield.Width = settings.Width;
            farfield.Smoothingcycles = settings.Smoothingcycles;
            farfield.Gridtype = settings.Gridtype;
            farfield.Tiling = settings.Tiling;

            //populate text boxes with farfield settings
            TextBoxHeight.Text = farfield.Height.ToString();
            TextBoxWidth.Text = farfield.Width.ToString();
            TextBoxSmoothingCycles.Text = farfield.Smoothingcycles.ToString();
            ComboBox1.SelectedItem = settings.Gridtype;
            comboBox2.SelectedItem = settings.Tiling;

            WindowState = FormWindowState.Maximized;

            UpdateGLSize();

            var ToolTip1 = new ToolTip();
            var ToolTip2 = new ToolTip();
            var ToolTip10 = new ToolTip();

            ToolTip1.SetToolTip(TextBoxWidth, "The width of the farfield in meters");
            ToolTip2.SetToolTip(TextBoxHeight, "The height of the farfield in meters");
            ToolTip10.SetToolTip(TextBoxSmoothingCycles, "Number of iterations of the smoothing routine");

            //set buttons to default
            ResetButtonStatus();

        }

        /// <summary>
        /// Runs on form paint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form2_Paint(object sender, PaintEventArgs e)
        {

            //clear GPU buffers
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            UpdateGLSize();

            //draw the Grid based on cells
            int? n1, n2, n3, n4, n5, n6, n7, n8;

            //UI layer dealing directly with the repository
            foreach (var cell in Repository.CellList)
            {
                n1 = cell.V1;
                n2 = cell.V2;
                n3 = cell.V3;
                n4 = cell.V4;
                n5 = cell.V5;
                n6 = cell.V6;
                n7 = cell.V7;
                n8 = cell.V8;

                // This is old school OpenGl 1.0
                // Wire frame
                GL.Begin(PrimitiveType.LineLoop);
                GL.Color3(255, 255, 255);
                GL.Vertex2(data.NodeV(n1).R.X, data.NodeV(n1).R.Y);
                GL.Vertex2(data.NodeV(n2).R.X, data.NodeV(n2).R.Y);
                GL.Vertex2(data.NodeV(n3).R.X, data.NodeV(n3).R.Y);
                if (n4 != null) GL.Vertex2(data.NodeV(n4).R.X, data.NodeV(n4).R.Y);
                if (n5 != null) GL.Vertex2(data.NodeV(n5).R.X, data.NodeV(n5).R.Y);
                if (n6 != null) GL.Vertex2(data.NodeV(n6).R.X, data.NodeV(n6).R.Y);
                if (n7 != null) GL.Vertex2(data.NodeV(n7).R.X, data.NodeV(n7).R.Y);
                if (n8 != null) GL.Vertex2(data.NodeV(n8).R.X, data.NodeV(n8).R.Y);

                GL.End();

            }

            GlControl.SwapBuffers();

        }

        /// <summary>
        /// Runs on form closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form2_Closing(object sender, EventArgs e)
        {

            //update settings.xml
            Settings.WriteSettings(farfield);

        }

        /// <summary>
        /// Initiates refresh of the form and repaint of the GL drawing control
        /// </summary>
        private void EventCompletion()
        {

            Refresh();
            GlControl.Invalidate();
            StatusMessage(MeshConstants.MSGCOMPLETE);

        }

        /// <summary>
        /// Resets and resizes the GL Control
        /// </summary>
        private void UpdateGLSize()
        {

            //set the GL control size
            GlControl.Width = 1000;
            GlControl.Height = (int)(1000 * farfield.Height / farfield.Width);

            GL.ClearColor(Color4.White);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Ortho(0, farfield.Width, 0, farfield.Height, -1, 1);
            GL.Viewport(0, 0, GlControl.Width, GlControl.Height);

        }

        /// <summary>
        ///  Runs delaunay triangulation on an irregular triangular grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_Click(object sender, EventArgs e)
        {

            StatusMessage(MeshConstants.MSGDELAUNAY);

            //call the logic layer - here we initiate the grid optimization
            DelaunayLogic delaunayLogic = Program.container.GetInstance<DelaunayLogic>();
            delaunayLogic.Logic();

            //repaint
            EventCompletion();

        }

        /// <summary>
        /// Refines an existing grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button3_Click(object sender, EventArgs e)
        {

            StatusMessage(MeshConstants.MSGDIVIDE);

            //call the logic layer - here we initiate grid refining
            Split split = Program.container.GetInstance<Split>();
            split.Logic(farfield);

            //repaint
            EventCompletion();

        }

        /// <summary>
        /// Perform Laplace smoothing on a triangular grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button4_Click(object sender, EventArgs e)
        {

            StatusMessage(MeshConstants.MSGSMOOTH);

            //call the logic layer - here we initiate grid smoothing
            Smooth smooth = Program.container.GetInstance<Smooth>();
            smooth.Logic(farfield);

            //repaint
            EventCompletion();

        }

        /// <summary>
        /// Builds grid for an empty space with no airfoil present
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button6_Click(object sender, EventArgs e)
        {

            StatusMessage(MeshConstants.MSGINITIALIZE);

            //lock down the input boxes
            TextBoxWidth.Enabled = false;
            TextBoxHeight.Enabled = false;
            ComboBox1.Enabled = false;

            //save settings
            Settings.WriteSettings(farfield);

            //call the logic layer - here we do the actual work of building the empty grid
            EmptySpace emptySpace = Program.container.GetInstance<EmptySpace>();
            emptySpace.Logic(farfield);

            //enable buttons for irregular grids
            if (farfield.Gridtype == GridType.Triangles)
            {
                Button2.Enabled = true;
                Button4.Enabled = true;
                Button7.Enabled = true;
            }
            else
            {
                Button2.Enabled = false;
                Button4.Enabled = false;
                Button7.Enabled = false;
            }

            //enable buttons for all grids
            Button3.Enabled = true;
            Button9.Enabled = true;

            //repaint
            EventCompletion();

        }

        /// <summary>
        /// Redistributes nodes on edges of farfield - used for irregular grids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button7_Click(object sender, EventArgs e)
        {

            StatusMessage(MeshConstants.MSGREDISTRIBUTE);

            //call the logic layer
            Redistribute redistribute = Program.container.GetInstance<Redistribute>();
            redistribute.Logic(farfield);

            // Repaint
            EventCompletion();

        }

        /// <summary>
        /// Locks out buttons so that grid can't be changed and performs tiling operations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button9_Click(object sender, EventArgs e)
        {

            //disable buttons to prevent further editing
            Button2.Enabled = false;
            Button3.Enabled = false;
            Button4.Enabled = false;
            Button6.Enabled = false;
            Button7.Enabled = false;
            Button9.Enabled = false;

            //only allow CFD or RESET
            Button8.Enabled = true;
            button10.Enabled = true;

            //save settings
            Settings.WriteSettings(farfield);

            //call the logic layer
            TilingLogic tilingLogic = Program.container.GetInstance<TilingLogic>();
            tilingLogic.Logic(farfield);

            // Repaint
            EventCompletion();

            //message to indicate when grid is finalized
            StatusMessage(MeshConstants.MSGFINALIZED);

        }

        /// <summary>
        /// Opens new form from where we run CFD calcs and display results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button8_MouseClick(object sender, MouseEventArgs e)
        {

            //update settings.xml
            Settings.WriteSettings(farfield);

            //open new form
            Form form1 = new Form1(farfield, data);
            form1.Show();

        }

        /// <summary>
        /// Resets all grid data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button10_Click(object sender, EventArgs e)
        {

            //reset lists of nodes and cells
            ResetData resetData = Program.container.GetInstance<ResetData>();
            resetData.Logic();

            //set buttons to default
            ResetButtonStatus();

            //unlock the input boxes
            TextBoxWidth.Enabled = true;
            TextBoxHeight.Enabled = true;
            ComboBox1.Enabled = true;
            comboBox2.Enabled = true;

        }

        private void TextBoxWidth_Validating(object sender, CancelEventArgs e)
        {
            farfield.Width = ValidateEntry<float>(ref TextBoxWidth, settings.Width);
        }

        private void TextBoxSmoothingCycles_Validating(object sender, CancelEventArgs e)
        {
            farfield.Smoothingcycles = ValidateEntry<short>(ref TextBoxSmoothingCycles, settings.Smoothingcycles);
        }

        private void TextBoxHeight_Validating(object sender, CancelEventArgs e)
        {
            farfield.Height = ValidateEntry<float>(ref TextBoxHeight, settings.Height);
        }

        private void ComboBox1_Validating(object sender, CancelEventArgs e)
        {
            farfield.Gridtype = (GridType)ComboBox1.SelectedItem;
        }

        private void comboBox2_Validating(object sender, CancelEventArgs e)
        {
            farfield.Tiling = (Tiling)comboBox2.SelectedItem;
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

        /// <summary>
        /// Sets the default button availability
        /// </summary>
        private void ResetButtonStatus()
        {

            //allow initial build and reset
            Button6.Enabled = true;
            button10.Enabled = true;

            //disable all other buttons
            Button2.Enabled = false;
            Button3.Enabled = false;
            Button4.Enabled = false;
            Button7.Enabled = false;
            Button8.Enabled = false;
            Button9.Enabled = false;

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

    }
}
