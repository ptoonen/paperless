using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using iText.Kernel.Pdf;
using System.IO;
using System.Threading.Tasks;
using System.Text;

namespace paperlessProcessor
{
    public static class RemovePDFPassword
    {
        [FunctionName("RemovePDFPassword")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] 
            HttpRequest req, ILogger log)
        {
            // Get the key from KeyVault using Key Vault References
            string password = System.Environment.GetEnvironmentVariable("PDFPassword");

            // Initialize properties
            var properties = new ReaderProperties();
            properties.SetPassword(Encoding.ASCII.GetBytes(password));

            // Now open the PDF
            using (var outputStream = new MemoryStream())
            {
                PdfDocument pdf = new PdfDocument(
                    new PdfReader(req.Body, properties), 
                    new PdfWriter(outputStream));

                // Save the PDF with removed password  
                pdf.Close();

                // Return the PDF to output            
                return new FileContentResult(outputStream.ToArray(), "application/pdf");
            }
        }
    }
}
