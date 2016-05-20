using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleGenerator
{
    class Program
    {
        static IEnumerable<string> Items(int count)
        {
            string phoneNumber = "0921859698";

            for (int i=0; i<count; i++)
            {
                yield return phoneNumber;
            }
        }

        static string GenerateFile(int count)
        {
            string outputDir = "";
            string filepath = Path.Combine(outputDir, string.Format("sameple_{0}.xlsx", count));
            FileInfo newFile = new FileInfo(filepath);
            if (newFile.Exists)
            {
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(filepath);
            }

            using (ExcelPackage package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("發送清單");

                int i = 1;
                foreach(var phoneNumber in Items(count))
                {
                    sheet.Cells[i++, 2].Value = phoneNumber;
                }
                

                package.SaveAs(newFile);
            }

            return filepath;
        }

        static void Main(string[] args)
        {
            string filepath = GenerateFile(1000 * 60);
        }
    }
}
