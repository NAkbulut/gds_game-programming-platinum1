using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Grid<TGridObject>
{
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float size;
    private TGridObject[,] gridArray;

    public Grid(int width, int height, float size, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject) {
        this.width = width;
        this.height = height;
        this.size = size;

        gridArray = new TGridObject[width, height];
        TextMesh[,] debugTextArray = new TextMesh[width, height];

        for (int i=0; i<gridArray.GetLength(0); i++) {
            for (int j=0; j<gridArray.GetLength(1); j++) {
                debugTextArray[i, j] = CreateWorldText(gridArray[i, j]?.ToString(), null, GetPosition(i, j) + new Vector3(size, size) * .5f, 30, Color.white, TextAnchor.MiddleCenter);
                gridArray[i, j] = createGridObject(this, i, j);
                Debug.DrawLine(GetPosition(i, j), GetPosition(i, j + 1), Color.white, 1000f);
                Debug.DrawLine(GetPosition(i, j), GetPosition(i + 1, j), Color.white, 1000f);
            }
        }
        Debug.DrawLine(GetPosition(0, height), GetPosition(width, height), Color.white, 1000f);
        Debug.DrawLine(GetPosition(width, 0), GetPosition(width, height), Color.white, 1000f);

        OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetSize() {
        return size;
    }

    public Vector3 GetPosition(int x, int y) {
        return new Vector3(x, y) * size;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt(worldPosition.x / size);
        y = Mathf.FloorToInt(worldPosition.y / size);
    }

    public void SetValue(int x, int y, TGridObject value) {
        gridArray[x, y] = value; 
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public void SetValue(Vector3 worldPosition, TGridObject value) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public TGridObject GetValue(int x, int y) {
        return gridArray[x, y];
    }

    public TGridObject GetValue(Vector3 worldposition) {
        int x, y;
        GetXY(worldposition, out x, out y);
        return GetValue(x, y);
    }
    
    public void TriggerGridObjectChanged(int x, int y) {
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000) {
            if (color == null) color = Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
        }
        
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder) {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}
