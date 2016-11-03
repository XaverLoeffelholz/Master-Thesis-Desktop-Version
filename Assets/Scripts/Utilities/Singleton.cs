using UnityEngine;
using System.Collections;

public class Singleton<T> : MyMonoBehaviour where T : MyMonoBehaviour {
	protected static T instance;
	
	/**
      Returns the instance of this singleton.
   */
	public static T Instance
	{
		get
		{
			if(instance == null)
			{
				instance = (T) FindObjectOfType(typeof(T));
				
				if (instance == null)
				{
					Debug.LogError("An instance of " + typeof(T) + 
					               " is needed in the scene, but there is none.");
				}
			}
			
			return instance;
		}
	}
}
