using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Speech.Synthesis;
using Microsoft.Speech.Recognition.SrgsGrammar;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Parser;

namespace GUI
{
    public class Parser
    {
        private XmlDocument vxmlDocument;

        public Parser(XmlDocument vxmlDocument)
        {
            this.vxmlDocument = vxmlDocument;
        }

        public String ParsePrompt()
        {
            XmlNodeList elemList = vxmlDocument.GetElementsByTagName("prompt");
            String prompt = elemList[0].InnerText;
            return prompt;
        }

        public String ParseNomatch()
        {
            XmlNodeList elemList = vxmlDocument.GetElementsByTagName("nomatch");
            String noMatch = elemList[0].InnerText;
            return noMatch;
        }

        public String GetFieldName()
        {
            XmlNodeList elemList = vxmlDocument.GetElementsByTagName("field");
            XmlElement element = elemList[0] as XmlElement;
            String fieldName = element.GetAttribute("name");
            return fieldName;
        }

        public String GetGrammarPath()
        {
            XmlNodeList elemList = vxmlDocument.GetElementsByTagName("grammar");
            XmlElement element = elemList[0] as XmlElement;
            String grammarPath = "";

            try
            {
                grammarPath = element.GetAttribute("src");
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return grammarPath;
        }



        public String GetNext(String answer)
        {
            try
            {
                String ifValue = GetIfValue(answer);
                if (ifValue == "")
                {
                    String elseIfValue = getElseIfValue(answer);

                    if (elseIfValue == "")
                    {
                        return "";

                    }
                    else
                    {
                        return getElseIfValue(answer);
                    }
                }
                else
                {
                    return ifValue;
                }
               
            }
            catch(ParseXmlException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return "";
            }         
        }

        

        private String GetIfValue(String answer)
        {
            XmlElement firstIf = GetFirstElementFromList("if");
            String condition = ParseConditionFromIf(GetConditionIf(firstIf));
            XmlElement firstGoto = GetFirstElementFromList("goto");
            String gotoNext = firstGoto.GetAttribute("next");
            if(condition == answer || answer == "CzasRezerwacji")
            {
                return gotoNext;
            }
            else
            {
                return "";
            }
        }

        private String getElseIfValue(String answer)
        {
            XmlNodeList elseIfList = GetXmlNodeList("elseif");
            for (int i = 0; i < elseIfList.Count; i++)
            {
                XmlElement elseifElement = elseIfList[i] as XmlElement;
                String condition = ParseConditionFromIf(GetConditionIf(elseifElement));
                XmlNodeList gotoList = GetXmlNodeList("goto");
                XmlElement firstGoto = gotoList[i + 1] as XmlElement;
                String gotoNext = firstGoto.GetAttribute("next");
                if (condition == answer)
                {
                    return gotoNext;
                }
            }
            return "";
        }

        private XmlNodeList GetXmlNodeList(String listName)
        {
            XmlNodeList list = vxmlDocument.GetElementsByTagName(listName);
            if (list == null || list[0] == null)
            {
                throw new ParseXmlException();
            }
            return list;
        }

        private String GetConditionIf(XmlElement firstIf)
        {
            return firstIf.GetAttribute("cond"); //Start == 'Dokonac rezerwacji'
        }

        private XmlElement GetFirstElementFromList(String listName)
        {
            XmlNodeList list = vxmlDocument.GetElementsByTagName(listName);
            if(list == null || list[0] == null)
            {
                throw new ParseXmlException();
            }
        
            return list[0] as XmlElement;
        }

        private String ParseConditionFromIf(String warunek)
        {

            var reg = new Regex("\'.*?\'");
            String zwrot = "";
            var matches = reg.Matches(warunek);

            foreach (var item in matches)
            {
                zwrot = item.ToString().Replace("\'", "");

            }
            return zwrot;

        }

    }
}
