namespace CharmGoldTooth.Domain
{
	/// <summary>
	/// The number that names a valuable between machines: the PhotonView id in multiplayer, the Unity instance id alone
	/// (where every view id is 0). Same rule as Pockets uses.
	/// </summary>
	internal static class ValuableKey
	{
		internal static int Of(bool isMultiplayer, int viewId, int instanceId)
		{
			return isMultiplayer ? viewId : instanceId;
		}

		/// <summary>A key a request can carry: in multiplayer the view id must be a real one (not 0).</summary>
		internal static bool IsUsable(bool isMultiplayer, int key)
		{
			return isMultiplayer ? key > 0 : key != 0;
		}
	}
}
