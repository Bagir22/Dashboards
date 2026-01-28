namespace Infrastructure.ETLPipeline.Synchronize.Utils
{
    internal static class DateUtils
    {
        public static List<DateTime> GetMonthlyDatesFrom2023()
        {
            var dates = new List<DateTime>();
            DateTime current = new DateTime( 2023, 1, 1 );
            DateTime end = new DateTime( DateTime.Now.Year, DateTime.Now.Month, 1 );

            while ( current <= end )
            {
                dates.Add( current );
                current = current.AddMonths( 1 );
            }

            return dates;
        }

        public static DateTime GetCurrentDate => new DateTime( DateTime.Now.Year, DateTime.Now.Month, 1 );
    }
}
