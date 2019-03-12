namespace ReSharper.Exceptional.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using JetBrains.ReSharper.Psi.CSharp.Tree;

    /// <summary>
    /// Generates a description to use as the documentation of ArgumentNullException.
    /// </summary>
    internal class ArgumentNullExceptionDescription
    {
        /// <summary>
        /// Arguments of the thrown exception.
        /// </summary>
        private readonly ICollection<ICSharpArgument> arguments;

        /// <summary>
        /// Tree representation of the thrown statement.
        /// </summary>
        private readonly IThrowStatement statement;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullExceptionDescription"/> class.
        /// </summary>
        /// <param name="statement">The thrown statement.</param>
        public ArgumentNullExceptionDescription(IThrowStatement statement)
        {
            this.statement = statement;
            arguments = GetArguments();
        }

        /// <summary>
        /// Creates a new <see cref="ArgumentNullExceptionDescription"/> from a <see cref="ThrownExceptionModel"/>.
        /// </summary>
        /// <param name="exception">The exception model.</param>
        /// <returns>
        /// A new <see cref="ArgumentNullExceptionDescription"/> instance or <see langword="null"/> when the exception type is not an <see cref="System.ArgumentNullException"/>
        /// </returns>
        public static ArgumentNullExceptionDescription CreateFrom(ThrownExceptionModel exception)
        {
            if ("System.ArgumentNullException".Equals(exception.ExceptionType.GetClrName().FullName))
            {
                return new ArgumentNullExceptionDescription(exception.ExceptionsOrigin.Node as IThrowStatement);
            }

            return null;
        }

        /// <summary>
        /// Generates a description based on the <see cref="System.ArgumentNullException"/>.
        /// </summary>
        /// <returns>
        /// A string description.
        /// </returns>
        public string GetDescription()
        {
            if (!IsOfType(@"ArgumentNullException"))
            {
                return string.Empty;
            }

            switch (arguments.Count)
            {
                case 0:
                    return @"Thrown when the arguments are <see langword=""null""/>";
                case 1:
                    return $@"<paramref name=""{GetArgumentText("paramName")}""/> is <see langword=""null""/>";
                case 2:
                    if (HasParamAndMessageArguments())
                    {
                        return $@"{GetArgumentText("message")} <paramref name=""{GetArgumentText("paramName")}""/>";
                    }
                    return GetArgumentText("message");
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets an argument by parameter name.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>
        /// The argument with that parameter name or <see langword="null"/>.
        /// </returns>
        private ICSharpArgument GetArgumentByName(string paramName)
        {
            return arguments.FirstOrDefault(a => paramName.Equals(GetParameterName(a)));
        }

        /// <summary>
        /// Gets the arguments of the exception.
        /// </summary>
        /// <returns>
        /// A collection of arguments.
        /// </returns>
        private ICollection<ICSharpArgument> GetArguments()
        {
            var expression = statement.Exception as IObjectCreationExpression;
            if (expression != null)
            {
                return expression.ArgumentList.Arguments;
            }

            return new Collection<ICSharpArgument>();
        }

        /// <summary>
        /// Gets the argument text.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>
        /// The text of the argument with that parameter name.
        /// </returns>
        private string GetArgumentText(string paramName)
        {
            ICSharpArgument argument = GetArgumentByName(paramName);

            if (argument != null)
            {
                return ExtractArgumentValue(argument);
            }

            return string.Empty;
        }

        /// <summary>
        /// Extracts the argument value from literal or nameof expressions.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// The argument value as a string.
        /// </returns>
        private string ExtractArgumentValue(ICSharpArgument argument)
        {
            var literal = argument.Value as ICSharpLiteralExpression;
            if (literal != null)
            {
                return literal.ConstantValue.Value?.ToString() ?? string.Empty;
            }

            var expression = argument.Value as IInvocationExpression;
            if ((expression != null) && @"nameof".Equals(expression.InvocationExpressionReference.GetName()))
            {
                return expression.ConstantValue.Value?.ToString() ?? string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the name of the parameter of the given argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// The name of the parameter.
        /// </returns>
        private string GetParameterName(ICSharpArgument argument)
        {
            return argument.MatchingParameter?.Element.ShortName ?? string.Empty;
        }

        /// <summary>
        /// Determines whether has parameter and message arguments.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if exception has parameter and message arguments; otherwise, <c>false</c>.
        /// </returns>
        private bool HasParamAndMessageArguments()
        {
            return GetArgumentByName(@"paramName") != null && GetArgumentByName(@"message") != null;
        }

        /// <summary>
        /// Determines whether the exception is of type specified by the CLR name.
        /// </summary>
        /// <param name="clrName">CLR name.</param>
        /// <returns>
        ///   <c>true</c> if exception is of that type; otherwise, <c>false</c>.
        /// </returns>
        private bool IsOfType(string clrName)
        {
            var expression = statement.Exception as IObjectCreationExpression;
            return expression.TypeName.QualifiedName.Equals(clrName);
        }
    }
}