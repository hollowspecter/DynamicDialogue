parser grammar BarkParser;

options { tokenVocab=BarkLexer; }

talk : (rule | response)+ ;

rule : RULE WORD EXPRESSION_START rule_body EXPRESSION_END ;

rule_body : conditions rule_response remember? trigger? ;

conditions	: CONDITIONS condition_statement+;

condition_statement	: WORD | equals_statement ;

equals_statement : WORD OPERATOR_LOGICAL_EQUALS (NUMBER | BOOLEAN | WORD | TEXT | MENTION) ; // TODO add boolean?

rule_response : RESPONSE WORD;

remember : REMEMBER equals_statement+ ;

trigger : TRIGGER MENTION WORD ;

response : RESPONSE WORD EXPRESSION_START response_body EXPRESSION_END ;

response_body : line+ ;

line : TEXT ;