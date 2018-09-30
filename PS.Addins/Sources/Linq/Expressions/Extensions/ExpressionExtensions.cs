using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PS.Addins.Extensions;

namespace PS.Addins.Linq.Expressions.Extensions
{
    public static class ExpressionExtensions
    {
        #region Static members

        public static Expression PackExpressions(this IEnumerable<Expression> codeBlocks, IEnumerable<ParameterExpression> blockParameters = null)
        {
            var blocks = codeBlocks.ToList();
            var parameters = blockParameters.Enumerate().ToList();
            Expression resultExpression;
            if (blocks.Count == 0) resultExpression = Expression.Empty();
            else if (blocks.Count == 1) resultExpression = blocks.First();
            else resultExpression = Expression.Block(parameters, blocks);
            return resultExpression;
        }

        #endregion
    }
}