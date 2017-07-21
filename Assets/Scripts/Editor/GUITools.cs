using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GUITools {
	static bool _isEnable = true;
	public static bool isEnable {
		get { return _isEnable; }
		set { _isEnable = value; }
	}

    public static void Box( Rect rt, string label ) {
		if( isEnable == false ) {
			return;
		}

		GUIStyle style = new GUIStyle (GUI.skin.box);
		style.normal = GUI.skin.button.active;
		GUI.Box (rt, label, style);
	}

	public static void LabelArea( Rect rt, string label ) {
		if( isEnable == false ) {
			return;
		}

		GUIStyle style = new GUIStyle( EditorStyles.textField );
		style.alignment = TextAnchor.MiddleLeft;
		EditorGUI.LabelField( rt, label, style );
	}

	public static void LabelField( Rect rt, string label ) {
		if( isEnable == false ) {
			return;
		}

		GUIStyle style = new GUIStyle (GUI.skin.label);
		style.alignment = TextAnchor.MiddleLeft;
		EditorGUI.LabelField (rt, label, style);
	}

	public static void LabelField( Rect rt, string label, TextAnchor anchor ) {
		if( isEnable == false ) {
			return;
		}

		GUIStyle style = new GUIStyle (GUI.skin.label);
		style.alignment = anchor;
		EditorGUI.LabelField (rt, label, style);
	}
	
	public static void LabelField( Rect rt, string label, string data, float labelSize ) {
		if( isEnable == false ) {
			return;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}
		
		rt.x += labelSize;
		rt.width = w - labelSize;
		LabelArea( rt, data );
	}

	public static int IntField( Rect rt, string label, int data, float labelSize ) {
		if( isEnable == false ) {
			return data;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}

		rt.x += labelSize;
		rt.width = w - labelSize;

        GUIStyle style = new GUIStyle( EditorStyles.textField );
		style.alignment = TextAnchor.MiddleLeft;
		return EditorGUI.IntField( rt, data, style );
	}
	
	public static void IntRange( Rect rt, string label, ref int min, int max, float labelSize ) {
		if( isEnable == false ) {
			return;
		}

		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}

        GUIStyle style = new GUIStyle( EditorStyles.textField );
		style.alignment = TextAnchor.MiddleLeft;

		float ww = 50;
		rt.x += labelSize;

		rt.width = ww;
		min = EditorGUI.IntField( rt, min, style );
		rt.x += rt.width;

		rt.width = 10;
		LabelField( rt, "/" );
		rt.x += rt.width;

		rt.width = ww;
		LabelArea( rt, max.ToString() );
	}

	public static int IntPopup( Rect rt, string label, int data, string [] strs, int [] ints, float labelSize ) {
		if( isEnable == false ) {
			return data;
		}

		float w = rt.width;
		if (labelSize > 0) {
			rt.width = labelSize;
			LabelField( rt, label );
		}

		rt.x += labelSize;
		rt.width = w - labelSize;

		GUIStyle style = new GUIStyle (EditorStyles.popup);
		style.fixedHeight = rt.height;
		return EditorGUI.IntPopup (rt, data, strs, ints, style);
	}

    public static string StringPopup( Rect rt, string label, string name, string [] strs, float labelSize ) {
		if( isEnable == false ) {
			return "";
		}

		int i, index = 0;

		List<int> intList = new List<int>();
		for( i=0; i<strs.Length; i++ ) {
			if( strs[i].CompareTo( name ) == 0 ) {
				index = i;
			}
			intList.Add( i );
		}
		
		index = GUITools.IntPopup( rt, label, index, strs, intList.ToArray(), labelSize );
		return strs[index];
    }

	public static int StringIndexPopup( Rect rt, string label, int start, int index, string [] strs, float labelSize ) {
		if( isEnable == false ) {
			return index;
		}

		int i, data;
		
		List<int> intList = new List<int>();
		for( i=0; i<strs.Length; i++ ) {
            data = start + i;
			intList.Add( data );
		}
		
		return GUITools.IntPopup( rt, label, index, strs, intList.ToArray(), labelSize );
	}
	
	public static string TextField( Rect rt, string label, string data, float labelSize ) {
		if( isEnable == false ) {
			return data;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}
		
		rt.x += labelSize;
		rt.width = w - labelSize;
		return EditorGUI.TextField( rt, data );
	}
	
	public static int BoolField( Rect rt, string label, int data ) {
		if( isEnable == false ) {
			return data;
		}

		bool bData = (data != 0)?true:false;
		if( EditorGUI.ToggleLeft( rt, label, bData ) == true ) {
			return 1;
		}
		return 0;
	}
	
	public static System.Enum EnumPopup( Rect rt, string label, System.Enum data, float labelSize ) {
		if( isEnable == false ) {
			return data;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}
		
		rt.x += labelSize;
		rt.width = w - labelSize;
		
		GUIStyle style = new GUIStyle( EditorStyles.popup );
		style.fixedHeight = rt.height;
		return EditorGUI.EnumPopup( rt, data, style );
	}

	public static Object ObjectField( Rect rt, string label, Object data, System.Type type, float labelSize ) {
		if( isEnable == false ) {
			return data;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}
		
		rt.x += labelSize;
		rt.width = w - labelSize;
		return EditorGUI.ObjectField( rt, data, type, false );
	}

	public static int IntSlider( Rect rt, string label, int data, int min, int max, float labelSize ) {
		if( isEnable == false ) {
			return data;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}

		float ww = 10 + 50;
		
		rt.x += labelSize;
		rt.width = w - labelSize - ww;
		int ret = EditorGUI.IntSlider( rt, data, min, max );
		rt.x += rt.width;

		rt.width = 10;
		GUITools.LabelField( rt, "/" );
		rt.x += rt.width;

		rt.width = ww - rt.width;
		GUITools.LabelArea( rt, max.ToString() );
		return ret;
	}

	public static long LongField( Rect rt, string label, long data, float labelSize ) {
		if( isEnable == false ) {
			return data;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}
		
		rt.x += labelSize;
		rt.width = w - labelSize;

        GUIStyle style = new GUIStyle( EditorStyles.textField );
		style.alignment = TextAnchor.MiddleLeft;
		return EditorGUI.LongField( rt, data, style );
	}

	public static double DoubleField( Rect rt, string label, double data, float labelSize ) {
		if( isEnable == false ) {
			return data;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}
		
		rt.x += labelSize;
		rt.width = w - labelSize;

        GUIStyle style = new GUIStyle( EditorStyles.textField );
		style.alignment = TextAnchor.MiddleLeft;
		return EditorGUI.DoubleField( rt, data, style );
	}

	public static float FloatField( Rect rt, string label, float data, float labelSize ) {
		if( isEnable == false ) {
			return data;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}
		
		rt.x += labelSize;
		rt.width = w - labelSize;

        GUIStyle style = new GUIStyle( EditorStyles.textField );
		style.alignment = TextAnchor.MiddleLeft;
		return EditorGUI.FloatField( rt, data, style );
	}

	public static float FloatSlider( Rect rt, string label, float data, float min, float max, float labelSize ) {
		if( isEnable == false ) {
			return data;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}
		
		float ww = 10 + 50;
		
		rt.x += labelSize;
		rt.width = w - labelSize - ww;
		float ret = EditorGUI.Slider( rt, data, min, max );
		rt.x += rt.width;
		
		rt.width = 10;
		GUITools.LabelField( rt, "/" );
		rt.x += rt.width;
		
		rt.width = ww - rt.width;
		GUITools.LabelArea( rt, max.ToString("F2") );
		return ret;
	}
	
	public static void FloatRange( Rect rt, string label, ref float minValue, ref float maxValue, float labelSize ) {
		if( isEnable == false ) {
			return;
		}

		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}

        GUIStyle style = new GUIStyle( EditorStyles.textField );
		style.alignment = TextAnchor.MiddleLeft;

		float ww = 50;
		rt.x += labelSize;

		rt.width = ww;
		minValue = EditorGUI.FloatField( rt, minValue, style );
		rt.x += rt.width;
		
		rt.width = 10;
		LabelField( rt, "-" );
		rt.x += rt.width;

		rt.width = ww;
		maxValue = EditorGUI.FloatField( rt, maxValue, style );
	}

    public static bool IntSlider( Rect rt, string name, ref int value, int maxLimit, Color color ) {
		if( isEnable == false ) {
			return false;
		}

		Color col = GUI.color;

        GUI.color = color;
		GUITools.Box( rt, "" );

		GUI.color = Color.white;

        rt.Set( rt.x + 5, rt.y + 5, rt.width - 10, 20 );

        int ret = value;
		value = GUITools.IntSlider( rt, name, ret, 0, maxLimit, 140 );

        GUI.color = col;

        return ((value != ret)?true:false);
	}

	public static int MinMaxSlider( Rect rt, string label, ref int minValue, ref int maxValue, int minLimit, int maxLimit, float labelSize ) {
		if( isEnable == false ) {
			return 0;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}

        int modify = 0;

		float ww = 50;
		float ww2 = 5;

		rt.x += labelSize;
		rt.width = w - ww2 - labelSize - ww - 10 - ww;
		float min = (float)minValue;
		float max = (float)maxValue;
		//GUI.skin.horizontalSliderThumb.normal.background = GUI.skin.button.normal.background;
        EditorGUI.MinMaxSlider( rt, ref min, ref max, (float)minLimit, (float)maxLimit );
        if( minValue != (int)min) {
            modify = -1;
        }
        if( maxValue != (int)max ) {
            modify = 1;
        }
		minValue = (int)min;
		maxValue = (int)max;
		rt.x += (rt.width + ww2);

        int ret;

		rt.width = ww;
        ret = minValue;
		minValue = GUITools.IntField( rt, "", ret, 0 );
		if( minValue < minLimit ) {
			minValue = minLimit;
		}
        if( minValue != ret ) {
            modify = -1;
        }
		rt.x += rt.width;

		rt.width = 10;
		GUITools.LabelField( rt, "/" );
		rt.x += rt.width;

		rt.width = ww;
        ret = maxValue;
		maxValue = GUITools.IntField( rt, "", ret, 0 );
		if( maxValue > maxLimit ) {
			maxValue = maxLimit;
		}
        if( maxValue != ret ) {
            modify = 1;
        }
		rt.x += rt.width;

        return modify;
	}

	public static int MinMaxSlider( Rect rt, string name, ref int minValue, ref int maxValue, int maxLimit, Color color ) {
		if( isEnable == false ) {
			return 0;
		}

		Color col = GUI.color;

        GUI.color = color;
		GUITools.Box( rt, "" );

		GUI.color = Color.white;

		if( (minValue == 0) && (maxValue <= 0) ) {
			maxValue = maxLimit;
		}
		
        rt.Set( rt.x + 5, rt.y + 5, rt.width - 10, 20 );
		int ret = GUITools.MinMaxSlider( rt, name, ref minValue, ref maxValue, 0, maxLimit, 140 );

        GUI.color = col;
        return ret;
	}
	
	public static void Vector3Field( Rect rt, string label, ref float x, ref float y, ref float z, float labelSize ) {
		if( isEnable == false ) {
			return;
		}

		Vector3 data = new Vector3( x, y, z );
		data = Vector3Field( rt, label, data, labelSize );
		x = data.x;
		y = data.y;
		z = data.z;
	}
	
	public static Vector3 Vector3Field( Rect rt, string label, Vector3 data, float labelSize ) {
		if( isEnable == false ) {
			return data;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}

		rt.x += labelSize;
		rt.width = (w - labelSize - 4) / 3;
		data.x = FloatField( rt, "X", data.x, 15 );

		rt.x += rt.width + 2;
		data.y = FloatField( rt, "Y", data.y, 15 );

		rt.x += rt.width + 2;
		data.z = FloatField( rt, "Z", data.z, 15 );

		return data;
	}

	public static void ColorField( Rect rt, string label, ref float r, ref float g, ref float b, float labelSize ) {
		if( isEnable == false ) {
			return;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}
		
		rt.x += labelSize;
		rt.width = w - labelSize;
		
		Color data = new Color(r,g,b);
		data = EditorGUI.ColorField( rt, data );
		r = data.r;
		g = data.g;
		b = data.b;
	}

	public static void ColorField( Rect rt, string label, ref float r, ref float g, ref float b, ref float a, float labelSize ) {
		if( isEnable == false ) {
			return;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}
		
		rt.x += labelSize;
		rt.width = w - labelSize;
		
		Color data = new Color(r,g,b,a);
		data = EditorGUI.ColorField( rt, data );
		r = data.r;
		g = data.g;
		b = data.b;
        a = data.a;
	}
	
	public static Color ColorField( Rect rt, string label, Color data, float labelSize ) {
		if( isEnable == false ) {
			return data;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}
		
		rt.x += labelSize;
		rt.width = w - labelSize;
		return EditorGUI.ColorField( rt, data );
	}

    public static AnimationCurve CurveField( Rect rt, string label, AnimationCurve value, float labelSize ) {
		if( isEnable == false ) {
			return value;
		}

		float w = rt.width;
		if( labelSize > 0 ) {
			rt.width = labelSize;
			LabelField( rt, label );
		}
		
		rt.x += labelSize;
		rt.width = w - labelSize;
        return EditorGUI.CurveField( rt, value );
    }

    public static bool ClickFocus( Vector2 click, float x, float y, float w, float h, int index, ref int focus, Rect rt, Vector2 scroll ) {
		if( (click.x != 0) || (click.y != 0) ) {
			float xMin = Mathf.Max( x - scroll.x, rt.x );
			float yMin = Mathf.Max( y - scroll.y, rt.y );
			float xMax = Mathf.Min( x + w - scroll.x, rt.x + rt.width );
			float yMax = Mathf.Min( y + h - scroll.y, rt.y + rt.height );
			rt.Set( xMin, yMin, xMax - xMin, yMax - yMin );
			if( rt.Contains( click ) == true ) {
				focus = index;
                return true;
			}
		}
        return false;
    }

    public static void ScrollFocus( float height, float length, float size, ref int show, int focus, int index, ref Vector2 scroll ) {
        if( show != focus ) {
            return;
        }
        show = -1;

        if( height > length ) {
            return;
        }

        float bottom = index * size;
        float top = (index + 1) * size - height;
        if( top < 0 ) {
            top = 0;
        }

        if( scroll.y < top ) {
            scroll.y = top;
        }
        if( scroll.y > bottom ) {
            scroll.y = bottom;
        }
    }

    public static void ScrollFocus( float y, float h, Rect rt, ref int show, int focus, int index, int select, ref Vector2 scroll ) {
        if( index != select ) {
            return;
        }

        if( show != focus ) {
            return;
        }
        show = -1;

        float bottom = (y - rt.y);
        float top = (y - rt.y) + (h - rt.height);
        if( top < 0 ) {
            top = 0;
        }

        if( scroll.y < top ) {
            scroll.y = top;
        }
        if( scroll.y > bottom ) {
            scroll.y = bottom;
        }
    }

	public static bool DrawHeader( string text ) {
		return DrawHeader( text, text );
	}

	public static bool DrawHeader( string text, string key ) {
		bool state = EditorPrefs.GetBool( key, true );
		if( !state ) {
			GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
		}

		GUILayout.BeginHorizontal();
		GUI.changed = false;

		text = "<b><size=11>" + text + "</size></b>";
		if( state ) {
			text = "\u25BC " + text;
		} else {
			text = "\u25BA " + text;
		}
		if( !GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f)) ) {
			state = !state;
		}

		if( GUI.changed ) {
			EditorPrefs.SetBool(key, state);
		}

		GUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;

		if( !state ) {
			GUILayout.Space(3f);
		}

		return state;
	}

	public static void BeginContents() {
		GUILayout.BeginHorizontal();
		EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));

		GUILayout.BeginVertical();
		GUILayout.Space(2f);
	}

	public static void EndContents() {
		GUILayout.Space(3f);
		GUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(3f);
		GUILayout.EndHorizontal();
		GUILayout.Space(3f);
	}

	public static void SetLabelWidth( float width ) {
		EditorGUIUtility.labelWidth = width;
	}

	public static void RegisterUndo( string name, params Object[] objects ) {
		if( objects != null && objects.Length > 0 ) {
			UnityEditor.Undo.RecordObjects( objects, name );
			for( int i=0; i<objects.Length; i++ ) {
				Object obj = objects[i];
				if( obj == null ) {
					continue;
				}
				EditorUtility.SetDirty(obj);
			}
		}
	}

	public static void SetDirty( UnityEngine.Object obj ) {
#if UNITY_EDITOR
		if( obj ) {
			UnityEditor.EditorUtility.SetDirty( obj );
		}
#endif
	}
}
