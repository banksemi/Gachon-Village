using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.IO;
namespace NetworkLibrary.File
{
    public class NServerFile : NetworkFile
    {
        ESocket socket;
        public NServerFile(ESocket socket, string Path) : base(Path)
        {
            this.socket = socket;
        }

        public NServerFile(ESocket socket, JObject json) : base(json)
        {
            this.socket = socket;
        }

        /// <summary>
        /// 이 파일에 대해서 업로드 요청을 허락합니다. 한번만 실행되야합니다.
        /// </summary>
        public void StartFileTransmission(string path)
        {
            this.Path = path;
            JObject jObject = new JObject();
            jObject["type"] = -2;
            jObject["no"] = No;
            jObject["serverkey"] = ServerKey;
            socket.Send(jObject);
        }
    }
}
