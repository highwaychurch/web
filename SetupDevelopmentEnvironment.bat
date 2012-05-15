ECHO OFF

SET appcmd="%ProgramFiles(x86)%\IIS Express\appcmd.exe"
SET environment=devlocal
SET domain=%environment%.highway.com.au

ECHO Using appcmd: %appcmd%
ECHO Using environment: %environment%
ECHO Using domain: %domain%

ECHO
ECHO ################################
ECHO Installing SSL and Signing Certficate
ECHO ################################
certutil -f -p highwaychristianchurch -importPFX ".\Certificates\devlocal.highway.com.au Wildcard Certificate.pfx"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
powershell -command .\DeploymentScripts\Utils\GrantAccessToPrivateKey.ps1 b5566e7c9b7d1963d643d5b6bc980f9122215c8a %username%
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

ECHO
ECHO ################################
ECHO Configuring IIS Express
ECHO ################################

%appcmd% delete site /site.name:Highway.Web
%appcmd% add site /site.name:Highway.Web
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% add app /site.name:Highway.Web /path:"/" /physicalPath:"%~dp0Highway.Web"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:Highway.Web /+bindings.[protocol='http',bindingInformation='*:9500:localhost']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:Highway.Web /+bindings.[protocol='http',bindingInformation='*:80:www.%domain%']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:Highway.Web /+bindings.[protocol='https',bindingInformation='*:443:www.%domain%']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

%appcmd% delete site /site.name:Raven.Web
%appcmd% add site /site.name:Raven.Web
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% add app /site.name:Raven.Web /path:"/" /physicalPath:"%~dp0Raven.Web"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:Raven.Web /+bindings.[protocol='http',bindingInformation='*:9501:localhost']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:Raven.Web /+bindings.[protocol='http',bindingInformation='*:80:ravendb.%domain%']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:Raven.Web /+bindings.[protocol='https',bindingInformation='*:443:ravendb.%domain%']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

%appcmd% delete site /site.name:Identity.Web
%appcmd% add site /site.name:Identity.Web
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% add app /site.name:Identity.Web /path:"/" /physicalPath:"%~dp0Identity.Web"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:Identity.Web /+bindings.[protocol='http',bindingInformation='*:9502:localhost']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:Identity.Web /+bindings.[protocol='http',bindingInformation='*:80:id.%domain%']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:Identity.Web /+bindings.[protocol='https',bindingInformation='*:443:id.%domain%']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

%appcmd% delete site /site.name:Creative.Web
%appcmd% add site /site.name:Creative.Web
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% add app /site.name:Creative.Web /path:"/" /physicalPath:"%~dp0Creative.Web"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:Creative.Web /+bindings.[protocol='http',bindingInformation='*:9503:localhost']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:Creative.Web /+bindings.[protocol='http',bindingInformation='*:80:creative.%domain%']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:Creative.Web /+bindings.[protocol='https',bindingInformation='*:443:creative.%domain%']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

%appcmd% delete site /site.name:F1PCO.Web
%appcmd% add site /site.name:F1PCO.Web
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% add app /site.name:F1PCO.Web /path:"/" /physicalPath:"%~dp0F1PCO.Web"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:F1PCO.Web /+bindings.[protocol='http',bindingInformation='*:9506:localhost']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:F1PCO.Web /+bindings.[protocol='http',bindingInformation='*:80:f1pco.%domain%']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
%appcmd% set site /site.name:F1PCO.Web /+bindings.[protocol='https',bindingInformation='*:443:f1pco.%domain%']
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

ECHO
ECHO ################################
ECHO Configuring HTTP.SYS
ECHO ################################

ECHO --------------------------------
ECHO www.%domain%
ECHO --------------------------------
netsh http delete urlacl url=http://www.%domain%:80/
netsh http add urlacl url=http://www.%domain%:80/ user=everyone
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
netsh http delete urlacl url=https://www.%domain%:443/
netsh http add urlacl url=https://www.%domain%:443/ user=everyone
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

ECHO --------------------------------
ECHO ravendb.%domain%
ECHO --------------------------------
netsh http delete urlacl url=http://ravendb.%domain%:80/
netsh http add urlacl url=http://ravendb.%domain%:80/ user=everyone
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
netsh http delete urlacl url=https://ravendb.%domain%:443/
netsh http add urlacl url=https://ravendb.%domain%:443/ user=everyone
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

ECHO --------------------------------
ECHO id.%domain%
ECHO --------------------------------
netsh http delete urlacl url=http://id.%domain%:80/
netsh http add urlacl url=http://id.%domain%:80/ user=everyone
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
netsh http delete urlacl url=https://id.%domain%:443/
netsh http add urlacl url=https://id.%domain%:443/ user=everyone
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

ECHO --------------------------------
ECHO creative.%domain%
ECHO --------------------------------
netsh http delete urlacl url=http://creative.%domain%:80/
netsh http add urlacl url=http://creative.%domain%:80/ user=everyone
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
netsh http delete urlacl url=https://creative.%domain%:443/
netsh http add urlacl url=https://creative.%domain%:443/ user=everyone
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

ECHO --------------------------------
ECHO f1pco.%domain%
ECHO --------------------------------
netsh http delete urlacl url=http://f1pco.%domain%:80/
netsh http add urlacl url=http://f1pco.%domain%:80/ user=everyone
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
netsh http delete urlacl url=https://f1pco.%domain%:443/
netsh http add urlacl url=https://f1pco.%domain%:443/ user=everyone
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

ECHO --------------------------------
ECHO SSL Certificate for %domain%
ECHO --------------------------------
netsh http delete sslcert ipport=0.0.0.0:443
netsh http add sslcert ipport=0.0.0.0:443 appid={BF01D84D-31ED-4651-9079-731C1C3F8D5E} certhash=b5566e7c9b7d1963d643d5b6bc980f9122215c8a
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

GOTO END

:ERROR

echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
echo !!! Stopping due to error    !!!
echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

:END