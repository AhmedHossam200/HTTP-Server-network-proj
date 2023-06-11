using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace HTTPServer
{
    class Program
    {
         
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            string filePath = @"C:\Users\Ahmed M. Farid\Desktop\Networks Project Template\Template[2021-2022]\HTTPServer\bin\Debug\redirectionRules.txt";
            //Start server

            // 1) Make server object on port 1000
            Server ser1 = new Server(1000, filePath);
            // 2) Start Server

            ser1.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            FileStream r = new FileStream(@"C:\Users\Ahmed M. Farid\Desktop\Networks Project Template\Template[2021-2022]\HTTPServer\bin\Debug\redirectionRules.txt", FileMode.OpenOrCreate);
            StreamWriter r2 = new StreamWriter(r);

            r2.WriteLine(@"aboutus.html,aboutus2.html");
            r2.Close();
        }
    }
}
