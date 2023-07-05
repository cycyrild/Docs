using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocsWASM.Shared.Annotations;

namespace DocsWASM.Shared.Annotations
{
	public class Annotation
	{
		public uint Id { get; set; }
		public uint PageId { get; set; }
		public Point Point { get; set; }
		public string Text { get; set; }
		public uint UserId { get; set; }
		public string UserName { get; set; }
		public DateTime ModifiedDate { get; set; }
	}

	public class AnnotationEqualityComparer : EqualityComparer<Annotation>
	{
		public override bool Equals(Annotation x, Annotation y)
		{
			if (x == null && y == null)
				return true;

			if (x == null || y == null)
				return false;

			return x.Text == y.Text
				   && x.Point.X == y.Point.X
				   && x.Point.Y == y.Point.Y
				   && x.PageId == y.PageId
				   && x.Id == y.Id
				   && x.UserId == y.UserId
				   && x.UserName == y.UserName
				   && x.ModifiedDate == y.ModifiedDate;
		}

		public override int GetHashCode(Annotation obj)
		{
			return obj.Text.GetHashCode() ^
				   obj.Point.X.GetHashCode() ^
				   obj.Point.Y.GetHashCode() ^
				   obj.PageId.GetHashCode() ^
				   obj.Id.GetHashCode() ^
				   obj.UserId.GetHashCode() ^
				   obj.UserName.GetHashCode() ^
				   obj.ModifiedDate.GetHashCode();
		}
	}
}
