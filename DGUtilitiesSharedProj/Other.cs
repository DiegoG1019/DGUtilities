using System.Security;

namespace DiegoG.Utilities
{
    public static class Other
    {
        public static SecureString ToSecureString(this string plainString)
        {
            if (plainString == null)
            {
                return null;
            }

            SecureString secureString = new SecureString();
            foreach (char c in plainString.ToCharArray())
            {
                secureString.AppendChar(c);
            }
            return secureString;
        }
    }
}
