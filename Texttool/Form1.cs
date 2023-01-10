using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DALLib.File;
using DALLib.IO;
using Scarlet.IO.ImageFormats;
using SharpCompress.Common;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using LargeXlsx;
using ExcelDataReader;

namespace Texttool
{
	public partial class Form1 : Form
	{
		private byte[] text_data = new byte[0];
		private PCKFile pck_file = null;
		private System.Windows.Forms.TextBox edit_box = new System.Windows.Forms.TextBox();
		private int editing = -1;
		private Script active_script = null;
		private List<int> block_map = new List<int>();

		public int CharsPerLine = 0x3D;

		public Form1()
		{
			InitializeComponent();

		}

		private void textBox1_DragDrop(object sender, DragEventArgs e)
		{
			populate(((string[])e.Data.GetData(DataFormats.FileDrop, false))[0]);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

			comboBox1.Items.Add("Shift-JIS");
			comboBox1.SelectedIndex = 0;

			edit_box.Size = textBox2.Size;
			edit_box.Font = textBox2.Font;
			this.Controls.Add(edit_box);
			edit_box.KeyDown += Edit_box_KeyDown;
			edit_box.LostFocus += Edit_box_LostFocus;
			edit_box.Visible = false;

			textBox2.Text = Texttool.Properties.Settings.Default.last_search;

			button4.Tag = "Exports the selected script.\n\nThis will create a source .bin file, or re-load it if it already exists.";
			button5.Tag = "Imports the selected script by loading the corresponding full excel spreadsheet and writes to the Scripts.sdat buffer.";
			button6.Tag = "Merges the selected script with the corresponding trimmed excel spreadsheet.";
			button3.Tag = "Trims the Japanese text from the selected script and writes it to the trimmed folder.";

			button4.MouseHover += Button_Showtooltip;
			button5.MouseHover += Button_Showtooltip;
			button6.MouseHover += Button_Showtooltip;
			button3.MouseHover += Button_Showtooltip;

			populate(Texttool.Properties.Settings.Default.last_dir);
		}
		private void Button_Showtooltip(object sender, EventArgs e)
		{
			var button = sender as Control;
			if (button == null || button.Tag == null || string.IsNullOrEmpty(button.Tag.ToString()))
				return;

			toolTip1.SetToolTip(button, (string)button.Tag.ToString());
		}

		private void Edit_box_LostFocus(object? sender, EventArgs e)
		{
			edit_box.Visible = false;
			editing = -1;
		}

		private void Edit_box_KeyDown(object? sender, KeyEventArgs e)
		{
			if (editing == -1 || !edit_box.Visible)
				return;

			if (e.KeyCode == Keys.Escape)
			{
				edit_box.Visible = false;
				e.Handled = true;
				editing = -1;
			}
			else if (e.KeyCode == Keys.Enter)
			{
				string s = edit_box.Text;
				s = filter_string(s);
				listBox1.Items[editing] = s;
				((LineBlock)active_script.Blocks[block_map[editing]]).Text = s;
				edit_box.Visible = false;
				editing = -1;
			}
		}

		private void populate(string path)
		{
			if (path == null || path == "")
				return;

			if (path.EndsWith(".tex"))
			{
				TEXFile tex = new TEXFile { UseBigEndian = true };
				tex.Load(path, true);
				foreach (var f in tex.Frames)
				{

				}
			}
			else if (path.EndsWith(".fnt"))
			{
				read_fnt(path);
				return;
			}
			else if (path.EndsWith(".png"))
			{
				read_png(path);
				return;

			}
			else if (path.EndsWith("WHITE ALBUM.exe"))
			{
				patch_exe(path);
				return;
			}
			bool save = false;
			if (Texttool.Properties.Settings.Default.last_dir != path)
			{
				Texttool.Properties.Settings.Default.last_dir = path;
				save = true;
			}

			cbFile.Items.Clear();
			try
			{
				cbFile.Enabled = false;

				pck_file = new PCKFile { UseBigEndian = true };
				pck_file.Load(path, false);
				foreach (var v in pck_file.FileEntries)
				{
					cbFile.Items.Add(v.FileName);// + " (" + v.DataLength + " bytes)");
				}
				//pck_file.ExtractAllFiles(Path.GetFileNameWithoutExtension(filePath));
				cbFile.Enabled = true;

				int index = 0;
				cbFile.SelectedIndex = Texttool.Properties.Settings.Default.last_index;

				button1.Text = "Find";

				this.Text = Path.GetFileName(path) + " - White Album script viewer";
			}
			catch
			{

			}

			if (save)
			{
				Texttool.Properties.Settings.Default.Save();
			}
		}

		private string filter_string(string s)
		{
			StringBuilder sb = new StringBuilder();
			int line_count = 0;
			int char_count = 0;
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];

				if (c >= '0' && c <= '9')
				{
					c = (char)('０' + (c - '0'));
				}
				else if (c >= 'A' && c <= 'Z')
				{
					c = (char)('Ａ' + (c - 'A'));
				}
				else if (c >= 'a' && c <= 'z')
				{
					c = (char)('ａ' + (c - 'a'));
				}
				else if (c == '.')
				{
					c = '．';
				}
				else if (c == ':')
				{
					c = '：';
				}
				else if (c == ';')
				{
					c = '；';
				}
				else if (c == '\'')
				{
					c = '′';
				}
				else if (c == '\"')
				{
					c = '″';
				}
				else if (c == '!')
				{
					c = '！';
				}
				else if (c == '?')
				{
					c = '？';
				}
				else if (c == ',')
				{
					c = '，';
				}
				else if (c == ' ' || c == '　')
				{
					int space = s.IndexOf(' ', i + 1);
					int dash = s.IndexOf('-', i + 1);
					int ind = (space != -1 ? space : dash);
					if (ind == -1)
						ind = s.Length - (i + 1);
					else
						ind -= i + 1;
					if (ind + line_count > CharsPerLine - 3)
					{
						if (char_count >= 150)
						{
							sb.Append("<ｋａｅｒｂ>");
							char_count = 0;
						}
						else
						{
							sb.Append("<ｂｒｅａｋ>");
						}
						line_count = 0;
						continue;
					}
					c = '　';
				}
				else if (c == '-')
				{
					c = 'ー';
				}

				sb.Append(c);
				line_count++;
				char_count++;
			}
			sb = sb.Replace("...", "…");
			sb = sb.Replace("．．．", "…");

			return sb.ToString();
		}

		private void patch_exe(string path)
		{
			byte[] bytes = File.ReadAllBytes(path);
			bytes[0x17DC1] = (byte)CharsPerLine; // Visual chars per line
			bytes[0x17B9C] = (byte)CharsPerLine; // Actual chars per line
			bytes[0x176C6] = 0x0E; // X spacing
			bytes[0x176D7] = 0x24; // Y spacing

			try
			{
				BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
				bw.Write(bytes);
				bw.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to patch exe " + path + "\n: " + ex.Message);
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			update_text();
		}

		private void read_fnt(string fname)
		{
			PCKFile pp = new PCKFile();
			pp.AddFile(Path.GetFileNameWithoutExtension(fname) + ".fnt", File.ReadAllBytes(fname));
			pp.Save(Path.GetDirectoryName(fname) + "\\" + Path.GetFileNameWithoutExtension(fname) + ".pck");
		}

		private void read_png(string fname)
		{
			Bitmap bmp = new Bitmap(fname);
			Font f = new Font("HGPGothicM", 28.0f);
			Font f2 = new Font("HGPGothicM", 28.0f, FontStyle.Bold);
			Graphics g = Graphics.FromImage(bmp);
			SolidBrush brush1 = new SolidBrush(Color.FromArgb(44, 56, 128, 61));
			LinearGradientBrush brush2 = new LinearGradientBrush(new PointF(0, 0.0f), new PointF(0, 1.0f), Color.White, Color.FromArgb(66, 72, 238));
			SolidBrush brush3 = new SolidBrush(Color.FromArgb(44, 56, 128, 238));
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

			for (int i = 32; i <= 126; i++)
			{
				int x = ((i - 32) % 56) * 36;
				int y = ((i - 32) / 56) * 36 + 1404;
				PointF point = new PointF((float)x, (float)y);
				//g.DrawString(((char)i).ToString(), f2, brush1, new PointF((float)(x - 1), (float)(y - 1)));
				//g.DrawString(((char)i).ToString(), f2, brush3, new PointF((float)(x + 1), (float)(y + 1)));
				//g.DrawString(((char)i).ToString(), f, brush2, point);
			}

			//bmp.Save(fname + " - copy.png");

			{
				PCKFile fnt_file = new PCKFile { UseBigEndian = true };
				fnt_file.Load("C:\\Program Files\\WA1\\Font.pck");
				ExtendedBinaryReader reader = new ExtendedBinaryReader(fnt_file.GetFileStream("MAINTEXT.fnt"), true);
				ExtendedBinaryWriter writer = new ExtendedBinaryWriter(new MemoryStream(64 * 1024 * 1024), true);

				reader.JumpAhead(8);
				int tex_off = reader.ReadInt32();
				int char_height = reader.ReadInt32();
				int orig_char_count = reader.ReadInt32();

				writer.WriteDALSignature("Table", true);
				writer.AddOffset("size");
				writer.Write(27);
				writer.AddOffset("char_count");

				List<int> excludes = new List<int>();
				for (int i = 32; i <= 127; i++)
				{
					char c = (char)i;
					if (c >= '0' && c <= '9')
					{
						c = (char)('０' + (c - '0'));
					}
					else if (c >= 'A' && c <= 'Z')
					{
						c = (char)('Ａ' + (c - 'A'));
					}
					else if (c >= 'a' && c <= 'z')
					{
						c = (char)('ａ' + (c - 'a'));
					}
					else if (c == '.')
					{
						c = '．';
					}
					else if (c == ':')
					{
						c = '：';
					}
					else if (c == ';')
					{
						c = '；';
					}
					else if (c == '\'')
					{
						c = '′';
					}
					else if (c == '\"')
					{
						c = '″';
					}
					else if (c == '!')
					{
						c = '！';
					}
					else if (c == '?')
					{
						c = '？';
					}
					else if (c == ',')
					{
						c = '，';
					}
					else if (c == ' ')
					{
						c = '　';
					}
					else if (c == '-')
					{
						c = 'ー';
					}
					else if (c == (char)127)
					{
						c = '…';
					}

					byte[] bs = Encoding.UTF8.GetBytes(new char[] { c });
					if (bs.Length < 3)
						continue;
					int value = 0;
					for (int k = 0; k < 3; k++)
					{
						value |= bs[2 - k] << (k * 8);
					}
					excludes.Add(value);
				}

				int chars = 0;
				for (int i = 0; i < orig_char_count; i++)
				{
					int id = reader.ReadInt32();
					float x = reader.ReadSingle();
					float y = reader.ReadSingle();

					if (excludes.Contains(id))
					{
						continue;
					}
					writer.Write(id);
					//x = (float)(((double)x * 2016.0) / (double)bmp.Width);
					writer.Write(x);
					y = (float)(((double)y * 1404.0 / 1512.0));
					writer.Write(y);
					chars++;
				}

				for (int i = 32; i <= 127; i++)
				{
					char c = (char)i;
					int x = ((i - 32) % 56) * 27;
					int y = ((i - 32) / 56) * 27 + 1053;
					if (c >= '0' && c <= '9')
					{
						c = (char)('０' + (c - '0'));
					}
					else if (c >= 'A' && c <= 'Z')
					{
						c = (char)('Ａ' + (c - 'A'));
					}
					else if (c >= 'a' && c <= 'z')
					{
						c = (char)('ａ' + (c - 'a'));
					}
					else if (c == '.')
					{
						c = '．';
					}
					else if (c == ':')
					{
						c = '：';
					}
					else if (c == ';')
					{
						c = '；';
					}
					else if (c == '\'')
					{
						c = '′';
					}
					else if (c == '\"')
					{
						c = '″';
					}
					else if (c == '!')
					{
						c = '！';
					}
					else if (c == '?')
					{
						c = '？';
					}
					else if (c == ',')
					{
						c = '，';
					}
					else if (c == ' ')
					{
						c = '　';
					}
					else if (c == '-')
					{
						c = 'ー';
					}
					else if (c == 127)
					{
						c = '…';
					}

					byte[] bs = Encoding.UTF8.GetBytes(new char[] { c });
					for (int k = bs.Length; k < 4; k++)
						writer.Write((byte)0);
					for (int k = 0; k < bs.Length; k++)
						writer.Write((byte)bs[k]);
					writer.Write((float)(x / (float)bmp.Width));
					writer.Write((float)(y / (float)bmp.Height));

					chars++;
				}

				writer.FillInOffset("size");
				writer.FillInOffset("char_count", (uint)chars);
				writer.WriteSignature("Texture ");
				writer.Write(bmp.Width * bmp.Height * 4 + 36);
				writer.Write(0);
				writer.Write(bmp.Width * bmp.Height * 4);
				writer.Write(bmp.Width);
				writer.Write(bmp.Height);
				writer.Write(bmp.Width);
				writer.Write(bmp.Height);

				for (int y = 0; y < bmp.Height; y++)
				{
					for (int x = 0; x < bmp.Width; x++)
					{
						Color c = bmp.GetPixel(x, y);
						writer.Write((byte)c.B);
						writer.Write((byte)c.G);
						writer.Write((byte)c.R);
						writer.Write((byte)c.A);
					}
				}

				fnt_file.ReplaceFile("MAINTEXT.fnt", ((MemoryStream)writer.BaseStream).ToArray());
				fnt_file.Save("C:\\Program Files\\WA1\\Font.pck");
			}
		}

		private void update_text()
		{
			var encoding = System.Text.CodePagesEncodingProvider.Instance.GetEncoding("Shift-JIS");

			string[] char_names =
			{
				"",
				"Touya",
				"Yuki"
			};

			string sum = "";
			int off = 0;
			List<byte> bstring = new List<byte>();
			// Start by finding 0x0F
			try
			{
				while (off < text_data.Length)
				{
					if (text_data[off] != 0xF)
					{
						off++;
						continue;
					}

					bstring.Clear();

					string s = "";
					int character = text_data[off - 1];
					for (; off + 2 < text_data.Length; off++)
					{
						byte b1 = text_data[off];
						byte b2 = text_data[off + 1];
						if (b1 == 0 && (b2 >= 0x81))
						{
							off += 1;
							break;
						}
						else if (b2 == 0xF && b1 != character)
							character = b1;
					}

					string character_name = "";

					if (off + 1 >= text_data.Length)
						break;

					int start = off;
					if (start == 0x643)
					{

					}

					for (; off + 4 < text_data.Length; off++)
					{
						byte b1 = text_data[off];
						byte b2 = text_data[off + 1];
						byte b3 = text_data[off + 2];
						byte b4 = text_data[off + 3];

						if (b1 == 0x5C && b2 == 0x6B)
						{
							bstring.Add(b1);
							bstring.Add(b2);
							off += 11;
							continue;
						}
						if (b1 == 0 && b3 != 0x41 && b3 != 0x42)
						{
							bstring.Clear();
							off += 2;
							continue;
						}
						if (b1 == 0 && (b3 == 0x41 || b3 == 0x42))
						{
							break;
						}

						bstring.Add(b1);
					}

					if (bstring.Count <= 1)
						continue;

					if (character == 255)
						character = 0;

					int end = off;
					//off += 1;
					if (character_name == "")
					{
						if (character != 0 && character < char_names.Length && char_names[character] != "")
							s = char_names[character] + ":";
						else if (character > 0)
							s += character.ToString() + ":";
					}
					else
						s += character_name + ":";

					s += encoding.GetString(bstring.ToArray()) + "\r\n";
					s = s.Replace("\\k\\n", "\r\n").Replace("\\k", "\r\n").Replace("\\n", "\r\n");
					sum += s;
				}
			}
			catch (Exception ex) { sum += "Exception: " + ex.Message; }

			if (off + 10 < text_data.Length)
			{
				sum += "Unexpected EOF";
			}
			else
			{
				sum += "File complete.";
			}

			textBox1.Text = sum;

			textBox2_TextChanged(null, null);

			active_script = new Script(text_data);
			try
			{
				active_script.Compile(text_data);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			listBox1.Items.Clear();
			block_map.Clear();
			int bi = 0;
			foreach (var baseblock in active_script.Blocks)
			{
				var line = baseblock as LineBlock;
				if (line == null)
				{
					bi++;
					continue;
				}

				block_map.Add(bi++);
				listBox1.Items.Add(line.Text.Replace("\\n", "<ｂｒｅａｋ>"));
			}
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			update_text();
		}

		private void Form1_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{
		}

		public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
		{
			for (int i = 0; i < source.Length; i++)
			{
				if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
				{
					yield return i;
				}
			}
		}

		public static IEnumerable<int> PatternAt(string source, string pattern)
		{
			for (int i = 0; i < source.Length; i++)
			{
				if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
				{
					yield return i;
				}
			}
		}

		private List<int> find_offsets = new List<int>();
		private int find_index = -1;
		private void button1_Click(object sender, EventArgs e)
		{
			/*if (find_offsets.Count > 0)
			{
				find_index = (++find_index) % find_offsets.Count;
				textBox1.SelectionStart = find_offsets[find_index];
				textBox1.SelectionLength = textBox2.Text.Length;
				textBox1.ScrollToCaret();
				textBox1.Focus();
			}*/

			Texttool.Properties.Settings.Default.last_search = textBox2.Text;
			Texttool.Properties.Settings.Default.Save();

			if (textBox2.Text == "")
				return;

			var encoding = System.Text.CodePagesEncodingProvider.Instance.GetEncoding("Shift-JIS");
			for (int i = 0; i < pck_file.FileEntries.Count; i++)
			{
				var file = pck_file.FileEntries[i];
				if (file.Data == null)
				{
					file.Data = pck_file.GetFileData(i);
				}
				string s = encoding.GetString(file.Data);

				int off = s.IndexOf(textBox2.Text);
				if (off != -1)
				{
					cbFile.SelectedIndex = i;
					update_text();
					off = textBox1.Text.IndexOf(textBox2.Text);
					if (off >= 0)
					{
						textBox1.SelectionStart = off;
						textBox1.ScrollToCaret();
						textBox1.SelectionLength = textBox2.Text.Length;
						textBox1.Focus();
					}
					return;
				}
			}
		}

		private void cbFile_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cbFile.SelectedIndex >= 0 && cbFile.SelectedIndex < cbFile.Items.Count)
			{
				//filename = filenames[cbFile.SelectedIndex];
				//text_data = File.ReadAllBytes(filename);
				string fn = cbFile.GetItemText(pck_file.FileEntries[cbFile.SelectedIndex].FileName);
				text_data = pck_file.GetFileData(fn);
				update_text();

				Texttool.Properties.Settings.Default.last_index = cbFile.SelectedIndex;
				Texttool.Properties.Settings.Default.Save();
			}
			else
			{
				textBox1.Text = "";
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			update_text();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			pck_file.UseBigEndian = true;
			pck_file.ReplaceFile(cbFile.GetItemText(pck_file.FileEntries[cbFile.SelectedIndex].FileName), active_script.Compile(null));
			pck_file.Save(Texttool.Properties.Settings.Default.last_dir);

			btnSave.Text = "Save";
			btnSave.BackColor = SystemColors.Control;
		}

		private void listBox1_DoubleClick(object sender, EventArgs e)
		{
			editing = listBox1.SelectedIndex;
			edit_box.Visible = true;

			var loc = listBox1.GetItemRectangle(listBox1.SelectedIndex).Location;
			loc.X += listBox1.Location.X;
			loc.Y += listBox1.Location.Y;
			edit_box.Location = loc;
			edit_box.Focus();
			edit_box.BringToFront();
			edit_box.Text = listBox1.GetItemText(listBox1.SelectedItem);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			//pck_file.ExtractAllFiles(Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir));

			var dir = Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir) + "\\excel\\blob";
			Directory.CreateDirectory(dir);

			export_excel(true, false);
		}

		private void export_excel(bool include_jp, bool merge_trimmed, string[] src_col_values = null)
		{
			var dir = Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir) + "\\excel\\";
			if (!include_jp)
				dir += "trimmed\\";
			Directory.CreateDirectory(dir);
			string fname = cbFile.GetItemText(cbFile.SelectedItem);
			if (File.Exists(dir + fname + ".xlsx") && include_jp && !merge_trimmed)
			{
				if (MessageBox.Show(dir + fname + ".xlsx already exists.\n\nReplace?", "Confirm File Overwrite", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
				{
					return;
				}
			}

			if (include_jp)
			{
				if(!File.Exists(dir + "blob\\" + fname))
					File.WriteAllBytes(dir + "blob\\" + fname, text_data);
				else
				{
					text_data = File.ReadAllBytes(dir + "blob\\" + fname);
					update_text();
				}
			}

			using var stream = new FileStream(dir + fname + ".xlsx", FileMode.Create, FileAccess.Write);
			using var writer = new XlsxWriter(stream);

			var headerStyle = new XlsxStyle(
				new XlsxFont("Microsoft GothicNeo", 10, Color.White, bold: true),
				new XlsxFill(Color.FromArgb(0x46, 0x46, 0x46)),
				XlsxBorder.Around(new XlsxBorder.Line(Color.Black, XlsxBorder.Style.Thin)),
				XlsxStyle.Default.NumberFormat,
				XlsxAlignment.Default);
			var text_style = new XlsxStyle(
				new XlsxFont("Microsoft GothicNeo", 10, Color.Black),
				new XlsxFill(Color.FromArgb(255, 255, 255)),
				XlsxBorder.Around(new XlsxBorder.Line(Color.Black, XlsxBorder.Style.Thin)),
				XlsxStyle.Default.NumberFormat,
				XlsxAlignment.Default);
			var en_style = new XlsxStyle(
				new XlsxFont("Segoe UI", 10, Color.Black),
				new XlsxFill(Color.FromArgb(255, 255, 255)),
				XlsxBorder.Around(new XlsxBorder.Line(Color.Black, XlsxBorder.Style.Thin)),
				XlsxStyle.Default.NumberFormat,
				XlsxAlignment.Default);
			var ml_style = new XlsxStyle(
				new XlsxFont("Segoe UI", 10, Color.Black),
				new XlsxFill(Color.FromArgb(200, 200, 200)),
				XlsxBorder.Around(new XlsxBorder.Line(Color.Black, XlsxBorder.Style.Thin)),
				XlsxStyle.Default.NumberFormat,
				XlsxAlignment.Default);



			writer
				.BeginWorksheet(fname, columns: new[]
				{
					XlsxColumn.Formatted(width: 8.0),
					XlsxColumn.Formatted(width: 8.0),
					XlsxColumn.Formatted(width: 11.0),
					XlsxColumn.Formatted(width: 22.0),
					XlsxColumn.Formatted(width: 115, 6),
				})
				.SetDefaultStyle(headerStyle)
				.BeginRow().Write("Status").Write("Block (DO NOT CHANGE)").Write("Speaker ID").Write("Speaker Name").Write("Japanese").Write("Edited").Write("Initial & Notes").Write("DeepL").Write("Google").Write("Other");

			int bid = 0;
			string[] col_values = { "", "", "", "", "" };
			int row = 0;

			foreach (var base_block in active_script.Blocks)
			{
				var line = base_block as LineBlock;
				if (line == null)
				{
					bid++;
					continue;
				}

				for (int i = 0; i < 5; i++)
				{
					string s = "";
					if (src_col_values != null && row * 5 + i < src_col_values.Length)
					{
						s = src_col_values[row * 5 + i];
					}
					if (s != null)
						col_values[i] = s;
					else
						col_values[i] = "";
				}

				writer.BeginRow()
					.SetDefaultStyle(text_style).Write(" ")
					.SetDefaultStyle(text_style).Write(bid.ToString())
					.SetDefaultStyle(text_style).Write(line.Character > 0 ? line.Character.ToString() : "")
					.SetDefaultStyle(text_style).Write(line.CharacterName)
					.SetDefaultStyle(text_style).Write(include_jp ? line.Text : "")
					.SetDefaultStyle(text_style).Write(col_values[0])
					.SetDefaultStyle(ml_style).Write(col_values[1])
					.SetDefaultStyle(ml_style).Write(col_values[2])
					.SetDefaultStyle(ml_style).Write(col_values[3])
					.SetDefaultStyle(ml_style).Write(col_values[4]);

				row++;
				bid++;
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			import_spreadsheet();
		}

		private void import_spreadsheet()
		{
			var dir = Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir) + "\\excel\\";
			string fname = cbFile.GetItemText(cbFile.SelectedItem);
			using var stream = new FileStream(dir + fname + ".xlsx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using var reader = ExcelReaderFactory.CreateReader(stream);

			Script new_script = new Script(File.ReadAllBytes(dir + "blob\\" + fname));

			try
			{
				int block = 0;
				do
				{
					reader.Read();
					while (reader.Read())
					{
						int src_index = 0;

						string status = reader.GetString(0);
						string str_bid = reader.GetString(1);
						string id = reader.GetString(2);
						string name = reader.GetString(3);
						string jp_text = reader.GetString(4);
						string en_text = reader.GetString(5);
						if (str_bid == null && jp_text == null)
							continue;

						LineBlock line_block = null;
						if (str_bid != null)
						{
							if (!int.TryParse(str_bid, out src_index) || src_index < 0 || src_index >= new_script.Blocks.Count)
							{
								throw new Exception("Block ID mismatch (" + str_bid + ")");
							}

							line_block = new_script.Blocks[src_index] as LineBlock;
						}
						else
						{
							while (line_block == null && block < new_script.Blocks.Count)
							{
								src_index = block;
								Block b = new_script.Blocks[block++];
								line_block = b as LineBlock;
							}
						}
						if (line_block == null)
						{
							throw new Exception("Line block mismatch (" + str_bid + " " + jp_text + ")");
						}

						if (jp_text != null && jp_text != "" && jp_text != line_block.Text)
						{
							throw new Exception("JP text mismatch");
						}

						if (!string.IsNullOrEmpty(en_text))
						{
							if (line_block.Character != 0 && line_block.Character != 255)
							{
								if (!en_text.StartsWith("\"") && !en_text.StartsWith("″"))
								{
									en_text = "″" + en_text;
								}
								if (!en_text.EndsWith("\"") && !en_text.EndsWith("″"))
								{
									en_text += "″";
								}
							}
							line_block.Text = filter_string(en_text);
						}
						else
						{
							line_block.Text = jp_text;
						}

						new_script.Blocks[src_index] = line_block;
					}
				} while (reader.NextResult());

				active_script = new_script;
				text_data = new_script.Compile(null);

				update_text();

				btnSave.Text = "Save *";
				btnSave.BackColor = Color.FromArgb(255, 200, 150);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to import:" + ex.Message);
			}
		}

		private void export_trimmed(bool merge)
		{
			try
			{
				var dir = Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir) + "\\excel\\";
				if (merge)
					dir += "trimmed\\";
				string fname = cbFile.GetItemText(cbFile.SelectedItem);
				using var stream = new FileStream(dir + fname + ".xlsx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				using var reader = ExcelReaderFactory.CreateReader(stream);

				List<string> col_values = new List<string>();

				do
				{
					reader.Read();
					while (reader.Read())
					{
						string status = reader.GetString(0);
						string id = reader.GetString(1);

						if (id == null)
							continue;

						col_values.Add(reader.GetString(5));
						col_values.Add(reader.GetString(6));
						col_values.Add(reader.GetString(7));
						col_values.Add(reader.GetString(8));
						col_values.Add(reader.GetString(9));
					}
				} while (reader.NextResult());

				export_excel(merge, merge, col_values.ToArray());
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to trim file: " + ex.Message);
			}
		}

		private void button6_Click(object sender, EventArgs e)
		{
			export_trimmed(true);
			import_spreadsheet();
			btnSave.Text = "Save *";
			btnSave.BackColor = Color.FromArgb(255, 200, 100);
		}

		private void button3_Click_1(object sender, EventArgs e)
		{
			export_trimmed(false);
		}
	}

	public abstract class Block
	{
		public int Start = 0;
		public abstract void Write(List<byte> dest);
	}

	public class RawBlock : Block
	{
		public byte[] Data;
		public override void Write(List<byte> dest)
		{
			dest.AddRange(Data);
		}
	}

	public class LineBlock : Block
	{
		public int Character;
		public string CharacterName = "";
		public string Text = "";

		public Encoding encoding;

		List<byte> breakblock;

		public LineBlock(Encoding _e, int start)
		{
			encoding = _e;
			Start = start;
		}

		public override void Write(List<byte> dest)
		{
			string text = Text;
			text = text.Replace("\n", "\\n");
			text = text.Replace("<ｂｒｅａｋ>", "\\n");
			if (text.IndexOf("<ｋａｅｒｂ>") != -1)
			{
				if (breakblock == null)
				{
					breakblock = new List<byte>();
					breakblock.Add(0);
					breakblock.Add(0);
					breakblock.Add(0x41);
					breakblock.Add((byte)Character);
					breakblock.Add(0xF);
					breakblock.Add(0);
					breakblock.Add(0);
					breakblock.Add(0x2C);
					breakblock.Add(0x7F);
					breakblock.Add(0);
					breakblock.Add(0);
					breakblock.Add(0);
				}
				text = text.Replace("<ｋａｅｒｂ>", encoding.GetString(breakblock.ToArray()));
			}
			if (text == null || text == "")
				return;
			dest.AddRange(encoding.GetBytes(text));
		}
	}

	public class Script
	{
		public List<Block> Blocks = new List<Block>();

		public byte[] Compile(byte[] compare)
		{
			List<byte> output = new List<byte>();
			foreach (var b in Blocks)
			{
				b.Write(output);
			}

			if (compare != null)
			{
				if (output.Count != compare.Length)
				{
					throw new Exception("Size mismatch (" + output.Count + " vs " + compare.Length + " expected)");
				}

				for (int i = 0; i < output.Count; i++)
				{
					if (output[i] != compare[i])
					{
						throw new Exception("Mismatch at " + i);
					}
				}
			}

			return output.ToArray();
		}

		public Script(byte[] text_data)
		{
			if (text_data == null)
			{
				Blocks = new List<Block>();
				return;
			}
			var encoding = System.Text.CodePagesEncodingProvider.Instance.GetEncoding("Shift-JIS");


			string[] char_names =
			{
				"",
				"Touya",
				"Yuki"
			};

			int off = 0;
			List<byte> bstring = new List<byte>();
			List<Block> blocks = new List<Block>();

			try
			{
				while (off < text_data.Length)
				{
					if (text_data[off] != 0xF)
					{
						off++;
						continue;
					}

					bstring.Clear();

					string s = "";
					int character = text_data[off - 1];
					for (; off + 2 < text_data.Length; off++)
					{
						byte b1 = text_data[off];
						byte b2 = text_data[off + 1];
						if (b1 == 0 && ((b2 >= 0x81) || (b2 >= 0x20 && b2 <= 0x7D)))
						{
							off += 1;
							break;
						}
						else if (b2 == 0xF && b1 != character)
							character = b1;
					}

					string character_name = "";

					if (off + 1 >= text_data.Length)
						break;

					int start = off;
					for (; off + 4 < text_data.Length; off++)
					{
						byte b1 = text_data[off];
						byte b2 = text_data[off + 1];
						byte b3 = text_data[off + 2];
						byte b4 = text_data[off + 3];

						if (b1 == 0x5C && b2 == 0x6B)
						{
							if (bstring.Count > 0)
							{
								LineBlock sub_block = new LineBlock(encoding, start);
								sub_block.Character = character;
								sub_block.Text = encoding.GetString(bstring.ToArray());
								blocks.Add(sub_block);
							}

							bstring.Clear();

							off += 11;

							continue;
						}
						else if (b1 == 0x5C && b2 == 0x6E)
						{
							if (bstring.Count == 0)
							{
								/*LineBlock sub_block = new LineBlock(encoding, start);
								sub_block.Character = character;
								sub_block.Text = encoding.GetString(bstring.ToArray());
								blocks.Add(sub_block);

								bstring.Clear();*/

								off++;

								continue;
							}
						}
						if (b1 == 0 && b3 != 0x41 && b3 != 0x42)
						{
							bstring.Clear();
							off += 2;
							continue;
						}
						if (b1 == 0 && (b3 == 0x41 || b3 == 0x42))
						{
							break;
						}

						if (bstring.Count == 0)
							start = off;
						bstring.Add(b1);
					}

					if (bstring.Count <= 1)
						continue;

					if (character == 255)
						character = 0;

					int end = off;
					if (character_name == "")
					{
						if (character != 0 && character < char_names.Length && char_names[character] != "")
							s = char_names[character] + ":";
						else if (character > 0)
							s += character.ToString() + ":";
					}
					else
						s += character_name + ":";

					LineBlock l = new LineBlock(encoding, start);
					l.Character = character;
					if (character >= 0 && character < char_names.Length)
						l.CharacterName = char_names[character];
					l.Text = encoding.GetString(bstring.ToArray());//.Replace("\\n", "<ｂｒｅａｋ>");
					blocks.Add(l);
				}
			}
			catch (Exception ex)
			{

			}

			if (blocks.Count == 0)
			{
				Blocks = blocks;
				return;
			}

			// Fill in the missing blocks
			off = 0;
			int next_block = 0;
			int num_blocks = blocks.Count;
			while (off < text_data.Length)
			{
				RawBlock b = new RawBlock();
				b.Start = off;
				if (next_block < num_blocks)
				{
					b.Data = new byte[blocks[next_block].Start - off];
					Buffer.BlockCopy(text_data, off, b.Data, 0, b.Data.Length);
					off += ((LineBlock)blocks[next_block]).encoding.GetBytes(((LineBlock)blocks[next_block]).Text).Length;
					next_block++;
				}
				else
				{
					b.Data = new byte[text_data.Length - off];
					Buffer.BlockCopy(text_data, off, b.Data, 0, b.Data.Length);
				}
				blocks.Add(b);
				off += b.Data.Length;
			}

			blocks.Sort((a, b) => a.Start - b.Start);

			Blocks = blocks;
		}
	}
}
