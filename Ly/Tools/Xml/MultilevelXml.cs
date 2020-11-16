using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Ly.Tools
{
    public class MultilevelXml
    {
        private string _rootName;
        public string filePath;
        private XmlDocument myxml;
        private XmlNode root;

        /// <summary>
        ///     <param name="Path"></param>
        ///     <param name="rootName">根节点命名</param>
        ///     <param name="autor"></param>
        ///     <param name="loadStr">是否加载外部json文本方式</param>
        /// </summary>
        public void InitXml(string Path = "", string rootName = "root", string autor = "Leo", string loadStr = "")
        {
            if (string.IsNullOrEmpty(loadStr))
            {
                if (string.IsNullOrEmpty(Path))
                {
                    Debug.Instance.DllLog("Path is IsNullOrEmpty!", LogType.UnityLogWarning);
                    return;
                }

                filePath = Path;
                myxml = new XmlDocument();
                _rootName = rootName;
                if (!File.Exists(filePath))
                {
                    var xmlSM = myxml.CreateXmlDeclaration("1.0", "UTF-8", null);
                    //追加xmldecl位置
                    myxml.AppendChild(xmlSM);
                    //创建root节点和root属性
                    var root = myxml.CreateElement("root");
                    root.SetAttribute("autor", autor);
                    root.SetAttribute("date", DateTime.Now.ToString());
                    myxml.AppendChild(root);
                    myxml.Save(filePath);
                }

                myxml.Load(filePath);
            }
            else
            {
                myxml = new XmlDocument();
                myxml.LoadXml(loadStr);
            }

            root = myxml.DocumentElement;
            Debug.Instance.DllLog("initxml初始化成功", LogType.UnityLog);
        }

        /// <summary>
        ///     <param name="index">在xml表单第几个层级添加此数据,大于1</param>
        ///     <param name="nodeName"></param>
        ///     <param name="keyValueList">属性表示键值列表</param>
        ///     <param name="text">intex</param>
        /// </summary>
        public void AddNode(string parentName, string nodeName, List<CCkeyValue> keyValueList, string text)
        {
            var temp = myxml.CreateElement(nodeName);
            for (var i = 0; i < keyValueList.Count; i++)
            {
                var att = myxml.CreateAttribute(keyValueList[i].key);
                att.Value = keyValueList[i].value;
                temp.Attributes.Append(att);
            }

            temp.InnerText = text;

            var parentNode = myxml.SelectSingleNode(parentName == root.Name ? parentName : root.Name + "//" + parentName);
            if (parentNode == null)
            {
                Debug.Instance.DllLog("Add 不存在此父节点" + parentName, LogType.UnityLogWarning);
                return;
            }

            parentNode.AppendChild(temp);
            myxml.Save(filePath);
            Debug.Instance.DllLog("成功添加数据信息：" + nodeName, LogType.UnityLog);
        }

        public void ReMoveNode(string parentName, string nodeName)
        {
            var parentNode = myxml.SelectSingleNode(parentName);
            if (parentNode == null)
            {
                Debug.Instance.DllLog("不存在此父节点", LogType.UnityLogWarning);
                return;
            }

            var childList = parentNode.ChildNodes;
            for (var i = 0; i < childList.Count; i++)
                if (childList[i].Name == nodeName)
                {
                    parentNode.RemoveChild(childList[i]);
                    Debug.Instance.DllLog("成功移除：" + nodeName, LogType.UnityLog);
                }

            myxml.Save(filePath);
        }

        public void UpdateNode(string parentName, string nodeName, List<CCkeyValue> keyValueList, string tex)
        {
            myxml.Load(filePath);
            var parentNode = myxml.SelectSingleNode(parentName == root.Name ? parentName : root.Name + "//" + parentName);
            if (parentNode == null)
            {
                Debug.Instance.DllLog("Update 不存在此父节点:" + parentName, LogType.UnityLogWarning);
                return;
            }

            var temp = myxml.CreateElement(nodeName);
            for (var i = 0; i < keyValueList.Count; i++)
            {
                var att = myxml.CreateAttribute(keyValueList[i].key);
                att.Value = keyValueList[i].value;
                temp.Attributes.Append(att);
            }

            temp.InnerText = tex;
            var isexsit = false;
            for (var i = 0; i < parentNode.ChildNodes.Count; i++)
                if (parentNode.ChildNodes[i].Name == nodeName)
                {
                    isexsit = true;
                    parentNode.ReplaceChild(temp, parentNode.ChildNodes[i]);
                    Debug.Instance.DllLog("成功修改数据:" + nodeName, LogType.UnityLog);
                    break;
                }

            if (!isexsit) parentNode.AppendChild(temp);
            myxml.Save(filePath);
        }

        public List<CCkeyValue> GetAllVaule(string nodeName)
        {
            if (myxml == null)
                return null;
            if (string.IsNullOrEmpty(nodeName))
                nodeName = root.Name;
            var parentNode = myxml.SelectSingleNode(nodeName == root.Name ? nodeName : root.Name + "//" + nodeName);
            if (parentNode == null)
            {
                Debug.Instance.DllLog("不存在此父节点", LogType.UnityLogWarning);
                return null;
            }

            var valueList = new List<CCkeyValue>();
            for (var i = 0; i < parentNode.ChildNodes.Count; i++)
                //for (int j = 0; j < parentNode.ChildNodes[i].Attributes.Count; j++)
            for (var j = 0; j < 1; j++)
                valueList.Add(new CCkeyValue(parentNode.ChildNodes[i].Attributes[j].Name, parentNode.ChildNodes[i].Attributes[j].Value));
            //valueList.Add(new mykeyValue("InnerText", parentNode.ChildNodes[i].InnerText));
            //TODO:需不需要遍历子级的子级
            //string consol = "";
            //for (int i = 0; i < valueList.Count; i++)
            //{
            //    consol += (" " + valueList[i].key + ":" + valueList[i].value + "; ");
            //}
            return valueList;
        }

        public string GetSingleVaule(string parentName, string nodeName, string attr, string defaultvalue, bool tex = false)
        {
            var parentNode = myxml.SelectSingleNode(parentName == root.Name ? parentName : root.Name + "//" + parentName);
            if (parentNode == null)
            {
                Debug.Instance.DllLog("没有此父节点", LogType.UnityLogWarning);
                return defaultvalue;
            }

            var str = defaultvalue;
            for (var i = 0; i < parentNode.ChildNodes.Count; i++)
                if (parentNode.ChildNodes[i].Name == nodeName)
                {
                    if (tex)
                        str = parentNode.ChildNodes[i].InnerText;
                    else
                        for (var j = 0; j < parentNode.ChildNodes[i].Attributes.Count; j++)
                            if (parentNode.ChildNodes[i].Attributes[j].Name == attr)
                            {
                                str = parentNode.ChildNodes[i].Attributes[j].Value;
                                break;
                            }

                    break;
                }

            return str;
        }

        public void TestXml()
        {
            InitXml();
            for (var i = 0; i < 20; i++) AddNode(_rootName, "name" + (i + 1), new List<CCkeyValue> {new CCkeyValue("key1", "v1"), new CCkeyValue("key2", "v2")}, (i + 1).ToString());
        }
    }

    public class CCkeyValue
    {
        public string key;
        public string value;

        public CCkeyValue(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
}