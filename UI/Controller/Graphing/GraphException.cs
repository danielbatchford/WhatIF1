namespace WhatIfF1.UI.Controller.Graphing
{
    public class GraphException : EventControllerException
    {
        public GraphException(string message) : base(message)
        {
        }

        public GraphException() : base()
        {
        }

        public GraphException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
