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
