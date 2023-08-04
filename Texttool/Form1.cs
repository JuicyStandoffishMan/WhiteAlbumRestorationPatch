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
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Collections;
using SharpCompress.Writers;
using DALLib;
using SharpCompress.Crypto;

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

		public const int CharsPerMessage = 90;
		public const int CharsPerLine = 0x38;// 0x3D;

		public static bool IsEnglish = true;

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

			comboBox1.Items.Add("UTF8");
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
			//button6.Tag = "Merges the selected script with the corresponding trimmed excel spreadsheet.";
			//button3.Tag = "Trims the Japanese text from the selected script and writes it to the trimmed folder.";

			button4.MouseHover += Button_Showtooltip;
			button5.MouseHover += Button_Showtooltip;
			//button6.MouseHover += Button_Showtooltip;
			//button3.MouseHover += Button_Showtooltip;

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
			if (path == null || path == "" || !File.Exists(path))
				return;

			if (path.ToLower().EndsWith(".tex"))
			{
				TEXFile tex = new TEXFile { UseBigEndian = false };
				tex.Load(path, true);
				if (tex.Frames.Count > 1)
				{
					string dir = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + " parts/";
					Directory.CreateDirectory(dir);
					tex.DumpFrames(dir);
				}
				return;
			}
			else if (path.EndsWith(".fnt"))
			{
				//read_fnt(path);
				return;
			}
			else if (path.EndsWith(".png"))
			{
				//read_png(path);
				WritePNGToTex(path);
				return;

			}
			else if (path.ToLower().EndsWith("white album.exe"))
			{
				//patch_exe(path);
				return;
			}
			else if (path.ToLower().EndsWith("bgm.pck"))
			{
				dump_music(path);
				return;
			}
			else if (path.ToLower().EndsWith("bg.pck"))
			{
				dump_bg(path);
				return;
			}
			else if (!path.ToLower().EndsWith("script.sdat"))
			{
				return;
			}

			bool save = false;
			if (Texttool.Properties.Settings.Default.last_dir != path)
			{
				Texttool.Properties.Settings.Default.last_dir = path;
				save = true;
			}

			if (path.ToLower().EndsWith("eng\\script.sdat") || path.ToLower().EndsWith("eng/scripts.sdat"))
			{
				IsEnglish = true;
			}
			else
			{
				IsEnglish = false;
			}

			string slist = "";
			cbFile.Items.Clear();

			cbFile.Enabled = false;
			last_filter_line_count = 0;
			last_filter_char_count = 0;
			last_filter_string = "";

			pck_file = new PCKFile { UseBigEndian = false };
			pck_file.Load(path, false);
			foreach (var v in pck_file.FileEntries)
			{
				if (!v.FileName.StartsWith("S99") && !v.FileName.StartsWith("Script.flag"))
				{
					cbFile.Items.Add(v.FileName);// + " (" + v.DataLength + " bytes)");
					slist += v.FileName + "\n";
				}
			}
			if (save)
			{
				/*if (MessageBox.Show("Would you like to import all scripts from the trimmed folder?", "Import trimmed scripts?", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					ImportAll();
				}*/
			}
			//PrintProgress();
			//Clipboard.SetText(slist);
			//pck_file.ExtractAllFiles(Path.GetFileNameWithoutExtension(filePath));
			cbFile.Enabled = true;

			int index = 0;
			cbFile.SelectedIndex = Texttool.Properties.Settings.Default.last_index;

			button1.Text = "Find";

			this.Text = Path.GetFileName(path) + " (" + (IsEnglish ? "English" : "Japanese") + ") - White Album script viewer";


			if (save)
			{
				Texttool.Properties.Settings.Default.Save();
			}
		}


		private int last_filter_line_count = 0;
		private int last_filter_char_count = 0;
		private string last_filter_string = "";
		private string filter_string(string s)
		{
			StringBuilder sb = new StringBuilder();
			int line_count = 0;
			int char_count = 0;
			if (s.Contains("WA_op"))
				return s;

			if (last_filter_string.EndsWith("<pause>"))
			{
				line_count = last_filter_line_count + 1;
				char_count = last_filter_char_count + 1;
			}

			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];

				if (s.IndexOf("<pause>", i) == i)
				{
					i += 6;
					sb.Append("<pause>");
					continue;
				}
				if (s.IndexOf("<nopause>", i) == i)
				{
					i += 8;
					sb.Append("<nopause>");
					char_count = 0;
					line_count = 0;
					continue;
				}

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
				else if (c == '(')
				{
					c = '（';
				}
				else if (c == ')')
				{
					c = '）';
				}
				else if (c == '%')
				{
					c = '％';
				}
				else if (c == '<')
				{
					// Next character should be R, c, or W
					sb.Append(c);
					sb.Append(s[i + 1]);

					if (s[i + 1] == 'c')
					{
						// ASCII color value
						sb.Append(s[i + 2]);
						i++;
					}
					i++;
					continue;
				}
				else if (c == ' ' || c == '　' || c == '\"')
				{
					int space = s.IndexOf(' ', i + 1);
					int dash = s.IndexOf('-', i + 1);
					int pause = s.IndexOf("<pause>", i + 1);
					int nopause = s.IndexOf("<nopause>", i + 1);
					// Get the min index out of space/dash/pause/nopause that is NOT -1
					int ind = 10000000;
					if (space != -1)
						ind = Math.Min(ind, space);
					if (dash != -1)
						ind = Math.Min(ind, dash);
					if (pause != -1)
						ind = Math.Min(ind, pause);
					if (nopause != -1)
						ind = Math.Min(ind, nopause);
					if (ind == 10000000)
						ind = -1;

					if (ind == -1)
						ind = s.Length - (i + 1);
					else
						ind -= i + 1;
					if (ind + line_count >= CharsPerLine - 4)
					{
						if (char_count >= CharsPerMessage)
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
					else if (char_count >= CharsPerMessage - CharsPerLine)
					{
						// Look ahead to get next word size
						int next_space = s.IndexOf(' ', i + 1);
						int next_dash = s.IndexOf('-', i + 1);
						int next_pause = s.IndexOf("<pause>", i + 1);
						int next_nopause = s.IndexOf("<nopause>", i + 1);
						int next_ind = 10000000;
						if (next_space != -1)
							next_ind = Math.Min(next_ind, next_space);
						if (next_dash != -1)
							next_ind = Math.Min(next_ind, next_dash);
						if (next_pause != -1)
							next_ind = Math.Min(next_ind, next_pause);
						if (next_nopause != -1)
							next_ind = Math.Min(next_ind, next_nopause);
						if (next_ind == 10000000)
							next_ind = -1;

						if (next_ind == -1)
							next_ind = s.Length - (i + 1);
						else
							next_ind -= i + 1;
						if (char_count >= CharsPerMessage - next_ind - 2)
						{
							sb.Append("<ｋａｅｒｂ>");
							char_count = 0;
							line_count = 0;
							continue;
						}
					}
					c = '　';
				}
				else if (c == '-')
				{
					c = 'ー';
				}
				else if (c == '~')
				{
					c = '～';
				}

				sb.Append(c);
				line_count++;
				char_count++;
			}
			sb = sb.Replace("...", "…");
			sb = sb.Replace("．．．", "…");

			last_filter_char_count = char_count;
			last_filter_line_count = line_count;
			last_filter_string = s;

			return sb.ToString();
		}

		private void PrintProgress()
		{
			int line_count = 0;
			int num_translated_lines = 0;
			int num_translated_words = 0;
			foreach (var v in pck_file.FileEntries)
			{
				Script ss;
				if (File.Exists("C:\\Program Files\\WA1\\Data\\Game\\excel\\blob\\" + v.FileName))
				{
					ss = new Script(File.ReadAllBytes("C:\\Program Files\\WA1\\Data\\Game\\excel\\blob\\" + v.FileName));
				}
				else
				{
					ss = new Script(v.Data);
				}
				foreach (var block in ss.Blocks)
				{
					if (block is LineBlock)
					{
						line_count++;
						if (((LineBlock)block).CharacterName == "Izumi")
						{

						}
					}
				}
			}

			foreach (var file in Directory.EnumerateFiles("C:\\Program Files\\WA1\\Data\\Game\\excel\\trimmed\\"))
			{
				using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				using var reader = ExcelReaderFactory.CreateReader(stream);

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

						if (string.IsNullOrEmpty(en_text))
							en_text = reader.GetString(6);

						if (!string.IsNullOrEmpty(en_text))
						{
							num_translated_lines++;
							num_translated_words += en_text.Split(' ').Length;
						}
					}
				} while (reader.NextResult());
			}

			if (num_translated_lines > 0)
				MessageBox.Show(num_translated_lines + "/" + line_count + " lines translated (" + ((double)num_translated_lines / (double)line_count * 100.0).ToString("0.00") + "%)\n" + num_translated_words + " words");
		}

		private void patch_exe(string path)
		{
			var encoding = System.Text.UTF8Encoding.UTF8;

			int spacing = -13;

			// Main text spacing
			byte[] bytes = File.ReadAllBytes(path);
			bytes[0x17DC1] = (byte)CharsPerLine; // Visual chars per line
			bytes[0x17B9C] = (byte)CharsPerLine; // Actual chars per line
			bytes[0x176C6] = 0x0E; // X spacing
			bytes[0x176D7] = 0x24; // Y spacing

			// Chat genre spacing
			bytes[0x4F2DB] = 0x0E; // X spacing

			// Name box X spacing
			byte bb = (byte)((spacing) & 0xff);
			bytes[0x4a71e] = bb;
			bytes[0x4a720] = bb;

			// asm modifying options spacing since i can't find where [EDI+0x58]=5 comes from...
			// mov eax, <spacing>
			// fst dword ptr ss:[esp+4]
			// nop
			// nop
			// nop
			// nop
			{
				byte[] asm =
				{
					0xb8, (byte)((spacing >> 0) & 0xff), (byte)((spacing >> 8) & 0xff),(byte)((spacing >> 16) & 0xff),(byte)((spacing >> 24) & 0xff), 0xd9, 0x54, 0x24, 0x04, 0x90, 0x90, 0x90, 0x90
				};
				Array.Copy(asm, 0, bytes, 0x160ae, asm.Length);
			}

			// asm modifying backlog spacing
			// mov edx, <spacing>
			// nop
			{
				byte[] asm =
				{
					0xba,  (byte)((spacing >> 0) & 0xff), (byte)((spacing >> 8) & 0xff),(byte)((spacing >> 16) & 0xff),(byte)((spacing >> 24) & 0xff), 0x90
				};
				Array.Copy(asm, 0, bytes, 0x2676a, asm.Length);
			}

			// Chat genre options
			/*string[] options =
			{
				"Chat",
				"Hobbies",
				"Entertainment",
				"Literature",
				"Love"
			};*/
			string[] options =
			{
				"Chat",
				"Hobby",
				"Music",
				"Books",
				"Love"
			};

			int[] src_options =
			{
				0x225198,
				0x2251a0,
				0x2251a8,
				0x2251b0,
				0x2251b8
			};

			int[] dst_options =
			{
				0x4f48d,
				0x4f49a,
				0x4f4a7,
				0x4f4b4,
				0x4f4c1
			};

			int dst = 0x225118;
			int start = dst;
			for (int i = 0; i < 5; i++)
			{
				// Update the string pointer
				int dst_value = 0x00625f98 - 0x80 + dst - start;
				bytes[dst_options[i]] = (byte)((dst_value >> 0) & 0xFF);
				bytes[dst_options[i] + 1] = (byte)((dst_value >> 8) & 0xFF);
				bytes[dst_options[i] + 2] = (byte)((dst_value >> 16) & 0xFF);
				bytes[dst_options[i] + 3] = (byte)((dst_value >> 24) & 0xFF);

				byte[] chat_bytes = encoding.GetBytes(filter_string(options[i]));
				Array.Copy(chat_bytes, 0, bytes, dst, chat_bytes.Length);
				dst += chat_bytes.Length;
				bytes[dst++] = 0;
				bytes[dst++] = 0;
			}

			// Character names
			if (true)
			{
				int char_name_ptr_table = 0x23e1f8;
				//int char_name_dest = 0x222ee8;
				int char_name_dest = 0x2258c0;
				int ptr_add_value = 0x400E00;
				string name_sum = ""; //string[] names = new string[32];
				for (int i = 0; i < 32; i++)
				{
					/*int program_addr = 0;
					program_addr += bytes[char_name_ptr_table + i * 4 + 0] << 0;
					program_addr += bytes[char_name_ptr_table + i * 4 + 1] << 8;
					program_addr += bytes[char_name_ptr_table + i * 4 + 2] << 16;
					program_addr += bytes[char_name_ptr_table + i * 4 + 3] << 24;

					int real_addr = program_addr - 0x6216b4 + 0x2208b4;
					List<byte> char_data = new List<byte>();
					while (true)
					{
						byte b = bytes[real_addr++];
						if (b == 0)
							break;
						char_data.Add(b);
					}

					name_sum += encoding.GetString(char_data.ToArray()) + "\r\n";*/
					bytes[char_name_ptr_table + i * 4 + 0] = (byte)(((char_name_dest + ptr_add_value) >> 0) & 0xFF);
					bytes[char_name_ptr_table + i * 4 + 1] = (byte)(((char_name_dest + ptr_add_value) >> 8) & 0xFF);
					bytes[char_name_ptr_table + i * 4 + 2] = (byte)(((char_name_dest + ptr_add_value) >> 16) & 0xFF);
					bytes[char_name_ptr_table + i * 4 + 3] = (byte)(((char_name_dest + ptr_add_value) >> 24) & 0xFF);

					byte[] nb = encoding.GetBytes(filter_string(Script.GetShortCharName(i + 1)));
					Array.Copy(nb, 0, bytes, char_name_dest, nb.Length);
					char_name_dest += nb.Length;
					bytes[char_name_dest++] = 0;
					bytes[char_name_dest++] = 0;

					/*if (i == 5)
					{
						char_name_dest = 0x222f58;//0x22991C;
					}
					else if (i == 9)
					{
						char_name_dest = 0x2298e0;
					}
					else if (i == 11)
					{
						char_name_dest = 0x22991c;
					}*/
				}
			}
			//Clipboard.SetText(name_sum);

			try
			{
				BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
				bw.Write(bytes);
				bw.Close();

				MessageBox.Show("Exe successfully patched.", "Patch Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to patch exe " + path + "\n: " + ex.Message);
			}
		}

		public void dump_music(string path)
		{
			string folder = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar + "bgm" + Path.DirectorySeparatorChar;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(folder);
			}
			var pck = new PCKFile { UseBigEndian = false };
			pck.Load(path, false);
			foreach (var v in pck.FileEntries)
			{
				File.WriteAllBytes(folder + v.FileName + ".ogg", v.Data);
			}
		}

		public void dump_bg(string path)
		{
			string folder = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar + "bg" + Path.DirectorySeparatorChar;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(folder);
			}
			var pck = new PCKFile { UseBigEndian = false };
			pck.Load(path, false);
			foreach (var v in pck.FileEntries)
			{
				File.WriteAllBytes(folder + v.FileName, v.Data);
				TEXFile tex = new TEXFile { UseBigEndian = true };
				tex.Load(folder + v.FileName, true);
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
				PCKFile fnt_file = new PCKFile { UseBigEndian = false };
				fnt_file.Load("C:\\Program Files\\WA1\\Font.pck");
				ExtendedBinaryReader reader = new ExtendedBinaryReader(fnt_file.GetFileStream("MAINTEXT.fnt"), false);
				ExtendedBinaryWriter writer = new ExtendedBinaryWriter(new MemoryStream(64 * 1024 * 1024), false);

				reader.JumpAhead(8);
				int tex_off = reader.ReadInt32();
				int char_height = reader.ReadInt32();
				int orig_char_count = reader.ReadInt32();

				writer.WriteDALSignature("Table", false);
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
					else if (c == '~')
					{
						c = '～';
					}
					else if (c == '(')
					{
						c = '（';
					}
					else if (c == ')')
					{
						c = '）';
					}
					else if (c == '%')
					{
						c = '％';
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
					else if (c == '~')
					{
						c = '～';
					}
					else if (c == '(')
					{
						c = '（';
					}
					else if (c == ')')
					{
						c = '）';
					}
					else if (c == '%')
					{
						c = '％';
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
				fnt_file.ReplaceFile("SELECTTEXT.fnt", ((MemoryStream)writer.BaseStream).ToArray());
				fnt_file.Save("C:\\Program Files\\WA1\\Font.pck");
			}
		}

		public void WritePNGToTex(string filename)
		{
			Bitmap bmp = new Bitmap(filename);
			ExtendedBinaryWriter writer = new ExtendedBinaryWriter(new MemoryStream(64 * 1024 * 1024), true);
			writer.WriteSignature("Texture ");
			writer.Write(bmp.Width * bmp.Height * 4 + 36);
			writer.Write((byte)0);
			writer.Write((byte)0);
			writer.Write((byte)0);
			writer.Write((byte)8);
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

			File.WriteAllBytes(Path.GetDirectoryName(filename) + "/" + Path.GetFileNameWithoutExtension(filename) + ".tex", ((MemoryStream)writer.BaseStream).ToArray());
		}

		private void update_text()
		{
			var encoding = System.Text.UTF8Encoding.UTF8;

			int off = 0;
			List<byte> bstring = new List<byte>();

			last_filter_line_count = 0;
			last_filter_char_count = 0;
			last_filter_string = "";

			try
			{
				active_script = new Script(text_data);
				active_script.Compile(text_data);
			}
			catch (Exception ex)
			{
				if (active_script == null || active_script.Blocks.Count > 1)
				{
					MessageBox.Show(ex.Message);
					return;
				}
			}

			string sum = "";
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

				string s = "";
				if (line.IsDialogue)
					s += line.CharacterName + ": ";
				s += line.Text;
				listBox1.Items.Add(s.Replace("\\n", "<ｂｒｅａｋ>"));
				s = s.Replace("\\n", " ");
				sum += s + "\r\n";
			}

			textBox1.Text = sum;
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
			if (pck_file == null)
				return;

			/*if (find_offsets.Count > 0)
			{
				find_index = (++find_index) % find_offsets.Count;
				textBox1.SelectionStart = find_offsets[find_index];
				textBox1.SelectionLength = textBox2.Text.Length;
				textBox1.ScrollToCaret();
				textBox1.Focus();
			}*/

			textBox2.Text = filter_string(textBox2.Text);

			Texttool.Properties.Settings.Default.last_search = textBox2.Text;
			Texttool.Properties.Settings.Default.Save();

			if (textBox2.Text == "")
				return;

			var encoding = System.Text.UTF8Encoding.UTF8;
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
			if (pck_file == null)
				return;

			last_filter_line_count = 0;
			last_filter_char_count = 0;
			last_filter_string = "";

			pck_file.UseBigEndian = false;
			pck_file.ReplaceFile(cbFile.GetItemText(pck_file.FileEntries[cbFile.SelectedIndex].FileName), active_script.Compile(null));
			pck_file.Save(Texttool.Properties.Settings.Default.last_dir);

			btnSave.Text = "Save";
			btnSave.BackColor = SystemColors.Control;
		}

		private void listBox1_DoubleClick(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex == -1)
				return;

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
			var dir = Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir) + "\\excel\\" + (IsEnglish ? "eng\\" : "jap\\");
			if (dir == null || pck_file == null)
				return;

			if (!include_jp)
				dir += "trimmed\\";
			Directory.CreateDirectory(dir);
			Directory.CreateDirectory(dir + "blob\\");

			string fname = cbFile.GetItemText(cbFile.SelectedItem);
			if (File.Exists(dir + fname + ".xlsx") && include_jp && !merge_trimmed)
			{
				if (MessageBox.Show(dir + fname + ".xlsx already exists.\n\nReplace?", "Confirm File Overwrite", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
				{
					return;
				}
			}

			byte[] old_text = null;
			if (include_jp)
			{
				if (!File.Exists(dir + "blob\\" + fname))
					File.WriteAllBytes(dir + "blob\\" + fname, text_data);
				else
				{
					text_data = File.ReadAllBytes(dir + "blob\\" + fname);
					update_text();
				}
			}
			else
			{
				old_text = new byte[text_data.Length];
				Array.Copy(text_data, old_text, old_text.Length);
				text_data = File.ReadAllBytes(Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir) + "\\excel\\" + "blob\\" + fname);
				update_text();
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
			var text_style2 = new XlsxStyle(
				new XlsxFont("Microsoft GothicNeo", 10, Color.Black),
				new XlsxFill(Color.FromArgb(230, 230, 230)),
				XlsxBorder.Around(new XlsxBorder.Line(Color.Black, XlsxBorder.Style.Thin)),
				XlsxStyle.Default.NumberFormat,
				XlsxAlignment.Default);
			var ml_style = new XlsxStyle(
				new XlsxFont("Segoe UI", 10, Color.Black),
				new XlsxFill(Color.FromArgb(220, 220, 220)),
				XlsxBorder.Around(new XlsxBorder.Line(Color.Black, XlsxBorder.Style.Thin)),
				XlsxStyle.Default.NumberFormat,
				XlsxAlignment.Default);
			var ml_style2 = new XlsxStyle(
				new XlsxFont("Segoe UI", 10, Color.Black),
				new XlsxFill(Color.FromArgb(190, 190, 190)),
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
				.BeginRow().Write("Status").Write("Block (DO NOT CHANGE)").Write("Speaker ID").Write("Speaker Name").Write("Japanese").Write("Edited").Write("Initial").Write("Notes").Write("Other").Write("Other");

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

				var s1 = (row % 2 == 0 ? text_style : text_style2);
				var s2 = (row % 2 == 0 ? ml_style : ml_style2);
				writer.BeginRow()
					.SetDefaultStyle(s1).Write(" ")
					.SetDefaultStyle(s1).Write(bid.ToString())
					.SetDefaultStyle(s1).Write((line.CharacterValue.b4 != 0 ? line.CharacterValue.b4.ToString() : ""))
					.SetDefaultStyle(s1).Write(line.CharacterName)
					.SetDefaultStyle(s1).Write(!IsEnglish && include_jp ? line.Text : "")
					.SetDefaultStyle(s1).Write(col_values[0])
					.SetDefaultStyle(s2).Write(IsEnglish && include_jp ? line.Text : "")
					.SetDefaultStyle(s2).Write(col_values[2])
					.SetDefaultStyle(s2).Write(col_values[3])
					.SetDefaultStyle(s2).Write(col_values[4]);

				row++;
				bid++;
			}

			if (old_text != null)
			{
				text_data = old_text;
				update_text();
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			if (!IsEnglish)
			{
				MessageBox.Show("Can only import English scripts", "English Only", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			//import_spreadsheet();
			import_raw_spreadsheet("", "stripped\\");
		}

		static string sum_removed = "";
		private void import_raw_spreadsheet(string fname = "", string extra_folder = "")
		{
			try
			{
				var dir = Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir) + "\\excel\\" + extra_folder;
				if (string.IsNullOrEmpty(fname))
				{
					fname = cbFile.GetItemText(cbFile.SelectedItem);
				}
				if (!File.Exists(dir + fname + ".xlsx"))
				{
					return;
				}
				using var stream = new FileStream(dir + fname + ".xlsx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				using var reader = ExcelReaderFactory.CreateReader(stream);

				Script new_script = null;
				if(File.Exists(dir + "blob\\" + fname))
					new_script = new Script(File.ReadAllBytes(dir + "blob\\" + fname));
				else if(File.Exists(Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir) + "\\excel\\eng\\blob\\" + fname))
					new_script = new Script(File.ReadAllBytes(Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir) + "\\excel\\eng\\blob\\" + fname));
				else
					throw new FileNotFoundException("Missing " + fname + " blob file");

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
						string en_text = reader.GetString(6);
						if (str_bid == null)
							continue;
						if (!string.IsNullOrEmpty(reader.GetString(5)))
							en_text = reader.GetString(5);

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

						LineBlock src_line_block = active_script.Blocks[src_index] as LineBlock;
						if (src_line_block != null)
						{
							if (string.IsNullOrEmpty(en_text))
								en_text = src_line_block.Text;
							else if (src_line_block.Text.StartsWith(" ") && !en_text.StartsWith(" "))
							{
								en_text = " " + en_text;
							}
						}

						line_block.Text = en_text;

						new_script.Blocks[src_index] = line_block;
					}
				} while (reader.NextResult());

				active_script = new_script;
				text_data = new_script.Compile(null);

				update_text();
				pck_file.ReplaceFile(fname, text_data);

				btnSave.Text = "Save *";
				btnSave.BackColor = Color.FromArgb(255, 200, 150);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to import " + fname + ":" + ex.Message);
			}
		}

		private void import_spreadsheet(string fname = "")
		{
			try
			{
				var dir = Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir) + "\\excel\\";
				if (string.IsNullOrEmpty(fname))
				{
					fname = cbFile.GetItemText(cbFile.SelectedItem);
				}
				using var stream = new FileStream(dir + fname + ".xlsx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				using var reader = ExcelReaderFactory.CreateReader(stream);

				Script new_script = new Script(File.ReadAllBytes(dir + "blob\\" + fname));

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

						if (jp_text != null && jp_text != "" && jp_text.Replace("<pause>", "").Replace("\\n", "") != line_block.Text.Replace("<pause>", "").Replace("\\n", ""))
						{
							throw new Exception("JP text mismatch");
						}

						if (string.IsNullOrEmpty(en_text))
							en_text = reader.GetString(6);

						if (!string.IsNullOrEmpty(en_text))
						{
							if (line_block.IsDialogue)
							{
								if (!en_text.StartsWith("\"") && !en_text.StartsWith("″") && jp_text != null && jp_text.StartsWith("「"))
								{
									en_text = "″" + en_text;
								}
								if (!en_text.EndsWith("\"") && !en_text.EndsWith("″") && jp_text != null && jp_text.EndsWith("」"))
								{
									en_text += "″";
								}
							}
							if (jp_text.EndsWith("<pause>") && !en_text.EndsWith("<nopause>"))
								en_text += "<pause>";
							line_block.Text = filter_string(en_text);
							if (jp_text.EndsWith("<pause>") && !en_text.EndsWith("<nopause>") && !line_block.Text.EndsWith("<pause>") && !en_text.EndsWith("<nopause>"))
							{
								line_block.Text += "<pause>";
							}
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
				pck_file.ReplaceFile(fname, text_data);

				btnSave.Text = "Save *";
				btnSave.BackColor = Color.FromArgb(255, 200, 150);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to import:" + ex.Message);
			}
		}

		private void ImportAll()
		{
			foreach (var filename in Directory.GetFiles(Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir) + "\\excel\\trimmed\\"))
			{
				if (filename.EndsWith(".xlsx"))
				{
					string fname = Path.GetFileNameWithoutExtension(filename);
					import_spreadsheet(fname);
				}
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
			var dir = Path.GetDirectoryName(Texttool.Properties.Settings.Default.last_dir) + "\\excel\\";
			string fname = cbFile.GetItemText(cbFile.SelectedItem);
			if (!File.Exists(dir + fname + ".xlsx"))
			{
				export_excel(true, false);
			}

			export_trimmed(true);
			import_spreadsheet();
			btnSave.Text = "Save *";
			btnSave.BackColor = Color.FromArgb(255, 200, 100);
		}

		private void button3_Click_1(object sender, EventArgs e)
		{
			export_trimmed(false);
		}

		private void textBox2_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				button1_Click(null, null);
				e.Handled = true;
			}
		}

		private void button7_Click(object sender, EventArgs e)
		{
			string s = textBox1.SelectedText;
			if (string.IsNullOrEmpty(s))
				return;

			string[] lines = s.Replace("\\r", "").Split('\n');

			s = "";
			s += "These are Japanese lines from the visual novel White Album. Translate them into English as accurately as possible. The narration is from Touya's point of view in present tense. Retain honorifics in the names.\n";
			string[] og_lines = textBox1.Text.Substring(0, textBox1.SelectionStart).Replace("\\r", "").Split('\n');
			for (int i = 0; i < lines.Length; i++)
			{
				s += (i + 1).ToString() + ". " + lines[i];
			}
			//s += "\n\n1.";
			s = s.Replace("<pause>", "");
			Clipboard.SetText(s);
			textBox1.Focus();
		}

		private void button8_Click(object sender, EventArgs e)
		{
			string s = Clipboard.GetText();
			if (string.IsNullOrEmpty(s))
				return;

			string[] lines = s.Split('\n');
			s = "";
			for (int i = 0; i < lines.Length; i++)
			{
				string l = lines[i];
				l = l.Replace((i + 1).ToString() + ") ", "");
				int dialogue = l.IndexOf(": \"");
				if (dialogue != -1)
				{
					l = l.Substring(dialogue + 3);
				}
				if (!l.StartsWith('\"') && l.EndsWith("\"\r"))
				{
					l = l.Substring(0, l.Length - 2) + "\r";
				}
				s += l + "\n";
			}

			Clipboard.SetText(s);
			textBox1.Focus();
		}

		private void button9_Click(object sender, EventArgs e)
		{
			string s = textBox1.SelectedText;
			if (string.IsNullOrEmpty(s))
				return;

			string[] og_lines = textBox1.Text.Substring(0, textBox1.SelectionStart).Replace("\\r", "").Split('\n');
			string[] lines = s.Replace("\\r", "").Split('\n');

			s = "";
			//s += "Translate these Japanese lines to English:\n";
			for (int i = 0; i < lines.Length; i++)
			{
				s += (i + og_lines.Length).ToString() + ". " + lines[i];
			}
			//s += "\n\n1.";
			s = s.Replace("<pause>", "");
			Clipboard.SetText(s);
			textBox1.Focus();
			/*string s = "Convert this from past to present tense:\n";
			s += Clipboard.GetText();
			s += "\n\n1)";
			Clipboard.SetText(s);
			textBox1.Focus();*/
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			string s = textBox1.SelectedText;
			if (string.IsNullOrEmpty(s))
				return;
			string[] lines = s.Replace("\\r", "").Split('\n');

			string numbers = "";
			for (int i = 0; i < lines.Length; i++)
			{
				numbers += (i + 1).ToString() + "." + "\r\n";
			}
			txtLines.Text = numbers;
		}

		private void button10_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Would you like to import all scripts from the trimmed folder?", "Import trimmed scripts?", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				ImportAll();
			}
		}

		private void button11_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var v in pck_file.FileEntries)
			{
				var ss = new Script(v.Data);
				bool appended = false;
				foreach (var block in ss.Blocks)
				{
					var line = block as LineBlock;
					if (line != null)
					{
						if (line.IsDialogue && line.JapaneseCharacterName != "")
						{
							sb.Append(line.JapaneseCharacterName + ": ");
						}

						var text = line.Text.Replace("<pause>", "").Replace("<nopause>", "").Replace("<break>", "").Replace("<ｋａｅｒｂ>", "").Replace("<ｂｒｅａｋ>", "").Replace("\\n", "");
						sb.Append(text);
						sb.AppendLine();

						appended = true;
					}
				}
				if (appended)
					sb.AppendLine("[[EndFile]]");
			}

			Clipboard.SetText(sb.ToString());
		}

		private void button12_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < cbFile.Items.Count; i++)
			{
				try
				{
					cbFile.SelectedIndex = i;

					export_excel(true, false);
				}
				catch (Exception ex)
				{
					Console.WriteLine(i.ToString() + ": " + ex.Message);
				}
			}

			cbFile.SelectedIndex = 100;
		}

		private void button13_Click(object sender, EventArgs e)
		{
			sum_removed = "";

			for (int i = 0; i < cbFile.Items.Count; i++)
			{
				try
				{
					cbFile.SelectedIndex = i;

					import_raw_spreadsheet("", "stripped\\");
				}
				catch (Exception ex)
				{
					Console.WriteLine(i.ToString() + ": " + ex.Message);
				}
			}

			cbFile.SelectedIndex = 100;
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void button14_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < cbFile.Items.Count; i++)
			{
				try
				{
					cbFile.SelectedIndex = i;

					export_excel(true, false);
				}
				catch (Exception ex)
				{
					Console.WriteLine(i.ToString() + ": " + ex.Message);
				}
			}

			cbFile.SelectedIndex = 100;
		}
	}
}
