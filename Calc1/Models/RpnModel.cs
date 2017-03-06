using System.Dynamic;

namespace Calc1.Models
{
    public class RpnModel
    {
        public enum TypeofToken : int
        {
            IsOperator,
            IsValue
        }
        public string Token { get; set; }
        public TypeofToken TokenType { get; set; }
    }
}
