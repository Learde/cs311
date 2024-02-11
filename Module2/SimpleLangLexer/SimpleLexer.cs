﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SimpleLexer
{

    public class LexerException : System.Exception
    {
        public LexerException(string msg)
            : base(msg)
        {
        }

    }

    public enum Tok
    {
        // eof
        EOF,
        LINE_BREAK,

        // comma
        COMMA,

        // id
        ID,

        // num
        INUM,

        // arithmetic ops
        PLUS,
        MINUS,
        MULT,
        DIVISION,

        // assign
        ASSIGN,
        SET,

        // stream
        STREAM,

        // brackets
        LEFT_SQUARE_BRACKET,
        RIGHT_SQUARE_BRACKET,
        LEFT_BRACKET,
        RIGHT_BRACKET,

        // if
        IF,
        ELSE,

        // while
        WHILE,

        // functions and procedures
        SUB,
        RETURN,

        // end
        END,

        // logic
        AND,
        OR,
        NOT,
        IS,
        LT,
        GT,
        LEQ,
        GEQ,
        NEQ,

        // address
        ADDRESS,

        // str char
        STRING,
        CHAR,

        // echo
        ECHO,

        // math
        MOD,
        DIV,   
    }

     public class Lexer
    {
        private int position;
        private char currentCh;                      // Текущий символ
        public int LexRow, LexCol;                  // Строка-столбец начала лексемы. Конец лексемы = LexCol+LexText.Length
        private int row, col;                        // текущие строка и столбец в файле
        private TextReader inputReader;
        private Dictionary<string, Tok> keywordsMap; // Словарь, сопоставляющий ключевым словам константы типа TLex. Инициализируется процедурой InitKeywords 
        public Tok LexKind;                         // Тип лексемы
        public string LexText;                      // Текст лексемы
        public int LexValue;                        // Целое значение, связанное с лексемой LexNum

        private string CurrentLineText;  // Накапливает символы текущей строки для сообщений об ошибках
        

        public Lexer(TextReader input)
        {
            CurrentLineText = "";
            inputReader = input;
            keywordsMap = new Dictionary<string, Tok>();
            InitKeywords();
            row = 1; col = 0;
            NextCh();       // Считать первый символ в ch
            NextLexem();    // Считать первую лексему, заполнив LexText, LexKind и, возможно, LexValue
        }

        public void Init() {

        }

        private void PassSpaces()
        {
            while (char.IsWhiteSpace(currentCh))
            {
                NextCh();
            }
        }

        private void InitKeywords()
        {
            keywordsMap["set"] = Tok.SET;
            keywordsMap["if"] = Tok.IF;
            keywordsMap["else"] = Tok.ELSE;
            keywordsMap["end"] = Tok.END;
            keywordsMap["while"] = Tok.WHILE;
            keywordsMap["not"] = Tok.NOT;
            keywordsMap["or"] = Tok.OR;
            keywordsMap["and"] = Tok.AND;
            keywordsMap["is"] = Tok.IS;
            keywordsMap["sub"] = Tok.SUB;
            keywordsMap["return"] = Tok.RETURN;
            keywordsMap["echo"] = Tok.ECHO;
            keywordsMap["mod"] = Tok.MOD;
            keywordsMap["div"] = Tok.DIV;
        }

        public string FinishCurrentLine()
        {
            return CurrentLineText + inputReader.ReadLine();
        }

        private void LexError(string message)
        {
            System.Text.StringBuilder errorDescription = new System.Text.StringBuilder();
            errorDescription.AppendFormat("Lexical error in line {0}:", row);
            errorDescription.Append("\n");
            errorDescription.Append(FinishCurrentLine());
            errorDescription.Append("\n");
            errorDescription.Append(new String(' ', col - 1) + '^');
            errorDescription.Append('\n');
            if (message != "")
            {
                errorDescription.Append(message);
            }
            throw new LexerException(errorDescription.ToString());
        }

        private void NextCh()
        {
            // В LexText накапливается предыдущий символ и считывается следующий символ
            LexText += currentCh;
            var nextChar = inputReader.Read();
            if (nextChar != -1)
            {
                currentCh = (char)nextChar;
                if (currentCh != '\n')
                {
                    col += 1;
                    CurrentLineText += currentCh;
                }
                else
                {
                    row += 1;
                    col = 0;
                    CurrentLineText = "";
                }
            }
            else
            {
                currentCh = (char)0; // если достигнут конец файла, то возвращается #0
            }
        }

        public void NextLexem()
        {
            PassSpaces();
            // R К этому моменту первый символ лексемы считан в ch
            LexText = "";
            LexRow = row;
            LexCol = col;
            // Тип лексемы определяется по ее первому символу
            // Для каждой лексемы строится синтаксическая диаграмма

            switch (currentCh)
            {
                case '~':
                    ProcessOneLineComment();
                    break;
                case '<':
                    ProcessMultilineCommentOrLTOrLEQorNEQ();
                    break;
                case '>':
                    ProcessGTOrGEQ();
                    break;
                case '+':
                    NextCh();
                    LexKind = Tok.PLUS;
                    break;
                case '-':
                    NextCh();
                    LexKind = Tok.MINUS;
                    break;
                case '*':
                    NextCh();
                    LexKind = Tok.MULT;
                    break;
                case '/':
                    NextCh();
                    LexKind = Tok.DIVISION;
                    break;
                case '=':
                    NextCh();
                    LexKind = Tok.ASSIGN;
                    break;
                case '|':
                    NextCh();
                    LexKind = Tok.STREAM;
                    break;
                case '[':
                    NextCh();
                    LexKind = Tok.LEFT_SQUARE_BRACKET;
                    break;
                case ']':
                    NextCh();
                    LexKind = Tok.RIGHT_SQUARE_BRACKET;
                    break;
                case '(':
                    NextCh();
                    LexKind = Tok.LEFT_BRACKET;
                    break;
                case ')':
                    NextCh();
                    LexKind = Tok.RIGHT_BRACKET;
                    break;
                case '@':
                    NextCh();
                    LexKind = Tok.ADDRESS;
                    break;
                case '#':
                    NextCh();
                    LexKind = Tok.LINE_BREAK;
                    break;
                case '\"':
                    NextCh();
                    LexKind = Tok.STRING;
                    break;
                case '\'':
                    NextCh();
                    LexKind = Tok.CHAR;
                    break;
                default:
                    if (char.IsLetter(currentCh))
                    {
                        ProcessIdOrKeyword();
                        return;
                    }
                    if (char.IsDigit(currentCh))
                    {
                        ProcessNum();
                        return;
                    }
                    if ((int)currentCh == 0)
                    {
                        LexKind = Tok.EOF;
                        return;
                    }

                    LexError("Incorrect symbol " + currentCh);
                    break;

            };
        }

        private void ProcessNum()
        {
            while (char.IsDigit(currentCh))
            {
                NextCh();
            }
            LexValue = Int32.Parse(LexText);
            LexKind = Tok.INUM;
        }

        private void ProcessIdOrKeyword()
        {
            while (char.IsLetterOrDigit(currentCh))
            {
                NextCh();
            }
            if (keywordsMap.ContainsKey(LexText))
            {
                LexKind = keywordsMap[LexText];
            }
            else
            {
                LexKind = Tok.ID;
            }
        }

        private void ProcessOneLineComment()
        {
            NextCh();

            if (currentCh != '~')
            {
                LexError("~ was expected");
            }

            while (currentCh != '\n' && (int)currentCh != 0)
            {
                NextCh();
            }

            LexText = "";

            if (currentCh == '\n')
            {
                NextCh();
            }

            NextLexem();
        }

        private void ProcessGTOrGEQ()
        {
            NextCh();
            if (currentCh == '=')
            {
                NextCh();
                LexKind = Tok.GEQ;
                return;
            }

            LexKind = Tok.GT;
        }

        private void ProcessMultilineCommentOrLTOrLEQorNEQ()
        {
            NextCh();
            if (currentCh == '/')
            {
                NextCh();
                if (currentCh == '/')
                {
                    ProcessMultiLineComment();
                    return;
                }
                else
                {
                    LexError("/ was expected");
                }
            }

            if (currentCh == '=')
            {
                NextCh();
                LexKind = Tok.LEQ;
                return;
            }

            if (currentCh == '>')
            {
                NextCh();
                LexKind = Tok.NEQ;
                return;
            }

            LexKind = Tok.LT;
        }

        private void ProcessMultiLineComment()
        {
            while ((int)currentCh != 0)
            {
                while (currentCh != '/' && (int)currentCh != 0)
                {
                    NextCh();
                }
                if ((int)currentCh == 0)
                {
                    LexError("multi line comment not closed");
                }

                NextCh();

                if ((int)currentCh == 0)
                {
                    LexError("multi line comment not closed");
                }

                if (currentCh == '/')
                {
                    NextCh();

                    if ((int)currentCh == 0)
                    {
                        LexError("multi line comment not closed");
                    }

                    if (currentCh == '>')
                    {
                        break;
                    }
                }
            }

            NextCh();
            NextLexem();
        }

        public virtual void ParseToConsole()
        {
            do
            {
                Console.WriteLine(TokToString(LexKind));
                NextLexem();
            } while (LexKind != Tok.EOF);
        }

        public string TokToString(Tok t)
        {
            var result = t.ToString();
            switch (t)
            {
                case Tok.ID: result += ' ' + LexText;
                    break;
                case Tok.INUM: result += ' ' + LexValue.ToString();
                    break;
            }
            return result;
        }
    }
}