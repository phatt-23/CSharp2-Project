namespace CoworkingApp.Models.Misc
{
    public static class StringExtensions
    {
        public static int? TryParseToInt(this string Source)
        {
            int result;
            if (int.TryParse(Source, out result))
                return result;
            else

                return null;
        }
    }
}
