using System;

namespace QuadroRendererSystem
{
    public static class QuadroRendererConstants 
    {
        public static readonly float VERSION = 1.05f;

        private static QuadroRendererSettings s_quadroRendererSettings;
        public static QuadroRendererSettings QuadroRendererSettings
        {
            get
            {
                if (s_quadroRendererSettings == null)
                {
                    s_quadroRendererSettings = QuadroRendererSettings.GetDefaultQuadroRendererSettings();
                }
                return s_quadroRendererSettings;
            }
            set
            {
                s_quadroRendererSettings = value;
            }
        }
        
        private const string ASSETS_PATH = "Assets/";
        public static string Resources = "Resources";
        public static string QuadroRenderer = "Quadro Renderer";
        public static string Shaders = "Shaders";
        public static string PersistentStorageData = "Persistent Storage Data";
        public static string QuadroRendererSettingsName = "Quadro Renderer Settings";
        public static string ShaderBindings = "Shader Bindings";
        public static string BillboardTextures = "Billboard Textures";
        public static string Prefab = "Prefab";
        public static string StorageData = "Storage Data";
        public static readonly string SHADER_VARIANT_COLLECTION_DEFAULT_NAME = "Shader Variant Collection";
        
        public static string PathToQuadroRendererSystem = GetDefaultPath();
        public static string PathToResources = CombinePath("Assets", Resources);
        public static string PathToResourcesQuadroRenderer = CombinePath(PathToResources, QuadroRenderer);
        public static string PathToPrototypesBillboardTextures = CombinePath(PathToResourcesQuadroRenderer, BillboardTextures);
        public static string PathToPrototypesPrefab = CombinePath(PathToResourcesQuadroRenderer, Prefab);
        public static string PathToPersistentStorageData = CombinePath(PathToResourcesQuadroRenderer, PersistentStorageData);
        public static string PathToQuadroRendererSettings = CombinePath(PathToResourcesQuadroRenderer, QuadroRendererSettingsName);
        public static string PathToShaderBindings = CombinePath(PathToResourcesQuadroRenderer, ShaderBindings);
        public static string PathToShaderVariantCollection = CombinePath(PathToResourcesQuadroRenderer, ShaderBindings);
        

        #region Shaders
        public static readonly string SHADER_GPUI_STANDARD = "QuadroRenderer/Standard";
        public static readonly string SHADER_GPUI_STANDARD_SPECULAR = "QuadroRenderer/Standard (Specular setup)";
        public static readonly string SHADER_GPUI_STANDARD_ROUGHNESS = "QuadroRenderer/Standard (Roughness setup)";

        public static readonly string SHADER_UNITY_STANDARD = "Standard";
        public static readonly string SHADER_UNITY_STANDARD_SPECULAR = "Standard (Specular setup)";
        public static readonly string SHADER_UNITY_STANDARD_ROUGHNESS = "Standard (Roughness setup)";

        public static readonly string SHADER_UNITY_SPEED_TREE = "Nature/SpeedTree";
        public static readonly string SHADER_UNITY_SPEED_TREE_URP = "Universal Render Pipeline/Nature/SpeedTree7";
        public static readonly string SHADER_UNITY_SPEED_TREE_8 = "Nature/SpeedTree8";
        public static readonly string SHADER_UNITY_SPEED_TREE_8_URP = "Universal Render Pipeline/Nature/SpeedTree8";
        public static readonly string SHADER_UNITY_TREE_CREATOR_BARK = "Nature/Tree Creator Bark";
        public static readonly string SHADER_UNITY_TREE_CREATOR_BARK_OPTIMIZED = "Hidden/Nature/Tree Creator Bark Optimized";
        public static readonly string SHADER_UNITY_TREE_CREATOR_LEAVES = "Nature/Tree Creator Leaves";
        public static readonly string SHADER_UNITY_TREE_CREATOR_LEAVES_OPTIMIZED = "Hidden/Nature/Tree Creator Leaves Optimized";
        public static readonly string SHADER_UNITY_TREE_CREATOR_LEAVES_FAST = "Nature/Tree Creator Leaves Fast";
        public static readonly string SHADER_UNITY_TREE_CREATOR_LEAVES_FAST_OPTIMIZED = "Hidden/Nature/Tree Creator Leaves Fast Optimized";
        public static readonly string SHADER_UNITY_TREE_SOFT_OCCLUSION_BARK = "Nature/Tree Soft Occlusion Bark";
        public static readonly string SHADER_UNITY_TREE_SOFT_OCCLUSION_LEAVES = "Nature/Tree Soft Occlusion Leaves";

        public static readonly string SHADER_GPUI_SPEED_TREE = "QuadroRenderer/Nature/SPDTree";
        public static readonly string SHADER_GPUI_SPEED_TREE_8 = "QuadroRenderer/Nature/SPDTree8";
        public static readonly string SHADER_GPUI_TREE_CREATOR_BARK = "QuadroRenderer/Nature/Tree Creator Bark";
        public static readonly string SHADER_GPUI_TREE_CREATOR_BARK_OPTIMIZED = "QuadroRenderer/Nature/Tree Creator Bark Optimized";
        public static readonly string SHADER_GPUI_TREE_CREATOR_LEAVES = "QuadroRenderer/Nature/Tree Creator Leaves";
        public static readonly string SHADER_GPUI_TREE_CREATOR_LEAVES_OPTIMIZED = "QuadroRenderer/Nature/Tree Creator Leaves Optimized";
        public static readonly string SHADER_GPUI_TREE_CREATOR_LEAVES_FAST = "QuadroRenderer/Nature/Tree Creator Leaves Fast";
        public static readonly string SHADER_GPUI_TREE_CREATOR_LEAVES_FAST_OPTIMIZED = "QuadroRenderer/Nature/Tree Creator Leaves Fast Optimized";
        public static readonly string SHADER_GPUI_TREE_SOFT_OCCLUSION_BARK = "QuadroRenderer/Nature/Tree Soft Occlusion Bark";
        public static readonly string SHADER_GPUI_TREE_SOFT_OCCLUSION_LEAVES = "QuadroRenderer/Nature/Tree Soft Occlusion Leaves";
        public static readonly string SHADER_GPUI_BILLBOARD_2D_RENDERER_TREECREATOR = "QuadroRenderer/Billboard/2DRendererTreeCreator";

        public static readonly string SHADER_UNITY_INTERNAL_ERROR = "Hidden/InternalErrorShader";
        public static readonly string SHADER_GPUI_BILLBOARD_ALBEDO_BAKER = "Hidden/QuadroRenderer/Billboard/AlbedoBake";
        public static readonly string SHADER_GPUI_BILLBOARD_NORMAL_BAKER = "Hidden/QuadroRenderer/Billboard/NormalBake";

        public static readonly string SHADER_SHADOWS_ONLY = "Hidden/QuadroRenderer/ShadowsOnly"; 

        #endregion Shaders

        #region CS Billboard
        public static readonly string COMPUTE_BILLBOARD_RESOURCE_PATH = "CSBillboard";
        public static readonly string COMPUTE_BILLBOARD_DILATION_KERNEL = "CSBillboardDilate";
        #endregion CS Billboard

        #region Platform Dependent
        public static float COMPUTE_SHADER_THREAD_COUNT_2D = 16;
        #endregion Platform Dependent

        #region Stride Sizes
        public static readonly int STRIDE_SIZE_MATRIX4X4 = 64;
        public static readonly int STRIDE_SIZE_BOOL = 4;
        public static readonly int STRIDE_SIZE_INT = 4;
        public static readonly int STRIDE_SIZE_FLOAT = 4;
        public static readonly int STRIDE_SIZE_FLOAT4 = 16;
        #endregion Stride Sizes

        private static string _defaultPath;
        public static string GetDefaultPath()
        {
            if (string.IsNullOrEmpty(_defaultPath))
            {
                _defaultPath = "Assets/Mega World/Quadro Renderer";
            }
            return _defaultPath;
        }

        public static string CombinePath(string path1, string path2)
        {
            return path1 + "/" + path2;
        }
    }
}