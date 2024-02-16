%using SimpleParser;
%using QUT.Gppg;
%using System.Linq;

%namespace SimpleScanner

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  {Digit}+
REALNUM {INTNUM}\.{INTNUM}
ID {Alpha}{AlphaDigit}* 
STRING				\"[^\"]*\"
CHAR				\'[^\']?\'
DotChr				[^\r\n]
OneLineCmnt			~~{DotChr}*\r\n
MultLineCmntBegin	<\/\/
MultLineCmntEnd		\/\/>
LineBreak			#[ ]*\r\n
NewLine				[\r][\n]

%{
	public int LexValueInt;
	public double LexValueDouble;
	public List<string> LexIds;
%}

%x COMMENT

%%

{LineBreak} {
	return (int)Tokens.LINE_BREAK;
}

{NewLine} {
	return (int)Tokens.NEW_LINE;
}

"," {
	return (int)Tokens.COMMA;
}

{INTNUM} { 
	return (int)Tokens.INUM; 
}

"+" {
	return (int)Tokens.PLUS;
}

"-" {
	return (int)Tokens.MINUS;
}

"*" {
	return (int)Tokens.MULT;
}

"/" {
	return (int)Tokens.DIVISION;
}

"=" { 
  return (int)Tokens.ASSIGN;
}

"|" { 
	return (int)Tokens.STREAM;
}

"[" { 
	return (int)Tokens.LEFT_SQUARE_BRACKET;
}

"]" { 
	return (int)Tokens.RIGHT_SQUARE_BRACKET;
}

"(" { 
	 return (int)Tokens.LEFT_BRACKET;
}

")" { 
	return (int)Tokens.RIGHT_BRACKET;
}

"<" { 
	return (int)Tokens.LT;
}

">" { 
	return (int)Tokens.GT;
}

"<=" { 
	return (int)Tokens.LEQ;
}

">=" { 
	return (int)Tokens.GEQ;
}

"<>" { 
	return (int)Tokens.NEQ;
}

"@" {
	return (int)Tokens.ADDRESS;
}

{STRING} {
	return (int)Tokens.STRING;
}

{CHAR} {
	return (int)Tokens.CHAR;
}

{ID} { 
	int res = ScannerHelper.GetIDToken(yytext);
	return res;
}

{MultLineCmntBegin} { 
	BEGIN(COMMENT);
}

<COMMENT> {MultLineCmntEnd} { 
	BEGIN(INITIAL);
}

{OneLineCmnt} {
}

[^ \r\n] {
	LexError();
	return (int)Tokens.EOF; // конец разбора
}

%{
  yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); // позици€ символа (терминального или нетерминального), возвращаема€ @1 @2 и т.д.
%}

%%

public override void yyerror(string format, params object[] args) // обработка синтаксических ошибок
{
  var ww = args.Skip(1).Cast<string>().ToArray();
  string errorMsg = string.Format("({0},{1}): ¬стречено {2}, а ожидалось {3}", yyline, yycol, args[0], string.Join(" или ", ww));
  throw new SyntaxException(errorMsg);
}

public void LexError()
{
	string errorMsg = string.Format("({0},{1}): Ќеизвестный символ {2}", yyline, yycol, yytext);
    throw new LexException(errorMsg);
}

class ScannerHelper 
{
  private static Dictionary<string,int> keywords;

  static ScannerHelper() 
  {
    keywords = new Dictionary<string,int>();
    keywords.Add("set",(int)Tokens.SET);
    keywords.Add("end",(int)Tokens.END);
    keywords.Add("if",(int)Tokens.IF);
	keywords.Add("else",(int)Tokens.ELSE);
	keywords.Add("while",(int)Tokens.WHILE);
	keywords.Add("sub",(int)Tokens.SUB);
	keywords.Add("return",(int)Tokens.RETURN);
	keywords.Add("or",(int)Tokens.OR);
	keywords.Add("and",(int)Tokens.AND);
	keywords.Add("not",(int)Tokens.NOT);
	keywords.Add("echo",(int)Tokens.ECHO);
	keywords.Add("mod",(int)Tokens.MOD);
	keywords.Add("div",(int)Tokens.DIV);
	keywords.Add("is",(int)Tokens.IS);
  }
  public static int GetIDToken(string s)
  {
    if (keywords.ContainsKey(s.ToLower())) // €зык нечувствителен к регистру
      return keywords[s];
    else
      return (int)Tokens.ID;
  }
}
