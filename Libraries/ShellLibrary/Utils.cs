using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ILNET.Common;

namespace ILNET
{
    public class Utils
    {
        static public void CloneHashDeep(Hashtable table)
        {
            Object[] keys = new Object[table.Count];
            table.Keys.CopyTo(keys, 0);
            foreach (Object o in keys)
            {
                if (table[o] is ICloneable)
                {
                    table[o] = ((ICloneable)table[o]).Clone();
                }
                if (table[o] is Hashtable) CloneHashDeep((Hashtable)table[o]);
            }
        }

        static public Hashtable Clone(Hashtable table)
        {
            Hashtable result = (Hashtable)table.Clone();

            CloneHashDeep(result);
            return result;
        }

        static public string Replace(string text, string id, string with)
        {
            StringBuilder res = new StringBuilder();

            Regex rs = new Regex("/\\*" + id + "\\*/", RegexOptions.Singleline | RegexOptions.Compiled);
            Regex re = new Regex("/\\*/" + id + "\\*/", RegexOptions.Singleline | RegexOptions.Compiled);

            int pos = 0;

            foreach (Match ms in rs.Matches(text))
            {
                int start = ms.Index;

                if (start - pos > 0) res.Append(text.Substring(pos, start - pos));

                Match me = re.Match(text, start);
                int end = me.Index + me.Length;

                res.Append(with);

                pos = end;
            }

            if (text.Length - pos > 0) res.Append(text.Substring(pos));

            return res.ToString();
        }

        static public string Extract(string text, string id)
        {
            Regex rs = new Regex("/\\*" + id + "\\*/", RegexOptions.Singleline | RegexOptions.Compiled);
            Regex re = new Regex("/\\*/" + id + "\\*/", RegexOptions.Singleline | RegexOptions.Compiled);

            foreach (Match ms in rs.Matches(text))
            {
                int start = ms.Index+ms.Length;

                Match me = re.Match(text, start);
                int end = me.Index;

                return text.Substring(start, end - start);
            }

            return "";
        }

        static public string Extract(string text, string id, int from)
        {
            Regex rs = new Regex("/\\*" + id + "\\*/", RegexOptions.Singleline | RegexOptions.Compiled);
            Regex re = new Regex("/\\*/" + id + "\\*/", RegexOptions.Singleline | RegexOptions.Compiled);

            foreach (Match ms in rs.Matches(text, from))
            {
                int start = ms.Index + ms.Length;

                Match me = re.Match(text, start);
                int end = me.Index;

                return text.Substring(start, end - start);
            }

            return "";
        }
    }

    public static class StringExtender
    {
        public static int NthIndexOf(this string target, string value, int n)
        {
            Match m = Regex.Match(target, "((" + value + ").*?){" + Math.Max(1, n) + "}");

            if (m.Success)
                return m.Groups[2].Captures[Math.Max(0, n - 1)].Index;
            else
                return -1;
        }
    }
}
