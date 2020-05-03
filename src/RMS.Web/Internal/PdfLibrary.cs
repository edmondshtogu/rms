///The Base Library For Creating Pdf files.
///Author : Mohamed Shiraz T K
///email: shiraztk@rediffmail.com
///Start Date: 27th June 2004
using System;
using System.Text;
using System.Collections;

namespace PdfLibrary
{

	/// <summary>
	/// The Color to be used while drawing
	/// </summary>
	public struct ColorSpec
	{
		private double red1;
		private double green1;
		private double blue1;
		public string red;
		public string green;
		public string blue;
		public ColorSpec(uint R, uint G, uint B)
		{
			//Convert in the range 0.0 to 1.0
			red1 = R; green1 = G; blue1 = B;
			red1 = Math.Round((red1 / 255), 3);
			green1 = Math.Round((green1 / 255), 3);
			blue1 = Math.Round((blue1 / 255), 3);
			red = red1.ToString();
			green = green1.ToString();
			blue = blue1.ToString();
		}
	}

	/// <summary>
	/// Specify the page size in 1/72 inches units.
	/// </summary>
	public struct PageSize
	{
		public uint xWidth;
		public uint yHeight;
		public uint leftMargin;
		public uint rightMargin;
		public uint topMargin;
		public uint bottomMargin;

		public PageSize(uint width, uint height)
		{
			xWidth = width;
			yHeight = height;
			leftMargin = 0;
			rightMargin = 0;
			topMargin = 0;
			bottomMargin = 0;
		}
		public void SetMargins(uint L, uint T, uint R, uint B)
		{
			leftMargin = L;
			rightMargin = R;
			topMargin = T;
			bottomMargin = B;
		}
	}

	/// <summary>
	/// Used with the Text inside a Table
	/// </summary>
	public enum Align
	{
		LeftAlign, CenterAlign, RightAlign
	}

	/// <summary>
	/// Passed to Table class to create tables
	/// </summary>
	public struct TableParams
	{
		public uint xPos;
		public uint yPos;
		public uint numRow;
		public uint columnWidth;
		public uint numColumn;
		public uint rowHeight;
		public uint tableWidth;
		public uint tableHeight;
		public uint[] columnWidths;
		/// <summary>
		/// Call this for columns of variable widhts, specify the widhts
		/// </summary>
		/// <param name="numColumns"></param>
		/// <param name="widths"></param>
		public TableParams(uint numColumns, params uint[] widths)
		{
			xPos = yPos = numRow = columnWidth = rowHeight = tableHeight = 0;
			tableWidth = 0;
			numColumn = numColumns;
			columnWidths = new uint[numColumn];
			columnWidths = widths;
			SetTableWidth();
		}
		/// <summary>
		/// Call this for columns of equal widths, column width should be set
		/// </summary>
		/// <param name="numColumns"></param>
		public TableParams(uint numColumns)
		{
			xPos = yPos = numRow = columnWidth = rowHeight = tableHeight = 0;
			tableWidth = 0;
			numColumn = numColumns;
			columnWidths = null;
			columnWidth = 0;
		}
		private void SetTableWidth()
		{
			for (uint i = 0; i < numColumn; i++)
				tableWidth += columnWidths[i];
		}
	}
	/// <summary>
	/// Holds the Byte offsets of the objects used in the Pdf Document
	/// </summary>
	internal class XrefEnteries
	{
		internal static ArrayList offsetArray;

		internal XrefEnteries()
		{
			offsetArray = new ArrayList();
		}
	}
	/// <summary>
	/// For Adding the Object number and file offset
	/// </summary>
	internal class ObjectList : IComparable
	{
		internal long offset;
		internal uint objNum;

		internal ObjectList(uint objectNum, long fileOffset)
		{
			offset = fileOffset;
			objNum = objectNum;
		}
		#region IComparable Members

		public int CompareTo(object obj)
		{

			int result = 0;
			result = (this.objNum.CompareTo(((ObjectList)obj).objNum));
			return result;
		}

		#endregion
	}
	/// <summary>
	/// This is the base object for all objects used within the pdf.
	/// </summary>
	public class PdfObject
	{
		/// <summary>
		/// Stores the Object Number
		/// </summary>
		internal static uint inUseObjectNum;
		public uint objectNum;
		//private UTF8Encoding utf8;
		private XrefEnteries Xref;
		/// <summary>
		/// Constructor increments the object number to 
		/// reflect the currently used object number
		/// </summary>
		protected PdfObject()
		{
			if (inUseObjectNum == 0)
				Xref = new XrefEnteries();
			inUseObjectNum++;
			objectNum = inUseObjectNum;
		}
		~PdfObject()
		{
			objectNum = 0;
		}
		/// <summary>
		/// Convert the unicode string 16 bits to unicode bytes. 
		/// This is written to the file to create Pdf 
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		protected byte[] GetUTF8Bytes(string str, long filePos, out int size)
		{
			ObjectList objList = new ObjectList(objectNum, filePos);
			byte[] abuf;
			try
			{
				byte[] ubuf = Encoding.Unicode.GetBytes(str);
				Encoding enc = Encoding.GetEncoding(1252);
				abuf = Encoding.Convert(Encoding.Unicode, enc, ubuf);
				size = abuf.Length;
				XrefEnteries.offsetArray.Add(objList);
			}
			catch (Exception e)
			{
				string str1 = string.Format("{0},In PdfObjects.GetBytes()", objectNum);
				Exception error = new Exception(e.Message + str1);
				throw error;
			}
			return abuf;
		}

	}
	/// <summary>
	/// Models the Catalog dictionary within a pdf file. This is the first created object. 
	/// It contains referencesto all other objects within the List of Pdf Objects.
	/// </summary>
	public class CatalogDict : PdfObject
	{
		private string catalog;
		public CatalogDict()
		{

		}
		/// <summary>
		///Returns the Catalog Dictionary 
		/// </summary>
		/// <param name="refPageTree"></param>
		/// <returns></returns>
		public byte[] GetCatalogDict(uint refPageTree, long filePos, out int size)
		{
			Exception error = new Exception(" In CatalogDict.GetCatalogDict(), PageTree.objectNum Invalid");
			if (refPageTree < 1)
			{
				throw error;
			}
			catalog = string.Format("{0} 0 obj<</Type /Catalog/Lang(EN-US)/Pages {1} 0 R>>\rendobj\r",
				this.objectNum, refPageTree);
			return this.GetUTF8Bytes(catalog, filePos, out size);
		}

	}
	/// <summary>
	/// The PageTree object contains references to all the pages used within the Pdf.
	/// All individual pages are referenced through the Kids arraylist
	/// </summary>
	public class PageTreeDict : PdfObject
	{
		private string pageTree;
		private string kids;
		private static uint MaxPages;

		public PageTreeDict()
		{
			kids = "[ ";
			MaxPages = 0;
		}
		/// <summary>
		/// Add a page to the Page Tree. ObjNum is the object number of the page to be added.
		/// pageNum is the page number of the page.
		/// </summary>
		/// <param name="objNum"></param>
		/// <param name="pageNum"></param>
		public void AddPage(uint objNum)
		{
			Exception error = new Exception("In PageTreeDict.AddPage, PageDict.ObjectNum Invalid");
			if (objNum < 0 || objNum > PdfObject.inUseObjectNum)
				throw error;
			MaxPages++;
			string refPage = objNum + " 0 R ";
			kids = kids + refPage;
		}
		/// <summary>
		/// returns the Page Tree Dictionary
		/// </summary>
		/// <returns></returns>
		public byte[] GetPageTree(long filePos, out int size)
		{
			pageTree = string.Format("{0} 0 obj<</Count {1}/Kids {2}]>>\rendobj\r",
				this.objectNum, MaxPages, kids);
			return this.GetUTF8Bytes(pageTree, filePos, out size);
		}
	}
	/// <summary>
	/// This class represents individual pages within the pdf. 
	/// The contents of the page belong to this class
	/// </summary>
	public class PageDict : PdfObject
	{
		private string page;
		private string pageSize;
		private string fontRef;
		private string resourceDict, contents;
		public PageDict()
		{
			resourceDict = null;
			contents = null;
			pageSize = null;
			fontRef = null;
		}
		/// <summary>
		/// Create The Pdf page
		/// </summary>
		/// <param name="refParent"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void CreatePage(uint refParent, PageSize pSize)
		{
			Exception error = new Exception("In PageDict.CreatePage(),PageTree.ObjectNum Invalid");
			if (refParent < 1 || refParent > PdfObject.inUseObjectNum)
				throw error;

			pageSize = string.Format("[0 0 {0} {1}]", pSize.xWidth, pSize.yHeight);
			page = string.Format("{0} 0 obj<</Type /Page/Parent {1} 0 R/Rotate 0/MediaBox {2}/CropBox {2}",
				this.objectNum, refParent, pageSize);
		}
		/// <summary>
		/// Add Resource to the pdf page
		/// </summary>
		/// <param name="font"></param>
		public void AddResource(FontDict font, uint contentRef)
		{
			fontRef += string.Format("/{0} {1} 0 R", font.font, font.objectNum);
			if (contentRef > 0)
			{
				contents = string.Format("/Contents {0} 0 R", contentRef);
			}
		}
		/// <summary>
		/// Get the Page Dictionary to be written to the file
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public byte[] GetPageDict(long filePos, out int size)
		{
			resourceDict = string.Format("/Resources<</Font<<{0}>>/ProcSet[/PDF/Text]>>", fontRef);
			page += resourceDict + contents + ">>\rendobj\r";
			return this.GetUTF8Bytes(page, filePos, out size);
		}
	}
	/// <summary>
	///Represents the general content stream in a Pdf Page. 
	///This is used only by the PageObjec 
	/// </summary>
	public class ContentDict : PdfObject
	{
		private string contentDict;
		private string contentStream;
		public ContentDict()
		{
			contentDict = null;
			contentStream = "%stream\r";
		}
		/// <summary>
		/// Set the Stream of this Content Dict.
		/// Stream is taken from TextAndTable Objects
		/// </summary>
		/// <param name="stream"></param>
		public void SetStream(string stream)
		{
			contentStream += stream;
		}
		/// <summary>
		/// Enter the text inside the table just created.
		/// </summary>
		/// <summary>
		/// Get the Content Dictionary
		/// </summary>
		public byte[] GetContentDict(long filePos, out int size)
		{
			contentDict = string.Format("{0} 0 obj<</Length {1}>>stream\r\n{2}\nendstream\rendobj\r",
				this.objectNum, contentStream.Length, contentStream);

			return GetUTF8Bytes(contentDict, filePos, out size);

		}

	}
	/// <summary>
	///Represents the font dictionary used in a pdf page
	///Times-Roman		Helvetica				Courier
	///Times-Bold		Helvetica-Bold			Courier-Bold
	///Times-Italic		Helvetica-Oblique		Courier-Oblique
	///Times-BoldItalic Helvetica-BoldOblique	Courier-BoldOblique
	/// </summary>
	public class FontDict : PdfObject
	{
		private string fontDict;
		public string font;
		public FontDict()
		{
			font = null;
			fontDict = null;
		}
		/// <summary>
		/// Create the font Dictionary
		/// </summary>
		/// <param name="fontName"></param>
		public void CreateFontDict(string fontName, string fontType)
		{
			font = fontName;
			fontDict = string.Format("{0} 0 obj<</Type/Font/Name /{1}/BaseFont/{2}/Subtype/Type1/Encoding /WinAnsiEncoding>>\nendobj\n",
				this.objectNum, fontName, fontType);
		}
		/// <summary>
		/// Get the font Dictionary to be written to the file
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public byte[] GetFontDict(long filePos, out int size)
		{
			return this.GetUTF8Bytes(fontDict, filePos, out size);
		}

	}
	/// <summary>
	///Store information about the document,Title, Author, Company, 
	/// </summary>
	public class InfoDict : PdfObject
	{
		private string info;
		public InfoDict()
		{
			info = null;
		}
		/// <summary>
		/// Fill the Info Dict
		/// </summary>
		/// <param name="title"></param>
		/// <param name="author"></param>
		public void SetInfo(string title, string author, string company)
		{
			info = string.Format("{0} 0 obj<</ModDate({1})/CreationDate({1})/Title({2})/Creator(CRM FactFind)" +
				"/Author({3})/Producer(CRM FactFind)/Company({4})>>\rendobj\r",
				this.objectNum, GetDateTime(), title, author, company);

		}
		/// <summary>
		/// Get the Document Information Dictionary
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public byte[] GetInfoDict(long filePos, out int size)
		{
			return GetUTF8Bytes(info, filePos, out size);
		}
		/// <summary>
		/// Get Date as Adobe needs ie similar to ISO/IEC 8824 format
		/// </summary>
		/// <returns></returns>
		private string GetDateTime()
		{
			DateTime universalDate = DateTime.UtcNow;
			DateTime localDate = DateTime.Now;
			string pdfDate = string.Format("D:{0:yyyyMMddhhmmss}", localDate);
			TimeSpan diff = localDate.Subtract(universalDate);
			int uHour = diff.Hours;
			int uMinute = diff.Minutes;
			char sign = '+';
			if (uHour < 0)
				sign = '-';
			uHour = Math.Abs(uHour);
			pdfDate += string.Format("{0}{1}'{2}'", sign, uHour.ToString().PadLeft(2, '0'), uMinute.ToString().PadLeft(2, '0'));
			return pdfDate;
		}

	}
	/// <summary>
	/// This class contains general Utility for the creation of pdf
	/// Creates the Header
	/// Creates XrefTable
	/// Creates the Trailer
	/// </summary>
	public class Utility
	{
		private uint numTableEntries;
		private string table;
		private string infoDict;
		public Utility()
		{
			numTableEntries = 0;
			table = null;
			infoDict = null;
		}
		/// <summary>
		/// Creates the xref table using the byte offsets in the array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public byte[] CreateXrefTable(long fileOffset, out int size)
		{
			//Store the Offset of the Xref table for startxRef
			try
			{
				ObjectList objList = new ObjectList(0, fileOffset);
				XrefEnteries.offsetArray.Add(objList);
				XrefEnteries.offsetArray.Sort();
				numTableEntries = (uint)XrefEnteries.offsetArray.Count;
				table = string.Format("xref\r\n{0} {1}\r\n0000000000 65535 f\r\n", 0, numTableEntries);
				for (int entries = 1; entries < numTableEntries; entries++)
				{
					ObjectList obj = (ObjectList)XrefEnteries.offsetArray[entries];
					table += obj.offset.ToString().PadLeft(10, '0');
					table += " 00000 n\r\n";
				}
			}
			catch (Exception e)
			{
				Exception error = new Exception(e.Message + " In Utility.CreateXrefTable()");
				throw error;
			}
			return GetUTF8Bytes(table, out size);
		}
		/// <summary>
		/// Returns the Header
		/// </summary>
		/// <param name="version"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public byte[] GetHeader(string version, out int size)
		{
			string header = string.Format("%PDF-{0}\r%{1}\r\n", version, "\x82\x82");
			return GetUTF8Bytes(header, out size);
		}
		/// <summary>
		/// Creates the trailer and return the bytes array
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public byte[] GetTrailer(uint refRoot, uint refInfo, out int size)
		{
			string trailer = null;
			try
			{
				if (refInfo > 0)
				{
					infoDict = string.Format("/Info {0} 0 R", refInfo);
				}
				//The sorted array will be already sorted to contain the file offset at the zeroth position
				ObjectList objList = (ObjectList)XrefEnteries.offsetArray[0];
				trailer = string.Format("trailer\n<</Size {0}/Root {1} 0 R {2}/ID[<5181383ede94727bcb32ac27ded71c68>" +
					"<5181383ede94727bcb32ac27ded71c68>]>>\r\nstartxref\r\n{3}\r\n%%EOF\r\n"
					, numTableEntries, refRoot, infoDict, objList.offset);

				XrefEnteries.offsetArray = null;
				PdfObject.inUseObjectNum = 0;
			}
			catch (Exception e)
			{
				Exception error = new Exception(e.Message + " In Utility.GetTrailer()");
				throw error;
			}

			return GetUTF8Bytes(trailer, out size);
		}
		/// <summary>
		/// Converts the string to byte array in utf 8 encoding
		/// </summary>
		/// <param name="str"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		private byte[] GetUTF8Bytes(string str, out int size)
		{
			try
			{
				byte[] ubuf = Encoding.Unicode.GetBytes(str);
				Encoding enc = Encoding.GetEncoding(1252);
				byte[] abuf = Encoding.Convert(Encoding.Unicode, enc, ubuf);
				size = abuf.Length;
				return abuf;
			}
			catch (Exception e)
			{
				Exception error = new Exception(e.Message + " In Utility.GetUTF8Bytes()");
				throw error;
			}
		}
	}

	/// <summary>
	/// Draw a table in the pdf file
	/// </summary>
	public class TextAndTables
	{
		private uint fixedTop, lastY;
		private uint tableX;
		private PageSize pSize;
		private ArrayList rowY;
		private uint cPadding;
		private string errMsg;
		private uint numColumn, rowHeight, numRow;
		private uint[] colWidth;
		private uint textX, textY;
		private string tableStream;
		private uint tableWidth;
		private ColorSpec cColor;
		private string textStream;

		private uint[] TimesRomanWidth ={0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,250,333,408,500,
		500,833,778,333,333,333,500,564,250,333,250,278,500,500,500,500,500,500,500,500,500,500,278,278,564,564,564,444,
		921,722,667,667,722,611,556,722,722,333,389,722,611,889,722,722,556,722,667,556,611,722,722,944,722,722,611,333,
		278,333,469,500,333,444,500,444,500,444,333,500,500,278,278,500,278,778,500,500,500,500,333,389,278,500,500,722,
		500,500,444,480,200,480,541,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,333,500,500,167,500,500,
		500,500,180,444,500,333,333,556,556,500,500,500,250,453,350,333,444,444,500,100,100,444,333,333,333,333,333,333,
		333,333,333,333,333,333,333,100,889,276,611,722,889,310,667,278,278,500,722,500};

		public TextAndTables(PageSize pageSize)
		{
			fixedTop = lastY = 0;
			pSize = pageSize;
			tableX = 0;
			textStream = tableStream = errMsg = null;
			rowHeight = 0; numRow = 0;
			textX = 0; textY = 0;
		}
		/// <summary>
		/// Set the parameters of the table
		/// </summary>
		public bool SetParams(TableParams table, ColorSpec cellColor, Align alignment, uint cellPadding)
		{
			if ((table.yPos > (pSize.yHeight - pSize.topMargin)) ||
				(tableWidth > (pSize.xWidth - (pSize.leftMargin + pSize.rightMargin))))
				return false;
			tableWidth = table.tableWidth;
			switch (alignment)
			{
				case (Align.LeftAlign):
					tableX = pSize.leftMargin;
					break;
				case (Align.CenterAlign):
					tableX = (pSize.xWidth - (pSize.leftMargin + pSize.rightMargin)
						- tableWidth) / 2;
					break;
				case (Align.RightAlign):
					tableX = pSize.xWidth - (pSize.rightMargin + tableWidth);
					break;
			};
			textX = tableX;
			textY = table.yPos;
			fixedTop = table.yPos;
			rowHeight = table.rowHeight;
			numColumn = table.numColumn;
			cColor = cellColor;
			cPadding = cellPadding;
			colWidth = new uint[numColumn];
			colWidth = table.columnWidths;
			rowY = new ArrayList();
			return true;

		}
		/// <summary>
		/// Get the lines of text in the cells, when text wrap is true
		/// </summary>
		/// <param name="rowText"></param>
		/// <param name="fontSize"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		private string[][] GetLinesInCell(string[] rowText, uint fontSize)
		{
			string[][] text = new string[numColumn][];
			char[] s = { '\x0020' };
			for (int i = 0; i < rowText.Length; i++)
			{
				uint width = (colWidth[i] - 2 * cPadding) * 1000 / fontSize;
				string cellText = rowText[i];//.TrimStart(s);
											 //No entries are mandatory. So just break in case if any entry is not 
											 //available
				if (cellText == null)
				{
					break;
				}
				ArrayList lineText = new ArrayList();
				int index = 0;
				uint cWidth = 0;
				int words = 0;
				for (int chars = 0; chars <= cellText.Length; chars++)
				{
					char[] cArray = cellText.ToCharArray();
					do
					{
						cWidth += TimesRomanWidth[cArray[words]];
						words++;
					} while (cWidth < width && words < cArray.Length);

					if (words == cArray.Length)
					{
						string line = cellText.Substring(0, words);
						line = line.TrimEnd(s);
						lineText.Add(line);
						break;
					}
					else
					{
						words--;
						int space = cellText.LastIndexOf('\x0020', words, words + 1);
						if (space > 0)
						{
							string line = cellText.Substring(0, space + 1);
							//To remove the trailing space from the word
							line = line.TrimEnd(s);
							lineText.Add(line);
							index = space + 1;
							words = 0;
						}
						else
						{
							string line = cellText.Substring(0, words);
							lineText.Add(line);
							index = words;
							words = 0;
						}
					}
					cWidth = 0;
					chars = 0;
					cellText = cellText.Substring(index);
				}
				text[i] = new string[lineText.Count];
				//Copy the lines into the array to be returned
				for (int j = 0; j < lineText.Count; j++)
					text[i][j] = (string)lineText[j];

			}
			return text;
		}
		/// <summary>
		/// Get the length of the string in points
		/// </summary>
		/// <param name="text"></param>
		/// <param name="fontSize"></param>
		/// <returns></returns>
		private uint StrLen(string text, uint fontSize)
		{
			char[] cArray = text.ToCharArray();
			uint cWidth = 0;
			foreach (char c in cArray)
			{
				cWidth += TimesRomanWidth[c];
			}
			uint k = (cWidth * fontSize / 1000);
			return (cWidth * fontSize / 1000);
		}
		/// <summary>
		/// Add One row to the table
		/// </summary>
		/// <param name="textWrap"></param>
		/// <param name="fontSize"></param>
		/// <param name="fontName"></param>
		/// <param name="alignment"></param>
		/// <param name="rowText"></param>
		/// <returns></returns>
		public bool AddRow(bool textWrap, uint fontSize, string fontName, Align[] alignment, params string[] rowText)
		{
			if (rowText.Length > numColumn)
				return false;
			uint maxLines = 1;
			//The x cordinate of the aligned table
			uint startX = tableX;
			uint y = textY;
			uint x = 0;
			const uint yCellMargin = 4;
			//if(rowText.Length<numColumn || alignment.Length<numColumn)
			//	return false;
			//increment the number of rows as it is added
			numRow++;
			//Wrap the text if textWrap is true
			if (textWrap)
			{
				string[][] text = GetLinesInCell(rowText, fontSize);
				//Loop for the required number of columns
				for (int column = 0; column < rowText.Length; column++)
				{
					startX += colWidth[column];
					uint lines;
					//No entries are mandatory. So just break in case if any entry is not 
					//available
					if (text[column] == null)
						break;
					//Loop for the number of lines in a cell
					for (lines = 0; lines < (uint)text[column].Length; lines++)
					{
						y = (uint)(textY - ((lines + 1) * rowHeight)) + yCellMargin;
						try
						{
							switch (alignment[column])
							{
								case (Align.LeftAlign):
									x = startX - colWidth[column] + cPadding;
									break;
								case (Align.CenterAlign):
									x = startX - (colWidth[column] + StrLen(text[column][lines], fontSize)) / 2;
									break;
								case (Align.RightAlign):
									x = startX - StrLen(text[column][lines], fontSize) - cPadding;
									break;
							};
						}
						catch (Exception E)
						{
							errMsg = "String too long to fit in the Column" + E.Message;
							Exception e = new Exception(errMsg);
							throw e;
						}
						//( this is a escape character in adobe so remove it.
						text[column][lines] = text[column][lines].Replace("(", "\\(");
						text[column][lines] = text[column][lines].Replace(")", "\\)");
						tableStream += string.Format("\rBT/{0} {1} Tf \r{2} {3} Td \r({4}) Tj\rET",
							fontName, fontSize, x, y, text[column][lines]);
					}
					//Calculate the maximum number of lines in this row
					if (lines > maxLines)
						maxLines = lines;
				}
				//Change the Y cordinate to skip to next page
				if (textY < pSize.bottomMargin)
				{
					textY = 0;
					return false;
				}
				//Change Y cordinate to skip to next row
				else
				{
					textY = textY - rowHeight * maxLines;
					rowY.Add(textY);
					rowY.Add(rowHeight * maxLines);
				}
			}
			//If no text wrap is required
			else
			{
				for (int column = 0; column < rowText.Length; column++)
				{
					startX += colWidth[column];
					y = (uint)(textY - rowHeight) + yCellMargin;
					try
					{
						switch (alignment[column])
						{
							case (Align.LeftAlign):
								x = startX - colWidth[column] + cPadding;
								break;
							case (Align.CenterAlign):
								x = startX - (colWidth[column] + StrLen(rowText[column], fontSize)) / 2;
								break;
							case (Align.RightAlign):
								x = startX - StrLen(rowText[column], fontSize) - cPadding;
								break;
						};
					}
					catch (Exception E)
					{
						errMsg = "String too long to fit in the Column" + E.Message;
						Exception error = new Exception(errMsg);
						throw error;
					}
					tableStream += string.Format("\rBT/{0} {1} Tf \r{2} {3} Td \r({4}) Tj\rET",
						fontName, fontSize, x, y, rowText[column]);
				}
				if (textY < pSize.bottomMargin)
				{
					textY = 0;
					return false;
				}
				//Change Y cordinate to skip to next row
				else
				{
					textY = textY - rowHeight;
					rowY.Add(textY);
					rowY.Add(rowHeight);
				}
			}
			return true;
		}
		/// <summary>
		/// start the Page Text element at the X Y position
		/// </summary>
		/// <returns></returns>
		public void AddText(uint X, uint Y, string text, uint fontSize, string fontName, Align alignment)
		{
			Exception invalidPoints = new Exception("The X Y cordinate outof Page Boundary");
			if (X > pSize.xWidth || Y > pSize.yHeight)
				throw invalidPoints;
			uint startX = 0;
			switch (alignment)
			{
				case (Align.LeftAlign):
					startX = X;
					break;
				case (Align.CenterAlign):
					startX = X - (StrLen(text, fontSize)) / 2;
					break;
				case (Align.RightAlign):
					startX = X - StrLen(text, fontSize);
					break;
			};
			textStream += string.Format("\rBT/{0} {1} Tf\r{2} {3} Td \r({4}) Tj\rET\r",
				fontName, fontSize, startX, (pSize.yHeight - Y), text);
		}
		/// <summary>
		/// End the Text Element on a page
		/// </summary>
		/// <returns></returns>
		public string EndText()
		{
			Exception noTextStream = new Exception("No Text Element Created");
			if (textStream == null)
				throw noTextStream;
			string stream = textStream;
			textStream = null;
			return stream;
		}
		/// <summary>
		/// Call to end the Table Creation and Get the Stream Data
		/// </summary>
		/// <returns></returns>
		public string EndTable(ColorSpec lineColor)
		{
			string tableCode;
			string rect = null;
			uint x = tableX;
			uint yBottom = 0;
			//if required number of rows are added
			if (rowY.Count < numRow * 2)
				return null;
			//Draw the number of rows.
			//int rowHeight=Get
			for (int row = 0, yCor = 0; row < numRow; row++, yCor += 2)
			{
				rect += string.Format("{0} {1} {2} {3} re\r",
					x, rowY[yCor], tableWidth, rowY[yCor + 1]);
			}
			//To get the ycordinate of the last row in the table
			if (rowY.Count < 1)
				return null;
			yBottom = (uint)rowY[rowY.Count - 2];
			string line = null;
			//Draw lines to form the columns
			for (uint column = 0; column < numColumn; column++)
			{
				x += colWidth[column];
				line += string.Format("{0} {1} m\r{0} {2} l\r",
					x, fixedTop, yBottom);
			}
			//Create the code for the Table
			tableCode = string.Format("\rq\r{5} {6} {7} RG {2} {3} {4} rg\r{0}\r{1}B\rQ\r",
				line, rect, cColor.red, cColor.green, cColor.blue, lineColor.red, lineColor.green, lineColor.blue);

			lastY = yBottom;
			tableCode += tableStream;
			//Initailize the variables so that they can be used again
			tableStream = null;
			numRow = 0;
			rowY = null;
			return tableCode;
		}

		/// <summary>
		/// Get the Y cordinate for next Table to be appended
		/// </summary>
		/// <returns></returns>
		public uint GetY()
		{
			return lastY;
		}
	}

}
