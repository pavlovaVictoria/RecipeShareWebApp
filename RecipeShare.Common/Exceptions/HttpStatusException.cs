using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeShare.Common.Exceptions
{
    public class HttpStatusException : Exception
    {
        public int StatusCode { get; }

        public HttpStatusException(int statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
