using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
namespace WebSupport
{
    public static class WebPacket
    {
        public static bool Debug_NotUseWeb = false;
        private static string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
        public static string Web_Get(Encoding encoding, CookieContainer Cookie, string url, string referer = null)
        {
            if (Debug_NotUseWeb) return null;
            HttpWebRequest hreq = (HttpWebRequest)WebRequest.Create(url);
            hreq.Method = "GET";
            hreq.Referer = referer;
            if (referer == null)
            {
                hreq.Referer = url;
            }
            hreq.ContentType = "application/x-www-form-urlencoded";
            hreq.CookieContainer = Cookie;
            hreq.UserAgent = UserAgent;
            HttpWebResponse hres = null;
            Stream dataStream = null;
            StreamReader sr = null;
            try
            {
                hres = (HttpWebResponse)hreq.GetResponse();
                if (hres.StatusCode == HttpStatusCode.OK)
                {
                    dataStream = hres.GetResponseStream();
                    sr = new StreamReader(dataStream, encoding);
                    return sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                if (hres != null) hres.Close();
                if (dataStream != null) dataStream.Close();
                if (sr != null) sr.Close();
            }
            return null;
        }
        public static string Web_POST(CookieContainer Cookie, string url, string referer, string post)
        {
            if (Debug_NotUseWeb) return null;
            HttpWebRequest hreq = (HttpWebRequest)WebRequest.Create(url);
            hreq.Method = "POST";
            hreq.Referer = referer;
            hreq.ContentType = "application/x-www-form-urlencoded";
            hreq.CookieContainer = Cookie;
            hreq.UserAgent = UserAgent;
            StreamWriter sw = new StreamWriter(hreq.GetRequestStream());
            sw.Write(post);
            sw.Close();
            HttpWebResponse hres = null;
            Stream dataStream = null;
            StreamReader sr = null;
            try
            {
                hres = (HttpWebResponse)hreq.GetResponse();
                if (hres.StatusCode == HttpStatusCode.OK)
                {
                    dataStream = hres.GetResponseStream();
                    sr = new StreamReader(dataStream, Encoding.UTF8);
                    return sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                if (hres != null) hres.Close();
                if (dataStream != null) dataStream.Close();
                if (sr != null) sr.Close();
            }
            return null;
        }
        public static HtmlDocument Web_GET_Html(Encoding encoding, CookieContainer Cookie, string url, string referer = null)
        {
            string data = Web_Get(encoding, Cookie, url, referer);
            if (data == null) return null;
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(data);
            return html;
        }
        public static HtmlDocument Web_POST_Html(CookieContainer Cookie, string url, string referer, string post)
        {
            string data = Web_POST(Cookie, url, referer, post);
            if (data == null) return null;
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(data);
            return html;
        }
    }
}
