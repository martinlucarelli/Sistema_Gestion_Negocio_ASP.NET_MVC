using System.Security.Cryptography;
using System.Text;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Services
{
    public class BashClaveService
    {


        public string ConvertirSha256(string texto)
        {
            if(texto == null)
            {
                return " ";
            }

            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                {
                    Sb.Append(b.ToString("x2"));
                }

            }
            return Sb.ToString();
        }




    }
}
