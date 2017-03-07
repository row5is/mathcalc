using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc1.Models;
using static Calc1.Models.RpnModel.TypeofToken;
namespace Calc1
{
    /// <summary>
    /// This will step through an RPN formatted string and return the result of the calculation.
    /// 
    /// If an error is encounted the errorMessage value will be set.
    /// </summary>
    class CalculateRpn
    {
        public static Tuple<double, string> RpnResult(List<RpnModel> rpn)
        {
            var errorMessage = string.Empty;
            var token = new Stack<double>();

            foreach (var part in rpn)
            {
                //Console.WriteLine(part);
                if (part.TokenType == IsValue)
                {
                    double value;
                    double.TryParse(part.Token, out value);
                    //a value so push it onto the stack
                    token.Push(value);
                }
                else
                {
                    //operator - so calculate 
                    //Check to make sure we have enough tokens
                    if (token.Count <= 1) continue;
                    
                    // pull off the tokens, calculate and place the result back on the stack
                    var right = token.Pop();
                    var left = token.Pop();
                    switch (part.Token)
                    {
                        case "+":
                            token.Push(left + right);
                            break;
                        case "-":
                            token.Push(left - right);
                            break;
                        case "*":
                            token.Push(left * right);
                            break;
                        case "/":
                            token.Push(left / right);
                            break;
                        case "^":
                            token.Push(Math.Pow(left, right));
                            break;
                        default:
                            errorMessage = $"error - unknown operator found '{part.Token}'";
                            return new Tuple<double, string>(0,errorMessage);
                            break;
                    }
                }
            }
            //At this point we should only have one value on the stack. If we have more or less then we have a problem.
            if (token.Count != 1)
            {
                errorMessage = "error - problem calculating value.";
            }
            return new Tuple<double, string>(token.Pop(), errorMessage);
        }
    }
}
