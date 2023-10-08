namespace Vmt2Vmat
{
    public class Vmat
    {
        public readonly static string ShaderDefault = "csgo_complex";
        public readonly static string TextureAmbientOcclusionDefault = "materials/default/default_ao.tga";
        public readonly static string TextureColorDefault = "materials/default/default_color.tga";
        public readonly static string TextureNormalDefault = "materials/default/default_normal.tga";
        public static string Format = """
// THIS FILE IS AUTO-GENERATED

Layer0
{
	shader "%SHADER%.vfx"

	//---- PBR ----
	F_SPECULAR 1

	//---- Ambient Occlusion ----
	g_flAmbientOcclusionDirectDiffuse "0.000"
	g_flAmbientOcclusionDirectSpecular "0.000"
	TextureAmbientOcclusion "%AOTEX%"

	//---- Color ----
	g_flModelTintAmount "1.000"
	g_vColorTint "[1.000000 1.000000 1.000000 0.000000]"
	TextureColor "%COLORTEX%"

	//---- Fade ----
	g_flFadeExponent "1.000"

	//---- Fog ----
	g_bFogEnabled "1"

	//---- Lighting ----
	g_flDirectionalLightmapMinZ "0.050"
	g_flDirectionalLightmapStrength "1.000"

	//---- Metalness ----
	g_flMetalness "0.000"

	//---- Normal ----
	TextureNormal "%NORMALTEX%"

	//---- Roughness ----
	TextureRoughness "[0.956863 0.345098 0.345098 0.000000]"

	//---- Texture Coordinates ----
	g_nScaleTexCoordUByModelScaleAxis "0"
	g_nScaleTexCoordVByModelScaleAxis "0"
	g_vTexCoordOffset "[0.000 0.000]"
	g_vTexCoordScale "[1.000 1.000]"
	g_vTexCoordScrollSpeed "[0.000 0.000]"
}
""";
        public string Shader { get; set; }
        public string TextureAmbientOcclusion { get; set; }
        public string TextureColor { get; set; }
        public string TextureNormal { get; set; }
        public override string ToString()
        {
            return Format
                .Replace("%SHADER%", Shader)
                .Replace("%AOTEX%", TextureAmbientOcclusion)
                .Replace("%COLORTEX%", TextureColor)
                .Replace("%NORMALTEX%", TextureNormal);
        }
    }
}