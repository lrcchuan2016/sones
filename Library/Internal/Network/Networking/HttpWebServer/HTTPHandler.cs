/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/


#region usings
using System;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Collections.Generic;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Authentication;
using System.Diagnostics;

using sones.Networking.TCPSocket;
using sones.Library.LanguageExtensions;

#endregion

namespace sones.Networking.HTTP
{

    /// <summary>
    /// This handles incoming urls and maps them to a method of T
    /// </summary>
    /// <typeparam name="T">the instance</typeparam>
    public class HttpHandler<T> : ATCPSocketConnection
        where T : new()
    {

        #region Data

        private URLParser _Parser;

        private T _Instance;

        /// <summary>
        /// If true, the dispose will be invoked in the _Instance.
        /// This should NOT be done if a instance was passed with the ctor
        /// </summary>
        private Boolean _AllowDispose = false;

        public HTTPSecurity HTTPSecurity { get; set; }

        #endregion

        #region Constructors

        public HttpHandler()
        {

            _Parser = new URLParser(new[] { '/' });
            _Instance = new T();
            _AllowDispose = true;

            ParseInterface();

        }

        public HttpHandler(T instance)
        {

            _Parser = new URLParser(new[] { '/' });

            if (instance == null)
            {
                _Instance = new T();
                _AllowDispose = true;
            }

            else
            {
                _Instance = instance;
                _AllowDispose = false;
            }

            ParseInterface();

        }

        #endregion

        #region ParseInterface

        /// <summary>
        /// Parses the urls of the instance
        /// </summary>
        private void ParseInterface()
        {

            String WebAttribute = "";
            String WebMethod = "";

            #region Find the correct interface

            var _AllInterfaces = from _Interface
                                 in _Instance.GetType().GetInterfaces()
                                 where _Interface.GetCustomAttributes(typeof(ServiceContractAttribute), false).IsNotNullOrEmpty()
                                 select _Interface;

            var _Count = _AllInterfaces.Count();

            if (_Count < 1)
                throw new Exception("Could not find any valid interface having the ServiceContract attribute!");

            if (_Count > 1)
                throw new Exception("Multiple interfaces having the ServiceContract attribute!");

            var _CurrentInterface = _AllInterfaces.FirstOrDefault();

            if (_CurrentInterface == null)
                throw new Exception("Could not find any valid interface having the ServiceContract attribute!");

            #endregion

            #region Check global Force-/NoAuthenticationAttribute

            var _GlobalNeedsExplicitAuthentication = false;

            if (_CurrentInterface.GetCustomAttributes(typeof(NoAuthenticationAttribute), false).IsNotNullOrEmpty())
                _GlobalNeedsExplicitAuthentication = false;

            if (_CurrentInterface.GetCustomAttributes(typeof(ForceAuthenticationAttribute), false).IsNotNullOrEmpty())
                _GlobalNeedsExplicitAuthentication = true;

            #endregion

            Boolean? NeedsExplicitAuthentication = null;

            foreach (var Method in _CurrentInterface.GetMethods())
            {

                WebAttribute = "";
                NeedsExplicitAuthentication = _GlobalNeedsExplicitAuthentication;

                foreach (var _Attribute in Method.GetCustomAttributes(true))
                {

                    #region WebGet

                    var _WebAttribute = _Attribute as WebGetAttribute;
                    if (_WebAttribute != null)
                    {
                        WebAttribute = _WebAttribute.UriTemplate.ToLower();
                        WebMethod = "GET";
                        continue;
                    }

                    #endregion

                    #region WebInvoke

                    var _WebInvokeAttribute = _Attribute as WebInvokeAttribute;
                    if (_WebInvokeAttribute != null)
                    {
                        WebAttribute = _WebInvokeAttribute.UriTemplate.ToLower();
                        WebMethod = _WebInvokeAttribute.Method;
                        continue;
                    }

                    #endregion

                    #region NoAuthentication

                    var _NoAuthenticationAttribute = _Attribute as NoAuthenticationAttribute;
                    if (_NoAuthenticationAttribute != null)
                    {
                        NeedsExplicitAuthentication = false;
                        continue;
                    }

                    #endregion

                    #region ForceAuthentication

                    var _ForceAuthenticationAttribute = _Attribute as ForceAuthenticationAttribute;
                    if (_ForceAuthenticationAttribute != null)
                    {
                        NeedsExplicitAuthentication = true;
                        continue;
                    }

                    #endregion

                    #region NeedsAuthentication

                    var _NeedsAuthenticationAttribute = _Attribute as NeedsAuthenticationAttribute;
                    if (_NeedsAuthenticationAttribute != null)
                    {
                        NeedsExplicitAuthentication = _NeedsAuthenticationAttribute.NeedsAuthentication;
                        continue;
                    }

                    #endregion

                }

                _Parser.AddUrl(WebAttribute, Method, WebMethod, NeedsExplicitAuthentication);

            }

        }

        #endregion

        #region ATCPSocketConnection (InitializeSocket, ConnectionEstablished)

        /// <summary>
        /// initialize the socket
        /// </summary>
        /// <param name="myDataObject"></param>
        public override void InitializeSocket(Object myDataObject)
        {
        }

        private Stream GetStream(TcpClient myTcpClient)
        {
            if (HTTPSecurity != null)
            {
                if (HTTPSecurity.ServerCertificate != null)
                {
                    #if __MonoCS__
                    var sslStream = new SslStream(myTcpClient.GetStream(), false, HTTPSecurity.RemoteCertificateValidationCallback, HTTPSecurity.LocalCertificateSelectionCallback);
                    #else
                    var sslStream = new SslStream(myTcpClient.GetStream(), false, HTTPSecurity.RemoteCertificateValidationCallback, HTTPSecurity.LocalCertificateSelectionCallback, HTTPSecurity.EncryptionPolicy);
                    #endif

                     sslStream.AuthenticateAsServer(HTTPSecurity.ServerCertificate, HTTPSecurity.UseClientCertificate, SslProtocols.Default, false);

                    return sslStream;
                }
            }

            return myTcpClient.GetStream();
        }

        public override Boolean ConnectionEstablished()
        {

            //NetworkStream dataStream = null;
            using (var _DataStream = GetStream(TcpClientConnection))
            {

                try
                {
                    #region Wait until new StreamData is available (returns true), timeout or server shutdown

                    if (!WaitForStreamDataAvailable())
                        return false;

                    #endregion

                    #region Get header & body

                    HTTPHeader requestHeader = null;
                    Byte[] requestBody = null;
                    Byte[] responseBodyBytes = new byte[0];
                    Byte[] responseHeaderBytes = null;
                    HTTPContext _HTTPWebContext = null;
                    Exception _LastException = null;
                    var _HeaderErrors = GetHeaderAndBody(_DataStream, out requestHeader, out requestBody);

#if DEBUG
                    if (requestHeader.Destination == null)
                        return false;
                    if (requestHeader.Headers.AllKeys.Contains("SENDER"))
                        Debug.WriteLine(string.Format("{0} Retrieved a request to resource {1} from {2}", DateTime.Now.TimeOfDay, requestHeader.Destination, requestHeader.Headers["SENDER"]), "HttpHandler");
                    else
                        Debug.WriteLine(string.Format("{0} Retrieved a request to resource {1}", DateTime.Now.TimeOfDay, requestHeader.Destination), "HttpHandler");
#endif
                    if (requestHeader == null || requestBody == null)
                        return false;

                    #endregion

                    #region Trace

                    //System.Diagnostics.Trace.WriteLine("-------------------request started-------------------");
                    //System.Diagnostics.Trace.Indent();
                    //System.Diagnostics.Trace.WriteLine("requestHeader:");
                    //System.Diagnostics.Trace.Indent();
                    //System.Diagnostics.Trace.WriteLine(requestHeader.PlainHeader);
                    //System.Diagnostics.Trace.Unindent();
                    //System.Diagnostics.Trace.WriteLine("requestBody:");
                    //System.Diagnostics.Trace.Indent();
                    //System.Diagnostics.Trace.WriteLine(Encoding.UTF8.GetString(requestBody));
                    //System.Diagnostics.Trace.Unindent();
                    //System.Diagnostics.Trace.Unindent();
                    //System.Diagnostics.Trace.WriteLine("-----------------------------------------------------");

                    #endregion

                    #region Check if a error occurred during header processing...

                    HTTPHeader responseHeader = null;

                    if (requestHeader.HttpStatusCode != HTTPStatusCodes.OK)
                    {

                        responseHeader = new HTTPHeader()
                        {
                            HttpStatusCode = requestHeader.HttpStatusCode,
                        };

                        _HTTPWebContext = new HTTPContext(requestHeader, requestBody, responseHeader, _DataStream);
                        HTTPServer.HTTPContext = _HTTPWebContext;

                    }

                    #endregion

                    #region ... or process request and create a response header

                    else
                    {

                        responseHeader = new HTTPHeader()
                        {
                            HttpStatusCode = HTTPStatusCodes.OK,
                            ContentType = new ContentType("text/html")
                        };

                        #region Create and set HTTPContext

                        _HTTPWebContext = new HTTPContext(requestHeader, requestBody, responseHeader, _DataStream);
                        HTTPServer.HTTPContext = _HTTPWebContext;

                        #endregion

                        // Process request
                        try
                        {

                            // Get Callback
                            var parsedCallback = _Parser.GetCallback(_HTTPWebContext.RequestHeader.RawUrl, _HTTPWebContext.RequestHeader.HttpMethodString);

                            #region Check callback...

                            if (parsedCallback == null || parsedCallback.Item1 == null)
                            {

                                Debug.WriteLine("Could not find a valid handler for url: " + _HTTPWebContext.RequestHeader.RawUrl);
                                responseBodyBytes = Encoding.UTF8.GetBytes("Could not find a valid handler for url: " + _HTTPWebContext.RequestHeader.RawUrl);

                                _HTTPWebContext.ResponseHeader = new HTTPHeader()
                                {
                                    HttpStatusCode = HTTPStatusCodes.NotFound,
                                    ContentType = new ContentType("text/plain"),
                                    ContentLength = responseBodyBytes.ULongLength()
                                };

                                responseHeaderBytes = _HTTPWebContext.ResponseHeader.ToBytes();

                            }

                            #endregion

                            #region ...check authentication and invoke method callback

                            else
                            {

                                var authenticated = false;

                                #region Check HTTPSecurity

                                // the server switched on authentication AND the method does not explicit allow not authentication
                                if (HTTPSecurity != null && !(parsedCallback.Item1.NeedsExplicitAuthentication.HasValue && !parsedCallback.Item1.NeedsExplicitAuthentication.Value))
                                {

                                    #region Authentication

                                    //in this case the client is already been authenticated by his certificate
                                    if (HTTPSecurity.CredentialType == HttpClientCredentialType.Certificate)
                                    {
                                        authenticated = true;
                                    }

                                    if (HTTPSecurity.CredentialType == HttpClientCredentialType.Basic)
                                    {

                                        if (requestHeader.Authorization == null)
                                        {

                                            #region No authorisation info was sent

                                            responseHeader = GetAuthenticationRequiredHeader();
                                            responseHeaderBytes = responseHeader.ToBytes();

                                            #endregion

                                        }
                                        else if (!Authorize(_HTTPWebContext.RequestHeader.Authorization))
                                        {

                                            #region Authorization failed

                                            responseHeader = GetAuthenticationRequiredHeader();
                                            responseHeaderBytes = responseHeader.ToBytes();

                                            #endregion

                                        }
                                        else
                                        {
                                            authenticated = true;
                                        }

                                    }
                                    else
                                    {
                                        if (HTTPSecurity.CredentialType != HttpClientCredentialType.Certificate)
                                        {
                                            responseBodyBytes = Encoding.UTF8.GetBytes("Authentication other than Basic or Certificate currently not provided");
                                            responseHeader = new HTTPHeader() { HttpStatusCode = HTTPStatusCodes.InternalServerError, ContentLength = responseBodyBytes.ULongLength() };
                                            responseHeaderBytes = responseHeader.ToBytes();

                                            Debug.WriteLine("------------------------------------------------------------");
                                            Debug.WriteLine("!!!Authentication other than Basic or Certificate currently not provided!!!");
                                        }
                                    }

                                    #endregion

                                }

                                else if (parsedCallback.Item1.NeedsExplicitAuthentication.HasValue && parsedCallback.Item1.NeedsExplicitAuthentication.Value)
                                {

                                    #region The server does not have authentication but the Interface explicitly needs authentication

                                    responseBodyBytes = Encoding.UTF8.GetBytes("Authentication not provided from server");
                                    responseHeader = new HTTPHeader() { HttpStatusCode = HTTPStatusCodes.InternalServerError, ContentLength = responseBodyBytes.ULongLength() };
                                    responseHeaderBytes = responseHeader.ToBytes();

                                    #endregion

                                    Debug.WriteLine("---------------------------------------------");
                                    Debug.WriteLine("!!!Authentication not provided from server!!!");

                                }
                                else
                                    authenticated = true;

                                #endregion

                                if (authenticated)
                                    InvokeURL(parsedCallback, _HTTPWebContext, ref responseHeaderBytes, ref responseBodyBytes);

                            }

                            #endregion

                        }

                        #region Handle exceptions occurred during request processing

                        catch (Exception ex)
                        {

                            Debug.WriteLine(ex.ToString());

                            responseBodyBytes = Encoding.UTF8.GetBytes(ex.ToString());

                            responseHeader = new HTTPHeader()
                            {
                                HttpStatusCode = HTTPStatusCodes.InternalServerError,
                                ContentType = new ContentType("text/plain"),
                                ContentLength = responseBodyBytes.ULongLength()
                            };

                            responseHeaderBytes = responseHeader.ToBytes();

                            ExceptionThrowed(this, ex);

                            _LastException = ex;

                        }

                        #endregion

                    }

                    #endregion


                    #region Handle errors...

                    if ((Int32)responseHeader.HttpStatusCode >= 400 && (Int32)responseHeader.HttpStatusCode <= 599)
                    {

                        #region Handle custom error pages...

                        var _CustomErrorPage = _Instance as ICustomErrorPageHandler;

                        if (_CustomErrorPage != null)
                        {
                            responseBodyBytes = _CustomErrorPage.GetCustomErrorPage(responseHeader.HttpStatusCode, _HTTPWebContext.RequestHeader, _HTTPWebContext.RequestBody, _LastException);
                            responseHeader.ContentLength = responseBodyBytes.ULongLength();
                            responseHeaderBytes = responseHeader.ToBytes();
                        }

                        #endregion

                        #region ...or generate a generic errorpage!

                        else
                        {

                            var _StringBuilder = new StringBuilder();

                            _StringBuilder.AppendLine("<!DOCTYPE HTML PUBLIC \"-//IETF//DTD HTML 2.0//EN\">");
                            _StringBuilder.AppendLine("<html>");
                            _StringBuilder.AppendLine("  <head>");
                            _StringBuilder.Append("    <title>").Append((Int32)responseHeader.HttpStatusCode).Append(" ").Append(HTTPHeader.HttpStatusCodeToSimpleString(responseHeader.HttpStatusCode)).AppendLine("</title>");
                            _StringBuilder.AppendLine("  </head>");
                            _StringBuilder.AppendLine("  <body>");
                            _StringBuilder.Append("    <h1>Error ").Append((Int32)responseHeader.HttpStatusCode).Append(" - ").Append(HTTPHeader.HttpStatusCodeToSimpleString(responseHeader.HttpStatusCode)).AppendLine("</h1>");
                            _StringBuilder.AppendLine("    Your client sent a request which led to an error!<br />");
                            _StringBuilder.AppendLine("  </body>");
                            _StringBuilder.AppendLine("</html>");
                            _StringBuilder.AppendLine();

                            responseBodyBytes = Encoding.UTF8.GetBytes(_StringBuilder.ToString());
                            responseHeader.ContentLength = responseBodyBytes.ULongLength();
                            responseHeaderBytes = responseHeader.ToBytes();

                        }

                        #endregion

                    }

                    #endregion

                    #region Send Response

                    // Remove HttpWebContext
                    HTTPServer.HTTPContext = null;

                    if (!_HTTPWebContext.StreamDataAvailable)
                    {

                        // The user did not write into the stream itself - we will add header and the invocation result
                        var BytesToSend = new Byte[responseBodyBytes.Length + responseHeaderBytes.Length];
                        Array.Copy(responseHeaderBytes, BytesToSend, responseHeaderBytes.Length);
                        Array.Copy(responseBodyBytes, 0, BytesToSend, responseHeaderBytes.Length, responseBodyBytes.Length);

                        _DataStream.Write(BytesToSend, 0, BytesToSend.Length);

                    }

                    #endregion
                }
                finally
                {
                    if (_DataStream != null)
                    {
                        _DataStream.Close();
                    }
                    if (TcpClientConnection != null)
                    {
                        TcpClientConnection.Close();
                    }
                }
            }

            #region Trace

            //System.Diagnostics.Trace.WriteLine("-------------------response started-------------------");
            //System.Diagnostics.Trace.Indent();
            //System.Diagnostics.Trace.WriteLine("responseHeader:");
            //System.Diagnostics.Trace.Indent();
            //System.Diagnostics.Trace.WriteLine(responseHeader.ToString());
            //System.Diagnostics.Trace.Unindent();
            //System.Diagnostics.Trace.WriteLine("responseBody:");
            //System.Diagnostics.Trace.Indent();
            //System.Diagnostics.Trace.WriteLine(Encoding.UTF8.GetString(responseBodyBytes));
            //System.Diagnostics.Trace.Unindent();
            //System.Diagnostics.Trace.Unindent();
            //System.Diagnostics.Trace.WriteLine("-----------------------------------------------------");

            #endregion

            return true;

        }

        private void InvokeURL(Tuple<UrlNode, List<Object>> myParsedCallback, HTTPContext myHTTPContext, ref Byte[] myResponseHeaderBytes, ref Byte[] myResponseBodyBytes)
        {

            //var parsedCallback = _Parser.GetCallback(httpWebContext.RequestHeader.RawUrl, httpWebContext.RequestHeader.HttpMethodString);

            if (myParsedCallback == null || myParsedCallback.Item1 == null)
            {
                System.Diagnostics.Debug.WriteLine("Could not find a valid handler for url: " + myHTTPContext.RequestHeader.RawUrl);
                myResponseBodyBytes = Encoding.UTF8.GetBytes("Could not find a valid handler for url: " + myHTTPContext.RequestHeader.RawUrl);
                myHTTPContext.ResponseHeader = new HTTPHeader() { HttpStatusCode = HTTPStatusCodes.NotFound, ContentType = new System.Net.Mime.ContentType("text/plain"), ContentLength = myResponseBodyBytes.ULongLength() };
                myResponseHeaderBytes = myHTTPContext.ResponseHeader.ToBytes();
            }

            else
            {

                #region Invoke method to that URL and get bodybytes of the return value

                Object targetInvocationResult;
                if (myParsedCallback.Item2 == null)
                {
                    targetInvocationResult = myParsedCallback.Item1.Callback.Invoke(_Instance, null);
                }
                else
                {
                    targetInvocationResult = myParsedCallback.Item1.Callback.Invoke(_Instance, myParsedCallback.Item2.ToArray());
                }

                #endregion

                if (!myHTTPContext.StreamDataAvailable) // The user did not write into the stream itself - we will add header and the invocation result
                {
                    #region Get invocation result and create header and body

                    if (targetInvocationResult is Stream)
                    {
                        (targetInvocationResult as Stream).Seek(0, SeekOrigin.Begin);
                        myResponseBodyBytes = new Byte[(targetInvocationResult as Stream).Length];
                        (targetInvocationResult as Stream).Read(myResponseBodyBytes, 0, (int)(targetInvocationResult as Stream).Length);
                    }
                    else if (targetInvocationResult is String)
                    {

                        #region Bad hack for WCF
                        //    BodyBytes = Encoding.UTF8.GetBytes("<string>" + (string)(result.Key) + "</string>");
                        #endregion
                        myResponseBodyBytes = Encoding.UTF8.GetBytes((string)(targetInvocationResult));
                    }
                    else
                    {
                        // this was a void
                    }

                    myHTTPContext.ResponseHeader.ContentLength = myResponseBodyBytes.ULongLength();
                    myResponseHeaderBytes = myHTTPContext.ResponseHeader.ToBytes();

                    #endregion
                }

            }

        }

        #endregion

        #region Authentication

        public HTTPHeader GetAuthenticationRequiredHeader()
        {
            return new HTTPHeader()
            {
                HttpStatusCode = HTTPStatusCodes.Unauthorized
            };
        }

        public Boolean Authorize(HTTPCredentials myHTTPCredentials)
        {

            try
            {
                HTTPSecurity.UserNamePasswordValidator.Validate(myHTTPCredentials.Username, myHTTPCredentials.Password);
                return true;
            }

            catch (SecurityTokenException ste)
            {
                Debug.WriteLine("Authorize failed with " + ste.ToString());
                return false;
            }

        }

        #endregion

        #region Dispose

        public override void Dispose()
        {
            if (_AllowDispose)
            {
                _Parser = null;
                if (_Instance is IDisposable)
                {
                    ((IDisposable)_Instance).Dispose();
                }
            }
        }

        #endregion

    }
}
