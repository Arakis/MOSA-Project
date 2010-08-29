/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (grover) <sharpos@michaelruck.de>
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Mosa.Runtime.Metadata.Signatures
{
	/// <summary>
	/// 
	/// </summary>
	public class TypeSpecSignature : Signature
	{
		/// <summary>
		/// 
		/// </summary>
		private SigType _type;

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		public SigType Type
		{
			get { return _type; }
		}

        /// <summary>
        /// Parses the signature.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reader"></param>
		protected override void ParseSignature(ISignatureContext context, SignatureReader reader)
		{
			_type = SigType.ParseTypeSignature(context, reader);
		}
	}
}
