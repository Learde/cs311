﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using SimpleLexer;
namespace SimpleLangParser
{
    public class ParserException : System.Exception
    {
        public ParserException(string msg)
            : base(msg)
        {
        }

    }

    public class Parser
    {
        private SimpleLexer.Lexer l;

        public Parser(SimpleLexer.Lexer lexer)
        {
            l = lexer;
        }

        public void Program()
        {
            OperatorList();
        }

        public void OperatorList()
        {
            PassNewLines();

            Operator();

            if (l.LexKind == Tok.LINE_BREAK)
            {
                l.NextLexem();
            }

            if (!IsNewLine())
            {
                SyntaxError("New line expected");
            }

            l.NextLexem();

            if (!IsEndOfProg())
            {
                OperatorList();
            }
        }

        public void Operator()
        {
            if (l.LexKind == Tok.LINE_BREAK)
            {
                l.NextLexem();
                if (l.LexKind == Tok.NEW_LINE)
                {
                    l.NextLexem();
                    Operator();
                } else
                {
                    SyntaxError("New line expected");
                }
            } else if (l.LexKind == Tok.SET)
            {
                Assignment();
            } else if (l.LexKind == Tok.SUB)
            {
                Subroutine();
            } else if (IsExpression())
            {
                ExpressionList();
            } else if (IsStatement())
            {
                Statement();
            } else
            {
                SyntaxError("Assign, Sub, Expression or Statement expected");
            }
        }

        private bool IsEndOfProg()
        {
            return l.LexKind == Tok.EOF || l.LexKind == Tok.END || l.LexKind == Tok.ELSE;
        }

        private bool IsNewLine()
        {
            return l.LexKind == Tok.NEW_LINE;
        }

        private void PassNewLines()
        {
            while (l.LexKind == Tok.NEW_LINE)
            {
                l.NextLexem();
            }
        }

        public void Statement()
        {
            if (l.LexKind == Tok.IF)
            {
                If();
            } else if (l.LexKind == Tok.WHILE)
            {
                While();
            } else if (l.LexKind == Tok.RETURN) {
                Return();
            }
            else
            {
                SyntaxError("IF or WHILE expected");
            }
        }

        public void Return()
        {
            l.NextLexem();  // пропуск return
            ExpressionList();
        }

        public void If()
        {
            l.NextLexem();  // пропуск if
            if (l.LexKind != Tok.LEFT_SQUARE_BRACKET)
            {
                SyntaxError("Left square bracket expected");
            }
            l.NextLexem();
            ExpressionList();
            if (l.LexKind != Tok.RIGHT_SQUARE_BRACKET)
            {
                SyntaxError("Right square bracket expected");
            }
            l.NextLexem();
            Program();
            if (l.LexKind == Tok.ELSE)
            {
                l.NextLexem();
                Program();
            }
            if (l.LexKind != Tok.END)
            {
                SyntaxError("END IF expected");
            }
            l.NextLexem();
            if (l.LexKind != Tok.IF)
            {
                SyntaxError("END IF expected");
            }
            l.NextLexem();
        }

        public void While()
        {
            l.NextLexem();  // пропуск while
            if (l.LexKind != Tok.LEFT_SQUARE_BRACKET)
            {
                SyntaxError("Left square bracket expected");
            }
            l.NextLexem();
            ExpressionList();
            if (l.LexKind != Tok.RIGHT_SQUARE_BRACKET)
            {
                SyntaxError("Right square bracket expected");
            }
            l.NextLexem();
            Program();
            if (l.LexKind != Tok.END)
            {
                SyntaxError("END WHILE expected");
            }
            l.NextLexem();
            if (l.LexKind != Tok.WHILE)
            {
                SyntaxError("END WHILE expected");
            }
            l.NextLexem();
        }

        public void Subroutine()
        {
            l.NextLexem();  // пропуск sub
            if (l.LexKind != Tok.ID)
            {
                SyntaxError("ID expected");
            }
            l.NextLexem();
            Program();
            if (l.LexKind != Tok.END)
            {
                SyntaxError("END SUB expected");
            }
            l.NextLexem();
            if (l.LexKind != Tok.SUB)
            {
                SyntaxError("END SUB expected");
            }
            l.NextLexem();
        }

        public void Assignment()
        {
            l.NextLexem();  // пропуск set
            if (l.LexKind != Tok.ID)
            {
                SyntaxError("ID expected");
            }
            l.NextLexem();
            if (l.LexKind != Tok.ASSIGN)
            {
                SyntaxError("Assign expected");
            }
            l.NextLexem();
            if (l.LexKind == Tok.LEFT_BRACKET)
            {
                Array();
            }
            else
            {
                ExpressionList();
            }
        }

        public void Array()
        {
            l.NextLexem();  // пропуск (
            if (l.LexKind == Tok.RIGHT_BRACKET)
            {
                l.NextLexem();
            }
            else
            {
                Expression();
                while (l.LexKind == Tok.COMMA)
                {
                    l.NextLexem();
                    Expression();
                }
                if (l.LexKind != Tok.RIGHT_BRACKET)
                {
                    SyntaxError("Right bracket expected");
                }
                l.NextLexem();
            }
        }

        public void ExpressionList()
        {
            Expression();

            if (IsBinop())
            {
                l.NextLexem();
                ExpressionList();
            }
        }

        public void Expression()
        {
            if (IsCall())
            {
                Call();
            } else if (IsUnop())
            {
                Unop();
            } else if (IsConst())
            {
                l.NextLexem();
            } else if (IsFunc()) {
                l.NextLexem();
            }
            else
            {
                SyntaxError("Call, Unop, Id or Const expected");
            }
        }

        public void Unop()
        {
            l.NextLexem();  // пропуск op
            ExpressionList();
        }

        public void Call()
        {
            l.NextLexem();  // пропуск id
            if (l.LexKind == Tok.LEFT_BRACKET)
            {
                l.NextLexem(); // значит вызов функции, а не обращение к переменной
                ParameterList();
            }
        }

        public void ParameterList()
        {
            if (l.LexKind == Tok.RIGHT_BRACKET)
            {
                l.NextLexem();
            } else
            {
                ExpressionList();
                if (l.LexKind == Tok.RIGHT_BRACKET)
                {
                    l.NextLexem();
                } else if (l.LexKind == Tok.COMMA)
                {
                    l.NextLexem();
                    ParameterList();
                } else
                {
                    SyntaxError("Comma or Right bracket expected");
                }
            }
        }

        private bool IsIf()
        {
            return l.LexKind == Tok.IF;
        }

        private bool IsWhile()
        {
            return l.LexKind == Tok.WHILE;
        }

        private bool IsReturn()
        {
            return l.LexKind == Tok.RETURN;
        }

        private bool IsStatement()
        {
            return IsIf() || IsWhile() || IsReturn();
        }

        private bool IsExpression()
        {
            return IsCall() || IsUnop() || IsConst();
        }

        private bool IsConst()
        {
            return IsNum() || IsString() || IsChar();
        }

        private bool IsCall()
        {
            return l.LexKind == Tok.ID;
        }

        private bool IsString()
        {
            return l.LexKind == Tok.STRING;
        }

        private bool IsChar()
        {
            return l.LexKind == Tok.CHAR;
        }

        private bool IsUnop()
        {
            return l.LexKind == Tok.PLUS ||
                l.LexKind == Tok.MINUS ||
                l.LexKind == Tok.NOT ||
                l.LexKind == Tok.ADDRESS;
        }

        private bool IsBinop()
        {
            return l.LexKind == Tok.PLUS ||
                l.LexKind == Tok.MINUS ||
                l.LexKind == Tok.OR ||
                l.LexKind == Tok.AND ||
                l.LexKind == Tok.MOD ||
                l.LexKind == Tok.DIV ||
                l.LexKind == Tok.MULT ||
                l.LexKind == Tok.DIVISION ||
                l.LexKind == Tok.STREAM ||
                l.LexKind == Tok.IS ||
                l.LexKind == Tok.LT ||
                l.LexKind == Tok.GT ||
                l.LexKind == Tok.LEQ ||
                l.LexKind == Tok.GEQ ||
                l.LexKind == Tok.NEQ ||
                l.LexKind == Tok.ADDRESS;
        }

        private bool IsNum()
        {
            return l.LexKind == Tok.INUM;
        }

        private bool IsFunc()
        {
            return IsId() || l.LexKind == Tok.ECHO;
        }

        private bool IsId()
        {
            return l.LexKind == Tok.ID;
        }

        public void SyntaxError(string message) 
        {
            var errorMessage = "Syntax error in line " + l.LexRow.ToString() + ":\n";
            errorMessage += l.FinishCurrentLine() + "\n";
            errorMessage += new String(' ', l.LexCol - 1) + "^\n";
            if (message != "")
            {
                errorMessage += message;
            }
            throw new ParserException(errorMessage);
        }
   
    }
}
