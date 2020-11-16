using System.Collections.Generic;
using System.Xml;

namespace Ly.Tools
{
    public class EasyXml
    {
        private string _configPath;
        private Dictionary<string, string> configDict;

        public void InitXML(string path)
        {
            _configPath = path;
            configDict = new Dictionary<string, string>();
            var xmlDoc = new XmlDocument();

            xmlDoc.Load(_configPath);
            XmlNode root = xmlDoc.DocumentElement;
            var nodeList = root.ChildNodes;
            for (var i = 0; i < nodeList.Count; i++)
            {
                var curNode = nodeList[i];
                var key = curNode.Name;
                var val = curNode.InnerText;
                if (!configDict.ContainsKey(key))
                    configDict.Add(key, val);
                else
                    Debug.Instance.DllLog("Duplicated key " + key + " in config file " + _configPath, LogType.UnityLogError);
            }
        }

        public string GetStringXML(string key, string defaultVal)
        {
            if (configDict.ContainsKey(key))
            {
                return configDict[key];
            }

            Debug.Instance.DllLog("配置表没有此配置返回默认值并写入默认值", LogType.UnityLogWarning);
            SetString(key, defaultVal);
            return defaultVal;
        }

        public void SetString(string key, string val)
        {
            if (configDict.ContainsKey(key))
                configDict[key] = val;
            else
                configDict.Add(key, val);
            UpdateXml(key, val);
        }

        public void RemoveNodeXML(string nodeName)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(_configPath);
            var list = xmlDoc.DocumentElement.ChildNodes;
            for (var i = 0; i < list.Count; i++)
            {
                var ee = (XmlElement) list[i];
                if (ee.Name == nodeName)
                {
                    xmlDoc.DocumentElement.RemoveChild(ee);
                    Debug.Instance.DllLog("成功移除配置表属性" + nodeName, LogType.UnityLog);
                    break;
                }
            }

            xmlDoc.Save(_configPath);
        }

        private void UpdateXml(string key, string val)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(_configPath);
            XmlNode root = xmlDoc.DocumentElement;
            var nodeList = root.ChildNodes;
            var found = false;
            for (var i = 0; i < nodeList.Count; i++)
            {
                var curNode = nodeList[i];
                if (curNode.Name == key)
                {
                    curNode.InnerText = val;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                var node = xmlDoc.CreateNode(XmlNodeType.Element, key, null);
                node.InnerText = val;
                root.AppendChild(node);
            }

            xmlDoc.Save(_configPath);
            UnityEngine.Debug.Log(xmlDoc.InnerXml);
        }
    }
}