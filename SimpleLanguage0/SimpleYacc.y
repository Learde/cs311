%{
// Ёти объ€влени€ добавл€ютс€ в класс GPPGParser, представл€ющий собой парсер, генерируемый системой gppg
    public Parser(AbstractScanner<int, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%namespace SimpleParser

%token SET SUB RETURN IF ELSE WHILE END ID STRING CHAR PLUS MINUS NOT ADDRESS OR AND MOD DIV MULT DIVISION STREAM IS LT GT LEQ GEQ NEQ INUM ECHO ASSIGN RIGHT_BRACKET LEFT_BRACKET COMMA LEFT_SQUARE_BRACKET RIGHT_SQUARE_BRACKET LINE_BREAK NEW_LINE

%%

program		: operlist
			;

subprog		: suboperlist
			;

operlist	: operlist NEW_LINE
			| operlist NEW_LINE oper
			| oper
			;

suboperlist	: suboperlist oper LINE_BREAK
			| suboperlist NEW_LINE
			| oper LINE_BREAK
			;

oper		: assignment
			| subroutine
			| exprlist
			| statement
			;

assignment	: SET ID ASSIGN exprlist
			| SET ID ASSIGN arr
			;

arr			: LEFT_BRACKET RIGHT_BRACKET
			| LEFT_BRACKET arrlist RIGHT_BRACKET
			;

arrlist		: expr
			| arrlist COMMA expr
			;

exprlist	: expr
			| exprlist binop expr
			;

binop		: PLUS
			| MINUS
			| OR
			| AND
			| MOD
			| DIV
			| MULT
			| DIVISION
			| STREAM
			| IS
			| LT
			| GT
			| LEQ
			| GEQ
			| NEQ
			| ADDRESS
			;

expr		: call
			| unop
			| const
			| func
			;

call		: ID LEFT_BRACKET paramlist RIGHT_BRACKET
			| ID LEFT_BRACKET RIGHT_BRACKET
			| ID
			;

paramlist	: exprlist
			| paramlist COMMA exprlist
			;

unop		: PLUS exprlist
			| MINUS exprlist
			| ADDRESS exprlist
			| NOT exprlist
			;

const		: STRING
			| CHAR
			| INUM
			;

func		: ECHO
			;

subroutine	: SUB ID LINE_BREAK subprog END SUB
			;

statement	: ifst
			| whilest
			| returnst
			;

ifst		: IF LEFT_SQUARE_BRACKET exprlist RIGHT_SQUARE_BRACKET LINE_BREAK subprog END IF
			| IF LEFT_SQUARE_BRACKET exprlist RIGHT_SQUARE_BRACKET LINE_BREAK subprog ELSE LINE_BREAK subprog END IF
			;

whilest		: WHILE LEFT_SQUARE_BRACKET exprlist RIGHT_SQUARE_BRACKET LINE_BREAK subprog END WHILE
			;

returnst	: RETURN exprlist
			;

%%
