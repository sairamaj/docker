FROM microsoft/dotnet-nightly:nanoserver

ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT Development

#make a directory in container
RUN mkdir c:\testing\web
RUN mkdir c:\temp

# make web as working directory
WORKDIR c:\\testing\\web



# download the packages in container
RUN dotnet new mvc
# copy the modified files for this demo. 
COPY src\\web\\AccountController.cs c:/testing/web/Controllers
# COPY src\\web\\project.json c:/testing/web
COPY src\\web\\Startup.cs c:/testing/web
# COPY src\\web\\Custom.cs c:/testing/web
COPY src\\web\\CustomLogger.cs c:/testing/web

RUN dotnet restore


EXPOSE 5000
# make starting command as npm
CMD [ "dotnet.exe", "run" ]