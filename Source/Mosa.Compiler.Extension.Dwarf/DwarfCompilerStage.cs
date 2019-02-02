﻿using System;
using Mosa.Compiler.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Mosa.Compiler.Framework.Linker.Elf;
using Mosa.Compiler.Framework.Common;
using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Linker;
using System.Linq;

namespace Mosa.Compiler.Extensions.Dwarf
{

	internal class DwarfCompilerStage : BaseCompilerStage
	{

		protected override void Setup()
		{
		}

		protected override void RunPreCompile()
		{
		}

		protected override void RunPostCompile()
		{
			if (Linker.CreateExtraSections == null)
				Linker.CreateExtraSections = CreateExtraSections;
			else
				Linker.CreateExtraSections = () =>
				{
					var sections = Linker.CreateExtraSections();
					sections.AddRange(CreateExtraSections());
					return sections;
				};
		}

		public static byte DW_LNS_extended_opcode = 0;
		public static byte DW_LNS_set_file = 4;
		public static byte DW_LNS_set_column = 5;
		public static byte DW_LNS_advance_line = 3;
		public static byte DW_LNS_advance_pc = 2;
		public static byte DW_LNS_copy = 1;
		public static byte DW_LNS_negate_stmt = 6;
		public static byte DW_LNS_set_basic_block = 7;

		public static byte DW_LNE_end_sequence = 1;
		public static byte DW_LNE_set_address = 2;
		public static byte DW_LNE_define_file = 3;

		private List<DwarfAbbrev> AbbrevList = new List<DwarfAbbrev>();

		private void EmitDebugInfo(EndianAwareBinaryWriter wr)
		{
			// note: Compilation Unit Header != Debugging Information Entry

			// Compilation Unit Header
			var compilationUnitSizePosition = wr.Position;
			wr.Write((uint)7); // length
			wr.Write((ushort)0x02); // version
			wr.Write((uint)0); // abbr tag offset.
			wr.WriteByte(4); //addr size of platform

			var context = new DwarfWriteContext { Writer = wr, AbbrevList = AbbrevList };

			// Debugging Information Entry
			var cu = new DwarfCompilationUnit
			{
				Producer = "Mosa Compiler",
				ProgramCounterLow = 0x00500000,
				ProgramCounterHigh = 0x00600000,
			};
			cu.Emit(context);

			uint compilationUnitSize = (uint)(wr.Position - compilationUnitSizePosition - sizeof(uint));
			wr.Position = compilationUnitSizePosition;
			wr.Write(compilationUnitSize);
			wr.Position = wr.BaseStream.Length;
		}

		public void EmitDebugInfoCompilationUnitHeader(EndianAwareBinaryWriter wr)
		{

		}

		private void EmitDebugAbbrev(EndianAwareBinaryWriter wr)
		{
			foreach (var abbr in AbbrevList)
				EmitDebugAbbrev(wr, abbr);

			wr.WriteULEB128(DwarfConstants.NullTag);
		}

		private void EmitDebugAbbrev(EndianAwareBinaryWriter wr, DwarfAbbrev abbr)
		{
			wr.WriteULEB128(abbr.Number);
			wr.WriteULEB128((uint)abbr.Tag);
			wr.WriteByte(abbr.HasChildren ? DwarfConstants.DW_CHILDREN_yes : DwarfConstants.DW_CHILDREN_no);
			foreach (var attr in abbr.Attributes)
			{
				wr.WriteULEB128((uint)attr.Attribute);
				wr.WriteULEB128((uint)attr.Form);
			}
			wr.WriteULEB128(DwarfConstants.NullAttributeName);
			wr.WriteULEB128(DwarfConstants.NullAttributeValue);

			if (abbr.HasChildren)
				foreach (var child in abbr.Children)
					EmitDebugAbbrev(wr, child);
		}

		private void EmitDebugLine(EndianAwareBinaryWriter wr)
		{
			foreach (var type in TypeSystem.AllTypes)
			{
				if (type.IsModule)
					continue;

				foreach (var method in type.Methods)
				{
					if (method.Code == null)
						continue;

					var instructions = method.Code.OrderBy(i => i.Document).ThenBy(i => i.Offset);
					var firstInstruction = instructions.FirstOrDefault();
					if (firstInstruction == null)
						continue;

					uint pc = 0;
					uint line = 1;
					uint col = 0;
					uint file = 1;

					var compilationUnitSizePosition = wr.Position;
					wr.Write((uint)0); // Placeholder for Compilation unit Size

					wr.WriteByte(0x02); // DWARF Version
					wr.WriteByte(0x00); // version (2 bytes)

					var headerSizePosition = wr.Position;
					wr.Write((uint)0); // Placeholder for header size

					wr.WriteByte(0x01); // Minimum Instruction length
					wr.WriteByte(0x01); // Default is_stmt value
					wr.WriteByte(0xFB); // Value doesn't matter, because we are not using special op codes
					wr.WriteByte(0x0E); // Value doesn't matter, because we are not using special op codes
					wr.WriteByte(9 + 1); // first special op code

					// wr.Write(0x00010101); // the number of arguments for the 12 standard opcodes
					// wr.Write(0x01000000);
					// wr.Write(0x01000001);

					// the number of arguments for the 9 standard opcodes
					wr.WriteByte(0x00);
					wr.WriteByte(0x01);
					wr.WriteByte(0x01);
					wr.WriteByte(0x01);

					wr.WriteByte(0x01);
					wr.WriteByte(0x00);
					wr.WriteByte(0x00);
					wr.WriteByte(0x00);

					wr.WriteByte(0x01);
					//---

					wr.WriteNullTerminatedString("dir1");
					wr.WriteByte(DwarfConstants.EndOfDirectories); // End of directories

					wr.WriteNullTerminatedString("file1.cs");
					wr.WriteULEB128(0); // Dir index 0
					wr.WriteULEB128(DwarfConstants.NullFileTime);
					wr.WriteULEB128(DwarfConstants.NullFileLength);

					wr.WriteByte(DwarfConstants.EndOfFiles); // End of files

					// Write header size
					uint headerSize = (uint)(wr.Position - headerSizePosition - sizeof(uint));
					wr.Position = headerSizePosition;
					wr.Write(headerSize);
					wr.Position = wr.BaseStream.Length;

					wr.WriteByte(0); // signals an extended opcode
					wr.WriteULEB128(0x05); // number of bytes after this used by the extended opcode (unsigned LEB128 encoded)
					wr.Write(DW_LNE_set_address);
					wr.Write(firstInstruction.Offset);

					foreach (var instruction in instructions)
					{
						continue;
						if (instruction.Document == null)
							continue;

						if (instruction.StartLine == 0xFEEFEE)
							continue;

						uint pcDiff = pc - (uint)instruction.Offset;
						int lineDiff = (int)line - instruction.StartLine;

						wr.Write(DW_LNS_set_file);
						wr.WriteULEB128(file);

						wr.Write(DW_LNS_advance_pc);
						wr.WriteSLEB128(pcDiff);

						wr.Write(DW_LNS_advance_line);
						wr.WriteSLEB128(lineDiff);

						wr.Write(DW_LNS_set_column);
						wr.WriteULEB128((ulong)instruction.StartColumn);

						wr.Write(DW_LNS_copy);

						pc += pcDiff;
						line += (uint)lineDiff;
					};

					uint compilationUnitSize = (uint)(wr.Position - compilationUnitSizePosition - sizeof(uint));
					wr.Position = compilationUnitSizePosition;
					wr.Write(compilationUnitSize);
					wr.Position = wr.BaseStream.Length;
				}

			}
		}

		List<Section> CreateExtraSections()
		{
			var sections = new List<Section>();

			sections.Add(new Section
			{
				Name = ".debug_info",
				Type = SectionType.ProgBits,
				AddressAlignment = 0x1000,
				EmitMethod = (section, writer) =>
				{
					var oldPos = writer.Position;
					EmitDebugInfo(writer);
					section.Size = (uint)writer.Position - (uint)oldPos;
				}
			});

			sections.Add(new Section
			{
				Name = ".debug_abbrev",
				Type = SectionType.ProgBits,
				AddressAlignment = 0x1000,
				EmitMethod = (section, writer) =>
				{
					var oldPos = writer.Position;
					EmitDebugAbbrev(writer);
					section.Size = (uint)writer.Position - (uint)oldPos;
				}
			});

			sections.Add(new Section
			{
				Name = ".debug_line",
				Type = SectionType.ProgBits,
				AddressAlignment = 0x1000,
				EmitMethod = (section, writer) =>
				{
					var oldPos = writer.Position;
					EmitDebugLine(writer);
					section.Size = (uint)writer.Position - (uint)oldPos;
				}
			});

			return sections;
		}

	}

}
