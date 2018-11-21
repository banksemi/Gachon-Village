using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
namespace NetworkLibrary.File
{
    public class NetworkFile
    {
        public string Path { get; private set; }
        public long FileSize { get; private set; }
        public string FileName { get; private set; }
        public bool upload { get; private set; }
        public JObject Information
        {
            get
            {
                JObject jObject = new JObject();
                jObject["size"] = FileSize;
                jObject["name"] = FileName;
                return jObject;
            }
        }

        public event System.Action Start;
        public event System.Action Process;
        public event System.Action End;
        public event System.Action Error;
        public NetworkFile(string Path, bool upload)
        {
            this.Path = Path;
            this.upload = upload;
            FileInfo info = new FileInfo(Path);
            FileSize = info.Length;
            FileName = info.Name;
        }
        public NetworkFile(JObject json)
        {
            this.upload = false;
            FileSize = (long)json["size"];
            FileName = (string)json["name"];
        }
    }
}
