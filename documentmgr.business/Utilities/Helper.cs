using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace documentmgr.business.Utilities
{
    public static class Helper
    {
        #region strings

        public static string Join(this IEnumerable<string> strings, string separator)
        {
            return string.Join(separator, strings);
        }

        public static bool IsNotEmpty(this string str)
        {
            return !str.IsEmpty();
        }

        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string Take(this string str, int count)
        {
            return new string(Enumerable.Take(str, count).ToArray());
        }

        public static string[] Split(this string str, string separator)
        {
            return str.Split(new string[] { separator }, StringSplitOptions.None);
        }

        public static string Mask(this string source, int start, int maskLength)
        {
            return source.Mask(start, maskLength, 'X');
        }

        public static string Mask(this string source, int start = 0, char maskCharacter = 'X')
        {
            return source.Mask(start, source.Length, maskCharacter);
        }

        public static string Mask(this string source, int start, int maskLength, char maskCharacter)
        {
            source = source.Trim();
            if (source.Length <= 0)
                return source;

            if (start > source.Length - 1)
            {
                throw new ArgumentException("Start position is greater than string length");
            }

            if (maskLength > source.Length)
            {
                throw new ArgumentException("Mask length is greater than string length");
            }

            if (start + maskLength > source.Length)
            {
                throw new ArgumentException("Start position and mask length imply more characters than are present");
            }

            string mask = new string(maskCharacter, maskLength);
            string unMaskStart = source.Substring(0, start);
            string unMaskEnd = source.Substring(start + maskLength, source.Length - maskLength);

            return unMaskStart + mask + unMaskEnd;
        }

        public static string GetModelStateErrors(ModelStateDictionary modelState)
        {
            return modelState.Keys.SelectMany(k => modelState[k].Errors).Select(m => m.ErrorMessage).Join("\n");
        }

        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static byte[] GetBytes(string input)
        {
            return System.Text.Encoding.UTF8.GetBytes(input);
        }


        #endregion

        #region path
        public static string PathCombine(string path1, string path2)
        {
            if (Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(Path.DirectorySeparatorChar);
                path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
            }

            return PathCombine(new[] { path1, path2 });
        }

        public static string SanitizePath(string path)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", path.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        }

        public static string PathCombine(string path1, string path2, string path3)
        {
            if (Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(Path.DirectorySeparatorChar);
                path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
            }
            if (Path.IsPathRooted(path3))
            {
                path3 = path3.TrimStart(Path.DirectorySeparatorChar);
                path3 = path3.TrimStart(Path.AltDirectorySeparatorChar);
            }

            return PathCombine(new[] { path1, path2, path3 });
        }

        public static string PathCombine(string path1, string path2, string path3, string path4)
        {
            if (Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(Path.DirectorySeparatorChar);
                path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
            }
            if (Path.IsPathRooted(path3))
            {
                path3 = path3.TrimStart(Path.DirectorySeparatorChar);
                path3 = path3.TrimStart(Path.AltDirectorySeparatorChar);
            }
            if (Path.IsPathRooted(path4))
            {
                path4 = path4.TrimStart(Path.DirectorySeparatorChar);
                path4 = path4.TrimStart(Path.AltDirectorySeparatorChar);
            }
            return PathCombine(new[] { path1, path2, path3, path4 });
        }

        private static string PathCombine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        #endregion


        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// 
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return !string.IsNullOrEmpty(request.Headers["X-Requested-With"]) &&
                    string.Equals(request.Headers["X-Requested-With"], "XmlHttpRequest", StringComparison.OrdinalIgnoreCase);

            return false;
        }

        public static int GeneralRandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="case">if case=1 then lowercase, if 2 then uppercase</param>
        /// <returns></returns>
        public static string GenerateRandomString(int size = 10, int @case = 1)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (@case == 1)
                return builder.ToString().ToLower();
            if (@case == 2)
                return builder.ToString().ToUpper();
            return builder.ToString();
        }
    }
}
