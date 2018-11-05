using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
namespace WebSupport
{
    public static class ParseSupport
    {
        private static Regex url_reg = new Regex(@"([0-9a-zA-Z_]+)=([0-9a-zA-Z_]+)", RegexOptions.Compiled);
        private static Regex reg_CyberCampus = new Regex(@"([가-힣0-9a-zA-z\- ]+) \(([0-9]+)_([0-9]+)\)",RegexOptions.Compiled);
        public static JObject UrlQueryParser(string url)
        {
            MatchCollection gas = url_reg.Matches(url);
            JObject result = new JObject();
            foreach (Match match in gas)
            {
                result[match.Groups[1].Value] = match.Groups[2].Value;
            }
            return result;
        }
        public static JObject UrlQueryParser(HtmlNode node)
        {
            return UrlQueryParser(node.Attributes["href"].Value);
        }
        public static JObject CyberCampusTitle(string data)
        {
            Match gasaa = reg_CyberCampus.Match(data);
            JObject json = new JObject();
            json["title"] = gasaa.Groups[1].Value;
            json["key"] = 2018 + gasaa.Groups[2].Value + gasaa.Groups[3].Value;
            return json;
        }
        public static string StringFromHtml(string text)
        {
            text = text.Replace("\r", "");
            text = text.Replace("\n", "");
            text = text.Replace("\t", "");
            text = text.Replace("&nbsp;", " ");
            text = text.Replace("&amp;", "&");
            text = text.Replace("&quot;", "\"");
            text = text.Replace("&lt;", "<");
            text = text.Replace("&gt;", ">");
            return text;
        }
        public static string StringFromHtmlNode(HtmlNode node)
        {
            string temp;
            temp = node.InnerHtml;
            temp = temp.Replace("</p>", "\n");
            temp = temp.Replace("</div>", "\n");
            temp = temp.Replace("</tr>", "\n");
            temp = temp.Replace("</td>", " ");

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].Equals('<'))
                {
                    int start_tag = i;
                    for (int j = i + 1; j < temp.Length; j++)
                    {
                        if (temp[j].Equals('>'))
                        {
                            int end_tag = j;
                            temp = temp.Substring(0, start_tag) + temp.Substring(end_tag + 1, temp.Length - end_tag - 1);
                            i--;
                            break;
                        }
                    }
                }
            }
            return temp;
        }
    }
}
