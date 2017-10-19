rem "C:\Program Files\Microsoft.NET\SDK\v2.0 64bit\Bin\soapsuds" -url:http://localhost:8001/ILNET?wsdl -oa:ServerProxy.dll
rem "c:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\soapsuds" -url:http://localhost:8001/ILNET?wsdl -oa:ServerProxy.dll

soapsuds -url:http://localhost:8001/ILNET?wsdl -oa:ServerProxy.dll

copy ServerProxy.dll Servers\WebService\bin\ /y

del ServerProxy.pdb
