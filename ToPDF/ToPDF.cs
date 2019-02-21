using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Collections.Generic;

/* do = 1
 * re = 2
 * mi = 3
 * fa = 4
 * so = 5
 * la = 6
 * si = 7
 * do = 8
 */

namespace PDF
{
    public class ToPDF
    {
        const float lineSpace = 6;
        const float headSpace = 4;
        const float scoreRadius = lineSpace / 2;
        const float defaultTempo = 4;
        const int defaultBarCount = 4;
        const float defaultBeginHeight = 100;
        const float defaultIntervalHeight = 80;
        const float defaultBeginLeft = 100;
        const float defaultEndRight = 500;
        static string signPath = "C:\\UCLA\\MrChorder-master\\MrChorder\\MrChorder\\Images\\sign.png";
        static string cornerPath = "C:\\UCLA\\MrChorder-master\\MrChorder\\MrChorder\\Images\\corner.png";
        Document document;

        public ToPDF(string resultFilePath, double[][] testMusic, int size, string name)
        {
            ScoreCreation(resultFilePath, testMusic, size, name);
        }

        /* Create the score file.
         * sourcePath: Image resource path.
         * resultFilePath: Output file path.
         * testMusic: Music scores.
         * size: Number of scores.
         * name: Music name.
         */
        void ScoreCreation(string resultFilePath, double[][] testMusic, int size, string name)
        {
            document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(resultFilePath, FileMode.OpenOrCreate));
            
            document.Open();
            document.AddTitle("Hello World");
            document.AddSubject("Music Score");
            
            float right = document.Right;
            float top = document.Top;
            float rightMargin = document.RightMargin;
            float topMargin = document.TopMargin;
            // A4 size: 842 595
            Font nameFont = new Font(Font.FontFamily.TIMES_ROMAN, 32f, Font.NORMAL, BaseColor.BLACK);
            Paragraph musicName = new Paragraph(name, nameFont);
            musicName.Alignment = Element.ALIGN_CENTER;
            document.Add(musicName);

            // Drawing content container.
            PdfContentByte content = writer.DirectContent;
            blankLine();

            // Descriptions.
            string copyRight = "Presented to you by Mr. Chorder.";
            Font copyRightFont = new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.ITALIC, BaseColor.GRAY);
            Paragraph description = new Paragraph(copyRight, copyRightFont);
            description.Alignment = Element.ALIGN_CENTER;
            document.Add(description);

            // Decorations.
            Image corner = Image.GetInstance(cornerPath);
            corner.SetAbsolutePosition(0, 0);
            corner.ScaleAbsoluteHeight(50f * lineSpace);
            corner.ScaleAbsoluteWidth(50f * lineSpace);
            content.AddImage(corner);

            // Draw everything.
            DrawFromArray(content, testMusic, size);
            
            document.Close();
        }

        private void blankLine()
        {
            document.Add(new Paragraph(" "));
        }

        /* Draw the scores from an array.
         * content: Drawing content container.
         * musicArray: Source data.
         * size: Data size.
         * count: Number of bars in one line.
         * tempo: Number of notes in one bar.
         */
        private void DrawFromArray(PdfContentByte content, double[][] musicArray, int size, int count = defaultBarCount, float tempo = defaultTempo)
        {
            float beginHeight = defaultBeginHeight;
            float beginLeft = defaultBeginLeft;
            float endRight = defaultEndRight;
            float intervalHeight = defaultIntervalHeight;
            int line = 0;
            int c = -1;
            float width = (endRight - beginLeft) / count;
            float widthScore = (endRight - beginLeft) / (count * (tempo + 1));
            float scoreStart = beginLeft + widthScore;
            // Music speed.
            content.BeginText();
            content.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED), 10);
            content.SetTextMatrix(beginLeft, PageSize.A4.Height - beginHeight + 3.5f * lineSpace - intervalHeight);
            content.ShowText("Moderate       = 120");
            content.EndText();
            DrawOneScore(content, beginLeft + 14 * headSpace, beginHeight - 7 * lineSpace + intervalHeight, 3);
            // Music tempo.
            content.BeginText();
            content.SetFontAndSize(BaseFont.CreateFont(BaseFont.COURIER_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 16);
            content.SetTextMatrix(beginLeft, PageSize.A4.Height - (beginHeight + 2 * lineSpace + intervalHeight));
            content.ShowText("4");
            content.SetTextMatrix(beginLeft, PageSize.A4.Height - (beginHeight + 4 * lineSpace + intervalHeight));
            content.ShowText("4");
            content.EndText();
            // Draw the scores.
            double[][] formalMusicArray = MusicFormalization(musicArray, tempo);
            float curScoreStart = scoreStart;
            float shortEndRight = 0;
            double curBarLength = 0;
            for (int i = 0; i < formalMusicArray.Length; i++)
            {
                if (curScoreStart == scoreStart)
                {
                    line++;
                    Image sign = Image.GetInstance(signPath);
                    sign.SetAbsolutePosition(beginLeft - 18, PageSize.A4.Height - (beginHeight + line * intervalHeight + 5 * lineSpace));
                    sign.ScaleAbsoluteHeight(6 * lineSpace);
                    sign.ScaleAbsoluteWidth(3.5f * lineSpace);
                    content.AddImage(sign);
                }
                DrawOneScore(content, curScoreStart, beginHeight + line * intervalHeight, formalMusicArray[i][0], formalMusicArray[i][1]);
                curBarLength += formalMusicArray[i][1];
                curScoreStart += widthScore;
                if (i == formalMusicArray.Length - 1)
                {
                    shortEndRight = curScoreStart;
                    DrawFiveLines(content, beginLeft, shortEndRight + 2, beginHeight + line * intervalHeight);
                    content.MoveTo(curScoreStart, PageSize.A4.Height - (beginHeight + line * intervalHeight));
                    content.LineTo(curScoreStart, PageSize.A4.Height - (beginHeight + line * intervalHeight) - lineSpace * 4);
                    content.Stroke();
                }
                else
                {
                    if (curScoreStart >= endRight)
                    {
                        DrawFiveLines(content, beginLeft, endRight, beginHeight + line * intervalHeight);
                        curScoreStart = scoreStart;
                    }
                    if (curBarLength == tempo)
                    {
                        if (curScoreStart + widthScore >= endRight)
                        {
                            DrawFiveLines(content, beginLeft, endRight, beginHeight + line * intervalHeight);
                            curScoreStart = scoreStart;
                        }
                        else if (curScoreStart != scoreStart)
                        {
                            content.MoveTo(curScoreStart, PageSize.A4.Height - (beginHeight + line * intervalHeight));
                            content.LineTo(curScoreStart, PageSize.A4.Height - (beginHeight + line * intervalHeight) - lineSpace * 4);
                            content.Stroke();
                            curScoreStart += widthScore;
                        }
                        curBarLength = 0;
                    }
                }
                //--------------------------------------------------
                /*
                if (i % ((int)count * tempo) == 0)
                {
                    line++;
                    // Draw the sign.
                    Image sign = Image.GetInstance(signPath);
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
                DrawOneScore(content, beginLeft + c * width + widthScore * (i % ((int)tempo) + 1), beginHeight + line * intervalHeight, musicArray[i][0], musicArray[i][1]);
                */
            }
        }

        /* Normalize the music array, arranging them into different bars based on the tempo.
         * Returned value includes info on those that are divided into two bars.
         */
        double[][] MusicFormalization(double[][] musicArray, float tempo)
        {
            List<double[]> tempList = new List<double[]>();
            double curLength = 0;
            for (int i = 0; i < musicArray.Length; i++)
            {
                if (curLength + musicArray[i][1] > tempo)
                {
                    double firstPart = tempo - curLength;
                    double secondPart = musicArray[i][1] - firstPart;
                    curLength = secondPart;
                    double[] firstNote = new double[3];
                    firstNote[0] = musicArray[i][0];
                    firstNote[1] = firstPart;
                    firstNote[2] = 1;
                    tempList.Add(firstNote);
                    double[] secondNote = new double[3];
                    secondNote[0] = musicArray[i][0];
                    secondNote[1] = secondPart;
                    secondNote[2] = 1;
                    tempList.Add(secondNote);
                }
                else
                {
                    curLength += musicArray[i][1];
                    double[] formalNote = new double[3];
                    formalNote[0] = musicArray[i][0];
                    formalNote[1] = musicArray[i][1];
                    formalNote[2] = 0;
                    tempList.Add(formalNote);
                    if (curLength == 4)
                    {
                        curLength = 0;
                    }
                }
            }
            double[][] result = new double[tempList.Count][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[3];
                result[i][0] = tempList[i][0];
                result[i][1] = tempList[i][1];
                result[i][2] = tempList[i][2];
            }
            return result;
        }

        /* Draw one note, do = 1.
         * content: Drawing content container.
         * left: Beginning position.
         * up: Beginning position.
         * number: Tone.
         * length: Length of the note, currently support 1, 0.75, 0.5, 0.25.
         */
        private void DrawOneScore(PdfContentByte content, float left, float up, double number, double length = 1)
        {
            // TODO(allenxie): Deal with flag == false.
            up = PageSize.A4.Height - up;
            float position = up - 5 * lineSpace + ((float)number - 1) * lineSpace / 2;
            // Circle
            content.SetColorFill(BaseColor.BLACK);
            // content.Circle(left, position, scoreRadius);
            content.Ellipse(left - 4, position - 3, left + 4, position + 3);
            content.Fill();
            // Vertical line (0.5 is for good-looking)
            if (number < 8)
            {
                content.MoveTo(left + scoreRadius + 0.5, position);
                content.LineTo(left + scoreRadius + 0.5, position + 3.5 * lineSpace);
                content.Stroke();
                if (length == 0.5 || length == 0.75)
                {
                    double left_start = left + scoreRadius + 0.5;
                    double up_start = position + 3.5 * lineSpace - 0.5;
                    content.MoveTo(left_start, up_start);
                    content.CurveTo(left_start + 1, up_start - 4, left_start + 7, up_start - 7, left_start + 4, up_start - 12);
                    content.Stroke();
                    if (length == 0.75)
                    {
                        double dot_position = (number % 2 == 0) ? position : position + 0.5 * lineSpace;
                        content.Circle(left_start + 3, dot_position, 1);
                        content.Fill();
                    }
                }
                if (length == 0.25)
                {
                    double left_start = left + scoreRadius + 0.5;
                    double up_start = position + 3.5 * lineSpace - 0.5;
                    content.MoveTo(left_start, up_start);
                    content.CurveTo(left_start + 1, up_start - 3, left_start + 6, up_start - 6, left_start + 3, up_start - 9);
                    content.Stroke();
                    up_start = up_start - 4;
                    content.MoveTo(left_start, up_start);
                    content.CurveTo(left_start + 1, up_start - 3, left_start + 6, up_start - 6, left_start + 3, up_start - 9);
                    content.Stroke();
                }
            }
            else
            {
                content.MoveTo(left - scoreRadius - 0.5, position);
                content.LineTo(left - scoreRadius - 0.5, position - 3.5 * lineSpace);
                content.Stroke();
                if (length == 0.5 || length == 0.75)
                {
                    double left_start = left - scoreRadius - 0.5;
                    double up_start = position - 3.5 * lineSpace + 0.5;
                    content.MoveTo(left_start, up_start);
                    content.CurveTo(left_start + 1, up_start + 4, left_start + 7, up_start + 7, left_start + 4, up_start + 12);
                    content.Stroke();
                    if (length == 0.75)
                    {
                        double dot_position = (number % 2 == 0) ? position : position + 0.5 * lineSpace;
                        content.Circle(left_start + 8, dot_position, 1);
                        content.Fill();
                    }
                }if (length == 0.25)
                {
                    double left_start = left - scoreRadius - 0.5;
                    double up_start = position - 3.5 * lineSpace + 0.5;
                    content.MoveTo(left_start, up_start);
                    content.CurveTo(left_start + 1, up_start + 3, left_start + 6, up_start + 6, left_start + 3, up_start + 9);
                    content.Stroke();
                    up_start = up_start + 4;
                    content.MoveTo(left_start, up_start);
                    content.CurveTo(left_start + 1, up_start + 3, left_start + 6, up_start + 6, left_start + 3, up_start + 9);
                    content.Stroke();
                }
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
        private void DrawFiveLines(PdfContentByte content, float left, float right, float up, int count = defaultBarCount, float height = lineSpace)
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
            /*
            float width = (right - left) / count;
            for (int i = 1; i < count; i++)
            {
                content.MoveTo(left + width * i, up);
                content.LineTo(left + width * i, up - height * 4);
            }
            */
            content.Stroke();
        }
    }
}
