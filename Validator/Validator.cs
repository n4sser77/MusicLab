
using System.Text.RegularExpressions;


namespace ValidatorLib
{
    public static class Validator
    {
        public static bool ValidateEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            var emailRegex = new Regex("^\\S+@\\S+\\.\\S+$");



            if (emailRegex.IsMatch(input))
            {

                return true;
            }

            return false;


            ;

        }
    }
}