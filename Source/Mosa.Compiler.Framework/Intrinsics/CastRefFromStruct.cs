// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework.IR;
using System;
using System.Collections.Generic;

namespace Mosa.Compiler.Framework.Intrinsics
{
	/// <summary>
	/// IntrinsicMethods
	/// </summary>
	static partial class IntrinsicMethods
	{
		[IntrinsicMethod("Mosa.Runtime.Intrinsic:CastRefFromStruct")]
		private static void CastRefFromStruct(Context context, MethodCompiler methodCompiler)
		{
			var result = context.Result;
			var operand1 = context.Operand1;

			var move = methodCompiler.Architecture.Is32BitPlatform ? (BaseInstruction)IRInstruction.Move32 : IRInstruction.Move64;

			context.SetInstruction(move, result, operand1);
		}
	}
}
