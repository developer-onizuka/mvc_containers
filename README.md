# 0. make a certain project and go to the directory
```
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
