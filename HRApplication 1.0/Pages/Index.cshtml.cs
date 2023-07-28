using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace HRApplication_1._0.Pages
{
    public class IndexModel : PageModel
    {
        //private readonly ILogger<IndexModel> _logger;

        //public IndexModel(ILogger<IndexModel> logger)
        //{
        //    _logger = logger;
        //}
        private readonly IWebHostEnvironment _webHostEnvironment;
        public IndexModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [BindProperty]
        public string PdfUrl { get; set; } = null;
        public void OnGet()
        {
            
        }
        public IActionResult OnPost()
        {
            string appDir=Path.GetTempPath();
            var file = Request.Form.Files["pdfFile"];
            if (file != null && file.Length > 0)
            {
                Stream blog = new MemoryStream();
                blog = file.OpenReadStream();
                byte[] resultByteArray = null;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    blog.CopyTo(memoryStream);
                    resultByteArray= memoryStream.ToArray();
                }
                string filePath = "";
                filePath = Path.Combine(Path.GetTempPath(), "fileImage.pdf");
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream(resultByteArray))
                    {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            memoryStream.WriteTo(fileStream);
                        }
                    }

                    Console.WriteLine("PDF file created successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There Is SomeThing Wrong");
                }
                PdfUrl = Url.Content(filePath);
            }

            return Page();
        }
    }
}