using Mosa.Compiler.MosaTypeSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Mosa.Compiler.Framework.Source
{
	public class SourceRegion
	{
		public int Address { get; set; }
		public int Length { get; set; }

		public int StartLine { get; set; }
		public int EndLine { get; set; }
		public int StartColumn { get; set; }
		public int EndColumn { get; set; }

		public string Source { get; set; }
	}
}


namespace Mosa.Compiler.Framework.Source
{
	public static class SourceRegions
	{

		public static List<SourceRegion> GetSourceRegions(MethodData data)
		{
			var method = data.Method;
			var regions = new List<SourceRegion>(data.LabelRegions.Count + 1);

			// Add prologue
			if (method.Code.Count > 0)
			{
				var firstInstruction = method.Code[0];

				regions.Add(new SourceRegion
				{
					Address = 0,
					Length = data.LabelRegions.Count > 0 ? data.LabelRegions[0].Start : 0,
					StartLine = firstInstruction.StartLine,
					EndLine = firstInstruction.EndLine,
					StartColumn = firstInstruction.StartColumn,
					EndColumn = firstInstruction.EndColumn,
					Source = firstInstruction.Document
				});
			}

			var startLine = 0;
			var endLine = 0;
			var startColumn = 0;
			var endColumn = 0;
			var document = "";

			foreach (var labelRegion in data.LabelRegions)
			{

				foreach (var instruction in method.Code)
				{

					// special case: the return label is always 0xFFFFF
					var searchForLabel = labelRegion.Label;
					if (labelRegion.Label == 0xFFFFF)
						searchForLabel = method.Code.Last().Offset;

					if (instruction.StartLine > 0)
					{
						startLine = instruction.StartLine;
						endLine = instruction.EndLine;
						startColumn = instruction.StartColumn;
						endColumn = instruction.EndColumn;
						document = instruction.Document;
					}

					if (instruction.Offset != searchForLabel)
						continue;

					var region = new SourceRegion()
					{
						Address = labelRegion.Start,
						Length = labelRegion.Length,
						StartLine = startLine,
						EndLine = endLine,
						StartColumn = startColumn,
						EndColumn = endColumn,
						Source = document
					};

					regions.Add(region);
				}
			}
			return regions;
		}

		public static List<SourceRegion> _GetSourceRegions(MosaMethod method, MethodData data)
		{
			var regions = new List<SourceRegion>(data.LabelRegions.Count);

			foreach (var labelRegion in data.LabelRegions)
			{
				foreach (var instruction in method.Code)
				{
					if (instruction.Offset != labelRegion.Label)
						continue;

					var region = new SourceRegion()
					{
						Address = labelRegion.Start,
						Length = labelRegion.Length,
						StartLine = instruction.StartLine,
						EndLine = instruction.EndLine,
						StartColumn = instruction.StartColumn,
						EndColumn = instruction.EndColumn,
						Source = instruction.Document
					};

					regions.Add(region);

					break;
				}
			}

			return regions;
		}
	}
}
