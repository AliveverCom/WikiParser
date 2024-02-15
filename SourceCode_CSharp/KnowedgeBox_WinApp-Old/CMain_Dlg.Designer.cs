
namespace KnowedgeBox.WinApp
{
    partial class CMain_Dlg
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
            this.Btn_解析XML2DB = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Btn_解析XML2DB
            // 
            this.Btn_解析XML2DB.Location = new System.Drawing.Point(55, 60);
            this.Btn_解析XML2DB.Name = "Btn_解析XML2DB";
            this.Btn_解析XML2DB.Size = new System.Drawing.Size(187, 35);
            this.Btn_解析XML2DB.TabIndex = 0;
            this.Btn_解析XML2DB.Text = "解析XML->DB";
            this.Btn_解析XML2DB.UseVisualStyleBackColor = true;
            this.Btn_解析XML2DB.Click += new System.EventHandler(this.Btn_解析XML2DB_Click);
            // 
            // CMain_Dlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Btn_解析XML2DB);
            this.Name = "CMain_Dlg";
            this.Text = "Main_Dlg";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Btn_解析XML2DB;
    }
}

