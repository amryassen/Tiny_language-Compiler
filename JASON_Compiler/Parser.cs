using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public  Node root;
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
          if (TokenStream[InputPointer+1].token_type == Token_Class.IdentiferMain)
            {
              
                program.Children.Add(Main());
            }
            else
            {
                program.Children.Add(Function());
         
               
                program.Children.Add(Main());
            }

                MessageBox.Show("Success");
            return program;
        }
        Node DataType()
        {

            Node dataType = new Node("datatype");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.DataTypeINT)
                {
                    dataType.Children.Add(match(Token_Class.DataTypeINT));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.DataTypeFloat)
                {
                    dataType.Children.Add(match(Token_Class.DataTypeFloat));
                }
                else
                {
                    dataType.Children.Add(match(Token_Class.DataTypeString));
                }
            }
            else
            {
                dataType.Children.Add(match(Token_Class.DataTypeINT));
                return dataType;
            }
            return dataType;
        }
        Node Main()
        {
            Node main = new Node("Main");
            main.Children.Add(DataType());
            main.Children.Add(match(Token_Class.IdentiferMain));
            main.Children.Add(match(Token_Class.LeftParentheses));
            main.Children.Add(match(Token_Class.RightParentheses));
            main.Children.Add(FunctionBody());
            return main;
        }
        Node Function()
        { 
            Node function = new Node("Function");
            function.Children.Add(FunctionDef());
            function.Children.Add(FunctionBody());
            if (InputPointer < TokenStream.Count)
            {
                if ((TokenStream[InputPointer].token_type == Token_Class.DataTypeINT || TokenStream[InputPointer].token_type == Token_Class.DataTypeFloat
                || TokenStream[InputPointer].token_type == Token_Class.DataTypeString) && TokenStream[InputPointer + 1].token_type != Token_Class.IdentiferMain)
                {
                    function.Children.Add(Function());
                }
            }
            return function;
        }
        Node FunctionDef()
        {
            Node funcDef = new Node("FunctionDef");
            funcDef.Children.Add(DataType());
            funcDef.Children.Add(match(Token_Class.Identifier));
            funcDef.Children.Add(match(Token_Class.LeftParentheses));
            funcDef.Children.Add(Parameters());
            funcDef.Children.Add(match(Token_Class.RightParentheses));
         
            return funcDef;
        }
        // How to keep get at least one parameter
        Node Parameters()
        {
            Node parameters = new Node("Parameters");
            if (TokenStream[InputPointer].token_type == Token_Class.DataTypeINT || TokenStream[InputPointer].token_type == Token_Class.DataTypeFloat
                    || TokenStream[InputPointer].token_type == Token_Class.DataTypeString)
            {
               
                parameters.Children.Add(DataType());
                parameters.Children.Add(match(Token_Class.Identifier));
                parameters.Children.Add(_Parameters());
            }
            else
            {
                return null;
            }
            return parameters;
        }
        Node _Parameters()
        {
            Node _parameters = new Node("_parameters");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                _parameters.Children.Add(match(Token_Class.Comma));
                _parameters.Children.Add(Parameters());
            }
           else {
                return null;
            }
            return _parameters;
        }
        Node FunctionBody()
        {
            Node funcBody = new Node("FuncBody");
            funcBody.Children.Add(match(Token_Class.LeftBraces));
            funcBody.Children.Add(Statement());
           funcBody.Children.Add(return_statement());
            funcBody.Children.Add(match(Token_Class.RightBraces));
            return funcBody;
        }

         // Return Statement
        Node return_statement()
        {
            Node read_state = new Node("return_statement");
            read_state.Children.Add(match(Token_Class.Return));
            read_state.Children.Add(Exp());
            read_state.Children.Add(match(Token_Class.Semicolon));
            return read_state;
        }



        Node Exp()
        { 
            Node exp = new Node("Exp");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    exp.Children.Add(match(Token_Class.String));
                }
                else if ((TokenStream[InputPointer].token_type == Token_Class.Number || TokenStream[InputPointer].token_type == Token_Class.Identifier) 
                    && (TokenStream[InputPointer+1].token_type == Token_Class.PlusOp ||
                    TokenStream[InputPointer+1].token_type == Token_Class.MinusOp ||
                    TokenStream[InputPointer+1].token_type == Token_Class.DivideOp ||
                    TokenStream[InputPointer+1].token_type == Token_Class.MultiplyOp))
                {
                    exp.Children.Add(Equ());

                }
                else exp.Children.Add(Term());
            }
            return exp;
        }

        Node Term()
        {

            Node term = new Node("Term");
                if (TokenStream[InputPointer].token_type == Token_Class.Number)
                {
                    term.Children.Add(match(Token_Class.Number));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Identifier && TokenStream[InputPointer+1].token_type != Token_Class.LeftParentheses)
                {
                    term.Children.Add(match(Token_Class.Identifier));
                }
                else term.Children.Add(FUN_CALL());
            return term;
        }
        Node FUN_CALL()
        {
            Node function_call = new Node("FUN_CALL");
            function_call.Children.Add(match(Token_Class.Identifier));
            function_call.Children.Add(Param());

            return function_call;
        }

        Node Param()
        {
            Node param = new Node("Param");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.LeftParentheses)
                {
                    param.Children.Add(match(Token_Class.LeftParentheses));
                    param.Children.Add(Parameter_fun_call());
                    param.Children.Add(match(Token_Class.RightParentheses));
                }
                else return null;
            }
            return param;
        }
        Node Parameter_fun_call()
        {
            Node parameter_fun_call = new Node("Parameter_fun_call");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    parameter_fun_call.Children.Add(match(Token_Class.Identifier));
                    parameter_fun_call.Children.Add(another_identifier());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Number)
                {
                    parameter_fun_call.Children.Add(match(Token_Class.Number));
                    parameter_fun_call.Children.Add(another_identifier());
                }
                else return null;
            }
            return parameter_fun_call;
        }

        Node another_identifier()
        {
            Node Another_identifier = new Node("another_identifier");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    Another_identifier.Children.Add(match(Token_Class.Comma));
                    Another_identifier.Children.Add(Parameter_fun_call());
                }
                else return null;
            }
            return Another_identifier;
        }
        Node Equ()
        {
            Node equ = new Node("Equ");
                if (TokenStream[InputPointer].token_type == Token_Class.LeftParentheses)
                {
                    equ.Children.Add(match(Token_Class.LeftParentheses));
                    equ.Children.Add(Equ());
                    equ.Children.Add(match(Token_Class.RightParentheses));
                    equ.Children.Add(new_equ());
                }
                else
                {
                    equ.Children.Add(Term());
                    equ.Children.Add(new_equ());
                }
            return equ;
        }

        Node new_equ()
        {
             Node New_equ = new Node("new_eq");
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp ||
                    TokenStream[InputPointer].token_type == Token_Class.MinusOp ||
                    TokenStream[InputPointer].token_type == Token_Class.DivideOp ||
                    TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                {
                    New_equ.Children.Add(Arith_Op());
                    New_equ.Children.Add(Equ());
                    Debug.WriteLine(TokenStream[InputPointer].token_type);
                    New_equ.Children.Add(new_equ());
                    return New_equ;

                }
             return null;
        }

        Node Arith_Op()
        {
            Node arith_op = new Node("Arith_Op");

                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                {
                    arith_op.Children.Add(match(Token_Class.PlusOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                {
                    arith_op.Children.Add(match(Token_Class.MinusOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                {
                   

                    arith_op.Children.Add(match(Token_Class.DivideOp));
              

                }
                else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                {
                    arith_op.Children.Add(match(Token_Class.MultiplyOp));
                }
            return arith_op;
        }

        private Node condition()
        {
            Node node = new Node("condition");
            node.Children.Add(match(Token_Class.Identifier));
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp || TokenStream[InputPointer].token_type == Token_Class.LessThanOp
                || TokenStream[InputPointer].token_type == Token_Class.EqualOp || TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
                {
                   
                    node.Children.Add(match(TokenStream[InputPointer].token_type));
                }
                node.Children.Add(Term());
            }
            return node;
        }

        private Node Condition_Statement()
        {
            Node node = new Node("Condition_Statement"); 
            node.Children.Add(condition());
            node.Children.Add(Rest_of_conditions());
            return node;
        }
        private Node Rest_of_conditions()
        {
            Node node = new Node("Rest_of_conditions");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.AndOp || TokenStream[InputPointer].token_type == Token_Class.OrOp)
                {
                    node.Children.Add(match(TokenStream[InputPointer].token_type));
                   
                    node.Children.Add(condition());
                   
                    node.Children.Add(Rest_of_conditions());
                   // node.Children.Add(Term());
                    return node;
                }
            }
            return null;
        }
        private Node If_Statement()
        {
            Node node = new Node("If_Statement");
            node.Children.Add(match(Token_Class.IF));
            node.Children.Add(Condition_Statement());
            Debug.Write("here");
            node.Children.Add(match(Token_Class.Then));
            Debug.Write("here2");
            node.Children.Add(Second_part_of_if_statement());
            node.Children.Add(second_part_of_else_if_statement());
            return node;
        }
        //private Node Statement()
        //{
        //    Node node = new Node("Statement");
        //    if (InputPointer < TokenStream.Count)
        //    {
        //        if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
        //        {
        //            node.Children.Add(Assignment_Statement());
        //            node.Children.Add(Statement());
        //            return node;
        //        }
        //        else if (TokenStream[InputPointer].token_type == Token_Class.DataTypeINT || TokenStream[InputPointer].token_type == Token_Class.DataTypeFloat
        //            || TokenStream[InputPointer].token_type == Token_Class.DataTypeString)
        //        {
        //            node.Children.Add(Declaration_Statement());
        //            node.Children.Add(Statement());
        //            return node;
        //        }
        //        else if (TokenStream[InputPointer].token_type == Token_Class.Write)
        //        {
        //            node.Children.Add(write_statement());
        //            node.Children.Add(Statement());
        //            return node;
        //        }
        //        else if (TokenStream[InputPointer].token_type == Token_Class.Read)
        //        {
        //            node.Children.Add(read_statement());
        //            node.Children.Add(Statement());
        //            return node;
        //        }
        //        else if (TokenStream[InputPointer].token_type == Token_Class.IF)
        //        {
        //            node.Children.Add(If_Statement());
        //            node.Children.Add(Statement());
        //            return node;
        //        }
        //        else if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
        //        {
        //            node.Children.Add(Repeat_Statement());
        //            node.Children.Add(Statement());
        //            return node;
        //        }
        //    }
        //    return null;
        //}
        private Node second_part_of_else_if_statement()
        {
            Node node = new Node("second_part_of_else_if_statement");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Else)
                {
                    node.Children.Add(Else_Statement());
                    return node;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.end)
                {
                    node.Children.Add(match(Token_Class.end));
                    return node;
                }
                else
                {
                    node.Children.Add(Second_part_of_if_statement());
                    node.Children.Add(Else_If_Statement());
                    return node;
                }
            }
            return node;
        }
        private Node Else_If_Statement()
        {
            Node node = new Node("Else_If_Statement");
            node.Children.Add(match(Token_Class.Elseif));
            node.Children.Add(Condition_Statement());
            node.Children.Add(match(Token_Class.Then));
            node.Children.Add(Second_part_of_if_statement());
            node.Children.Add(second_part_of_else_if_statement());
            return node;
        }
        private Node Else_Statement()
        {
            Node node = new Node("Else_Statement");
            node.Children.Add(match(Token_Class.Else));
            node.Children.Add(Second_part_of_if_statement());
            node.Children.Add(match(Token_Class.end));
            return node;
        }
        private Node Second_part_of_if_statement()
        {
            Node node = new Node("Second_part_of_if_statement");
            node.Children.Add(Statement());
            return node;
        }
        private Node Repeat_Statement()
        {
            Node node = new Node("Repeat_Statement");
            node.Children.Add(match(Token_Class.Repeat));
            node.Children.Add(Statement());
            node.Children.Add(match(Token_Class.Until));
           node.Children.Add(Condition_Statement());
            return node;
        }
        Node Statement()
        {
            Node statement = new Node("Statement");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    statement.Children.Add(Assignment_Statement());
                    statement.Children.Add(Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Read)
                {
                    statement.Children.Add(read_statement());
                    statement.Children.Add(Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Write)
                {
                    statement.Children.Add(write_statement());
                    statement.Children.Add(Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.DataTypeINT ||
                         TokenStream[InputPointer].token_type == Token_Class.DataTypeFloat ||
                         TokenStream[InputPointer].token_type == Token_Class.DataTypeString)
                {

                    statement.Children.Add(Declaration_Statement());
                    statement.Children.Add(Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.IF)
                {
                    statement.Children.Add(If_Statement());
                    statement.Children.Add(Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
                {
                    statement.Children.Add(Repeat_Statement());
                    statement.Children.Add(Statement());
                  
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Comment)
                {
                    statement.Children.Add(match(Token_Class.Comment));
                    statement.Children.Add(Statement());
                }
                else
                {
                    return null;
                }

            }

            return statement;
        }
        Node Assignment_Statement()
        {
            Node assign = new Node("Assignment_Statement");
            assign.Children.Add(match(Token_Class.Identifier));
            assign.Children.Add(match(Token_Class.Assign));
            assign.Children.Add(Exp());
            assign.Children.Add(match(Token_Class.Semicolon));
            return assign;
        }

        Node Declaration_Statement()
        {
            Node declar = new Node("Declaration_Statement");
            declar.Children.Add(DataType());
            declar.Children.Add(match(Token_Class.Identifier));
            declar.Children.Add(DS2());
            declar.Children.Add(DS1());
            declar.Children.Add(match(Token_Class.Semicolon));
            return declar;
        }
        
        Node DS1()
        {
            Node ds1 = new Node("DS1");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    ds1.Children.Add(match(Token_Class.Comma));
                    ds1.Children.Add(match(Token_Class.Identifier));
                    ds1.Children.Add(DS2());
                }
                else return null;
            }

            return ds1;
        }

        Node DS2()
        {
            Node ds2 = new Node("DS2");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Assign)
                {
                    ds2.Children.Add(match(Token_Class.Assign));
                    ds2.Children.Add(Exp());
                }
                else return null;
            }
            return ds2;
        }

        Node write_statement()
        {
            Node write = new Node("write_statement");
            write.Children.Add(match(Token_Class.Write));
            write.Children.Add(Out());
            write.Children.Add(match(Token_Class.Semicolon));
            return write;
        }

        Node Out()
        {
            Node oute = new Node("Out");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Endline)
                {
                    oute.Children.Add(match(Token_Class.Endline));
                }
                else oute.Children.Add(Exp());
            }
            return oute;
        }

        Node read_statement()
        {
            Node read_state = new Node("read_statement");
            read_state.Children.Add(match(Token_Class.Read));
            read_state.Children.Add(match(Token_Class.Identifier));
            read_state.Children.Add(match(Token_Class.Semicolon));
            return read_state;
        }


        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
