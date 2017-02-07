using UnityEngine;
using UnityEngine.UI;

//-----------------------------------------------------------------------------
// Copyright 2015-2017 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo
{
	/// <summary>
	/// Update uGUI Text element with subtitle text as it plays from the MediaPlayer
	/// </summary>
	[AddComponentMenu("AVPro Video/Subtitles uGUI", 201)]
	public class SubtitlesUGUI : MonoBehaviour
	{
		[SerializeField]
		private MediaPlayer _mediaPlayer = null;

		[SerializeField]
		private Text _text = null;

		void Start()
		{
			ChangeMediaPlayer(_mediaPlayer);
		}

		void OnDestroy()
		{
			ChangeMediaPlayer(null);
		}

		public void ChangeMediaPlayer(MediaPlayer newPlayer)
		{
			// When changing the media player, handle event subscriptions
			if (_mediaPlayer != null)
			{
				_mediaPlayer.Events.RemoveListener(OnMediaPlayerEvent);
				_mediaPlayer = null;
			}

			_mediaPlayer = newPlayer;
			if (_mediaPlayer != null)
			{
				_mediaPlayer.Events.AddListener(OnMediaPlayerEvent);
			}
		}

		// Callback function to handle events
		private void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
		{
			switch (et)
			{
				case MediaPlayerEvent.EventType.SubtitleChange:
					{
						string text = _mediaPlayer.Subtitles.GetSubtitleText();

						// Change RichText for Unity uGUI Text
						text = text.Replace("<font color=", "<color=");
						text = text.Replace("</font>", "</color>");
						text = text.Replace("<u>", string.Empty);
						text = text.Replace("</u>", string.Empty);

						_text.text = text;
					}
					break;
			}
		}
	}
}