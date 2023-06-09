using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PieceData), false)]
[CanEditMultipleObjects]
[System.Serializable]

public class PieceDataEditor : Editor
{
    private PieceData PieceDataInstance => target as PieceData;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ClearBoardButton();
        EditorGUILayout.Space();

        DrawColumsInputFields();
        EditorGUILayout.Space();

        if (PieceDataInstance.board != null && PieceDataInstance.columns > 0 && PieceDataInstance.rows > 0)
        {
            DrawBoardTable();
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(PieceDataInstance);
        }
    }
    public void ClearBoardButton()
    {
        if (GUILayout.Button("Clear Board"))
        {
            PieceDataInstance.Clear();
        }
    }
    private void DrawColumsInputFields()
    {
        var columnsTemp = PieceDataInstance.columns;
        var rowsTemp = PieceDataInstance.rows;

        PieceDataInstance.columns = EditorGUILayout.IntField("Columns", PieceDataInstance.columns);
        PieceDataInstance.rows = EditorGUILayout.IntField("Rows", PieceDataInstance.rows);

        if ((PieceDataInstance.columns != columnsTemp || PieceDataInstance.rows != rowsTemp) && PieceDataInstance.columns > 0 && PieceDataInstance.rows > 0)
        {
            PieceDataInstance.CreateNewBoard();
        }
    }

    private void DrawBoardTable()
    {
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;

        var headerColumStyle = new GUIStyle();
        headerColumStyle.fixedWidth = 65;
        headerColumStyle.alignment = TextAnchor.MiddleCenter;

        var rowstyle = new GUIStyle();
        rowstyle.fixedHeight = 25;
        rowstyle.alignment = TextAnchor.MiddleCenter;

        var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFieldStyle.normal.background = Texture2D.grayTexture;
        dataFieldStyle.onNormal.background = Texture2D.whiteTexture;

        for (var row = 0; row < PieceDataInstance.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(headerColumStyle);

            for (var column = 0; column < PieceDataInstance.columns; column++)
            {
                EditorGUILayout.BeginHorizontal(rowstyle);
                var data = EditorGUILayout.Toggle(PieceDataInstance.board[row].column[column], dataFieldStyle);
                PieceDataInstance.board[row].column[column] = data;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
