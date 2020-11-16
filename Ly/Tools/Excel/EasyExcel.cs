using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEngine;

namespace Ly.Tools
{
    [HelpURL("make sure its PC plateform now")]
    public class EasyExcel : MonoBehaviour
    {
        public EnumExcelFormat EnumExcelFormat = EnumExcelFormat.xlsx;
        public string excelName = "";
        public char[] _columnName = "ABCDEFGHIGKLMNOPQRSTUVWXYZ".ToCharArray();
        public MultilevelXml easyXml;

        public List<ExcelRowData> ReadExcelOnPc(string path, bool needreturn = false)
        {
            var excelList = new List<ExcelRowData>();
            try
            {
                var stream = File.Open(path, FileMode.Open, FileAccess.Read);
                var excelReader = ExcelReaderFactory.CreateReader(stream);
                var result = excelReader.AsDataSet();
                var columns = result.Tables[0].Columns.Count;
                var rows = result.Tables[0].Rows.Count;
                Debug.Instance.DllLog("行数：" + rows, LogType.UnityLog);
                Debug.Instance.DllLog("列数：" + columns, LogType.UnityLog);
                for (var i = 0; i < rows; i++)
                {
                    var column_Value = new ExcelRowData();
                    for (var j = 0; j < columns; j++)
                    {
                        var value = result.Tables[0].Rows[i][j].ToString();
                        Debug.Instance.DllLog(string.Format("第{0}行，第{1}列：{2}", i, j, value));
                        column_Value.values.Add(value);
                    }

                    excelList.Add(column_Value);
                }

                stream.Close();
            }
            catch (Exception ex)
            {
                Debug.Instance.DllLog("\n===>:" + ex.Message, LogType.UnityLogError);
            }

            return excelList;
        }

        public IEnumerator LoadOnPhone(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Instance.DllLog("\n不存在" + path, LogType.UnityLogError);
                yield return 0;
            }

            string skipBom;
            var www = new WWW(path);
            skipBom = Encoding.UTF8.GetString(www.bytes, 0, www.bytes.Length);
            yield return www;
            var data = skipBom;
            Debug.Instance.DllLog("\ndata =" + data, LogType.UnityLog);
        }

        [ContextMenu("ExcelToXmlOnPC")]
        public void ExcelToXmlOnPc()
        {
            //#if UNITY_STANDALONE_WIN &&UNITY_EDITOR
            var excelPath = Application.dataPath + "/StreamingAssets/excel/" + excelName + "." + EnumExcelFormat;
            if (!File.Exists(excelPath))
            {
                Debug.Instance.DllLog(excelPath + "is not exist!", LogType.UnityLogError);
                return;
            }

            var excelResult = ReadExcelOnPc(excelPath, true);
            easyXml.InitXml(Application.dataPath + "/StreamingAssets/config/" + excelName + ".xml");
            for (var i = 0; i < excelResult.Count; i++)
            {
                var rows = excelResult[i].values.Count;
                var mykeyValues = new List<CCkeyValue>();
                for (var j = 0; j < rows; j++) mykeyValues.Add(new CCkeyValue(_columnName[j % _columnName.Length].ToString(), excelResult[i].values[j]));
                //Debug.Instance.DllLog(  string.Format("写入xml第{0}行数据:{1}", i, excelResult[i].values[0]),DebugTool.LogType.UnityLog);
                if (!string.IsNullOrEmpty(excelResult[i].values[0]))
                    easyXml.AddNode("root", "A" + (i + 1), mykeyValues, mykeyValues[0].value);
            }

            Debug.Instance.DllLog("  excel 转换 xml 成功:" + excelPath, LogType.UnityLog);
            //#endif
        }
    }

    public class ExcelRowData
    {
        public List<string> values;

        public ExcelRowData()
        {
            values = new List<string>();
        }
    }


    public enum EnumExcelFormat
    {
        xlsm = 1,
        xlsx = 2
    }
}