namespace TkDotNetScrewAoi.view
{
    partial class FormMechnicalTest
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
            this.btn_ServoPicth = new System.Windows.Forms.Button();
            this.btn_ServoPicthR = new System.Windows.Forms.Button();
            this.btn_ModeIdle = new System.Windows.Forms.Button();
            this.btn_ModeHandle = new System.Windows.Forms.Button();
            this.btn_LightOnOff = new System.Windows.Forms.Button();
            this.btn_SortHandle = new System.Windows.Forms.Button();
            this.btn_CcdTrigger = new System.Windows.Forms.Button();
            this.btn_CcdSwitch = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.btn_CcdSwitch);
            this.panel1.Controls.Add(this.btn_CcdTrigger);
            this.panel1.Controls.Add(this.btn_SortHandle);
            this.panel1.Controls.Add(this.btn_LightOnOff);
            this.panel1.Controls.Add(this.btn_ServoPicth);
            this.panel1.Controls.Add(this.btn_ServoPicthR);
            this.panel1.Controls.Add(this.btn_ModeIdle);
            this.panel1.Controls.Add(this.btn_ModeHandle);
            this.panel1.Location = new System.Drawing.Point(17, 17);
            this.panel1.Margin = new System.Windows.Forms.Padding(8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(190, 593);
            this.panel1.TabIndex = 0;
            // 
            // btn_ServoPicth
            // 
            this.btn_ServoPicth.Location = new System.Drawing.Point(3, 141);
            this.btn_ServoPicth.Name = "btn_ServoPicth";
            this.btn_ServoPicth.Size = new System.Drawing.Size(179, 63);
            this.btn_ServoPicth.TabIndex = 3;
            this.btn_ServoPicth.Text = "螺桿步進";
            this.btn_ServoPicth.UseVisualStyleBackColor = true;
            this.btn_ServoPicth.Click += new System.EventHandler(this.btn_ServoPicth_Click);
            // 
            // btn_ServoPicthR
            // 
            this.btn_ServoPicthR.Location = new System.Drawing.Point(3, 210);
            this.btn_ServoPicthR.Name = "btn_ServoPicthR";
            this.btn_ServoPicthR.Size = new System.Drawing.Size(179, 63);
            this.btn_ServoPicthR.TabIndex = 2;
            this.btn_ServoPicthR.Text = "螺桿步進R";
            this.btn_ServoPicthR.UseVisualStyleBackColor = true;
            this.btn_ServoPicthR.Click += new System.EventHandler(this.btn_ServoPicthR_Click);
            // 
            // btn_ModeIdle
            // 
            this.btn_ModeIdle.Location = new System.Drawing.Point(3, 72);
            this.btn_ModeIdle.Name = "btn_ModeIdle";
            this.btn_ModeIdle.Size = new System.Drawing.Size(179, 63);
            this.btn_ModeIdle.TabIndex = 1;
            this.btn_ModeIdle.Text = "待機模式";
            this.btn_ModeIdle.UseVisualStyleBackColor = true;
            this.btn_ModeIdle.Click += new System.EventHandler(this.btn_ModeIdle_Click);
            // 
            // btn_ModeHandle
            // 
            this.btn_ModeHandle.Location = new System.Drawing.Point(3, 3);
            this.btn_ModeHandle.Name = "btn_ModeHandle";
            this.btn_ModeHandle.Size = new System.Drawing.Size(179, 63);
            this.btn_ModeHandle.TabIndex = 0;
            this.btn_ModeHandle.Text = "手動模式";
            this.btn_ModeHandle.UseVisualStyleBackColor = true;
            this.btn_ModeHandle.Click += new System.EventHandler(this.btn_modeChange_Click);
            // 
            // btn_LightOnOff
            // 
            this.btn_LightOnOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_LightOnOff.Location = new System.Drawing.Point(3, 317);
            this.btn_LightOnOff.Name = "btn_LightOnOff";
            this.btn_LightOnOff.Size = new System.Drawing.Size(179, 63);
            this.btn_LightOnOff.TabIndex = 4;
            this.btn_LightOnOff.Text = "光源開";
            this.btn_LightOnOff.UseVisualStyleBackColor = true;
            this.btn_LightOnOff.Click += new System.EventHandler(this.btn_LightOnOff_Click);
            // 
            // btn_SortHandle
            // 
            this.btn_SortHandle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_SortHandle.Location = new System.Drawing.Point(3, 386);
            this.btn_SortHandle.Name = "btn_SortHandle";
            this.btn_SortHandle.Size = new System.Drawing.Size(179, 63);
            this.btn_SortHandle.TabIndex = 5;
            this.btn_SortHandle.Text = "手動分料";
            this.btn_SortHandle.UseVisualStyleBackColor = true;
            this.btn_SortHandle.Click += new System.EventHandler(this.btn_SortHandle_Click);
            // 
            // btn_CcdTrigger
            // 
            this.btn_CcdTrigger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_CcdTrigger.Location = new System.Drawing.Point(3, 524);
            this.btn_CcdTrigger.Name = "btn_CcdTrigger";
            this.btn_CcdTrigger.Size = new System.Drawing.Size(179, 63);
            this.btn_CcdTrigger.TabIndex = 6;
            this.btn_CcdTrigger.Text = "相機觸發";
            this.btn_CcdTrigger.UseVisualStyleBackColor = true;
            this.btn_CcdTrigger.Click += new System.EventHandler(this.btn_CcdTrigger_Click);
            // 
            // btn_CcdSwitch
            // 
            this.btn_CcdSwitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_CcdSwitch.Location = new System.Drawing.Point(3, 455);
            this.btn_CcdSwitch.Name = "btn_CcdSwitch";
            this.btn_CcdSwitch.Size = new System.Drawing.Size(179, 63);
            this.btn_CcdSwitch.TabIndex = 7;
            this.btn_CcdSwitch.Text = "相機開關";
            this.btn_CcdSwitch.UseVisualStyleBackColor = true;
            this.btn_CcdSwitch.Click += new System.EventHandler(this.btn_CcdSwitch_Click);
            // 
            // FormMechnicalTest
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(214, 616);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微軟正黑體", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "FormMechnicalTest";
            this.Text = "FormMechnicalTest";
            this.Load += new System.EventHandler(this.FormMechnicalTest_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_ModeHandle;
        private System.Windows.Forms.Button btn_ServoPicth;
        private System.Windows.Forms.Button btn_ServoPicthR;
        private System.Windows.Forms.Button btn_ModeIdle;
        private System.Windows.Forms.Button btn_LightOnOff;
        private System.Windows.Forms.Button btn_SortHandle;
        private System.Windows.Forms.Button btn_CcdTrigger;
        private System.Windows.Forms.Button btn_CcdSwitch;
    }
}