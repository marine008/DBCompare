using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Marine.Database
{
    public class XmlOperation
    {
        private XmlDocument _xDocument = null;
        public XmlDocument XDocuement
        {
            get
            {
                if (_xDocument == null)
                    throw new Exception("XML文档为空值!");
                return _xDocument;
            }
        }

        private XmlNode _rootNode;
        public XmlNode RootNode
        {
            set { _rootNode = value; }
            get { return _rootNode; }
        }

        public XmlOperation()
        {
            _xDocument = new XmlDocument();
        }

        public void Load(string fileName)
        {
            try
            {
                _xDocument.Load(fileName);
            }
            catch (Exception err)
            {
                System.Diagnostics.Trace.WriteLine(err);
            }
        }

        public XmlNodeList GetNodes(string nodeName)
        {
           XmlNodeList nodeList =  _xDocument.GetElementsByTagName(nodeName);
           return nodeList;
        }

        public Dictionary<string, string> GetTagElementInnerValues(XmlNode node)
        {
            Dictionary<string, string> innderValues = new Dictionary<string, string>();
            foreach (XmlNode child in node.ChildNodes)
            {
                if (!innderValues.ContainsKey(child.Name))
                {
                    innderValues.Add(child.Name, child.InnerText);
                }
            }
            return innderValues;
        }

        public void Create()
        {
            XmlDeclaration xDeclaration = XDocuement.CreateXmlDeclaration("1.0", "utf-8", "");
            XDocuement.InsertBefore(xDeclaration, XDocuement.DocumentElement);
        }

        public void Create(string rootName)
        {
            Create();
            _rootNode = XDocuement.AppendChild(CreateXmlNode(rootName, "", null));
        }

        public XmlNode CreateXmlNode(string nodeName, string nodeText, Dictionary<string, string> nodeAttribute)
        {
            XmlNode xNode = XDocuement.CreateNode(XmlNodeType.Element, nodeName, "");
            if (!string.IsNullOrEmpty(nodeText))
                xNode.InnerText = nodeText;
            if (nodeAttribute != null)
                foreach (string attName in nodeAttribute.Keys)
                {
                    XmlAttribute xAttribute = XDocuement.CreateAttribute(attName);
                    xAttribute.Value = nodeAttribute[attName];

                    xNode.Attributes.Append(xAttribute);
                }
            return xNode;
        }

        public bool Save(string fileName)
        {
            bool isSave = false;
            try
            {
                _xDocument.Save(fileName);
                isSave = true;
            }
            catch (Exception err)
            {
                System.Diagnostics.Trace.WriteLine(err);
            }
            return isSave;
        }

        public static void GetXmlNode(string xmlFileName, string xpathValue)
        {
            XmlDocument xDocument = new XmlDocument();

            try
            { xDocument.Load(xmlFileName); }
            catch (Exception err)
            { System.Diagnostics.Trace.WriteLine(err); }

            XmlNodeList xNodeList = xDocument.SelectNodes(xpathValue);
        }

        public static XmlDocument CreateXmlDocument()
        {
            XmlDocument xDocument = new XmlDocument();
            XmlDeclaration xDeclaration = xDocument.CreateXmlDeclaration("1.0", "utf-8", "");
            xDocument.InsertBefore(xDeclaration, xDocument.DocumentElement);
            return xDocument;
        }

        public static XmlNode CreateXmlDocument(string rootName)
        {
            XmlDocument xDocument = CreateXmlDocument();
            XmlNode xNode = xDocument.CreateElement(rootName);
            XmlNode rootNode = xDocument.AppendChild(xNode);
            return rootNode;
        }

        public static XmlNode ReturnXmlNode(XmlDocument xDoc, string nodeName, string nodeText, Dictionary<string, string> nodeAttribute)
        {
            XmlNode xNode = xDoc.CreateElement(nodeName);
            xNode.Value = nodeText;
            if (nodeAttribute != null)
                foreach (string attName in nodeAttribute.Keys)
                {
                    XmlAttribute xAttribute = xDoc.CreateAttribute(attName);
                    xAttribute.Value = nodeAttribute[attName];
                }
            return xNode;
        }
    }
}