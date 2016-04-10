#region

using System.ComponentModel;

#endregion

namespace Hearthstone_Deck_Tracker.Enums
{
	public enum TimeFrame
	{
		[Description("ENUM_Today")]
		Today,
<<<<<<< HEAD
		[Description("ENUM_Yesterday")]
		Yesterday,
		[Description("ENUM_Last24Hours")]
		Last24Hours,
		[Description("ENUM_ThisWeek")]
		ThisWeek,
		[Description("ENUM_PreviousWeek")]
		PreviousWeek,
		[Description("ENUM_Last7Days")]
		Last7Days,
		[Description("ENUM_ThisMonth")]
		ThisMonth,
		[Description("ENUM_PreviousMonth")]
		PreviousMonth,
		[Description("ENUM_ThisYear")]
		ThisYear,
		[Description("ENUM_PreviousYear")]
		PreviousYear,
		[Description("ENUM_AllTime")]
=======

		[Description("Yesterday")]
		Yesterday,

		[Description("Last 24 Hours")]
		Last24Hours,

		[Description("This Week")]
		ThisWeek,

		[Description("Previous Week")]
		PreviousWeek,

		[Description("Last 7 Days")]
		Last7Days,

		[Description("This Month")]
		ThisMonth,

		[Description("Previous Month")]
		PreviousMonth,

		[Description("This Year")]
		ThisYear,

		[Description("Previous Year")]
		PreviousYear,

		[Description("All Time")]
>>>>>>> refs/remotes/Epix37/master
		AllTime
	}

	public enum DisplayedTimeFrame
	{
		[Description("Today")]
		Today,

		[Description("Week")]
		ThisWeek,

		[Description("Season")]
		CurrentSeason,

		[Description("All Time")]
		AllTime,

		[Description("Custom")]
		Custom
	}
}