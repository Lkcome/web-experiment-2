1.数据库

Scaffold-DbContext "persist security info=True;data source=rm-bp1tg219t7o5j5ex8fo.rwlb.rds.aliyuncs.com;port=3306;initial catalog=2109060123_db;user id=2109060123;password=2109060123;character set=utf8;allow zero datetime=true;convert zero datetime=true;pooling=true;maximumpoolsize=3000" Pomelo.EntityFrameworkCore.MySql -OutputDir Models -f

连接数据库操作（这是连接的一个远程的数据库），可以将此更新为指向本地的数据库，修改指令
也可直接生成之后在

#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=selectcourse;user id=root;password=200301071314Lk;character set=utf8;allow zero datetime=true;convert zero datetime=true;pooling=true;maximumpoolsize=3000", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql"));
![image](https://github.com/user-attachments/assets/9db2a75f-12ff-438b-a30f-86422c2fd635)在models里直接修改这里也可以。


2.下载nuget包

![image](https://github.com/user-attachments/assets/0da7893d-d08c-4391-9a33-334cac9a50f8)
