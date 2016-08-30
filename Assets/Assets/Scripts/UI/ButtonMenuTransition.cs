using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonMenuTransition : ControlableBehaviour {

	[SerializeField]
	private GameObject[] newMenus = new GameObject[0];
	[SerializeField]
	private GameObject[] oldMenus = new GameObject[0];
	[SerializeField]
	private bool disableParent = true;
	[SerializeField]
	private KeyCode keyCode = KeyCode.None;
	[SerializeField]
	private string keyName = "";

    public override void RefreshBinds()
    {
        keyCode = Controls.controls[keyName];
    }

    void Start () {
		Button button = GetComponent<Button> ();
		if (button)
			button.onClick.AddListener (Transition);
        if (!string.IsNullOrEmpty(keyName))
        {
            keyCode = Controls.controls[keyName];
            Controls.AddListener(this);
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (keyCode))
			Transition ();
	}

	void Transition()
	{
		for (int i = 0; i < newMenus.Length; i++) {
			newMenus [i].SetActive (true);
		}

		for (int i = 0; i < oldMenus.Length; i++) {
			oldMenus [i].SetActive (false);
		}

		if (disableParent)
			transform.parent.gameObject.SetActive (false);
	}
}
