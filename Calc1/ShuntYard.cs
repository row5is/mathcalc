using System;
using System.Collections.Generic;
using System.Linq;
using Calc1.Models;
using static Calc1.Models.RpnModel.TypeofToken;
using static Calc1.Models.OperatorPrecedence.Associative;

namespace Calc1
{
    /// <summary>
    /// The ShuntYard will pull off numbers and operators and place them in a list in RPN order. 
    /// Each operator has a precedence that is used to determine the placement of the operator in 
    /// relation to the numbers.
    ///
    /// If an unknown operator is encountered it will result in an error.
    /// </summary>
    class ShuntYard
    {
        /// testdddd
        //Setup operators
        private static readonly List<OperatorPrecedence> Operators = new List<OperatorPrecedence>
        {
            new OperatorPrecedence
            {
                Operator = "^",
                Precedence = 4,
                Associativity = IsRight
            },

            new OperatorPrecedence
            {
                Operator = "*",
                Precedence = 3,
                Associativity = IsLeft
            },
            new OperatorPrecedence
            {
                Operator = "/",
                Precedence = 3,
                Associativity = IsLeft
            },
            new OperatorPrecedence
            {
                Operator = "+",
                Precedence = 2,
                Associativity = IsLeft
            },
            new OperatorPrecedence
            {
                Operator = "-",
                Precedence = 2,
                Associativity = IsLeft
            },
            new OperatorPrecedence
            {
                Operator = "(",
                Precedence = 0,
                Associativity = NA
            },
            new OperatorPrecedence
            {
                Operator = ")",
                Precedence = 0,
                Associativity = NA
            }

        };
        public static Tuple<List<RpnModel>,string> ShuntRpnModel(List<string> input)
        {
            var op = new Stack<OperatorPrecedence>();
            var rpn = new List<RpnModel>();
            var errorMessage = string.Empty;

            foreach (var part in input)
            {
                double value;
                if (double.TryParse(part, out value))
                {
                    rpn.Add(new RpnModel { Token = part, TokenType = IsValue });
                }
                else
                {
                    // not a number so assume an operator - if an operator is not found we will ignore it.
                    var op1 = Operators.FirstOrDefault(o => o.Operator == part);
                    switch (part)
                    {
                        case "(":
                            op.Push(op1);
                            break;
                        case ")":
                            //pop operators off the stack until a left parenthesis

                            while (op.Count > 0 && op.Peek().Operator != "(")
                            {
                                rpn.Add(new RpnModel { Token = op.Pop().Operator, TokenType = IsOperator });
                                //op.RemoveAt(op.Count - 1);
                            }
                            //make sure we haven't reached the end without finding a "("
                            if (op.Count == 0)
                            {
                                //Problem - let the user know and exit the function
                                errorMessage = "error - unbalanced parenthesis, no matching left parenthesis.";
                                return new Tuple<List<RpnModel>, string>(null, errorMessage);
                            }
                            //remove the left paren
                            op.Pop();

                            break;
                        default:
                            if (op1 != null)
                            {
                                if (op.Count > 0)
                                {
                                    var op2 = op.Peek();
                                    if ((op1.Associativity == IsLeft && op1.Precedence <= op2.Precedence) ||
                                        op1.Associativity == IsRight && op1.Precedence < op2.Precedence)
                                    {
                                        rpn.Add(new RpnModel { Token = op2.Operator, TokenType = IsOperator });
                                        op.Pop();
                                    }

                                }
                                op.Push(op1);
                            }
                            else
                            {
                                errorMessage = $"error - unkown operator '{part}'\nValid operators are\n";
                                errorMessage = Operators.Aggregate(errorMessage, (current, operatorPrecedence) => current + $"{operatorPrecedence.Operator}\n");
                            }
                            break;
                    }

                }
                //Console.WriteLine(part);
            }
            //get the remaining operators
            while (op.Count > 0)
            {
                //we should not have any left parenthesis left on the stack. If we do then we have a problem
                if (op.Peek().Operator == "(")
                {
                    errorMessage = "error - unbalanced parenthesis, no matching right parenthesis.";
                }
                rpn.Add(new RpnModel { Token = op.Pop().Operator, TokenType = IsOperator });
            }
            return new Tuple<List<RpnModel>, string>(rpn, errorMessage);
        }
    }
}
