using Pi.Parser.Syntax;
using Pi.Parser.Syntax.Declarations;
using Pi.Parser.Syntax.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Pi.Interpreter
{
    internal class PiInterpreter
    {
        private readonly Node[] Nodes;

        private readonly Context Context = new Context();

        public PiInterpreter(IEnumerable<Node> nodes)
        {
            this.Nodes = nodes.ToArray();
        }

        public void Run()
        {
            foreach (var node in Nodes)
            {
                Execute(node);
            }
        }

        private void Execute(Node node)
        {
            if (node is VariableDeclaration varDec)
            {
                var value = EvaluateExpression(varDec.Value);

                Context.SetLocal(varDec.Name, value);
            }
        }

        private object EvaluateExpression(Expression expression)
        {
            if (expression is ConstantExpression constant)
            {
                switch (constant.Kind)
                {
                    case ConstantKind.String:
                        return constant.Value;
                    case ConstantKind.Integer:
                        return int.Parse(constant.Value);
                    case ConstantKind.Decimal:
                        return float.Parse(constant.Value);
                    case ConstantKind.Boolean:
                        return constant.Value == "true" ? true : constant.Value == "false" ? false : false;
                }
            }
            else if (expression is BinaryExpression binary)
            {
                var left = EvaluateExpression(binary.Left);
                var right = EvaluateExpression(binary.Right);

                switch (binary.Operator)
                {
                    case BinaryOperators.Assign:
                        break;
                    case BinaryOperators.Add:
                        break;
                    case BinaryOperators.Subtract:
                    case BinaryOperators.Multiply:
                    case BinaryOperators.Divide:
                        if (!left.IsNumber() || !right.IsNumber())
                        {

                        }
                        break;
                }
            }

            return null;
        }
    }
}
