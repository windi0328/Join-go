using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinGo.Models.Author
{
    public class AppSessionExpireException : Exception
    {
        public string ErrorMessage
        {
            get { return base.Message.ToString(); }
        }

        public AppSessionExpireException(string errorMessage) : base(errorMessage) { }

        public AppSessionExpireException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) { }
    }
}