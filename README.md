# 0. make a certain project and go to the directory and pull mongodb
```
$ sudo docker pull mongo
$ dotnet new mvc -o Employee
$ cd Employee
$ dotnet add package MongoDB.Driver
(See also my HomeController.cs)
```

# 1. C# build self-contained
```
$ dotnet publish -c release -r linux-x64 --self-contained
Microsoft (R) Build Engine version 16.6.0+5ff7b0c9e for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored /home/xxxxxxxxx/dotnet/src/Employee/Employee.csproj (in 41.7 sec).
  Employee -> /home/xxxxxxxx/dotnet/src/Employee/bin/release/netcoreapp3.1/linux-x64/Employee.dll
  Employee -> /home/xxxxxxxx/dotnet/src/Employee/bin/release/netcoreapp3.1/linux-x64/Employee.Views.dll
  Employee -> /home/xxxxxxxx/dotnet/src/Employee/bin/release/netcoreapp3.1/linux-x64/publish/
```

# 2. building container for Docker
```
$ mkdir -p ~/docker/Employee
$ cd bin/release/netcoreapp3.1/linux-x64/
$ tar cvfz ~/docker/Employee/publish.tar.gz publish
$ cat Dockerfile 
FROM ubuntu:latest
RUN mkdir /usr/local/dotnet
ADD publish.tar.gz /usr/local/dotnet
ADD dotnet-sdk-3.1.405-linux-x64.tar.gz /usr/local/dotnet
RUN apt-get update
RUN apt-get install -y libssl-dev

ENV PATH $PATH:/usr/local/dotnet
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV ASPNETCORE_URLS="https://*:5001;http://*:5000"

RUN dotnet dev-certs https --clean
RUN dotnet dev-certs https

$ ls
Dockerfile  dotnet-sdk-3.1.405-linux-x64.tar.gz  publish.tar.gz

$ sudo docker build -t employee .
...
Successfully built b70eb3858423
Successfully tagged employee:latest

$ sudo docker images
REPOSITORY          TAG                 IMAGE ID            CREATED             SIZE
employee            latest              b70eb3858423        14 minutes ago      563MB
ubuntu              latest              bb0eaf4eee00        5 months ago        72.9MB
```

# 3. reverse proxy with nginx
```
$ sudo docker pull nginx
Using default tag: latest
latest: Pulling from library/nginx
45b42c59be33: Pull complete 
8acc495f1d91: Pull complete 
ec3bd7de90d7: Pull complete 
19e2441aeeab: Pull complete 
f5a38c5f8d4e: Pull complete 
83500d851118: Pull complete 
Digest: sha256:f3693fe50d5b1df1ecd315d54813a77afd56b0245a404055a946574deb6b34fc
Status: Downloaded newer image for nginx:latest
docker.io/library/nginx:latest

$ sudo docker images
REPOSITORY          TAG                 IMAGE ID            CREATED             SIZE
employee            latest              b70eb3858423        17 minutes ago      563MB
nginx               latest              35c43ace9216        2 weeks ago         133MB
ubuntu              latest              bb0eaf4eee00        5 months ago        72.9MB
centos              latest              0d120b6ccaa8        6 months ago        215MB
zookeeper           latest              6ad6cb039dfa        7 months ago        252MB
mongo               latest              aa22d67221a0        7 months ago        493MB

$ sudo docker volume create nginx_conf
nginx_conf
$ sudo docker inspect nginx_conf
[
    {
        "CreatedAt": "2021-03-07T04:21:43Z",
        "Driver": "local",
        "Labels": {},
        "Mountpoint": "/var/snap/docker/common/var-lib-docker/volumes/nginx_conf/_data",
        "Name": "nginx_conf",
        "Options": {},
        "Scope": "local"
    }
]
$ sudo vi /var/snap/docker/common/var-lib-docker/volumes/nginx_conf/_data/default.conf
$ sudo cat /var/snap/docker/common/var-lib-docker/volumes/nginx_conf/_data/default.conf
upstream proxy.com {
	server 172.17.0.1:5001;
	server 172.17.0.1:5011;
	server 172.17.0.1:5021;
	server 172.17.0.1:5031;
	server 172.17.0.1:5041;
	server 172.17.0.1:5051;
	server 172.17.0.1:5061;
	server 172.17.0.1:5071;
}

server {
	listen 80;
	server_name localhost;
	location / {
		root /usr/share/nginx/html;
		index index.html index.htm;
		proxy_pass https://proxy.com;
	}
}

```

# 4. Run everything
```
sudo docker run -itd -p 27017:27017 --rm --name="mongodb" mongo:latest
sudo docker run -itd -p 8080:80 --rm -v nginx_conf:/etc/nginx/conf.d --name="nginx" nginx:latest
sudo docker run -itd -p 5001:5001 -p 5000:5000 --name="emp0" --rm employee:latest /usr/local/dotnet/publish/Employee
sudo docker run -itd -p 5011:5001 -p 5010:5000 --name="emp1" --rm employee:latest /usr/local/dotnet/publish/Employee
sudo docker run -itd -p 5021:5001 -p 5020:5000 --name="emp2" --rm employee:latest /usr/local/dotnet/publish/Employee
sudo docker run -itd -p 5031:5001 -p 5030:5000 --name="emp3" --rm employee:latest /usr/local/dotnet/publish/Employee
sudo docker run -itd -p 5041:5001 -p 5040:5000 --name="emp4" --rm employee:latest /usr/local/dotnet/publish/Employee
sudo docker run -itd -p 5051:5001 -p 5050:5000 --name="emp5" --rm employee:latest /usr/local/dotnet/publish/Employee
sudo docker run -itd -p 5061:5001 -p 5060:5000 --name="emp6" --rm employee:latest /usr/local/dotnet/publish/Employee
sudo docker run -itd -p 5071:5001 -p 5070:5000 --name="emp7" --rm employee:latest /usr/local/dotnet/publish/Employee
```
```
$ sudo docker ps -a
CONTAINER ID        IMAGE               COMMAND                  CREATED             STATUS              PORTS                                            NAMES
81510b94bf72        employee:latest     "/usr/local/dotnet/p…"   6 seconds ago       Up 5 seconds        0.0.0.0:5070->5000/tcp, 0.0.0.0:5071->5001/tcp   emp7
235a78a320a5        employee:latest     "/usr/local/dotnet/p…"   10 seconds ago      Up 8 seconds        0.0.0.0:5060->5000/tcp, 0.0.0.0:5061->5001/tcp   emp6
090db8821389        employee:latest     "/usr/local/dotnet/p…"   11 seconds ago      Up 10 seconds       0.0.0.0:5050->5000/tcp, 0.0.0.0:5051->5001/tcp   emp5
aa85ab77d220        employee:latest     "/usr/local/dotnet/p…"   12 seconds ago      Up 11 seconds       0.0.0.0:5040->5000/tcp, 0.0.0.0:5041->5001/tcp   emp4
2110da18eb5f        employee:latest     "/usr/local/dotnet/p…"   14 seconds ago      Up 12 seconds       0.0.0.0:5030->5000/tcp, 0.0.0.0:5031->5001/tcp   emp3
030445c3c539        employee:latest     "/usr/local/dotnet/p…"   15 seconds ago      Up 13 seconds       0.0.0.0:5020->5000/tcp, 0.0.0.0:5021->5001/tcp   emp2
5c4d24302cae        employee:latest     "/usr/local/dotnet/p…"   16 seconds ago      Up 14 seconds       0.0.0.0:5010->5000/tcp, 0.0.0.0:5011->5001/tcp   emp1
a3bc29c772eb        employee:latest     "/usr/local/dotnet/p…"   16 seconds ago      Up 15 seconds       0.0.0.0:5000-5001->5000-5001/tcp                 emp0
28c141ca417d        nginx:latest        "/docker-entrypoint.…"   14 minutes ago      Up 14 minutes       0.0.0.0:8080->80/tcp                             nginx
d704188e6730        mongo:latest        "docker-entrypoint.s…"   4 hours ago         Up 4 hours          0.0.0.0:27017->27017/tcp                         mongodb
```

# 5. PC web browser
```
If you use the Virtual Box, use the portforwarding so that you can accsess the NGINX's port 8080.
Type "http://127.0.0.1:8080". You can see the Employee Application through the browser.
```
