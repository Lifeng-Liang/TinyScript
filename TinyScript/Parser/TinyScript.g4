grammar TinyScript;

options {
	language=CSharp;
}

//==============================================================

program
	: statement+ EOF
	;

//==============================================================

statement
	: ifStatement
	| blockStatement
	| assignStatement
	| declareStatement
	| printStatement
	| whileStatement
	| doWhileStatement
	| forStatement
	;
ifStatement
	: 'if' quoteExpr blockStatement
	| 'if' quoteExpr blockStatement 'else' blockStatement
	;
quoteExpr
	: '(' expression ')'
	;
blockStatement
	: '{' (statement)* '}'
	;
assignStatement
	: assign ';'
	;
declareStatement
	: declareExpression ';'
	;
printStatement
	: 'print' '(' expression ')' ';'
	;
whileStatement
	: 'while' '(' expression ')' blockStatement
	;
doWhileStatement
	: 'do' blockStatement 'while' '(' expression ')' ';'
	;
forStatement
	: 'for' '(' commonExpression ';' expression ';' assignAbleStatement ')' blockStatement
	;
commonExpression
	: declareExpression
	| assignAbleStatement
	;
assignAbleStatement
	: assign
	| expression
	;

//==============================================================

declareExpression
	: basicType declarators
	;
expression
	: andAndExpression ('||' andAndExpression)*
	;
andAndExpression
	: cmpExpression ('&&' cmpExpression)*
	;
cmpExpression
	: addExpression (('==' | '!=' | '<' | '<=' | '>' | '>=') addExpression)?
	;
addExpression
	: mulExpression (('+' | '-') mulExpression)*
	;
mulExpression
	: unaryExpression (('*' | '/') unaryExpression)*
	;
unaryExpression
	: primaryExpression
	| ('-' | '!') unaryExpression
	;
primaryExpression
	: variableExpression
	| numericLiteral
	| '(' expression ')'
	;
variableExpression
	: Identifier
	| 'true'
	| 'false'
	| StringLiteral
	;
//==============================================================

basicType
	: 'decimal'
	| 'string'
	| 'bool'
	| 'var'
	;
declarators
	: assign (',' assign)*
	;
assign
	: Identifier '=' expression
	;

//==============================================================

numericLiteral
	: Decimal
	;
StringLiteral
	: '"' (~[\\\r\n])*? '"'
	;
Decimal
	: '0' ('.' (DigitChar)* NonZeroDigit)?
	| NonZeroDigit (DigitChar)* ('.' (DigitChar)* NonZeroDigit)?
	;
fragment NonZeroDigit
	: '1'..'9'
	;
fragment DigitChar
	: '0'
	| NonZeroDigit
	| '_'
	;

//==============================================================

Identifier
	: (IdentifierStart) (IdentifierChar)*
	;
fragment IdentifierChar
	: (IdentifierStart)
	| '0'
	| NonZeroDigit
	;
fragment IdentifierStart
	: '_'
	| Letter
	// | UniversalAlpha
	;
fragment Letter
	: 'a'..'z'
	| 'A'..'Z'
	;

//==============================================================

LineComment
	: '//' ~('\n'|'\r')* '\r'? '\n' -> skip
	;
Comment
	: '/*' (.)*? '*/' -> skip
	;
WhiteSpace
	: (' ' | '\r' | '\n' | '\t') -> skip
	;
