using ExcelDataReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = Ly.DebugTool.Debug;

namespace Ly.Excel
{
    [HelpURL("make sure its PC plateform now")]
    public class EasyExcel : MonoBehaviour
    {
        public Xml.MultilevelXml easyXml;
        public EnumExcelFormat EnumExcelFormat = EnumExcelFormat.xlsx;
        public string excelName = "";
        public char[] _columnName = "ABCDEFGHIGKLMNOPQRSTUVWXYZ".ToCharArray();

        public List<ExcelRowData> ReadExcelOnPc(string path, bool needreturn = false)
        {
            List<ExcelRowData> excelList = new List<ExcelRowData>();
            try
            {
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);
                DataSet result = excelReader.AsDataSet();
                int columns = result.Tables[0].Columns.Count;
                int rows = result.Tables[0].Rows.Count;
                Debug.Instance.DllLog("行数：" + rows, DebugTool.LogType.UnityLog);
                Debug.Instance.DllLog("列数：" + columns, DebugTool.LogType.UnityLog);
                for (int i = 0; i < rows; i++)
                {
                    ExcelRowData column_Value = new ExcelRowData();
                    for (int j = 0; j < columns; j++)
                    {
                        string value = result.Tables[0].Rows[i][j].ToString();
                        Debug.Instance.DllLog(string.Format("第{0}行，第{1}列：{2}", i, j, value));
                        column_Value.values.Add(value);
                    }
                    excelList.Add(column_Value);
                }
                stream.Close();
            }
            catch (Exception ex)
            {
                Debug.Instance.DllLog("\n===>:" + ex.Message, DebugTool.LogType.UnityLogError);
            }
            return excelList;
        }
        public IEnumerator LoadOnPhone(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Instance.DllLog("\n不存在" + path, DebugTool.LogType.UnityLogError);
                yield return 0;
            }
            string skipBom;
            WWW www = new WWW(path);
            skipBom = Encoding.UTF8.GetString(www.bytes, 0, www.bytes.Length);
            yield return www;
            string data = skipBom;
            Debug.Instance.DllLog("\ndata =" + data, DebugTool.LogType.UnityLog);
        }

        [ContextMenu("ExcelToXmlOnPC")]
        public void ExcelToXmlOnPc()
        {
            //#if UNITY_STANDALONE_WIN &&UNITY_EDITOR
            string excelPath = Application.dataPath + "/StreamingAssets/excel/" + excelName + "." + EnumExcelFormat.ToString();
            if (!File.Exists(excelPath))
            {
                Debug.Instance.DllLog(excelPath + "is not exist!", DebugTool.LogType.UnityLogError);
                return;
            }
            List<ExcelRowData> excelResult = ReadExcelOnPc(excelPath, true);
            easyXml.InitXml(Application.dataPath + "/StreamingAssets/config/" + excelName + ".xml", "root", "Leo", "");
            for (int i = 0; i < excelResult.Count; i++)
            {
                int rows = excelResult[i].values.Count;
                List<Xml.CCkeyValue> mykeyValues = new List<Xml.CCkeyValue>();
                for (int j = 0; j < rows; j++)
                {
                    mykeyValues.Add(new Xml.CCkeyValue(_columnName[j % _columnName.Length].ToString(), excelResult[i].values[j]));
                }
                //Debug.Instance.DllLog(  string.Format("写入xml第{0}行数据:{1}", i, excelResult[i].values[0]),DebugTool.LogType.UnityLog);
                if (!string.IsNullOrEmpty(excelResult[i].values[0]))
                    easyXml.AddNode("root", "A" + (i + 1).ToString(), mykeyValues, mykeyValues[0].value);
            }
            Debug.Instance.DllLog("  excel 转换 xml 成功:" + excelPath, DebugTool.LogType.UnityLog);
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
