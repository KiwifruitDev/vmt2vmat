namespace Vmt2Vmat
{
    public class Shaders
    {
        public readonly static int ShaderCountValid = 3;
        public readonly static List<string> ShadersList = new List<string>()
        {
            // No warning
            "LightmappedGeneric",
            "VertexLitGeneric",
            "UnlitGeneric",
            // Warning
            "LightmappedReflective",
            "LightmappedTwoTexture",
            "Lightmapped_4WayBlend",
            "MultiBlend",
            "WorldTwoTextureBlend",
            "WorldVertexTransition",
            "WindowImposter",
            "Water",
            "UnlitTwoTexture",
            "WorldGGX",
            "ParallaxTest",
            "PaintBlob",
            "Eyes",
            "EyeRefract",
            "VortWarp",
            "Aftershock",
            "Teeth",
            "SurfaceGGX",
            "Character",
            "SolidEnergy",
            "VolumeCloud",
            "Sky",
            "Core",
            "SpriteCard",
            "Cable",
            "SplineRope",
            "UnlitGeneric",
            "Refract",
            "MonitorScreen",
            "Modulate",
            "WindowImposter",
            "Screenspace_general",
            "Pyro_vision",
            "DepthOfField",
            "Engine_Post",
            "LensFX",
            "Suppression",
            "MotionBlur",
            "RGBFilmGrain",
            "DecalModulate",
            "Shadow",
            "Subrect",
        };
    }
}