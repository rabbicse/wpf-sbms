using EkushApp.Logging;
using EkushApp.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMS.Reports
{
    public class ReportGenerator
    {
        public static void CreateHardwareReport(string fileName, IEnumerable<HardwareReport> hardwares)
        {
            try
            {
                Document document = new Document(PageSize.A4, 72, 72, 72, 72);
                PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None));
                document.Open();
                document.Add(new Paragraph(Element.ALIGN_CENTER, "Hardware Report", new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, Font.BOLD)));
                var table = new PdfPTable(3);
                table.SetTotalWidth(new float[] { 25f, 50f, 25f });
                table.WidthPercentage = 100;
                table.AddCell(new Phrase("Hardware Category"));
                table.AddCell(new Phrase("Hardware Model/Type"));
                table.AddCell(new Phrase("Total"));
                foreach (var hw in hardwares)
                {
                    table.AddCell(new Phrase(hw.Category));
                    table.AddCell(new Phrase(hw.Model));
                    table.AddCell(new Phrase(hw.Count));
                }
                document.Add(table);
                document.Close();
            }
            catch (Exception x)
            {
                Log.Error("Error when creating report.", x);
            }
        }
    }
}
