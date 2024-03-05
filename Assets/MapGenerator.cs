// Saara Honka, Peliteknologia Sykli 3
using JetBrains.Annotations;
using System;
using Unity.Profiling;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Renderer textureRenderer;

    // This can be set to true in the Inspector to automatically re-render the map on any change.
    public bool autoUpdate;

    [Header("General Settings")]
    public int mapWidth;
    public int mapHeight;
    public Vector2 offset;

    // See documentation for the different noise settings in the Noise.cs file.
    // Feel free to add Range sliders if you want to limit the values.
    [Header("Noise Settings")]
    public float noiseScale;
    public int octaves;
    [Range(0, 1)] public float persistence;
    public float lacunarity;
    public int seed;
    [Range(0, 1)] public float groundLimit;

    public TerrainType[] regions; // Taulukko erilaisia maanmuotoja, n‰m‰ m‰‰ritell‰‰n editorissa

    public void GenerateMap()
    {
        // If you want to generate additional noisemaps, you can call the function many times with randomized seeds and different options.
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset);

        // This actually draws the map so don't remove it.
        DrawNoiseMap(noiseMap);
    }

    /// <summary>
    /// There are used to clamp values in the inspector, since they break some parts of the map.
    /// </summary>
    void OnValidate()
    {
        if (mapWidth < 1) mapWidth = 1;
        if (mapHeight < 1) mapHeight = 1;
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
    }

    public void DrawNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // This is how you can get the value at a location in the noise map.
                float noise = noiseMap[y, x];

                // Vertaa nykyisen pikselin korkeutta editorissa m‰‰riteltyihin alueiden raja-arvoihin,
                // Sen perusteella piirt‰‰ kyseiselle alueelle editorissa valitun v‰rin
                for (int i = 0; i < regions.Length; i++)
                {
                    if (noise <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }

            }
        }

        // These just set colors to the texture and apply it. No need to touch these.
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width * 0.1f, 1f, height * 0.1f);
    }

}

/// <summary>
/// Erilaisten maanmuotojen j‰sent‰miseen,
/// N‰m‰ asetetaan editorissa.
/// </summary>
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;

}
