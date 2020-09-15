using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System;

public class DiagramContainer : MonoBehaviour {

    public TextAsset ewdXML;
    public bool CableHere;

    public string[] tag1;
    public string[] tag2;
    public string[] cond;

    private int nLine;
    
    private void Start()
    {
        nLine = GameObject.FindGameObjectsWithTag("Line").Length;
        tag1 = new string[nLine];
        tag2 = new string[nLine];
        if(CableHere) cond = new string[nLine];
        string data = ewdXML.text;
        ParseXmlFile(data);
    }

    private void ParseXmlFile(string data)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(data));

        string xmlPathPattern = "//EWD/Line";
        XmlNodeList nodeList = xmlDoc.SelectNodes(xmlPathPattern);
        int i = 0;
        foreach (XmlNode node in nodeList)
        {
            tag1[i] = node.FirstChild.InnerXml;
            tag2[i] = node.FirstChild.NextSibling.InnerXml;
            if(CableHere) cond[i] = node.LastChild.InnerXml;
            i++;

            Debug.Log("tag ke-" + i + " sudah di-add dengan value " + tag1[i] + " dan " + tag2[i]);
            
        }
    }
}
