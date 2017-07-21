using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
	protected static T __instance = null;
	public static T _instance {
		get {
			if( __instance == null ) {
				__instance = FindObjectOfType( typeof(T) ) as T;
				if( __instance == null ) {
					GameObject go = new GameObject();
					Transform form = go.transform;
					form.name = typeof(T).Name;

					form.SetParent( null );

					form.localPosition = Vector3.zero;
					form.localRotation = Quaternion.identity;
					form.localScale = Vector3.one;

					__instance = go.AddComponent<T>();
				}
			}
			return __instance;
		}
	}

	public static void Destroy() {
		if( __instance != null ) {
			GameObject.DestroyObject( __instance.gameObject );
			__instance = null;
		}
	}
}

public class SingletonEx<T> : MonoBehaviour where T : MonoBehaviour {
	protected static T __instance = null;
	public static T _instance {
		get {
			if( __instance == null ) {
				__instance = FindObjectOfType( typeof(T) ) as T;
				if( __instance == null ) {
					GameObject go = new GameObject();
					Transform form = go.transform;
					form.name = typeof(T).Name;

					form.SetParent( null );

					form.localPosition = Vector3.zero;
					form.localRotation = Quaternion.identity;
					form.localScale = Vector3.one;

					DontDestroyOnLoad( go );
					__instance = go.AddComponent<T>();
				}
			}
			return __instance;
		}
	}

	public static void Destroy() {
		if( __instance != null ) {
			GameObject.DestroyObject( __instance.gameObject );
			__instance = null;
		}
	}
}

//<Binary_Pack>
public partial class TableData {
	//<Binary_Pack_Start>
	public int _ID;
	//<Binary_Pack_End>

	public TableData() {
		_ID = 0;
	}

	/*public virtual void UnpackData( jSerializerReader sr ) {
		_ID = sr.ReadInt32();
	}

	public virtual void PackData( jSerializerWriter sw ) {
		sw.Write( _ID );
	}

	public virtual void CopyFrom( TableData src ) {
		_ID = src._ID;
	}*/
}

//<Binary_Pack>
public partial class TableList<T> where T : TableData, new() {
	//<Binary_Pack_Start>
	public List<T> _DataList;
	//<Binary_Pack_End>

	public void Add( T data ) {
		for( int i=0; i<_DataList.Count; i++ )  {
			if( data._ID < _DataList[i]._ID ) {
				_DataList.Insert( i, data );
				return;
			}
		}

		_DataList.Add( data );
	}

	public int IndexOf( int id ) {
		int low = 0, high = _DataList.Count - 1, mid;

		while( low <= high ) {
			mid = (low + high) / 2;
			if( _DataList[mid]._ID > id ) {
				high = mid - 1;
			} else if( _DataList[mid]._ID < id ) {
				low = mid + 1;
			} else {
				return mid;
			}
		}

		return -1;
	}

	public T Find( int id ) {
		int find;
		if( (find = IndexOf( id )) >= 0 ) {
			return _DataList[find];
		}
		return null;
	}

	public void Sort() {
		T tmp;
		for( int i=0; i<(_DataList.Count - 1); i++ ) {
			for( int j=(i + 1); j<_DataList.Count; j++ ) {
				if( _DataList[i]._ID > _DataList[j]._ID ) {
					tmp = _DataList[i] ;
					_DataList[i] = _DataList[j];
					_DataList[j] = tmp as T;
				}
			}
		}
	}
}

//<Binary_Pack>
public partial class StringData : TableData {
	//<Binary_Pack_Start>
	public string _message;
	//<Binary_Pack_End>

	public StringData() {
		_message = "";
	}
}

public class StringTable : TableList<StringData> {
	public StringTable() {
		_DataList = new List<StringData>();
	}
}

//<Binary_Pack>
public partial class AnimationCurveData : TableData {
	//<Binary_Pack_Start>
	public string _desc;
	public AnimationCurve _curve;
	//<Binary_Pack_End>

	public AnimationCurveData() {
		_ID = 0;

		_desc = "";

		_curve = new AnimationCurve( new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f) );
	}
}

public class AnimationCurveTable : TableList<AnimationCurveData> {
	public AnimationCurveTable() {
		_DataList = new List<AnimationCurveData>();
	}
}
