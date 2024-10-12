using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Xml.Linq;

public class ReportGenerator
{
    public static byte[] GeneratePdf(IEnumerable<ReportItem> reportData)
    {
        using (var stream = new MemoryStream())
        {
            var document = new Document(PageSize.A4);
            PdfWriter.GetInstance(document, stream);
            document.Open();

           
            document.Add(new Paragraph("Report", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20)));

            
            foreach (var item in reportData)
            {
                document.Add(new Paragraph($"Task: {item.TaskName}, Due Date: {item.DueDate}"));
            }

            document.Close();
            return stream.ToArray();
        }
    }
}

public class ReportItem
{
    public string TaskName { get; set; }
    public DateTime DueDate { get; set; }
}
