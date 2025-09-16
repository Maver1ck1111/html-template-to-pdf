using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLTemplateGenerator.Application
{
    public class Result
    {
        public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }

        public static Result Success(int statusCode = 200) => new Result { StatusCode = statusCode };
    }
}
