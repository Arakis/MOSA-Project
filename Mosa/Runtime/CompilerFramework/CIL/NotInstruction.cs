﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mosa.Runtime.CompilerFramework.CIL
{
	/// <summary>
	/// 
	/// </summary>
	public class NotInstruction : UnaryArithmeticInstruction
	{
		#region Operand Table

		/// <summary>
		/// Operand table according to ISO/IEC 23271:2006 (E), Partition III, 1.5, Table 5.
		/// </summary>
		private static readonly StackTypeCode[] _opTable = new StackTypeCode[] {
            StackTypeCode.Unknown,
            StackTypeCode.Int32,   
            StackTypeCode.Int64, 
            StackTypeCode.N,
            StackTypeCode.Unknown, 
            StackTypeCode.Unknown, 
            StackTypeCode.Unknown
        };

		#endregion // Operand Table
		
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="NotInstruction"/> class.
		/// </summary>
		/// <param name="opcode">The opcode.</param>
		public NotInstruction(OpCode opcode)
			: base(opcode)
		{
		}

		#endregion // Construction

		#region Methods

		/// <summary>
		/// Validates the instruction operands and creates a matching variable for the result.
		/// </summary>
		/// <param name="instruction">The instruction.</param>
		/// <param name="compiler">The compiler.</param>
		public override void Validate(ref InstructionData instruction, IMethodCompiler compiler)
		{
			base.Validate(ref instruction, compiler);

			// Validate the operand
			StackTypeCode result = _opTable[(int)instruction.Operand1.StackType];
			if (StackTypeCode.Unknown == result)
				throw new InvalidOperationException(@"Invalid operand to Not instruction.");

			instruction.Result = compiler.CreateTemporary(instruction.Operand1.Type);
		}

		/// <summary>
		/// Allows visitor based dispatch for this instruction object.
		/// </summary>
		/// <param name="vistor">The vistor.</param>
		/// <param name="context">The context.</param>
		public override void Visit(CILVisitor vistor, Context context)
		{
			vistor.Not(context);
		}

		#endregion Methods

	}
}
