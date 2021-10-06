using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASUTGen.Devices
{
    /// <summary>
    /// 
    /// </summary>
    public class ExternalMessageBuilder
    {
        private string ConvertToHex(string value)
        {
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string BuildChecksum(string data)
        {
            return "";
 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public string BuildWriteMessage(string address, string command)
        {
            string checksum = BuildChecksum(address + command);

            return command + "\r";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="numberOfBytes"></param>
        /// <returns></returns>
        public string BuildReadMessage(string address, int numberOfBytes)
        {

            return "";
        }
    }
}
