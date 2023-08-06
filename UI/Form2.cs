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

        //public Farfield Farfield { get { return farfield; } }

        private System.Windows.Forms.Timer _timer = null!;
        private float _angle = 0.0f;

        public Form2(IDataAccessService data)
        {
            this.data = data;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            //get values for combo box
            comboBox1.DataSource = Enum.GetValues(typeof(GridType));

            //get settings
            settings.CreateSettings();

            //initial farfield values are copied from saved settings
            farfield.Height = settings.Height;
            farfield.Width = settings.Width;
            farfield.Smoothingcycles = settings.Smoothingcycles;
            farfield.Gridtype = settings.Gridtype;

            //populate text boxes with farfield settings
            TextBoxHeight.Text = farfield.Height.ToString();
            TextBoxWidth.Text = farfield.Width.ToString();
            TextBoxSmoothingCycles.Text = farfield.Smoothingcycles.ToString();
            comboBox1.SelectedItem = settings.Gridtype;

            WindowState = FormWindowState.Maximized;
            GL.ClearColor(Color4.White);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Ortho(0, farfield.Width, 0, farfield.Height, -1, 1);
            GL.Viewport(0, 0, 1000, (int)(1000 * farfield.Height / farfield.Width));

            var ToolTip1 = new ToolTip();
            var ToolTip2 = new ToolTip();
            var ToolTip10 = new ToolTip();

            ToolTip1.SetToolTip(TextBoxWidth, "The width of the farfield in meters");
            ToolTip2.SetToolTip(TextBoxHeight, "The height of the farfield in meters");
            ToolTip10.SetToolTip(TextBoxSmoothingCycles, "Number of iterations of the smoothing routine");

            //set buttons to default
            ResetButtonStatus();
        }

        private void GlControl_Load(object? sender, EventArgs e)
        {
            // Make sure that when the GLControl is resized or needs to be painted,
            // we update our projection matrix or re-render its contents, respectively.
            GlControl.Resize += GlControl_Resize;
            GlControl.Paint += GlControl_Paint;

            // Redraw the screen every 1/20 of a second.
            _timer = new System.Windows.Forms.Timer();
            _timer.Tick += (sender, e) =>
            {
                _angle += 0.5f;
                Render();
            };
            _timer.Interval = 50;   // 1000 ms per sec / 50 ms per frame = 20 FPS
            _timer.Start();

            // Ensure that the viewport and projection matrix are set correctly initially.
            GlControl_Resize(GlControl, EventArgs.Empty);
        }

        private void GlControl_Resize(object? sender, EventArgs e)
        {

            GlControl.MakeCurrent();

            if (GlControl.ClientSize.Height == 0)
                GlControl.ClientSize = new Size(GlControl.ClientSize.Width, 1);

            GL.Viewport(0, 0, GlControl.ClientSize.Width, GlControl.ClientSize.Height);

            float aspect_ratio = Math.Max(GlControl.ClientSize.Width, 1) / (float)Math.Max(GlControl.ClientSize.Height, 1);
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }

        private void GlControl_Paint(object? sender, PaintEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            GlControl.MakeCurrent();

            GL.ClearColor(Color4.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);

            Matrix4 lookat = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            GL.Rotate(_angle, 0.0f, 1.0f, 0.0f);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Begin(PrimitiveType.Quads);
            GL.Color4(Color4.Silver);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);

            GL.Color4(Color4.Honeydew);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);

            GL.Color4(Color4.Moccasin);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);

            GL.Color4(Color4.IndianRed);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);

            GL.Color4(Color4.PaleVioletRed);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);

            GL.Color4(Color4.ForestGreen);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);

            GL.End();

            GlControl.SwapBuffers();
        }
        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            //clear GPU buffers
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            //set size of Gl Control scaled to reasonable viewing size (used to display grid)
            GL.Viewport(0, 0, 1000, (int)(1000 * farfield.Height / farfield.Width));

            //draw the Grid based on cells
            int? n1, n2, n3, n4;

            //UI layer dealing directly with the repository
            foreach (var cell in Repository.CellList)
            {
                n1 = cell.V1;
                n2 = cell.V2;
                n3 = cell.V3;
                n4 = cell.V4;

                // This is old school OpenGl 1.0
                // Wire frame
                GL.Begin(PrimitiveType.LineLoop);
                GL.Color3(255, 255, 255);
                GL.Vertex2(data.NodeV(n1).R.X, data.NodeV(n1).R.Y);
                GL.Vertex2(data.NodeV(n2).R.X, data.NodeV(n2).R.Y);
                GL.Vertex2(data.NodeV(n3).R.X, data.NodeV(n3).R.Y);
                if (n4 != null) GL.Vertex2(data.NodeV(n4).R.X, data.NodeV(n4).R.Y);
                GL.End();

                ////Optional fill for debugging cell creation
                //GL.Begin(PrimitiveType.Quads);
                //GL.Color3(Color.Red);
                //GL.Vertex2((short)data.NodeV(n1).R.X, (short)data.NodeV(n1).R.Y);
                //GL.Vertex2((short)data.NodeV(n2).R.X, (short)data.NodeV(n2).R.Y);
                //GL.Vertex2((short)data.NodeV(n3).R.X, (short)data.NodeV(n3).R.Y);
                //if (n4 != null) GL.Vertex2((short)data.NodeV(n4).R.X, (short)data.NodeV(n4).R.Y);
                //GL.End();

            }

            GlControl.SwapBuffers();

        }

        private void Form2_Closing(object sender, EventArgs e)
        {
            //update settings.xml
            Settings.WriteSettings(farfield);
        }

        private void EventCompletion()
        {
            //initiates refresh of the form and repaint of the GL drawing control
            Refresh();
            GlControl.Invalidate();
            StatusMessage(MeshConstants.MSGCOMPLETE);
        }

        private void UpdateGLSize()
        {
            //set the GL control size
            GlControl.Width = 1000;
            GlControl.Height = (int)(1000 * farfield.Height / farfield.Width);
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

            // Repaint
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
            comboBox1.Enabled = false;

            //save settings
            Settings.WriteSettings(farfield);

            //set window drawing size
            UpdateGLSize();

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

            // Repaint
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

        private int StatusMessage(string s)
        {
            // displays a status message
            TextBoxStatus.Text = s;
            TextBoxStatus.Refresh();
            return 0;
        }

        /// <summary>
        /// Locks out buttons so that grid can't be changed
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
        private void button10_Click(object sender, EventArgs e)
        {

            //reset lists of nodes and cells
            ResetData resetData = Program.container.GetInstance<ResetData>();
            resetData.Logic();

            //set buttons to default
            ResetButtonStatus();

            //unlock the input boxes
            TextBoxWidth.Enabled = true;
            TextBoxHeight.Enabled = true;
            comboBox1.Enabled = true;

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

        private void comboBox1_Validating(object sender, CancelEventArgs e)
        {
            farfield.Gridtype = (GridType)comboBox1.SelectedItem;
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
    }
}
