using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Texttool
{
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

	public struct CharacterValue
	{
		public byte b0;
		public byte b1;
		public byte b2;
		public byte b3;
		public byte b4;
	}

	public class LineBlock : Block
	{
		public CharacterValue CharacterValue = new CharacterValue();
		public string CharacterName
		{
			get
			{
				if (CharacterValue.b2 == 1)
				{
					return Script.GetCharName(CharacterValue.b4 + 15);
				}
				else
				{
					string s = "";
					int v = CharacterValue.b3 * 0x100 + CharacterValue.b4;
					for (int i = 0; i < 16; i++)
					{
						if ((v & (1 << i)) != 0)
						{
							if (s != "")
								s += " & ";
							s += Script.GetCharName(i + 1);
						}
					}

					return s;
				}
			}
		}
		public string Text = "";

		public bool IsDialogue
		{
			get
			{
				return !(CharacterValue.b0 == 0 && CharacterValue.b1 == 0 && CharacterValue.b2 == 0 && CharacterValue.b3 == 0 && CharacterValue.b4 == 0);
			}
		}

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
			text = text.Replace("<pause>", "");
			text = text.Replace("<nopause>", "");
			// break kaerb
			if (text.IndexOf("<ｋａｅｒｂ>") != -1)
			{
				if (breakblock == null)
				{
					breakblock = new List<byte>();
					breakblock.Add(0);
					breakblock.Add(0);
					breakblock.Add(0x41);
					breakblock.Add(0);
					if (IsDialogue)
					{
						breakblock.Add(0x22);
						breakblock.Add(0x01);
						breakblock.Add(CharacterValue.b0);
						breakblock.Add(CharacterValue.b1);
						breakblock.Add(CharacterValue.b2);
						breakblock.Add(CharacterValue.b3);
						breakblock.Add(CharacterValue.b4);
					}
					breakblock.Add(0xF);
					breakblock.Add(0);
					breakblock.Add(0);
					breakblock.Add(0x2C);
					breakblock.Add(0x7F);
					breakblock.Add(0);
					breakblock.Add(0xFF);
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

		public static string[] char_names = new string[64];
		public static string[] char_short_names = new string[64];
		public static string[] jp_names = new string[32];
		public static string GetCharName(int v)
		{
			if (v < 0 || v >= char_names.Length || string.IsNullOrEmpty(char_names[v]))
				return v.ToString();
			return char_names[v];
		}
		public static string GetShortCharName(int v)
		{
			if (v < 0 || v >= char_short_names.Length || string.IsNullOrEmpty(char_short_names[v]))
				return v.ToString();
			return char_short_names[v];
		}

		static Script()
		{
			char_names[1] = "Touya";
			char_names[2] = "Yuki";
			char_names[3] = "Rina";
			char_names[4] = "Haruka";
			char_names[5] = "Misaki";
			char_names[6] = "Mana";
			char_names[7] = "Yayoi";
			char_names[8] = "Sayoko";
			char_names[9] = "Akira";
			char_names[10] = "Eiji";
			char_names[11] = "Nobuko";
			char_names[12] = "Izumi";
			char_names[13] = "Nagase";
			char_names[14] = "Other Female";
			char_names[15] = "???";
			char_names[16] = "AD Staff";
			char_names[17] = "Answering Machine";
			char_names[18] = "Floor Director";
			char_names[19] = "Father";
			char_names[20] = "Courier";
			char_names[21] = "Female Voice";
			char_names[22] = "Male Voice";
			char_names[23] = "Nanase";
			char_names[24] = "Mediator";
			char_names[25] = "Drunk A";
			char_names[26] = "Drunk B";
			char_names[27] = "Sawakura";
			char_names[28] = "Drama Club Member"; // Drama club member
			char_names[29] = "Kawashima";
			char_names[30] = "Girl";
			char_names[31] = "Girl A";
			char_names[32] = "Girl B";

			char_short_names[1] = "Touya";
			char_short_names[2] = "Yuki";
			char_short_names[3] = "Rina";
			char_short_names[4] = "Haruka";
			char_short_names[5] = "Misaki";
			char_short_names[6] = "Mana";
			char_short_names[7] = "Yayoi";
			char_short_names[8] = "Sayoko";
			char_short_names[9] = "Akira";
			char_short_names[10] = "Eiji";
			char_short_names[11] = "Nobuko";
			char_short_names[12] = "Izumi";
			char_short_names[13] = "Nagase";
			char_short_names[14] = "Female 2";
			char_short_names[15] = "???";
			char_short_names[16] = "AD Staff";
			char_short_names[17] = "Voicemail";
			char_short_names[18] = "FD";
			char_short_names[19] = "Father";
			char_short_names[20] = "Courier";
			char_short_names[21] = "Woman";
			char_short_names[22] = "Man";
			char_short_names[23] = "Nanase";
			char_short_names[24] = "Mediator";
			char_short_names[25] = "Drunk A";
			char_short_names[26] = "Drunk B";
			char_short_names[27] = "Sawakura";
			char_short_names[28] = "Drama Mbr"; // Drama club member
			char_short_names[29] = "Kawashima";
			char_short_names[30] = "Girl";
			char_short_names[31] = "Girl A";
			char_short_names[32] = "Girl B";

			jp_names[0] = "冬弥";
			jp_names[1] = "由綺";
			jp_names[2] = "理奈";
			jp_names[3] = "はるか";
			jp_names[4] = "美咲";
			jp_names[5] = "マナ";
			jp_names[6] = "弥生";
			jp_names[7] = "小夜子";
			jp_names[8] = "彰";
			jp_names[9] = "英二";
			jp_names[10] = "ノブコ";
			jp_names[11] = "イズミ";
			jp_names[12] = "長瀬";
			jp_names[13] = "その他女性";
			jp_names[14] = "？？？";
			jp_names[15] = "ＡＤスタッフ";
			jp_names[16] = "留守番電話";
			jp_names[17] = "フロアディレクター";
			jp_names[18] = "親父";
			jp_names[19] = "宅配業者";
			jp_names[20] = "女性の声";
			jp_names[21] = "男性の声";
			jp_names[22] = "七瀬";
			jp_names[23] = "斡旋業者";
			jp_names[24] = "酔っ払いＡ";
			jp_names[25] = "酔っ払いＢ";
			jp_names[26] = "澤倉";
			jp_names[27] = "演劇部員";
			jp_names[28] = "河島";
			jp_names[29] = "女の子";
			jp_names[30] = "女の子Ａ";
			jp_names[31] = "女の子Ｂ";

		}

		public byte[] Compile(byte[] compare)
		{
			List<byte> output = new List<byte>();
			List<byte> lo = new List<byte>();
			int off = 0;
			foreach (var b in Blocks)
			{
				lo.Clear();
				b.Write(lo);

				if (compare != null)
				{
					for (int i = 0; i < lo.Count; i++)
					{
						byte b1 = lo[i];
						byte b2 = compare[i + off];
						if (lo[i] != compare[i + off])
						{
							throw new Exception("Mismatch at " + i + off);
						}
					}
				}

				off += lo.Count;
				output.AddRange(lo);
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

			int off = 0;
			List<byte> bstring = new List<byte>();
			List<Block> blocks = new List<Block>();

			try
			{
				while (off < text_data.Length)
				{
					bstring.Clear();
					CharacterValue character_value = new CharacterValue();
					bool string_start = false;
					int counter_id = 0;
					bool in_option = false;

					byte opcode = text_data[off++];
					if (opcode == 0x22)
					{
						if (off == 0x2798)
						{

						}
						off++;
						if (text_data[off - 1] == 0)
						{
							// No character
						}
						else if (text_data[off - 1] == 1)
						{
							character_value.b0 = text_data[off++];
							character_value.b1 = text_data[off++];
							character_value.b2 = text_data[off++];
							character_value.b3 = text_data[off++];
							character_value.b4 = text_data[off++];
						}
						else
						{
							// Unknown...
							continue;
						}

						while (off < text_data.Length)
						{
							opcode = text_data[off++];
							if (opcode == 0xF)
							{
								off++;
								if (text_data[off] == 0)
								{
									off++;
									counter_id |= text_data[off++] << 8;
									counter_id |= text_data[off++] << 0;
								}
							}
							else if ((opcode >= 0x80 || (opcode == 0x3C && (text_data[off] == 0x63 || text_data[off] == 0x52 || text_data[off] == 0x57))) && (text_data[off - 2] == 0 || text_data[off - 2] == 0x22))
							{
								off--;
								string_start = true;
								break;
							}
						}
					}
					else if (opcode == 0xF)
					{
						off++;
						if (text_data[off] == 0)
						{
							off++;
							counter_id |= text_data[off++] << 8;
							counter_id |= text_data[off++] << 0;
						}

						while (off < text_data.Length)
						{
							opcode = text_data[off++];
							if ((opcode >= 0x80 || (opcode == 0x3C && (text_data[off] == 0x63 || text_data[off] == 0x52 || text_data[off] == 0x57))) && text_data[off - 2] == 0)
							{
								off--;
								string_start = true;
								break;
							}
							if (opcode == 0xFF && text_data[off] == 0xFF && text_data[off + 1] == 0xFF)
							{
								off += 2;
								string_start = true;
								in_option = true;
								break;
							}
						}
					}
					else if (opcode == 0xFF)
					{
						// Option
						if (text_data[off] == 0xFF && text_data[off + 1] == 0xFF)
						{
							off += 2;
							string_start = true;
							in_option = true;
						}
					}

					string s = "";
					if (!string_start)
						continue;

					int start = off;
					LineBlock last_line_block = null;
					for (; off + 4 < text_data.Length; off++)
					{
						byte b1 = text_data[off];
						byte b2 = text_data[off + 1];
						byte b3 = text_data[off + 2];
						byte b4 = text_data[off + 3];

						if (b1 == 0xFF && b2 == 0xFF && b3 == 0xFF)
							break;

						if (b1 == 0x5C && b2 == 0x6B)
						{
							if (off == 0x16A4)
							{

							}
							if (bstring.Count > 0)
							{
								LineBlock sub_block = new LineBlock(encoding, start);
								sub_block.CharacterValue = character_value;
								sub_block.Text = encoding.GetString(bstring.ToArray()) + "<pause>";
								last_line_block = sub_block;
								blocks.Add(sub_block);
							}

							bstring.Clear();

							off += 11;

							continue;
						}
						else if (b1 == '<')
						{
							if (b2 == 'R')
							{
								// R is used to add Furigana to kanji and works like <R把手|とって>
								// Leave it in
							}
							else if (b2 == 'c')
							{
								// c is used for color, like c4
								// Leave it in
							}
							else
							{
								if (bstring.Count > 0)
								{
									LineBlock sub_block = new LineBlock(encoding, start);
									sub_block.CharacterValue = character_value;
									sub_block.Text = encoding.GetString(bstring.ToArray());
									blocks.Add(sub_block);
								}

								else if (b2 == 'W')
									off += 3;
								else if (bstring.Count > 0)
								{

									MessageBox.Show("Unknown < escape code: " + (char)b2);
									throw new Exception("Unknown < escape code: " + (char)b2);
								}

								bstring.Clear();

								continue;
							}
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

								if (last_line_block != null && last_line_block.Text.EndsWith("<pause>"))
								{
									last_line_block.Text = last_line_block.Text.Substring(0, last_line_block.Text.Length - 7);
								}

								off++;

								continue;
							}
						}
						if (in_option && b1 == 0)
						{
							break;
						}
						if (b1 == 0 && b3 != 0x41 && b3 != 0x42 && b3 != 0x16 && b3 != 0x25)
						{
							bstring.Clear();
							off += 2;
							continue;
						}
						if (b1 == 0 && (b3 == 0x41 || b3 == 0x42 || b3 == 0x16 || b3 == 0x25))
						{
							break;
						}


						if (bstring.Count == 0)
							start = off;
						bstring.Add(b1);
					}

					if (bstring.Count <= 1)
						continue;

					LineBlock l = new LineBlock(encoding, start);
					l.CharacterValue = character_value;
					l.Text = encoding.GetString(bstring.ToArray());
					last_line_block = l;
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
					off += ((LineBlock)blocks[next_block]).encoding.GetBytes(((LineBlock)blocks[next_block]).Text.Replace("<pause>", "")).Length;
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
