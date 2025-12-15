using UnityEngine;

namespace Midevil.Helpers
{
	public static class AudioHelper
	{
		public static void PlayClip(this AudioSource source, AudioClip clip)
		{
			source.Stop();
			source.clip = clip;
			source.Play();
		}
	}
}
