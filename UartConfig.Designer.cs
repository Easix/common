namespace Common
{
    partial class UartConfig
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
            if(disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UartConfig));
            this.bnt_OK = new System.Windows.Forms.Button();
            this.cobBox_Name = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cobBox_DataBit = new System.Windows.Forms.ComboBox();
            this.cobBox_StopBit = new System.Windows.Forms.ComboBox();
            this.cobBox_ParityBit = new System.Windows.Forms.ComboBox();
            this.cobBox_BaudRate = new System.Windows.Forms.ComboBox();
            this.btn_Open = new System.Windows.Forms.Button();
            this.label_Msg = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnt_OK
            // 
            resources.ApplyResources(this.bnt_OK, "bnt_OK");
            this.bnt_OK.Name = "bnt_OK";
            this.bnt_OK.UseVisualStyleBackColor = true;
            this.bnt_OK.Click += new System.EventHandler(this.bnt_OK_Click);
            // 
            // cobBox_Name
            // 
            this.cobBox_Name.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobBox_Name.FormattingEnabled = true;
            resources.ApplyResources(this.cobBox_Name, "cobBox_Name");
            this.cobBox_Name.Name = "cobBox_Name";
            this.cobBox_Name.DropDown += new System.EventHandler(this.cobBox_Name_DropDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cobBox_DataBit);
            this.groupBox1.Controls.Add(this.cobBox_StopBit);
            this.groupBox1.Controls.Add(this.cobBox_ParityBit);
            this.groupBox1.Controls.Add(this.cobBox_BaudRate);
            this.groupBox1.Controls.Add(this.cobBox_Name);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cobBox_DataBit
            // 
            this.cobBox_DataBit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobBox_DataBit.FormattingEnabled = true;
            this.cobBox_DataBit.Items.AddRange(new object[] {
            resources.GetString("cobBox_DataBit.Items"),
            resources.GetString("cobBox_DataBit.Items1"),
            resources.GetString("cobBox_DataBit.Items2"),
            resources.GetString("cobBox_DataBit.Items3")});
            resources.ApplyResources(this.cobBox_DataBit, "cobBox_DataBit");
            this.cobBox_DataBit.Name = "cobBox_DataBit";
            // 
            // cobBox_StopBit
            // 
            this.cobBox_StopBit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobBox_StopBit.FormattingEnabled = true;
            this.cobBox_StopBit.Items.AddRange(new object[] {
            resources.GetString("cobBox_StopBit.Items"),
            resources.GetString("cobBox_StopBit.Items1"),
            resources.GetString("cobBox_StopBit.Items2")});
            resources.ApplyResources(this.cobBox_StopBit, "cobBox_StopBit");
            this.cobBox_StopBit.Name = "cobBox_StopBit";
            // 
            // cobBox_ParityBit
            // 
            this.cobBox_ParityBit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobBox_ParityBit.FormattingEnabled = true;
            this.cobBox_ParityBit.Items.AddRange(new object[] {
            resources.GetString("cobBox_ParityBit.Items"),
            resources.GetString("cobBox_ParityBit.Items1"),
            resources.GetString("cobBox_ParityBit.Items2"),
            resources.GetString("cobBox_ParityBit.Items3"),
            resources.GetString("cobBox_ParityBit.Items4")});
            resources.ApplyResources(this.cobBox_ParityBit, "cobBox_ParityBit");
            this.cobBox_ParityBit.Name = "cobBox_ParityBit";
            // 
            // cobBox_BaudRate
            // 
            this.cobBox_BaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobBox_BaudRate.FormattingEnabled = true;
            this.cobBox_BaudRate.Items.AddRange(new object[] {
            resources.GetString("cobBox_BaudRate.Items"),
            resources.GetString("cobBox_BaudRate.Items1"),
            resources.GetString("cobBox_BaudRate.Items2"),
            resources.GetString("cobBox_BaudRate.Items3"),
            resources.GetString("cobBox_BaudRate.Items4"),
            resources.GetString("cobBox_BaudRate.Items5"),
            resources.GetString("cobBox_BaudRate.Items6"),
            resources.GetString("cobBox_BaudRate.Items7"),
            resources.GetString("cobBox_BaudRate.Items8"),
            resources.GetString("cobBox_BaudRate.Items9"),
            resources.GetString("cobBox_BaudRate.Items10"),
            resources.GetString("cobBox_BaudRate.Items11"),
            resources.GetString("cobBox_BaudRate.Items12"),
            resources.GetString("cobBox_BaudRate.Items13")});
            resources.ApplyResources(this.cobBox_BaudRate, "cobBox_BaudRate");
            this.cobBox_BaudRate.Name = "cobBox_BaudRate";
            // 
            // btn_Open
            // 
            resources.ApplyResources(this.btn_Open, "btn_Open");
            this.btn_Open.Name = "btn_Open";
            this.btn_Open.UseVisualStyleBackColor = true;
            this.btn_Open.Click += new System.EventHandler(this.btn_Open_Click);
            // 
            // label_Msg
            // 
            resources.ApplyResources(this.label_Msg, "label_Msg");
            this.label_Msg.ForeColor = System.Drawing.Color.Black;
            this.label_Msg.Name = "label_Msg";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bnt_OK);
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.btn_Open);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // UartConfig
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label_Msg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UartConfig";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnt_OK;
        private System.Windows.Forms.ComboBox cobBox_Name;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cobBox_StopBit;
        private System.Windows.Forms.ComboBox cobBox_ParityBit;
        private System.Windows.Forms.ComboBox cobBox_BaudRate;
        private System.Windows.Forms.Button btn_Open;
        private System.Windows.Forms.Label label_Msg;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cobBox_DataBit;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}