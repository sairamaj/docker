FROM microsoft/dotnet-nightly:nanoserver

ENV ASPNETCORE_URLS http://*:5000

#make a directory in container
RUN mkdir c:\testing\web

# make web as working directory
WORKDIR c:\\testing\\web



# download the packages in container
RUN dotnet new -t web
# copy the modified files for this demo. 
COPY web\\src c:/testing/web
RUN dotnet restore

EXPOSE 5000
# make starting command as npm
CMD [ "dotnet.exe", "run" ]