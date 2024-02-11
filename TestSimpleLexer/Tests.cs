using System;
using NUnit.Framework;
using System.IO;
using SimpleLexer;
using System.Collections.Generic;
using System.Linq;


namespace TestSimpleLexer
{
    
    public class LexemList: List<KeyValuePair<Tok, string>>
    {
        public void Add(Tok key, string value)
        {
            var element = new KeyValuePair<Tok, string>(key, value);
            this.Add(element);
        }
    }
    
    [TestFixture]
    //[Ignore("This test is disabled")]
    public class TestSimpleLexer
    {
        public static List< KeyValuePair<Tok, string> > getLexerOutput(Lexer l)
        {
            List< KeyValuePair<Tok, string> > result = new List< KeyValuePair<Tok, string> >();
            do
            {
                result.Add(new KeyValuePair<Tok, string>(l.LexKind, l.LexText));
                l.NextLexem();
            } while (l.LexKind != Tok.EOF);

            if (result[0].Key == Tok.EOF && result.Count == 1)
            {
                result.Clear();
            }

            return result;
        }
        
        [Test]
        public void TestId()
        {
            string text = @"id123";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            try
            {
                var lexems = getLexerOutput(l);
                Assert.IsTrue(lexems.Count == 1);
                Assert.IsTrue(lexems.First().Key == Tok.ID);
                Assert.IsTrue(lexems.First().Value == "id123");
            }
            catch (LexerException e)
            {
                Assert.Fail();
            }
        }
        
    }
    
    [TestFixture]
    //[Ignore("This test is disabled")]
    public class TestSimpleLexerOps
    {
        [Test]
        public void TestArithmeticOps()
        {
            string text = @"+-*/ / -*   +";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            
            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 8);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.PLUS, "+"},
                {Tok.MINUS, "-"},
                {Tok.MULT, "*"},
                {Tok.DIVISION, "/"},
                {Tok.DIVISION, "/"},
                {Tok.MINUS, "-"},
                {Tok.MULT, "*"},
                {Tok.PLUS, "+"} 
            }.ToList(), lexems);
        }

        [Test]
        public void TestAssignOp()
        {
            string text = @"set = set  = = set set";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 7);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.SET, "set"},
                {Tok.ASSIGN, "="},
                {Tok.SET, "set"},
                {Tok.ASSIGN, "="},
                {Tok.ASSIGN, "="},
                {Tok.SET, "set"},
                {Tok.SET, "set"},
            }.ToList(), lexems);
        }

        [Test]
        public void TestStreamOp()
        {
            string text = @"|| |  |";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 4);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.STREAM, "|"},
                {Tok.STREAM, "|"},
                {Tok.STREAM, "|"},
                {Tok.STREAM, "|"},
            }.ToList(), lexems);
        }

        [Test]
        public void TestConditionOps()
        {
            string text = @"[][[]]][";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 8);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.LEFT_SQUARE_BRACKET, "["},
                {Tok.RIGHT_SQUARE_BRACKET, "]"},
                {Tok.LEFT_SQUARE_BRACKET, "["},
                {Tok.LEFT_SQUARE_BRACKET, "["},
                {Tok.RIGHT_SQUARE_BRACKET, "]"},
                {Tok.RIGHT_SQUARE_BRACKET, "]"},
                {Tok.RIGHT_SQUARE_BRACKET, "]"},
                {Tok.LEFT_SQUARE_BRACKET, "["},
            }.ToList(), lexems);
        }

        [Test]
        public void TestBracketsOps()
        {
            string text = @"()(()))(";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 8);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.LEFT_BRACKET, "("},
                {Tok.RIGHT_BRACKET, ")"},
                {Tok.LEFT_BRACKET, "("},
                {Tok.LEFT_BRACKET, "("},
                {Tok.RIGHT_BRACKET, ")"},
                {Tok.RIGHT_BRACKET, ")"},
                {Tok.RIGHT_BRACKET, ")"},
                {Tok.LEFT_BRACKET, "("},
            }.ToList(), lexems);
        }

        [Test]
        public void TestIfOps()
        {
            string text = @"if end if if else end end";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 7);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.IF, "if"},
                {Tok.END, "end"},
                {Tok.IF, "if"},
                {Tok.IF, "if"},
                {Tok.ELSE, "else"},
                {Tok.END, "end"},
                {Tok.END, "end"},
            }.ToList(), lexems);
        }

        [Test]
        public void TestMathOps()
        {
            string text = @"5 mod 2 10 div 3";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 6);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.INUM, "5"},
                {Tok.MOD, "mod"},
                {Tok.INUM, "2"},
                {Tok.INUM, "10"},
                {Tok.DIV, "div"},
                {Tok.INUM, "3"},
            }.ToList(), lexems);
        }

        [Test]
        public void TestWhileOps()
        {
            string text = @"while end while while end end";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 6);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.WHILE, "while"},
                {Tok.END, "end"},
                {Tok.WHILE, "while"},
                {Tok.WHILE, "while"},
                {Tok.END, "end"},
                {Tok.END, "end"},
            }.ToList(), lexems);
        }

        [Test]
        public void TestEchoOp()
        {
            string text = @"1 | echo";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 3);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.INUM, "1"},
                {Tok.STREAM, "|"},
                {Tok.ECHO, "echo"},
            }.ToList(), lexems);
        }

        [Test]
        public void TestSubOps()
        {
            string text = @"sub end sub return sub end end";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 7);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.SUB, "sub"},
                {Tok.END, "end"},
                {Tok.SUB, "sub"},
                {Tok.RETURN, "return"},
                {Tok.SUB, "sub"},
                {Tok.END, "end"},
                {Tok.END, "end"},
            }.ToList(), lexems);
        }

        [Test]
        public void TestAddressOp()
        {
            string text = @"@ @ @@@          @";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 6);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.ADDRESS, "@"},
                {Tok.ADDRESS, "@"},
                {Tok.ADDRESS, "@"},
                {Tok.ADDRESS, "@"},
                {Tok.ADDRESS, "@"},
                {Tok.ADDRESS, "@"},
            }.ToList(), lexems);
        }

        [Test]
        public void TestLineBreakOp()
        {
            string text = @"# # ###          #";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 6);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.LINE_BREAK, "#"},
                {Tok.LINE_BREAK, "#"},
                {Tok.LINE_BREAK, "#"},
                {Tok.LINE_BREAK, "#"},
                {Tok.LINE_BREAK, "#"},
                {Tok.LINE_BREAK, "#"},
            }.ToList(), lexems);
        }

        [Test]
        public void TestStringOp()
        {
            string text = "\"text\"";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 3);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.STRING, "\""},
                {Tok.ID, "text"},
                {Tok.STRING, "\""},
            }.ToList(), lexems);
        }

        [Test]
        public void TestCharOp()
        {
            string text = "\'t\' \'b\'";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 6);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.CHAR, "\'"},
                {Tok.ID, "t"},
                {Tok.CHAR, "\'"},
                {Tok.CHAR, "\'"},
                {Tok.ID, "b"},
                {Tok.CHAR, "\'"},
            }.ToList(), lexems);
        }

        [Test]
        public void TestLogicOps()
        {
            string text = @"and or    and not   is  or not and is";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 9);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.AND, "and"},
                {Tok.OR, "or"},
                {Tok.AND, "and"},
                {Tok.NOT, "not"},
                {Tok.IS, "is"},
                {Tok.OR, "or"},
                {Tok.NOT, "not"},
                {Tok.AND, "and"},
                {Tok.IS, "is"},
            }.ToList(), lexems);
        }
        
        [Test]
        public void TestOpsFail()
        {
            string text = @"-`-$ # @";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            Assert.Throws<LexerException>(() =>
            {
                var lexems = TestSimpleLexer.getLexerOutput(l);
            });
        }
        
    }
    
    [TestFixture]
    public class TestSimpleLexerAssigns
    {
        [Test]
        public void TestAssigns()
        {
            string text = @"+ is =6-*+/";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            
            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 8);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.PLUS, "+"},
                {Tok.IS, "is" },
                {Tok.ASSIGN, "="},
                {Tok.INUM, "6"},
                {Tok.MINUS, "-"},
                {Tok.MULT, "*"},
                {Tok.PLUS, "+"},
                {Tok.DIVISION, "/"}
   
            }.ToList(), lexems);
        }
        
    }
    
    [TestFixture]
    public class TestSimpleLexerComparisons
    {
        [Test]
        public void TestComparisons()
        {
            string text = @"><>>=<=+<> > <=";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            
            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 8);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.GT, ">"},
                {Tok.NEQ, "<>"},
                {Tok.GEQ, ">="},
                {Tok.LEQ, "<="},
                {Tok.PLUS, "+"},
                {Tok.NEQ, "<>"},
                {Tok.GT, ">"},
                {Tok.LEQ, "<="}
   
            }.ToList(), lexems);
        }
        
        [Test]
        public void TestComparisonsAndOps()
        {
            string text = @">+6<>>=<= +<> ><= <=";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            
            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 11);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.GT, ">"},
                {Tok.PLUS, "+"},
                {Tok.INUM, "6"},
                {Tok.NEQ, "<>"},
                {Tok.GEQ, ">="},
                {Tok.LEQ, "<="},
                {Tok.PLUS, "+"},
                {Tok.NEQ, "<>"},
                {Tok.GT, ">"},
                {Tok.LEQ, "<="},
                {Tok.LEQ, "<="},
   
            }.ToList(), lexems);
        }
        
    }
    
    [TestFixture]
    //[Ignore("This test is disabled")]
    public class TestSimpleLexerLineCmt
    {
        [Test]
        public void TestComment()
        {
            string text = @"set ts = 623 ~~ 23 sa 3";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            
            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 4);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.SET, "set" },
                {Tok.ID, "ts"},
                {Tok.ASSIGN, "="},
                {Tok.INUM, "623"},
   
            }.ToList(), lexems);
        }

        [Test] public void TestCommentFileEnd()
        {
            string text = @" set ts = 623 ~~";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            
            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 4);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.SET, "set" },
                {Tok.ID, "ts"},
                {Tok.ASSIGN, "="},
                {Tok.INUM, "623"},

            }.ToList(), lexems);
        }

        [Test]
        public void TestCommentFullLine()
        {
            string text = @"~~ set ts = 623";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 0, "Неверное количество лексем");
            CollectionAssert.AreEqual(new LexemList()
            {}.ToList(), lexems);
        }

        [Test] public void TestCommentNextLine()
        {
            string text = @" set ts = 623 ~~ cmt
                            set id = 22";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            
            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.AreEqual(8, lexems.Count);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.SET, "set"},
                {Tok.ID, "ts"},
                {Tok.ASSIGN, "="},
                {Tok.INUM, "623"},
                {Tok.SET, "set"},
                {Tok.ID, "id"},
                {Tok.ASSIGN, "="},
                {Tok.INUM, "22"},
   
            }.ToList(), lexems);
        }
        
    }
    
    [TestFixture]
    //[Ignore("This test is disabled")]
    public class TestSimpleLexerMultLineCmt
    {
        [Test]
        public void TestMultLineComment()
        {
            string text = @" set ts = 623 <// 23 sa 3 //>";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            
            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 4);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.SET, "set"},
                {Tok.ID, "ts"},
                {Tok.ASSIGN, "="},
                {Tok.INUM, "623"},

            }.ToList(), lexems);
        }

        [Test] 
        public void TestCommentFileEnd()
        {
            string text = @"set ts = 623 <// //>";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            
            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.IsTrue(lexems.Count == 4);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.SET, "set"},
                {Tok.ID, "ts"},
                {Tok.ASSIGN, "="},
                {Tok.INUM, "623"},

            }.ToList(), lexems);
        }
        
        [Test] 
        public void TestCommentNextLine()
        {
            string text = @" set ts = 623 <// cmt
                           cmt //> set id = 22";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);
            
            var lexems = TestSimpleLexer.getLexerOutput(l);
            Assert.AreEqual(8, lexems.Count);
            CollectionAssert.AreEqual(new LexemList()
            {
                {Tok.SET, "set"},
                {Tok.ID, "ts"},
                {Tok.ASSIGN, "="},
                {Tok.INUM, "623"},
                {Tok.SET, "set"},
                {Tok.ID, "id"},
                {Tok.ASSIGN, "="},
                {Tok.INUM, "22"},

            }.ToList(), lexems);
        }
        
        [Test] 
        public void TestCommentNotClosed()
        {
            string text = @" set ts = 623 <// cmt
                           cmt  set id = 22";
            TextReader inputReader = new StringReader(text);
            Lexer l = new Lexer(inputReader);

            Assert.Throws<LexerException>(() =>
            {
                var lexems = TestSimpleLexer.getLexerOutput(l);
            });


        }
        
    }
    
}