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
            ButtonRun = new Button();
            TextBoxTimestep = new TextBox();
            TextBoxCalcTime = new TextBox();
            TextBoxUVelocity = new TextBox();
            TextBoxNu = new TextBox();
            TextBoxRho = new TextBox();
            LabelTimestep = new Label();
            label2 = new Label();
            LabelCalcTime = new Label();
            LabelUVelocity = new Label();
            LabelNu = new Label();
            LabelRho = new Label();
            RadioButtonU = new RadioButton();
            RadioButtonV = new RadioButton();
            LabelReynolds = new Label();
            label15 = new Label();
            panel1 = new Panel();
            RadioButtonTest = new RadioButton();
            RadioButtonP = new RadioButton();
            LabelCFL = new Label();
            LabelActualElapsed = new Label();
            saveFileDialog1 = new SaveFileDialog();
            ButtonSavePlot = new Button();
            ButtonPreCalc = new Button();
            TextBoxStatus = new TextBox();
            TextBoxPIteration = new TextBox();
            LabelPIteration = new Label();
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
            // ButtonRun
            // 
            ButtonRun.Location = new Point(1485, 490);
            ButtonRun.Name = "ButtonRun";
            ButtonRun.Size = new Size(94, 37);
            ButtonRun.TabIndex = 1;
            ButtonRun.Text = "Run";
            ButtonRun.UseVisualStyleBackColor = true;
            ButtonRun.Click += ButtonRun_Click;
            // 
            // TextBoxTimestep
            // 
            TextBoxTimestep.Location = new Point(1381, 38);
            TextBoxTimestep.Name = "TextBoxTimestep";
            TextBoxTimestep.Size = new Size(125, 27);
            TextBoxTimestep.TabIndex = 2;
            TextBoxTimestep.Validating += TextBoxTimestep_Validating;
            // 
            // TextBoxCalcTime
            // 
            TextBoxCalcTime.Location = new Point(1666, 38);
            TextBoxCalcTime.Name = "TextBoxCalcTime";
            TextBoxCalcTime.Size = new Size(91, 27);
            TextBoxCalcTime.TabIndex = 3;
            TextBoxCalcTime.Validating += TextBoxCalcTime_Validating;
            // 
            // TextBoxUVelocity
            // 
            TextBoxUVelocity.Location = new Point(1381, 105);
            TextBoxUVelocity.Name = "TextBoxUVelocity";
            TextBoxUVelocity.Size = new Size(125, 27);
            TextBoxUVelocity.TabIndex = 4;
            TextBoxUVelocity.Validating += TextBoxUVelocity_Validating;
            // 
            // TextBoxNu
            // 
            TextBoxNu.Location = new Point(1361, 332);
            TextBoxNu.Name = "TextBoxNu";
            TextBoxNu.Size = new Size(125, 27);
            TextBoxNu.TabIndex = 8;
            TextBoxNu.Validating += TextBoxNu_Validating;
            // 
            // TextBoxRho
            // 
            TextBoxRho.Location = new Point(1361, 377);
            TextBoxRho.Name = "TextBoxRho";
            TextBoxRho.Size = new Size(125, 27);
            TextBoxRho.TabIndex = 9;
            TextBoxRho.Validating += TextBoxRho_Validating;
            // 
            // LabelTimestep
            // 
            LabelTimestep.AutoSize = true;
            LabelTimestep.Location = new Point(1273, 38);
            LabelTimestep.Name = "LabelTimestep";
            LabelTimestep.Size = new Size(90, 20);
            LabelTimestep.TabIndex = 10;
            LabelTimestep.Text = "Timestep (s)";
            // 
            // label2
            // 
            label2.Location = new Point(0, 0);
            label2.Name = "label2";
            label2.Size = new Size(100, 23);
            label2.TabIndex = 13;
            // 
            // LabelCalcTime
            // 
            LabelCalcTime.AutoSize = true;
            LabelCalcTime.Location = new Point(1563, 38);
            LabelCalcTime.Name = "LabelCalcTime";
            LabelCalcTime.Size = new Size(94, 20);
            LabelCalcTime.TabIndex = 12;
            LabelCalcTime.Text = "Calc Time (s)";
            // 
            // LabelUVelocity
            // 
            LabelUVelocity.AutoSize = true;
            LabelUVelocity.Location = new Point(1253, 105);
            LabelUVelocity.Name = "LabelUVelocity";
            LabelUVelocity.Size = new Size(113, 20);
            LabelUVelocity.TabIndex = 14;
            LabelUVelocity.Text = "U velocity (m/s)";
            // 
            // LabelNu
            // 
            LabelNu.AutoSize = true;
            LabelNu.Location = new Point(1308, 332);
            LabelNu.Name = "LabelNu";
            LabelNu.Size = new Size(28, 20);
            LabelNu.TabIndex = 18;
            LabelNu.Text = "Nu";
            // 
            // LabelRho
            // 
            LabelRho.AutoSize = true;
            LabelRho.Location = new Point(1308, 380);
            LabelRho.Name = "LabelRho";
            LabelRho.Size = new Size(35, 20);
            LabelRho.TabIndex = 19;
            LabelRho.Text = "Rho";
            // 
            // RadioButtonU
            // 
            RadioButtonU.AutoSize = true;
            RadioButtonU.Checked = true;
            RadioButtonU.Location = new Point(33, 48);
            RadioButtonU.Name = "RadioButtonU";
            RadioButtonU.Size = new Size(40, 24);
            RadioButtonU.TabIndex = 28;
            RadioButtonU.TabStop = true;
            RadioButtonU.Text = "U";
            RadioButtonU.UseVisualStyleBackColor = true;
            RadioButtonU.Click += RadioButtonU_Click;
            // 
            // RadioButtonV
            // 
            RadioButtonV.AutoSize = true;
            RadioButtonV.Location = new Point(79, 48);
            RadioButtonV.Name = "RadioButtonV";
            RadioButtonV.Size = new Size(39, 24);
            RadioButtonV.TabIndex = 29;
            RadioButtonV.Text = "V";
            RadioButtonV.UseVisualStyleBackColor = true;
            RadioButtonV.Click += RadioButtonV_Click;
            // 
            // LabelReynolds
            // 
            LabelReynolds.AutoSize = true;
            LabelReynolds.Location = new Point(1533, 336);
            LabelReynolds.Name = "LabelReynolds";
            LabelReynolds.Size = new Size(134, 20);
            LabelReynolds.TabIndex = 31;
            LabelReynolds.Text = "Reynolds Number: ";
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
            panel1.Controls.Add(RadioButtonTest);
            panel1.Controls.Add(RadioButtonP);
            panel1.Controls.Add(RadioButtonV);
            panel1.Controls.Add(RadioButtonU);
            panel1.Location = new Point(1362, 607);
            panel1.Name = "panel1";
            panel1.Size = new Size(334, 119);
            panel1.TabIndex = 33;
            // 
            // RadioButtonTest
            // 
            RadioButtonTest.AutoSize = true;
            RadioButtonTest.Location = new Point(214, 49);
            RadioButtonTest.Name = "RadioButtonTest";
            RadioButtonTest.Size = new Size(56, 24);
            RadioButtonTest.TabIndex = 32;
            RadioButtonTest.TabStop = true;
            RadioButtonTest.Text = "Test";
            RadioButtonTest.UseVisualStyleBackColor = true;
            RadioButtonTest.Click += RadioButtonTest_Click;
            // 
            // RadioButtonP
            // 
            RadioButtonP.AutoSize = true;
            RadioButtonP.Location = new Point(124, 48);
            RadioButtonP.Name = "RadioButtonP";
            RadioButtonP.Size = new Size(84, 24);
            RadioButtonP.TabIndex = 31;
            RadioButtonP.TabStop = true;
            RadioButtonP.Text = "Pressure";
            RadioButtonP.UseVisualStyleBackColor = true;
            RadioButtonP.Click += RadioButtonP_Click;
            // 
            // LabelCFL
            // 
            LabelCFL.AutoSize = true;
            LabelCFL.Location = new Point(1533, 377);
            LabelCFL.Name = "LabelCFL";
            LabelCFL.Size = new Size(39, 20);
            LabelCFL.TabIndex = 34;
            LabelCFL.Text = "CFL :";
            // 
            // LabelActualElapsed
            // 
            LabelActualElapsed.AutoSize = true;
            LabelActualElapsed.Location = new Point(1533, 414);
            LabelActualElapsed.Name = "LabelActualElapsed";
            LabelActualElapsed.Size = new Size(81, 20);
            LabelActualElapsed.TabIndex = 35;
            LabelActualElapsed.Text = "Calc Time: ";
            // 
            // ButtonSavePlot
            // 
            ButtonSavePlot.Location = new Point(1609, 490);
            ButtonSavePlot.Name = "ButtonSavePlot";
            ButtonSavePlot.Size = new Size(87, 37);
            ButtonSavePlot.TabIndex = 36;
            ButtonSavePlot.Text = "Save Plot";
            ButtonSavePlot.UseVisualStyleBackColor = true;
            ButtonSavePlot.Click += ButtonSavePlot_Click;
            // 
            // ButtonPreCalc
            // 
            ButtonPreCalc.Location = new Point(1361, 490);
            ButtonPreCalc.Name = "ButtonPreCalc";
            ButtonPreCalc.Size = new Size(103, 37);
            ButtonPreCalc.TabIndex = 39;
            ButtonPreCalc.Text = "Precalc";
            ButtonPreCalc.UseVisualStyleBackColor = true;
            ButtonPreCalc.Click += ButtonPreCalc_Click;
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
            // TextBoxPIteration
            // 
            TextBoxPIteration.Location = new Point(1381, 212);
            TextBoxPIteration.Name = "TextBoxPIteration";
            TextBoxPIteration.Size = new Size(125, 27);
            TextBoxPIteration.TabIndex = 41;
            TextBoxPIteration.Validating += TextBoxPIteration_Validating;
            // 
            // LabelPIteration
            // 
            LabelPIteration.AutoSize = true;
            LabelPIteration.Location = new Point(1282, 215);
            LabelPIteration.Name = "LabelPIteration";
            LabelPIteration.Size = new Size(77, 20);
            LabelPIteration.TabIndex = 42;
            LabelPIteration.Text = "P iteration";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1801, 863);
            Controls.Add(LabelPIteration);
            Controls.Add(TextBoxPIteration);
            Controls.Add(TextBoxStatus);
            Controls.Add(ButtonPreCalc);
            Controls.Add(ButtonSavePlot);
            Controls.Add(LabelActualElapsed);
            Controls.Add(LabelCFL);
            Controls.Add(panel1);
            Controls.Add(label15);
            Controls.Add(LabelReynolds);
            Controls.Add(LabelRho);
            Controls.Add(LabelNu);
            Controls.Add(LabelUVelocity);
            Controls.Add(LabelCalcTime);
            Controls.Add(label2);
            Controls.Add(LabelTimestep);
            Controls.Add(TextBoxRho);
            Controls.Add(TextBoxNu);
            Controls.Add(TextBoxUVelocity);
            Controls.Add(TextBoxCalcTime);
            Controls.Add(TextBoxTimestep);
            Controls.Add(ButtonRun);
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
        private Button ButtonRun;
        private TextBox TextBoxTimestep;
        private TextBox TextBoxCalcTime;
        private TextBox TextBoxUVelocity;
        private TextBox TextBoxNu;
        private TextBox TextBoxRho;
        private Label LabelTimestep;
        private Label label2;
        private Label LabelCalcTime;
        private Label LabelUVelocity;
        private Label LabelNu;
        private Label LabelRho;
        private RadioButton RadioButtonU;
        private RadioButton RadioButtonV;
        private Label LabelReynolds;
        private Label label15;
        private Panel panel1;
        private Label LabelCFL;
        private Label LabelActualElapsed;
        private RadioButton RadioButtonP;
        private RadioButton RadioButtonTest;
        private SaveFileDialog saveFileDialog1;
        private Button ButtonSavePlot;
        private Button ButtonPreCalc;
        private TextBox TextBoxStatus;
        private TextBox TextBoxPIteration;
        private Label LabelPIteration;
    }
}