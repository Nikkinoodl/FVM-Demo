namespace CFDSolv
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            plot1 = new OxyPlot.WindowsForms.PlotView();
            button1 = new Button();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            textBox7 = new TextBox();
            textBox8 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label8 = new Label();
            label9 = new Label();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            label14 = new Label();
            label15 = new Label();
            panel1 = new Panel();
            radioButton4 = new RadioButton();
            radioButton3 = new RadioButton();
            label5 = new Label();
            label6 = new Label();
            saveFileDialog1 = new SaveFileDialog();
            button2 = new Button();
            button3 = new Button();
            TextBoxStatus = new TextBox();
            textBox4 = new TextBox();
            label10 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // plot1
            // 
            plot1.ForeColor = SystemColors.WindowText;
            plot1.Location = new Point(0, 0);
            plot1.Name = "plot1";
            plot1.PanCursor = Cursors.Hand;
            plot1.Size = new Size(905, 782);
            plot1.TabIndex = 0;
            plot1.Text = "plot1";
            plot1.ZoomHorizontalCursor = Cursors.SizeWE;
            plot1.ZoomRectangleCursor = Cursors.SizeNWSE;
            plot1.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // button1
            // 
            button1.Location = new Point(1485, 490);
            button1.Name = "button1";
            button1.Size = new Size(94, 37);
            button1.TabIndex = 1;
            button1.Text = "Run";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(1381, 38);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(125, 27);
            textBox1.TabIndex = 2;
            textBox1.Validating += textBox1_Validating;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(1666, 38);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(91, 27);
            textBox2.TabIndex = 3;
            textBox2.Validating += textBox2_Validating;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(1381, 105);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(125, 27);
            textBox3.TabIndex = 4;
            textBox3.Validating += textBox3_Validating;
            // 
            // textBox7
            // 
            textBox7.Location = new Point(1361, 332);
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(125, 27);
            textBox7.TabIndex = 8;
            textBox7.Validating += textBox7_Validating;
            // 
            // textBox8
            // 
            textBox8.Location = new Point(1361, 377);
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(125, 27);
            textBox8.TabIndex = 9;
            textBox8.Validating += textBox8_Validating;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1273, 38);
            label1.Name = "label1";
            label1.Size = new Size(90, 20);
            label1.TabIndex = 10;
            label1.Text = "Timestep (s)";
            // 
            // label2
            // 
            label2.Location = new Point(0, 0);
            label2.Name = "label2";
            label2.Size = new Size(100, 23);
            label2.TabIndex = 13;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(1563, 38);
            label3.Name = "label3";
            label3.Size = new Size(94, 20);
            label3.TabIndex = 12;
            label3.Text = "Calc Time (s)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(1253, 105);
            label4.Name = "label4";
            label4.Size = new Size(113, 20);
            label4.TabIndex = 14;
            label4.Text = "U velocity (m/s)";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(1308, 332);
            label8.Name = "label8";
            label8.Size = new Size(28, 20);
            label8.TabIndex = 18;
            label8.Text = "Nu";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(1308, 380);
            label9.Name = "label9";
            label9.Size = new Size(35, 20);
            label9.TabIndex = 19;
            label9.Text = "Rho";
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(33, 48);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(40, 24);
            radioButton1.TabIndex = 28;
            radioButton1.TabStop = true;
            radioButton1.Text = "U";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.Click += radioButton1_Click;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(79, 48);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(39, 24);
            radioButton2.TabIndex = 29;
            radioButton2.Text = "V";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.Click += radioButton2_Click;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(1533, 336);
            label14.Name = "label14";
            label14.Size = new Size(134, 20);
            label14.TabIndex = 31;
            label14.Text = "Reynolds Number: ";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(958, 598);
            label15.Name = "label15";
            label15.Size = new Size(0, 20);
            label15.TabIndex = 32;
            // 
            // panel1
            // 
            panel1.Controls.Add(radioButton4);
            panel1.Controls.Add(radioButton3);
            panel1.Controls.Add(radioButton2);
            panel1.Controls.Add(radioButton1);
            panel1.Location = new Point(1362, 607);
            panel1.Name = "panel1";
            panel1.Size = new Size(334, 119);
            panel1.TabIndex = 33;
            // 
            // radioButton4
            // 
            radioButton4.AutoSize = true;
            radioButton4.Location = new Point(214, 49);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new Size(56, 24);
            radioButton4.TabIndex = 32;
            radioButton4.TabStop = true;
            radioButton4.Text = "Test";
            radioButton4.UseVisualStyleBackColor = true;
            radioButton4.Click += radioButton4_Click;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(124, 48);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(84, 24);
            radioButton3.TabIndex = 31;
            radioButton3.TabStop = true;
            radioButton3.Text = "Pressure";
            radioButton3.UseVisualStyleBackColor = true;
            radioButton3.Click += radioButton3_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(1533, 377);
            label5.Name = "label5";
            label5.Size = new Size(39, 20);
            label5.TabIndex = 34;
            label5.Text = "CFL :";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(1533, 414);
            label6.Name = "label6";
            label6.Size = new Size(81, 20);
            label6.TabIndex = 35;
            label6.Text = "Calc Time: ";
            // 
            // button2
            // 
            button2.Location = new Point(1609, 490);
            button2.Name = "button2";
            button2.Size = new Size(87, 37);
            button2.TabIndex = 36;
            button2.Text = "Save Plot";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(1361, 490);
            button3.Name = "button3";
            button3.Size = new Size(103, 37);
            button3.TabIndex = 39;
            button3.Text = "Precalc";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // TextBoxStatus
            // 
            TextBoxStatus.BackColor = SystemColors.Control;
            TextBoxStatus.BorderStyle = BorderStyle.None;
            TextBoxStatus.Location = new Point(1361, 544);
            TextBoxStatus.Name = "TextBoxStatus";
            TextBoxStatus.Size = new Size(338, 20);
            TextBoxStatus.TabIndex = 40;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(1381, 212);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(125, 27);
            textBox4.TabIndex = 41;
            textBox4.Validating += textBox4_Validating;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(1282, 215);
            label10.Name = "label10";
            label10.Size = new Size(77, 20);
            label10.TabIndex = 42;
            label10.Text = "P iteration";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1801, 863);
            Controls.Add(label10);
            Controls.Add(textBox4);
            Controls.Add(TextBoxStatus);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(panel1);
            Controls.Add(label15);
            Controls.Add(label14);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox8);
            Controls.Add(textBox7);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Controls.Add(plot1);
            Name = "Form1";
            Text = "Form1";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OxyPlot.WindowsForms.PlotView plot1;
        private Button button1;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox7;
        private TextBox textBox8;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label8;
        private Label label9;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private Label label14;
        private Label label15;
        private Panel panel1;
        private Label label5;
        private Label label6;
        private RadioButton radioButton3;
        private RadioButton radioButton4;
        private SaveFileDialog saveFileDialog1;
        private Button button2;
        private Button button3;
        private TextBox TextBoxStatus;
        private TextBox textBox4;
        private Label label10;
    }
}