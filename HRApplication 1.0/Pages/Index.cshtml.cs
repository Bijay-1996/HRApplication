using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using HRApplication_1._0.MlModel;
using static System.Net.WebRequestMethods;

namespace HRApplication_1._0.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public IndexModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [BindProperty]
        public string PdfUrl { get; set; } = null;
        public string[] filelist { get; set; }
       
        public void OnGet()
        {
            
        }
        public async Task<IActionResult> OnPost(List<IFormFile> files)
        {
            List<DataModelClass.Profile> Profiles = new List<DataModelClass.Profile>();
            string appDir = Path.GetTempPath();
            string filePath = "";
            //var file = Request.Form.Files["pdfFile"];
            foreach (var temp in files)
            {
                if (temp != null && temp.Length > 0)
                {

                    Stream blog = new MemoryStream();
                    blog = temp.OpenReadStream();
                    byte[] resultByteArray = null;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        blog.CopyTo(memoryStream);
                        resultByteArray = memoryStream.ToArray();
                    }

                    filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
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
                }
                PdfUrl = Url.Content(filePath);
                DataModelClass.Profile ob = new DataModelClass.Profile();
                ob.Name = Guid.NewGuid().ToString();
                ob.Description = Guid.NewGuid().ToString();
                ob.CV = PdfUrl;
                Profiles.Add(ob);
            }
            MLFunctions.InputMlModel(Profiles);
            return Page();
        }
        
    }
}