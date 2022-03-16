using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public enum Token_Class
{
    Number, DataTypeINT, DataTypeFloat, DataTypeString, Read, Write, Repeat, Until, IF, Elseif, Else,
    Then, Return, Endline, Identifier, MinusOp, MultiplyOp, DivideOp, PlusOp,
    Assign, Semicolon, LeftParentheses, RightParentheses, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, AndOp, OrOp, LeftBraces, RightBraces,
    Comma, end, String, Comment , IdentiferMain
}

namespace JASON_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.DataTypeINT);
            ReservedWords.Add("float", Token_Class.DataTypeFloat);
            ReservedWords.Add("string", Token_Class.DataTypeString);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.IF);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endline);
            ReservedWords.Add("end", Token_Class.end);
            ReservedWords.Add("main", Token_Class.IdentiferMain);


            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add(":=", Token_Class.Assign);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add("(", Token_Class.LeftParentheses);
            Operators.Add(")", Token_Class.RightParentheses);
            Operators.Add("{", Token_Class.LeftBraces);
            Operators.Add("}", Token_Class.RightBraces);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
            Operators.Add(",", Token_Class.Comma);



        }

        public void StartScanning(string SourceCode)
        {

            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;
                else if (CurrentChar == '"')
                {
                    j++;
                    while (SourceCode[j] != '"')
                    {
                        j++;
                    }
                    j++;

                    FindTokenClass(SourceCode.Substring(i, j - i));
                    i = j - 1;
                }
                else if ((CurrentChar >= 'A' && CurrentChar <= 'Z') || (CurrentChar >= 'a' && CurrentChar <= 'z')) //if you read a character
                {
                    while ((j < SourceCode.Length) && (SourceCode[j] >= 'A' && SourceCode[j] <= 'Z' || SourceCode[j] >= 'a' && SourceCode[j] <= 'z' || SourceCode[j] >= '0' && SourceCode[j] <= '9'))
                    {
                        j++;
                    }
                    FindTokenClass(SourceCode.Substring(i, j - i));
                    i = j - 1;
                }

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    while ((j < SourceCode.Length && SourceCode[j] >= '0' && SourceCode[j] <= '9') || (j < SourceCode.Length && SourceCode[j] == '.'))
                    {
                        j++;
                    }

                    FindTokenClass(SourceCode.Substring(i, j - i));
                    i = j - 1;

                }
                else if (CurrentChar == '{')
                {
                    FindTokenClass("{");
                }
                else if (CurrentChar == '}')
                {
                    FindTokenClass("}");
                }
                else if (CurrentChar == ')')
                {
                    FindTokenClass(")");
                }
                else if (CurrentChar == '(')
                {
                    FindTokenClass("(");
                }
                else if (CurrentChar == '*')
                {
                    FindTokenClass("*");
                }
                else if (CurrentChar == '+')
                {
                    FindTokenClass("+");
                }
                else if (CurrentChar == '/')
                {
                    if (j + 2 < SourceCode.Length && SourceCode[j + 1] == '*')
                    {
                        j += 2;
                        while (j < SourceCode.Length && SourceCode[j] != '/')
                        {
                            j++;
                        }

                        if (j < SourceCode.Length && SourceCode[j - 1] == '*' && SourceCode[j] == '/')
                        {
                            Token Tok = new Token();
                            Tok.lex = SourceCode.Substring(i, j - i + 1);
                            Tok.token_type = Token_Class.Comment;
                            Tokens.Add(Tok);
                            i = j;
                        }
                        else
                        {
                            Errors.Error_List.Add(SourceCode.Substring(i, j - i));
                            i = j;
                        }
                    }
                    else
                    {
                        FindTokenClass("/");
                      
                    }
                }
                else if (CurrentChar == '-')
                {
                    FindTokenClass("-");
                }
                else if (CurrentChar == ';')
                {
                    FindTokenClass(";");
                }

                else if (CurrentChar == '<')
                {
                    if (j + 1 < SourceCode.Length && SourceCode[j + 1] == '>')
                    {
                        FindTokenClass("<>");
                        j += 2;
                        i = j - 1;
                    }
                    else
                        FindTokenClass("<");
                }
                else if (CurrentChar == ':')
                {
                    if (j + 1 < SourceCode.Length && SourceCode[j + 1] == '=')
                    {
                        FindTokenClass(":=");
                        j += 2;
                        i = j - 1;
                    }
                    else
                        Errors.Error_List.Add(CurrentChar.ToString());
                }
                else if (CurrentChar == '>')
                {
                    FindTokenClass(">");
                }
                else if (CurrentChar == '=')
                {
                    FindTokenClass("=");
                }
                else if (CurrentChar == ',')
                {
                    FindTokenClass(",");
                }
                else if (CurrentChar == '&')
                {
                    if (j + 1 < SourceCode.Length && SourceCode[j + 1] == '&')
                    {
                        FindTokenClass("&&");
                        j += 2;
                        i = j - 1;
                    }
                    else
                        Errors.Error_List.Add(CurrentChar.ToString());
                }
                else if (CurrentChar == '|')
                {
                    if (j + 1 < SourceCode.Length && SourceCode[j + 1] == '|')
                    {
                        FindTokenClass("||");
                        j += 2;
                        i = j - 1;
                    }
                    else
                        Errors.Error_List.Add(CurrentChar.ToString());
                }
                else
                {
                    Errors.Error_List.Add(CurrentChar.ToString());
                }
            }
            TINY.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            // Token_Class TC;
            Token Tok = new Token();

            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            else if (Operators.ContainsKey(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }

            else if (isNumber(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);

            }
            else if (isString(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }
            else if (isIdentifier(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Token_Class.Identifier;
                Tokens.Add(Tok);
            }

            else
            {
                Errors.Error_List.Add(Lex);
            }
        }


        bool isNumber(string lex)
        {
            bool isValid = true;
            Regex rx = new Regex("^([0-9]+|[0-9]+\\.[0-9]+)$", RegexOptions.Compiled);
            if (rx.IsMatch(lex))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
            return isValid;
        }
        bool isIdentifier(string lex)
        {
            bool isValid = true;
            Regex rx = new Regex("^(^[a-zA-Z]([0-9a-zA-Z])*)$", RegexOptions.Compiled);
            if (rx.IsMatch(lex))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
            return isValid;
        }
        bool isString(string lex)
        {
            if (lex[0] == '"' && lex[lex.Length - 1] == '"')
            {
                return true;
            }
            return false;

            //Regex rx = new Regex("\"(\\w(\\s)*(\\W)*)+\"", RegexOptions.Compiled);

        }
    }
}
