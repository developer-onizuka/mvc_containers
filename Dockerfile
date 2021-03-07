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

