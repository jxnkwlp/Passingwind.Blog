using System;

namespace Passingwind.Blog.Guids
{
	public interface IGuidGenerator
	{
		SequentialGuidType DefaultType { get; set; }

		Guid Create(SequentialGuidType? guidType = null);
	}
}
