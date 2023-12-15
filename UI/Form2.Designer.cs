namespace CFDSolv
{
    partial class Form2
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Panel1 = new Panel();
            LabelTiling = new Label();
            TextBoxTiling = new ComboBox();
            label1 = new Label();
            label15 = new Label();
            label14 = new Label();
            label13 = new Label();
            LabelGrid = new Label();
            TextBoxGrid = new ComboBox();
            ButtonReset = new Button();
            ButtonFinalize = new Button();
            LabelStatus = new Label();
            TextBoxStatus = new TextBox();
            ButtonCFD = new Button();
            ButtonDelaunay = new Button();
            TextBoxHeight = new TextBox();
            ButtonRefine = new Button();
            TextBoxWidth = new TextBox();
            ButtonSmooth = new Button();
            ButtonRedistribute = new Button();
            LabelHeight = new Label();
            ButtonBuild = new Button();
            LabelWidth = new Label();
            TextBoxSmoothing = new TextBox();
            LabelSmoothing = new Label();
            OpenFileDialog1 = new OpenFileDialog();
            GlControl = new OpenTK.WinForms.GLControl();
            Panel1.SuspendLayout();
            SuspendLayout();
            // 
            // Panel1
            // 
            Panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Panel1.BorderStyle = BorderStyle.FixedSingle;
            Panel1.Controls.Add(LabelTiling);
            Panel1.Controls.Add(TextBoxTiling);
            Panel1.Controls.Add(label1);
            Panel1.Controls.Add(label15);
            Panel1.Controls.Add(label14);
            Panel1.Controls.Add(label13);
            Panel1.Controls.Add(LabelGrid);
            Panel1.Controls.Add(TextBoxGrid);
            Panel1.Controls.Add(ButtonReset);
            Panel1.Controls.Add(ButtonFinalize);
            Panel1.Controls.Add(LabelStatus);
            Panel1.Controls.Add(TextBoxStatus);
            Panel1.Controls.Add(ButtonCFD);
            Panel1.Controls.Add(ButtonDelaunay);
            Panel1.Controls.Add(TextBoxHeight);
            Panel1.Controls.Add(ButtonRefine);
            Panel1.Controls.Add(TextBoxWidth);
            Panel1.Controls.Add(ButtonSmooth);
            Panel1.Controls.Add(ButtonRedistribute);
            Panel1.Controls.Add(LabelHeight);
            Panel1.Controls.Add(ButtonBuild);
            Panel1.Controls.Add(LabelWidth);
            Panel1.Controls.Add(TextBoxSmoothing);
            Panel1.Controls.Add(LabelSmoothing);
            Panel1.Location = new Point(1328, 6);
            Panel1.Margin = new Padding(4);
            Panel1.Name = "Panel1";
            Panel1.Size = new Size(322, 848);
            Panel1.TabIndex = 0;
            // 
            // LabelTiling
            // 
            LabelTiling.AutoSize = true;
            LabelTiling.Location = new Point(21, 438);
            LabelTiling.Name = "LabelTiling";
            LabelTiling.Size = new Size(40, 20);
            LabelTiling.TabIndex = 44;
            LabelTiling.Text = "Type";
            // 
            // TextBoxTiling
            // 
            TextBoxTiling.FormattingEnabled = true;
            TextBoxTiling.Location = new Point(111, 435);
            TextBoxTiling.Name = "TextBoxTiling";
            TextBoxTiling.Size = new Size(143, 28);
            TextBoxTiling.TabIndex = 43;
            TextBoxTiling.Validating += TextBoxTiling_Validating;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(21, 399);
            label1.Name = "label1";
            label1.Size = new Size(134, 20);
            label1.TabIndex = 42;
            label1.Text = "3. Extended Tiling";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label15.Location = new Point(18, 499);
            label15.Name = "label15";
            label15.Size = new Size(82, 20);
            label15.TabIndex = 41;
            label15.Text = "4. Commit";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label14.Location = new Point(26, 212);
            label14.Name = "label14";
            label14.Size = new Size(89, 20);
            label14.TabIndex = 40;
            label14.Text = "2. Optimize";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label13.Location = new Point(22, 124);
            label13.Name = "label13";
            label13.Size = new Size(100, 20);
            label13.TabIndex = 39;
            label13.Text = "1. Start Build";
            // 
            // LabelGrid
            // 
            LabelGrid.AutoSize = true;
            LabelGrid.Location = new Point(20, 76);
            LabelGrid.Name = "LabelGrid";
            LabelGrid.Size = new Size(72, 20);
            LabelGrid.TabIndex = 38;
            LabelGrid.Text = "Base Grid";
            // 
            // TextBoxGrid
            // 
            TextBoxGrid.FormattingEnabled = true;
            TextBoxGrid.Location = new Point(111, 73);
            TextBoxGrid.Name = "TextBoxGrid";
            TextBoxGrid.Size = new Size(143, 28);
            TextBoxGrid.TabIndex = 37;
            TextBoxGrid.Validating += TextBoxGrid_Validating;
            // 
            // ButtonReset
            // 
            ButtonReset.Location = new Point(18, 612);
            ButtonReset.Name = "ButtonReset";
            ButtonReset.Size = new Size(106, 28);
            ButtonReset.TabIndex = 36;
            ButtonReset.Text = "RESET";
            ButtonReset.UseVisualStyleBackColor = true;
            ButtonReset.Click += ButtonReset_Click;
            // 
            // ButtonFinalize
            // 
            ButtonFinalize.Location = new Point(18, 536);
            ButtonFinalize.Name = "ButtonFinalize";
            ButtonFinalize.Size = new Size(97, 28);
            ButtonFinalize.TabIndex = 33;
            ButtonFinalize.Text = "Finalize";
            ButtonFinalize.UseVisualStyleBackColor = true;
            ButtonFinalize.Click += ButtonFinalize_Click;
            // 
            // LabelStatus
            // 
            LabelStatus.AutoSize = true;
            LabelStatus.Location = new Point(12, 691);
            LabelStatus.Margin = new Padding(4, 0, 4, 0);
            LabelStatus.Name = "LabelStatus";
            LabelStatus.Size = new Size(49, 20);
            LabelStatus.TabIndex = 22;
            LabelStatus.Text = "Status";
            // 
            // TextBoxStatus
            // 
            TextBoxStatus.BackColor = SystemColors.Control;
            TextBoxStatus.BorderStyle = BorderStyle.None;
            TextBoxStatus.CausesValidation = false;
            TextBoxStatus.Location = new Point(10, 735);
            TextBoxStatus.Margin = new Padding(4);
            TextBoxStatus.Name = "TextBoxStatus";
            TextBoxStatus.ReadOnly = true;
            TextBoxStatus.Size = new Size(291, 20);
            TextBoxStatus.TabIndex = 21;
            TextBoxStatus.TabStop = false;
            // 
            // ButtonCFD
            // 
            ButtonCFD.Location = new Point(122, 536);
            ButtonCFD.Name = "ButtonCFD";
            ButtonCFD.Size = new Size(97, 28);
            ButtonCFD.TabIndex = 32;
            ButtonCFD.Text = "CFD";
            ButtonCFD.UseVisualStyleBackColor = true;
            ButtonCFD.MouseClick += ButtonCFD_Click;
            // 
            // ButtonDelaunay
            // 
            ButtonDelaunay.Location = new Point(26, 328);
            ButtonDelaunay.Margin = new Padding(4);
            ButtonDelaunay.Name = "ButtonDelaunay";
            ButtonDelaunay.Size = new Size(97, 28);
            ButtonDelaunay.TabIndex = 25;
            ButtonDelaunay.Text = "Delaunay";
            ButtonDelaunay.UseVisualStyleBackColor = true;
            ButtonDelaunay.Click += ButtonDelaunay_Click;
            // 
            // TextBoxHeight
            // 
            TextBoxHeight.Location = new Point(113, 39);
            TextBoxHeight.Margin = new Padding(4);
            TextBoxHeight.Name = "TextBoxHeight";
            TextBoxHeight.Size = new Size(68, 27);
            TextBoxHeight.TabIndex = 2;
            TextBoxHeight.Validating += TextBoxHeight_Validating;
            // 
            // ButtonRefine
            // 
            ButtonRefine.Location = new Point(26, 292);
            ButtonRefine.Margin = new Padding(4);
            ButtonRefine.Name = "ButtonRefine";
            ButtonRefine.Size = new Size(97, 28);
            ButtonRefine.TabIndex = 26;
            ButtonRefine.Text = "Refine";
            ButtonRefine.UseVisualStyleBackColor = true;
            ButtonRefine.Click += ButtonRefine_Click;
            // 
            // TextBoxWidth
            // 
            TextBoxWidth.Location = new Point(113, 5);
            TextBoxWidth.Margin = new Padding(4);
            TextBoxWidth.Name = "TextBoxWidth";
            TextBoxWidth.Size = new Size(68, 27);
            TextBoxWidth.TabIndex = 1;
            TextBoxWidth.Validating += TextBoxWidth_Validating;
            // 
            // ButtonSmooth
            // 
            ButtonSmooth.Location = new Point(130, 292);
            ButtonSmooth.Margin = new Padding(4);
            ButtonSmooth.Name = "ButtonSmooth";
            ButtonSmooth.Size = new Size(87, 28);
            ButtonSmooth.TabIndex = 27;
            ButtonSmooth.Text = "Smooth";
            ButtonSmooth.UseVisualStyleBackColor = true;
            ButtonSmooth.Click += ButtonSmooth_Click;
            // 
            // ButtonRedistribute
            // 
            ButtonRedistribute.Location = new Point(130, 328);
            ButtonRedistribute.Margin = new Padding(4);
            ButtonRedistribute.Name = "ButtonRedistribute";
            ButtonRedistribute.Size = new Size(89, 28);
            ButtonRedistribute.TabIndex = 31;
            ButtonRedistribute.Text = "Redistrib";
            ButtonRedistribute.UseVisualStyleBackColor = true;
            ButtonRedistribute.Click += ButtonRedistribute_Click;
            // 
            // LabelHeight
            // 
            LabelHeight.AutoSize = true;
            LabelHeight.Location = new Point(18, 39);
            LabelHeight.Margin = new Padding(4, 0, 4, 0);
            LabelHeight.Name = "LabelHeight";
            LabelHeight.Size = new Size(73, 20);
            LabelHeight.TabIndex = 14;
            LabelHeight.Text = "Height/m";
            // 
            // ButtonBuild
            // 
            ButtonBuild.Location = new Point(22, 148);
            ButtonBuild.Margin = new Padding(4);
            ButtonBuild.Name = "ButtonBuild";
            ButtonBuild.Size = new Size(134, 28);
            ButtonBuild.TabIndex = 30;
            ButtonBuild.Text = "Build Grid";
            ButtonBuild.UseVisualStyleBackColor = true;
            ButtonBuild.Click += ButtonBuild_Click;
            // 
            // LabelWidth
            // 
            LabelWidth.AutoSize = true;
            LabelWidth.Location = new Point(23, 5);
            LabelWidth.Margin = new Padding(4, 0, 4, 0);
            LabelWidth.Name = "LabelWidth";
            LabelWidth.Size = new Size(68, 20);
            LabelWidth.TabIndex = 0;
            LabelWidth.Text = "Width/m";
            // 
            // TextBoxSmoothing
            // 
            TextBoxSmoothing.Location = new Point(167, 242);
            TextBoxSmoothing.Margin = new Padding(4);
            TextBoxSmoothing.Name = "TextBoxSmoothing";
            TextBoxSmoothing.Size = new Size(68, 27);
            TextBoxSmoothing.TabIndex = 23;
            TextBoxSmoothing.Validating += TextBoxSmoothingCycles_Validating;
            // 
            // LabelSmoothing
            // 
            LabelSmoothing.AutoSize = true;
            LabelSmoothing.Location = new Point(26, 242);
            LabelSmoothing.Margin = new Padding(4, 0, 4, 0);
            LabelSmoothing.Name = "LabelSmoothing";
            LabelSmoothing.Size = new Size(127, 20);
            LabelSmoothing.TabIndex = 24;
            LabelSmoothing.Text = "Smoothing Cycles";
            // 
            // OpenFileDialog1
            // 
            OpenFileDialog1.FileName = "OpenFileDialog1";
            // 
            // GlControl
            // 
            GlControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            GlControl.APIVersion = new Version(4, 0, 0, 0);
            GlControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            GlControl.IsEventDriven = true;
            GlControl.Location = new Point(7, 6);
            GlControl.Name = "GlControl";
            GlControl.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
            GlControl.Size = new Size(379, 254);
            GlControl.TabIndex = 22;
            GlControl.Text = "GlControl";
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1651, 1055);
            Controls.Add(GlControl);
            Controls.Add(Panel1);
            Margin = new Padding(4);
            Name = "Form2";
            Text = "Form2";
            Load += Form2_Load;
            Paint += Form2_Paint;
            Panel1.ResumeLayout(false);
            Panel1.PerformLayout();
            ResumeLayout(false);
        }

        private Panel Panel1;
        private TextBox TextBoxHeight;
        private TextBox TextBoxWidth;
        private Label LabelSmoothing;
        private Label LabelStatus;
        private Label LabelHeight;
        private Label LabelWidth;
        private TextBox TextBoxStatus;
        private TextBox TextBoxSmoothing;
        private Button ButtonDelaunay;
        private Button ButtonRefine;
        private Button ButtonSmooth;
        private OpenFileDialog OpenFileDialog1;
        private Button ButtonBuild;
        private Button ButtonRedistribute;
        private Button ButtonCFD;
        private OpenTK.WinForms.GLControl GlControl;

        #endregion

        private Button ButtonFinalize;
        private Button ButtonReset;
        private ComboBox TextBoxGrid;
        private Label label13;
        private Label LabelGrid;
        private Label label15;
        private Label label14;
        private Label LabelTiling;
        private ComboBox TextBoxTiling;
        private Label label1;
    }
}