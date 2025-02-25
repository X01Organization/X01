using System.Linq.Expressions;

namespace X01.Core.Extensions;

public static class ExpressionExtensions
{
    public static Expression<Func<TParameter, TResult>> Return<TParameter, TResult>(
        this Expression<Func<TParameter, TResult>> expression)
    {
        return expression;
    }

    public static Expression<Func<TParameter1, TParameter2, TResult>> Return<TParameter1, TParameter2, TResult>(
        this Expression<Func<TParameter1, TParameter2, TResult>> expression)
    {
        return expression;
    }

    public static Expression<Func<TParameter1, TParameter2, TParameter3, TResult>> Return<TParameter1, TParameter2,
        TParameter3, TResult>(
        this Expression<Func<TParameter1, TParameter2, TParameter3, TResult>> expression)
    {
        return expression;
    }

    public static Expression<Func<TParameter1, TParameter2, TParameter3, TParameter4, TResult>> Return<TParameter1,
        TParameter2,
        TParameter3, TParameter4, TResult>(
        this Expression<Func<TParameter1, TParameter2, TParameter3, TParameter4, TResult>> expression)
    {
        return expression;
    }
}