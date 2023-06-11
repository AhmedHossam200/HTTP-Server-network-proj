using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            FileStream sw1 = new FileStream(@"C:\Users\Ahmed M. Farid\Desktop\Networks Project Template\Template[2021-2022]\HTTPServer\bin\Debug\logger.txt"
                            , FileMode.OpenOrCreate);
            StreamWriter sw2 = new StreamWriter(sw1);
            //Datetime:
            sw2.WriteLine("Date: " + DateTime.Now.ToString());
            //message:
            sw2.WriteLine("Exception: " + ex.Message);
            // for each exception write its details associated with datetime 
            sw2.Close();
        }
    }
}
