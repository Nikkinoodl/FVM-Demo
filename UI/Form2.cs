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

        public Farfield Farfield { get { return farfield; } }


        private System.Windows.Forms.Timer _timer = null!;
        private float _angle = 0.0f;

        public Form2(IDataAccessService data)
        {
            this.data = data;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            //Get values for combo box and make a default selection
            comboBox1.DataSource = Enum.GetValues(typeof(GridType));
            comboBox1.SelectedItem = GridType.Triangles;

            // initialize settings and read from settings.xml file
            settings.CreateSettings();

            // Initial settings copied to public properties that will persist through the class
            farfield.Height = settings.Height;
            farfield.Width = settings.Width;
            farfield.Scale = settings.Scale;
            farfield.Layers = settings.Layers;
            farfield.Cellheight = settings.Cellheight;
            farfield.Cellfactor = settings.Cellfactor;
            farfield.Nodetrade = settings.Nodetrade;
            farfield.Expansionpower = settings.Expansionpower;
            farfield.Offset = settings.Offset;
            farfield.Smoothingcycles = settings.Smoothingcycles;
            farfield.Filename = settings.Filename;
            farfield.Gridtype = settings.Gridtype;

            // populate text boxes with farfield settings
            TextBoxHeight.Text = farfield.Height.ToString();
            TextBoxWidth.Text = farfield.Width.ToString();
            TextBoxScale.Text = farfield.Scale.ToString();
            TextBoxLayers.Text = farfield.Layers.ToString();
            TextBoxCellHeight.Text = farfield.Cellheight.ToString();
            TextBoxCellFactor.Text = farfield.Cellfactor.ToString();
            TextBoxNodeTrade.Text = farfield.Nodetrade.ToString();
            TextBoxExpansionPower.Text = farfield.Expansionpower.ToString();
            TextBoxOffset.Text = farfield.Offset.ToString();
            TextBoxSmoothingCycles.Text = farfield.Smoothingcycles.ToString();
            TextBoxFileName.Text = farfield.Filename;

            WindowState = FormWindowState.Maximized;
            GL.ClearColor(Color4.White);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Ortho(0, farfield.Width, 0, farfield.Height, -1, 1);
            GL.Viewport(0, 0, farfield.Width, farfield.Height);

            var ToolTip1 = new ToolTip();
            var ToolTip2 = new ToolTip();
            var ToolTip3 = new ToolTip();
            var ToolTip4 = new ToolTip();
            var ToolTip5 = new ToolTip();
            var ToolTip6 = new ToolTip();
            var ToolTip7 = new ToolTip();
            var ToolTip8 = new ToolTip();
            var ToolTip9 = new ToolTip();
            var ToolTip10 = new ToolTip();

            ToolTip1.SetToolTip(TextBoxWidth, "The width of the farfield in meters");
            ToolTip2.SetToolTip(TextBoxHeight, "The height of the farfield in meters");
            ToolTip3.SetToolTip(TextBoxScale, "The amount by which to scale the airfoil");
            ToolTip4.SetToolTip(TextBoxLayers, "The number of layers of cells to create around the airfoil in the first pass");
            ToolTip5.SetToolTip(TextBoxCellHeight, "The initial cell layer height in meters");
            ToolTip6.SetToolTip(TextBoxCellFactor, "A factor used to fine-tune the layer height");
            ToolTip7.SetToolTip(TextBoxExpansionPower, "The amount by which to increase the height of subsequent layers: height = cellheight*cellfactor*layernumber^expansionpower");
            ToolTip8.SetToolTip(TextBoxNodeTrade, "Number of boundary nodes to reallocate between vertical/horizontal edges");
            ToolTip9.SetToolTip(TextBoxOffset, "Number of nodes to offsetp between layers - reduces the initial grid distortion");
            ToolTip10.SetToolTip(TextBoxSmoothingCycles, "Number of iterations of the smoothing routine");
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
            // Clear GPU buffers
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            // Set size of Gl Control scaled to reasonable viewing size (used to display grid)
            GL.Viewport(0, 0, 1000, 1000 * farfield.Height / farfield.Width);

            // Draw the Grid based on cells
            int? n1, n2, n3, n4;

            // UI layer dealing directly with the repository
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
            // on closing the form update settings.xml
            Settings.WriteSettings(farfield);
        }

        private void EventCompletion()
        {
            // initiates refresh of the form and repaint of the GL drawing control
            Refresh();
            GlControl.Invalidate();
            StatusMessage(MeshConstants.MSGCOMPLETE);
        }

        private void UpdateFarfield()
        {
            // Obtain form data and perform a simple validation. Default to saved settings in 
            // case of error
            //farfield.Height = ValidateEntry<int>(ref TextBoxHeight, ref settings.Height);
            //farfield.Width = ValidateEntry<int>(ref TextBoxWidth, ref settings.Width);
            //farfield.Scale = ValidateEntry<short>(ref TextBoxScale, ref settings.Scale);
            //farfield.Layers = ValidateEntry<short>(ref TextBoxLayers, ref settings.Layers);
            //farfield.Cellheight = ValidateEntry<short>(ref TextBoxCellHeight, ref settings.Cellheight);
            //farfield.Cellfactor = ValidateEntry<double>(ref TextBoxCellFactor, ref settings.Cellfactor);
            //farfield.Nodetrade = ValidateEntry<short>(ref TextBoxNodeTrade, ref settings.Nodetrade);
            //farfield.Expansionpower = ValidateEntry<double>(ref TextBoxExpansionPower, ref settings.Expansionpower);
            //farfield.Offset = ValidateEntry<short>(ref TextBoxOffset, ref settings.Offset);
            //farfield.Smoothingcycles = ValidateEntry<short>(ref TextBoxSmoothingCycles, ref settings.Smoothingcycles);
            //farfield.Filename = TextBoxFileName.Text;

            farfield.Height = int.Parse(TextBoxHeight.Text);
            farfield.Width = int.Parse(TextBoxWidth.Text);
            farfield.Scale = float.Parse(TextBoxScale.Text);
            farfield.Layers = short.Parse(TextBoxLayers.Text);
            farfield.Cellheight = float.Parse(TextBoxCellHeight.Text);
            farfield.Cellfactor = double.Parse(TextBoxCellFactor.Text);
            farfield.Nodetrade = short.Parse(TextBoxNodeTrade.Text);
            farfield.Expansionpower = double.Parse(TextBoxExpansionPower.Text);
            farfield.Offset = short.Parse(TextBoxOffset.Text);
            farfield.Smoothingcycles = short.Parse(TextBoxSmoothingCycles.Text);
            farfield.Filename = TextBoxFileName.Text;
            farfield.Gridtype = (GridType)comboBox1.SelectedItem;
        }

        private void UpdateGLSize()
        {
            // set the GL control size

            GlControl.Width = 1000;
            GlControl.Height = 1000 * farfield.Height / farfield.Width;
        }

        /// <summary>
        /// Builds grid around an airfoil
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {

            StatusMessage(MeshConstants.MSGINITIALIZE);

            // pull in any changed farfield values
            UpdateFarfield();

            //take this opportunity to save the settings
            Settings.WriteSettings(farfield);

            // set window drawing size
            UpdateGLSize();

            StatusMessage(MeshConstants.MSGCONSTRUCT);

            // call the logic layer - here we initiate the actual work of building the grid
            // the first line gets the object by type from the container
            Build build = Program.container.GetInstance<Build>();
            build.Logic(farfield);

            // toggle the empty grid build button to prevent accidents
            if (Button6.Enabled == false)
            {
                Button6.Enabled = true;
            }
            else
            {
                Button6.Enabled = false;
            }

            //Enable buttons for grid refinement
            Button2.Enabled = true;
            Button3.Enabled = true;
            Button4.Enabled = true;
            Button7.Enabled = true;
            Button9.Enabled = true;

            // Repaint
            EventCompletion();
        }

        /// <summary>
        ///  Runs delaunay triangulation on triangular grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_Click(object sender, EventArgs e)
        {

            StatusMessage(MeshConstants.MSGDELAUNAY);

            // call the logic layer - here we initiate the grid optimization
            // the first line gets the object by type from the container
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

            // pull in any changed farfield values
            UpdateFarfield();

            // call the logic layer - here we initiate grid refining
            Split split = Program.container.GetInstance<Split>();
            split.Logic(farfield);

            // Repaint
           
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

            // pull in any changed farfield values
            UpdateFarfield();

            // call the logic layer - here we initiate grid smoothing
            // the first line gets the object by type from the container
            Smooth smooth = Program.container.GetInstance<Smooth>();
            smooth.Logic(farfield);

            // Repaint
            EventCompletion();
        }

        /// <summary>
        /// Initiates load of airfoil data from selected file when OK button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

            Stream strm;
            strm = OpenFileDialog1.OpenFile();
            TextBoxFileName.Text = OpenFileDialog1.FileName.ToString();
            if (!(strm == null))
            {
                // insert additionalcode to process the file data here
                strm.Close();
                MessageBox.Show(MeshConstants.MSGLOADED);
            }
        }

        /// <summary>
        /// Runs the open file dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button5_Click(object sender, EventArgs e)
        {

            OpenFileDialog1.Title = MeshConstants.DIALOGTITLE;
            OpenFileDialog1.InitialDirectory = MeshConstants.FILELOCATION;
            OpenFileDialog1.ShowDialog();
        }

        /// <summary>
        /// Builds grid for an empty space with no airfoil present
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button6_Click(object sender, EventArgs e)
        {

            StatusMessage(MeshConstants.MSGINITIALIZE);

            // pull in any changed farfield values
            UpdateFarfield();

            //take this opportunity to save the settings
            Settings.WriteSettings(farfield);

            // set window drawing size
            UpdateGLSize();

            // call the logic layer - here we do the actual work of building the empty grid
            // the first line gets the object by type from the container
            EmptySpace emptySpace = Program.container.GetInstance<EmptySpace>();
            emptySpace.Logic(farfield);

            // toggle the airfoil build button to prevent accidents
            if (Button1.Enabled == false)
            {
                Button1.Enabled = true;
            }
            else
            {
                Button1.Enabled = false;
            }

            //Enable buttons for grid refinement
            Button2.Enabled = true;
            Button3.Enabled = true;
            Button4.Enabled = true;
            Button7.Enabled = true;
            Button9.Enabled = true;

            // Repaint
            EventCompletion();
        }

        /// <summary>
        /// Redistributes nodes on edges of farfield - used for triangular grids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button7_Click(object sender, EventArgs e)
        {

            StatusMessage(MeshConstants.MSGREDISTRIBUTE);

            // pull in any changed farfield values
            // UpdateFarfield()

            // call the logic layer - here we initiate redistribution of the edge nodes
            // the first line gets the object by type from the container
            Redistribute redistribute = Program.container.GetInstance<Redistribute>();
            redistribute.Logic(farfield);

            // Repaint
            EventCompletion();
        }

        private int StatusMessage(string s)
        {
            // displays a status message in the form
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
            //At this point we don't want to make any more changes so disable buttons
            Button1.Enabled = false;
            Button2.Enabled = false;
            Button3.Enabled = false;
            Button4.Enabled = false;
            Button5.Enabled = false;
            Button6.Enabled = false;
            Button7.Enabled = false;

            //Message to indicate whether grid element is finalized
            StatusMessage(MeshConstants.MSGFINALIZED);

            //For later:
            //      Optionally save the nodes and cells (filesystem, database?)
            //      Optionally change airfoil loader to also load grid details
            //      Have the ability to replicate the farfield in any direction
        }

        /// <summary>
        /// Opens new form from where we run CFD calcs and display results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button8_MouseClick(object sender, MouseEventArgs e)
        {

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

            //OK to re-enable buttons
            Button1.Enabled = true;
            Button5.Enabled = true;
            Button6.Enabled = true;

        }

        //private T ValidateEntry<T>(ref object value, ref object fallback)
        //{
        //    // On success return the value converted to specified type. On failure return the fallback value and highlight
        //    // the box in yellow.
        //    try
        //    {
        //        value.BackColor = Color.White;
        //        return (T)value.Text;
        //    }
        //    catch (Exception ex)
        //    {
        //        value.BackColor = Color.Yellow;
        //        return fallback;
        //    }
        //}
    }
}
