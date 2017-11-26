namespace z80.ide
{
    partial class Screen
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
            this.cmdFillDemo = new System.Windows.Forms.Button();
            this.cmdClearDemo = new System.Windows.Forms.Button();
            this.cmdLoadGloria = new System.Windows.Forms.Button();
            this.cmdLoadColour = new System.Windows.Forms.Button();
            this.cmdLoadManicMinor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(3, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1024, 768);
            this.panel1.TabIndex = 0;
            // 
            // cmdFillDemo
            // 
            this.cmdFillDemo.Location = new System.Drawing.Point(1033, 12);
            this.cmdFillDemo.Name = "cmdFillDemo";
            this.cmdFillDemo.Size = new System.Drawing.Size(166, 39);
            this.cmdFillDemo.TabIndex = 1;
            this.cmdFillDemo.Text = "Fill Demo";
            this.cmdFillDemo.UseVisualStyleBackColor = true;
            this.cmdFillDemo.Click += new System.EventHandler(this.cmdFillDemo_Click);
            // 
            // cmdClearDemo
            // 
            this.cmdClearDemo.Location = new System.Drawing.Point(1034, 57);
            this.cmdClearDemo.Name = "cmdClearDemo";
            this.cmdClearDemo.Size = new System.Drawing.Size(166, 39);
            this.cmdClearDemo.TabIndex = 2;
            this.cmdClearDemo.Text = "Clear Demo";
            this.cmdClearDemo.UseVisualStyleBackColor = true;
            this.cmdClearDemo.Click += new System.EventHandler(this.cmdClearDemo_Click);
            // 
            // cmdLoadGloria
            // 
            this.cmdLoadGloria.Location = new System.Drawing.Point(1033, 102);
            this.cmdLoadGloria.Name = "cmdLoadGloria";
            this.cmdLoadGloria.Size = new System.Drawing.Size(166, 39);
            this.cmdLoadGloria.TabIndex = 3;
            this.cmdLoadGloria.Text = "Load Gloria";
            this.cmdLoadGloria.UseVisualStyleBackColor = true;
            this.cmdLoadGloria.Click += new System.EventHandler(this.cmdLoadGloria_Click);
            // 
            // cmdLoadColour
            // 
            this.cmdLoadColour.Location = new System.Drawing.Point(1034, 147);
            this.cmdLoadColour.Name = "cmdLoadColour";
            this.cmdLoadColour.Size = new System.Drawing.Size(166, 39);
            this.cmdLoadColour.TabIndex = 4;
            this.cmdLoadColour.Text = "Load Colour";
            this.cmdLoadColour.UseVisualStyleBackColor = true;
            this.cmdLoadColour.Click += new System.EventHandler(this.cmdLoadColour_Click);
            // 
            // cmdLoadManicMinor
            // 
            this.cmdLoadManicMinor.Location = new System.Drawing.Point(1034, 192);
            this.cmdLoadManicMinor.Name = "cmdLoadManicMinor";
            this.cmdLoadManicMinor.Size = new System.Drawing.Size(166, 39);
            this.cmdLoadManicMinor.TabIndex = 5;
            this.cmdLoadManicMinor.Text = "Load Manic Miner";
            this.cmdLoadManicMinor.UseVisualStyleBackColor = true;
            this.cmdLoadManicMinor.Click += new System.EventHandler(this.cmdLoadManicMinor_Click);
            // 
            // Screen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1212, 796);
            this.Controls.Add(this.cmdLoadManicMinor);
            this.Controls.Add(this.cmdLoadColour);
            this.Controls.Add(this.cmdLoadGloria);
            this.Controls.Add(this.cmdClearDemo);
            this.Controls.Add(this.cmdFillDemo);
            this.Controls.Add(this.panel1);
            this.Name = "Screen";
            this.Text = "Screen";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdFillDemo;
        private System.Windows.Forms.Button cmdClearDemo;
        private System.Windows.Forms.Button cmdLoadGloria;
        private System.Windows.Forms.Button cmdLoadColour;
        private System.Windows.Forms.Button cmdLoadManicMinor;
    }
}