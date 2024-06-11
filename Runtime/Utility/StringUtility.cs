namespace Packages.Clayze.Editor
{
	public static class StringUtility
	{
		public static string ConvertSlashToUnicodeSlash(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			return text.Replace('/', '\u2215');
		}

		public static string ConvertUnicodeSlashToSlash(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			return text.Replace('\u2215', '/');
		}
	}
}