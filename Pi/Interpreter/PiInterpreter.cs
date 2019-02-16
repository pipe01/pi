using Pi.Parser.Syntax;
using Pi.Parser.Syntax.Declarations;
using Pi.Parser.Syntax.Expressions;
using Pi.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Pi.Interpreter
{
    internal class PiInterpreter
    {
        private readonly Node[] Nodes;

        private readonly Context Context = new Context();

        private int Index;
        private Node Current => Nodes[Index];
        private Node Next => Index < Nodes.Length - 1 ? Nodes[Index + 1] : null;

        public PiInterpreter(IEnumerable<Node> nodes)
        {
            this.Nodes = nodes.ToArray();
        }

        public void Run()
        {
            while (Advance())
                Execute();
        }

        private bool Advance()
        {
            Index++;

            if (Index >= Nodes.Length)
                return false;

            return true;
        }

        private void Execute()
        {
            if (Current is VariableDeclaration varDec)
            {
                Context[varDec.Name] = EvaluateExpression(varDec.Value);
            }
            else if (Current is Expression expr)
            {
                EvaluateExpression(expr);
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
                        return constant.Value == "true";
                }
            }
            else if (expression is BinaryExpression binary)
            {
                var left = binary.Left is ReferenceExpression ? binary.Left : EvaluateExpression(binary.Left);
                var right = binary.Right is ReferenceExpression ? binary.Right : EvaluateExpression(binary.Right);
                
                if (binary.Operator == BinaryOperators.Assign)
                {
                    if (!(left is ReferenceExpression reference))
                        throw new InterpreterException("Left side of an assignment must be a reference", expression.Location);

                    DoAssignment(reference, right);
                }
                else if (binary.Operator == BinaryOperators.Add && (left is string || right is string))
                {
                    return (left as string) + (right as string);
                }

                if (!left.IsNumber())
                    throw new InterpreterException("Left side of the operation must be a number", expression.Location);
                if (!right.IsNumber())
                    throw new InterpreterException("Right side of the operation must be a number", expression.Location);

                float leftN = (float)left;
                float rightN = (float)right;
                float ret = 0;

                switch (binary.Operator)
                {
                    case BinaryOperators.Add:
                        ret = leftN + rightN;
                        break;
                    case BinaryOperators.Subtract:
                        ret = leftN - rightN;
                        break;
                    case BinaryOperators.Multiply:
                        ret = leftN * rightN;
                        break;
                    case BinaryOperators.Divide:
                        ret = leftN / rightN;
                        break;
                }

                if (left is int && right is int)
                    return (int)ret;

                return ret;
            }
            else if (expression is ReferenceExpression reference)
            {
                return GetReferenceValue(reference);
            }

            return null;
        }

        private void DoAssignment(ReferenceExpression reference, object value)
        {
            GetReferenceValue(reference);
        }

        private object GetReferenceValue(ReferenceExpression reference)
        {
            IKeyed<object> container = Context;

            foreach (var item in reference.References)
            {
                var identifier = (IdentifierExpression)item;
                
                if (!container.Get(identifier.Name, out var val))
                    throw new InterpreterException($"Field \"{identifier.Name}\" not found on object", reference.Location);

                if (!(val is IKeyed<object> keyed))
                    throw new InterpreterException("Tried to access field on primitive", reference.Location);

                container = keyed;
            }

            return null;
        }
    }
}
