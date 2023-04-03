namespace Texttool
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
			label2 = new System.Windows.Forms.Label();
			comboBox1 = new System.Windows.Forms.ComboBox();
			textBox1 = new System.Windows.Forms.TextBox();
			textBox2 = new System.Windows.Forms.TextBox();
			button1 = new System.Windows.Forms.Button();
			cbFile = new System.Windows.Forms.ComboBox();
			label1 = new System.Windows.Forms.Label();
			button2 = new System.Windows.Forms.Button();
			btnSave = new System.Windows.Forms.Button();
			listBox1 = new System.Windows.Forms.ListBox();
			button4 = new System.Windows.Forms.Button();
			button5 = new System.Windows.Forms.Button();
			button6 = new System.Windows.Forms.Button();
			toolTip1 = new System.Windows.Forms.ToolTip(components);
			button3 = new System.Windows.Forms.Button();
			button7 = new System.Windows.Forms.Button();
			button8 = new System.Windows.Forms.Button();
			button9 = new System.Windows.Forms.Button();
			txtLines = new System.Windows.Forms.TextBox();
			button10 = new System.Windows.Forms.Button();
			SuspendLayout();
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(17, 14);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(57, 15);
			label2.TabIndex = 2;
			label2.Text = "Encoding";
			// 
			// comboBox1
			// 
			comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBox1.FormattingEnabled = true;
			comboBox1.Location = new System.Drawing.Point(80, 12);
			comboBox1.Name = "comboBox1";
			comboBox1.Size = new System.Drawing.Size(95, 23);
			comboBox1.TabIndex = 3;
			comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
			// 
			// textBox1
			// 
			textBox1.AllowDrop = true;
			textBox1.BackColor = System.Drawing.SystemColors.Window;
			textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			textBox1.Location = new System.Drawing.Point(36, 79);
			textBox1.Multiline = true;
			textBox1.Name = "textBox1";
			textBox1.ReadOnly = true;
			textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			textBox1.Size = new System.Drawing.Size(945, 510);
			textBox1.TabIndex = 4;
			textBox1.WordWrap = false;
			textBox1.TextChanged += textBox1_TextChanged;
			textBox1.DragDrop += textBox1_DragDrop;
			textBox1.DragEnter += Form1_DragEnter;
			// 
			// textBox2
			// 
			textBox2.AllowDrop = true;
			textBox2.Font = new System.Drawing.Font("Lucida Sans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			textBox2.Location = new System.Drawing.Point(422, 885);
			textBox2.Name = "textBox2";
			textBox2.Size = new System.Drawing.Size(481, 26);
			textBox2.TabIndex = 6;
			textBox2.WordWrap = false;
			textBox2.TextChanged += textBox2_TextChanged;
			textBox2.KeyDown += textBox2_KeyDown;
			// 
			// button1
			// 
			button1.Location = new System.Drawing.Point(909, 887);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(75, 23);
			button1.TabIndex = 7;
			button1.Text = "Find";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// cbFile
			// 
			cbFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbFile.FormattingEnabled = true;
			cbFile.Location = new System.Drawing.Point(673, 12);
			cbFile.Name = "cbFile";
			cbFile.Size = new System.Drawing.Size(311, 23);
			cbFile.TabIndex = 8;
			cbFile.SelectedIndexChanged += cbFile_SelectedIndexChanged;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(642, 15);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(25, 15);
			label1.TabIndex = 9;
			label1.Text = "File";
			// 
			// button2
			// 
			button2.Location = new System.Drawing.Point(384, 12);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(83, 23);
			button2.TabIndex = 10;
			button2.Text = "Reload";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// btnSave
			// 
			btnSave.BackColor = System.Drawing.SystemColors.Control;
			btnSave.Location = new System.Drawing.Point(17, 887);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(231, 23);
			btnSave.TabIndex = 11;
			btnSave.Text = "Save";
			btnSave.UseVisualStyleBackColor = false;
			btnSave.Click += button3_Click;
			// 
			// listBox1
			// 
			listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			listBox1.FormattingEnabled = true;
			listBox1.ItemHeight = 20;
			listBox1.Location = new System.Drawing.Point(17, 633);
			listBox1.Name = "listBox1";
			listBox1.Size = new System.Drawing.Size(969, 224);
			listBox1.TabIndex = 12;
			listBox1.DoubleClick += listBox1_DoubleClick;
			// 
			// button4
			// 
			button4.BackColor = System.Drawing.Color.FromArgb(255, 128, 128);
			button4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			button4.Location = new System.Drawing.Point(507, 12);
			button4.Name = "button4";
			button4.Size = new System.Drawing.Size(129, 23);
			button4.TabIndex = 13;
			button4.Text = "Export Orig";
			button4.UseVisualStyleBackColor = false;
			button4.Click += button4_Click;
			// 
			// button5
			// 
			button5.BackColor = System.Drawing.Color.FromArgb(128, 255, 128);
			button5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			button5.Location = new System.Drawing.Point(181, 11);
			button5.Name = "button5";
			button5.Size = new System.Drawing.Size(149, 23);
			button5.TabIndex = 14;
			button5.Text = "Import";
			button5.UseVisualStyleBackColor = false;
			button5.Click += button5_Click;
			// 
			// button6
			// 
			button6.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
			button6.Location = new System.Drawing.Point(181, 40);
			button6.Name = "button6";
			button6.Size = new System.Drawing.Size(149, 23);
			button6.TabIndex = 15;
			button6.Text = "Merge Trimmed";
			button6.UseVisualStyleBackColor = false;
			button6.Click += button6_Click;
			// 
			// button3
			// 
			button3.BackColor = System.Drawing.Color.FromArgb(255, 192, 192);
			button3.Location = new System.Drawing.Point(507, 41);
			button3.Name = "button3";
			button3.Size = new System.Drawing.Size(129, 23);
			button3.TabIndex = 16;
			button3.Text = "Export Trimmed";
			button3.UseVisualStyleBackColor = false;
			button3.Click += button3_Click_1;
			// 
			// button7
			// 
			button7.BackColor = System.Drawing.SystemColors.Control;
			button7.Location = new System.Drawing.Point(17, 595);
			button7.Name = "button7";
			button7.Size = new System.Drawing.Size(371, 32);
			button7.TabIndex = 17;
			button7.Text = "Copy Prompt";
			button7.UseVisualStyleBackColor = false;
			button7.Click += button7_Click;
			// 
			// button8
			// 
			button8.BackColor = System.Drawing.SystemColors.Control;
			button8.Location = new System.Drawing.Point(695, 595);
			button8.Name = "button8";
			button8.Size = new System.Drawing.Size(291, 32);
			button8.TabIndex = 18;
			button8.Text = "Copy Result";
			button8.UseVisualStyleBackColor = false;
			button8.Click += button8_Click;
			// 
			// button9
			// 
			button9.BackColor = System.Drawing.SystemColors.Control;
			button9.Location = new System.Drawing.Point(394, 595);
			button9.Name = "button9";
			button9.Size = new System.Drawing.Size(295, 32);
			button9.TabIndex = 19;
			button9.Text = "Copy Plain Lines";
			button9.UseVisualStyleBackColor = false;
			button9.Click += button9_Click;
			// 
			// txtLines
			// 
			txtLines.AllowDrop = true;
			txtLines.BackColor = System.Drawing.SystemColors.Window;
			txtLines.Enabled = false;
			txtLines.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			txtLines.Location = new System.Drawing.Point(12, 79);
			txtLines.Multiline = true;
			txtLines.Name = "txtLines";
			txtLines.ReadOnly = true;
			txtLines.Size = new System.Drawing.Size(18, 510);
			txtLines.TabIndex = 20;
			txtLines.WordWrap = false;
			// 
			// button10
			// 
			button10.BackColor = System.Drawing.Color.DodgerBlue;
			button10.Location = new System.Drawing.Point(384, 40);
			button10.Name = "button10";
			button10.Size = new System.Drawing.Size(83, 23);
			button10.TabIndex = 21;
			button10.Text = "Import All";
			button10.UseVisualStyleBackColor = false;
			button10.Click += button10_Click;
			// 
			// Form1
			// 
			AllowDrop = true;
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(998, 918);
			Controls.Add(button10);
			Controls.Add(txtLines);
			Controls.Add(button9);
			Controls.Add(button8);
			Controls.Add(button7);
			Controls.Add(button3);
			Controls.Add(button6);
			Controls.Add(button5);
			Controls.Add(button4);
			Controls.Add(listBox1);
			Controls.Add(btnSave);
			Controls.Add(button2);
			Controls.Add(label1);
			Controls.Add(cbFile);
			Controls.Add(button1);
			Controls.Add(textBox2);
			Controls.Add(textBox1);
			Controls.Add(comboBox1);
			Controls.Add(label2);
			Name = "Form1";
			Text = "Form1";
			Load += Form1_Load;
			DragDrop += textBox1_DragDrop;
			DragEnter += Form1_DragEnter;
			ResumeLayout(false);
			PerformLayout();
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
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Button button8;
		private System.Windows.Forms.Button button9;
		private System.Windows.Forms.TextBox txtLines;
		private System.Windows.Forms.Button button10;
	}
}
