namespace MegaWorld
{
    public enum PrefabType
    {
        Mesh = 0,
        Texture = 1
    }

    public enum TransformAxis
    {
        X,
        Y,
        Z,
    }

    public enum TransformSpace
    {
        Global,
        Local
    }

    public enum FalloffType 
    { 
        None,
        Add,
    }

    public enum FromDirection
    {
        SurfaceNormal,
        X,
        Y,
        Z,
    }
}