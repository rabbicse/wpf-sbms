using EkushApp.Logging;
using EkushApp.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                document.Add(new Chunk(Chunk.NEWLINE));
                var table = new PdfPTable(3);
                table.SetTotalWidth(new float[] { 25f, 50f, 25f });
                table.WidthPercentage = 100;
                table.AddCell(new Phrase("Category"));
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

        public static void CreateHardwareStatusReport(string fileName, IEnumerable<Hardware> hardwares)
        {
            try
            {
                Document document = new Document(PageSize.A4, 72, 72, 72, 72);
                PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None));
                document.Open();
                document.Add(new Paragraph(Element.ALIGN_CENTER, "Hardware Report", new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, Font.BOLD)));
                document.Add(new Chunk(Chunk.NEWLINE));
                var table = new PdfPTable(6);
                table.SetTotalWidth(new float[] { 20f, 20f, 20f, 20f, 20f, 20f });
                table.WidthPercentage = 100;
                table.AddCell(new Phrase("Category"));
                table.AddCell(new Phrase("Hardware Tag No."));
                table.AddCell(new Phrase("Model"));
                table.AddCell(new Phrase("Brand"));
                table.AddCell(new Phrase("Receive Date"));
                table.AddCell(new Phrase("Status"));
                foreach (var hw in hardwares)
                {
                    table.AddCell(new Phrase(hw.Category.ToString()));
                    table.AddCell(new Phrase(hw.HardwareTagNo));
                    table.AddCell(new Phrase(hw.Model));
                    table.AddCell(new Phrase(hw.BrandName));
                    table.AddCell(new Phrase(hw.ReceiveDate.Value.ToString("dd/mm/yyyy", CultureInfo.InvariantCulture) ?? string.Empty));
                    table.AddCell(new Phrase(hw.Status.ToString()));
                }
                document.Add(table);
                document.Close();
            }
            catch (Exception x)
            {
                Log.Error("Error when creating report.", x);
            }
        }

        public static void CreateUserStatusReport(string fileName, IEnumerable<UserReport> userReport)
        {
            try
            {
                Document document = new Document(PageSize.A4, 72, 72, 72, 72);
                PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None));
                document.Open();
                document.Add(new Paragraph(Element.ALIGN_CENTER, "User Report", new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, Font.BOLD)));
                document.Add(new Chunk(Chunk.NEWLINE));

                foreach (var report in userReport)
                {
                    var userTable = new PdfPTable(3);
                    userTable.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    userTable.WidthPercentage = 100;
                    userTable.DefaultCell.Border = Rectangle.NO_BORDER;
                    userTable.AddCell(new Phrase(report.UserName));
                    userTable.AddCell(new Phrase(report.Designation));
                    userTable.AddCell(new Phrase(report.Department));
                    document.Add(userTable);

                    var table = new PdfPTable(4);
                    table.WidthPercentage = 100;
                    table.AddCell(new Phrase("Hardware Category"));
                    table.AddCell(new Phrase("Hardware Tag No."));
                    table.AddCell(new Phrase("BrandName"));
                    table.AddCell(new Phrase("Model"));
                    foreach (var hw in report.Hardwares)
                    {
                        table.AddCell(new Phrase(hw.Category.ToString()));
                        table.AddCell(new Phrase(hw.HardwareTagNo));
                        table.AddCell(new Phrase(hw.Model));
                        table.AddCell(new Phrase(hw.BrandName));
                        //table.AddCell(new Phrase(hw.ReceiveDate.Value.ToString("dd/mm/yyyy", CultureInfo.InvariantCulture) ?? string.Empty));
                        //table.AddCell(new Phrase(hw.Status.ToString()));
                    }
                    document.Add(table);
                    document.Add(new Chunk(Chunk.NEWLINE));
                }
                document.Close();
            }
            catch (Exception x)
            {
                Log.Error("Error when creating report.", x);
            }
        }
    }
}
