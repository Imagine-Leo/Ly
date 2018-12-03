using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Debug = Ly.DebugTool.Debug;

namespace Ly.Xml
{
    public class EasyXml
    {
        private string _configPath;
        private Dictionary<string, string> configDict;

        public void InitXML(string path)
        {
            _configPath = path;
            configDict = new Dictionary<string, string>();
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(_configPath);
            XmlNode root = xmlDoc.DocumentElement;
            XmlNodeList nodeList = root.ChildNodes;
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode curNode = nodeList[i];
                string key = curNode.Name;
                string val = curNode.InnerText;
                if (!configDict.ContainsKey(key))
                {
                    configDict.Add(key, val);
                }
                else
                {
                    Debug.Instance.DllLog("Duplicated key " + key + " in config file " + _configPath, DebugTool.LogType.UnityLogError);
                }
            }
        }
        public string GetStringXML(string key, string defaultVal)
        {
            if (configDict.ContainsKey(key))
            {
                return configDict[key];
            }
            else
            {
                Debug.Instance.DllLog("配置表没有此配置返回默认值并写入默认值",DebugTool.LogType.UnityLogWarning);
                SetString(key, defaultVal);
                return defaultVal;
            }
        }
        public void SetString(string key, string val)
        {
            if (configDict.ContainsKey(key))
            {
                configDict[key] = val;
            }
            else
            {
                configDict.Add(key, val);
            }
            UpdateXml(key, val);
        }
        public void RemoveNodeXML(string nodeName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_configPath);
            XmlNodeList list = xmlDoc.DocumentElement.ChildNodes;
            for (int i = 0; i < list.Count; i++)
            {
                XmlElement ee = (XmlElement)list[i];
                if (ee.Name == nodeName)
                {
                    xmlDoc.DocumentElement.RemoveChild(ee);
                    Debug.Instance.DllLog("成功移除配置表属性" + nodeName,DebugTool.LogType.UnityLog);
                    break;
                }
            }

            xmlDoc.Save(_configPath);
        }
        private void UpdateXml(string key, string val)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_configPath);
            XmlNode root = xmlDoc.DocumentElement;
            XmlNodeList nodeList = root.ChildNodes;
            bool found = false;
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode curNode = nodeList[i];
                if (curNode.Name == key)
                {
                    curNode.InnerText = val;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, key, null);
                node.InnerText = val;
                root.AppendChild(node);
            }
            xmlDoc.Save(_configPath);
            UnityEngine.Debug.Log(xmlDoc.InnerXml);
        }
    }
}
