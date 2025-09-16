using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLTemplateGenerator.Application
{
    public class Result<T>: Result
    {
        public T? Value { get; set; }

        public static Result<T> Success(T value, int statusCode = 200) => new Result<T> { Value = value, StatusCode = statusCode };
        public static Result<T> Failure(int statusCode, string errorMessage, T? value) => new Result<T> { StatusCode = statusCode, ErrorMessage = errorMessage, Value = value };
    }
}
