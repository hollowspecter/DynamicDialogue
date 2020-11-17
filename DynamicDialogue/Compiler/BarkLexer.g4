lexer grammar BarkLexer;

fragment A	: ('A'|'a') ;
fragment B	: ('B'|'b') ;
fragment C	: ('C'|'c') ;
fragment D	: ('D'|'d') ;
fragment E	: ('E'|'e') ;
fragment F	: ('F'|'f') ;
fragment G	: ('G'|'g') ;
fragment H	: ('H'|'h') ;
fragment I	: ('I'|'i') ;
fragment J	: ('J'|'j') ;
fragment K	: ('K'|'k') ;
fragment L	: ('L'|'l') ;
fragment M	: ('M'|'m') ;
fragment N	: ('N'|'n') ;
fragment O	: ('O'|'o') ;
fragment P	: ('P'|'p') ;
fragment Q	: ('Q'|'q') ;
fragment R	: ('R'|'r') ;
fragment S	: ('S'|'s') ;
fragment T	: ('T'|'t') ;
fragment U	: ('U'|'u') ;
fragment V	: ('V'|'v') ;
fragment W	: ('W'|'w') ;
fragment X	: ('X'|'x') ;
fragment Y	: ('Y'|'y') ;
fragment Z	: ('Z'|'z') ;

fragment AT : '@' ;

fragment DIGIT	: [0-9] ;
fragment INT : DIGIT+ ;
fragment LOWERCASE : [a-z] ;
fragment UPPERCASE : [A-Z] ;
fragment TRUE : T R U E ;
fragment FALSE : F A L S E ;

OPERATOR_LOGICAL_EQUALS : '=' ;

EXPRESSION_START : '{' ;
EXPRESSION_END : '}' ;

NUMBER : INT | INT '.' INT ;
BOOLEAN : TRUE | FALSE ;

RULE : R U L E ;

RESPONSE : R E S P O N S E ;

CONDITIONS : C O N D I T I O N S ;

REMEMBER : R E M E M B E R ;

TRIGGER : T R I G G E R ;

WORD : (LOWERCASE | UPPERCASE | '_' | DIGIT)+ ;

MENTION : AT WORD ;

// A run of text. Escaped quotes and backslashes are allowed.
TEXT : '"' (~('"' | '\\' | '\r' | '\n') | '\\' ('"' | '\\'))* '"' ;

WHITESPACE : (' '|'\t')+ -> skip ;

COMMENT : '//' ~('\r'|'\n')* -> skip;

NEWLINE : ('\r'? '\n' | '\r')+ -> skip ;
