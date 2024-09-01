#ifndef COURIER_FUNCTIONS_INCLUDED
#define COURIER_FUNCTIONS_INCLUDED



half when_eq(half x, half y) {
    return 1.0 - abs(sign(x - y));
}

half when_neq(half x, half y) {
    return abs(sign(x - y));
}

half when_gt(half x, half y) {
    return max(sign(x - y), 0.0);
}

half when_lt(half x, half y) {
    return max(sign(y - x), 0.0);
}

half when_ge(half x, half y) {
    return 1.0 - when_lt(x, y);
}

half when_le(half x, half y) {
    return 1.0 - when_gt(x, y);
}

half4 when_eq(half4 x, half4 y) {
    return 1.0 - abs(sign(x - y));
}

half4 when_neq(half4 x, half4 y) {
    return abs(sign(x - y));
}

half4 when_gt(half4 x, half4 y) {
    return max(sign(x - y), 0.0);
}

half4 when_lt(half4 x, half4 y) {
    return max(sign(y - x), 0.0);
}

half4 when_ge(half4 x, half4 y) {
    return 1.0 - when_lt(x, y);
}

half4 when_le(half4 x, half4 y) {
    return 1.0 - when_gt(x, y);
}

half xIsCloser(half x, half y, half v)
{
    return when_gt(abs(y - v), abs(x - v));
}


half invlerp(half a, half b, half v)
{
    return (v - a) / (b - a);
}

half2 invlerp(half2 a, half2 b, half2 v)
{
    return (v - a) / (b - a);
}

half4 invlerp(half4 a, half4 b, half4 v)
{
    return (v - a)/ (b - a); 
}

half remap(half s1, half s2, half e1, half e2, half v)
{
    half val = invlerp(s1, s2, v);
    return lerp(e1, e2, val);
}
half2 remap(half2 s1, half2 s2, half2 e1, half2 e2, half2 v)
{
    half2 val = invlerp(s1, s2, v);
    return lerp(e1, e2, val);
}

half4 remap(half4 s1, half4 s2, half4 e1, half4 e2, half4 v)
{
    half4 val = invlerp(s1, s2, v);
    return lerp(e1, e2, val);
}


half changeResult(half valTrue, half valFalse, half dec)
{
    return valTrue * dec + valFalse * (1 - dec);
}

half3 NormalBlend(half3 a, half3 b)
{
    return normalize(half3(a.rg + b.rg, a.b * b.b));
}

half3 RGBtoHSV(half3 color)
{
    half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    half4 P = lerp(half4(color.bg, K.wz), half4(color.gb, K.xy), step(color.b, color.g));
    half4 Q = lerp(half4(P.xyw, color.r), half4(color.r, P.yzx), step(P.x, color.r));
    half D = Q.x - min(Q.w, Q.y);
    half E = 1e-10;
    return half3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
}

half3 HSVtoRGB(half3 color)
{
    half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    half3 P = abs(frac(color.xxx + K.xyz) * 6.0 - K.www);
    return color.z * lerp(K.xxx, saturate(P - K.xxx), color.y);
}

half3 Overlay(half3 base, half3 blend, half opacity)
{

    half3 r1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    half3 r2 = 2.0 * base * blend;
    half3 dec = when_gt(base, 0.5);
    half3 overlay = dec * r1 + (1 - dec) * r2;
    return lerp(base, overlay, opacity);
}

half4 HSVLerp(half4 c1, half4 c2, half p)
{
    c1.xyz = RGBtoHSV(c1.xyz);
    c2.xyz = RGBtoHSV(c2.xyz);
    
    half d = c2.x - c1.x; //hue

    half more = when_gt(c1.x, c2.x);

    // change these to compiler shortcut functions (i forgor name)
    /*
    half tmp = c2.x;
    c1.x = changeResult(c2.x, c1.x, more);
    c2.x = changeResult(tmp, c2.x, more);
    d = changeResult(-d, d, more);
    p = changeResult(1-p, p, more);
    */

    half dec1 = when_gt(d, 0.5);
    c1.x += dec1;
    half hue = changeResult((c1.x + p * (c2.x - c1.x)) % 1, c1.x + p * d, dec1);

    half sat = lerp(c1.y, c2.y, p);
    half val = lerp(c1.z, c2.z, p);
    half alpha = lerp(c1.w, c2.w, p);

    half3 rgb = HSVtoRGB(half3(hue, sat, val));
    return half4(rgb.xyz, alpha);
}

half GetLuminance(float3 rgb)
{
    return max(rgb.r, max(rgb.g, rgb.b));
}

half GetAccurateLuminance(float3 rgb)
{
    return rgb.r * 0.3 + rgb.g * 0.59 + rgb.b * 0.11;
}

// using alexander ameye's version
float3 GerstnerWave(float3 position, float steepness, float wavelength, float speed, float direction, inout float3 tangent, inout float3 binormal)
{
    direction = direction * 2 - 1;
    float2 d = normalize(float2(cos(3.14 * direction), sin(3.14 * direction)));
    float k = 2 * 3.14 / wavelength;                                           
    float f = k * (dot(d, position.xz) - speed * _Time.y);
    float a = steepness / k;

    tangent += float3(
    -d.x * d.x * (steepness * sin(f)),
    d.x * (steepness * cos(f)),
    -d.x * d.y * (steepness * sin(f))
    );

    binormal += float3(
    -d.x * d.y * (steepness * sin(f)),
    d.y * (steepness * cos(f)),
    -d.y * d.y * (steepness * sin(f))
    );

    return float3(
    d.x * (a * cos(f)),
    a * sin(f),
    d.y * (a * cos(f))
    );
}

void GerstnerWaves_float(float3 position, float steepness, float wavelength, float speed, float4 directions, out float3 Offset, out float3 normal)
{
    Offset = 0;
    float3 tangent = float3(1, 0, 0);
    float3 binormal = float3(0, 0, 1);

    Offset += GerstnerWave(position, steepness, wavelength, speed, directions.x, tangent, binormal);
    Offset += GerstnerWave(position, steepness, wavelength, speed, directions.y, tangent, binormal);
    Offset += GerstnerWave(position, steepness, wavelength, speed, directions.z, tangent, binormal);
    Offset += GerstnerWave(position, steepness, wavelength, speed, directions.w, tangent, binormal);

    normal = normalize(cross(binormal, tangent));
    //TBN = transpose(float3x3(tangent, binormal, normal));
}

half2 RotateByAngle(half2 coord, half angle)
{
    return half2(coord.x * cos(angle) + coord.y * sin(angle),
                 -coord.x * sin(angle) + coord.y * cos(angle));
}

float4 NDCToClip(float4 scpos, float2 clipZW)
{
    float4 o = scpos;
    o.xy *= o.w;
    o.zw = clipZW.xy / 2;
    o.xy -= o.w;
    o.y /= _ProjectionParams.x;
    o *= 2;
    return o;
}

#endif