namespace WMS_Client.UI
{
    partial class HolderFrm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label_Id = new System.Windows.Forms.Label();
            this.label_cnt = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.Controls.Add(this.label_Id, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label_cnt, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(100, 100);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.DoubleClick += new System.EventHandler(this.HolderFrm_DoubleClick);
            // 
            // label_Id
            // 
            this.label_Id.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Id.Font = new System.Drawing.Font("宋体", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Id.Location = new System.Drawing.Point(3, 0);
            this.label_Id.Name = "label_Id";
            this.tableLayoutPanel1.SetRowSpan(this.label_Id, 2);
            this.label_Id.Size = new System.Drawing.Size(94, 66);
            this.label_Id.TabIndex = 0;
            this.label_Id.Text = "101";
            this.label_Id.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_Id.DoubleClick += new System.EventHandler(this.HolderFrm_DoubleClick);
            // 
            // label_cnt
            // 
            this.label_cnt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_cnt.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_cnt.Location = new System.Drawing.Point(3, 66);
            this.label_cnt.Name = "label_cnt";
            this.label_cnt.Size = new System.Drawing.Size(94, 34);
            this.label_cnt.TabIndex = 1;
            this.label_cnt.Text = "0 / 0";
            this.label_cnt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_cnt.DoubleClick += new System.EventHandler(this.HolderFrm_DoubleClick);
            // 
            // HolderFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(217)))), ((int)(((byte)(247)))));
            this.ClientSize = new System.Drawing.Size(100, 100);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.EnableGlass = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HolderFrm";
            this.Text = "HolderFrm";
            this.DoubleClick += new System.EventHandler(this.HolderFrm_DoubleClick);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label_Id;
        private System.Windows.Forms.Label label_cnt;
    }
}