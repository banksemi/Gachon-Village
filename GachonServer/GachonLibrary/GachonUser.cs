using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSupport;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using SQL_Library;
namespace GachonLibrary
{
    public class GachonUser
    {
        private static Object Login_Lock = new object();
        private CookieContainer cookie = new CookieContainer();
        /// <summary>
        /// 로그인 여부를 확인합니다.
        /// </summary>
        public bool LoginOk { get; private set; }
        /// <summary>
        ///  학교 로그인 아이디입니다.
        /// </summary>
        public string ID { get; private set; }
        private string password = null;
        private string escape_password { get { return Uri.EscapeDataString(password); } }
        #region 유저 정보값 변수 설정
        /// <summary>
        ///  해당 학생의 이름입니다.
        /// </summary>
        public string Name
        {
            get
            {
                if (_name == null) GetUserInfo();
                return _name;
            }
            set { _name = value; }
        }
        public string Department
        {
            get
            {
                if (_department == null) GetUserInfo();
                return _department;
            }
            private set { _department = value; }
        }
        /// <summary>
        /// 해당 학생의 휴대폰 번호입니다.
        /// </summary>
        public string Phone
        {
            get
            {
                if (_phone == null) GetUserInfo();
                return _phone;
            }
            private set { _phone = value; }
        }
        /// <summary>
        /// 해당 학생의 이메일 주소입니다.
        /// </summary>
        public string Email
        {
            get
            {
                if (_email == null) GetUserInfo();
                return _email;
            }
            private set { _email = value; }
        }
        /// <summary>
        /// 해당 학생의 학번입니다.
        /// </summary>
        public string StudentNumber
        {
            get
            {
                if (_studentnumber == null) GetUserInfo();
                return _studentnumber;
            }
            private set { _studentnumber = value; }
        }
        /// <summary>
        /// 현재 학생이 수강중인 강의 목록입니다.
        /// </summary>
        public List<GachonClass> Takes = new List<GachonClass>();
        private string _name = null;
        private string _department = null;
        private string _phone = null;
        private string _email = null;
        private string _studentnumber = null;
        #endregion
        private GachonUser(string ID, string password)
        {
            this.ID = ID;
            this.password = password;
            Login();
        }
        public static GachonUser GetObject(string ID, string password)
        {
            /*
             * 동시에 여러 쓰레드에서 같은 아이디에 대해 새로 로그인하는경우 문제가 있습니다.
             * 지금은 이 함수 전체에 lock을 걸어 해결하지만, 더 나은 성능을 위해서는
             * 각각의 유저에 대해 lock을 진행할 필요가 있습니다.
             */
            lock (Login_Lock)
            {
                if (GachonObjects.AllUser.ContainsKey(ID))
                {
                    if (GachonObjects.AllUser[ID].password == password) return GachonObjects.AllUser[ID];
                    else return null;
                }
                else
                {
                    GachonUser user = new GachonUser(ID, password);
                    if (user.LoginOk)
                    {
                        GachonObjects.AllUser.Add(ID, user);
                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        /// <summary>
        /// 로그인 세션을 강제로 실행시킵니다.
        /// 주의: 각 함수에서 로그인 세션을 유지되는지 확인후 자동으로 실행하기때문에 따로 이 함수를 실행하지 않아도 됨.
        /// </summary>
        public void Login()
        {
            // 가천대 로그인페이지 접근 시도/ (sson.kyungwon.ac.kr)에서 세션값을 가져오는 과정(GID 오류때문에 반드시 필요)
            VisitPage("https://www.gachon.ac.kr/site/login.jsp");
            string a = WebPacket.Web_POST(cookie,
                "https://sson.kyungwon.ac.kr/sso/pmi-sso-login-uid-password2.jsp",
                "https://www.gachon.ac.kr/site/login_sso.jsp",
                "return_url=&uid=" + ID + "&password=" + escape_password + "&x=41&y=20");
            if (a.IndexOf("location.href='https://www.gachon.ac.kr:443/site/login.jsp'") >= 0)
            {
                // 사이버 캠퍼스에 로그인을 하면서 강의 정보 불러옴.
                HtmlDocument data = WebPacket.Web_POST_Html(cookie, "https://cyber.gachon.ac.kr/login/index.php", "https://cyber.gachon.ac.kr/login.php", "username=" + ID + "&password=" + escape_password);
                // 학생 테이블에 ID가 없으면 무조건 추가
                MysqlNode search_id = new MysqlNode(GachonOption.MysqlOption, "SELECT id FROM account WHERE id = ?id");               
                search_id["id"] = ID;
                search_id.ExecuteReader();

                if (!search_id.Read())
                {
                    MysqlNode idnode = new MysqlNode(GachonOption.MysqlOption, "INSERT into account (id) values (?id) ");
                    idnode["id"] = ID;
                    idnode.ExecuteNonQuery();
                }  //아이디만 있을수도있고 아닐수도있음

                //takes_course에서 course_no가 없으면 추가
                MysqlNode search_course = new MysqlNode(GachonOption.MysqlOption, "SELECT course_no FROM takes_course WHERE student_id = ?id");
                search_course["id"] = ID;
                         
                using (search_course.ExecuteReader())
                {
                    bool isExist = false;
                    //있으면 연결만
                    while (search_course.Read())
                    {
                        isExist = true;
                        CombineClass(GachonObjects.AllClass[search_course.GetString("course_no")]);
                    }
                    if (isExist == false)
                    {
                        List<GachonClass> need_eclass_info = new List<GachonClass>();
                        foreach (HtmlNode node in data.DocumentNode.SelectNodes("//div[@class='course_box']"))
                        {
                            JObject box = ParseSupport.CyberCampusTitle(node.SelectSingleNode(".//div[@class='course-title']/h3").InnerText);
                            string title = (string)box["title"];
                            string key = (string)box["key"];
                            lock (GachonObjects.AllClass)
                            {
                                if (!GachonObjects.AllClass.ContainsKey(key))
                                {
                                    GachonClass newclass = GachonClass.GetObject(title, key, true);
                                    JObject urlq = ParseSupport.UrlQueryParser(node.SelectSingleNode(".//a[@class='course_link']").Attributes["href"].Value);
                                    newclass.CombineSite(new GachonCyberCampus(urlq["id"].ToString()));
                                    need_eclass_info.Add(newclass);
                                }
                                MysqlNode insertNode = new MysqlNode(GachonOption.MysqlOption,
                                    "INSERT into takes_course(student_id, course_no) values (?id, ?course_no)");
                                insertNode["id"] = ID;
                                insertNode["course_no"] = key;
                                insertNode.ExecuteNonQuery();

                                CombineClass(GachonObjects.AllClass[key]);
                            }
                        }
                        HtmlDocument eclassinfo = null;
                        if (need_eclass_info.Count > 0)
                            eclassinfo = WebPacket.Web_GET_Html(Encoding.UTF8, cookie, "http://eclass.gachon.ac.kr/index.jsp");
                        // 새로운 클래스가 생겼으니 eclass 정보를 읽어서 새로 생긴 클래스와 연결한다.
                        foreach (GachonClass newclass in need_eclass_info)
                        {
                            HtmlNode node = eclassinfo.DocumentNode.SelectSingleNode("//a[@title=" + newclass.ID + newclass.Sec_ID + "]");
                            if (node != null)
                            {
                                string e_id = ParseSupport.UrlQueryParser(node.Attributes["href"].Value)["Forum_seq"].ToString();
                                newclass.CombineSite(new GachonEClass(e_id));
                            }
                        }
                    }
                }
                // 카페 도메인에도 세션을 만들어줌. 없을경우 처음 카페 접속시 리다이렉션 걸림.
                VisitPage("http://cafe.gachon.ac.kr");
                LoginOk = true;
            }
            else LoginOk = false;
        }
        /// <summary>
        /// 수강중인 강의들에서 자신의 정보를 지웁니다.
        /// </summary>
        public void Logout()
        {
            LoginOk = false;
            password = null;
            foreach(GachonClass gc in Takes)
            {
                gc.Users.Remove(this);
            }
            GachonObjects.AllUser.Remove(ID);
        }
        public void Dispose()
        {
            Logout();
        }
        public void CombineClass(GachonClass gachonClass)
        {
            gachonClass.CombineTakeUser(this);
        }
        /// <summary>
        /// 해당 학생의 개인정보를 강제로 갱신합니다.
        /// </summary>
        public void GetUserInfo()
        {
            if (LoginOk == false) return; // 마지막 로그인을 실패했을경우
            MysqlNode node = new MysqlNode(GachonOption.MysqlOption, "SELECT * FROM account WHERE id = ?ID");  //수정? 
            node["ID"] = ID;

            using (node.ExecuteReader())
            {
                node.Read();
                StudentNumber = node.GetString("studentnumber");

                if (Int32.Parse(StudentNumber)!=0)
                {
                    //StudentNumber = node.GetString("studentnumber");                 
                    Name = node.GetString("name");
                    Email = node.GetString("email");
                    Phone = node.GetString("phone");
                    Department = node.GetString("department");
                }
                else
                {
                    HtmlDocument data = WebPacket.Web_GET_Html(Encoding.UTF8, cookie, "https://cyber.gachon.ac.kr/user/user_edit.php");
                    StudentNumber = data.DocumentNode.SelectSingleNode("//div[@class='felement fstatic']").InnerText;
                    Name = data.GetElementbyId("id_firstname").Attributes["value"].Value;
                    Email = data.GetElementbyId("id_email").Attributes["value"].Value;
                    Phone = data.GetElementbyId("id_phone2").Attributes["value"].Value;
                    Department = data.DocumentNode.SelectSingleNode("//p[@class='department']").InnerText;

                    MysqlNode insert = new MysqlNode(GachonOption.MysqlOption, "UPDATE  account SET studentnumber = ?num, name = ?name, department = ?dept"+
                                                                               ",phone = ?phone , email = ?email where id = ?id ");
                    insert["id"] = ID;
                    insert["num"] = StudentNumber;
                    insert["name"] = Name;
                    insert["dept"] = Department;
                    insert["phone"] = Phone;
                    insert["email"] = Email;
                    insert.ExecuteNonQuery();
                }
            }         
        }
        public override string ToString()
        {
            return Name + "(" + ID + ")";
        }
        /// <summary>
        /// 객체를 스트링으로 반환합니다.
        /// </summary>
        /// <param name="full">모든 정보를 반환합니다.</param>
        /// <returns></returns>
        public string ToString(bool full)
        {
            if (full == true)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("아이디 : " + ID);
                if (LoginOk == true)
                {
                    sb.AppendLine("로그인 상태 : 성공");
                    sb.AppendLine("이름 : " + Name);
                    sb.AppendLine("학번 : " + StudentNumber);
                    sb.AppendLine("학과 : " + Department);
                    sb.AppendLine("이메일 : " + Email);
                    sb.AppendLine("핸드폰 : " + Phone);
                    sb.AppendLine("수강중인 과목(" + Takes.Count + ")");
                    foreach (GachonClass gc in Takes)
                    {
                        sb.AppendLine(gc.ToString());
                    }
                }
                else
                {
                    sb.AppendLine("로그인 상태 : 실패");
                }
              return sb.ToString();
            }
            else
                return ToString();
        }
        public HtmlDocument VisitPage(Uri url, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            return WebPacket.Web_GET_Html(encoding, cookie, url.AbsoluteUri);
        }
        public HtmlDocument VisitPage(string url, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            return WebPacket.Web_GET_Html(encoding, cookie, url);
        }
    }
}
