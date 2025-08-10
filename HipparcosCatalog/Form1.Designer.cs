using OpenTK.GLControl;
using WindowsFormsExpander;

namespace HipparcosCatalog
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
            components = new System.ComponentModel.Container();
            expander1 = new Expander();
            label6 = new Label();
            numericUpDown3 = new NumericUpDown();
            checkBox4 = new CheckBox();
            label3 = new Label();
            comboBox1 = new ComboBox();
            numericUpDown1 = new NumericUpDown();
            label4 = new Label();
            panel1 = new Panel();
            expander5 = new Expander();
            richTextBox1 = new RichTextBox();
            expander4 = new Expander();
            checkedListBox1 = new CheckedListBox();
            expander3 = new Expander();
            splitContainer1 = new SplitContainer();
            comboBox2 = new ComboBox();
            label5 = new Label();
            numericUpDown2 = new NumericUpDown();
            radioButton3 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            expander2 = new Expander();
            checkBox1 = new CheckBox();
            label1 = new Label();
            trackBar1 = new TrackBar();
            checkBox2 = new CheckBox();
            label2 = new Label();
            button1 = new Button();
            expander6 = new Expander();
            checkBox3 = new CheckBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            glControl1 = new GLControl();
            timer1 = new System.Windows.Forms.Timer(components);
            timer2 = new System.Windows.Forms.Timer(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            expander1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            panel1.SuspendLayout();
            expander5.SuspendLayout();
            expander4.SuspendLayout();
            expander3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            expander2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            expander6.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // expander1
            // 
            expander1.Controls.Add(label6);
            expander1.Controls.Add(numericUpDown3);
            expander1.Controls.Add(checkBox4);
            expander1.Controls.Add(label3);
            expander1.Controls.Add(comboBox1);
            expander1.Controls.Add(numericUpDown1);
            expander1.Controls.Add(label4);
            expander1.Dock = DockStyle.Top;
            expander1.ExpandedHeight = 209;
            expander1.Location = new Point(0, 46);
            expander1.Margin = new Padding(3, 2, 3, 2);
            expander1.Name = "expander1";
            expander1.Padding = new Padding(3, 2, 3, 2);
            expander1.Size = new Size(222, 209);
            expander1.TabIndex = 15;
            expander1.Text = "Фильтры";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(21, 171);
            label6.Name = "label6";
            label6.Size = new Size(105, 15);
            label6.TabIndex = 13;
            label6.Text = "В радиус  попало:";
            // 
            // numericUpDown3
            // 
            numericUpDown3.Location = new Point(21, 137);
            numericUpDown3.Margin = new Padding(3, 2, 3, 2);
            numericUpDown3.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.Size = new Size(193, 23);
            numericUpDown3.TabIndex = 12;
            numericUpDown3.Value = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDown3.ValueChanged += numericUpDown3_ValueChanged;
            // 
            // checkBox4
            // 
            checkBox4.AutoSize = true;
            checkBox4.Location = new Point(21, 113);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new Size(143, 19);
            checkBox4.TabIndex = 11;
            checkBox4.Text = "Как центр выделения";
            checkBox4.UseVisualStyleBackColor = true;
            checkBox4.CheckedChanged += checkBox4_CheckedChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 68);
            label3.Name = "label3";
            label3.Size = new Size(104, 15);
            label3.TabIndex = 8;
            label3.Text = "По именам звезд:";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(6, 85);
            comboBox1.Margin = new Padding(3, 2, 3, 2);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(209, 23);
            comboBox1.TabIndex = 5;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(5, 43);
            numericUpDown1.Margin = new Padding(3, 2, 3, 2);
            numericUpDown1.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(209, 23);
            numericUpDown1.TabIndex = 10;
            numericUpDown1.Value = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 26);
            label4.Name = "label4";
            label4.Size = new Size(187, 15);
            label4.TabIndex = 9;
            label4.Text = "По удаленности от Солнца (с.г.):";
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(expander5);
            panel1.Controls.Add(expander4);
            panel1.Controls.Add(expander3);
            panel1.Controls.Add(expander2);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(expander1);
            panel1.Controls.Add(expander6);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(606, 2);
            panel1.Margin = new Padding(3, 2, 3, 2);
            panel1.Name = "panel1";
            panel1.Size = new Size(226, 598);
            panel1.TabIndex = 1;
            // 
            // expander5
            // 
            expander5.Controls.Add(richTextBox1);
            expander5.Dock = DockStyle.Top;
            expander5.ExpandedHeight = 119;
            expander5.Location = new Point(0, 419);
            expander5.Margin = new Padding(3, 2, 3, 2);
            expander5.Name = "expander5";
            expander5.Padding = new Padding(3, 2, 3, 2);
            expander5.Size = new Size(222, 119);
            expander5.TabIndex = 18;
            expander5.Text = "Информация";
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Location = new Point(3, 26);
            richTextBox1.Margin = new Padding(3, 2, 3, 2);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(216, 91);
            richTextBox1.TabIndex = 13;
            richTextBox1.Text = "";
            // 
            // expander4
            // 
            expander4.Controls.Add(checkedListBox1);
            expander4.Dock = DockStyle.Top;
            expander4.Expanded = false;
            expander4.ExpandedHeight = 159;
            expander4.Location = new Point(0, 397);
            expander4.Margin = new Padding(3, 2, 3, 2);
            expander4.Name = "expander4";
            expander4.Padding = new Padding(3, 2, 3, 2);
            expander4.Size = new Size(222, 22);
            expander4.TabIndex = 17;
            expander4.Text = "Созвездия";
            // 
            // checkedListBox1
            // 
            checkedListBox1.CheckOnClick = true;
            checkedListBox1.Dock = DockStyle.Fill;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(3, 20);
            checkedListBox1.Margin = new Padding(3, 2, 3, 2);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(218, 98);
            checkedListBox1.TabIndex = 0;
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            // 
            // expander3
            // 
            expander3.Controls.Add(splitContainer1);
            expander3.Controls.Add(radioButton3);
            expander3.Controls.Add(radioButton2);
            expander3.Controls.Add(radioButton1);
            expander3.Dock = DockStyle.Top;
            expander3.Expanded = false;
            expander3.ExpandedHeight = 190;
            expander3.Location = new Point(0, 375);
            expander3.Margin = new Padding(3, 2, 3, 2);
            expander3.Name = "expander3";
            expander3.Padding = new Padding(3, 2, 3, 2);
            expander3.Size = new Size(222, 22);
            expander3.TabIndex = 16;
            expander3.Text = "Подписи";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Top;
            splitContainer1.Location = new Point(3, 74);
            splitContainer1.Margin = new Padding(3, 2, 3, 2);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(comboBox2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(label5);
            splitContainer1.Panel2.Controls.Add(numericUpDown2);
            splitContainer1.Size = new Size(218, 82);
            splitContainer1.SplitterDistance = 25;
            splitContainer1.SplitterWidth = 3;
            splitContainer1.TabIndex = 16;
            // 
            // comboBox2
            // 
            comboBox2.Dock = DockStyle.Top;
            comboBox2.Enabled = false;
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(0, 0);
            comboBox2.Margin = new Padding(3, 2, 3, 2);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(218, 23);
            comboBox2.TabIndex = 15;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Top;
            label5.Location = new Point(0, 0);
            label5.Name = "label5";
            label5.Size = new Size(192, 15);
            label5.TabIndex = 11;
            label5.Text = "Дистанция видимости подписей):";
            // 
            // numericUpDown2
            // 
            numericUpDown2.Enabled = false;
            numericUpDown2.Location = new Point(3, 17);
            numericUpDown2.Margin = new Padding(3, 2, 3, 2);
            numericUpDown2.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(209, 23);
            numericUpDown2.TabIndex = 11;
            numericUpDown2.Value = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDown2.ValueChanged += numericUpDown2_ValueChanged;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Dock = DockStyle.Top;
            radioButton3.Location = new Point(3, 56);
            radioButton3.Margin = new Padding(3, 2, 3, 2);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(218, 18);
            radioButton3.TabIndex = 14;
            radioButton3.Text = "Звезды созвездий";
            radioButton3.UseVisualStyleBackColor = true;
            radioButton3.CheckedChanged += radioButton3_CheckedChanged;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Dock = DockStyle.Top;
            radioButton2.Location = new Point(3, 38);
            radioButton2.Margin = new Padding(3, 2, 3, 2);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(218, 18);
            radioButton2.TabIndex = 13;
            radioButton2.Text = "Звезды по дистанции";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Dock = DockStyle.Top;
            radioButton1.Location = new Point(3, 20);
            radioButton1.Margin = new Padding(3, 2, 3, 2);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(218, 18);
            radioButton1.TabIndex = 12;
            radioButton1.TabStop = true;
            radioButton1.Text = "Без подписей";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // expander2
            // 
            expander2.Controls.Add(checkBox1);
            expander2.Controls.Add(label1);
            expander2.Controls.Add(trackBar1);
            expander2.Controls.Add(checkBox2);
            expander2.Controls.Add(label2);
            expander2.Dock = DockStyle.Top;
            expander2.ExpandedHeight = 120;
            expander2.Location = new Point(0, 255);
            expander2.Margin = new Padding(3, 2, 3, 2);
            expander2.Name = "expander2";
            expander2.Padding = new Padding(3, 2, 3, 2);
            expander2.Size = new Size(222, 120);
            expander2.TabIndex = 16;
            expander2.Text = "Вращение";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(6, 76);
            checkBox1.Margin = new Padding(3, 2, 3, 2);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(111, 19);
            checkBox1.TabIndex = 3;
            checkBox1.Text = "Авто вращение";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(5, 27);
            label1.Name = "label1";
            label1.Size = new Size(132, 15);
            label1.TabIndex = 1;
            label1.Text = "Шаг маштабирования:";
            // 
            // trackBar1
            // 
            trackBar1.LargeChange = 10;
            trackBar1.Location = new Point(5, 44);
            trackBar1.Margin = new Padding(3, 2, 3, 2);
            trackBar1.Maximum = 1000;
            trackBar1.Minimum = 1;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(214, 45);
            trackBar1.TabIndex = 0;
            trackBar1.Value = 1;
            trackBar1.ValueChanged += trackBar1_ValueChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(5, 99);
            checkBox2.Margin = new Padding(3, 2, 3, 2);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(176, 19);
            checkBox2.TabIndex = 4;
            checkBox2.Text = "Вращение модели мышью";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(157, 27);
            label2.Name = "label2";
            label2.Size = new Size(13, 15);
            label2.TabIndex = 2;
            label2.Text = "1";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Location = new Point(82, 567);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(82, 22);
            button1.TabIndex = 6;
            button1.Text = "Центр";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // expander6
            // 
            expander6.Controls.Add(checkBox3);
            expander6.Dock = DockStyle.Top;
            expander6.ExpandedHeight = 46;
            expander6.Location = new Point(0, 0);
            expander6.Margin = new Padding(3, 2, 3, 2);
            expander6.Name = "expander6";
            expander6.Padding = new Padding(3, 2, 3, 2);
            expander6.Size = new Size(222, 46);
            expander6.TabIndex = 19;
            expander6.Text = "Общие";
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Checked = true;
            checkBox3.CheckState = CheckState.Checked;
            checkBox3.Dock = DockStyle.Top;
            checkBox3.Location = new Point(3, 26);
            checkBox3.Margin = new Padding(3, 2, 3, 2);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(216, 19);
            checkBox3.TabIndex = 5;
            checkBox3.Text = "Отображать оси";
            checkBox3.UseVisualStyleBackColor = true;
            checkBox3.CheckedChanged += checkBox3_CheckedChanged;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 232F));
            tableLayoutPanel1.Controls.Add(panel1, 1, 0);
            tableLayoutPanel1.Controls.Add(glControl1, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(835, 602);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // glControl1
            // 
            glControl1.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl1.APIVersion = new Version(3, 3, 0, 0);
            glControl1.Dock = DockStyle.Fill;
            glControl1.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glControl1.IsEventDriven = true;
            glControl1.Location = new Point(3, 2);
            glControl1.Margin = new Padding(3, 2, 3, 2);
            glControl1.Name = "glControl1";
            glControl1.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
            glControl1.SharedContext = null;
            glControl1.Size = new Size(597, 598);
            glControl1.TabIndex = 2;
            glControl1.Paint += glControl1_Paint;
            glControl1.MouseDown += glControl1_MouseDown;
            glControl1.MouseMove += glControl1_MouseMove;
            glControl1.MouseUp += glControl1_MouseUp;
            glControl1.MouseWheel += glControl1_MouseWheel;
            glControl1.Resize += glControl1_Resize;
            // 
            // timer1
            // 
            timer1.Interval = 10;
            // 
            // timer2
            // 
            timer2.Tick += timer2_Tick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2, toolStripMenuItem3 });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(269, 70);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(268, 22);
            toolStripMenuItem1.Text = "Использовать как центр координат";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.CheckOnClick = true;
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(268, 22);
            toolStripMenuItem2.Text = "Прикрепить выделение";
            toolStripMenuItem2.CheckedChanged += toolStripMenuItem2_CheckedChanged;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(268, 22);
            toolStripMenuItem3.Text = "Открепить выделение";
            toolStripMenuItem3.CheckedChanged += toolStripMenuItem3_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(835, 602);
            Controls.Add(tableLayoutPanel1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            expander1.ResumeLayout(false);
            expander1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            panel1.ResumeLayout(false);
            expander5.ResumeLayout(false);
            expander4.ResumeLayout(false);
            expander3.ResumeLayout(false);
            expander3.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            expander2.ResumeLayout(false);
            expander2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            expander6.ResumeLayout(false);
            expander6.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel1;
        private GLControl glControl1;
        private Label label2;
        private Label label1;
        private TrackBar trackBar1;
        private CheckBox checkBox1;
        private System.Windows.Forms.Timer timer1;
        private CheckBox checkBox2;
        private ComboBox comboBox1;
        private Button button1;
        private NumericUpDown numericUpDown1;
        private Label label4;
        private Label label3;
        private NumericUpDown numericUpDown2;
        private Label label5;
        private ComboBox comboBox2;
        private System.Windows.Forms.Timer timer2;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private RichTextBox richTextBox1;
        private Expander expander1;
        private Expander expander2;
        private Expander expander3;
        private Expander expander4;
        private CheckedListBox checkedListBox1;
        private RadioButton radioButton1;
        private RadioButton radioButton3;
        private RadioButton radioButton2;
        private Expander expander5;
        private Expander expander6;
        private CheckBox checkBox3;
        private SplitContainer splitContainer1;
        private ToolStripMenuItem toolStripMenuItem3;
        private CheckBox checkBox4;
        private NumericUpDown numericUpDown3;
        private Label label6;
    }
}
