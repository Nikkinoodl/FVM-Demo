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
            label15 = new Label();
            label14 = new Label();
            label13 = new Label();
            label12 = new Label();
            comboBox1 = new ComboBox();
            button10 = new Button();
            Button9 = new Button();
            Label10 = new Label();
            TextBoxStatus = new TextBox();
            Button8 = new Button();
            Button2 = new Button();
            TextBoxHeight = new TextBox();
            Button3 = new Button();
            TextBoxWidth = new TextBox();
            Button4 = new Button();
            Button7 = new Button();
            Label3 = new Label();
            Button6 = new Button();
            Label2 = new Label();
            TextBoxSmoothingCycles = new TextBox();
            Label11 = new Label();
            OpenFileDialog1 = new OpenFileDialog();
            GlControl = new OpenTK.WinForms.GLControl();
            Panel1.SuspendLayout();
            SuspendLayout();
            // 
            // Panel1
            // 
            Panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Panel1.BorderStyle = BorderStyle.FixedSingle;
            Panel1.Controls.Add(label15);
            Panel1.Controls.Add(label14);
            Panel1.Controls.Add(label13);
            Panel1.Controls.Add(label12);
            Panel1.Controls.Add(comboBox1);
            Panel1.Controls.Add(button10);
            Panel1.Controls.Add(Button9);
            Panel1.Controls.Add(Label10);
            Panel1.Controls.Add(TextBoxStatus);
            Panel1.Controls.Add(Button8);
            Panel1.Controls.Add(Button2);
            Panel1.Controls.Add(TextBoxHeight);
            Panel1.Controls.Add(Button3);
            Panel1.Controls.Add(TextBoxWidth);
            Panel1.Controls.Add(Button4);
            Panel1.Controls.Add(Button7);
            Panel1.Controls.Add(Label3);
            Panel1.Controls.Add(Button6);
            Panel1.Controls.Add(Label2);
            Panel1.Controls.Add(TextBoxSmoothingCycles);
            Panel1.Controls.Add(Label11);
            Panel1.Location = new Point(1328, 6);
            Panel1.Margin = new Padding(4);
            Panel1.Name = "Panel1";
            Panel1.Size = new Size(322, 848);
            Panel1.TabIndex = 0;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label15.Location = new Point(19, 468);
            label15.Name = "label15";
            label15.Size = new Size(82, 20);
            label15.TabIndex = 41;
            label15.Text = "3. Commit";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label14.Location = new Point(19, 258);
            label14.Name = "label14";
            label14.Size = new Size(89, 20);
            label14.TabIndex = 40;
            label14.Text = "2. Optimize";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label13.Location = new Point(19, 150);
            label13.Name = "label13";
            label13.Size = new Size(100, 20);
            label13.TabIndex = 39;
            label13.Text = "1. Start Build";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(20, 76);
            label12.Name = "label12";
            label12.Size = new Size(72, 20);
            label12.TabIndex = 38;
            label12.Text = "Grid Type";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(111, 73);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(143, 28);
            comboBox1.TabIndex = 37;
            // 
            // button10
            // 
            button10.Location = new Point(18, 612);
            button10.Name = "button10";
            button10.Size = new Size(106, 28);
            button10.TabIndex = 36;
            button10.Text = "RESET";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // Button9
            // 
            Button9.Location = new Point(19, 505);
            Button9.Name = "Button9";
            Button9.Size = new Size(97, 28);
            Button9.TabIndex = 33;
            Button9.Text = "Finalize";
            Button9.UseVisualStyleBackColor = true;
            Button9.Click += Button9_Click;
            // 
            // Label10
            // 
            Label10.AutoSize = true;
            Label10.Location = new Point(12, 691);
            Label10.Margin = new Padding(4, 0, 4, 0);
            Label10.Name = "Label10";
            Label10.Size = new Size(49, 20);
            Label10.TabIndex = 22;
            Label10.Text = "Status";
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
            // Button8
            // 
            Button8.Location = new Point(123, 505);
            Button8.Name = "Button8";
            Button8.Size = new Size(97, 28);
            Button8.TabIndex = 32;
            Button8.Text = "CFD";
            Button8.UseVisualStyleBackColor = true;
            Button8.MouseClick += Button8_MouseClick;
            // 
            // Button2
            // 
            Button2.Location = new Point(19, 374);
            Button2.Margin = new Padding(4);
            Button2.Name = "Button2";
            Button2.Size = new Size(97, 28);
            Button2.TabIndex = 25;
            Button2.Text = "Delaunay";
            Button2.UseVisualStyleBackColor = true;
            Button2.Click += Button2_Click;
            // 
            // TextBoxHeight
            // 
            TextBoxHeight.Location = new Point(113, 39);
            TextBoxHeight.Margin = new Padding(4);
            TextBoxHeight.Name = "TextBoxHeight";
            TextBoxHeight.Size = new Size(68, 27);
            TextBoxHeight.TabIndex = 2;
            // 
            // Button3
            // 
            Button3.Location = new Point(19, 338);
            Button3.Margin = new Padding(4);
            Button3.Name = "Button3";
            Button3.Size = new Size(97, 28);
            Button3.TabIndex = 26;
            Button3.Text = "Refine";
            Button3.UseVisualStyleBackColor = true;
            Button3.Click += Button3_Click;
            // 
            // TextBoxWidth
            // 
            TextBoxWidth.Location = new Point(113, 5);
            TextBoxWidth.Margin = new Padding(4);
            TextBoxWidth.Name = "TextBoxWidth";
            TextBoxWidth.Size = new Size(68, 27);
            TextBoxWidth.TabIndex = 1;
            // 
            // Button4
            // 
            Button4.Location = new Point(123, 338);
            Button4.Margin = new Padding(4);
            Button4.Name = "Button4";
            Button4.Size = new Size(87, 28);
            Button4.TabIndex = 27;
            Button4.Text = "Smooth";
            Button4.UseVisualStyleBackColor = true;
            Button4.Click += Button4_Click;
            // 
            // Button7
            // 
            Button7.Location = new Point(123, 374);
            Button7.Margin = new Padding(4);
            Button7.Name = "Button7";
            Button7.Size = new Size(89, 28);
            Button7.TabIndex = 31;
            Button7.Text = "Redistrib";
            Button7.UseVisualStyleBackColor = true;
            Button7.Click += Button7_Click;
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.Location = new Point(18, 39);
            Label3.Margin = new Padding(4, 0, 4, 0);
            Label3.Name = "Label3";
            Label3.Size = new Size(73, 20);
            Label3.TabIndex = 14;
            Label3.Text = "Height/m";
            // 
            // Button6
            // 
            Button6.Location = new Point(19, 174);
            Button6.Margin = new Padding(4);
            Button6.Name = "Button6";
            Button6.Size = new Size(134, 28);
            Button6.TabIndex = 30;
            Button6.Text = "Build Grid";
            Button6.UseVisualStyleBackColor = true;
            Button6.Click += Button6_Click;
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Location = new Point(23, 5);
            Label2.Margin = new Padding(4, 0, 4, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(68, 20);
            Label2.TabIndex = 0;
            Label2.Text = "Width/m";
            // 
            // TextBoxSmoothingCycles
            // 
            TextBoxSmoothingCycles.Location = new Point(160, 288);
            TextBoxSmoothingCycles.Margin = new Padding(4);
            TextBoxSmoothingCycles.Name = "TextBoxSmoothingCycles";
            TextBoxSmoothingCycles.Size = new Size(68, 27);
            TextBoxSmoothingCycles.TabIndex = 23;
            // 
            // Label11
            // 
            Label11.AutoSize = true;
            Label11.Location = new Point(19, 288);
            Label11.Margin = new Padding(4, 0, 4, 0);
            Label11.Name = "Label11";
            Label11.Size = new Size(127, 20);
            Label11.TabIndex = 24;
            Label11.Text = "Smoothing Cycles";
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
        private Label Label11;
        private Label Label10;
        private Label Label3;
        private Label Label2;
        private TextBox TextBoxStatus;
        private TextBox TextBoxSmoothingCycles;
        private Button Button2;
        private Button Button3;
        private Button Button4;
        private OpenFileDialog OpenFileDialog1;
        private Button Button6;
        private Button Button7;
        private Button Button8;
        private OpenTK.WinForms.GLControl GlControl;

        #endregion

        private Button Button9;
        private Button button10;
        private ComboBox comboBox1;
        private Label label13;
        private Label label12;
        private Label label15;
        private Label label14;
    }
}