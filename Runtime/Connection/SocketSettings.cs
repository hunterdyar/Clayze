using System.Collections.Generic;

namespace Connection
{
	[System.Serializable]
	public class SocketSettings
	{
		public string connectionURL;
		public List<string> recentConnectionURLs = new List<string>();
		private int maxRecentURLs = 10;

		public void AddRecent(string connectionURL)
		{
			if (!recentConnectionURLs.Contains(connectionURL))
			{
				recentConnectionURLs.Insert(0, connectionURL);
				if (recentConnectionURLs.Count > maxRecentURLs)
				{
					recentConnectionURLs.RemoveAt(recentConnectionURLs.Count - 1);
				}
			}
			else
			{
				//move to top of list.
				recentConnectionURLs.Remove(connectionURL);
				recentConnectionURLs.Insert(0, connectionURL);
			}
		}
	}
}