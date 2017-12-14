// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

namespace Mosa.Compiler.Framework.IR
{
	/// <summary>
	/// LogicalAnd
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.IR.BaseIRInstruction" />
	public sealed class LogicalAnd : BaseIRInstruction
	{
		public LogicalAnd()
			: base(2, 1)
		{
		}

		public override bool Commutative { get { return true; } }
	}
}

