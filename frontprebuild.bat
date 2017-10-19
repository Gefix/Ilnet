rem "C:\Program Files\Microsoft.NET\SDK\v2.0 64bit\Bin\soapsuds"
wsdl /out:WebServiceProxy.cs http://localhost/ILNET/ILNET.asmx?WSDL
fart WebServiceProxy.cs "public WebService() {" "public WebService(string URL) {"
fart WebServiceProxy.cs "this.Url = \"http://localhost/ILNET/ILNET.asmx\"" "this.Url = URL"
"c:\Windows\Microsoft.NET\Framework\v3.5\csc.exe" /target:library WebServiceProxy.cs
copy WebServiceProxy.dll Clients\WindowsClient\ /y
del WebServiceProxy.cs
