FROM microsoft/windowsservercore

ENV NPM_CONFIG_LOGLEVEL info  
ENV NODE_VERSION 4.4.5  
ENV NODE_SHA256 7b2409605c871a40d60c187bd24f6f6ddf10590df060b7d905ef46b3b3aa7f81

RUN powershell -Command \  
    wget -Uri https://nodejs.org/dist/v%NODE_VERSION%/node-v%NODE_VERSION%-x64.msi -OutFile node.msi -UseBasicParsing ; \
    if ((Get-FileHash node.msi -Algorithm sha256).Hash -ne $env:NODE_SHA256) {exit 1} ; \
    Start-Process -FilePath msiexec -ArgumentList /q, /i, node.msi -Wait ; \
    Remove-Item -Path node.msi

#make a directory in container
RUN mkdir c:\oauthserver

# copy src from host to simulator
copy oauthserver\\src C:\oauthserver

# make simulator as working directory
WORKDIR c:\\oauthserver

# download the packages in container
RUN npm install

EXPOSE 5001
# make starting command as npm
CMD [ "npm.cmd", "start" ]