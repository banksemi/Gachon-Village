using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSupport
{
    /// <summary>
    /// 이 클래스는 디버그할때 걸린 시간을 측정하여 콘솔에 출력해줍니다.
    /// 타임아웃 기능으로 시간이 오래 걸리는 작업을 잡아낼 수 있습니다.
    /// </summary>
    public class Timer
    {
        double error_max = 0;
        string name;
        DateTime time;
        public Timer(string name)
        {
            this.name = name;
            time = DateTime.Now;
        }
        public Timer(string name, double error_max) : this(name)
        {
            this.error_max = error_max;
        }
        public void Print()
        {
            double sp = (DateTime.Now - time).TotalSeconds;
            if (error_max !=0 && error_max < sp)
            {
                throw new Exception("설정한 시간을 초과했습니다");
            }
            Console.WriteLine("[Timer] " + name + " - " + sp + "s");
        }
    }
}
