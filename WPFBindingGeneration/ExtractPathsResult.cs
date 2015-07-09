using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WPFBindingGeneration
{
	class ExtractPathsResult<T>
	{
		readonly Func<CreatePathParameter, T> createExpression;
		readonly Utility.SortedSet<Expression> paths;

		public ExtractPathsResult(Func<CreatePathParameter, T> createExpression, params Expression[] paths)
			: this(createExpression, new Utility.SortedSet<Expression>(paths))
		{
		}

		public ExtractPathsResult(Func<CreatePathParameter, T> createExpression, Utility.SortedSet<Expression> paths)
		{
			this.paths = paths;
			this.createExpression = createExpression;
		}

		public Utility.SortedSet<Expression> Paths
		{
			get { return paths; }
		}

		public Func<CreatePathParameter, T> CreateExpression
		{
			get { return createExpression; }
		}

		public static ExtractPathsResult<IEnumerable<T>> Flatten(IEnumerable<ExtractPathsResult<T>> input)
		{
			var seed = new ExtractPathsResult<IEnumerable<T>>(c => Enumerable.Empty<T>(), new Utility.SortedSet<Expression>());
			return input.Aggregate(seed, (results, result) => results.Combine(result, (items, item) => items.Concat(new[] {item})));
		}

		public ExtractPathsResult<R> Combine<U, R>(ExtractPathsResult<U> other, Func<T, U, R> merge)
		{
			var combinedPaths = new Utility.SortedSet<Expression>();
			foreach (var item in other.paths.Concat(paths))
				combinedPaths.Add(item);

			return new ExtractPathsResult<R>(c => merge(createExpression(c), other.createExpression(c)), combinedPaths);
		}

		public ExtractPathsResult<U> Select<U>(Func<T, U> func)
		{
			return new ExtractPathsResult<U>(c => func(createExpression(c)), paths);
		}

		internal delegate Expression CreatePathParameter(Expression path, Type type);
	}
}