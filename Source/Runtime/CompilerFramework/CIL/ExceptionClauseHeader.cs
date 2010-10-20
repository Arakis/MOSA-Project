﻿/*
 * (c) 2010 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Simon Wollwage (rootnode) <rootnode@mosa-project.org>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mosa.Runtime.CompilerFramework.CIL
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class ExceptionClauseHeader
	{
		/// <summary>
		/// 
		/// </summary>
		private List<EhClause> clauses = new List<EhClause>();

		/// <summary>
		/// 
		/// </summary>
		public List<EhClause> Clauses
		{
			get { return this.clauses; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clause"></param>
		public void AddClause(EhClause clause)
		{
			this.clauses.Add(clause);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Sort()
		{

		}
	}
}