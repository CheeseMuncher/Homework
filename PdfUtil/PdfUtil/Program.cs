using System;
using System.Linq;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using iText.IO;

namespace PdfUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var input = "/home/tom/Downloads/Test2.pdf";
            var temp  = "/home/tom/Downloads/Temp.pdf";
            var output = "/home/tom/Downloads/Output.pdf";
            using (var doc = new PdfDocument(new PdfReader(input)))
            {
                var page = doc.GetFirstPage();
                var size = page.GetPageSize();
                Console.WriteLine(size.GetX().ToString());
                Console.WriteLine(size.GetY().ToString());
                Console.WriteLine(size.GetWidth().ToString());
                Console.WriteLine(size.GetHeight().ToString());
                // Document document = new Document(doc);
                // //var imageData = ImageDataFactory.Create(input);
                // var page = doc.GetFirstPage();
                // Image image = new Image(page).ScaleAbsolute(100, 200).SetFixedPosition(1, 25, 25);
                // PdfDocument pdf = new PdfDocument(new PdfWriter(output));
                // PdfFormXObject pageCopy = page.copyAsFormXObject(pdf);
                // Image image = new Image(pageCopy);
                // // This adds the image to the page
                // document.Add(image);
                // document.Close();
            }
        }

            // Console.WriteLine("Hello World!");
            // var files = Directory.GetFiles("/home/tom/Downloads");
            // foreach (var file in files.Where(f => f.EndsWith(".pdf")))
            // {
            //     //Console.WriteLine(file);
            // }
            // var input = "/home/tom/Downloads/Order.pdf";
            // var output = "/home/tom/Downloads/Output.pdf";
            // using (var doc = new PdfDocument(new PdfReader(input)))
            // {
            //     var count = doc.GetNumberOfPages();
            //     var page = doc.GetFirstPage();
            //     var size = page.GetPageSize();
            //     Console.WriteLine(count);
            //     Console.WriteLine(size.GetX().ToString());
            //     Console.WriteLine(size.GetY().ToString());
            //     Console.WriteLine(size.GetWidth().ToString());
            //     Console.WriteLine(size.GetHeight().ToString());
            //     var height = size.GetHeight();
            //     var width = size.GetWidth();
            //     size.SetHeight(width);
            //     size.SetWidth(height);
            //     var imageData = ImageDataFactory.Create(FileToByte(input));

            //     // PdfDictionary obj = (PdfDictionary)doc.GetPdfObject(1);
            //     // var imageData = ImageDataFactory.Create(FileToByte(input));
            //     // if (obj != null && obj.IsStream())
            //     // {
            //     //     Console.WriteLine("CastOK");
            //     //     var pd = (PdfDictionary)obj;
            //     //     string filter = pd.Get(PdfName.Filter).ToString();
            //     //     Console.WriteLine(filter);
            //     // }
            //     using (var write = new PdfDocument(new PdfWriter(output)))
            //     {
            //         //var pageCopy = page.copyAsFormXObject(write);
            //         var image = new Image(pageCopy);
            //         image.SetBorder(Border.NO_BORDER);
            //         var document = new Document(write);
            //         document.Add(image);
            //         //var newPage = write.AddNewPage(new PageSize(height, width));
                    
            //         // Console.WriteLine(write.GetNumberOfPages());
            //         // Console.WriteLine(write.GetNumberOfPages());
            //         // page.CopyTo(write);
            //         // Console.WriteLine(write.GetNumberOfPages());
            //         write.Close();
            //     }
            // }

    }
}
