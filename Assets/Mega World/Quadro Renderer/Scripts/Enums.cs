namespace QuadroRendererSystem
{
    public enum CurrentTab
    {
        QualitySettings,
        CamerasSettings
    }

    public enum BillboardGenerationActions
    {
        ReplaceLastLOD,
        AddNewLOD
    }

    public enum GetAdditionalShadow 
    {
        None,
        MinCullingDistance,
        IncreaseBoundingSphere,
        DirectionLightShadowVisible
    }
    
    public enum RenderMode
    {
        GPUInstanced,
        GPUInstancedIndirect
    }

    public enum HandleSettingsMode
    { 
        Custom,
        Standard
    }

    public enum TreeType
    {
        None,
        MeshTree,
        TreeCreatorTree,
        SoftOcclusionTree,
        SpeedTree,
        SpeedTree8
    }
}