%using ScannerHelper;
%namespace SimpleScanner

Alpha 				[a-zA-Z_]
Digit				[0-9] 
AlphaDigit			{Alpha}|{Digit}
INTNUM				{Digit}+
REALNUM				{INTNUM}\.{INTNUM}
ID					{Alpha}{AlphaDigit}* 
STRING				\"[^\"]*\"
CHAR				\'[^\']?\'
DotChr				[^\r\n]
OneLineCmnt			~~{DotChr}*
MultLineCmntBegin	<\/\/
MultLineCmntEnd		\/\/>


// Здесь можно делать описания типов, переменных и методов - они попадают в класс Scanner
%{
	public int LexValueInt;
	public double LexValueDouble;
	public List<string> LexIds;
%}

%x COMMENT

%%
"#" {
	return (int)Tok.LINE_BREAK;
}

"," {
	return (int)Tok.COMMA;
}

{INTNUM} { 
	LexValueInt = int.Parse(yytext);
	return (int)Tok.INUM;
}

"+" {
	return (int)Tok.PLUS;
}

"-" {
	return (int)Tok.MINUS;
}

"*" {
	return (int)Tok.MULT;
}

"/" {
	return (int)Tok.DIVISION;
}

"=" { 
  return (int)Tok.ASSIGN;
}

set {
	return (int)Tok.SET;
}

"|" { 
	return (int)Tok.STREAM;
}

"[" { 
	return (int)Tok.LEFT_SQUARE_BRACKET;
}

"]" { 
	return (int)Tok.RIGHT_SQUARE_BRACKET;
}

"(" { 
	 return (int)Tok.LEFT_BRACKET;
}

")" { 
	return (int)Tok.RIGHT_BRACKET;
}

if {
	return (int)Tok.IF;
}

else {
	return (int)Tok.ELSE;
}

while {
	return (int)Tok.WHILE;
}

sub {
	return (int)Tok.SUB;
}

return {
	return (int)Tok.RETURN;
}

end { 
	return (int)Tok.END;
}

or { 
	return (int)Tok.OR;
}

and { 
	return (int)Tok.AND;
}

not { 
	return (int)Tok.NOT;
}

"<" { 
	return (int)Tok.LT;
}

">" { 
	return (int)Tok.GT;
}

"<=" { 
	return (int)Tok.LEQ;
}

">=" { 
	return (int)Tok.GEQ;
}

"<>" { 
	return (int)Tok.NEQ;
}

"@" {
	return (int)Tok.ADDRESS;
}

echo {
	return (int)Tok.ECHO;
}

mod {
	return (int)Tok.MOD;
}

div {
	return (int)Tok.DIV;
}

{STRING} {
	return (int)Tok.STRING;
}

{CHAR} {
	return (int)Tok.CHAR;
}

{ID} { 
	return (int)Tok.ID;
}

{MultLineCmntBegin} { 
	BEGIN(COMMENT);
	LexIds = new List<string>();
}

<COMMENT> {MultLineCmntEnd} { 
	BEGIN(INITIAL);
	return (int)Tok.MULT_LINE_CMNT;
}

<COMMENT>{ID} {
	LexIds.Add(yytext);
}

{OneLineCmnt} {
	return (int)Tok.ONE_LINE_CMNT;
}


[^ \r\n] {
	LexError();
	return 0; // конец разбора
}

%%

// Здесь можно делать описания переменных и методов - они тоже попадают в класс Scanner

public void LexError()
{
	Console.WriteLine("({0},{1}): Неизвестный символ {2}", yyline, yycol, yytext);
}

public string TokToString(Tok tok)
{
	switch (tok)
	{
		case Tok.ID:
			return tok + " " + yytext;
		case Tok.INUM:
			return tok + " " + LexValueInt;
		case Tok.STRING:
			return tok + " " + yytext;
		case Tok.MULT_LINE_CMNT:
			return tok + " IDS inside: (" + string.Join(" ", LexIds.ToArray()) + ")";
		default:
			return tok + "";
	}
}

