using Hotvenues.Services;
using Syncfusion.Drawing;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hotvenues.Helpers
{
    public class PdfReportConfig
    {
        public string Content { get; set; }
        public string RootPath { get; set; }
        public bool Landscape { get; set; }
        public string Watermark { get; set; }
        public SizeF PageSize { get; set; }
    }

    public static class PdfReporter
    {
        public static byte[] Generate(string content, string rootPath, bool landscape = false)
        {
            var document = BuildDocument(content, rootPath, PdfPageSize.A4, landscape);

            var stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;
            document.Close(true);

            return stream.ToArray();
        }

        public static byte[] MergeDocuments(List<byte[]> streams)
        {
            PdfDocument mergedDocuments = new PdfDocument();
            var docs = streams.Select(x => new MemoryStream(x)).ToArray();
            PdfDocumentBase.Merge(mergedDocuments, docs);


            var stream = new MemoryStream();
            mergedDocuments.Save(stream);
            stream.Position = 0;
            mergedDocuments.Close(true);

            return stream.ToArray();
        }

        public static byte[] Generate(PdfReportConfig config)
        {
            var document = BuildDocument(config.Content, config.RootPath, config.PageSize, config.Landscape);
            if (!string.IsNullOrEmpty(config.Watermark)) Watermark(document, config.Watermark);

            var stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;
            document.Close(true);

            return stream.ToArray();
        }

        private static PdfDocument BuildDocument(string content, string rootPath, SizeF size, bool landscape = false)
        {
            var htmlConverter = new HtmlToPdfConverter
            {
                ConverterSettings = new WebKitConverterSettings
                {
                    WebKitPath = Path.Combine(rootPath, "QtBinariesWindows"),
                    PdfPageSize = size,
                    Orientation = landscape ? PdfPageOrientation.Landscape : PdfPageOrientation.Portrait
                }
            };

            htmlConverter.ConverterSettings.Margin.Top = 25;
            htmlConverter.ConverterSettings.Margin.Right = 15;
            htmlConverter.ConverterSettings.Margin.Left = 10;
            htmlConverter.ConverterSettings.Margin.Bottom = 10;
            htmlConverter.ConverterSettings.EnableRepeatTableHeader = true;

            var tempLocation = Path.Combine(rootPath, "Temp");

            var css = File.ReadAllText(Path.Combine(rootPath, "ReportTemplates", "ReportStyle.css"));
            content = $"<style type='text/css'>{css}</style>{content}";
            var document = htmlConverter.Convert(content, tempLocation);
            document.Template.Bottom = CreateFooter(document, landscape);

            return document;
        }

        private static PdfPageTemplateElement CreateFooter(PdfDocument document, bool landscape = false)
        {
            RectangleF bounds = new RectangleF(0, 0, document.Pages[0].GetClientSize().Width, 50);
            var footer = new PdfPageTemplateElement(bounds);
            var font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            var brush = new PdfSolidBrush(Color.Black);
            var pageNumber = new PdfPageNumberField(font, brush);
            var count = new PdfPageCountField(font, brush);

            var pageNumbering = new PdfCompositeField(font, brush, "Page {0} of {1}", pageNumber, count) { Bounds = footer.Bounds };
            var timeStamp = new PdfCompositeField(font, brush)
            {
                Bounds = footer.Bounds,
                Text = $"Printed: {DateTime.UtcNow.ToString("MMMM dd, yyyy h:mm tt")}"
            };

            var advert = new PdfCompositeField(new PdfStandardFont(PdfFontFamily.TimesRoman, 7), new PdfSolidBrush(Color.Gray))
            {
                Bounds = footer.Bounds,
                Text = "KODZI APP®"
            };

            pageNumbering.Draw(footer.Graphics, new PointF(5, 30));
            timeStamp.Draw(footer.Graphics, new PointF(landscape ? 680 : 440, 30));
            advert.Draw(footer.Graphics, new PointF(landscape ? 765 : 525, 43));

            return footer;
        }

        private static PdfDocument Watermark(PdfDocument document, string text)
        {
            PdfFont bigFont = new PdfStandardFont(PdfFontFamily.Helvetica, 80f);

            foreach (PdfPage pdfPage in document.Pages)
            {
                PdfGraphics graphics = pdfPage.Graphics;
                SizeF textSize = bigFont.MeasureString(text);
                pdfPage.Graphics.TranslateTransform((pdfPage.Size.Width / 2) - 250, pdfPage.Size.Height / 2 + 100);
                pdfPage.Graphics.RotateTransform(-45);
                pdfPage.Graphics.SetTransparency(0.4f);
                pdfPage.Graphics.DrawString(text, bigFont, PdfPens.LightGray, PdfBrushes.LightGray, new PointF(0, 0));
            }

            return document;
        }
    }
}
