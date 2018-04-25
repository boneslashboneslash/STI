using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Semestralka
{
    public class Save
    {
        /// <summary>
        /// Export list of objects to xlsx
        /// </summary>
        public static void ExportFilesToExcel(List<GitFile> listFiles, string pathToFile, string sheetName = "export_")//Soubor
        {
            try
            {
                if (listFiles != null)
                {
                    string name = sheetName + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss");
                    FileInfo newFile = new FileInfo(pathToFile + "\\" + name + ".xlsx");

                    ExcelPackage pck = new ExcelPackage(newFile);
                    //Add the Content sheet
                    int rowIndex = 1;
                    int columnIndex = 1;
                    List<string> list_headerArea = new List<string> { "name", "date time", "count lines" };
                    ExcelWorksheet ws;

                    ws = pck.Workbook.Worksheets.Add(name);
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
                        ws.Cells[rowIndex, columnIndex].Value = file.datetime;
                        columnIndex++;
                        ws.Cells[rowIndex, columnIndex].Value = file.countLines;
                        columnIndex++;

                        rowIndex++; columnIndex = 1;
                    }

                    pck.Save(); //save excel file 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void SaveDatagridContent(string fileName, IDictionary<string, List<string[]>> dict)
        {
            MainWindow win = (MainWindow)Application.Current.MainWindow;
            //List<GitFile> listGitFile = win.dataGrid.ItemsSource.Cast<GitFile>().ToList();
                
            string fileContent = String.Empty;
            string fileDestination = AppDomain.CurrentDomain.BaseDirectory;

            //foreach (var item in listGitFile)
            //{
            //    fileContent += item.name + "|" + item.datetime + "|" + item.countLines + "\r\n";
            //}

            foreach(var dictItem in dict)
            {
                foreach (var item in dictItem.Value)
                {
                    //file name|Date of commit|Number of file changes|Commit identificator
                    fileContent += item[4] + "|" + item[2] + "|" + item[0] + "|" + item[3] + "\r\n";
                }
            }
            // Create directory if doesn't exists
            FileInfo fi = new FileInfo(fileDestination);
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }
            // Write to file
            File.WriteAllText(fileDestination + fileName, fileContent);
        }

        public static List<string> LoadBackupContentFromFile(string fileName)
        {
            List<string> lines = new List<string>();
            string fileDestination = AppDomain.CurrentDomain.BaseDirectory;

            if(File.Exists(fileDestination + fileName))
            {
                lines = System.IO.File.ReadAllLines(fileDestination + fileName).ToList();
            }
            return lines;
        }
    }
}
