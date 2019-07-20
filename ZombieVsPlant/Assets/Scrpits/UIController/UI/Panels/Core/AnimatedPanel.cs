using UnityEngine;
using System.Collections;

public class AnimatedPanel : PanelController
{
	[Header ("-----Sounds-----")]
	[SerializeField] AudioClip sfxWillAppear;
	[SerializeField] AudioClip sfxDidAppear;
	
	[SerializeField] AudioClip sfxWillHide;
	[SerializeField] AudioClip sfxDidHide;
	
	[SerializeField] float defaultFadeTime = 0.1f;
	
	System.Action onShowFinishCallback = null;
	System.Action onHideFinishCallback = null;

	public override void Show (System.Action onFinish = null)
	{
		
		if (this.gameObject.activeSelf) {
			return;
		}
		
		onShowFinishCallback = onFinish;
		
		this.gameObject.SetActive (true);
		
		//TODO: Block user input here	
		//TouchBlockerPanel.Block();
		
		if (sfxWillAppear) {
			//AudioManager.instance.PlayOneShotClip (sfxWillAppear);
		}
		
		try {
			PanelWillShow ();
		} catch (System.Exception e) {
			Debug.LogError ("PanelWillShow Failed " + e.Message + "\n " + e.StackTrace);
		}

		this.scale = Vector3.one;
						

		OnShowFinished ();
		
	}

	public override void Hide (System.Action onFinish = null)
	{
		
		if (this.scale == Vector3.zero) {
			return;
		}
		
		onHideFinishCallback = onFinish;
		
		if (sfxWillHide) {
			//AudioManager.instance.PlayOneShotClip (sfxWillHide);
		}
		
		try {
			PanelWillHide ();
		} catch {
			Debug.LogError ("PanelWillHide Failed " + this.name);
		}
		
		//TODO: Start blocking user input here
		//TouchBlockerPanel.Block();
		

		OnHideFinished ();
		
	}

	protected virtual void OnShowFinished (System.Action onFinish = null)
	{
		
		if (sfxDidAppear) {
			//AudioManager.instance.PlayOneShotClip (sfxDidAppear);
		}
		
		//TODO: Stop blocking user input here
		//TouchBlockerPanel.Unblock();

		
		try {
			PanelDidShow ();
		} catch {
			Debug.LogError ("PanelDidShow Failed " + this.name);
		}
		
		if (onShowFinishCallback != null) {
			onShowFinishCallback ();
			onShowFinishCallback = null;
		}
		
	}

	protected virtual void OnHideFinished (System.Action onFinish = null)
	{
				
		if (sfxDidHide) {
			//AudioManager.instance.PlayOneShotClip (sfxDidHide);
		}
		
		//TODO: Stop blocking user input here
		//TouchBlockerPanel.Unblock();

		this.scale = Vector3.zero;
		this.gameObject.SetActive (false);
		
		try {
			PanelDidHide ();
		} catch {
			Debug.LogError ("PanelDidHide Failed " + this.name);
		}
		
		if (onHideFinishCallback != null) {
			onHideFinishCallback ();
			onHideFinishCallback = null;
		}
		
	}
	
}
