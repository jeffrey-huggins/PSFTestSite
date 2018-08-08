using System;

namespace AtriumWebApp.Models.Exceptions
{
    public class IFCException : Exception
    {
        public int TableMax { get; set; }

        public IFCException(int tableMax)
        {
            TableMax = tableMax;
        }
    }
}