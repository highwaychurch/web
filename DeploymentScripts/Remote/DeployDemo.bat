@echo off

Set environment=demo
Set webDomainName=%environment%.highway.com.au
Set driveName=C:
Set rootPath=%driveName%\Highway
Set msdeploy="C:\Program Files\IIS\Microsoft Web Deploy V2\msdeploy.exe"

echo ################################
echo Stopping Web Sites
echo ################################
echo STOP Application Pool for Web-Site:www.%webDomainName%
%msdeploy% -verb:sync -source:recycleApp -dest:recycleApp=www.%webDomainName%,recycleMode=StopAppPool
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo STOP Application Pool for Web-Site:identity.%webDomainName%
%msdeploy% -verb:sync -source:recycleApp -dest:recycleApp=identity.%webDomainName%,recycleMode=StopAppPool
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo STOP Application Pool for Web-Site:creative.%webDomainName%
%msdeploy% -verb:sync -source:recycleApp -dest:recycleApp=creative.%webDomainName%,recycleMode=StopAppPool
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo STOP Application Pool for Web-Site:ravendb.%webDomainName%
%msdeploy% -verb:sync -source:recycleApp -dest:recycleApp=ravendb.%webDomainName%,recycleMode=StopAppPool
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo ################################
echo Deploying Web Sites
echo ################################
%msdeploy% -verb:sync -source:package=%rootPath%\DeploymentPackages\Highway.Web.zip -dest:auto -setParam:name="IIS Web Application Name",value="www.%webDomainName%" -skip:skipAction=Delete,absolutePath="cachedassets"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo Setting ACLs
%msdeploy% -verb:sync -source:setacl -dest:setacl="www.%webDomainName%/cachedassets",setaclaccess=Write
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo Copying %environment% configuration file
copy /Y %rootPath%\www.%webDomainName%\web.%environment%.config %rootPath%\www.%webDomainName%\web.config
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo Deleting all other environment configuration files
del /Q /F %rootPath%\www.%webDomainName%\web.*.config
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

%msdeploy% -verb:sync -source:package=%rootPath%\DeploymentPackages\Identity.Web.zip -dest:auto -setParam:name="IIS Web Application Name",value="identity.%webDomainName%" -skip:skipAction=Delete,absolutePath="cachedassets"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo Setting ACLs
%msdeploy% -verb:sync -source:setacl -dest:setacl="identity.%webDomainName%/cachedassets",setaclaccess=Write
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo Copying %environment% configuration file
copy /Y %rootPath%\identity.%webDomainName%\web.%environment%.config %rootPath%\identity.%webDomainName%\web.config
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo Deleting all other environment configuration files
del /Q /F %rootPath%\identity.%webDomainName%\web.*.config
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

%msdeploy% -verb:sync -source:package=%rootPath%\DeploymentPackages\Creative.Web.zip -dest:auto -setParam:name="IIS Web Application Name",value="creative.%webDomainName%" -skip:skipAction=Delete,absolutePath="cachedassets"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo Setting ACLs
%msdeploy% -verb:sync -source:setacl -dest:setacl="creative.%webDomainName%/cachedassets",setaclaccess=Write
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo Copying %environment% configuration file
copy /Y %rootPath%\creative.%webDomainName%\web.%environment%.config %rootPath%\creative.%webDomainName%\web.config
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo Deleting all other environment configuration files
del /Q /F %rootPath%\creative.%webDomainName%\web.*.config
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

%msdeploy% -verb:sync -source:package=%rootPath%\DeploymentPackages\Raven.Web.zip -dest:auto -setParam:name="IIS Web Application Name",value="ravendb.%webDomainName%" -skip:skipAction=Delete,absolutePath="Data"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo Setting ACLs
%msdeploy% -verb:sync -source:setacl -dest:setacl="ravendb.%webDomainName%/Data",setaclaccess=Write
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo Copying %environment% configuration file
copy /Y %rootPath%\ravendb.%webDomainName%\web.%environment%.config %rootPath%\ravendb.%webDomainName%\web.config
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo Deleting all other environment configuration files
del /Q /F %rootPath%\ravendb.%webDomainName%\web.*.config
IF %ERRORLEVEL% NEQ 0 GOTO ERROR


echo ################################
echo Creating required folders
echo ################################
if not exist %rootPath% md %rootPath%
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
if not exist %rootPath%\Logs md %rootPath%\Logs
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
if not exist %rootPath%\Email md %rootPath%\Email
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo ################################
echo Starting IIS
echo ################################
echo START Application Pool for Web-Site:ravendb.%webDomainName%
%msdeploy% -verb:sync -source:recycleApp -dest:recycleApp=ravendb.%webDomainName%,recycleMode=StartAppPool
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo START Application Pool for Web-Site:creative.%webDomainName%
%msdeploy% -verb:sync -source:recycleApp -dest:recycleApp=creative.%webDomainName%,recycleMode=StartAppPool
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo START Application Pool for Web-Site:identity.%webDomainName%
%msdeploy% -verb:sync -source:recycleApp -dest:recycleApp=identity.%webDomainName%,recycleMode=StartAppPool
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo START Application Pool for Web-Site:www.%webDomainName%
%msdeploy% -verb:sync -source:recycleApp -dest:recycleApp=www.%webDomainName%,recycleMode=StartAppPool
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

echo ################################
echo Deployment Complete
echo ################################


GOTO END

:ERROR

echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
echo !!! Stopping due to error    !!!
echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

echo ##teamcity[buildStatus status='FAILED' text='{build.status.text} failed to deploy on remote machine']
EXIT /B 1

:END
