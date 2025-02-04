using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PensionSystem.Application.DTOS;

namespace PensionSystem.Application.Helpers
{
    public static class ResponseBuilder
    {
        public static GlobalResponse<T> BuildResponse<T>(int status, string statusText, ModelStateDictionary errs, T data)
        {
            var listOfErrorItems = new List<ErrorItemModel>();
            var benchMark = new List<string>();

            if (errs != null)
            {
                foreach (var err in errs)
                {
                    ///err.error.errors
                    var key = err.Key;
                    var errValues = err.Value;
                    var errList = new List<string>();
                    foreach (var errItem in errValues.Errors)
                    {
                        errList.Add(errItem.ErrorMessage);
                        if (!benchMark.Contains(key))
                        {
                            listOfErrorItems.Add(new ErrorItemModel { Key = key, ErrorMessages = errList });
                            benchMark.Add(key);
                        }
                    }
                }
            }

            var response = new GlobalResponse<T>
            {
                Data = data,
                Errors = listOfErrorItems,
                Status = status,
                StatusText = statusText,
            };

            return response;
        }
    }
}
