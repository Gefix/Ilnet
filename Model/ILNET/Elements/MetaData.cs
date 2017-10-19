using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ILNET.Elements
{
    public class MetaData
    {
        private string m_name, m_value;

        public MetaData()
        {
            m_name = "";
            m_value = "";
        }

        public MetaData(string name, string value)
        {
            m_name = name;
            m_value = value;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        [XmlAttribute("value")]
        public string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }
    }

    public class MetaDataCollection
    {
        private Dictionary<string, string> m_meta;

        public MetaDataCollection()
        {
            m_meta = new Dictionary<string, string>();
        }

        public string this[string key]
        {
            get
            {
                if (!m_meta.ContainsKey(key)) return "";
                return m_meta[key];
            }
            set
            {
                m_meta[key] = value;
            }
        }

        public int Count { get { return m_meta.Count; } }
        public string[] Keys { get { return m_meta.Keys.ToArray(); } }
        public bool ContainsKey(string key) { return m_meta.ContainsKey(key); }

        public MetaData[] Data
        {
            get
            {
                MetaData[] items = new MetaData[m_meta.Count];
                int i = 0;
                foreach (string key in m_meta.Keys)
                {
                    items[i++] = new MetaData(key, m_meta[key]);
                }
                return items;
            }
            set
            {
                m_meta.Clear();
                if (value == null) return;
                foreach (MetaData meta in value)
                {
                    m_meta.Add(meta.Name, meta.Value);
                }
            }
        }

        public void Clear()
        {
            m_meta.Clear();
        }
    }
}
