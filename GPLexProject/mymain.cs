using System;
using System.IO;
using SimpleScanner;
using ScannerHelper;

namespace Main
{
    class mymain
    {
        static void Main(string[] args)
        {
            // ����� ������������ ����� �������������� � ������������ � ������� 3.14 (� �� 3,14 ��� � ������� Culture)
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            var fname = @"..\..\c.txt";
            Console.WriteLine(File.ReadAllText(fname));
            Console.WriteLine("-------------------------");

            int countIds = 0;
            int minLengthId = int.MaxValue;
            int maxLengthId = int.MinValue;
            double sumLengthId = 0;
            int sumInum = 0;

            Scanner scanner = new Scanner(new FileStream(fname, FileMode.Open));

            int tok = 0;
            do {
                tok = scanner.yylex();
                if (tok == (int)Tok.EOF)
                    break;
                Console.WriteLine(scanner.TokToString((Tok)tok));

                if (tok == (int)Tok.ID)
                {
                    int length = scanner.yytext.Length;
                    countIds++;
                    sumLengthId += length;
                    minLengthId = Math.Min(minLengthId, length);
                    maxLengthId = Math.Max(maxLengthId, length);
                }

                if (tok == (int)Tok.INUM)
                {
                    sumInum += int.Parse(scanner.yytext);
                }
            } while (true);

            double middleLengthId = sumLengthId / countIds;
            Console.WriteLine($"���������� id: {countIds}");
            Console.WriteLine($"����������� ����� id: {minLengthId}");
            Console.WriteLine($"������������ ����� id: {maxLengthId}");
            Console.WriteLine($"������� ����� id: {middleLengthId}");
            Console.WriteLine($"����� ���� inum: {sumInum}");
            Console.WriteLine($"����� ���� rnum: <���� �� ������������ ������������>");

            Console.ReadKey();
        }
    }
}
