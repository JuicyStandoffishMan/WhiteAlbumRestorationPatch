﻿namespace Texttool
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
			this.label2 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.cbFile = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(17, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "Encoding";
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(80, 12);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(209, 23);
			this.comboBox1.TabIndex = 3;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// textBox1
			// 
			this.textBox1.AllowDrop = true;
			this.textBox1.BackColor = System.Drawing.SystemColors.Window;
			this.textBox1.Font = new System.Drawing.Font("HGGothicM", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.textBox1.Location = new System.Drawing.Point(17, 45);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(969, 582);
			this.textBox1.TabIndex = 4;
			this.textBox1.WordWrap = false;
			this.textBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox1_DragDrop);
			this.textBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
			// 
			// textBox2
			// 
			this.textBox2.AllowDrop = true;
			this.textBox2.Font = new System.Drawing.Font("Lucida Sans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.textBox2.Location = new System.Drawing.Point(422, 885);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(481, 26);
			this.textBox2.TabIndex = 6;
			this.textBox2.WordWrap = false;
			this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(909, 887);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 7;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// cbFile
			// 
			this.cbFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbFile.FormattingEnabled = true;
			this.cbFile.Location = new System.Drawing.Point(673, 12);
			this.cbFile.Name = "cbFile";
			this.cbFile.Size = new System.Drawing.Size(311, 23);
			this.cbFile.TabIndex = 8;
			this.cbFile.SelectedIndexChanged += new System.EventHandler(this.cbFile_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(642, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(25, 15);
			this.label1.TabIndex = 9;
			this.label1.Text = "File";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(541, 12);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(83, 23);
			this.button2.TabIndex = 10;
			this.button2.Text = "Reload";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(17, 887);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(139, 23);
			this.button3.TabIndex = 11;
			this.button3.Text = "Save";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// listBox1
			// 
			this.listBox1.Font = new System.Drawing.Font("HGGothicM", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 16;
			this.listBox1.Location = new System.Drawing.Point(17, 633);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(969, 228);
			this.listBox1.TabIndex = 12;
			this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(295, 10);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(83, 23);
			this.button4.TabIndex = 13;
			this.button4.Text = "Export";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(384, 10);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(83, 23);
			this.button5.TabIndex = 14;
			this.button5.Text = "Import";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// Form1
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(998, 918);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cbFile);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.label2);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox1_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ComboBox cbFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
	}
}
