using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace PDF
{
    public class ToPDF
    {
        const float lineSpace = 6;
        const float headSpace = 4;
        const float scoreRadius = lineSpace / 2;
        const float defaultTempo = 4;
        const int defaultCount = 4;
        const float defaultBeginHeight = 100;
        const float defaultIntervalHeight = 80;
        const float defaultBeginLeft = 100;
        const float defaultEndRight = 500;
        static string imagePath;

        /* Create the score file.
         * sourcePath: Image resource path.
         * resultFilePath: Output file path.
         * testMusic: Music scores.
         * size: Number of scores.
         * name: Music name.
         * fromName: Whom the music is sent from.
         * toName: Whom the music is sent to.
         * mode: Fancy modes for drawing.
         */
        public static void ScoreCreation(string sourcePath, string resultFilePath, float[] testMusic, int size, string name, string fromName, string toName, int mode = 1)
        {
            imagePath = sourcePath + "sign.png";
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(resultFilePath, FileMode.OpenOrCreate));
            
            document.Open();
            document.AddTitle("Hello World");
            document.AddSubject("Music Score");

            // A4 size: 842 595
            Font nameFont = new Font(Font.FontFamily.COURIER, 32f, Font.NORMAL, new BaseColor(0, 128, 128));
            Paragraph musicName = new Paragraph(name, nameFont);
            musicName.Alignment = Element.ALIGN_CENTER;
            document.Add(musicName);

            // Drawing content container.
            PdfContentByte content = writer.DirectContent;

            Font desFont = new Font(Font.FontFamily.TIMES_ROMAN, 16f, Font.NORMAL, new BaseColor(128, 128, 255));
            
            // Fancy mode 1: The song is sent to A from B.
            if (mode == 1)
            {
                string des = "This piece of music is sent from " + fromName + " to " + toName + ",\nexpressing the unconditional love.";
                Paragraph description = new Paragraph(des, desFont);
                description.Alignment = Element.ALIGN_CENTER;
                document.Add(description);

                string copyRight = "Presented to you by Mr. Chorder.";
                description = new Paragraph(copyRight, new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.ITALIC, new BaseColor(128, 128, 128)));
                description.Alignment = Element.ALIGN_CENTER;
                document.Add(description);

                Image corner1 = Image.GetInstance(sourcePath + "corner.png");
                corner1.SetAbsolutePosition(0, 0);
                corner1.ScaleAbsoluteHeight(50f * lineSpace);
                corner1.ScaleAbsoluteWidth(50f * lineSpace);
                content.AddImage(corner1);
            }

            // Draw everything.
            DrawFromArray(content, testMusic, size);
            
            document.Close();
        }

        /* Draw the scores from an array.
         * content: Drawing content container.
         * musicArray: Source data.
         * size: Data size.
         * count: Number of bars in one line.
         * tempo: Number of notes in one bar.
         */
        static void DrawFromArray(PdfContentByte content, float[] musicArray, int size, int count = defaultCount, float tempo = defaultTempo)
        {
            float beginHeight = defaultBeginHeight;
            float beginLeft = defaultBeginLeft;
            float endRight = defaultEndRight;
            float intervalHeight = defaultIntervalHeight;
            int line = 0;
            int c = -1;
            float width = (endRight - beginLeft) / count;
            float widthScore = (endRight - beginLeft) / (count * (tempo + 1));
            // Music speed.
            content.BeginText();
            content.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED), 10);
            content.SetTextMatrix(beginLeft, PageSize.A4.Height - beginHeight + 3 * lineSpace - intervalHeight);
            content.ShowText("Moderate       = 120");
            content.EndText();
            DrawOneScore(content, beginLeft + 14 * headSpace, beginHeight - 6.5f * lineSpace + intervalHeight, 2);
            // Music tempo.
            content.BeginText();
            content.SetFontAndSize(BaseFont.CreateFont(BaseFont.COURIER_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 16);
            content.SetTextMatrix(beginLeft, PageSize.A4.Height - (beginHeight + 2 * lineSpace + intervalHeight));
            content.ShowText("4");
            content.SetTextMatrix(beginLeft, PageSize.A4.Height - (beginHeight + 4 * lineSpace + intervalHeight));
            content.ShowText("4");
            content.EndText();
            // Draw the scores.
            for (int i = 0; i < size; i++)
            {
                if (i % ((int)count * tempo) == 0)
                {
                    line++;
                    // Draw the sign.
                    Image sign = Image.GetInstance(imagePath);
                    sign.SetAbsolutePosition(beginLeft - 18, PageSize.A4.Height - (beginHeight + line * intervalHeight + 5 * lineSpace));
                    sign.ScaleAbsoluteHeight(6 * lineSpace);
                    sign.ScaleAbsoluteWidth(3.5f * lineSpace);
                    content.AddImage(sign);
                    if (size - i <= count * tempo)
                    {
                        int remain = (size - i) / (int)tempo + 1;
                        if ((size - i) % tempo == 0)
                        {
                            remain--;
                        }
                        DrawFiveLines(content, beginLeft, beginLeft + ((float)remain / (float)count) * (endRight - beginLeft), beginHeight + line * intervalHeight, remain);
                        // Dnd vertical line.
                        content.MoveTo(beginLeft + ((float)remain / (float)count) * (endRight - beginLeft) - headSpace, PageSize.A4.Height - (beginHeight + line * intervalHeight));
                        content.LineTo(beginLeft + ((float)remain / (float)count) * (endRight - beginLeft) - headSpace, PageSize.A4.Height - (beginHeight + line * intervalHeight) - lineSpace * 4);
                        content.Stroke();
                    }
                    else
                    {
                        DrawFiveLines(content, beginLeft, endRight, beginHeight + line * intervalHeight, count);
                    }
                }
                // Switch to next bar.
                if (i % ((int)tempo) == 0)
                {
                    c = (c + 1) % count;
                }
                DrawOneScore(content, beginLeft + c * width + widthScore * (i % ((int)tempo) + 1), beginHeight + line * intervalHeight, musicArray[i]);
            }
        }

        /* Draw one note.
         * content: Drawing content container.
         * left: Beginning position.
         * up: Beginning position.
         * number: Tone.
         * flag: Whether needs half tone.
         */
        static void DrawOneScore(PdfContentByte content, float left, float up, float number, bool flag = true)
        {
            // TODO(allenxie): Deal with flag == false.
            up = PageSize.A4.Height - up;
            float position = up - 5 * lineSpace + (number - 1) * lineSpace / 2;
            // Circle
            content.SetColorFill(BaseColor.BLACK);
            content.Circle(left, position, scoreRadius);
            content.Fill();
            // Vertical line
            if (number < 8)
            {
                content.MoveTo(left + scoreRadius, position);
                content.LineTo(left + scoreRadius, position + 3 * lineSpace);
                content.Stroke();
            }
            else
            {
                content.MoveTo(left - scoreRadius, position);
                content.LineTo(left - scoreRadius, position - 3 * lineSpace);
                content.Stroke();
            }
            // If need addition lateral line
            if (number < 2 && ((int)number % 2 == 0))
            {
                content.MoveTo(left - scoreRadius * 2, position + scoreRadius);
                content.LineTo(left + scoreRadius * 2, position + scoreRadius);
                content.Stroke();
            }
            else if (number < 2 && ((int)number % 2 != 0))
            {
                content.MoveTo(left - scoreRadius * 2, position);
                content.LineTo(left + scoreRadius * 2, position);
                content.Stroke();
            }
            else if (number > 12 && ((int)number % 2 == 0))
            {
                content.MoveTo(left - scoreRadius * 2, position - scoreRadius);
                content.LineTo(left + scoreRadius * 2, position - scoreRadius);
                content.Stroke();
            }
            else if (number > 12 && ((int)number % 2 != 0))
            {
                content.MoveTo(left - scoreRadius * 2, position);
                content.LineTo(left + scoreRadius * 2, position);
                content.Stroke();
            }
        }

        /* Draw five lines.
         * content: Drawing content container.
         * left: Beginning position.
         * right: Ending position.
         * up: Beginning position.
         * count: Number of bars.
         * height: Space between lines.
         */
        static void DrawFiveLines(PdfContentByte content, float left, float right, float up, int count, float height = lineSpace)
        {
            up = PageSize.A4.Height - up;
            content.SetColorStroke(BaseColor.BLACK);
            // Five lateral lines.
            content.MoveTo(left - 20, up);
            content.LineTo(right, up);
            content.MoveTo(left - 20, up - height);
            content.LineTo(right, up - height);
            content.MoveTo(left - 20, up - height * 2);
            content.LineTo(right, up - height * 2);
            content.MoveTo(left - 20, up - height * 3);
            content.LineTo(right, up - height * 3);
            content.MoveTo(left - 20, up - height * 4);
            content.LineTo(right, up - height * 4);
            // Start & end vertical lines.
            content.MoveTo(left - 20, up);
            content.LineTo(left - 20, up - height * 4);
            content.MoveTo(right, up);
            content.LineTo(right, up - height * 4);
            // Middle lines.
            float width = (right - left) / count;
            for (int i = 1; i < count; i++)
            {
                content.MoveTo(left + width * i, up);
                content.LineTo(left + width * i, up - height * 4);
            }
            content.Stroke();
        }
    }
}
