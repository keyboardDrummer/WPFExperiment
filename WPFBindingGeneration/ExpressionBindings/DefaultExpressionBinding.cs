﻿using System.Windows.Data;

namespace WPFBindingGeneration.ExpressionBindings
{
	public abstract class DefaultExpressionBinding<From, To> : IExpressionBinding<From, To>
	{
		public abstract BindingBase ToBindingBase();
		public abstract To Evaluate(From @from);
		public abstract bool IsWritable { get; }
	}
}