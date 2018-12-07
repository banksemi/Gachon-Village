using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer
{
    /// <summary>
    /// 게임 오브젝트의 좌표,회전값을 나타내기 위한 클래스입니다.
    /// </summary>
    public class Vector4
    {
        /// <summary>
        /// X축 (좌우)
        /// </summary>
        public float x;
        /// <summary>
        /// Y 축 (높이)
        /// </summary>
        public float y;
        /// <summary>
        /// Z 축 (평면상의 상하)
        /// </summary>
        public float z;
        /// <summary>
        /// Y축 기준 회전값, 우리 게임에서는 오브젝트는 모두 Y축 기준으로만 회전
        /// </summary>
        public float q;
        public Vector4(float x, float y, float z, float q)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.q = q;
        }
    }
}
