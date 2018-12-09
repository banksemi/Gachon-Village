# Gachon-Village
Welcome our virtual world which contains gachon life!


# How to build

### 1. Unzip to the appropriate path.

![image](https://user-images.githubusercontent.com/17453822/49692248-6e115d80-fb99-11e8-8d25-3f1c666e939f.png)

## Mysql Setup

### 2. Install MariaDB in a different path. (MYSQL is possible but not verified)

### 3. Create Mysql Database (gachon_village).
Please set the encoding setting to <code>utf8 -- UTF-8 Unicode</code>.

### 4. Run gachon_village.sql to your database!
So, The following table and view will be created.
![image](https://user-images.githubusercontent.com/17453822/49692299-61d9d000-fb9a-11e8-8f0f-648de9bed255.png)

## Server Start

### 5. Open the file to /Gachon-Server/MainServer/private_data(git).cs

### 6. Please Enter your database setting.
<pre>
namespace MainServer
{
    class private_data
    {
        public static MysqlOption mysqlOption = new MysqlOption("mysql_host", "mysql_database", "id", "password");
    }
}
</pre>

### 7. Run Or Build your server!!

If the build succeeds, your network library will be connected to Unity because of my build event setting.

- Check server screen
![image](https://user-images.githubusercontent.com/17453822/49692346-c3e70500-fb9b-11e8-94ea-00492c7c608e.png)

- Check created file in /Gachon-Village/Unity Client/Gachon Vilage/Assets/Plugins/NetworkLibrary/
![image](https://user-images.githubusercontent.com/17453822/49692349-d2cdb780-fb9b-11e8-9052-8b0dfd817cda.png)

## Unity Client

###  8. Please install the version('2018.2.12' to '2018.2.18.') of Unity engine

### 9. Open the project /Gachon-Village/Unity Client/Gachon Vilage/ by using Unity.

### 10. Select Login Scene. (double click Assets/Scenes/Login.scene file)
However, if NGUI is not set, nothing will be visible.

### 11. Import your NGUI file.
Run the NGUI file with the Unity running!
[NGUI_Temp.zip](https://github.com/banksemi/Gachon-Village/files/2660174/NGUI_Temp.zip)

![image](https://user-images.githubusercontent.com/17453822/49692397-81bec300-fb9d-11e8-9525-ac8a55133bf3.png)

So, This is result.

![image](https://user-images.githubusercontent.com/17453822/49692413-c9454f00-fb9d-11e8-854e-1ed2968fcc3c.png)

Since the resolution of our game is 1600*900, It would be better to fix the resolution.

![image](https://user-images.githubusercontent.com/17453822/49692417-d8c49800-fb9d-11e8-8639-a3214124f9aa.png)


### 12. Enter your server IP
Open the file to ./Unity Client/Gachon Vilage/Assets/Script/Network/NetworkMain.cs
<pre>
server = new Client("127.0.0.1", 1119);
</pre>
<code>1119</code> is Default Port for Main Socket.

<code>8282</code> is port for File Transfer.

