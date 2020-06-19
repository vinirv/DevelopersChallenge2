using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;

namespace Common
{
    public class Functions
    {
        public static string IFormFileToString(IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }

            return result.ToString();
        }
    }
}
