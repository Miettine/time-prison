using UnityEngine;

/// <summary>
/// My way of finding singleton instances of important classes. 
/// Classes that are meant to have only one instance in the scene should inherit this class.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : Component {
	/// <summary>
	/// With this method the single instance can be found easily without needing to type repeated code to each singleton class.
	/// Throws error if there is no instance of the Singleton in the scene or if there is more than one.
	/// </summary>
	/// <returns>Instance of the class</returns>
	public static T GetInstance()
	{
		var objects = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
		if (objects.Length == 0) {
			throw new UnityException("Failed to find singleton");
		}
		if (objects.Length > 1) {
			throw new UnityException("Found more than one singleton");
		}
		return objects[0];
	}
	
	/// <summary>
	/// looks for the instance. If it is not found, an instance is created.
	/// </summary>
	/// <returns>The instance</returns>
	protected static T GetOrCreateInstance()
	{
		var objects = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
		if (objects.Length > 1) {
			throw new UnityException("Found more than one singleton");
		}

		if (objects.Length == 0) {
			var gameObject = new GameObject
			{
				name = typeof(T).Name
			};
			return gameObject.AddComponent<T>();
		}
		
		return objects[0];
	}
}