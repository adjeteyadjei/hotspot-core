using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

namespace Hotvenues.Helpers
{
    public class ExceptionHelper
    {
        private static string ProcessSqlExceptionMessage(string message)
        {
            if (message.Contains("The INSERT statement conflicted with the FOREIGN KEY constraint"))
            {
                var modelName = message.Substring(message.IndexOf("table \""))?
                    .Split(',')?.FirstOrDefault()?.Split('.')?.LastOrDefault()?.Trim('"') ?? "data";
                return $"The {modelName.Singularize()} you are referencing doesn't exist.";
            }


            if (message.Contains("unique index"))
                return "Operation failed. Data is constrained to be unique.";

            if (message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                return "This record is referenced by other records hence can not be deleted.";

            return message;
        }

        public static string ProcessException(ModelStateDictionary modelState)
        {
            var msg = modelState.Values.SelectMany(s => s.Errors)?
                .Select(e => e.ErrorMessage).Aggregate((a, b) => $"{a}\n{b}");
            return msg;
        }

        public static string ProcessException(IdentityResult identityResult)
        {
            var msg = identityResult.Errors.Select(x => x.Description).Aggregate("", (current, error) => current + error + "\n");
            return msg;
        }

        public static string ProcessException(Exception exception)
        {
            var msg = ErrorMsg(exception);
            return msg;
        }

        private static string ErrorMsg(Exception exception)
        {
            var msg = exception.Message;
            if (exception.InnerException == null) return ProcessSqlExceptionMessage(msg);
            msg = exception.InnerException.Message;

            if (exception.InnerException.InnerException == null) return ProcessSqlExceptionMessage(msg);
            msg = exception.InnerException.InnerException.Message;

            if (exception.InnerException.InnerException.InnerException != null)
            {
                msg = exception.InnerException.InnerException.InnerException.Message;
            }

            return ProcessSqlExceptionMessage(msg);
        }

    }
}
