namespace Calc1.Models
{
    public class OperatorPrecedence
    {
        public enum Associative
        {
            NA,
            IsLeft,
            IsRight
        }
        public string Operator { get; set; }
        public int Precedence { get; set; }
        public Associative Associativity { get; set; }
    }
}