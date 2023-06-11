using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);

            //TODO: initialize this.serverSocket
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 1000);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(ipep);

        }

        public void StartServer()
        {
            Console.WriteLine("Start Listening....");
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(500);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientsocket = serverSocket.Accept();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(clientsocket);
            }
        }

        public void HandleConnection(object obj)
        {
            Console.WriteLine("Conection Accepted");   
            // TODO: Create client socket 
            Socket newclient = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            newclient.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] msg = new byte[65536];
                    int msglen = newclient.Receive(msg);
                    String msgstr = Encoding.ASCII.GetString(msg);

                    Console.WriteLine(msgstr);
                    // TODO: break the while loop if receivedLen==0
                    if (msglen == 0)
                    {
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Request req = new Request(msgstr);
                    // TODO: Call HandleRequest Method that returns the response
                    Response serverrsponse = HandleRequest(req);
                    string res = serverrsponse.ResponseString;

                    byte[] response = Encoding.ASCII.GetBytes(res);
                    // TODO: Send Response back to client
                    newclient.Send(response);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                    
                }
            }
            // TODO: close client socket
            newclient.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string content="";
            string code = "";
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest()) 
                {
                    code = "400";
                    content = "<!DOCTYPE html>< html >< body >< h1 > 400 Bad Request</ h1 >< p > 400 Bad Request</ p ></ body ></ html >";
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string[] name = request.relativeURI.Split('/');
                string ph_path = Configuration.RootPath + '\\' + name[1];
                //TODO: check for redirect
                for(int i=0;i<Configuration.RedirectionRules.Count;i++)
                {
                    if('/'+Configuration.RedirectionRules.Keys.ElementAt(i).ToString()==request.relativeURI)
                    {
                        code = "301";
                        request.relativeURI = '/' + Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                        name[1] = Configuration.RedirectionRules.Values.ElementAt(i).ToString();

                        ph_path = Configuration.RootPath + '\\' + name[1];
                        content = File.ReadAllText(ph_path);
                        string location = "http://localhost:1000/" + name[1];
                        Response res = new Response(code, "text/html", content, location);
                        
                        return res;
                    }
                }

                //TODO: check file exists
                if (!File.Exists(ph_path))
                {
                    ph_path = Configuration.RootPath + '\\' + "NotFound.html";
                    code = "404";
                    content = File.ReadAllText(ph_path);
                }
                //TODO: read the physical file
                else
                {
                    content = File.ReadAllText(ph_path);
                    code = "200";
                }
                // Create OK response
                Response re = new Response(code, "text/html", content, ph_path);
                return re;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                String ph_path = Configuration.RootPath +'\\'+ "InternalError.html";
                code = "500";
                content = File.ReadAllText(ph_path);
                Response re = new Response(code, "text/html", content, ph_path);
                return re;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            for (int i = 0; i < Configuration.RedirectionRules.Count; i++) 
            {
                if (relativePath == '/' + Configuration.RedirectionRules.Keys.ElementAt(i).ToString())
                {
                    String redpath = Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                    String phypath = Configuration.RootPath + '\\' + redpath;
                    return phypath;
                }
            }

            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            string content = "";
            try
            {
                // TODO: check if filepath not exist log exception using Logger class and return empty string
                if (File.Exists(filePath))
                {
                    content = File.ReadAllText(filePath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            // else read file and return its content
            return content;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                FileStream sr1 = new FileStream(filePath, FileMode.Open);
                StreamReader sr2 = new StreamReader(sr1);
                // then fill Configuration.RedirectionRules dictionary 
                while (sr2.Peek() != -1)
                {
                    string line = sr2.ReadLine();
                    string[] msg = line.Split(',');
                    if (msg[0] == "")
                    {
                        break; 
                    }
                    Configuration.RedirectionRules.Add(msg[0], msg[1]);
                }
                sr1.Close();
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}