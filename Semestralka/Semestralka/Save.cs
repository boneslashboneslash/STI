using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semestralka
{
    public class Save
    {
        /// <summary>
        /// Export list of objects to xlsx
        /// </summary>
        public static void ExportFilesToExcel(List<Soubor> listFiles, string pathToFile, string sheetName = "export")
        {
            try
            {
                FileInfo newFile = new FileInfo(pathToFile);

                ExcelPackage pck = new ExcelPackage(newFile);
                //Add the Content sheet
                int rowIndex = 1;
                int columnIndex = 1;
                List<string> list_headerArea = new List<string> { "name", "date", "type" };
                ExcelWorksheet ws;

                ws = pck.Workbook.Worksheets.Add(sheetName);
                rowIndex = 1;
                columnIndex = 1;

                foreach (string area in list_headerArea)
                {
                    ws.Cells[rowIndex, columnIndex].Value = area;
                    columnIndex++;
                }
                rowIndex++; columnIndex = 1;

                foreach (var file in listFiles)
                {
                    ws.Cells[rowIndex, columnIndex].Value = file.name;
                    columnIndex++;
                    ws.Cells[rowIndex, columnIndex].Value = file.type;
                    columnIndex++;
                    ws.Cells[rowIndex, columnIndex].Value = file.date;
                    columnIndex++;

                    rowIndex++; columnIndex = 1;
                }

                pck.Save(); //save excel file
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
