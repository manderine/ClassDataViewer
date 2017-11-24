using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Reflection;

public class ClassDataEditor : EditorWindow {
    Dictionary<string, object> _TableList = new Dictionary<string, object>();

    List<string> _FileList = new List<string>();
    string _PathName = "";//"Xmls/Tables/";
    string _TablePathName = "";

	public class SubData {
		public int _Index;
		public int _Count;
		public bool _Flush;
		public Vector2 _vScroll1;
		public Vector2 _vScroll2;

		public SubData() {
			_Index = 0;
			_Count = 0;
			_Flush = false;
			_vScroll1 = new Vector2( 0, 60 );
			_vScroll2 = Vector2.zero;
		}
	}

	SubData _SubData = new SubData();
	List<FieldInfo> _fiList = new List<FieldInfo>();

	[MenuItem("jTools/Table Info.", priority = 301)]
	static void Init() {
		ClassDataEditor window = GetWindow<ClassDataEditor>( "Table Info." );
		window.Show();
	}

	void OnEnable() {
        _TableList.Clear();

        _FileList.Clear();
		jUtil.GetFiles( _FileList, "Assets/Resources/" + _PathName, "*.xml" );
        _FileList.Insert( 0, "none" );

		if( (_SubData = jRes.LoadPrefs<SubData>( "class_data" )) == null ) {
			_SubData = new SubData();
		}

		if( (_TablePathName != null) && (_TablePathName.Length > 0) ) {
			if( _TableList.ContainsKey( _TablePathName ) == false ) {
				LoadTable( _TablePathName );
			}
		}
	}

	private void OnDisable() {
		jRes.SavePrefs( "class_data", _SubData );
	}

	private void OnDestroy() {
		jRes.DeletePrefs( "class_data" );
	}

	void OnGUI() {
		KeyEvent();
		DrawWindow();
		_SubData._Flush = false;
	}
	
	void OnInspectorUpdate() {
		Repaint();
	}

	void KeyEvent() {
		Event e = Event.current;
		switch( e.type ) {
		case EventType.keyDown :
			switch( e.keyCode ) {
			case KeyCode.UpArrow :
				if( _SubData._Index > 0 ) {
					_SubData._Index--;
					_SubData._Flush = true;
					Repaint();
				}
				e.Use();
				break;
			case KeyCode.DownArrow :
				if( _SubData._Index < (_SubData._Count - 1) ) {
					_SubData._Index++;
					_SubData._Flush = true;
					Repaint();
				}
				e.Use();
				break;
			}
			break;
		}
	}

	void DrawWindow() {
		float x = 0, y = 0, w = Screen.width, h = Screen.height - 23;

		float hh = 30;

        DrawFileData( x, y, w, hh );
		y += hh;
		h -= hh;

		DrawTableList( x, y, w, h );
	}

    void LoadTable( string tableName ) {
        if( tableName == "none" ) {
            return;
        }

        _TableList[ tableName ] = (object)jRes.LoadXml( tableName );
    }

    void SaveTable( string tableName ) {
        if( _TableList.ContainsKey( tableName ) == false ) {
            return;
        }

        if( _TableList[ tableName ] == null ) {
            return;
        }

        MethodInfo mi = _TableList[ tableName ].GetType().GetMethod( "Sort" );
        if( mi != null ) {
            mi.Invoke( _TableList[ tableName ], null );
        }

        jRes.SaveXml( tableName, _TableList[ tableName ] );
    }

    void DrawTableList( float x, float y, float w, float h ) {
        if( _TableList.ContainsKey( _TablePathName ) == false ) {
            return;
        }

        DrawDataTable( x, y, w, h, _TableList[ _TablePathName ], _SubData, _fiList );
    }

	void DrawFileData( float x, float y, float w, float h ) {
		x+=3;
		y+=2;
		h-=4;
		w-=6;
		
		Rect rt = new Rect( x, y, w, h );
		//GUITools.Box( rt, "" );
		
		float xx = x + 5;
		float ww = 400;
		float hh = 20;
		float labelSize = 120;
		
		y += 2;

        if( _FileList.Count <= 1 ) {
            return;
        }
		
		rt.Set( xx, y, ww, hh );
        string str = GUITools.StringPopup( rt, "Table Name", _TablePathName, _FileList.ToArray(), labelSize );
        if( _TablePathName.CompareTo( str ) != 0 ) {
            _TablePathName = str;

            if( _TableList.ContainsKey( _TablePathName ) == false ) {
                LoadTable( _TablePathName );
            }
        }
		
		x += (w - 300);
		w = 300;
        h -= 4;

        if( _TablePathName != "none" ) {
            ww = w / 2;

		    rt.Set( x, y, ww, h );
		    if( GUI.Button( rt, "Load" ) ) {
			    GUI.FocusControl( "Load" );

                LoadTable( _TablePathName );
		    }
		    x += ww;

		    rt.Set( x, y, ww, h );
		    if( GUI.Button( rt, "Save" ) ) {
			    GUI.FocusControl( "Save" );

                SaveTable( _TablePathName );
		    }
		    x += ww;
        }
	}

	static System.Type GetListType( object obj ) {
		System.Type type = obj.GetType().GetElementType();
		if( type == null ) {
			System.Type [] currList = obj.GetType().GetGenericArguments();
			if( (currList != null) && (currList.Length > 0) ) {
				type = currList[ currList.Length - 1 ];
			}
		}
		return type;
	}

    public static object GetDataList( object data ) {
        if( data == null ) {
            return null;
        }

        FieldInfo [] fis = data.GetType().GetFields();
		if( fis == null ) {
			return null;
		}

		int count = 0;
		for( int i=0; i<fis.Length; i++ ) {
			FieldInfo fi = fis[i];
            object temp = fi.GetValue( data );
            if( temp == null ) {
                continue;
            }

			if( fi.Attributes != FieldAttributes.Public ) {
				continue;
			}

			count++;
        }

		if( count > 1 ) {
			return null;
		}

        object obj = null;
		for( int i=0; i<fis.Length; i++ ) {
			FieldInfo fi = fis[i];
            object temp = fi.GetValue( data );
            if( temp == null ) {
                continue;
            }

			if( fi.Attributes != FieldAttributes.Public ) {
				continue;
			}

            if( temp is IList ) {
                obj = temp as IList;
            }
        }

        return obj;
    }

    public static void DrawDataTable( float x, float y, float w, float h, object data, SubData sub_data, List<FieldInfo> fiList ) {
        if( data == null ) {
            return;
        }

        float ww = 200;
        float add = 30;
		bool isFocus;

        object temp = null, obj = null;
        obj = data;

		//string typeName = obj.GetType().BaseType.Name.ToString();
		//string typeName2 = "TableList`1";
		//if( typeName.CompareTo( typeName2 ) == 0 ) {
			if( (temp = GetDataList( obj )) != null ) {
				//System.Type type = GetListType( temp );

				DrawDataList( x, y, ww, h - add, data.GetType().ToString(), temp, ref sub_data._vScroll1, ref sub_data._Index, ref sub_data._Count, out isFocus, sub_data._Flush );
				DrawDataAdd( x, y + h - add, ww, add, temp, ref sub_data._Index );

				IList lst = temp as IList;
				if( (sub_data._Index >= 0) && (sub_data._Index < lst.Count) ) {
					obj = lst[ sub_data._Index ];
				} else {
					obj = null;
				}

				x += ww;
				w -= ww;
			}
		//}

        DrawFieldList( x, y, w, h, obj, ref sub_data._vScroll2, fiList );
    }

    public static void DrawDataList( float x, float y, float w, float h, string name, object obj, ref Vector2 scroll, ref int index, ref int count, out bool isFocus, bool flush = false ) {
		isFocus = false;

        IList list = obj as IList;
        if( list == null ) {
            return;
        }

		Color color = GUI.color;
		
		Rect rt = new Rect( x, y, w, h );
		GUITools.Box( rt, name );
		
		float title = 20;
		float hh = 30;
		
		x += 5;
		y += (5 + title);
		w -= 10;
		h -= (10 + title);
		
		rt.Set( x, y, w, h - 4 );
		
		Rect rt2 = new Rect();
		float height = rt.height;
		float length = list.Count * hh;
		if( height < length ) {
			height = length;
			rt2.Set( x, y, w - 20, height );
		} else {
			rt2.Set( x, y, w, height );
		}
		
		count = list.Count;

        if( index > (count - 1) ) {
            index = (count - 1);
        }

		if( index < 0 ) {
            index = 0;
        }

		if( flush == true ) {
			float t = y + index * hh;
			float b = t + hh;

			float t1 = rt.y + scroll.y;
			if( t < t1 ) {
				scroll.y -= (t1 - t);
			}

			float b2 = rt.y + rt.height + scroll.y;
			if( b > b2 ) {
				scroll.y += (b - b2);
			}
		}

		scroll = GUI.BeginScrollView( rt, scroll, rt2 );

        FieldInfo fi;
		for( int i=0; i<count; i++ ) {
			if( i == index ) {
				GUI.color = Color.yellow;
			} else {
				GUI.color = Color.white;
			}
			
			string str = "(" + i + ") ID : ";
            if( list[i] != null ) {
                if( (fi = list[i].GetType().GetField( "_ID" )) != null ) {
                    str += fi.GetValue( list[i] );
                } else {
                    str += i;
                }
            } else {
                str += "null";
            }

			rt.Set( x, y + i * hh, rt2.width, hh );
			if( GUI.Button( rt, str ) ) {
				GUI.FocusControl( str );
				isFocus = true;
				
				index = i;
			}
		}
		
		GUI.EndScrollView();
		
		GUI.color = color;
    }

	public static void DrawDataAdd( float x, float y, float w, float h, object lst, ref int index ) {
        System.Type curr = lst.GetType();
        if( curr.IsGenericType == false ) {
            return;
        }

        System.Type [] currList = curr.GetGenericArguments();
        if( (currList == null) || (currList.Length == 0) ) {
            return;
        }

        System.Type type = currList[0];
        IList list = lst as IList;

        w /= 5;
		
		Rect rt = new Rect();
        object obj = null;
		
		rt.Set( x, y, w, h );
		if( GUI.Button( rt, "Ins" ) ) {
			GUI.FocusControl( "Ins" );

			if( (index >= 0) && (index < list.Count) ) {
                obj = System.Activator.CreateInstance( type );
				list.Insert( index, obj );
			} else {
                obj = System.Activator.CreateInstance( type );
				list.Add( obj );
				index = (list.Count - 1);
			}
		}
		x += w;
		
		rt.Set( x, y, w, h );
		if( GUI.Button( rt, "Add" ) ) {
			GUI.FocusControl( "Add" );
			
            obj = System.Activator.CreateInstance( type );
			list.Add( obj );
			index = (list.Count - 1);
		}
		x += w;
		
		rt.Set( x, y, w, h );
		if( GUI.Button( rt, "Copy" ) ) {
			GUI.FocusControl( "Copy" );
			
			if( (index >= 0) && (index < list.Count) ) {
				obj = jUtil.Clone( list[index], type );
				list.Insert( index, obj );
				index++;
			}
		}
		x += w;

		rt.Set( x, y, w, h );
		if( GUI.Button( rt, "Del" ) ) {
			GUI.FocusControl( "Del" );
			
			if( (index >= 0) && (index < list.Count) ) {
				list.RemoveAt( index );
				if( index > (list.Count - 1) ) {
					index = (list.Count - 1);
				}
			}
		}
		x += w;

		rt.Set( x, y, w, h / 2 );
		if( GUI.Button( rt, "Up" ) ) {
			GUI.FocusControl( "Up" );
			
			if( (index > 0) && (index < list.Count) ) {
				obj = list[index];
				list.RemoveAt( index );
				index--;
				list.Insert( index, obj );
			}
		}
		rt.y += rt.height;
		
		if( GUI.Button( rt, "Down" ) ) {
			GUI.FocusControl( "Down" );
			
			if( (index >= 0) && (index < (list.Count - 1)) ) {
				obj = list[index];
				list.RemoveAt( index );
				index++;
				list.Insert( index, obj );
			}
		}
	}

    static void DrawFieldList( float x, float y, float w, float h, object data, ref Vector2 scroll, List<FieldInfo> fiList ) {
        if( data == null ) {
            return;
        }

		Color color = GUI.color;
		
		Rect rt = new Rect( x, y, w, h );
		GUITools.Box( rt, data.GetType().ToString() );

		float labelSize = 250;
		float title = 20;
		float hh = 30;
		
		x += 5;
		y += (5 + title);
		w -= 10;
		h -= (10 + title);
        rt.Set( x, y, w, h );

		float height = rt.height;
		float length = GetFieldSubCount( data ) * hh;

        Rect rt2 = new Rect( 0, 0, 1, 1 );
		if( height < length ) {
			height = length;
			rt2.Set( x, y, w - 20, height );
		} else {
			rt2.Set( x, y, w, height );
		}
		scroll = GUI.BeginScrollView( rt, scroll, rt2 );
		
        //GUI.enabled = false;

		rt.Set( x + 5, y, rt2.width - 10, hh );
		DrawFieldSubList( rt.x, rt.y, rt.width, rt.height, "", ref data, labelSize, fiList );

        //GUI.enabled = true;

        GUI.EndScrollView();

        GUI.color = color;
    }

    public static int GetFieldSubCount( object data ) {
        int count = 0;
        FieldInfo [] fis = data.GetType().GetFields();
		if( fis == null ) {
			return 0;
		}

		for( int i=0; i<fis.Length; i++ ) {
			FieldInfo fi = fis[i];
            object temp = fi.GetValue( data );
            if( temp == null ) {
                continue;
            }

			if( fi.Attributes != FieldAttributes.Public ) {
				continue;
			}

			if( temp is IList ) {
				count++;
				foreach( object next in temp as IList ) {
					object value = next as object;
					if( isValue( value ) == false ) {
						count += GetFieldSubCount( value );
					}
					count++;
				}
            } else {
				if( isValue( temp ) == false ) {
					count += GetFieldSubCount( temp );
				}
				count++;
			}
        }

        return count;
    }

	static float DrawFieldSubData( float x, float y, float w, float h, string fiName, ref object data, float labelSize, bool check = true ) {
		float hSize = h;

		Rect rt = new Rect( x, y, w, h );
		string name;

		string typeName = data.GetType().ToString().Replace("UnityEngine.", "");

        if( data is int ) {
            int value = (int)data;
            name = "(int) " + fiName;
            value = GUITools.IntField( rt, name, value, labelSize );
			data = value;
        } else if( data is float ) {
            float value = (float)data;
            name = "(float) " + fiName;
            value = GUITools.FloatField( rt, name, value, labelSize );
			data = value;
        } else if( data is long ) {
            long value = (long)data;
            name = "(long) " + fiName;
            value = GUITools.LongField( rt, name, value, labelSize );
			data = value;
        } else if( data is double ) {
            double value = (double)data;
            name = "(double) " + fiName;
            value = GUITools.DoubleField( rt, name, value, labelSize );
			data = value;
        } else if( data is string ) {
            string value = data as string;
            name = "(string) " + fiName;
            value = GUITools.TextField( rt, name, value, labelSize );
			data = value;
        } else if( data is bool ) {
            bool value = (bool)data;
            name = "(bool) " + fiName;

			Rect rtSub = new Rect( rt.x, rt.y + 6, labelSize, rt.height - 6 );
            GUITools.LabelField( rtSub, name );
			rtSub.x += rtSub.width;
			rtSub.width = rt.width - (rtSub.x - rt.x);
            value = EditorGUI.ToggleLeft( rtSub, "", value );
			data = value;
        } else if( data is System.Enum ) {
            object value = data;
            name = "(" + data.GetType() + ") " + fiName;
            value = GUITools.EnumPopup( rt, name, (System.Enum)value, labelSize );
			data = value;
        } else if( data is AnimationCurve ) {
            AnimationCurve value = data as AnimationCurve;
            name = "(" + typeName + ") " + fiName;
            value = GUITools.CurveField( rt, name, value, labelSize );
			data = value;
        } else if( data is Color ) {
            Color value = (Color)data;
            name = "(" + typeName + ") " + fiName;
            value = GUITools.ColorField( rt, name, value, labelSize );
			data = value;
		} else if( data is System.DateTime ) {
			System.DateTime value = (System.DateTime)data;
			int year = value.Year, month = value.Month, day = value.Day, hour = value.Hour, minute = value.Minute, second = value.Second, millisecond = value.Millisecond;
			int dummy, flush = 0;
			Rect rtSub = rt;

			name = "(System.DateTime)" + fiName;

			rtSub.width = 300;
			dummy = year;
			year = GUITools.IntField( rtSub, name, year, labelSize );
			if( year != dummy ) {
				flush = 1;
			}
			rtSub.x += rtSub.width;

				rtSub.width = 10;
				GUITools.LabelField( rtSub, "/" );
				rtSub.x += rtSub.width;

			rtSub.width = 30;
			dummy = month;
			month = GUITools.IntField( rtSub, "월", month, 0 );
			if( month != dummy ) {
				flush = 1;
			}
			rtSub.x += rtSub.width;

				rtSub.width = 10;
				GUITools.LabelField( rtSub, "/" );
				rtSub.x += rtSub.width;

			rtSub.width = 30;
			dummy = day;
			day = GUITools.IntField( rtSub, "일", day, 0 );
			if( day != dummy ) {
				flush = 1;
			}
			rtSub.x += rtSub.width;

				rtSub.width = 30;
				GUITools.LabelField( rtSub, "-", TextAnchor.MiddleCenter );
				rtSub.x += rtSub.width;

			rtSub.width = 30;
			dummy = hour;
			hour = GUITools.IntField( rtSub, "시", hour, 0 );
			if( hour != dummy ) {
				flush = 1;
			}
			rtSub.x += rtSub.width;

				rtSub.width = 10;
				GUITools.LabelField( rtSub, ":" );
				rtSub.x += rtSub.width;

			rtSub.width = 30;
			dummy = minute;
			minute = GUITools.IntField( rtSub, "분", minute, 0 );
			if( minute != dummy ) {
				flush = 1;
			}
			rtSub.x += rtSub.width;

				rtSub.width = 10;
				GUITools.LabelField( rtSub, ":" );
				rtSub.x += rtSub.width;

			rtSub.width = 30;
			dummy = second;
			second = GUITools.IntField( rtSub, "초", second, 0 );
			if( second != dummy ) {
				flush = 1;
			}
			rtSub.x += rtSub.width;

				rtSub.width = 10;
				GUITools.LabelField( rtSub, ":" );
				rtSub.x += rtSub.width;

			rtSub.width = 30;
			dummy = millisecond;
			millisecond = GUITools.IntField( rtSub, "초2", millisecond, 0 );
			if( millisecond != dummy ) {
				flush = 1;
			}
			rtSub.x += rtSub.width;

			if( flush == 1 ) {
				value = new System.DateTime( year, month, day, hour, minute, second, millisecond );
			}
			data = value;
        } else {
			if( check == true ) {
				name = "(" + typeName + ")" + fiName;
	            GUITools.LabelField( rt, name );
			}
        }

		return hSize;
	}

	public static float DrawFieldSubList( float x, float y, float w, float h, string fiName, ref object data, float labelSize, List<FieldInfo> fiList, bool sub = false ) {
		float height = 0;

		if( isValue( data ) == true ) {
			height += DrawFieldSubData( x, y, w, h, fiName, ref data, labelSize, false );
		} else {
			if( (fiName != null) && (fiName.Length > 0) ) {
				GUITools.LabelField( new Rect(x,y,w,h), "<" + fiName + ">" );
				y += h;
				height += h;

				x += 20;
				w -= 20;
			}

			System.Type type = data.GetType();
			FieldInfo [] fis = type.GetFields();
			fiList.Clear();
			for( int i=0; i<fis.Length; i++ ) {
				if( fis[i].Name.CompareTo("_ID") == 0 ) {
					fiList.Insert( 0, fis[i] );
				} else {
					fiList.Add( fis[i] );
				}
			}
			fis = fiList.ToArray();

			for( int i=0; i<fis.Length; i++ ) {
				FieldInfo fi = fis[i];
				object temp = fi.GetValue( data );
				if( temp == null ) {
					continue;
				}

				if( fi.Attributes != FieldAttributes.Public ) {
					continue;
				}

				if( temp is IList ) {
					System.Type tempType = GetListType( temp );

					string str;
					if( temp is System.Array ) {
						System.Array arr1 = temp as System.Array;
						int count1 = arr1.Length;

						str = "(" + tempType + "[])" + fi.Name;

						Rect rt = new Rect( x + 10, y, w - 10, h );
						int count2 = GUITools.IntField( rt, str, count1, labelSize );
						if( count2 != count1 ) {
							object obj = System.Array.CreateInstance( tempType, count2 );
							System.Array arr2 = obj as System.Array;

							for( int j=0; j<arr2.Length; j++ ) {
								if( j < arr1.Length ) {
									arr2.SetValue( arr1.GetValue( j ), j );
								} else {
									arr2.SetValue( System.Activator.CreateInstance( tempType ), j );
								}
							}

							temp = obj;
						}
					} else {
						IList lst = temp as IList;
						int count1 = lst.Count;

						str = "(List<" + tempType + ">)" + fi.Name;

						Rect rt = new Rect( x + 10, y, w - 10, h );
						int count2 = GUITools.IntField( rt, str, count1, labelSize );
						if( count2 != count1 ) {
							if( count2 > count1 ) {
								while( count2 > lst.Count ) {
									object tempObj = System.Activator.CreateInstance( tempType );
									lst.Add( tempObj );
								}
							} else {
								while( count2 < lst.Count ) {
									lst.RemoveAt( lst.Count - 1 );
								}
							}
						}
					}

					y += h;
					height += h;

					{
						IList lst = temp as IList;

						for( int j=0; j<lst.Count; j++ ) {
							object value = lst[j];

							if( isValue( value ) == true ) {
								str = fi.Name + ("[" + j + "]");
							} else {
								str = "(" + value.GetType() + ")" + fi.Name + ("[" + j + "]");
							}

							float ret = DrawFieldSubList( x + 20, y, w - 20, h, str, ref value, labelSize, fiList );
							lst[j] = value;

							y += ret;
							height += ret;
						}

						fi.SetValue( data, temp );
					}
				} else {
					string str;
					object value = temp;

					if( isValue( value ) == true ) {
						str = fi.Name;
					} else {
						str = "(" + value.GetType() + ")" + fi.Name;
					}

					float ret2 = DrawFieldSubList( x, y, w, h, str, ref value, labelSize, fiList );
					fi.SetValue( data, value );

					y += ret2;
					height += ret2;
				}
			}
		}

		return height;
	}

	static bool isValue( object data ) {
        if( data is int ) {
        } else if( data is float ) {
        } else if( data is long ) {
        } else if( data is double ) {
        } else if( data is string ) {
        } else if( data is bool ) {
        } else if( data is System.Enum ) {
        } else if( data is AnimationCurve ) {
        } else if( data is Color ) {
		} else if( data is System.DateTime ) {
		} else {
			return false;
		}
		return true;
	}
}
