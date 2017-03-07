using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Calc1.Models;
using static Calc1.Models.RpnModel.TypeofToken;
using static Calc1.Models.OperatorPrecedence.Associative;

namespace Calc1
{
    /// <summary>
    /// A simple arithmatic calculator that will handle +, -, *, /, ^ and (). 
    /// There is no implied multiplication with parenthesis, only a way to modify the percedence of 
    /// the calculations.
    /// 
    /// Doubles were used for the numeric values to make it easier to handl the power function.
    /// 
    /// The general algorythm used is to first convert the infix notation to reverse polish notation 
    /// and then calculate the answer.
    /// 
    /// There a no parameters except the equation to solve so all arguments will be concatinated together.
    /// 
    /// Sample equations
    /// 4+6*5/25*(4+1) This should result with 10
    /// 2^10
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            var argBuilder = Initialize(args);

            //check to make sure we actually have an argument.
            if (argBuilder.Length == 0)
            {
                Console.WriteLine("Nothing to calculate.");
                return;
            }

            var calcstring = argBuilder.ToString();
            Console.WriteLine($"Calculating {calcstring}");

            var input = ParseInputString(calcstring);

            //Create an Reverse Polish Notation (RPN) representation of the input string
            var shunt = ShuntYard.ShuntRpnModel(input);
            if (!string.IsNullOrEmpty(shunt.Item2))
            {
                //problem with the shuntyard - display the error and end
                Console.WriteLine(shunt.Item2);
                return;
            }
            var rpn = shunt.Item1; 

            //calculate the result.
            var calcResult = CalculateRpn.RpnResult(rpn);
            if (!string.IsNullOrEmpty(calcResult.Item2))
            {
                //problem with the calculator portion - display the error and end
                Console.WriteLine(calcResult.Item2);
                return;
            }

            Console.WriteLine(calcResult.Item1);

        }

        private static StringBuilder Initialize(string[] args)
        {
            var argBuilder = new StringBuilder();

            // concatinate all the args into one calculation string
            foreach (var arg in args)
            {
                argBuilder.Append(arg);
            }

            return argBuilder;
        }

        private static List<string> ParseInputString(string calcString)
        {
            var b = new char[1];
            var input = new List<string>();
            //Parse the input string into a list
            using (var sr = new StringReader(calcString))
            {
                //NOTE: We could make the number variable a stringbuilder but I'm not sure for values less than 1000 if it
                //would make sense.
                var number = string.Empty;
                while (sr.Read(b, 0, 1) == 1)
                {
                    //Console.WriteLine($"{b[0]}");
                    double value;
                    if (double.TryParse(b[0].ToString(), out value))
                    {
                        number += b[0];
                    }
                    else
                    {
                        //add number to the list
                        //if two operators are together like * and ( then the number will be empty. i.e 1*(2+3) 
                        if (!string.IsNullOrEmpty(number)) input.Add(number);
                        input.Add(b[0].ToString());
                        number = string.Empty;
                    }
                }
                if (!string.IsNullOrEmpty(number))
                {
                    input.Add(number);
                }
            }

            return input;
        }
    }
}
