using UnityEngine;
// xml
using System.Xml;
using System.Xml.Serialization;
// io
using System.IO;
// dictionary
using System.Collections;
using System.Collections.Generic;
// Assembly
using System.Reflection;

#if UNITY_5_2
#else
using UnityEngine.SceneManagement;
#endif

public class jUtil {
	public readonly static string _ResPathName = "Assets/Resources/";
    public readonly static char [] _Split_Line = new char[] { '\r', '\n' };
    public readonly static char [] _Split_Tab = new char[] { '\t' };
    public readonly static char [] _Split_Comma = new char[] { ',' };

	static string _DirectoryReplace( string path ) {
		path = path.Replace( "\\", "/" );
		int find = path.LastIndexOf( _ResPathName );
        if( find > 0 ) {
		    path = path.Substring( find + _ResPathName.Length );
        }
		return path;
	}

    static void _DirectoryFiles( List<string> lst, DirectoryInfo dir, string ext ) {
		FileInfo [] fis = dir.GetFiles( ext );
		if( fis != null ) {
			for( int i=0; i<fis.Length; i++ ) {
				FileInfo fi = fis[i];
				if( fi.Extension.CompareTo( ".meta" ) == 0 ) {
					continue;
				}
				string str = _DirectoryReplace( fi.DirectoryName ) + "/" + GetFileTitle( fi.Name );
				lst.Add( str );
			}
			fis = null;
		}

		DirectoryInfo [] dis = dir.GetDirectories();
		if( dis != null ) {
			for( int i=0; i<dis.Length; i++ ) {
				DirectoryInfo di = dis[i];
				_DirectoryFiles( lst, di, ext );
			}
			dis = null;
		}
    }

    public static void GetFiles( List<string> lst, string path, string ext ) {
        DirectoryInfo dir = new DirectoryInfo( path );
        _DirectoryFiles( lst, dir, ext );
        dir = null;
    }
	
	public static string GetFileTitle( string path ) {
		// Assets/Resources/Enemy/pfGoblinGreen.prefab -> pfGoblinGreen
		return Path.GetFileNameWithoutExtension( path );
	}

	public static string GetFilePath( Object go ) {
		if( go == null ) {
			return "";
		}
		
#if UNITY_EDITOR_WIN
		string path = UnityEditor.AssetDatabase.GetAssetPath( go );
		// Assets/Resources/Enemy/pfGoblinGreen.prefab
		string file = GetFileTitle( path );
		// pfGoblinGreen
		path = Path.GetDirectoryName( path );
		// Assets/Resources/Enemy
		path += "/";
		// Assets/Resources/Enemy/
		path = path.Replace( "Assets/Resources/", "" );
		// Enemy/
		path += file;
		// Enemy/pfGoblinGreen
		return path;
#else
		return go.name;
#endif
	}

	public static string GetPathTitle( string path ) {
		int index = path.LastIndexOf( '/' );
		if( index < 0 ) {
			if( (index = path.LastIndexOf( '\\' )) < 0 ) {
				return path;
			}
		}

		return path.Substring( index + 1, path.Length - index - 1 );
	}

    public static System.Type GetType( string typeName ) {
        System.Type type = System.Type.GetType( typeName );
        if( type != null ) {
            return type;
        }

        if( typeName.Contains( "." ) == true ) {
            string assemName = typeName.Substring( 0, typeName.IndexOf( '.' ) );
            Assembly assembly = Assembly.Load( assemName );
            if( assembly == null ) {
                return null;
            }

            if( (type = assembly.GetType( typeName )) != null ) {
                return type;
            }
        }

        Assembly assem = Assembly.GetExecutingAssembly();
        if( assem != null ) {
            AssemblyName [] assemNames = assem.GetReferencedAssemblies();
            if( assemNames != null ) {
				for( int i=0; i<assemNames.Length; i++ ) {
					AssemblyName assemName = assemNames[i];
                    Assembly assembly = Assembly.Load( assemName );
                    if( assembly != null ) {
                        if( (type = assembly.GetType( typeName )) != null ) {
                            return type;
                        }
                    }
                }
            }
        }

        return null;
    }

	public static object Clone( object source, System.Type type ) {
		XmlWriterSettings settings = new XmlWriterSettings{ Encoding = System.Text.Encoding.UTF8, Indent = true };
		MemoryStream stream = new MemoryStream();
		using( XmlWriter xmlWriter = XmlWriter.Create( stream, settings ) ) {
			new XmlSerializer(type).Serialize( xmlWriter, source );
			stream.Seek( 0, SeekOrigin.Begin );
			return new XmlSerializer(type).Deserialize( stream );
		}
	}

    public static T Clone<T>( T source ) where T : class {
        return (T)Clone( source, typeof(T) );
	}

	/////////////////////////////////////////////////////////////////////////////////////////
	// ENCODING
	public static byte [] FromBase64( string s ) {
		return System.Convert.FromBase64String( s );
	}

	public static string ToBase64( byte [] inArray ) {
		return System.Convert.ToBase64String( inArray );
	}

	public static string Bytes2String( byte [] bytes ) {
		using( MemoryStream s = new MemoryStream( bytes ) ) {
			using( StreamReader r = new StreamReader( s ) ) {
				string str = r.ReadToEnd();
				r.Close();
				s.Close();
				return str;
			}
		}
	}

	public static byte [] String2Bytes( string s ) {
		return System.Text.Encoding.UTF8.GetBytes( s );
	}
	/////////////////////////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////////////////////////
	// VERSION
	public static int levelCount {
		get {
#if UNITY_5_2
			return Application.levelCount;
#else
			return SceneManager.sceneCountInBuildSettings;
#endif
		}
	}

	public static int loadedLevel {
		get {
#if UNITY_5_2
			return Application.loadedLevel;
#else
			return SceneManager.GetActiveScene().buildIndex;
#endif
		}
	}

	public static string loadedLevelName {
		get {
#if UNITY_5_2
			return Application.loadedLevelName;
#else
			return SceneManager.GetActiveScene().name;
#endif
		}
	}

	public static void LoadLevel( string loadLevelName ) {
		Resources.UnloadUnusedAssets();

#if UNITY_5_2
		Application.LoadLevel( loadLevelName );
#else
		SceneManager.LoadScene( loadLevelName );
#endif

		Resources.UnloadUnusedAssets();
	}

	public static AsyncOperation LoadLevelAdditiveAsync( string levelName ) {
#if UNITY_5_2
		return Application.LoadLevelAdditiveAsync( levelName );
#else
		return SceneManager.LoadSceneAsync( levelName );
#endif
	}

	public static AsyncOperation LoadLevelAsync( string levelName ) {
#if UNITY_5_2
		return Application.LoadLevelAsync( levelName );
#else
		return SceneManager.LoadSceneAsync( levelName );
#endif
	}
	/////////////////////////////////////////////////////////////////////////////////////////

	public static void LayerMask( Transform form, int layer ) {
		form.gameObject.layer = layer;

		for( int i=(form.childCount - 1); i>=0; i-- ) {
			LayerMask( form.GetChild( i ), layer );
		}
	}

	public static void LayerMask( Transform form, string layer_name ) {
		int layer = UnityEngine.LayerMask.NameToLayer( layer_name );

		LayerMask( form, layer );
	}

	public static void GetPointOfContact( Vector3 pos, Vector3 circle, float radius, out Vector3 posL, out Vector3 posR ) {
		float dist = (pos - circle).magnitude;

		float line = Mathf.Sqrt( dist * dist - radius * radius );
		float angle = Mathf.Asin( radius / dist ) * Mathf.Rad2Deg;

		Vector3 dir = (circle - pos).normalized;

		Quaternion rot = Quaternion.LookRotation( dir, Vector3.up );
		posL = pos + (rot * Quaternion.Euler(0,-angle,0)) * (Vector3.forward * line);
		posR = pos + (rot * Quaternion.Euler(0,angle,0)) * (Vector3.forward * line);
	}

	/////////////////////////////////////////////////////////////////////////////////////////
	// String Split
    public static string [] Split_Line( string str ) {
        return str.Split( _Split_Line, System.StringSplitOptions.RemoveEmptyEntries );
    }

    public static string [] Split_Tab( string str ) {
        return str.Split( _Split_Tab, System.StringSplitOptions.None );
    }

    public static string [] Split_Comma( string str ) {
        return str.Split( _Split_Comma, System.StringSplitOptions.None );
    }
	/////////////////////////////////////////////////////////////////////////////////////////

	///////////////////////////////////////////////////////////////////////////////////////
	// Create GameObject
	public static GameObject NewUI( string path, Transform parent ) {
		GameObject obj = new GameObject();
		if( obj != null ) {
			Transform form = obj.transform;
			form.name = path;

			form.SetParent( parent );

			form.localPosition = Vector3.zero;
			form.localRotation = Quaternion.identity;
			form.localScale = Vector3.one;
		}
		return obj;
	}

	public static GameObject LoadUI( string path, Transform parent ) {
		GameObject ori = jRes.LoadAsset<GameObject>( path );
		if( ori != null ) {
			return LoadUI( ori, parent );
		}
		return null;
	}

	public static GameObject LoadUI( GameObject go, Transform parent ) {
		GameObject obj = GameObject.Instantiate( go ) as GameObject;
		if( obj != null ) {
			Transform form = obj.transform;
			form.name = obj.name;

			form.SetParent( parent );

			form.localPosition = Vector3.zero;
			form.localRotation = Quaternion.identity;
			form.localScale = Vector3.one;
		}
		return obj;
	}
	///////////////////////////////////////////////////////////////////////////////////////
}
