# Gachon-Village
Welcome our virtual world which contains gachon life!


# How to build

1. Unzip to the appropriate path.
![image](https://user-images.githubusercontent.com/17453822/49692248-6e115d80-fb99-11e8-8d25-3f1c666e939f.png)

### Mysql Setup

2. Install MariaDB in a different path. (MYSQL is possible but not verified)

3. Create Mysql Database (gachon_village)
Please set the encoding setting to <code>utf8 -- UTF-8 Unicode</code>.

4. Run gachon_village.sql to your database!
So, The following table and view will be created.
![image](https://user-images.githubusercontent.com/17453822/49692299-61d9d000-fb9a-11e8-8f0f-648de9bed255.png)

### Server Start

5. Open the file to /Gachon-Server/MainServer/private_data(git).cs

6. Please Enter your database setting.
<pre>
namespace MainServer
{
    class private_data
    {
        public static MysqlOption mysqlOption = new MysqlOption("mysql_host", "mysql_database", "id", "password");
    }
}
</pre>

7. Run Or Build your server!!
If the build succeeds, your network library will be connected to Unity because of my build event setting.

- Check server screen
![image](https://user-images.githubusercontent.com/17453822/49692346-c3e70500-fb9b-11e8-94ea-00492c7c608e.png)

- Check created file in /Gachon-Village/Unity Client/Gachon Vilage/Assets/Plugins/NetworkLibrary/
![image](https://user-images.githubusercontent.com/17453822/49692349-d2cdb780-fb9b-11e8-9052-8b0dfd817cda.png)
