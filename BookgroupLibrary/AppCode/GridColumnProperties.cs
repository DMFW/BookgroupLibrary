namespace BookgroupLibraryCode
{
	public class GridColumnProperties
	{

		public GridColumnProperties(int columnIndex,string name)
		{
			ColumnIndex = columnIndex;
			Name = name;
			Hidden = false;
			RightAlign = false;
			HyperLink = false;
		}

		public int ColumnIndex { get; set; }
		public string Name { get; set; }
		public bool Hidden { get; set; }
		public bool RightAlign { get; set; }
		public bool HyperLink { get; set; }
	}
}