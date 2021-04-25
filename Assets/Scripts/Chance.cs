using System;


public static class Chance
{
    public static bool CoinFlip()
    {
        Random r = new Random();
        int result = r.Next(0, 2);
        return result == 1;
    }

    public static int Range(int start, int end)
    {
        Random r = new Random();
        int result = r.Next(Math.Min(start, end), Math.Max(start, end));
        return result;
    }

    public static float Range(float start, float end)
    {
        Random r = new Random();
        double result = (r.NextDouble() * (end - start) + start);
        return (float)result;
    }
}
