namespace TkDotNetScrewAoi.view
{
    partial class FormImagGet
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.hWControl_Roi4 = new HalconDotNet.HWindowControl();
            this.hWControl_Roi3 = new HalconDotNet.HWindowControl();
            this.hWControl_Ball4 = new HalconDotNet.HWindowControl();
            this.hWControl_Ball3 = new HalconDotNet.HWindowControl();
            this.panel3 = new System.Windows.Forms.Panel();
            this.hWControl_Roi1 = new HalconDotNet.HWindowControl();
            this.hWControl_Ball1 = new HalconDotNet.HWindowControl();
            this.hWControl_Ball2 = new HalconDotNet.HWindowControl();
            this.hWControl_Roi2 = new HalconDotNet.HWindowControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radioButton_Software = new System.Windows.Forms.RadioButton();
            this.radioButton_External = new System.Windows.Forms.RadioButton();
            this.btn_MechaismTest = new System.Windows.Forms.Button();
            this.btn_CcdConnect = new System.Windows.Forms.Button();
            this.btn_ImageSave = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Location = new System.Drawing.Point(0, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1281, 533);
            this.panel1.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Controls.Add(this.hWControl_Roi4);
            this.panel4.Controls.Add(this.hWControl_Roi3);
            this.panel4.Controls.Add(this.hWControl_Ball4);
            this.panel4.Controls.Add(this.hWControl_Ball3);
            this.panel4.Location = new System.Drawing.Point(646, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(632, 527);
            this.panel4.TabIndex = 5;
            // 
            // hWControl_Roi4
            // 
            this.hWControl_Roi4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.hWControl_Roi4.BackColor = System.Drawing.Color.Black;
            this.hWControl_Roi4.BorderColor = System.Drawing.Color.Black;
            this.hWControl_Roi4.ImagePart = new System.Drawing.Rectangle(0, 0, 2448, 2048);
            this.hWControl_Roi4.Location = new System.Drawing.Point(323, 3);
            this.hWControl_Roi4.Name = "hWControl_Roi4";
            this.hWControl_Roi4.Size = new System.Drawing.Size(306, 256);
            this.hWControl_Roi4.TabIndex = 3;
            this.hWControl_Roi4.WindowSize = new System.Drawing.Size(306, 256);
            // 
            // hWControl_Roi3
            // 
            this.hWControl_Roi3.BackColor = System.Drawing.Color.Black;
            this.hWControl_Roi3.BorderColor = System.Drawing.Color.Black;
            this.hWControl_Roi3.ImagePart = new System.Drawing.Rectangle(0, 0, 2448, 2048);
            this.hWControl_Roi3.Location = new System.Drawing.Point(3, 3);
            this.hWControl_Roi3.Name = "hWControl_Roi3";
            this.hWControl_Roi3.Size = new System.Drawing.Size(306, 256);
            this.hWControl_Roi3.TabIndex = 2;
            this.hWControl_Roi3.WindowSize = new System.Drawing.Size(306, 256);
            // 
            // hWControl_Ball4
            // 
            this.hWControl_Ball4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.hWControl_Ball4.BackColor = System.Drawing.Color.Black;
            this.hWControl_Ball4.BorderColor = System.Drawing.Color.Black;
            this.hWControl_Ball4.ImagePart = new System.Drawing.Rectangle(0, 0, 2448, 2048);
            this.hWControl_Ball4.Location = new System.Drawing.Point(323, 268);
            this.hWControl_Ball4.Name = "hWControl_Ball4";
            this.hWControl_Ball4.Size = new System.Drawing.Size(306, 256);
            this.hWControl_Ball4.TabIndex = 7;
            this.hWControl_Ball4.WindowSize = new System.Drawing.Size(306, 256);
            // 
            // hWControl_Ball3
            // 
            this.hWControl_Ball3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.hWControl_Ball3.BackColor = System.Drawing.Color.Black;
            this.hWControl_Ball3.BorderColor = System.Drawing.Color.Black;
            this.hWControl_Ball3.ImagePart = new System.Drawing.Rectangle(0, 0, 2448, 2048);
            this.hWControl_Ball3.Location = new System.Drawing.Point(3, 268);
            this.hWControl_Ball3.Name = "hWControl_Ball3";
            this.hWControl_Ball3.Size = new System.Drawing.Size(306, 256);
            this.hWControl_Ball3.TabIndex = 6;
            this.hWControl_Ball3.WindowSize = new System.Drawing.Size(306, 256);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.Controls.Add(this.hWControl_Roi1);
            this.panel3.Controls.Add(this.hWControl_Ball1);
            this.panel3.Controls.Add(this.hWControl_Ball2);
            this.panel3.Controls.Add(this.hWControl_Roi2);
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(633, 527);
            this.panel3.TabIndex = 5;
            // 
            // hWControl_Roi1
            // 
            this.hWControl_Roi1.BackColor = System.Drawing.Color.Black;
            this.hWControl_Roi1.BorderColor = System.Drawing.Color.Black;
            this.hWControl_Roi1.ImagePart = new System.Drawing.Rectangle(0, 0, 2448, 2048);
            this.hWControl_Roi1.Location = new System.Drawing.Point(3, 3);
            this.hWControl_Roi1.Name = "hWControl_Roi1";
            this.hWControl_Roi1.Size = new System.Drawing.Size(306, 256);
            this.hWControl_Roi1.TabIndex = 0;
            this.hWControl_Roi1.WindowSize = new System.Drawing.Size(306, 256);
            // 
            // hWControl_Ball1
            // 
            this.hWControl_Ball1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.hWControl_Ball1.BackColor = System.Drawing.Color.Black;
            this.hWControl_Ball1.BorderColor = System.Drawing.Color.Black;
            this.hWControl_Ball1.ImagePart = new System.Drawing.Rectangle(0, 0, 2448, 2048);
            this.hWControl_Ball1.Location = new System.Drawing.Point(3, 268);
            this.hWControl_Ball1.Name = "hWControl_Ball1";
            this.hWControl_Ball1.Size = new System.Drawing.Size(306, 256);
            this.hWControl_Ball1.TabIndex = 4;
            this.hWControl_Ball1.WindowSize = new System.Drawing.Size(306, 256);
            // 
            // hWControl_Ball2
            // 
            this.hWControl_Ball2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.hWControl_Ball2.BackColor = System.Drawing.Color.Black;
            this.hWControl_Ball2.BorderColor = System.Drawing.Color.Black;
            this.hWControl_Ball2.ImagePart = new System.Drawing.Rectangle(0, 0, 2448, 2048);
            this.hWControl_Ball2.Location = new System.Drawing.Point(324, 268);
            this.hWControl_Ball2.Name = "hWControl_Ball2";
            this.hWControl_Ball2.Size = new System.Drawing.Size(306, 256);
            this.hWControl_Ball2.TabIndex = 5;
            this.hWControl_Ball2.WindowSize = new System.Drawing.Size(306, 256);
            // 
            // hWControl_Roi2
            // 
            this.hWControl_Roi2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.hWControl_Roi2.BackColor = System.Drawing.Color.Black;
            this.hWControl_Roi2.BorderColor = System.Drawing.Color.Black;
            this.hWControl_Roi2.ImagePart = new System.Drawing.Rectangle(0, 0, 2448, 2048);
            this.hWControl_Roi2.Location = new System.Drawing.Point(324, 3);
            this.hWControl_Roi2.Name = "hWControl_Roi2";
            this.hWControl_Roi2.Size = new System.Drawing.Size(306, 256);
            this.hWControl_Roi2.TabIndex = 1;
            this.hWControl_Roi2.WindowSize = new System.Drawing.Size(306, 256);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.radioButton_Software);
            this.panel2.Controls.Add(this.radioButton_External);
            this.panel2.Controls.Add(this.btn_MechaismTest);
            this.panel2.Controls.Add(this.btn_CcdConnect);
            this.panel2.Controls.Add(this.btn_ImageSave);
            this.panel2.Location = new System.Drawing.Point(12, 551);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1257, 153);
            this.panel2.TabIndex = 1;
            // 
            // radioButton_Software
            // 
            this.radioButton_Software.AutoSize = true;
            this.radioButton_Software.Checked = true;
            this.radioButton_Software.Location = new System.Drawing.Point(133, 8);
            this.radioButton_Software.Name = "radioButton_Software";
            this.radioButton_Software.Size = new System.Drawing.Size(104, 28);
            this.radioButton_Software.TabIndex = 4;
            this.radioButton_Software.TabStop = true;
            this.radioButton_Software.Text = "軟體觸發";
            this.radioButton_Software.UseVisualStyleBackColor = true;
            this.radioButton_Software.CheckedChanged += new System.EventHandler(this.radioButton_Software_CheckedChanged);
            // 
            // radioButton_External
            // 
            this.radioButton_External.AutoSize = true;
            this.radioButton_External.Location = new System.Drawing.Point(3, 8);
            this.radioButton_External.Name = "radioButton_External";
            this.radioButton_External.Size = new System.Drawing.Size(104, 28);
            this.radioButton_External.TabIndex = 3;
            this.radioButton_External.Text = "外部觸發";
            this.radioButton_External.UseVisualStyleBackColor = true;
            // 
            // btn_MechaismTest
            // 
            this.btn_MechaismTest.Location = new System.Drawing.Point(133, 81);
            this.btn_MechaismTest.Name = "btn_MechaismTest";
            this.btn_MechaismTest.Size = new System.Drawing.Size(104, 39);
            this.btn_MechaismTest.TabIndex = 2;
            this.btn_MechaismTest.Text = "機構測試";
            this.btn_MechaismTest.UseVisualStyleBackColor = true;
            this.btn_MechaismTest.Click += new System.EventHandler(this.btn_MechaismTest_Click);
            // 
            // btn_CcdConnect
            // 
            this.btn_CcdConnect.Location = new System.Drawing.Point(3, 39);
            this.btn_CcdConnect.Name = "btn_CcdConnect";
            this.btn_CcdConnect.Size = new System.Drawing.Size(234, 36);
            this.btn_CcdConnect.TabIndex = 1;
            this.btn_CcdConnect.Text = "相機連線";
            this.btn_CcdConnect.UseVisualStyleBackColor = true;
            this.btn_CcdConnect.Click += new System.EventHandler(this.btn_CcdConnect_Click);
            // 
            // btn_ImageSave
            // 
            this.btn_ImageSave.Location = new System.Drawing.Point(3, 81);
            this.btn_ImageSave.Name = "btn_ImageSave";
            this.btn_ImageSave.Size = new System.Drawing.Size(104, 39);
            this.btn_ImageSave.TabIndex = 0;
            this.btn_ImageSave.Text = "存圖模式";
            this.btn_ImageSave.UseVisualStyleBackColor = true;
            this.btn_ImageSave.Click += new System.EventHandler(this.btn_ImageSave_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 707);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1281, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // FormImagGet
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1281, 729);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "FormImagGet";
            this.Text = "FormImagGet";
            this.Load += new System.EventHandler(this.FormImagGet_Load);
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private HalconDotNet.HWindowControl hWControl_Roi4;
        private HalconDotNet.HWindowControl hWControl_Roi3;
        private HalconDotNet.HWindowControl hWControl_Roi2;
        private HalconDotNet.HWindowControl hWControl_Roi1;
        private HalconDotNet.HWindowControl hWControl_Ball4;
        private HalconDotNet.HWindowControl hWControl_Ball3;
        private HalconDotNet.HWindowControl hWControl_Ball2;
        private HalconDotNet.HWindowControl hWControl_Ball1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_CcdConnect;
        private System.Windows.Forms.Button btn_ImageSave;
        private System.Windows.Forms.Button btn_MechaismTest;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.RadioButton radioButton_Software;
        private System.Windows.Forms.RadioButton radioButton_External;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
    }
}