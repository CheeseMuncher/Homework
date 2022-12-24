// namespace PdfUtil;

// public class Snippet
// {
//     [STAThread]
//     private static void Main(string[] args)
//     {
//         string directory = AppDomain.CurrentDomain.BaseDirectory;
        
//         OpenFileDialog ofd = new OpenFileDialog();
//         ofd.InitialDirectory = directory;
//         ofd.DefaultExt = ".pdf";
//         ofd.Filter = "PDF Files(.*PDF)|*.PDF|All Files(*.*)|*.*";
//         string inputPdfFilePath;
//         if (ofd.ShowDialog() == true)
//         {
//             inputPdfFilePath = ofd.FileName.ToString();
//             string temporaryFilePath = string.Empty;
//             string pdfInputsDirectoryPath = System.IO.Path.GetDirectoryName(inputPdfFilePath);

//             using (Stream inputPdfStream = new FileStream(inputPdfFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
//             {
//                 PdfReader pdfReader = new PdfReader(inputPdfStream);


//                 #region Create temporary file to rotate page without content rotation and shrink content
//                 //this part of code is created because without saving rotated to Landscape orientatio pdf file and reading it again
//                 //into pdf stream didn't find other solution which set content in Portrait orienation page into required horizontal
//                 //position

//                 string temporaryFileName = "Temporary_" + Path.GetFileNameWithoutExtension(inputPdfFilePath) +
//                             Path.GetExtension(inputPdfFilePath);
//                 temporaryFilePath = System.IO.Path.Combine(pdfInputsDirectoryPath, temporaryFileName);
//                 using (Stream outputPdfStream =
//                     new FileStream(temporaryFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
//                 {
//                     PdfStamper stamper = new PdfStamper(pdfReader, outputPdfStream);

//                     int pagesAmount = pdfReader.NumberOfPages;

//                     for (int pageNumber = 1; pageNumber <= pagesAmount; pageNumber++)
//                     {
//                         //This code rotates all page 
//                         PdfDictionary page = pdfReader.GetPageN(pageNumber);
//                         PdfNumber rotate = page.GetAsNumber(PdfName.ROTATE);

//                         int rotation = rotate == null ? 270 : (rotate.IntValue + 270) % 360;
//                         stamper.RotateContents = true;//this line leave content of page not rotated
//                         page.Put(PdfName.ROTATE, new PdfNumber(rotation));
//                         stamper.GetUnderContent(pageNumber).SetLiteral("\nq 0.53 0 0 0.53 10 25 cm\nq\n");
                        
//                     }
//                     stamper.Close();
//                 }

//                 #endregion Create temporary file to rotate page without content rotation and shrink content
//             }

//             #region Create new  pdfReader and load into it data from temporary file and then rotate pages to required position

//             using (Stream inputPdfStream = new FileStream(temporaryFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
//             {
//                 PdfReader pdfReader = new PdfReader(inputPdfStream);
//                 File.Delete(temporaryFilePath);
//                 string resultFileName = "Output_" + Path.GetFileNameWithoutExtension(inputPdfFilePath) +
//                     Path.GetExtension(inputPdfFilePath);
//                 string resultFilePath = System.IO.Path.Combine(pdfInputsDirectoryPath, resultFileName);
//                 using (Stream outputPdfStream =
//                     new FileStream(resultFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
//                 {
//                     PdfStamper stamper = new PdfStamper(pdfReader, outputPdfStream);
//                     int pagesAmount = pdfReader.NumberOfPages;

//                     for (int pageNumber = 1; pageNumber <= pagesAmount; pageNumber++)
//                     {
//                         //This code rotates all page 
//                         PdfDictionary page = pdfReader.GetPageN(pageNumber);
//                         PdfNumber rotate = page.GetAsNumber(PdfName.ROTATE);
//                         stamper.RotateContents = true;//this line leave content of page not rotated

//                         int rotation = rotate == null ? 90 : (rotate.IntValue + 90) % 360;
//                         page.Put(PdfName.ROTATE, new PdfNumber(rotation));
//                         //after the above code line execution the initial content of pdf document is in the correct position 

//                     }
//                     stamper.Close();
//                 }
//             }

//             #endregion Create new  pdfReader and load into it data from temporary file and then rotate pages to required position

//         }
//     }
// }
