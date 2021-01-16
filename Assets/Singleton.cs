using UnityEngine;

/// <summary>
/// My way of finding singleton instances of important classes. 
/// Classes that are meant to have only one instance in the scene should inherit this class.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour
{
	/// <summary>
	/// With this method the single instance can be found easily without needing to type repeated code to each singleton class
	/// </summary>
	/// <returns>Instance of the class</returns>
	public static T GetInstance() {
		return GameObject.Find(typeof(T).Name).GetComponent<T>();
	}
}
