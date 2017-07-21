using UnityEngine;
// xml
using System.Xml;
using System.Xml.Serialization;
// io
using System.IO;
// dictionary
using System.Collections;
using System.Collections.Generic;
// method info
using System.Reflection;

public class jRes {
	public static Object LoadAsset( string path, System.Type type ) {
		return Resources.Load( path, type );
	}

	public static T LoadAsset<T>( string path ) where T : Object {
		return LoadAsset( path, typeof(T) ) as T;
	}

    /////////////////////////////////////////////////////////////////////////////////////////
    // Load XML
    static object Deserialize_Xml( StreamReader r, System.Type type ) {
		XmlSerializer s = new XmlSerializer( type );
		return s.Deserialize( r );
    }

    static object Deserialize_Xml( string str, System.Type type ) {
        return Deserialize_Xml( System.Text.Encoding.UTF8.GetBytes( str ), type );
    }

    static object Deserialize_Xml( byte [] bytes, System.Type type ) {
        object obj = null;
        using( MemoryStream ms = new MemoryStream( bytes ) ) {
            using( StreamReader r = new StreamReader( ms, System.Text.Encoding.UTF8 ) ) {
				obj = Deserialize_Xml( r, type );
				r.Close();
			}
			ms.Close();
        }
        return obj;
    }

	static System.Type Xml2Type( string str ) {
		int idx1 = str.IndexOf( '<' ) + 1;
		if( idx1 < 0 ) {
			return null;
		}

		while( str[idx1] == '?' ) {
			idx1 = str.IndexOf( '<', idx1 ) + 1;
		}

		int idx2 = str.IndexOf( ' ', idx1 );
		if( idx2 < 0 ) {
			return null;
		}

		string typeName = str.Substring( idx1, idx2 - idx1 );
		return System.Type.GetType( typeName );
	}

	public static object LoadXml( string path ) {
#if UNITY_EDITOR_WIN
		UnityEditor.AssetDatabase.Refresh();
#endif

		TextAsset ts = LoadAsset<TextAsset>( path );
		if( (ts != null) && (ts.bytes != null) ) {
			string str = jUtil.Bytes2String( ts.bytes );

			System.Type type;
			if( (type = Xml2Type( str )) != null ) {
				return Deserialize_Xml( ts.bytes, type );
			}
		}

		return null;
	}
    /////////////////////////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////////////////////////
    // Save XML
	static void Serialize_Xml( StreamWriter w, object obj ) {
		XmlSerializer s = new XmlSerializer( obj.GetType() );
		s.Serialize( w, obj );
    }

    static string Serialize_Xml( object obj ) {
        using( MemoryStream ms = new MemoryStream() ) {
            using( StreamWriter w = new StreamWriter( ms, System.Text.Encoding.UTF8 ) ) {
				Serialize_Xml( w, obj );
				w.Close();

				string str = System.Text.Encoding.UTF8.GetString( ms.ToArray() );
				ms.Close();

				return str;
			}
        }
    }

	public static void SaveXml( string path, object obj ) {
		using( StreamWriter w = new StreamWriter( "Assets/Resources/" + path + ".xml", false, System.Text.Encoding.UTF8 ) ) {
            Serialize_Xml( w, obj );
			w.Close();
		}
		
#if UNITY_EDITOR_WIN
		UnityEditor.AssetDatabase.Refresh();
#endif
	}
	/////////////////////////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////////////////////////
    // LoadResString
	public static string LoadResString( string path ) {
		string str = "";

		TextAsset textAsset = LoadAsset<TextAsset>( path );
		if( (textAsset != null) && (textAsset.bytes != null) ) {
			str = jUtil.Bytes2String( textAsset.bytes );
			textAsset = null;
		}

		return str;
	}

	public static void SaveResString( string path, string str ) {
		using( FileStream fs = new FileStream( path, FileMode.Create ) ) {
			byte [] ret = System.Text.Encoding.UTF8.GetBytes( str );
			fs.Write( ret, 0, ret.Length );
			fs.Close();
		}
	}
    /////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////
    // PlayerPrefs
    public static object LoadPrefs( string key, System.Type type ) {
        if( PlayerPrefs.HasKey( key ) ) {
            string str = PlayerPrefs.GetString( key );
            if( str.Length > 0 ) {

				byte [] bytes = jUtil.FromBase64( str );
				str = jUtil.Bytes2String( bytes );

                return Deserialize_Xml( str, type );
            }
        }
        return null;
    }

    public static void SavePrefs( string key, object data ) {
        string str = Serialize_Xml( data );

		byte [] bytes = jUtil.String2Bytes( str );
		str = jUtil.ToBase64( bytes );

        PlayerPrefs.SetString( key, str );
		PlayerPrefs.Save();
    }

    public static void DeletePrefs( string key ) {
		PlayerPrefs.DeleteKey( key );
		PlayerPrefs.Save();
	}

    public static T LoadPrefs<T>( string key ) where T : class, new() {
		return LoadPrefs( key, typeof(T) ) as T;
    }
    /////////////////////////////////////////////////////////////////////////////////////////
}
