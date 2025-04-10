﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Shapes © Freya Holmér - https://twitter.com/FreyaHolmer/
// Website & Documentation - https://acegikmo.com/shapes/
namespace Shapes {

	[CustomEditor( typeof(Triangle) )]
	[CanEditMultipleObjects]
	public class TriangleEditor : ShapeRendererEditor {

		SerializedProperty propA = null;
		SerializedProperty propB = null;
		SerializedProperty propC = null;
		SerializedProperty propColorMode = null;
		SerializedProperty propColorB = null;
		SerializedProperty propColorC = null;
		SerializedProperty propRoundness = null;
		SerializedProperty propHollow = null;
		SerializedProperty propThickness = null;
		SerializedProperty propThicknessSpace = null;

		ScenePointEditor scenePointEditor;

		List<Color> colors = new List<Color> { default, default, default };

		public override void OnEnable() {
			base.OnEnable();
			scenePointEditor = new ScenePointEditor( this ) { hasAddRemoveMode = false, hasEditColorMode = true };
			scenePointEditor.onValuesChanged += OnChangeColor;
		}

		void OnSceneGUI() {
			Triangle tri = target as Triangle;
			List<Vector3> pts = new List<Vector3>() { tri.A, tri.B, tri.C };
			if( tri.ColorMode == Triangle.TriangleColorMode.Single )
				( colors[0], colors[1], colors[2] ) = ( tri.ColorA, tri.ColorA, tri.ColorA );
			else
				( colors[0], colors[1], colors[2] ) = ( tri.ColorA, tri.ColorB, tri.ColorC );
			bool changed = scenePointEditor.DoSceneHandles( false, tri, pts, colors, tri.transform );
			if( changed )
				( tri.A, tri.B, tri.C ) = ( pts[0], pts[1], pts[2] );
		}

		void OnChangeColor( ShapeRenderer shape, int changeIndex ) {
			Triangle tri = shape as Triangle;
			Color newColor = colors[changeIndex];
			if( tri.ColorMode == Triangle.TriangleColorMode.Single ) {
				colors[0] = newColor;
				colors[1] = newColor;
				colors[2] = newColor;
			}

			( tri.ColorA, tri.ColorB, tri.ColorC ) = ( colors[0], colors[1], colors[2] );
		}

		public override void OnInspectorGUI() {
			base.BeginProperties( showColor: false );

			EditorGUILayout.PropertyField( propColorMode );
			EditorGUILayout.PropertyField( propRoundness );
			EditorGUILayout.PropertyField( propHollow );
			using( new EditorGUI.DisabledScope( propHollow.boolValue == false || propHollow.hasMultipleDifferentValues ) )
				ShapesUI.FloatInSpaceField( propThickness, propThicknessSpace );
			if( propColorMode.enumValueIndex == (int)Triangle.TriangleColorMode.Single ) {
				ShapesUI.PosColorField( "A", propA, base.propColor );
				ShapesUI.PosColorField( "B", propB, base.propColor, false );
				ShapesUI.PosColorField( "C", propC, base.propColor, false );
			} else {
				ShapesUI.PosColorField( "A", propA, base.propColor );
				ShapesUI.PosColorField( "B", propB, propColorB );
				ShapesUI.PosColorField( "C", propC, propColorC );
			}

			scenePointEditor.GUIEditButton( "Edit Points in Scene" );

			base.EndProperties();
		}

	}

}