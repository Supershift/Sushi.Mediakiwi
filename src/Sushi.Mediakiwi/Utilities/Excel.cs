using System;
using System.Collections.Generic;
using System.Text;

namespace Wim.Utilities
{
    public class ExcelFileInfo
    {
        private string m_FileName; 
        public string FileName 
        { 
          get {return m_FileName;} 
          set {m_FileName = value;}
        }

        private string m_Workbook;
        public string Workbook
        {
            get { return m_Workbook; }
            set { m_Workbook = value; }
        }

        private int m_WorkbookNumber;
        public int WorkbookNumber
        {
            get { return m_WorkbookNumber; }
            set { m_WorkbookNumber = value; }
        }

        private List<String> m_Columns; 
        public List<String> Columns 
        { 
          get {return m_Columns;} 
          set {m_Columns = value;}
        }
    }
    public static class Excel
    {
        public delegate void ExcelLineReader(ExcelFileInfo fileInfo, List<string> row);

        public static void ReadExcelForeachLine(string location, bool firstLineHasColumns, ExcelLineReader rowReader)
        {
            if (string.IsNullOrEmpty(location)) throw new Exception("No location suplied");
            if (!System.IO.File.Exists(location)) throw new Exception("No file present at location suplied");

            Aspose.Cells.Workbook m_Book = new Aspose.Cells.Workbook();
            m_Book.LoadData(location);
            if (m_Book.Worksheets.Count == 0) throw new Exception("No workbooks supplied in file");

            for (int wbIndex = 0; wbIndex < m_Book.Worksheets.Count; wbIndex++)
            {
                ExcelFileInfo efi = new ExcelFileInfo();
                efi.FileName = location;
                efi.WorkbookNumber = wbIndex;
                efi.Workbook = m_Book.Worksheets[wbIndex].Name;
                efi.Columns = new List<string>();

                if (m_Book.Worksheets[wbIndex] != null && m_Book.Worksheets[wbIndex].Cells != null && m_Book.Worksheets[wbIndex].Cells.Rows != null)
                {
                    int start = 0;
                    int columnCount = m_Book.Worksheets[wbIndex].Cells.Columns.Count;
                    if (columnCount != 0)
                    {

                        if (firstLineHasColumns)
                        {
                            for (int c = 0; c < columnCount; c++)
                                efi.Columns.Add(m_Book.Worksheets[wbIndex].Cells[0, c].StringValue);
                            start = 1;
                        }                        
                        for (int index = start; index < m_Book.Worksheets[wbIndex].Cells.Rows.Count; index++)
                        {
                            List<String> row = new List<string>();
                            for (int c = 0; c < columnCount; c++)
                                row.Add(m_Book.Worksheets[wbIndex].Cells[index, c].StringValue);
                            rowReader.Invoke(efi, row);
                        }
                    }
                }
                else
                {
                    if (m_Book.Worksheets.Count == 0) throw new Exception("No valid workbooks/cells supplied in file");
                }
            }
        }
    }
}
