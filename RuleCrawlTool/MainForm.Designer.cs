namespace RuleCrawlTool
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.labInfo1 = new System.Windows.Forms.Label();
            this.txtRule = new System.Windows.Forms.RichTextBox();
            this.labInfo2 = new System.Windows.Forms.Label();
            this.btnStartJob = new System.Windows.Forms.Button();
            this.btnOutputResult = new System.Windows.Forms.Button();
            this.dgvResult = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).BeginInit();
            this.SuspendLayout();
            // 
            // labInfo1
            // 
            this.labInfo1.AutoSize = true;
            this.labInfo1.Location = new System.Drawing.Point(13, 14);
            this.labInfo1.Name = "labInfo1";
            this.labInfo1.Size = new System.Drawing.Size(61, 13);
            this.labInfo1.TabIndex = 0;
            this.labInfo1.Text = "Crawl Rule:";
            // 
            // txtRule
            // 
            this.txtRule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRule.BackColor = System.Drawing.Color.Silver;
            this.txtRule.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRule.Location = new System.Drawing.Point(16, 39);
            this.txtRule.Name = "txtRule";
            this.txtRule.Size = new System.Drawing.Size(676, 237);
            this.txtRule.TabIndex = 1;
            this.txtRule.Text = "";
            // 
            // labInfo2
            // 
            this.labInfo2.AutoSize = true;
            this.labInfo2.Location = new System.Drawing.Point(13, 287);
            this.labInfo2.Name = "labInfo2";
            this.labInfo2.Size = new System.Drawing.Size(40, 13);
            this.labInfo2.TabIndex = 2;
            this.labInfo2.Text = "Result:";
            // 
            // btnStartJob
            // 
            this.btnStartJob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartJob.Location = new System.Drawing.Point(463, 282);
            this.btnStartJob.Name = "btnStartJob";
            this.btnStartJob.Size = new System.Drawing.Size(93, 23);
            this.btnStartJob.TabIndex = 3;
            this.btnStartJob.Text = "Start Job";
            this.btnStartJob.UseVisualStyleBackColor = true;
            this.btnStartJob.Click += new System.EventHandler(this.btnStartJob_Click);
            // 
            // btnOutputResult
            // 
            this.btnOutputResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOutputResult.Location = new System.Drawing.Point(596, 282);
            this.btnOutputResult.Name = "btnOutputResult";
            this.btnOutputResult.Size = new System.Drawing.Size(93, 23);
            this.btnOutputResult.TabIndex = 4;
            this.btnOutputResult.Text = "Output Result";
            this.btnOutputResult.UseVisualStyleBackColor = true;
            this.btnOutputResult.Click += new System.EventHandler(this.btnOutputResult_Click);
            // 
            // dgvResult
            // 
            this.dgvResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResult.Location = new System.Drawing.Point(16, 311);
            this.dgvResult.Name = "dgvResult";
            this.dgvResult.Size = new System.Drawing.Size(676, 232);
            this.dgvResult.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 555);
            this.Controls.Add(this.dgvResult);
            this.Controls.Add(this.btnOutputResult);
            this.Controls.Add(this.btnStartJob);
            this.Controls.Add(this.labInfo2);
            this.Controls.Add(this.txtRule);
            this.Controls.Add(this.labInfo1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rule Crawl Tool";
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labInfo1;
        private System.Windows.Forms.RichTextBox txtRule;
        private System.Windows.Forms.Label labInfo2;
        private System.Windows.Forms.Button btnStartJob;
        private System.Windows.Forms.Button btnOutputResult;
        private System.Windows.Forms.DataGridView dgvResult;
    }
}

