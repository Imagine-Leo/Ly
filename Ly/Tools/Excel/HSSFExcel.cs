using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ly.Excel
{
    public class HSSFExcel
    {
                /// <summary>
        /// 
        /// </summary>
        /// <param name="data">一个数组是一行数据</param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        public void CreateExcel(List<string[]> data, string name, string path)
        {
            //考虑用二位数组还是列表
            HSSFWorkbook workbook = new HSSFWorkbook();
            var table = workbook.CreateSheet(name);
            for (int i = 0; i < data.Count; i++)
            {
                var row = table.CreateRow(i);

                for (int j = 0; j < data[i].Length; j++)
                {
                    row.CreateCell(j).SetCellValue(data[i][j]);
                }
            }
            using (var fs = File.OpenWrite(path))
            {
                workbook.Write(fs);
                Console.WriteLine("写入完毕");
            }
        }
        public List<string[]> ReadExcel(string path)
        {
            List<string[]> result = new List<string[]>();
            var workbook = new HSSFWorkbook();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var sheet = workbook.GetSheetAt(i);
                for (int j = 0; j < sheet.LastRowNum; j++)
                {

                    var cell = sheet.GetRow(j);
                    string[] rowdata = new string[cell.LastCellNum];
                    for (int k = 0; k < cell.LastCellNum; k++
                        )
                    {
                        var d = cell.GetCell(k);
                        rowdata[k] = d.ToString();
                    }
                    result.Add(rowdata);
                }
            }
            return result;
        }
    }
}
