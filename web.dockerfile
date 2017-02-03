FROM microsoft/dotnet-nightly:nanoserver

ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT Development

#make a directory in container
RUN mkdir c:\testing\web
RUN mkdir c:\temp

# make web as working directory
WORKDIR c:\\testing\\web



# download the packages in container
RUN dotnet new -t web
# copy the modified files for this demo. 
COPY web\\src\\AccountController.cs c:/testing/web/Controllers
COPY web\\src\\project.json c:/testing/web
COPY web\\src\\Startup.cs c:/testing/web
COPY web\\src\\Custom.cs c:/testing/web
COPY web\\src\\CustomLogger.cs c:/testing/web

RUN dotnet restore
RUN dotnet ef database update

EXPOSE 5000
# make starting command as npm
CMD [ "dotnet.exe", "run" ]