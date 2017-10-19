using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ILNET.Elements
{
    public class Element
    {
        protected string m_Name;
        public readonly MetaDataCollection Meta;

        public Element()
        {
            m_Name = "";
            Meta = new MetaDataCollection();
        }

        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        [XmlElement("meta")]
        public MetaData[] MetaData
        {
            get
            {
                return Meta.Data;
            }
            set
            {
                Meta.Data = value;
            }
        }
    }
}
