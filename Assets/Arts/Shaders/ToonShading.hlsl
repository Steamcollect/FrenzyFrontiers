void ToonShading_float(in float3 normal, in float toonRampSmoothness, in float3 clipSpacePos, in float3 worldSpacePos,
                       in float4 toonRampTinting, in float toonRampOffset, out float3 toonRampOutput, out float3 directionOutput)
{
    #ifdef SHADERGRAPH_PREVIEW
        toonRampOutput = float3(0.5,0.5,0);
        directionOutput = float3(0.5,0.5,0);
    #else
        #if SHADOWS_SCREEN
            half4 shadowCoord = ComputeScreenPos(clipSpacePos);
        #else
            half4 shadowCoord = TransformWorldToShadowCoord(worldSpacePos);
        #endif

        #if _MAIN_LIGHT_SHADOWS_CASCADE ||_MAIN_LIGHT_SHADOWS
            Light light = GetMainLight(shadowCoord);
        #else
            Light light = GetMainLight();
        #endif
    
        half d =dot(normal, light.direction) * 0.5 + 0.5;

        half toonRamp = smoothstep(toonRampOffset, toonRampOffset + toonRampSmoothness,d);

        toonRamp *= light.shadowAttenuation;

        toonRampOutput = light.color *(toonRamp + toonRampTinting);

        directionOutput = light.direction;
    #endif
}