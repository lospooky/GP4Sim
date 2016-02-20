using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GP4Sim.SymbolicTrees
{
    public static class ExpressionExtensions
    {
        public static Expression Average(this Expression[] expArr)
        {
            int size = expArr.Length;
            if (size == 1)
                return expArr[0];
            else if (size > 1)
            {
                Expression sum = expArr[0];
                for (int i = 1; i < size; i++)
                    sum = Expression.Add(sum, expArr[i]);

                return Expression.Divide(sum, Expression.Constant(size));
            }
            else throw new ArgumentNullException();
        }
    }
}
