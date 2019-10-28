using UnityEngine;

public static class NoisyScreen {
    
    static int width = 80;
    static int height = 48;
    static Texture2D texture = new Texture2D(width, height);
    static float scale = 100;
    static float offsetX = 0;
    static float offsetY = 1;

    public static Texture2D UpdateTexture() {
        offsetX = Random.Range(-1000, 1000);
        offsetY = Random.Range(-1000, 1000);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return texture;
    }

    static Color CalculateColor(int x, int y) {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }
}
