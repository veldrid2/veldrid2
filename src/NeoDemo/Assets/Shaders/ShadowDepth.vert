#version 450

struct Veldrid_NeoDemo_Objects_WorldAndInverse
{
    mat4 World;
    mat4 InverseWorld;
};

layout(set = 0, binding = 0) uniform ViewProjection
{
    mat4 _ViewProjection;
};

layout(set = 1, binding = 0) uniform WorldAndInverse
{
    Veldrid_NeoDemo_Objects_WorldAndInverse _WorldAndInverse;
};

layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec2 TexCoord;

void main()
{
    gl_Position = _ViewProjection * _WorldAndInverse.World * vec4(Position, 1);
    gl_Position.y += TexCoord.y * .0001f;
}
